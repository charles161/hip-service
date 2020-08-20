namespace In.ProjectEKA.HipService
{
    public interface ICache
    {
        void add(string key, object value);
        void remove(string key);
        object get(string key);
        void clear();
    }
}