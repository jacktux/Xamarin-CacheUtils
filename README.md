# Cache Utilities for Xamarin.Forms

## Install
Install into your Shared and OS specific projects

Nuget : https://www.nuget.org/packages/Ib.Xamarin.CacheUtils/

## Initialize
In your OS specific startup file (MainAcivity.cs for Android and/or AppDelegate.cs for iOS) add
```csharp
CacheUtils.Init(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal));
```

## HttpRest Caching
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
```csharp
var result = await restManager.GetRestDataAsync<T>(url);
```
Where T is the object type you expect to receive form the call to the url


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
