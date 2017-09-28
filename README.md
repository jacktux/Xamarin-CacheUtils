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
The maximum size for the response buffer. **Default**: *1024000*
```csharp
CacheUtils.MAX_RESPONSE_CONTENT_BUFFER_SIZE = 1024000;
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
var result = await restManager.GetRestDataAsync<UserDTO>(url);
```
or, if many UserDTO are returned
```csharp
var result = await restManager.GetRestDataAsync<List<UserDTO>>(url);
```

### Cache Updated - Event Handler
When using *GetRestDataAsync()* you will get the last object from the cache (if exists), but if the **CACHE_HOLD_TIME** has expired then *GetRestDataAsync()* will get a new update for the data and insert it into the cache ready for the next call to *GetRestDataAsync()*.

Sometimes you might want to get the lastest as soon as the cache is updated, to do this you need to subcribe to the *CacheUpdated* event.

**Example**
```csharp
...
restManager.CacheUpdated += MyCacheUpdated;
...

private void MyCacheUpdated(object sender, CacheEventArgs e)
{
    if (e.Results.GetType() == typeof(List<UserDTO>))
    {
        Users = (List<UserDTO>)e.Results;
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
