using System.Runtime.InteropServices;

namespace redis_com_client
{
    [ComVisible(true)]
    [Guid("c8109c73-2528-4e90-a999-81abd1fc7a70")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ICacheManager
    {
        void Add(string key, object value);
        object Get(string key);
        void RemoveAll();
        object this[string key] { get; set; }
        void Init(string cacheId);
		void Remove(string key);
        bool Exists(string key);
        void SetExpiration(string key, int milliseconds);
    }
}