カスタマイズ
============

このドキュメントでは、この認可サーバーの実装をカスタマイズする方法を説明します。


概要
----

この認可サーバーの実装では、[Authlete][1] をバックエンドとして使用しています。
これは、(1) [OAuth 2.0][2] と [OpenID Connect][3] の実装の中心となる部分が
csharp-oauth-server のソースツリー内ではなくクラウド上の Authlete
サーバー内にあること、そして (2)
アクセストークンなどの認可データ、認可サーバー自体の設定やクライアントアプリケーションの設定が、
ローカルデータベース内ではなくクラウド上のデータベースに保存されるということ、
を意味します。 そのため、非常に単純化して言うと、次の図が示すように、
この実装はクライアントアプリケーションと Authlete サーバーの間の仲介役でしかありません。

```
+--------+          +---------------------+          +----------+
|        |          |                     |          |          |
| Client | <------> | csharp-oauth-server | <------> | Authlete |
|        |          |                     |          |          |
+--------+          +---------------------+          +----------+
```

とはいえ、Authlete は**認可**に特化しており、エンドユーザーの**認証**に関することは何もしないので、
認証に関わる機能は csharp-oauth-server のソースツリー内に実装されています。

ですので、少なくとも、エンドユーザーの認証に関する部分についてはカスタマイズが必要です。
一方、認可画面の UI デザインなどの他の部分のカスタマイズは任意です。


全体の構成
----------

Authlete が提供する [Web API][4] を使い、認可サーバーを実装することができます。
[authlete-csharp][5] ライブラリは、Authlete Web API と直接やりとりするための
`IAuthleteApi` インターフェースとその実装を含んでいます。 また、`IAuthleteApi`
をラッピングするユーティリティークラス群も含んでおり、それらのクラス群を使えば、
Authlete API を直接使用するよりもかなり簡単に認可サーバーを書くことができます。

[authlete-csharp][5] ライブラリは、NuGet パッケージ [Authlete.Authlete][6]
として利用できるようになっています。


認可エンドポイント
------------------

認可エンドポイントの実装は <code>[AuthorizationController.cs][7]</code>
内にあります。

まずはじめに、`AuthorizationController` は Authlete の <c>/api/auth/authorization</c>
API を呼びます。 次に、`AuthorizationController` は当 API からのレスポンスに含まれる
`action` パラメーターの値をチェックし、次にとる行動を決めます。

`action` が `INTERACTION` の場合、`AuthorizationController` は認可ページ (HTML)
をクライアントアプリケーションに返します。 `action=INTERACTION`
のときにおこなわなければならないことは、authlete-csharp の [API リファレンス][8]
内にある `AuthorizationResponse` の説明に書かれています。

`action` が `NO_INTERACTION` の場合、`AuthorizationController` は
`NoInteractionHandler` の `Handle()` メソッドを呼びます。 重要な点は、
`NoInteractionHandler` のコンストラクターが `INoInteractionHandlerSpi`
インターフェースの実装を要求することです。
このインターフェースは、あなたが実装する必要のある Service Provider Interface
(SPI) です。 csharp-oauth-server には簡単な例 (`NoInteractionHandlerSpiImpl`)
が含まれています。

`action` が `INTERACTION` でも `NO_INTERACTION` でもない場合、
`AuthorizationController` は残りの仕事を `AuthorizationRequestErrorHandler`
に任せます。 このハンドラーは SPI を要求しません。


認可ページ
----------

`AuthorizationController` は認可ページを手作業で描画し、クライアントアプリケーションに返します。
`Views/Authorization/Page.cshtml` がビューとして、`Models/AuthorizationPageModel.cs`
がモデルとして使用されます。

csharp-oauth-server の認可サーバーの実装は単純ですが、変更したいだろうと思われる点が多くあります。
下記は考慮項目の例です。

#### 国際化

認可ページの国際化に際して、認可リクエストに含まれる `ui_locales`
パラメーターを考慮に入れてもよいでしょう。 これは [OpenID Connect Core 1.0][9]
で新たに定義されたリクエストパラメーターです。
下記は、このパラメータに関する説明を仕様から抜粋したものです。

> OPTIONAL. End-User's preferred languages and scripts for the user interface,
> represented as a space-separated list of BCP47 [RFC5646] language tag values,
> ordered by preference. For instance, the value "fr-CA fr en" represents a
> preference for French as spoken in Canada, then French (without a region
> designation), followed by English (without a region designation). An error
> SHOULD NOT result if some or all of the requested locales are not supported
> by the OpenID Provider.

`AuthorizationResponse` インスタンスの `UiLocales` プロパティーを参照することで、`ui_locales`
リクエストパラメーターの値を `string` の配列として取得することができます。
ただし、`UiLocales` プロパティーはサポートされている UI ロケールしか返さないので、
管理コーンソール ([Service Owner Console][10]) を使って明示的に UI
ロケールを指定する必要があることに注意してください。 別の言い方をすると、`ui_locales`
リクエストパラメーターがどのような値であろうとも、`UiLocales`
が返す配列にはサポートしている UI ロケールしか含まれていないことが保証されています。

`ui_locales` パラメーターを尊重するか否かはあなたの自由です。
もちろん、認可ページの国際化は好きな方法でおこなうことができます。


#### 表示モード

