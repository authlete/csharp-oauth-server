Authorization Server Implementation in C#
=========================================

Overview
--------

This is an authorization server implementation in C# which supports
[OAuth 2.0][1] and [OpenID Connect][2].

This implementation is written using ASP.NET Core API and
[authlete-csharp][3] library which is provided as a NuGet package
[Authlete.Authlete][4].

This implementation is _DB-less_. What this means is that you don't
have to manage a database server that stores authorization data (e.g.
access tokens), settings of the authorization server itself and
settings of client applications. This is achieved by using
[Authlete][5] as a backend service.

Please read
*[New Architecture of OAuth 2.0 and OpenID Connect Implementation][6]*
for details about the architecture of Authlete.
True engineers will love the architecture ;-)

> The primary advantage of this architecture is in that the
> backend service can focus on implementing OAuth 2.0 and OpenID
> Connect without caring about other components such as identity
> management, user authentication, login session management, API
> management and fraud detection. And, consequently, it leads to
> another major advantage which enables the backend service
> (implementation of OAuth 2.0 and OpenID Connect) to be combined
> with any solution of other components and thus gives flexibility
> to frontend server implementations.


License
-------

  Apache License, Version 2.0


Source Code
-----------

  <code>https://github.com/authlete/csharp-oauth-server</code>


How To Run
----------

1. Download the source code of this authorization server implementation.

        $ git clone https://github.com/authlete/csharp-oauth-server.git
        $ cd csharp-oauth-server/AuthorizationServer

2. Edit the configuration file to set the API credentials of yours.

        $ vi authlete.properties

3. Start the authorization server on [http://localhost:5000][18].

        $ dotnet run


#### Configuration File

`csharp-oauth-server` refers to `authlete.properties` as a configuration
file. If you want to use another different file, specify the name of the
file by the environment variable `AUTHLETE_CONFIGURATION_FILE` like the
following.

    $ export AUTHLETE_CONFIGURATION_FILE=local.authlete.properties


Endpoints
---------

This implementation exposes endpoints as listed in the table below.

| Endpoint                  | Path                                |
|:--------------------------|:------------------------------------|
| Authorization Endpoint    | `/api/authorization`                |
| Token Endpoint            | `/api/token`                        |
| JWK Set document Endpoint | `/api/jwks`                         |
| Configuration Endpoint    | `/.well-known/openid-configuration` |
| Revocation Endpoint       | `/api/revocation`                   |
| Introspection Endpoint    | `/api/introspection`                |

The authorization endpoint and the token endpoint accept parameters
described in [RFC 6749][1], [OpenID Connect Core 1.0][7],
[OAuth 2.0 Multiple Response Type Encoding Practices][8],
[RFC 7636][9] ([PKCE][10]) and other specifications.

The JWK Set document endpoint exposes a JSON Web Key Set document
(JWK Set) so that client applications can (1) verify signatures by
this OpenID Provider and (2) encrypt their requests to this OpenID
Provider.

The configuration endpoint exposes the configuration information of
this OpenID Provider in the JSON format defined in
[OpenID Connect Discovery 1.0][11].

The revocation endpoint is a Web API to revoke access tokens and
refresh tokens. Its behavior is defined in [RFC 7009][12].

The introspection endpoint is a Web API to get information about access
tokens and refresh tokens. Its behavior is defined in [RFC 7662][13].


Authorization Request Example
-----------------------------

The following is an example to get an access token from the authorization
endpoint using [Implicit Flow][14]. Don't forget to replace `{client-id}`
in the URL with the real client ID of one of your client applications.
As for client applications, see [Getting Started][15] and the
[document][16] of _Developer Console_.

    http://localhost:5000/api/authorization?client_id={client-id}&response_type=token

The request above will show you an authorization page. The page asks
you to input login credentials and click "Authorize" button or "Deny"
button. Use one of the following as login credentials.

| Login ID | Password |
|:--------:|:--------:|
|   john   |   john   |
|   jane   |   jane   |

Of course, these login credentials are dummy data, so you need to replace
the user database implementation with your own.


Customization
-------------

How to customize this implementation is described in [CUSTOMIZATION.md][17].
Basically, you need to do programming for _end-user authentication_ because
Authlete does not manage end-user accounts. This is by design. The
architecture of Authlete carefully seperates authorization from authentication
so that you can add OAuth 2.0 and OpenID Connect functionalities seamlessly
into even an existing web service which may already have a mechanism for
end-user authentication.


See Also
--------

- [Authlete][5] - Authlete Home Page
- [authlete-csharp][3] - Authlete Library for C#
- [csharp-resource-server][20] - Resource Server Implementation


Contact
-------

| Purpose   | Email Address        |
|:----------|:---------------------|
| General   | info@authlete.com    |
| Sales     | sales@authlete.com   |
| PR        | pr@authlete.com      |
| Technical | support@authlete.com |


[1]: https://tools.ietf.org/html/rfc6749
[2]: https://openid.net/connect/
[3]: https://github.com/authlete/authlete-csharp
[4]: https://www.nuget.org/packages/Authlete.Authlete
[5]: https://www.authlete.com/
[6]: https://medium.com/@darutk/new-architecture-of-oauth-2-0-and-openid-connect-implementation-18f408f9338d
[7]: https://openid.net/specs/openid-connect-core-1_0.html
[8]: https://openid.net/specs/oauth-v2-multiple-response-types-1_0.html
[9]: https://tools.ietf.org/html/rfc7636
[10]: https://www.authlete.com/documents/article/pkce
[11]: https://openid.net/specs/openid-connect-discovery-1_0.html
[12]: https://tools.ietf.org/html/rfc7009
[13]: https://tools.ietf.org/html/rfc7662
[14]: https://tools.ietf.org/html/rfc6749#section-4.2
[15]: https://www.authlete.com/documents/getting_started
[16]: https://www.authlete.com/documents/cd_console
[17]: CUSTOMIZATION.md
[18]: http://localhost:5000/
[20]: https://github.com/authlete/csharp-resource-server
