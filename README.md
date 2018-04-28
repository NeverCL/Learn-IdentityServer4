# Geek.IdentityServer4

## Layer & Package

dotnet new web -o Geek.IdentityServer4.ApiDemo
dotnet new console -o Geek.IdentityServer4.ClientDemo
dotnet new web -o Geek.IdentityServer4.MvcClientDemo
dotnet new web -o Geek.IdentityServer4.IdentityServer4Demo
dotnet new mvc --auth Individual -o Geek.IdentityServer4.IdentityServer4

IdentityServer4
IdentityServer4.AccessTokenValidation
IdentityModel
IdentityServer4.AspNetIdentity

## Basic

debug:

`cd ./Geek.IdentityServer4 && dotnet restore && dotnet watch run`

`cd ./Geek.Api && dotnet restore && dotnet watch run`

`cd ./Geek.Client && dotnet restore && dotnet watch run`

`cd ./Geek.MvcClient && dotnet restore && dotnet watch run`

### Protecting an API using Client Credentials

In this scenario we will define an API and a client that wants to access it. The client will request an access token at IdentityServer and use it to gain access to the API.

dotnet new web -o Geek.Api && cd ./Geek.Api && dotnet add package IdentityServer4.AccessTokenValidation

dotnet new console -o Geek.Client && cd ./Geek.Client && dotnet add package IdentityModel

[discovery document](http://localhost:5000/.well-known/openid-configuration)

### Protecting an API using Passwords

The OAuth 2.0 resource owner password grant allows a client to send username and password to the token service and get an access token back that represents that user.

The presence (or absence) of the sub claim lets the API distinguish between calls on behalf of clients and calls on behalf of users.

### Adding User Authentication with OpenID Connect

dotnet new web -o Geek.MvcClient && cd Geek.MvcClient && dotnet new page -n Index -o Pages -na Geek.MvcClient.Pages

All the protocol support needed for OpenID Connect is already built into IdentityServer. You need to provide the necessary UI parts for login, logout, consent and error.

`CallbackPath` = `RedirectUris`

`iex ((New-Object System.Net.WebClient).DownloadString('https://raw.githubusercontent.com/IdentityServer/IdentityServer4.Quickstart.UI/release/get.ps1'))`

id_token固定为Jwt，所以无法revoke 或 实时更新
AddOpenIdConnect默认的scope为openid,profile

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
As well, we’ve turned off the JWT claim type mapping to allow well-known claims (e.g. ‘sub’ and ‘idp’) to flow through unmolested:

ResponseType
    idtoken

Claims
    Profile:name + website

Remember My Decision
    Store In Memory

User.Identity.Name or Other Claims
    [Identity Resource](http://openid.net/specs/openid-connect-basic-1_0-28.html#scopes)
    在Implicit模式下，不可以获取oidc以外的claim

Logout?
PostLogoutRedirectUris?

### Switching to Hybrid Flow and adding API Access back

In the previous quickstarts we explored both API access and user authentication. Now we want to bring the two parts together.

Access tokens are a bit more sensitive than identity tokens

OpenID Connect includes a flow called “Hybrid Flow” which gives us the best of both worlds：
    the identity token is transmitted via the browser channel
    the client opens a back-channel to the token service to retrieve the access token

HybridAndClientCredentials => ResponseType = "code id_token"

Hybrid flow is a combination of the implicit and authorization code flow - it uses combinations of multiple grant types, most typically code id_token
    Hybrid = Implicit + Authorize Code

## Using ASP.NET Core Identity

dotnet add package Pomelo.EntityFrameworkCore.MySql
dotnet add package IdentityServer4.AspNetIdentity

## Endpoint

[userinfo](http://localhost.:5000/connect/userinfo)
    通过idtoken获取IdentityResource

[token](http://localhost.:5000/connect/token)
    根据不同类型(authorization_code etc)获取id_token、access_token

[endsession](http://localhost.:5000/connect/endsession)
    参数：post_logout_redirect_uri+state申请在oidc上退出登录

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
    关闭jwt到默认scheme的映射
    如：`role -> http://schemas.microsoft.com/ws/2008/06/identity/claims/role`

opt.GetClaimsFromUserInfoEndpoint = true; // 请求userinfo获取IdentityResource
    从UserInfoEndpoint再获取一次Claims，虽然能获取，但是在Client上并不会自动重新赋值

AlwaysIncludeUserClaimsInIdToken = true,  // 默认为false,
    在返回id_token的时候，将其他非oidc标准的 且 用户许可的Claims 一起返回给Client

## JWT

### playload

载荷就是存放有效信息的地方。这个名字像是特指飞机上承载的货品，这些有效信息包含三个部分

- 标准中注册的声明
- 公共的声明
- 私有的声明

标准中注册的声明 (建议但不强制使用) ：

1. iss: jwt签发者
1. sub: jwt所面向的用户
1. aud: 接收jwt的一方
1. exp: jwt的过期时间，这个过期时间必须要大于签发时间
1. nbf: 定义在什么时间之前，该jwt都是不可用的.
1. iat: jwt的签发时间
1. jti: jwt的唯一身份标识，主要用来作为一次性token,从而回避重放攻击。

### 示例：

Client:

```json
[
  {
    "type": "nbf",
    "value": "1524710268"
  },
  {
    "type": "exp",
    "value": "1524713868"
  },
  {
    "type": "iss",
    "value": "https://sso.neverc.cn"
  },
  {
    "type": "aud",
    "value": "https://sso.neverc.cn/resources"
  },
  {
    "type": "aud",
    "value": "api1"
  },
  {
    "type": "client_id",
    "value": "client"
  },
  {
    "type": "client_name",
    "value": "名称"
  },
  {
    "type": "scope",
    "value": "api1"
  }
]
```

RO.Client:

```json
[
  {
    "type": "nbf",
    "value": "1524710168"
  },
  {
    "type": "exp",
    "value": "1524713768"
  },
  {
    "type": "iss",
    "value": "https://sso.neverc.cn"
  },
  {
    "type": "aud",
    "value": "https://sso.neverc.cn/resources"
  },
  {
    "type": "aud",
    "value": "api1"
  },
  {
    "type": "client_id",
    "value": "ro.client"
  },
  {
    "type": "sub",
    "value": "1"
  },
  {
    "type": "auth_time",
    "value": "1524710168"
  },
  {
    "type": "idp",
    "value": "local"
  },
  {
    "type": "scope",
    "value": "api1"
  },
  {
    "type": "amr",
    "value": "pwd"
  }
]
```