認可リクエストには認可ページの表示方法を指定するための `display`
パラメーターが含まれることがあります。 これは [OpenID Connect Core 1.0][9]
で定義された新しいパラメーターです。
このリクエストパラメーターが取りうる定義済みの値は次のとおりです。
表中の説明は仕様からの抜粋です。

| Value | Description |
|:------|:------------|
| page  | The Authorization Server SHOULD display the authentication and consent UI consistent with a full User Agent page view. If the display parameter is not specified, this is the default display mode. |
| popup | The Authorization Server SHOULD display the authentication and consent UI consistent with a popup User Agent window. The popup User Agent window should be of an appropriate size for a login-focused dialog and should not obscure the entire window that it is popping up over. |
| touch | The Authorization Server SHOULD display the authentication and consent UI consistent with a device that leverages a touch interface. |
| wap   | The Authorization Server SHOULD display the authentication and consent UI consistent with a "feature phone" type display. |

`AuthorizationResponse` インスタンスの `Display` プロパティーで、`display`
リクエストパラメーターの値を列挙型 `Display` のインスタンスとして取得することができます。
デフォルトでは、管理コンソール ([Service Owner Console][10])
では全ての表示タイプがチェックされており、サポートしていることを示していますが、
チェックをはずすことでサポートしないと宣言することもできます。
サポートしていない値が `display` リクエストパラメーターに指定された場合、
その認可リクエストを出したクライアントアプリケーションには `invalid_request`
エラーが返されることになります。


認可決定エンドポイント
----------------------

認可ページでエンドユーザーは、認可リクエストをおこなったクライアントアプリケーションに権限を与えるか、
もしくは認可リクエストを拒否するか、どちらかを選択します。
認可サーバはその決定を受け取り、それに従ってクライアントアプリケーションに適切な応答を返せなければなりません。

csharp-oauth-server は、エンドユーザーの決定を `/api/authorization/decision` で受け取ります。
この文書では、当該エンドポイントを認可決定エンドポイントと呼びます。 csharp-oauth-server
では、認可決定エンドポイントの実装は <code>[AuthorizationDeisionController.cs][11]</code>
内にあります。

最後の手順として、`AuthorizationDecisionController` は `AuthorizationRequestDecisionHandler`
の `Handle()` メソッドを呼びます。 重要な点は、`AuthorizationRequestDecisionHandler`
のコンストラクターが `IAuthorizationRequestDecisionHandlerSpi`
インターフェースの実装を要求することです。
このインターフェースは、あなたが実装する必要のある Service Provider Interface です。
csharp-oauth-server には簡単な例 (`AuthorizationRequestDecisionHandlerSpiImpl`)
が含まれています。


#### エンドユーザー認証

設計により、エンドユーザーをどのように認証するかについては、Authlete は全く気にしません。
その代わりに、Authlete は認証されたエンドユーザーのサブジェクトを要求します。

「サブジェクト」はアイデンティティー関連分野の専門用語で、一意識別子のことを意味します。
典型的には、エンドユーザーのサブジェクトは、
ユーザーデータベース内のプライマリーキーカラムもしくは他のユニークカラムの値です。

エンドユーザーがクライアントアプリケーションに権限を与えたときは、そのエンドユーザーのサブジェクトを
Authlete に伝える必要があります。 `AuthorizationRequestDecisionhandlerSpi`
インターフェースの文脈では、次のように表現することができます：
_「もしも `IsClientAuthorized()` が `true` を返すのであれば、そのときは
`GetUserSubject()` はエンドユーザーのサブジェクトを返さなければならない。」_

エンドユーザー認証のため、csharp-oauth-server には `UserDao` クラスと `UserEntity`
クラスがあります。この二つのクラスでダミーのユーザーデータベースを構成しています。
もちろん、実際のユーザーデータベースを参照するためには、これらをあなたの実装で置き換える必要があります。


トークンエンドポイント
----------------------

トークンエンドポイントの実装は <code>[TokenController.cs][12]</code> 内にあります。

`TokenController` は `TokenRequestHandler` クラスを使い、トークンリクエストの処理を当クラスの
`Handle()` メソッドに任せます。 重要な点は、`TokenRequestHandler` のコンストラクターが
`ITokenRequestHandlerSpi` インターフェースの実装を要求することです。
このインターフェースは、あなたが実装する必要のある Service Provider Interface です。
csharp-oauth-server には簡単な例 (`TokenRequestHandlerSpi`) が含まれています。

`ITokenRequestHandlerSpi` の `AuthenticateUser` メソッドはエンドユーザーを認証するのにしようされます。
しかし、このメソッドが呼ばれるのはトークンリクエストの認可タイプが
[Resource Owner Password Credentials][13] の場合のみです。
そのため、この認可タイプをサポートしないのであれば、メソッドの実装は空でかまいません。


イントロスペクションエンドポイント
----------------------------------

イントロスペクションエンドポイントの実装は <code>[IntrospectionController.cs][14]</code>
内にあります。

[RFC 7662][15] (OAuth 2.0 Token Introspection)
は、イントロスペクションエンドポイントを何らかの方法で保護することを要求しています。
`IntrospectionController.cs`
内の保護の実装はデモンストレーション用のものであり、本番環境での利用には向かないので、適宜変更してください。


コンタクト
----------

| 目的 | メールアドレス       |
|:-----|:---------------------|
| 一般 | info@authlete.com    |
| 営業 | sales@authlete.com   |
| 広報 | pr@authlete.com      |
| 技術 | support@authlete.com |


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
