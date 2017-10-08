# Cache Utilities for Xamarin.Forms

## Install
Install into your Shared and OS specific projects

Nuget : https://www.nuget.org/packages/Ib.Xamarin.CacheUtils/

## Initialize
In your OS specific startup file (MainAcivity.cs for Android and/or AppDelegate.cs for iOS) add
```csharp
CacheUtils.Init(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal));
```

## Globals
#### Cache Hold Time (TimeSpan)
The amount of time to hold the results before getting new data. **Default**: *10 minutes*
```csharp
CacheUtils.CACHE_HOLD_TIME = new TimeSpan(0,10,0);
```
#### Max Response Content Buffer Size (long)
The maximum size for the response buffer. **Default**: *256000*
```csharp
CacheUtils.MAX_RESPONSE_CONTENT_BUFFER_SIZE = 256000;
```
#### HTTP Client Request Timeout (TimeSpan)
The amount of time to hold the results before getting new data. **Default**: *1 minute 30 seconds*
```csharp
CacheUtils.TIMEOUT = new TimeSpan(0,1,30);
```

## Override Globals per DTO
Each global variable can be overwritten by using ***Attributes*** per DTO.
#### Cache Hold Time (int, int, int)
```csharp
[CacheDtoHoldTime(<hours>,<minutes>,<seconds>)]
```
#### HTTP Client Max Buffer Size (long)
```csharp
[CacheUtilsHttpClient(MaxBufferSize = <buffer size>)]
```
#### HTTP Client Request Timeout (long)
```csharp
[CacheUtilsHttpClient(Timeout = <seconds>)]
```
**Example Usage**
```csharp
[CacheDtoHoldTime(0,1,0)]
[CacheUtilsHttpClient(MaxBufferSize = 1024000, Timeout = 300)]
public class UserDTO
{
    public UserDTO() { }

    ...   
}
```

## HttpRest Caching (json)
In your Shared project initialize the CacheRestManager
with Basic Auth
```csharp
var restManager = new CacheRestManager(new CacheRestService(username, password)); 
```
or with no auth
```csharp
var restManager = new CacheRestManager(new CacheRestService()); 
```

To Use
Create a DTO to receive your REST request
```csharp
public class UserDTO
{
    public UserDTO() { }

    [JsonProperty(PropertyName = "username")]
    public string Username { get; set; }

    [JsonProperty(PropertyName = "user_password")]
    public string Password { get; set; }
}
```
Set the *PropertyName* to match your json.

**Sample json**
```json
{
   "username" : "John",
   "user_password" : "Superman"
}
```

**Example Usage**
```csharp
var result = await restManager.GetRestDataAsync<UserDTO>(url, forceRefresh);
```
***Where***  
**url** (string) is the REST endpoint  
**forceRefresh** (bool) true to get latest data, false to read from cache and update if CACHE_HOLD_TIME has expired.  

or, supply a Tag to know which cache has been updated
```csharp
var result = await restManager.GetRestDataAsync<UserDTO>(url, eventListenerTag);
```
***Where***  
**url** (string) is the REST endpoint  
**eventListenerTag** (string) A string to Tag for cache event.  

### Cache Updated - Event Handler
When using *GetRestDataAsync()* you will get the last object from the cache (if exists), but if the **CACHE_HOLD_TIME** has expired then *GetRestDataAsync()* will get a new update for the data and insert it into the cache ready for the next call to *GetRestDataAsync()*.

Sometimes you might want to get the lastest as soon as the cache is updated, to do this you need to subcribe to the *CacheUpdated* event.

**Example**
```csharp
...
restManager.CacheUpdated += MyCacheUpdated;
...

// if using restManager.GetRestDataAsync<UserDTO>(url, false)
// Event will only get called if forceRefresh = false
private void MyCacheUpdated(object sender, CacheEventArgs e)
{
    if (e.Results.GetType() == typeof(UserDTO))
    {
        Users = (UserDTO)e.Results;
    }
}

// or if restManager.GetRestDataAsync<UserDTO>(url, "login")
private void MyCacheUpdated(object sender, CacheEventArgs e)
{
    if (e.EventListenerTag == "login")
    {
        Users = (UserDTO)e.Results;
    }
}

...
restManager.CacheUpdated -= MyCacheUpdated;
...
```

## Object Caching
Create a POCO and extend **ObjectStorage**
```csharp
public class UserLogin : ObjectStorage
{
    public string Username { get; set; }
    public string Password { get; set; }
}
```

To Save Object
```csharp
await new UserLogin()
    {
        Username = username,
        Password = password
    }.SaveAsync();
```

To Get from Cache
```csharp
var user = CacheHelper.GetCacheObject<UserLogin>();
```

To Remove from Cache
```csharp
CacheHelper.RemoveCacheObject<UserLogin>();
```

#### Many Thanks
Akavache => https://github.com/akavache/Akavache  
ModernHttpClient => https://github.com/paulcbetts/ModernHttpClient  
Newtonsoft Json => https://www.newtonsoft.com/json    
ConnectivityPlugin => https://github.com/jamesmontemagno/ConnectivityPlugin
         