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
# EDUGraphAPI - Office 365 教育版代码示例 (ASP.NET MVC)

在此示例中，我们将向你展示如何通过 Graph API 与学校角色/名册数据以及可用的 O365 服务集成。 

O365 教育版租户中的学校数据通过 [Microsoft School Data Sync](http://sds.microsoft.com) 保持同步。  

**目录**
* [示例目标](#sample-goals)
* [先决条件](#prerequisites)
* [在 Azure Active Directory 中注册应用程序](#register-the-application-in-azure-active-directory)
* [在本地构建和调试](#build-and-debug-locally)
* [将示例部署到 Azure](#deploy-the-sample-to-azure)
* [理解代码](#understand-the-code)
* [[可选]在本地构建和调试 WebJob](#optional-build-and-debug-the-webjob-locally)
* [问题和意见](#questions-and-comments)
* [参与](#contributing)

## 示例目标

本示例演示：

* 调用 Graph API，包括：

  * [Microsoft Azure Active Directory Graph API](https://www.nuget.org/packages/Microsoft.Azure.ActiveDirectory.GraphClient/)
  * [Microsoft Graph API](https://www.nuget.org/packages/Microsoft.Graph/)

* 关联本地托管用户帐户和 Office 365 (Azure Active Directory) 用户帐户。 

  关联帐户后，用户可使用本地或 Office 365 帐户登录示例网站并使用它。

* 从 Office 365 教育版中获取学校、校区、教师和学生信息：

  * [Office 365 Schools REST API 参考](https://msdn.microsoft.com/office/office365/api/school-rest-operations)
  * [差异查询](https://msdn.microsoft.com/en-us/library/azure/ad/graph/howto/azure-ad-graph-api-differential-query)用于同步由 SyncData Web 作业在本地数据库中缓存的数据。

EDUGraphAPI 基于 ASP.NET MVC，并且还在此项目中使用 [ASP.NET Identity](https://www.asp.net/identity)。

## 先决条件

**部署和运行此示例需要**：
* 具有注册新应用程序和部署 Web 应用的权限的 Azure 订阅。
*已启用 Microsoft School Data Sync 的 O365 教育版租户
* 下列浏览器之一：Microsoft Edge、Internet Explorer 9、Safari 5.0.6、Firefox 5、Chrome 13 或这些浏览器的更高版本。
另外：本地部署/运行此示例需要下列内容：  
    * Visual Studio 2017（任何版本）。
	* 熟悉 C#、.NET Web 应用程序、JavaScript 编程和 Web 服务。

## 在 Azure Active Directory 中注册应用程序

1. 登录新的 Azure 门户：[https://portal.azure.com/](https://portal.azure.com/)。

2. 在页面右上角选择帐户，选择 Azure AD 租户：

   ![](Images/aad-select-directory.png)

3. 单击“**Azure Active Directory**” -> “**应用程序注册** -> “**+添加**”。

   ![](Images/aad-create-app-01.png)

4. 输入“**名称**”，并选择“**Web 应用/API**”作为“**应用程序类型**”。

   输入**登录 URL**：https://localhost:44311/

   ![](Images/aad-create-app-02.png)

   单击“**创建**”。

5. 完成后，应用程序在列表中显示。

   ![](/Images/aad-create-app-03.png)

6. 单击可查看详细信息。 

   ![](/Images/aad-create-app-04.png)

7. 如果设置窗口未显示，单击“**所有设置**”。

   * 单击“**属性**”，然后设置“**多租户**”至“**是**”。

     ![](/Images/aad-create-app-05.png)

     复制“**应用程序 ID**”，随后单击“**保存**”。

   * 单击“**必需的权限**”。添加以下权限：

     | API | 应用程序权限 | 委派权限 |
| ------------------------------ | ---------------------------------------- | ---------------------------------------- |
| Microsoft Graph | 读取所有用户的完整个人资料<br> 读取目录数据<br> 读取所有组 | 读取目录数据<br>以登录用户身份访问目录<br>让用户登录<br> 具备对用户可以访问的所有文件的完全访问权限<br> 具有对用户文件的完全访问权限<br> 读取不含成绩的用户课堂作业<br> 对不含成绩的用户课堂作业执行读取和写入操作<br> 读取用户的课堂作业及其成绩<br> 读取和写入用户的课堂作业及其成绩 |
| Microsoft Azure Active Directory | 读取目录数据 | 登录和读取用户个人资料<br>读取和写入目录数据 |

     ![](/Images/aad-create-app-06.png)

     ​

     **应用程序权限**

     | 权限 | 说明 | 需经过管理员同意 |
| ----------------------------- | ---------------------------------------- | ---------------------- |
| 读取所有用户的完整个人资料 | 允许应用在没有登录用户的情况下读取组织中其他用户的整套个人资料属性、组成员身份、下属和经理。| 是 |
| 读取目录数据 | 允许应用在没有登录用户的情况下读取组织目录中的数据（如用户、组和应用）。| 是 |

     **委派权限**

     | 权限 | 说明 | 需经过管理员同意 |
| -------------------------------------- | ---------------------------------------- | ---------------------- |
| 读取目录数据 | 允许应用读取组织目录中的数据（如用户、组和应用）。| 是 |
| 以登录用户身份访问目录 | 允许应用与登录用户对目录中的信息具有相同的访问权限。| 是 |
| 让用户登录 | 允许用户以其工作或学校帐户登录应用，并允许应用查看用户的基本个人资料信息。| 否 |
| 登录和读取用户个人资料 | 允许用户登录应用，并允许应用读取登录用户的个人资料。它还允许应用读取登录用户的基本公司信息。| 否 |
| 读取和写入目录数据 | 允许应用读取和写入组织目录中的数据（如用户和组）。它不允许应用删除用户或组，或重置用户密码。| 是 |

     ​

   * 单击“**密钥**”，随后添加新密钥：

     ![](Images/aad-create-app-07.png)

     单击“**保存**”，随后复制密钥“**值**”。 

   关闭“设置”窗口。

8. 单击“**清单**”。

   ![](Images/aad-create-app-08.png)

   将以下 JSON 插入到 **keyCredentials** 的数组中。

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

   单击“**保存**”。

   > 注意：此步骤将配置 Web 作业使用的证书。有关详细信息，请查看**应用程序身份验证流**部分。

## 在本地构建和调试

可使用已有的 Visual Studio 2017 版本打开此项目，也可以下载并安装 Community 版以在本地运行、构建和/或开发此应用程序。



调试 **EDUGraphAPI.Web**：

1. 在 **Web.config** 中配置 **appSettings**。 

   ![](Images/web-app-config.png)

   - **ida:ClientId**：使用之前创建的应用注册客户端 ID。
   - **ida:ClientSecret**：使用之前创建的应用注册密钥值。
   - **SourceCodeRepositoryURL**：使用你的分支的存储库 URL。


2. 将 **EDUGraphAPI.Web** 设置为启动项目，然后按 F5。 

## 将示例部署到 Azure

**GitHub 身份验证**

1. 生成令牌

   - 在 Web 浏览器中打开 https://github.com/settings/tokens。
   - 登录分叉此存储库的 GitHub 账户。
   - 单击“**生成令牌**”
   - 在“**令牌说明**”文本框中输入数值。
   - 选择下列内容（选择内容应与以下截屏匹配）：
        - repo (all) -> repo:status, repo_deployment, public_repo
        - admin:repo_hook -> read:repo_hook

   ![](Images/github-new-personal-access-token.png)

   - 单击“**生成令牌**”
   - 复制令牌

2. 在 Azure 资源浏览器中添加 GitHub 令牌至 Azure

   - 在 Web 浏览器中打开 https://resources.azure.com/providers/Microsoft.Web/sourcecontrols/GitHub。
   - 使用你的 Azure 帐户登录。
   - 已选择正确的 Azure 订阅。
   - 选择“**读/写**”模式。
   - 单击“**编辑**”。
   - 粘贴令牌到“**令牌参数**”中。

   ![](Images/update-github-token-in-azure-resource-explorer.png)

   - 单击“**放入**”

**从 GitHub 部署 Azure 组件**

1. 检查确保该构建正在传递 VSTS Build。

2. 分叉此存储库至你的 GitHub 账户。

3. 单击“部署到 Azure”按钮：

   [![部署到 Azure](http://azuredeploy.net/deploybutton.png)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FOfficeDev%2FO365-EDU-AspNetMVC-Samples%2Fmaster%2Fazuredeploy.json)

4. 填写部署页面中的数值并选中“**我同意上述条款和条件**”复选框。

   ![](Images/azure-auto-deploy.png)

   * **资源组**：建议新建一个资源组。

   * **网站名称**：请输入名称。如 EDUGraphAPICanviz 或 EDUGraphAPI993。

     > 注意：如果输入的名称已被占用，将会出现一些验证错误：
     >
     > ![](Images/azure-auto-deploy-validation-errors-01.png)
     >
     > 单击将获得更多信息，如其他资源组/订阅中的存储帐户。
     >
     > 在这种情况中，请使用其他名称。

   * **源代码存储库 URL**：使用分支的存储库名称替换 <YOUR REPOSITORY>。

   * **源代码手动集成**：**false**，因为正在从自有分支部署。

   * **Client Id**：使用之前创建的应用注册客户端 ID。

   * **Client Secret**：使用之前创建的应用密钥值。

   * 单击“**我同意上述条款和条件**”。

5. 单击“**购买**”。

**添加回复 URL 至应用程序注册**

1. 部署后，打开资源组：

   ![](Images/azure-resource-group.png)

2. 单击 Web 应用。

   ![](Images/azure-web-app.png)

   复制 URL 并更改架构为 **https**。这是重播 URL，将在下一步中使用。

3. 导航至新 Azure 门户中的应用程序注册，随后打开设置窗口。

   添加回复 URL：

   ![](Images/aad-add-reply-url.png)

   > 注意：如果要在本地调试示例，确保回复 URL 是 https://localhost:44311/。

4. 单击“**保存**”。

## 了解代码

### 简介

**解决方案组件图**

![](Images/solution-component-diagram.png)

解决方案的顶层包含 Web 应用程序和 WebJob 控制台应用程序。

中间层包含两个类库项目。 

底层包含三个数据源。

**EDUGraphAPI.Web**

此 Web 应用程序基于 ASP.NET MVC 项目模板，该模板中已选中“**个人用户帐户**”选项。 

![](Images/mvc-auth-individual-user-accounts.png)

以下文件是由 MVC 模板创建的，仅进行了次要更改：

1. **/App_Start/Startup.Auth.Identity.cs**（原始名称为 Startup.Auth.cs）
2. **/Controllers/AccountController.cs**

此示例项目使用 **[ASP.NET Identity](https://www.asp.net/identity)** 和 **[Owin](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/owin)**。这两种技术使不同的身份验证方法能够轻松共存。除了 ASP.NET MVC 之外，熟悉这些组件对于理解此示例也非常重要。

下面是此 Web 项目中使用的重要类文件：

| 文件 | 说明 |
| --------------------------------- | ---------------------------------------- |
| /App_Start/Startup.Auth.AAD.cs | 与 Azure Active Directory 身份验证集成 |
| /Controllers/AdminController.cs | 包含管理操作：<br>管理员许可、管理链接的帐户和安装应用。|
| /Controllers/LinkController.cs | 包含链接 AD 和本地用户帐户的操作 |
| /Controllers/SchoolsController.cs | 包含呈现教育数据的操作 |

此 Web 应用程序是**多租户应用**。在 AAD 中，我们启用选项：

![](Images/app-is-multi-tenant.png)

任何 Azure Active Directory 租户中的用户可访问此应用程序。由于此应用使用某些应用程序权限，租户的管理员应首先注册（同意）。否则，用户将会遇到错误：

![](Images/app-requires-admin-to-consent.png)

有关详细信息，参见“[使用 Azure AD & OpenID Connect 生成多租户 SaaS Web 应用程序](https://azure.microsoft.com/en-us/resources/samples/active-directory-dotnet-webapp-multitenant-openidconnect/)”。

**EDUGraphAPI.SyncData**

这是用于同步用户数据的 WebJob。在 **Functions.SyncUsersAsync** 方法中，使用 EDUGraphAPI.Common 项目中的 **UserSyncService**。

创建该项目是为了演示差异查询。有关详细信息，请查看[差异查询](#differential-query)部分。

**EDUGraphAPI.Common**

此类库项目使用 **EDUGraphAPI.Web** 和 **EDUGraphAPI.SyncData**。 

下表显示了该项目中的文件夹：

| 文件夹 | 说明 |
| ------------------ | ---------------------------------------- |
| /Data | 包含 ApplicationDbContext 和实体类 |
| /DataSync | 包含由 EDUGraphAPI.SyncData WebJob 使用的 UserSyncSerextensionsvice 类 |
| /DifferentialQuery | 包含 DifferentialQueryService 类，它用于发送差异查询和解析结果。|
| /Extensions | 包含许多用于简化编码的扩展方法，使代码易于读取 |
| /Utils | 包含广泛使用的 AuthenticationHelper.cs 类 |

**Microsoft.Education**

此项目封装了 **[Schools REST API](https://msdn.microsoft.com/en-us/office/office365/api/school-rest-operations)** 客户端。此项目的核心类是 **EducationServiceClient**。

### 数据访问和数据模型

ASP.NET Identity 使用 [Entity Framework Code First](https://msdn.microsoft.com/en-us/library/jj193542(v=vs.113).aspx) 来实现所有长久性机制。它使用 [Microsoft.AspNet.Identity.EntityFramework](https://www.nuget.org/packages/Microsoft.AspNet.Identity.EntityFramework/) 程序包。 

在此示例中，已创建 **ApplicationDbContext** 来访问数据库。它继承自上面提到的 NuGet 程序包中定义的 **IdentityDbContext**。

下面是此示例中使用的重要数据模型（及其重要属性）：

**ApplicationUsers**

继承自 **IdentityUser**。 

| 属性 | 说明 |
| ------------- | ---------------------------------------- |
| 组织 | 用户的租户。对于本地未链接的用户，其值为 null |
| O365UserId | 用于链接 Office 365 帐户 |
| O365Email | 已链接的 Office 365 帐户的电子邮件 |
| JobTitle | 用于演示差异查询 |
| Department | 用于演示差异查询 |
| Mobile | 用于演示差异查询 |
| FavoriteColor | 用于演示本地数据 |

**组织**

此表中的行表示 AAD 中的租户。

| 属性 | 说明 |
| ---------------- | ------------------------------------ |
| TenantId | 租户的 Guid |
| Name | 租户的名称 |
| IsAdminConsented | 租户是否经过管理员的同意 |

### 管理员许可流

仅限应用的权限始终需要租户管理员的同意。如果应用程序请求仅限应用的权限，当用户尝试登录应用程序时，将会显示一条错误消息，指出该用户无法同意。

有些委托的权限也需要租户管理员的同意。例如，若要能够以登录用户身份写回 Azure AD，就需要租户管理员的同意。与仅限应用的权限一样，如果普通用户尝试登录请求委托权限的应用程序，而该权限需要管理员同意，则应用程序将会收到错误。权限是否需要管理员同意是由发布资源的开发人员决定的，可以在该资源的文档中找到相关信息。

如果应用程序使用需要管理员同意的权限，则需要提供某种表示，例如可供管理员发起操作的按钮或链接。应用程序针对此操作发送的请求是一个普通的 OAuth2/OpenID Connect 授权请求，但此请求也包含 `prompt=admin_consent` 查询字符串参数。在管理员已同意且系统已在客户的租户中创建服务主体之后，后续登录请求就不再需要 `prompt=admin_consent` 参数。由于管理员已确定可接受请求的权限，因此从该时间点之后，将不再提示租户中的任何其他用户同意。



### 身份验证流

此项目中有 4 个身份验证流。

前 2 个流（本地登录/O365登录）能够让用户使用本地账户或 Office 365 账户登录，随后关联至其它类型账户。此程序在 LinkController 中实现。

**本地登录身份验证流**

![](Images/auth-flow-local-login.png)

**O365登录身份验证流**

![](Images/auth-flow-o365-login.png)

**管理员登录身份验证流**

该流显示管理员如何登录系统并执行管理操作。

使用 Office 365 帐户登录后，系统提示要求管理员关联至本地帐户。此步骤不需要，可以忽略。 

如前文所述，Web 应用是使用部分应用程序权限的多租户应用，因此租户管理员必须首先对租户进行许可。  

该流在 AdminController 中实现。 

![](Images/auth-flow-admin-login.png)

**应用程序身份验证流**

该流在 SyncData WebJob 中实现。

![](Images/auth-flow-app-login.png)

使用 X509 证书。有关详细信息，请查看以下链接：

* [守护程序或服务器应用程序到 Web API](https://docs.microsoft.com/en-us/azure/active-directory/active-directory-authentication-scenarios#daemon-or-server-application-to-web-api)
* [使用证书在守护程序应用中对 Azure AD 进行身份验证](https://azure.microsoft.com/en-us/resources/samples/active-directory-dotnet-daemon-certificate-credential/)
* [在 Office 365 中构建服务和守护程序应用](https://msdn.microsoft.com/en-us/office/office365/howto/building-service-apps-in-office-365)

### 两种类型的 Graph API

此示例中使用两种不同的 Graph API：

|
| [Azure AD Graph API](https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-graph-api) | [Microsoft Graph API](https://graph.microsoft.io/) |
| ------------ | ---------------------------------------- | ---------------------------------------- |
| 说明 |Azure Active Directory 图形 API 通过 REST API 终结点提供对 Azure Active Directory 的编程访问权限。Apps 可使用 Azure AD Graph API 在目录数据和目录对象上执行创建、读取、更新和删除 (CRUD) 操作，如用户、组合和组织联系人 | 所有包含 Microsoft 服务中 API 的统一 API，如 Outlook、OneDrive、OneNote、 Planner 和 Office Graph，使用单独的令牌通过单独的终结点进行访问。 |
| 客户端 | 安装包 [Microsoft.Azure.ActiveDirectory.GraphClient](https://www.nuget.org/packages/Microsoft.Azure.ActiveDirectory.GraphClient/) | 安装包 [Microsoft.Graph](https://www.nuget.org/packages/Microsoft.Graph/) |
| 终结点 | https://graph.windows.net | https://graph.microsoft.com |
| API 资源管理器 | https://graphexplorer.cloudapp.net/ | https://graph.microsoft.io/graph-explorer |

在此示例中，我们使用以下基于公共接口的类来演示 API 之间的关系：  

![](Images/class-diagram-graphs.png)

**IGraphClient** 接口定义了两种方法：**GeCurrentUserAsync** 和 **GetTenantAsync**。

**MSGraphClient** 通过 Microsoft Graph 客户端库实现 **IGraphClient** 接口。

接口和图形客户端类驻留在 Web 应用的 **/Services/GraphClients** 文件夹中。下面突出显示了一些代码，用于演示如何获取用户和租户。

**Microsoft Graph** \- MSGraphClient.cs

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

请注意，在应用注册设置中，各 Graph API 权限单独配置：

![](/Images/aad-create-app-06.png) 

### Office 365 教育版 API

[Office 365 教育版 API](https://msdn.microsoft.com/office/office365/api/school-rest-operations) 可帮助用户从由 Microsoft School Data Sync 同步至云端的任何 Office 365 租户中提取数据。这些结果提供有关学校、校区、教师、学生和名册信息。Schools REST API 为 Office 365 教育版租户中的学校实体提供访问权限。

在此示例中，已创建用于封装 Office 365 教育版 API 的 **Microsoft.Education** 类库项目。 

**EducationServiceClient** 是库的核心类。借助它，我们可以轻松获取教育数据。

**获取学校**

~~~c#
public async Task<School[]> GetSchoolsAsync()
{
    var schools = await HttpGetArrayAsync<EducationSchool>("education/schools");
    return schools.ToArray();

}
~~~



**获取校区**

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


以下是显示教育数据的示例应用的一些屏幕截图。

![](Images/edu-schools.png)



![](Images/edu-classes.png)

![](Images/edu-class.png)

在 **EducationServiceClient**，创建了带有 HttpGet 前缀的三种专用方法来简化 REST API 的调用。

* **HttpGetAsync**：将 http GET 请求发送到目标终结点，并返回 JSON 响应字符串。持有者身份验证标头中包含访问令牌。
* **HttpGetObjectAsync<T>**：将 HttpGetAsync 返回的 JSON 字符串反序列化为目标类型 T，然后返回结果对象。
* **HttpGetArrayAsync<T>**：将 HttpGetAsync 返回的 JSON 字符串反序列化为目标数组类型 T[]，然后返回数组。

### 差异查询

[差异查询](https://msdn.microsoft.com/en-us/Library/Azure/Ad/Graph/howto/azure-ad-graph-api-differential-query)请求返回两次连续请求之间的时间内对指定实体所做的所有更改。例如，如果在上次差异查询请求之后的一小时发出差异查询请求，则只返回在该小时内所做的更改。将租户目录数据与应用程序的数据存储同步时，此功能特别有用。

相关代码位于 **EDUGraphAPI.Common** 项目的以下两个文件夹中：

* **/DifferentialQuery**：包含用于发送差异查询和解析差异结果的类。
* **/DataSync**：包含用于演示如何同步用户的类。

> 请注意，**DifferentialQuery** 文件夹中的类使用某些高级 .NET 技术。请重点关注这些类的用法，而不是其实现详细信息。

为了同步用户，我们定义了用户类：

~~~c#
public class User
{
    public string ObjectId { get; set; }
    public virtual string JobTitle { get; set; }
    public virtual string Department { get; set; }
    public virtual string Mobile { get; set; }
}
~~~

请注意，可更改的属性 *JobTitle*、*Department* 和 *Mobile* 是虚拟属性。**DifferentialQuery** 文件夹中的类将为用户类型创建一个代理类型，并覆盖这些虚拟属性以进行更改跟踪。

在 **UserSyncService** 类中，我们演示了如何使用 **DifferentialQueryService** 发送差异查询并获取差异结果。

```c#
var differentialQueryService = new DifferentialQueryService(/**/);
DeltaResult<Delta<User>> result = await differentialQueryService.QueryAsync<User>(url);
```
以及如何使用增量结果来更新（或删除）本地数据库中的用户：

~~~c#
foreach (var differentialUser in result.Items)
    await UpdateUserAsync(differentialUser);
//...
private async Task UpdateUserAsync(Delta<User> differentialUser) { /**/ }
~~~

**DataSyncRecord** 数据模型用于永久性 deltaLinks。

以下是由 SyncData WebJob 生成的日志：

![](Images/sync-data-web-job-log.png) 

### 筛选器

在 Web 项目的 **/Infrastructure** 文件夹中，有多个 FilterAttributes。

**EduAuthorizeAttribute**

这是从 AuthorizeAttribute 继承的授权筛选器。

创建它是为了让 Web 应用在多重身份验证方法方案中将用户重定向到正确的登录页面。

我们重写了 **HandleUnauthorizedRequest** 方法以将用户重定向到 /Account/Login：

~~~c#
protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
{
    filterContext.Result = new RedirectResult("/Account/Login");
}
~~~

**HandleAdalExceptionAttribute**

**AuthenticationHelper** 类公开许多用于返回访问令牌或 API 客户端实例的方法。大多数方法都在内部调用 **[AuthenticationContext.AcquireTokenSilentAsync](https://msdn.microsoft.com/en-us/library/mt473642.aspx)**。通常情况下，**AcquireTokenSilentAsync** 会成功获取访问令牌，因为令牌会通过 **ADALTokenCache** 缓存到数据库中。 

在某些情况下，例如缓存的令牌已过期或请求了新的资源令牌，**AcquireTokenSilentAsync** 将引发 **AdalException**。需要使用 **HandleAdalExceptionAttribute** 来处理 **AdalException**，并将用户导航到身份验证终结点以获取新令牌。

在某些情况下，我们会通过调用以下内容将用户直接重定向到身份验证终结点：

~~~c#
filterContext.HttpContext.GetOwinContext().Authentication.Challenge(
   new AuthenticationProperties { RedirectUri = requestUrl },
   OpenIdConnectAuthenticationDefaults.AuthenticationType);
~~~

在其他情况下，我们希望向用户显示以下页面，以告知用户执行重定向的原因，尤其是对于使用本地帐户登录的用户。

![](Images/web-app-login-o365-required.png)

我们使用切换来控制此操作。切换值通过以下方式检索：

~~~c#
//public static readonly string ChallengeImmediatelyTempDataKey = "ChallengeImmediately";
var challengeImmediately = filterContext.Controller.TempData[ChallengeImmediatelyTempDataKey];
~~~

如果值为 true，我们会立即将用户重定向到身份验证终结点。否则，将先显示上面的页面，然后用户可单击“登录”按钮继续。

**LinkedOrO365UsersOnlyAttribute**

这是另一个授权筛选器。使用它，我们只允许已链接的用户或 Office 365 用户访问受保护的控制器/操作。

~~~c#
protected override bool AuthorizeCore(HttpContextBase httpContext)
{
    var applicationService = DependencyResolver.Current.GetService<ApplicationService>();
    var user = applicationService.GetUserContext();
    return user.AreAccountsLinked || user.IsO365Account;
}
~~~

对于未经授权的用户，我们将向他们显示 NoAccess 页面：

~~~c#
protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
{
    filterContext.Result = new ViewResult { ViewName = "NoAccess" };
}
~~~

到目前为止，它仅用于 **SchoolsController**。

### 主类

**Microsoft.Education**

* `EducationServiceClient`：该类的实例处理构建请求，将其发送到 Office 365 教育版 API，并处理响应。

  | 方法 | 说明 |
| ------------------- | ---------------------------------------- |
| GetSchoolsAsync | 获取 Azure Active Directory 租户中存在的所有学校 |
| GetSchoolAsync | 使用对象 ID 获取学校 |
| GetAllSectionsAsync | 获取学校内的各校区 |
| GetMySectionsAsync | 获取我所在的学校校区 |
| GetSectionAsync | 使用对象 ID 获取校区 |
| GetMembersAsync | 获取学校内的成员 |
| GetStudentAsync | 获取当前以学生身份登录的用户 |
| GetTeacherAsync | 获取当前以教师身份登录的用户 |

**EDUGraphAPI.Common**

* **`Data.ApplicationUser`**：该类的实例表示用户。

* **`Data.Organization`**：该类的实例表示 Azure AD 中的租户。 

* **`Data.ApplicationDbContext`**：由 Entity Framework 使用的 DbContext 类，它继承自 `IdentityDbContext<ApplicationUser>`。

* **`DataSync.User`**：该类的实例表示 Azure AD 中的用户。请注意，用于跟踪更改的属性是虚拟属性。

* **`DataSync.UserSyncService`**：该类的实例使用差异查询处理本地数据库中的用户同步。调用 `SyncAsync` 方法以开始同步用户。

* **`DifferentialQuery.DifferentialQueryService`**：该类的实例处理构建请求，将其发送到服务终结点，并处理响应。使用 deltaLink 调用 `QueryAsync` 方法以开始差异查询。差异结果将由 `DeltaResultParser` 类转换为 `DeltaResult<Delta<TEntity>>`。

* **`Utils.AuthenticationHelper`**：静态帮助程序类，用于获取访问令牌、身份验证结果、身份验证上下文和服务客户端实例。

  | 方法 | 说明 |
| -------------------------------------- | ---------------------------------------- |
| GetActiveDirectoryClientAsync | 获取 ActiveDirectoryClient 的实例 |
| GetGraphServiceClientAsync | 获取 GraphServiceClient 的实例 |
| GetEducationServiceClientAsync | 获取 EducationServiceClient 的实例 |
| GetActiveDirectoryClient | 从指定的 AuthenticationResult 中获取 ActiveDirectoryClient 的实例 |
| GetGraphServiceClient | 从指定的 AuthenticationResult 中获取 GraphServiceClient 的实例 |
| GetAccessTokenAsync | 获取指定资源的访问令牌 |
| GetAuthenticationResult | 获取指定资源的 AuthenticationResult |
| GetAuthenticationContext | 获取 AuthenticationContext 的实例 |
| GetAuthenticationResultAsync | 从指定的授权代码中获取 AuthenticationResult |
| GetAppOnlyAccessTokenForDaemonAppAsync | 获取守护程序应用的“仅限应用”访问令牌 |

  上面的大多数方法都有一个称为“权限”的参数。其类型为`权限`，即包含两个已定义值的枚举类型：

  * `委派`：客户端以已登录用户的身份访问 Web API。
  * `应用程序`：客户端直接访问 Web API（没有用户上下文）。这种类型的权限需要管理员同意。

* **`Utils.AuthenticationHelper`**：用于构建授权 URL 的静态类。`GetUrl` 是该类中定义的唯一方法。

* **`Constants`**：该静态类包含应用设置的值和其他常量值。

**EDUGraphAPI.Web**

* **`Controllers.AccountController`**：包含用户注册、登录和更改密码的操作。

* **`Controllers.AdminController`**：实现**管理员登录身份验证流**。有关详细信息，请查看[身份验证流](#authentication-flows)部分。

* **`Controllers.LinkController`**：实现**本地/O365 登录身份验证流**。有关详细信息，请查看[身份验证流](#authentication-flows)部分。

* **`Controllers.SchoolsController`**：包含显示学校和课程的操作。`SchoolsService` 类主要由此控制器使用。有关详细信息，请查看 [Office 365 教育版 API](#office-365-education-api) 部分。

* **`Infrastructure.EduAuthorizeAttribute`**：让 Web 应用在多重身份验证方法方案中将用户重定向到正确的登录页面。有关详细信息，请查看[筛选器](#filters)部分。

* **`Infrastructure.HandleAdalExceptionAttribute`**：处理 AdalException 并将用户导航到授权终结点或 /Link/LoginO365Required。有关详细信息，请查看[筛选器](#filters)部分。

* **`Infrastructure.LinkedOrO365UsersOnlyAttribute`**：只允许已链接的用户或 Office 365 用户访问受保护的控制器/操作。有关详细信息，请查看[筛选器](#filters)部分。

* **`Models.UserContext`**：已登录用户的上下文。

* **`Services.GraphClients.AADGraphClient`**：通过 Azure AD Graph API 实现 `IGraphClient` 接口。有关详细信息，请查看[两种类型的 Graph API](#two-kinds-of-graph-api) 部分。

* **`Services.GraphClients.MSGraphClient`**：通过 Microsoft Graph API 实现 `IGraphClient` 接口。有关详细信息，请查看[两种类型的 Graph API](#two-kinds-of-graph-api) 部分。

* **`Services.ApplicationService`**：该类的实例处理获取/更新用户/组织。

  | 方法 | 说明 |
| ------------------------------- | ---------------------------------------- |
| CreateOrUpdateOrganizationAsync | 创建或更新组织 |
| GetAdminContextAsync | 获取当前管理员的上下文 |
| GetCurrentUser | 获取当前用户 |
| GetCurrentUserAsync | 获取当前用户 |
| GetUserAsync | 通过 ID 获取用户 |
| GetUserContext | 获取当前用户的上下文 |
| GetUserContextAsync | 获取当前用户的上下文 |
| GetLinkedUsers | 通过指定的筛选器获取已链接的用户 |
| IsO365AccountLinkedAsync | 指定的 O365 帐户是否与本地帐户关联 |
| SaveSeatingArrangements | 保存座位安排 |
| UnlinkAccountsAsync | 取消链接指定的帐户 |
| UnlinkAllAccounts | 取消链接指定租户中的所有帐户 |
| UpdateLocalUserAsync | 使用 O365 用户和租户信息更新本地用户 |
| UpdateOrganizationAsync | 更新组织 |
| UpdateUserFavoriteColor | 更新当前用户喜欢的颜色 |

* **`Services.SchoolsService`**：用于获取教育数据的服务类。

  | 方法 | 说明 |
| ------------------------------- | ---------------------------------------- |
| GetSchoolsViewModelAsync | 获取 SchoolsViewModel |
| GetSchoolUsersAsync | 获取指定学校的教师和学生信息 |
| GetSectionsViewModelAsync | 获取指定学校的 SectionsViewModel |
| GetSectionDetailsViewModelAsync | 获取指定校区的 SectionDetailsViewModel |
| GetMyClasses | 获取我的课程 |

**EDUGraphAPI.SyncData**

* **`Functions`**：包含 `SyncUsersAsync` 方法，该方法会定期执行以同步用户数据。
* **`Program`**：包含用于配置和启动 WebJob 主机的 `Main` 方法。

## [可选]在本地构建和调试 WebJob

调试 **EDUGraphAPI.SyncData**：

1. 在 Azure 中创建存储帐户并获取连接字符串。
   > 注意：
   - 与 [Azure WebJobs SDK](https://github.com/Azure/azure-webjobs-sdk) V2 关联后，将支持使用 Azure 存储模拟器进行本地调试。有关详细信息，请参阅[支持 Azure 存储模拟器](https://github.com/Azure/azure-webjobs-sdk/issues/53)。当已发布的 Web 作业使用相同的存储帐户在 Azure 中运行时，**不**建议进行本地调试。有关详细信息，请查看[此问题](http://stackoverflow.com/questions/42020647/what-happened-when-using-same-storage-account-for-multiple-azure-webjobs-dev-li)。

2. 配置 **App.config**：

   ![](Images/webjob-app-config.png)

   - **连接字符串**：
     - **AzureWebJobsDashboard**：使用在上一步中获得的连接字符串。
     - **AzureWebJobsStorage**：使用在上一步中获得的连接字符串。
   - **应用设置**：
     - *ida:ClientId*\*：使用之前创建的应用注册客户端 ID。

3. 将 **EDUGraphAPI.SyncData** 设置为启动项目，然后按 F5。 

## 问题和意见

* 如果你在运行此示例时遇到任何问题，请[记录问题](https://github.com/OfficeDev/O365-EDU-AspNetMVC-Samples/issues)。
* 与GraphAPI开发相关的问题一般应发布到 [Stack Overflow](http://stackoverflow.com/questions/tagged/office-addins)。请务必将提问或评论用 [ms-graph-api] 标记。 

## 参与

我们鼓励你参与我们的示例。有关如何继续的指南，请参阅我们的[参与指南](/Contributing.md)。

此项目已采用 [Microsoft 开放源代码行为准则](https://opensource.microsoft.com/codeofconduct/)。有关详细信息，请参阅[行为准则常见问题解答](https://opensource.microsoft.com/codeofconduct/faq/)。如有其他任何问题或意见，也可联系 [opencode@microsoft.com](mailto:opencode@microsoft.com)。



**版权所有 (c) 2017 Microsoft。保留所有权利。**
