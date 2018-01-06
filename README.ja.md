認可サーバー実装 (C#)
=====================

概要
----

[OAuth 2.0][1] と [OpenID Connect][2] をサポートする認可サーバーの
C# による実装です。

この実装は、ASP.NET Core API と、NuGet パッケージ [Authlete.Authlete][4]
として提供される [authlete-csharp][3] ライブラリを用いて書かれています。

この実装は「DB レス」です。 これの意味するところは、認可データ
(アクセストークン等) や認可サーバー自体の設定、クライアントアプリケーション群の設定を保持するためのデータベースを管理する必要がないということです。
これは、[Authlete][5] をバックエンドサービスとして利用することにより実現しています。

Authlete のアーキテクチャーの詳細については、
*[New Architecture of OAuth 2.0 and OpenID Connect Implementation][6]*
をお読みください。真のエンジニアであれば、このアーキテクチャーを気に入ってくれるはずです ;-)
なお、日本語版は「[OAuth 2.0 / OIDC 実装の新アーキテクチャー][19]」です。

> The primary advantage of this architecture is in that the
> backend service can focus on implementing OAuth 2.0 and OpenID
> Connect without caring about other components such as identity
> management, user authentication, login session management, API
> management and fraud detection. And, consequently, it leads to
> another major advantage which enables the backend service
> (implementation of OAuth 2.0 and OpenID Connect) to be combined
> with any solution of other components and thus gives flexibility
> to frontend server implementations.
>
> このアーキテクチャーの一番の利点は、アイデンティティー管理やユーザー認証、
> ログインセッション管理、API 管理、不正検出などの機能について気にすることなく、
> バックエンドサービスが OAuth 2.0 と OpenID Connect の実装に集中できることにあります。
> この帰結として、バックエンドサービス (OAuth 2.0 と OpenID Connect の実装)
> をどのような技術部品とも組み合わせることが可能というもう一つの大きな利点が得られ、
> フロントエンドサーバーの実装に柔軟性がもたらされます。



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


ライセンス
----------

  Apache License, Version 2.0


ソースコード
------------

  <code>https://github.com/authlete/csharp-oauth-server</code>


実行方法
--------

1. この認可サーバーの実装をダウンロードします。

        $ git clone https://github.com/authlete/csharp-oauth-server.git
        $ cd csharp-oauth-server/AuthorizationServer

2. 設定ファイルを編集して API クレデンシャルズをセットします。

        $ vi authlete.properties

3. Start the authorization server on [http://localhost:5000][18].

        $ dotnet run


#### 設定ファイル

`csharp-oauth-server` は `authlete.properties` を設定ファイルとして参照します。
他のファイルを使用したい場合は、次のようにそのファイルの名前を環境変数
`AUTHLETE_CONFIGURATION_FILE` で指定してください。

    $ export AUTHLETE_CONFIGURATION_FILE=local.authlete.properties


エンドポイント
--------------

この実装は、下表に示すエンドポイントを公開します。

| エンドポイント                     | パス                                |
|:-----------------------------------|:------------------------------------|
| 認可エンドポイント                 | `/api/authorization`                |
| トークンエンドポイント             | `/api/token`                        |
| JWK Set ドキュメントエンドポイント | `/api/jwks`                         |
| 設定エンドポイント                 | `/.well-known/openid-configuration` |
| 取り消しエンドポイント             | `/api/revocation`                   |
| イントロスペクションエンドポイント | `/api/introspection`                |

認可エンドポイントとトークンエンドポイントは、[RFC 6749][1]、[OpenID Connect Core 1.0][7]、
[OAuth 2.0 Multiple Response Type Encoding Practices][8]、[RFC 7636][9] ([PKCE][10])、
その他の仕様で説明されているパラメーター群を受け付けます。

JWK Set ドキュメントエンドポイントは、クライアントアプリケーションが
(1) この OpenID プロバイダーによる署名を検証できるようにするため、また
(2) この OpenID へのリクエストを暗号化できるようにするため、JSON Web
Key Set ドキュメント (JWK Set) を公開します。

設定エンドポイントは、この OpenID プロバイダーの設定情報を
[OpenID Connect Discovery 1.0][11] で定義されている JSON フォーマットで公開します。

取り消しエンドポイントはアクセストークンやリフレッシュトークンを取り消すための
Web API です。 その動作は [RFC 7009][12] で定義されています。

イントロスペクションエンドポイントはアクセストークンやリフレッシュトークンの情報を取得するための
Web API です。 その動作は [RFC 7662][13] で定義されています。


認可リクエストの例
------------------

次の例は [Implicit フロー][14]を用いて認可エンドポイントからアクセストークンを取得する例です。
`{クライアントID}` となっているところは、あなたのクライアントアプリケーションの実際のクライアント
ID で置き換えてください。 クライアントアプリケーションについては、[Getting Started][15]
および開発者コンソールの[ドキュメント][16]を参照してください。

    http://localhost:5000/api/authorization?client_id={クライアントID}&response_type=token

上記のリクエストにより、認可ページが表示されます。
認可ページでは、ログイン情報の入力と、"Authorize" ボタン (認可ボタン) もしくは "Deny" ボタン
(拒否ボタン) の押下が求められます。 ログイン情報として、下記のいずれかを使用してください。


| ログイン ID | パスワード |
|:-----------:|:----------:|
|     john    |    john    |
|     jane    |    jane    |

もちろんこれらのログイン情報はダミーデータですので、ユーザーデータベースの実装をあなたの実装で置き換える必要があります。


カスタマイズ
------------

この実装をカスタマイズする方法については [CUSTOMIZATION.ja.md][17] に記述されています。
Authlete はユーザーアカウントを管理しないので、基本的には「ユーザー認証」に関わる部分についてプログラミングが必要となります。
これは設計によるものです。 ユーザー認証の仕組みを実装済みの既存の Web
サービスにもスムーズに OAuth 2.0 と OpenID Connect の機能を組み込めるようにするため、Authlete
のアーキテクチャーは認証と認可を慎重に分離しています。


コンタクト
----------

| 目的 | メールアドレス       |
|:-----|:---------------------|
| 一般 | info@authlete.com    |
| 営業 | sales@authlete.com   |
| 広報 | pr@authlete.com      |
| 技術 | support@authlete.com |


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
[17]: CUSTOMIZATION.ja.md
[18]: http://localhost:5000/
[19]: https://qiita.com/TakahikoKawasaki/items/b2a4fc39e0c1a1949aab
