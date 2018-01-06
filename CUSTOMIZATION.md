Customization
=============

This document describes how to customize this authorization server
implementation.


Overview
--------

This authorization server implementation uses [Authlete][1] as its backend.
What this means are (1) that the core part of the implementation of [OAuth
2.0][2] and [OpenID Connect][3] is not in the source tree of csharp-oauth-server
but in the Authlete server on cloud, and (2) that authorization data such as
access tokens, settings of the authorization server itself and settings of
client applications are not stored in any local database but in the database
on cloud. Therefore, to put it very simply, this implementation is just an
intermediary between client applications and Authlete server as illustrated
below.

```
+--------+          +---------------------+          +----------+
|        |          |                     |          |          |
| Client | <------> | csharp-oauth-server | <------> | Authlete |
|        |          |                     |          |          |
+--------+          +---------------------+          +----------+
```

However, because Authlete focuses on **authorization** and does NOT do
anything about end-user **authentication**, functions related to
authentication are implemented in the source tree of csharp-oauth-server.

Therefore, at least, you must customize parts related to end-user authentication.
On the other hand, customization of other parts such as UI design of the
authorization page is optional.


Overall Structure
-----------------

Authlete provides [Web APIs][4] that can be used to write an authorization
server. [authlete-csharp][5] is a library which contains `IAuthleteApi`
interface and its implementation to communicates with the Authlete Web APIs
directly. It also contains utility classes which wraps `IAuthleteApi` in
order to make it much easier for developers to implement an authorization
server than calling Authlete APIs directly.

[authlete-csharp][5] is available as a NuGet package, [Authlete.Authlete][6].


Authorization Endpoint
----------------------

The implementation of the authorization endpoint is in
<code>[AuthorizationController.cs][7]</code>.

First, `AuthorizationController` calls Authlete's <c>/api/auth/authorization</c>
API. Next, `AuthorizationController` checks the value of the `action` parameter
in the response from the API and decides the next action to take.

When `action` is `INTERACTION`, `AuthorizationController` returns an
authorization page (HTML) to the client application. What has to be done in
the case of `action=INTERACTION` is described in the description of
`AuthorizationResponse` in the [API reference][8] of authlete-csharp.

When `action` is `NO_INTERACTION`, `AuthorizationController` calls `Handle()`
method of `NoInteractionHandler`. The important point is that the constructor
of `NoInteractionHandler` requires an implementation of `INoInteractionHandlerSpi`
interface. The interface is the Service Provider Interface (SPI) that you need
to implement. csharp-oauth-server includes a simple example
(`NoInteractionHandlerSpiImpl`).

When `action` is neither `INTERACTION` nor `NO_INTERACTION`,
`AuthorizationController` delegates the remaining tasks to
`AuthorizationRequestErrorHandler`. The handler does not require any SPI.


Authorization Page
------------------

`AuthorizationController` manually renders an authorization page and returns
it to the client application. `Views/Authorization/Page.cshtml` is used as
the view and `Models/AuthorizationPageModel.cs` is used as the model class.

The implementation of the authorization page in csharp-oauth-server is
simple, but there exist many other things you may want to change. Following
are some consideration points.


#### Internationalization

For the internationalization of the authorization page, you may take
`ui_locales` parameter into consideration which may be contained in an
authorization request. It is a new request parameter defined in [OpenID
Connect Core 1.0][9]. The following is the description about the parameter
excerpted from the specification.

> OPTIONAL. End-User's preferred languages and scripts for the user interface,
> represented as a space-separated list of BCP47 [RFC5646] language tag values,
> ordered by preference. For instance, the value "fr-CA fr en" represents a
> preference for French as spoken in Canada, then French (without a region
> designation), followed by English (without a region designation). An error
> SHOULD NOT result if some or all of the requested locales are not supported
> by the OpenID Provider.

You can get the value of `ui_locales` request paremeter as a `string` array
by referring to `UiLocales` property of `AuthorizationResponse` instance.
Note that, however, you have to explicitly specify which UI locales to
support using the management console ([Service Owner Console][10]) because
`UiLocales` property returns only supported UI locales. In other words,
it is ensured that the array returned by `UiLocales` never contains
unsupported UI locales whatever `ui_locales` request parameter contains.

It is up to you whether to honor `ui_locales` parameter or not. Of course,
you may use any means you like to internationalize the authorization page.


#### Display type

An authorization request may contain `display` request parameter to specify
how to display the authorization page. It is a new request parameter defined
in [OpenID Connect Core 1.0][9]. The predefined values of the request
parameter are as follows. The descriptions in the table are excerpts from
the specification.

