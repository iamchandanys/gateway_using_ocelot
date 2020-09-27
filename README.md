# Gateway using Ocelot (.net core 3.1)

#### Requirement:
1. dot net core 3.1

#### Packages/nuget to be installed:
1. Ocelot
2. Microsoft.AspNetCore.Authentication.JwtBearer
3. System.IdentityModel.Tokens.Jwt

#### Steps:

##### Step 1 (ocelot.json):

1. Add ocelot.json file in root folder.
2. Add the below code in ocelot.json,
```
{
  "ReRoutes": [
    {
      "DownstreamPathTemplate": "/{everything}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "localhost",
          "Port": 5000
        }
      ],
      "UpstreamPathTemplate": "/identity/{everything}"
    },
    {
      "DownstreamPathTemplate": "/{everything}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "localhost",
          "Port": 5001
        }
      ],
      "UpstreamPathTemplate": "/order/{everything}"
    }
  ],
  "GlobalConfiguration": {
    "BaseUrl": "http://localhost:4200"
  }
}
```
###### DownstreamScheme : http/https
###### DownstreamHostAndPorts-Host : API Host Name
###### DownstreamHostAndPorts-Port : API Port Number
###### UpstreamPathTemplate : Unique name for each API's

##### Note: Make sure that this ocelot.json file has a Copy to Output Directory of: Copy if newer

##### Step 2 (appsettings.json):

2. Add the below code in appsettings.json,
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AppSettings": {
    "Secret": "YourSecretKey",
    "WhitelistedUrls": [
      "/identity/api/UserAccount/GetAllUserData"
    ]
  },
  "AllowedHosts": "*"
}

```
###### Secret : The secret key which is used to generate JWT Token.
###### WhitelistedUrls : The URL's which needs to be whitelisted.

##### Note: AppSettings dependency registery needs to be done in Startup.cs file.

##### Step 3 (Program.cs):

1. Add the below line in public static IWebHostBuilder CreateWebHostBuilder(string[] args) method,
```
.ConfigureAppConfiguration((host, config) => {
    config.AddJsonFile("ocelot.json");
})
```

##### Step 4 (Startup.cs):

1. Add the below code in Configure(IApplicationBuilder app, IWebHostEnvironment env) method,
```
app.Use(async (ctx, next) =>
{
    if (appSettings.AllowedHosts.Any(x => ctx.Request.Headers["Origin"].ToString().Equals(x, StringComparison.InvariantCultureIgnoreCase)))
    {
        if (ctx.Request.IsHttps)
        {
            if (ctx.User.Identity.IsAuthenticated || appSettings.WhitelistedUrls.Any(s => ctx.Request.Path.Value.Equals(s, StringComparison.InvariantCultureIgnoreCase)))
            {
                await next.Invoke();
            }
            else
            {
                ctx.Response.StatusCode = 401;
                ctx.Response.Headers.Clear();
                ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            }
        }
        else
        {
            ctx.Response.StatusCode = 401;
            ctx.Response.Headers.Clear();
            ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
    }
    else
    {
        ctx.Response.StatusCode = 401;
        ctx.Response.Headers.Clear();
        ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");
    }
});
```
