# redis-com-client
Redis Client for COM+ | StackExchange.Redis Wrapper

This was made to be used on Classic ASP (ASP 3.0).

Line command to install the COM+: 

%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\regasm redis-com-client.dll /tlb:redis-com-client.tlb /codebase

On the ASP side you have not create the object. I initialized this in the global.asa using this:
- <OBJECT RUNAT=Server SCOPE=Application ID=Cache PROGID=CacheManager></OBJECT>

However, it also works if you want to create a scoped object:
-  Set Cache = Server.CreateObject("CacheManager")


Later you can use these operations:

- Initiliaze (this operation is required in order to share the same Redis instance with N sites)
  Cache.Init "prefix1"

-**Add**

  Cache.Add "key1", "value"
 
 **or**

  Cache("key1") = "value"

-**Add with expiration**

  Cache.SetExpiration "key1", "value", 1000 'ms
  
-**Get**

  Cache.Get "key1"

**or**

  Cache("key1")
  
-**Remove**

  Cache.Remove "key1"
  
-**Remove All**

  Cache.RemoveAll()
