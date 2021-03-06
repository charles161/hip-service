namespace In.ProjectEKA.HipService.DataFlow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Gateway;
    using HipLibrary.Patient.Model;
    using Logger;
    using Model;
    using static Common.HttpRequestHelper;

    public class DataFlowClient
    {
        private readonly HttpClient httpClient;
        private readonly GatewayClient gatewayClient;
        private readonly DataFlowNotificationClient dataFlowNotificationClient;
        private readonly GatewayConfiguration gatewayConfiguration;

        public DataFlowClient(HttpClient httpClient,
            GatewayClient gatewayClient,
            DataFlowNotificationClient dataFlowNotificationClient,
            GatewayConfiguration gatewayConfiguration)
        {
            this.httpClient = httpClient;
            this.gatewayClient = gatewayClient;
            this.dataFlowNotificationClient = dataFlowNotificationClient;
            this.gatewayConfiguration = gatewayConfiguration;
        }

        public virtual async Task SendDataToHiu(HipLibrary.Patient.Model.DataRequest dataRequest,
            IEnumerable<Entry> data,
            KeyMaterial keyMaterial)
        {
            await PostTo(dataRequest.ConsentId,
                dataRequest.DataPushUrl,
                dataRequest.CareContexts,
                new DataResponse(dataRequest.TransactionId, data, keyMaterial),
                dataRequest.CmSuffix).ConfigureAwait(false);
        }

        private async Task PostTo(string consentId,
            string dataPushUrl,
            IEnumerable<GrantedContext> careContexts,
            DataResponse dataResponse,
            string cmSuffix)
        {
            var grantedContexts = careContexts as GrantedContext[] ?? careContexts.ToArray();
            try
            {
                var token = await gatewayClient.Authenticate();
                token.MatchSome(async accessToken =>
                {
                    try
                    {
                        await httpClient.SendAsync(CreateHttpRequest(dataPushUrl, dataResponse, accessToken))
                            .ConfigureAwait(false);
                    }
                    catch (Exception exception)
                    {
                        Log.Error(exception, exception.StackTrace);
                        await GetDataNotificationRequest(consentId,
                            grantedContexts,
                            dataResponse,
                            HiStatus.ERRORED,
                            SessionStatus.FAILED,
                            "Failed to deliver health information",
                            cmSuffix).ConfigureAwait(false);
                    }
                });
                token.MatchNone(() => Log.Error("Did not post data to HIU"));
                await GetDataNotificationRequest(consentId,
                    grantedContexts,
                    dataResponse,
                    HiStatus.DELIVERED,
                    SessionStatus.TRANSFERRED,
                    "Successfully delivered health information",
                    cmSuffix).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.StackTrace);
            }
        }

        private async Task GetDataNotificationRequest(string consentId,
            IEnumerable<GrantedContext> careContexts,
            DataResponse dataResponse,
            HiStatus hiStatus,
            SessionStatus sessionStatus,
            string description,
            string cmSuffix)
        {
            var statusResponses = careContexts
                .Select(grantedContext =>
                    new StatusResponse(grantedContext.CareContextReference, hiStatus, description))
                .ToList();

            await dataFlowNotificationClient.NotifyGateway(cmSuffix,
                new DataNotificationRequest(dataResponse.TransactionId,
                    DateTime.Now.ToUniversalTime(),
                    new Notifier(Type.HIP, gatewayConfiguration.ClientId),
                    new StatusNotification(sessionStatus, gatewayConfiguration.ClientId, statusResponses),
                    consentId,
                    Guid.NewGuid()));
        }
    }
}