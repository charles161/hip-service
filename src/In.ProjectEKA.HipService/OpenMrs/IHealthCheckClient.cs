using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
namespace In.ProjectEKA.HipService.OpenMrs {
    public interface IHealthCheckClient {
        Task<Dictionary<string, string>> CheckHealth ();
    }
}