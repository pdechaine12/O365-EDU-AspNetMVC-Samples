---
page_type: sample
products:
- office-365
- ms-graph
languages:
- csharp
- aspx
- javascript
extensions:
  contentType: samples
  technologies:
  - Microsoft Graph
  services:
  - Education
  - Office 365
  createdDate: 2/16/2016 5:46:53 PM
  scenarios:
  - Education
---
# EDUGraphAPI - Office 365 Education のコード サンプル (ASP.NET MVC)

このサンプルでは、Microsoft Graph API 経由で学校での役割/生徒名簿のデータに加え、利用できる O365 サービスと統合する方法を示します。 

学校のデータは、[Microsoft School Data Sync](http://sds.microsoft.com) によって O365 Education テナントで同期されます。  

**目次**
* [サンプルの目標](#sample-goals)
* [前提条件](#prerequisites)
* [Azure Active Directory にアプリケーションを登録する](#register-the-application-in-azure-active-directory)
* [ローカルでビルドしてデバッグする](#build-and-debug-locally)
* [Azure にサンプルを展開する](#deploy-the-sample-to-azure)
* [コードを理解する](#understand-the-code)
* [[オプション] WebJob をローカルでビルドしてデバッグする](#optional-build-and-debug-the-webjob-locally)
* [質問とコメント](#questions-and-comments)
* [投稿](#contributing)

## サンプルの目標

このサンプルでは、以下を例示します。

* 以下を含む Graph API の呼び出し:

  * [Microsoft Azure Active Directory Graph API](https://www.nuget.org/packages/Microsoft.Azure.ActiveDirectory.GraphClient/)
  * [Microsoft Graph API](https://www.nuget.org/packages/Microsoft.Graph/)

* ローカル管理のユーザー アカウントと Office 365 (Azure Active Directory) ユーザー アカウントのリンク。 

  アカウントをリンクした後、ユーザーはローカルまたは Office 365 アカウントを使用してサンプル Web サイトにログインし、それを使用できます。

* Office 365 Education から学校、セクション、教師、学生を取得する:

  * [Office 365 教育機関 REST API リファレンス](https://msdn.microsoft.com/office/office365/api/school-rest-operations)
  * SyncData WebJob によってローカル データベースにキャッシュされたデータを同期するには、[差分クエリ](https://msdn.microsoft.com/en-us/library/azure/ad/graph/howto/azure-ad-graph-api-differential-query)を使用します。

EDUGraphAPI は ASP.NET MVC に基づいており、[ASP.NET Identity](https://www.asp.net/identity) もこのプロジェクトで使用されています。

## 前提条件

**このサンプルを展開して実行するには、次のものが必要です**:
* 新しいアプリケーションを登録し、Web アプリを展開する権限を持つ Azure サブスクリプション。
* Microsoft School Data Sync が有効になっている O365 Education テナント
* 次のブラウザーのいずれか:Microsoft Edge、Internet Explorer 9、Safari 5.0.6、Firefox 5、Chrome 13、これらのブラウザーのいずれかの最新バージョン。
さらに:このサンプルをローカルで開発/実行するには、次のものが必要です:  
    * Visual Studio 2017 (すべてのエディション)。
	* C#、.NET Web アプリケーション、JavaScript プログラミング、および Web サービスに精通していること。

## Azure Active Directory にアプリケーションを登録する

1. 新しい Azure ポータル ([https://portal.azure.com/](https://portal.azure.com/)) にサインインします。

2. ページの右上隅のアカウント名を選択して、Azure AD テナントを選択します:

   ![](Images/aad-select-directory.png)

3. [**Azure Active Directory**]、[**アプリの登録**]、[**+追加**] の順にクリックします。

   ![](Images/aad-create-app-01.png)

4. **名前** を入力し、**アプリケーション タイプ**として [**Web App / API**] (Web アプリ/API) を選択します。

   [**サインオン URL**] として https://localhost:44311/ と入力します。

   ![](Images/aad-create-app-02.png)

   [**作成**] をクリックします。

5. 完了すると、アプリが一覧に表示されます。

   ![](/Images/aad-create-app-03.png)

6. クリックして詳細を表示します。 

   ![](/Images/aad-create-app-04.png)

7. [設定] ウィンドウが表示されない場合は、[**すべての設定**] をクリックします。

   * [**プロパティ**] をクリックし、[**マルチテナント**] を**はい**に設定します。

     ![](/Images/aad-create-app-05.png)

     **アプリケーション ID** を任意の場所にコピーして、[**保存**] をクリックします。

   * [**必要なアクセス許可**] をクリックします。以下のアクセス許可を追加します。

     | API | アプリケーション アクセス許可 | 委任されたアクセス許可 |
| ------------------------------ | ---------------------------------------- | ---------------------------------------- |
| Microsoft Graph | すべてのユーザーの完全なプロファイルの読み取り<br> ディレクトリ データの読み取り<br> すべてのグループの読み取り | ディレクトリデータの読み取り<br>サインインしたユーザーとしてディレクトリにアクセス<br>ユーザーのサインイン<br> ユーザーがアクセスできるすべてのファイルへのフル アクセス<br> ユーザー ファイルへのフル アクセスを持つ<br> 成績を含まないユーザーのクラスの課題の読み取り<br> 成績を含まないユーザーのクラスの課題の読み取りと書き込み<br> ユーザーのクラスの課題とその成績の読み取り<br> ユーザーのクラスの課題とその成績の読み取りと書き込み |
| Windows Azure Active Directory | ディレクトリ データの読み取り | サインインしてユーザー プロファイルを読み取る<br>ディレクトリ データの読み取りおよび書き込み |

     ![](/Images/aad-create-app-06.png)

     ​

     **アプリケーション アクセス許可**

     | アクセス許可 | 説明 | 管理者の同意が必要 |
| ----------------------------- | ---------------------------------------- | ---------------------- |
| すべてのユーザーの完全なプロファイルの読み取り | サインインしているユーザーなしで、組織内の他のユーザーのプロファイル プロパティ、グループ メンバーシップ、レポート、およびマネージャーの完全なセットの読み取りをアプリに許可します。| はい |
| ディレクトリ データの読み取り | サインインしているユーザーなしで、ユーザー、グループ、アプリなどの組織のディレクトリ内のデータをアプリで読み取りできるようにします。| はい |

     **委任されたアクセス許可**

     | アクセス許可 | 説明 | 管理者の同意が必要 |
| ----------------------------- | ---------------------------------------- | ---------------------- |
| ディレクトリ データの読み取り | アプリで、ユーザー、グループ、アプリなどの組織のディレクトリ内のデータを読み取ることができるようにします。| はい |
| サインインしたユーザーとしてディレクトリにアクセス | サインインしているユーザーと同じように、アプリでディレクトリ内の情報にアクセスできるようにします。| はい |
| ユーザーのサインイン | ユーザーが職場または学校アカウントでアプリにサインインできるようにします。またアプリで、ユーザーの基本的なプロファイル情報を読み取れるようにします。| いいえ |
| サインインとユーザー プロファイルの読み取り | ユーザーがアプリにサインインできるようにします。またサインインしているユーザーのプロファイルをアプリで読み取ることができるようにします。また、サインインしているユーザーの基本会社情報をアプリで読み取れるようにします。| いいえ |
| ディレクトリ データの読み取りおよび書き込み | アプリで、ユーザーやグループなどの組織のディレクトリ内のデータを読み書きできるようにします。アプリでユーザーまたはグループの削除や、ユーザー パスワードのリセットはできません。| はい |

     ​

   * [**キー**] をクリックして、新しいキーを追加します。

     ![](Images/aad-create-app-07.png)

     [**保存**] をクリックしてから、キーの**値**を任意の場所にコピーします。 

   [設定] ウィンドウを閉じます。

8. [**マニフェスト**] をクリックします。

   ![](Images/aad-create-app-08.png)

   次の JSON を **keyCredentials** の配列に挿入します。

   ~~~json
       {
         "customKeyIdentifier": "nNWUyxhgK5zcg7pPj8UFo1xFM9Y=",
         "keyId": "fec5af6a-1cc8-45ec-829f-95999e623b2d",
         "type": "AsymmetricX509Cert",
         "usage": "Verify",
         "value": "MIIDIzCCAg+gAwIBAgIQUWcl+kIoiZxPK2tT8v05WzAJBgUrDgMCHQUAMCQxIjAgBgNVBAMTGUVkdUdyYXBoQVBJIEFwcCBPbmx5IENlcnQwHhcNMTYxMDMxMTYwMDAwWhcNMzYxMDMwMTYwMDAwWjAkMSIwIAYDVQQDExlFZHVHcmFwaEFQSSBBcHAgT25seSBDZXJ0MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAwND0Wclbty/5UYwsrjAvSFaw8JOi33lXP1QI4qecOH4HXvrhz4L5ZN8thG6L/nSIcocuELNbDfJhLehBzxKwtvq9tO0o3MpFK0aloQS5JmoAstMns427osG8DpfnaqwiFyMv558fUHSEkx8GSU/IQgZ6IoSLahTSGCy0LRFHKIyZ6Xo0z9zYN07eQO53obakNlF6YzUg+2v6jLmKnmSXbog+46F9yVvTK2/4MLdPy7lKO2lycec+mljlBWJF4shLacaVrdtQZtanY0zN+XlM48mUVSToNz18tGX/cW9PT5WqIO/O5liEnz9O5u4NTBUUYDqiSuxA4yHV63A+zxhwPwIDAQABo1kwVzBVBgNVHQEETjBMgBAUkYVZ6pBIdTnoV4pmTRzwoSYwJDEiMCAGA1UEAxMZRWR1R3JhcGhBUEkgQXBwIE9ubHkgQ2VydIIQUWcl+kIoiZxPK2tT8v05WzAJBgUrDgMCHQUAA4IBAQBwRDrFpLRYGFARs20Ez+sK6ACrtFbVC5tAnFxr97FWTbixXFm1GPC/pmSnYsiRtiLMliX1+QmTIT80OFk2rfnv3EjY2uCF0XWXH7oRonUFpScA2rQ0geEPRVDXHQ9TJcdEX6+QD6/hAFyANUkWb9uHT1srIxUHerwPCmprOfSCqLVkYXZgvnvWC9XeJP4KriftiqNkfr2FIjqvWrkUMn7iHBHRMW42gfsHoX9LmRLjoqnm1YEyS/t2tibL3FAsJvWv0T03JDCwePF13oItzV0lp0jJDz+xahz8aG3gkacmjzliBeXWWEo9VfxOGLsHjonj3lRSsQLfOn5k3e6lxsJG"
       }
   ~~~

   ![](Images/aad-create-app-09.png)

   [**保存**] をクリックします。

   > 注: この手順では、WebJob で使用される証明書を構成します。詳細については、「**アプリケーションの認証フロー**」セクションを確認してください。

## ローカルでビルドしてデバッグする

このアプリケーションをローカルで実行、ビルド、または開発するには、お客様が既に所有している Visual Studio 2017 のエディションでこのプロジェクトを開くことも、Community エディションをダウンロードしてインストールすることもできます。



**EDUGraphAPI.Web** をデバッグします。

1. **Web.config** で **appSettings** を構成します。 

   ![](Images/web-app-config.png)

   - **ida:ClientId**: 前に作成したアプリ登録のクライアント ID を使用します。
   - **ida:ClientSecret**: 前に作成したアプリ登録のキーの値を使用します。
   - **SourceCodeRepositoryURL**: 自分のフォークのリポジトリ URL を使用します。


2. **EDUGraphAPI.Web** をスタートアップ プロジェクトとして設定し、F5 を押します。 

## Azure にサンプルを展開する

**GitHub 認証**

1. トークンを生成する

   - Web ブラウザーで https://github.com/settings/tokens を開きます。
   - このリポジトリをフォークした GitHub アカウントにサインインします。
   - [**トークンの生成**] をクリックします。
   - [**トークンの説明**] テキスト ボックスに値を入力します
   - 以下を選択します (選択内容は下のスクリーンショットと一致する必要があります):
        - repo (all) -> repo:status, repo_deployment, public_repo
        - admin:repo_hook -> read:repo_hook

   ![](Images/github-new-personal-access-token.png)

   - [**トークンの生成**] をクリックします
   - トークンをコピーする

2. Azure リソース エクスプローラーで GitHub トークンを Azure に追加する

   - Web ブラウザーで https://resources.azure.com/providers/Microsoft.Web/sourcecontrols/GitHub を開きます。
   - Azure アカウントでログインします。
   - 正しい Azure サブスクリプションを選択しました。
   - [**読み取り/書き込み**] モードを選択します。
   - [**編集**] をクリックします。
   - トークンを**トークン パラメーター**に貼り付けます。

   ![](Images/update-github-token-in-azure-resource-explorer.png)

   - [**PUT**] をクリックします

**GitHub から Azure コンポーネントを展開する**

1. ビルドが VSTS ビルドを通過していることを確認してください。

2. このリポジトリを GitHub アカウントにフォークします。

3. [Azure への展開] ボタンをクリックします:

   [![Azure への展開](http://azuredeploy.net/deploybutton.png)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FOfficeDev%2FO365-EDU-AspNetMVC-Samples%2Fmaster%2Fazuredeploy.json)

4. 展開ページで値を入力し、[**上記の使用条件に同意する**] チェック ボックスを選択します。

   ![](Images/azure-auto-deploy.png)

   * **リソース グループ**: 新しいグループを作成することをお勧めします。

   * **サイト名**: 名前を入力してください。EDUGraphAPICanviz や EDUGraphAPI993 などです。

     > 注:入力した名前がすでに使用されている場合、いくつかの検証エラーが発生します。
     >
     > ![](Images/azure-auto-deploy-validation-errors-01.png)
     >
     > クリックすると、ストレージ アカウントが既に他のリソース グループ/サブスクリプションにあるなどの詳細情報が表示されます。
     >
     > この場合は、別の名前を使用してください。

   * **ソース コード リポジトリの URL**: <YOUR REPOSITORY>をフォークのリポジトリ名に置き換えます。

   * **ソース コードの手動統合**: 独自のフォークからデプロイしているため、**false** を選択します。

   * **クライアント ID**: 前に作成したアプリ登録のクライアント ID を使用します。

   * **クライアント シークレット**: 前に作成したアプリ登録のキーの値を使用します。

   * [**上記の使用条件に同意する**] をチェックします。

5. [**購入**] をクリックします。

**応答 URL をアプリの登録に追加する**

1. 展開が完了したら、リソース グループを開きます。

   ![](Images/azure-resource-group.png)

2. Web アプリをクリックします。

   ![](Images/azure-web-app.png)

   URL を任意の場所にコピーして、スキーマを **https** に変更します。これは応答 URL であり、次のステップで使用されます。

3. 新しい Azure ポータルでアプリの登録に移動し、[設定] ウィンドウを開きます。

   応答 URL を追加します。

   ![](Images/aad-add-reply-url.png)

   > 注: サンプルをローカルでデバッグするには、応答 URL として https://localhost:44311/ が入力されていることを確認してください。

4. [**保存**] をクリックします。

## コードを理解する

### 概要

**ソリューション コンポーネント図**

![](Images/solution-component-diagram.png)

ソリューションの最上位のレイヤーには、Web アプリケーションと WebJob コンソール アプリケーションが含まれています。

中間のレイヤーには、2 つのクラス ライブラリ プロジェクトが含まれます。 

一番下のレイヤーには、3 つのデータ ソースが含まれます。

**EDUGraphAPI.Web**

この Web アプリケーションは、**個々のユーザー アカウント** オプションが選択された ASP.NET MVC プロジェクト テンプレートに基づいています。 

![](Images/mvc-auth-individual-user-accounts.png)

次のファイルは MVC テンプレートによって作成され、わずかな変更のみが行われました。

1. **/App_Start/Startup.Auth.Identity.cs** (元の名前は Startup.Auth.cs です)
2. **/Controllers/AccountController.cs**

このサンプル プロジェクトでは、**[ASP.NET Identity](https://www.asp.net/identity)** と **[Owin](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/owin)** を使用しています。これら 2 つのテクノロジーにより、異なる認証方法が簡単に共存できます。このサンプルを理解するには、ASP.NET MVC に加えて、これらのコンポーネントにも精通していることが不可欠です。

以下は、この Web プロジェクトで使用される重要なクラス ファイルです。

| ファイル | 説明 |
| --------------------------------- | ---------------------------------------- |
| /App_Start/Startup.Auth.AAD.cs | Azure Active Directory の認証と統合します |
| /Controllers/AdminController.cs| 管理アクションが含まれます:<br>管理者の同意、リンクされたアカウントの管理、アプリのインストール。|
| /Controllers/LinkController.cs | AD とローカル ユーザー アカウントをリンクするアクションが含まれています |
| /Controllers/SchoolsController.cs | 教育データを提示するアクションが含まれています |

この Web アプリケーションは、**マルチテナント アプリ**です。AAD では、次のオプションを有効にしました:

![](Images/app-is-multi-tenant.png)

すべての Azure Active Directory テナントのユーザーはこのアプリにアクセスできます。このアプリはいくつかのアプリケーション アクセス許可を使用するため、テナントの管理者は最初にサインアップ (同意) する必要があります。そうしなければ、ユーザーはエラーになります。

![](Images/app-requires-admin-to-consent.png)

詳細については、「[Build a multi-tenant SaaS web application using Azure AD & OpenID Connect (Azure AD と OpenID Connect を使用してマルチテナント SaaS Web アプリケーションを構築する)](https://azure.microsoft.com/en-us/resources/samples/active-directory-dotnet-webapp-multitenant-openidconnect/)」を参照してください。

**EDUGraphAPI.SyncData**

これは、ユーザー データの同期に使用される WebJob です。**Functions.SyncUsersAsync** メソッドでは、EDUGraphAPI.Common プロジェクトから **UserSyncService** が使用されます。

このプロジェクトは、差分クエリのデモンストレーションを行うために作成されました。詳細については、「[差分クエリ](#differential-query)」セクションをご覧ください。

**EDUGraphAPI.Common**

クラス ライブラリ プロジェクトは、**EDUGraphAPI.Web** と **EDUGraphAPI.SyncData** の両方で使用されます。 

次のテーブルに、プロジェクト内のフォルダーを示します。

| フォルダー | 説明 |
| ------------------ | ---------------------------------------- |
| /Data | ApplicationDbContext およびエンティティ クラスが含まれています |
| /DataSync | EDUGraphAPI.SyncData WebJob によって使用される UserSyncSerextensionsvice クラスが含まれています |
| /DifferentialQuery | 差分クエリを送信し、結果を解析するために使用される DifferentialQueryService クラスが含まれます。|
| /Extensions | コーディングを簡素化し、コードを読みやすくする多くの拡張メソッドが含まれています |
| /Utils | 広く使用されているクラス AuthenticationHelper.cs が含まれています |

**Microsoft.Education**

このプロジェクトは、**[Schools REST API](https://msdn.microsoft.com/en-us/office/office365/api/school-rest-operations)** クライアントをカプセル化します。このプロジェクトのコア クラスは **EducationServiceClient** です。

### データ アクセスとデータ モデル

ASP.NET Identity は、[Entity Framework Code First](https://msdn.microsoft.com/en-us/library/jj193542(v=vs.113).aspx) を使用して、永続化メカニズムのすべてを実装します。これには、パッケージ [Microsoft.AspNet.Identity.EntityFramework](https://www.nuget.org/packages/Microsoft.AspNet.Identity.EntityFramework/) が使用されます。 

このサンプルでは、データベースにアクセスするための **ApplicationDbContext** が作成されます。前述の NuGet パッケージで定義されている **IdentityDbContext** を継承しています。

以下は、このサンプルで使用した重要なデータ モデル (およびその重要なプロパティ) です。

**ApplicationUsers**

**IdentityUser** から継承されます。 

| プロパティ | 説明 |
| ------------- | ---------------------------------------- |
| Organization | ユーザーのテナントです。リンクされていないローカル ユーザーの場合、その値は null です |
| O365UserId | Office 365 アカウントとのリンクに使用されます |
| O365Email | リンクされた Office 365 アカウントのメールです |
| JobTitle | 差分クエリのデモンストレーションに使用されます |
| Department | 差分クエリのデモンストレーションに使用されます |
| Mobile | 差分クエリのデモンストレーションに使用されます |
| FavoriteColor | ローカル データのデモンストレーションに使用されます |

**組織**

このテーブルの行は、AAD のテナントを表します。

| プロパティ | 説明 |
| ---------------- | ------------------------------------ |
| TenantId | テナントの GUID |
| Name | テナントの名前 |
| IsAdminConsented | テナントが管理者の同意を得ているか |

### 管理者の同意フロー

アプリケーション専用アクセス許可では、常にテナント管理者の同意が必要になります。アプリケーションがアプリケーション専用アクセス許可を要求する場合に、ユーザーがそのアプリケーションにサインインしようとすると、このユーザーは同意できないことを示すエラー メッセージが表示されます。

一部の委任アクセス許可でも、テナント管理者の同意が必要になります。たとえば、サインイン済みユーザーとして Azure AD に書き戻しを行うアクセス許可には、テナント管理者の同意が必要です。アプリケーション専用アクセス許可と同様に、管理者の同意が必要な委任アクセス許可を要求するアプリケーションに通常のユーザーがサインインしようとすると、アプリケーションでエラーが発生します。アクセス許可に管理者の同意が必要かどうかは、リソースを公開した開発者により決定されており、リソースのドキュメントに記載されています。

アプリケーションで管理者の同意が必要なアクセス許可を使用する場合、ジェスチャ (管理者がアクションを開始できるボタンやリンク) を設定する必要があります。通常、この操作に対してアプリケーションから送信される要求は OAuth2/OpenID Connect 承認要求ですが、この要求には `prompt=admin_consent` クエリ文字列パラメーターも含まれています。管理者が同意し、ユーザーのテナントにサービス プリンシパルが作成されると、以降のサインイン要求では `prompt=admin_consent` パラメーターは不要になります。管理者は要求されたアクセス許可を許容可能と判断しているため、その時点からは、テナント内の他のユーザーが同意を求められることはありません。



### 認証フロー

このプロジェクトには 4 つの認証フローがあります。

最初の 2 つのフロー (ローカルログイン/O365 ログイン) では、ユーザーはローカル アカウントまたは Office 365 アカウントのいずれかでログインし、他の種類のアカウントにリンクすることができます。この手順は、LinkController に実装されています。

**ローカル ログイン認証フロー**

![](Images/auth-flow-local-login.png)

**O365 ログイン認証フロー**

![](Images/auth-flow-o365-login.png)

**管理者ログイン認証フロー**

このフローは、管理者がシステムにログインして管理操作を実行する方法を示しています。

Office 365 アカウントでアプリにログインした後、管理者はローカル アカウントにリンクするよう求められます。この手順は必須ではなく、スキップできます。 

前述のように、Web アプリはいくつかのアプリケーション アクセス許可を使用するマルチテナント アプリであるため、テナントの管理者は最初にテナントに同意する必要があります。  

このフローは、AdminController に実装されています。 

![](Images/auth-flow-admin-login.png)

**アプリケーションの認証フロー**

このフローは、SyncData WebJob に実装されています。

![](Images/auth-flow-app-login.png)

X509 証明書が使用されます。詳細については、以下のリンクを確認してください。

* [デーモン またはサーバー アプリケーション対 Web API](https://docs.microsoft.com/en-us/azure/active-directory/active-directory-authentication-scenarios#daemon-or-server-application-to-web-api)
* [証明書を使用したデーモン アプリでの Azure AD への認証](https://azure.microsoft.com/en-us/resources/samples/active-directory-dotnet-daemon-certificate-credential/)
* [Office 365 でサービス アプリおよびデーモン アプリを構築する](https://msdn.microsoft.com/en-us/office/office365/howto/building-service-apps-in-office-365)

### 2 種類の Graph API

このサンプルでは、2 つの異なる Graph API が使用されています。

|
| [Azure AD Graph API](https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-graph-api) | [Microsoft Graph API](https://graph.microsoft.io/) |
| ------------ | ---------------------------------------- | ---------------------------------------- |
| 説明 | Azure Active Directory Graph API を使用すると、REST API エンドポイントを介して Azure Active Directory にプログラムによってアクセスできます。アプリは、Azure AD Graph API を使用して、ユーザー、グループ、組織の連絡先などのディレクトリ データとディレクトリ オブジェクトの作成、読み取り、更新、削除 (CRUD) 操作を実行できます。| Outlook、OneDrive、OneNote、Planner、Office Graph などの他の Microsoft サービスからの API も含まれる統合 APIです。すべてが単一のアクセス トークンを持つ単一のエンドポイントからアクセスされます。|
| クライアント | インストールパッケージ [Microsoft.Azure.ActiveDirectory.GraphClient](https://www.nuget.org/packages/Microsoft.Azure.ActiveDirectory.GraphClient/) | インストールパッケージ [Microsoft.Graph](https://www.nuget.org/packages/Microsoft.Graph/) |
| エンドポイント | https://graph.windows.net | https://graph.microsoft.com |
| API エクスプローラー | https://graphexplorer.cloudapp.net/ | https://graph.microsoft.io/graph-explorer |

このサンプルでは、共通インターフェイスに基づいた以下のクラスを使用して、API がどのように関連しているかを示します。  

![](Images/class-diagram-graphs.png)

**IGraphClient** インターフェイスは次の 2 つのメソッドを定義します。**GeCurrentUserAsync** および **GetTenantAsync** です。

**MSGraphClient** は、Microsoft Graph クライアント ライブラリを使用して **IGraphClient** インターフェイスを実装します。

インターフェイスおよび Graph クライアント クラスは、Web アプリの **/Services/GraphClients** フォルダーにあります。ユーザーとテナントの取得方法を示すために、一部のコードを以下に強調表示します。

**Microsoft Graph** - MSGraphClient.cs

~~~c#
        public async Task<UserInfo> GetCurrentUserAsync()
        {
            var me = await graphServiceClient.Me.Request()
                .Select("id,givenName,surname,userPrincipalName,assignedLicenses")
                .GetAsync();
            return new UserInfo
            {
                Id = me.Id,
                GivenName = me.GivenName,
                Surname = me.Surname,
                Mail = me.Mail,
                UserPrincipalName = me.UserPrincipalName,
                Roles = await GetRolesAsync(me)
            };
        }
~~~

~~~c#
        public async Task<TenantInfo> GetTenantAsync(string tenantId)
        {
            var tenant = await graphServiceClient.Organization[tenantId].Request().GetAsync();
            return new TenantInfo
            {
                Id = tenant.Id,
                Name = tenant.DisplayName
            };
        }

~~~

アプリの登録設定では、各 Graph API のアクセス許可が個別に構成されていることに注意してください:

![](/Images/aad-create-app-06.png) 

### Office 365 Education API

[Office 365 Education API](https://msdn.microsoft.com/office/office365/api/school-rest-operations) は、Microsoft School Data Sync でクラウドに同期されている Office 365 テナントからデータを抽出する際に役立ちます。その結果により、教育機関、セクション、教師、学生および名簿に関する情報が得られます。教育機関 REST API は、Education テナント用の Office 365 に含まれる教育機関エンティティへのアクセスを提供します。

このサンプルでは、**Microsoft.Education** クラス ライブラリ プロジェクトは Office 365 Education API をカプセル化するために作成されました。 

**EducationServiceClient** は、ライブラリのコア クラスです。これにより、教育データを簡単に取得できます。

**教育機関の取得**

~~~c#
public async Task<School[]> GetSchoolsAsync()
{
    var schools = await HttpGetArrayAsync<EducationSchool>("education/schools");
    return schools.ToArray();

}
~~~


**セクションの取得**

~~~c#
 public async Task<ArrayResult<EducationClass>> GetAllClassesAsync(string schoolId, string nextLink)
{
           if (string.IsNullOrEmpty(schoolId))
            {
                return new ArrayResult<EducationClass>
                {
                    Value = new EducationClass[] { },
                     NextLink = nextLink
                };
            }
            else
            {
                var relativeUrl = $"education/schools/{schoolId}/classes?$top=12";
                return await HttpGetArrayAsync<EducationClass>(relativeUrl, nextLink);

        
            }

}
~~~

```c#
        public async Task<EducationClass[]> GetMyClassesAsync(bool loadMembers = false, string expandField = "members")
        {
            var relativeUrl = $"education/me/classes";

            // Important to do this in one round trip, not in a sequence of calls.
            if (loadMembers)
            {
                relativeUrl += "?$expand=" + expandField;
            }

            var memberOf = await HttpGetArrayAsync<EducationClass>(relativeUrl);
            var classes = memberOf.ToArray();

            return classes;
        }
```


以下は、教育データを表示するサンプル アプリのスクリーンショットです。

![](Images/edu-schools.png)



![](Images/edu-classes.png)

![](Images/edu-class.png)

**EducationServiceClient** では、REST API の呼び出しを簡素化するために、HttpGet というプレフィックスが付いた 3 つのプライベート メソッドが作成されました。

* **HttpGetAsync**: HTTP GET 要求をターゲット エンドポイントに送信し、JSON 応答文字列を返します。アクセス トークンは、ベアラー認証ヘッダーに含まれています。
* **HttpGetObjectAsync<T>**: HttpGetAsync によって返された JSON 文字列をターゲット型 T に逆シリアル化し、結果オブジェクトを返します。
* **HttpGetArrayAsync<T>**: HttpGetAsync によって返された JSON 文字列をターゲット配列型 T[] に逆シリアル化し、配列を返します。

### 差分クエリ

[差分クエリ](https://msdn.microsoft.com/en-us/Library/Azure/Ad/Graph/howto/azure-ad-graph-api-differential-query)要求は、2 つの連続した要求の間の時間に指定されたエンティティに加えられたすべての変更を返します。たとえば、前の差分クエリ要求の 1 時間後に差分クエリ要求を行うと、その 1 時間以内に行われた変更のみが返されます。この機能は、テナントのディレクトリ データをアプリケーションのデータ ストアと同期するときに特に役立ちます。

関連コードは、**EDUGraphAPI.Common** プロジェクトの次の 2 つのフォルダー内にあります。

* **/DifferentialQuery**: 差分クエリを送信し、差分結果を解析するクラスが含まれています。
* **/DataSync**: ユーザーの同期方法をデモンストレーションするために使用されるクラスが含まれています。

> **DifferentialQuery** フォルダー内のクラスは、いくつかの高度な .NET テクノロジーを使用していることに注意してください。実装の詳細ではなく、これらのクラスの使用に注目してください。

ユーザーを同期するために、User クラスを定義しました:

~~~c#
public class User
{
    public string ObjectId { get; set; }
    public virtual string JobTitle { get; set; }
    public virtual string Department { get; set; }
    public virtual string Mobile { get; set; }
}
~~~

変更可能なプロパティ *JobTitle*、*Department*、*Mobile* は仮想であることに注意してください。**DifferentialQuery** フォルダー内のクラスは、ユーザーの種類のためにプロキシの種類を作成し、変更の追跡のためにこれらの仮想プロパティをオーバーライドします。

**UserSyncService** クラスでは、**DifferentialQueryService** を使用して差分クエリを送信し、差分結果を取得する方法をデモンストレーションします。

```c#
var differentialQueryService = new DifferentialQueryService(/**/);
DeltaResult<Delta<User>> result = await differentialQueryService.QueryAsync<User>(url);
```
そして、差分結果でローカル データベースのユーザーを更新 (または削除) する方法:

~~~c#
foreach (var differentialUser in result.Items)
    await UpdateUserAsync(differentialUser);
//...
private async Task UpdateUserAsync(Delta<User> differentialUser) { /**/ }
~~~

**DataSyncRecord** データ モデルは、永続 DeltaLink に使用されます。

以下は、SyncData WebJob によって生成されたログです:

![](Images/sync-data-web-job-log.png) 

### フィルター

Web プロジェクトの **/Infrastructure** フォルダーには、いくつかの FilterAttributes があります。

**EduAuthorizeAttribute**

これは、AuthorizeAttribute から継承した承認フィルターです。

マルチ認証方式のシナリオで、Web アプリがユーザーを適切なログイン ページにリダイレクトできるようにするために作成されました。

**HandleUnauthorizedRequest** メソッドをオーバーライドして、ユーザーを /Account/Login にリダイレクトします。

~~~c#
protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
{
    filterContext.Result = new RedirectResult("/Account/Login");
}
~~~

**HandleAdalExceptionAttribute**

**AuthenticationHelper** クラスは、アクセス トークンまたは API クライアントのインスタンスを返す数多くのメソッドを公開します。これらのメソッドのほとんどは、**[AuthenticationContext.AcquireTokenSilentAsync](https://msdn.microsoft.com/en-us/library/mt473642.aspx)** を内部的に呼び出します。通常、トークンは **ADALTokenCache** によってデータベースにキャッシュされるため、**AcquireTokenSilentAsync** はアクセス トークンを正常に取得します。 

キャッシュされたトークンの有効期限が切れたり、新しいリソース トークンが要求されたりする場合などには、**AcquireTokenSilentAsync** は **AdalException** をスローします。**HandleAdalExceptionAttribute** は、**AdalException** を処理し、ユーザーを認証エンドポイントへと移動して新しいトークンを取得するために必要です。

場合によっては、以下を呼び出してユーザーを認証エンドポイントへと直接リダイレクトします。

~~~c#
filterContext.HttpContext.GetOwinContext().Authentication.Challenge(
   new AuthenticationProperties { RedirectUri = requestUrl },
   OpenIdConnectAuthenticationDefaults.AuthenticationType);
~~~

また、特にローカル アカウントでログインしたユーザーの場合、リダイレクトされた理由をそのユーザーに伝えるために、以下のページをユーザーに表示する場合があります。

![](Images/web-app-login-o365-required.png)

スイッチを使用して、これを制御します。スイッチの値は、以下によって取得されます。

~~~c#
//public static readonly string ChallengeImmediatelyTempDataKey = "ChallengeImmediately";
var challengeImmediately = filterContext.Controller.TempData[ChallengeImmediatelyTempDataKey];
~~~

値が true の場合、ユーザーをすぐに認証エンドポイントへとリダイレクトします。それ以外の場合は、上記のページが最初に表示され、ユーザーは [ログイン] ボタンをクリックして続行します。

**LinkedOrO365UsersOnlyAttribute**

これは別の承認フィルターです。これにより、リンクされたユーザーまたは Office 365 ユーザーのみが保護されたコントローラー/アクションにアクセスできます。

~~~c#
protected override bool AuthorizeCore(HttpContextBase httpContext)
{
    var applicationService = DependencyResolver.Current.GetService<ApplicationService>();
    var user = applicationService.GetUserContext();
    return user.AreAccountsLinked || user.IsO365Account;
}
~~~

承認されていないユーザーの場合、NoAccess ページを表示します。

~~~c#
protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
{
    filterContext.Result = new ViewResult { ViewName = "NoAccess" };
}
~~~

これまでのところ、**SchoolsController** でのみ使用されています。

### 主要なクラス

**Microsoft.Education**

* `GraphServiceClient`: 要求の作成、Office 365 Education API へのそれらの送信、応答の処理を扱うクラスのインスタンスです。

  | メソッド | 説明 |
| ------------------- | ---------------------------------------- |
| GetSchoolsAsync | Azure Active Directory テナントに存在するすべての教育機関を取得します |
| GetSchoolAsync | オブジェクト ID を使用して教育機関を取得します |
| GetAllSectionsAsync | 教育機関内のセクションを取得します |
| GetMySectionsAsync | 教育機関内で自分のセクションを取得します |
| GetSectionAsync | オブジェクト ID を使用してセクションを取得します |
| GetMembersAsync | 教育機関内のメンバーを取得します |
| GetStudentAsync | 生徒として現在ログインしているユーザーを取得します |
| GetTeacherAsync | 教師として現在ログインしているユーザーを取得します |

**EDUGraphAPI.Common**

* **`Data.ApplicationUser`**: ユーザーを表すクラスのインスタンスです。

* **`Data.Organization`**: Azure AD のテナントを表すクラスのインスタンスです。 

* **`Data.ApplicationDbContext`**:`IdentityDbContext<ApplicationUser>` から継承した、Entity Framework が使用する DbContext クラスです。

* **`DataSync.User`**: Azure AD のユーザーを表すクラスのインスタンスです。変更の追跡に使用されるプロパティは仮想であることに注意してください。

* **`DataSync.UserSyncService`**: ローカル データベース内のユーザーの差分クエリとの同期を処理するクラスのインスタンスです。`SyncAsync` メソッドを呼び出して、ユーザーの同期を開始します。

* **`DifferentialQuery.DifferentialQueryService`**:要求の作成、サービス エンドポイントへの送信、および応答の処理を扱うクラスのインスタンスです。deltaLink で `QueryAsync` メソッドを呼び出して、差分クエリを開始します。差分結果は、`DeltaResultParser`クラスによって `DeltaResult<Delta<TEntity>>` に変換されます。

* **`Utils.AuthenticationHelper`**: アクセス トークン、認証結果、認証コンテキスト、およびサービス クライアントのインスタンスを取得するために使用される静的ヘルパー クラスです。

  | メソッド | 説明 |
| -------------------------------------- | ---------------------------------------- |
| GetActiveDirectoryClientAsync | ActiveDirectoryClient のインスタンスを取得します |
| GetGraphServiceClientAsync | GraphServiceClient のインスタンスを取得します |
| GetEducationServiceClientAsync | EducationServiceClient のインスタンスを取得します |
| GetActiveDirectoryClient | 指定された AuthenticationResult から ActiveDirectoryClient のインスタンスを取得します |
| GetGraphServiceClient | 指定された AuthenticationResult から GraphServiceClient のインスタンスを取得します |
| GetAccessTokenAsync | 指定されたリソースのアクセス トークンを取得します |
| GetAuthenticationResult | 指定されたリソースの AuthenticationResult を取得します |
| GetAuthenticationContext | AuthenticationContext のインスタンスを取得します |
| GetAuthenticationResultAsync | 指定された認証コードから AuthenticationResult を取得します |
| GetAppOnlyAccessTokenForDaemonAppAsync | デーモン アプリのアプリ専用アクセス トークンを取得します |

  上記のメソッドのほとんどに、permission と呼ばれる引数があります。その種類は `Permissions` であり、2 つの定義された値を持つ 列挙型です。

  * `Delegated`: クライアントはサインインしたユーザーとして Web API にアクセスします。
  * `Application`: クライアントは、Web API にそれ自体として直接アクセスします (ユーザー コンテキストなし)。この種類のアクセス許可には、管理者の同意が必要です。

* **`Utils.AuthenticationHelper`**: 承認 URL の作成に使用される静的クラス。`GetUrl` はクラスで定義されている唯一のメソッドです。

* **`Constants`**: アプリ設定の値とその他の定数値が含まれる静的クラスです。

**EDUGraphAPI.Web**

* **`Controllers.AccountController`**: ユーザーが登録、ログイン、およびパスワードを変更するためのアクションが含まれています。

* **`Controllers.AdminController`**: **管理者ログイン認証フロー**を実装します。詳細については、「[認証フロー](#authentication-flows)」セクションを確認してください。

* **`Controllers.LinkController`**: **Local/O365 ログイン認証フロー** を実装します。詳細については、「[認証フロー](#authentication-flows)」セクションを確認してください。

* **`Controllers.SchoolsController`**: 教育機関およびクラスを表示するアクションが含まれています。`SchoolsService` クラスは、主にこのコントローラーによって使用されます。詳細については、「[Office 365 Education API](#office-365-education-api)」セクションを確認してください。

* **`Infrastructure.EduAuthorizeAttribute`**:マルチ認証方式のシナリオで、Web アプリが現在のユーザーを適切なログイン ページにリダイレクトできるようにします。詳細については、「[フィルター](#filters)」セクションを確認してください。

* **`Infrastructure.HandleAdalExceptionAttribute`**: AdalException を処理し、ユーザーを認証エンドポイントまたは /Link/LoginO365Required へと移動します。詳細については、「[フィルター](#filters)」セクションを確認してください。

* **`Infrastructure.LinkedOrO365UsersOnlyAttribute`**: リンクされたユーザーまたは Office 365 ユーザーのみが保護されたコントローラー/アクションにアクセスできるようにします。詳細については、「[フィルター](#filters)」セクションを確認してください。

* **`Models.UserContext`**: ログインしているユーザーのコンテキストです。

* **`Services.GraphClients.AADGraphClient`**: Azure AD Graph API を使用して `IGraphClient` インターフェイスを実装します。詳細については、「[2 種類の Graph API](#two-kinds-of-graph-api)」セクションを確認してください。

* **`Services.GraphClients.MSGraphClient`**: Microsoft Graph API を使用して `IGraphClient` インターフェイスを実装します。詳細については、「[2 種類の Graph API](#two-kinds-of-graph-api)」セクションを確認してください。

* **`Services.ApplicationService.`**: ユーザー/組織の取得/更新を処理するクラスのインスタンスです。

  | メソッド | 説明 |
| ------------------------------- | ---------------------------------------- |
| CreateOrUpdateOrganizationAsync | 組織を作成または更新します |
| GetAdminContextAsync | 現在の管理者のコンテキストを取得します |
| GetCurrentUser | 現在のユーザーを取得します |
| GetCurrentUserAsync | 現在のユーザーを取得します |
| GetUserAsync | ID でユーザーを取得します |
| GetUserContext | 現在のユーザーのコンテキストを取得します |
| GetUserContextAsync | 現在のユーザーのコンテキストを取得します |
| GetLinkedUsers | 指定されたフィルターでリンクされたユーザーを取得します |
| IsO365AccountLinkedAsync | 指定された O365 アカウントがローカル アカウントにリンクされているかどうか |
| SaveSeatingArrangements | 座席の配置を保存します |
| UnlinkAccountsAsync | 指定されたアカウントのリンクを解除します |
| UnlinkAllAccounts | 指定されたテナントのすべてのアカウントのリンクを解除します |
| UpdateLocalUserAsync | O365 ユーザーおよびテナント情報でローカル ユーザーを更新する |
| UpdateOrganizationAsync | 組織を更新する |
| UpdateUserFavoriteColor | 現在のユーザーのお気に入りの色を更新する |

* **`Services.SchoolsService`**: 教育データを取得するために使用されるサービス クラスです。

  | メソッド | 説明 |
| ------------------------------- | ---------------------------------------- |
| GetSchoolsViewModelAsync | SchoolsViewModel を取得します |
| GetSchoolUsersAsync | 指定された教育機関の教師と生徒を取得する |
| GetSectionsViewModelAsync | 指定された教育機関の SectionsViewModel を取得します |
| GetSectionDetailsViewModelAsync | 指定されたセクションの SectionDetailsViewModel を取得します |
| GetMyClasses | 自分のクラスを取得します |

**EDUGraphAPI.SyncData**

* **`Functions`**: ユーザー データを同期するために定期的に実行される `SyncUsersAsync` メソッドが含まれています。
* **`Program`**: WebJob ホストを構成して起動する `Main` メソッドが含まれています。

## [オプション] WebJob をローカルでビルドしてデバッグする

**EDUGraphAPI.SyncData** をデバッグします。

1. Azure でストレージ アカウントを作成し、接続文字列を取得します。
   > 注:
   - [Azure WebJobs SDK](https://github.com/Azure/azure-webjobs-sdk) V2 関連の後、Azure Storage Emulator を使用したローカル デバッグがサポートされます。詳細については、「[Support Azure Storage Emulator (Azure Storage Emulator をサポートする)](https://github.com/Azure/azure-webjobs-sdk/issues/53)」を参照してください。
   - 発行された WebJob が同じストレージ アカウントを使用して Azure で実行されている間は、ローカル デバッグを推奨**しません**。詳細については、[この質問](http://stackoverflow.com/questions/42020647/what-happened-when-using-same-storage-account-for-multiple-azure-webjobs-dev-li)を確認してください。

2. **App.config** を構成します。

   ![](Images/webjob-app-config.png)

   - **接続文字列**:
     - **AzureWebJobsDashboard**: 前の手順で取得した接続文字列を使用します。
     - **AzureWebJobsStorage**: 前の手順で取得した接続文字列を使用します。
   - **アプリの設定**:
     - *ida:ClientId*\*: 前に作成したアプリ登録のクライアント ID を使用します。

3. **EDUGraphAPI.SyncData** をスタートアップ プロジェクトとして設定し、F5 を押します。 

## 質問とコメント

* このサンプルの実行について問題がある場合は、[問題をログに記録](https://github.com/OfficeDev/O365-EDU-AspNetMVC-Samples/issues)してください。
* Graph API 開発全般の質問につきましては、「[Stack Overflow](http://stackoverflow.com/questions/tagged/office-addins)」に投稿してください。ご質問またはコメントに [ms-graph-api] タグが付けられていることを確認してください。 

## 投稿

当社のサンプルに是非貢献してください。投稿方法のガイドラインについては、[投稿ガイド](/Contributing.md)を参照してください。

このプロジェクトでは、[Microsoft Open Source Code of Conduct (Microsoft オープン ソース倫理規定)](https://opensource.microsoft.com/codeofconduct/) が採用されています。詳細については、「[Code of Conduct FAQ (倫理規定の FAQ)](https://opensource.microsoft.com/codeofconduct/faq/)」を参照してください。また、その他の質問やコメントがあれば、[opencode@microsoft.com](mailto:opencode@microsoft.com) までお問い合わせください。



**Copyright (c) 2017 Microsoft.All rights reserved.**