| Value | Description |
|:------|:------------|
| page  | The Authorization Server SHOULD display the authentication and consent UI consistent with a full User Agent page view. If the display parameter is not specified, this is the default display mode. |
| popup | The Authorization Server SHOULD display the authentication and consent UI consistent with a popup User Agent window. The popup User Agent window should be of an appropriate size for a login-focused dialog and should not obscure the entire window that it is popping up over. |
| touch | The Authorization Server SHOULD display the authentication and consent UI consistent with a device that leverages a touch interface. |
| wap   | The Authorization Server SHOULD display the authentication and consent UI consistent with a "feature phone" type display. |

You can get the value of `display` request parameter as an instance of
`Display` enum by referring to `Display` property or `AuthorizationResponse`
instance. By default, all the display types are checked as supported in the
management console ([Service Owner Console][10]), but you can uncheck them
to declare some values are not supported. If an unsupported value is
specified as the value of `display` request parameter, it will result in
returning an `invalid_request` error to the client application that made
the authorization request.


Authorization Decision Endpoint
-------------------------------

In an authorization page, an end-user decides either to grant permissions to
the client application which made the authorization request or to deny the
authorization request. An authorization server must be able to receive the
decision and return a proper response to the client application according to
the decision.

csharp-oauth-server receives the end-user's decision at
`/api/authorization/decision`. In this document, we call the endpoint
_authorization decision endpoint_. In csharp-oauth-server, the implementation
of the authorization decision endpoint is in
<code>[AuthorizationDecisionController.cs][11]</code>.

As the final step, `AuthorizationDecisionController` calls `Handle()` method
of `AuthorizationRequestDecisionHandler`. The important point is that the
constructor of `AuthorizationRequestDecisionHandler` requires an implementation
of `IAuthorizationRequestDecisionHandlerSpi` interface. The interface is the
Service Provider Interface that you need to implement. csharp-oauth-server
includes a simple example (`AuthorizationRequestDecisionHandlerSpiImpl`).


#### End-User Authentication

By design, Authlete does not care about how end-users are authenticated at all.
Instead, Authlete requires the subject of the authenticated end-user.

_Subject_ is a technical term in the area related to identity and it means
a unique identifier. In a typical case, subjects of end-users are values of
the primary key column or another unique column in a user database.

When an end-user grants permissions to a client application, you have to
let Authlete know the subject of the end-user. In the context of
`AuthorizationRequestDecisionHandlerSpi` interface, this can be described
as follows: _"if `IsClientAuthorized()` returns `true`, then
`GetUserSubject()` must return the subject of the end-user."_

For end-user authentication, csharp-oauth-server has `UserDao` class and
`UserEntity` class. These two classes compose a dummy user database.
Of course, you have to replace them with your own implementation to
refer to your actual user database.


Token Endpoint
--------------

The implementation of the token endpoint is in <code>[TokenController.cs][12]</code>.

`TokenController` uses `TokenRequestHandler` class and delegates the task to
handle a token request to `Handle()` method of the class. The important point
is that the constructor of `TokenRequestHandler` requires an implementation of
`ITokenRequestHandlerSpi` interface. The interface is the Service Provider
Interface that you need to implement. csharp-oauth-server includes a simple
example (`TokenRequestHandlerSpiImpl`).

`AuthenticateUser` method of `ITokenRequestHandlerSpi` is used to authenticate
an end-user. However, the method is called only when the grant type of a token
request is [Resource Owner Password Credentials][13]. Therefore, if you don't
support the grant type, you can leave your implementation of the method empty.


Introspection Endpoint
----------------------

The implementation of the introspection endpoint is in
<code>[IntrospectionController.cs][14]</code>.

[RFC 7662][15] (OAuth 2.0 Token Introspection) requires that the endpoint
be protected in some way or other. The implementation of the protection in
`IntrospectionController.cs`  is for demonstration purpose only, and it is
not suitable for production use. Therefore, modify the code accordingly.


Contact
-------

| Purpose   | Email Address        |
|:----------|:---------------------|
| General   | info@authlete.com    |
| Sales     | sales@authlete.com   |
| PR        | pr@authlete.com      |
| Technical | support@authlete.com |


[1]: https://www.authlete.com/
[2]: https://tools.ietf.org/html/rfc6749
[3]: https://openid.net/connect/
[4]: https://docs.authlete.com/
[5]: https://github.com/authlete/authlete-csharp
[6]: https://www.nuget.org/packages/Authlete.Authlete
[7]: AuthorizationServer/Controllers/AuthorizationController.cs
[8]: https://authlete.github.io/authlete-csharp/
[9]: https://openid.net/specs/openid-connect-core-1_0.html
[10]: https://www.authlete.com/documents/so_console/
[11]: AuthorizationServer/Controllers/AuthorizationDecisionController.cs
[12]: AuthorizationServer/Controllers/TokenController.cs
[13]: https://tools.ietf.org/html/rfc6749#section-4.3
[14]: AuthorizationServer/Controllers/IntrospectionController.cs
[15]: https://tools.ietf.org/html/rfc7662
