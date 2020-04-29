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
# EDUGraphAPI – Exemplo de Código Educacional do Office 365 (ASP.NET MVC)

Neste exemplo, vamos mostrar como integrar com dados da escola/lista de funções, além de serviços do O365 através da API do Graph. 

Os dados da escola são mantidos em sincronia nos locatários do O365 Education pelo [Microsoft School Data Sync](http://sds.microsoft.com).  

**Sumário**
* [Principais Metas](#sample-goals)
* [Pré-requisitos](#prerequisites)
* [Registrar o aplicativo no Azure Active Directory](#register-the-application-in-azure-active-directory)
* [Criar e depurar localmente](#build-and-debug-locally)
* [Implantar o aplicativo no Azure](#deploy-the-sample-to-azure)
* [Compreender o código](#understand-the-code)
* [[Opcional] Criar e depurar o WebJob local](#optional-build-and-debug-the-webjob-locally)
* [Perguntas e comentários](#questions-and-comments)
* [Contribuindo](#contributing)

## Metas de exemplo

O exemplo demonstra:

* Chamando APIs do Graph, incluindo:

  * [Microsoft Azure Active Directory Graph](https://www.nuget.org/packages/Microsoft.Azure.ActiveDirectory.GraphClient/)
  * [API do Microsoft Graph](https://www.nuget.org/packages/Microsoft.Graph/)

* Vincular contas de usuário gerenciadas localmente e contas de usuário do Office 365 (Azure Active Directory). 

  Após vincular as contas, os usuários podem usar as contas locais ou do Office 365 para entrar no site de exemplo e usá-lo.

* Obter escolas, seções, professores e alunos do Office 365 Education:

  * [Referência da API REST do Office 365 Schools](https://msdn.microsoft.com/office/office365/api/school-rest-operations)
  * Uma consulta [diferencial](https://msdn.microsoft.com/en-us/library/azure/ad/graph/howto/azure-ad-graph-api-differential-query) é usada para sincronizar os dados armazenados em cache em um banco de dados local pelo SyncData Web Job.

O EDUGraphAPI baseia-se no ASP.NET MVC e a[identidade de ASP.NET](https://www.asp.net/identity) também é usada neste projeto.

## Pré-requisitos

A **implantação e execução desse exemplo requer**:
* Uma assinatura do Azure com permissões para registrar um novo aplicativo e implantação do aplicativo Web.
* um locatário do O365 Education com o Microsoft School Data Sync habilitado em um dos seguintes navegadores:
* Edge, Internet Explorer 9, Safari 5.0.6, Firefox 5, Chrome 13 ou uma versão mais recente de um desses navegadores.
Além disso: O desenvolvimento/execução desse exemplo local requer o seguinte:  
    * Visual Studio 2017 (qualquer edição).
	* Familiaridade com C#, aplicativos .NET Web, programação e serviços da Web.

## Registrar o aplicativo no Azure Active Directory

1. Entre no portal do Azure: [https://portal.azure.com](https://portal.azure.com/).

2. Escolha seu locatário do Azure AD ao selecionar sua conta no canto superior direito da página:

   ![](Images/aad-select-directory.png)

3. Clique em **Azure Active Directory** -> **Registros de aplicativo** -> **+Adicionar**.

   ![](Images/aad-create-app-01.png)

4. Insira um **Nome**, e selecione **Aplicativo Web / API** como **Tipo de Aplicativo**.

   Insira uma **URL de Logon**: https://localhost:44311/

   ![](Images/aad-create-app-02.png)

   Clique em **Criar**.

5. Depois de concluído, o aplicativo será exibido na lista.

   ![](/Images/aad-create-app-03.png)

6. Clique nele para exibir os detalhes. 

   ![](/Images/aad-create-app-04.png)

7. Clique em **Todas configurações**, se a janela de configuração não aparecer.

   * Clique em **Propiedades**, em seguida, defina **Multilocatário** para **Sim**.

     ![](/Images/aad-create-app-05.png)

     Copie aparte o **ID do aplicativo**, em seguida, clique em **Salvar**.

   * Clique em **Permissões necessárias**. Adicione as seguintes permissões:

     | API | Permissões de Aplicativo | Permissões Delegadas |
| ------------------------------ | ----------------------- | ---------------------------------------- |
| Microsoft Graph || Ler os perfis completos de todos os usuários<br> Ler dados do diretório<br> Ler todos os grupos | Ler dados de diretório<br>Acessar o diretório como o usuário conectado<br>Conectar os usuários<br> Ter acesso total a todos os arquivos que o usuário pode acessar<br> Ter acesso total aos arquivos do usuário<br> Ler as tarefas de classe sem notas dos usuários<br> Ler e gravar as tarefas de classe sem notas dos usuários<br> Ler as atribuições de classe dos usuários e suas notas<br> Ler e gravar as atribuições de classe dos usuários e suas notas |
| Active Directory do Microsoft Azure | Ler dados do diretório | Entrar e ler perfil do usuário<br>Ler e gravar dados de diretório |

     ![](/Images/aad-create-app-06.png)

     ​

     **Permissões de Aplicativo**

     | Permissão | Descrição | Consentimento do administrador necessário |
| ----------------------------- | ---------------------------------------- | ---------------------- |
| Leia os perfis completos de todos os usuários | Permite que o aplicativo Leia todo o conjunto de propriedades de perfil, membro de grupo, relatórios e gerentes de outros usuários da organização, sem um usuário conectado. | Sim |
| Ler dados do diretório | Permite que o aplicativo leia dados no diretório da sua organização, como usuários, grupos e aplicativos, sem um usuário conectado. | Sim |

     **Permissões Delegadas**

     | Permissão | Descrição | Consentimento do administrador necessário |
| -------------------------------------- | ---------------------------------------- | ---------------------- |
| Lê dados do diretório | Permite que o aplicativo leia dados no diretório da sua organização, como usuários, grupos e aplicativos. | Sim |
| Acessar o diretório como o usuário conectado | Permite que o aplicativo tenha o mesmo acesso às informações no diretório que o usuário conectado. | Sim |
| Entrada de usuários | Permite que os usuários entrem no aplicativo com contas corporativas ou de estudante e permite que o aplicativo veja informações básicas de perfil de usuário. | Não |
| Entrar e ler perfil do usuário | Permite que os usuários entrem no aplicativo e permite que o aplicativo leia o perfil dos usuários conectados. Ele também permite que o aplicativo leia informações básicas da empresa de usuários conectados. | Não |
| Ler e gravar dados de diretório | Permite que o aplicativo Leia e grave dados no diretório da sua organização, como usuários e grupos. Não permite ao aplicativo excluir usuários ou grupos, ou redefinir senhas de usuário. | Sim |

     ​

   * Clique em **Chaves**, em seguida, adicione uma nova:

     ![](Images/aad-create-app-07.png)

     Clique em **Salvar**, em seguida, copie aparte o **VALOR** da chave. 

   Feche a janela de configurações.

8. Clique em **Manifesto**.

   ![](Images/aad-create-app-08.png)

   Insira o seguinte JSON na matriz de **keyCredentials**.

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

   Clique em **Salvar**.

   > Observação: esta etapa configura a certificação usada por um trabalho da Web. Marque a seção **Aplicativo de fluxo de autenticação** para obter mais detalhes.

## Criar e depurar localmente

Esse projeto pode ser aberto com a edição do Visual Studio 2017, ou baixe e instale a edição da Comunidade para executar, criar e/ou desenvolver esse aplicativo localmente.



Depurar o **EDUGraphAPI.Web**:

1. Configure **appSettings** em **Web.config**. 

   ![](Images/web-app-config.png)

   - **ida:ClientId**\*: usa a ID do cliente do registro do aplicativo criado anteriormente.
   - **ida.clientSecret**: use o valor da chave do registro do aplicativo criado anteriormente.
   - **SourceCodeRepositoryURL**: use a URL do repositório da bifurcação.


2. Defina **EDUGraphAPI.Web** como projeto inicial e pressione F5. 

## Implantar o exemplo no Azure

**Autorização do GitHub**

1. Gerar token

   - Abra https://github.com/settings/tokens no seu navegador da Web.
   - Entre na sua conta do GitHub na qual você bifurca esse repositório.
   - Clique em **Gerar token**.
   - Inserir um valor na caixa de texto da **descrição do Token**
   - Selecione os seguintes (as seleções devem corresponder à captura de tela abaixo):
        - repositórios (tudo)-> repositório: status, repo_deployment, public_repo
        - admin:repo_hook -> read:repo_hook

   ![](Images/github-new-personal-access-token.png)

   - Clique em **Gerar token**.
   - Copiar o token

2. Adicionar o token GitHub ao Azure no Gerenciador de recursos do Azure

   - Abra https://resources.azure.com/providers/Microsoft.Web/sourcecontrols/GitHub no seu navegador da Web.
   - Entre com sua conta do Azure.
   - A assinatura do Azure está correta.
   - Marque modo**Leitura/gravação**.
   - Clique em **Editar**.
   - Cole o token no **token de parâmetro**.

   ![](Images/update-github-token-in-azure-resource-explorer.png)

   - Clique em **Colocar**

**Implantar os componentes do Azure do GitHub**

1. Verifique se a compilação está passando pela criação do VSTS.

2. Bifurque esse repositório para a sua conta do GitHub.

3. Clique no botão implantar no Azure:

   [![Implantar no Azure](http://azuredeploy.net/deploybutton.png)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FOfficeDev%2FO365-EDU-AspNetMVC-Samples%2Fmaster%2Fazuredeploy.json)

4. Preencha os valores na página de implantação e marque a caixa de seleção **Concordo com os termos e condições descritos acima**.

   ![](Images/azure-auto-deploy.png)

   * **Grupo de recursos**: Sugerimos que você crie um novo grupo.

   * **Nome do site**: forneça um nome. Como EDUGraphAPICanviz ou EDUGraphAPI993.

     > Observação: Se o nome digitado já foi usado, ocorrerá alguns erros de validação:
     >
     > ![](Images/azure-auto-deploy-validation-errors-01.png)
     >
     > Clique nele para obter mais detalhes, por exemplo, se a conta de armazenamento já está em outro grupo de recursos/assinatura.
     >
     > Nesse caso, use outro nome.

   * **URL do Repositório de Código-fonte**: substitua <YOUR REPOSITORY> pelo nome do repositório da sua bifurcação.

   * **Integração Manual do Código-fonte**: escolha **falso**, uma vez que você está implantando da sua bifurcação.

   * **Id do Cliente**: use a ID do cliente do registro do aplicativo criado anteriormente.

   * **Segredo do Cliente**: use o valor da chave do registro do aplicativo criado anteriormente.

   * Marque **Concordo com os termos e condições declarados acima**.

5. Clique em **Comprar**.

**Adicionar URL DE RESPOSTA ao registro de aplicativo**

1. Após a implantação, abra o grupo de recursos:

   ![](Images/azure-resource-group.png)

2. Clique no aplicativo Web.

   ![](Images/azure-web-app.png)

   Copie a URL e altere o esquema para **https**. Esta é a URL de reprodução e será usada na próxima etapa.

3. Navegue até o registro do aplicativo no novo portal do Azure e, em seguida, abra as configurações do Windows.

   Adicionar a URL de resposta:

   ![](Images/aad-add-reply-url.png)

   > Observação: para depurar o exemplo localmente, certifique-se de que https://localhost:44311/ esteja nas URLs de resposta.

4. Clique em **SALVAR**.

## Compreender o código

### Introdução

**Diagrama de componente de solução**

![](Images/solution-component-diagram.png)

A camada superior da solução contém um aplicativo web e um aplicativo de console do WebJob.

A camada intermediária contém dois projetos de biblioteca de classe. 

As camadas inferiores contêm as três fontes de dados.

**EDUGraphAPI.Web**

Esse aplicativo Web baseia-se em um modelo de projeto ASP.NET MVC com a opção **contas de usuário individual** selecionada. 

![](Images/mvc-auth-individual-user-accounts.png)

Os seguintes arquivos foram criados pelo modelo MVC e somente as pequenas alterações foram feitas:

1. **/App_Start/Startup.Auth.Identity.cs** (o nome original é Startup.Auth.cs)
2. **/Controllers/AccountController.cs**

Este projeto de amostra usa **[ASP.NET Identity](https://www.asp.net/identity)** e **[Owin](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/owin)**. Essas duas tecnologias fazem diferentes métodos de autenticação coexistirem facilmente. A familiaridade com esses componentes, além de ASP.NET MVC, é essencial para a compreensão desse exemplo.

Veja a seguir arquivos de classe importantes usados neste projeto da Web:

| Arquivo | Descrição |
| --------------------------------- | ---------------------------------------- |
| /App_Start/Startup.Auth.AAD.cs | Integração com a autenticação do Azure Active Directory |
| /Controllers/AdminController.cs | Contém as ações administrativas: <br>consentimento do administrador, gerenciar contas vinculadas e instalar o aplicativo. |
| /Controllers/LinkController.cs | Contém as ações para vincular contas de usuários locais e do AD |
| /Controllers/SchoolsController.cs | Contém as ações para apresentar dados de formação educacional |

Este aplicativo da Web é um **aplicativo multilocatário**. No AAD, habilitamos a opção:

![](Images/app-is-multi-tenant.png)

Os usuários de qualquer locatário do Azure Active Directory podem acessar esse aplicativo. Como este aplicativo usa algumas permissões do aplicativo, um administrador do locatário deve se inscrever (consentir) primeiro. Caso contrário, os usuários seriam um erro:

![](Images/app-requires-admin-to-consent.png)

Para saber mais, confira [Criar um aplicativo Web SaaS multilocatário usando o Azure AD & OpenID Connect](https://azure.microsoft.com/en-us/resources/samples/active-directory-dotnet-webapp-multitenant-openidconnect/).

**EDUGraphAPI.SyncData**

Esse é o WebJob usado para sincronizar os dados do usuário. No método **Functions.SyncUsersAsync**, **UserSyncService** de EDUGraphAPI.Common Project é usado.

O projeto foi criado para demonstrar a consulta diferencial. Marque [Consulta diferencial](#differential-query) para obter mais detalhes.

**EDUGraphAPI.Common**

O projeto biblioteca de classe é usado tanto em **EDUGraphAPI.Web** **EDUGraphAPI.SyncData**. 

A tabela a seguir mostra as pastas do projeto:

| Pasta | Descrição |
| ------------------ | ---------------------------------------- |
| /Data | Contém ApplicationDbContext e classes de entidade |
| /DataSync | Contém a classe UserSyncSerextensionsvice, que é usada pelo EDUGraphAPI.SyncData WebJob |
| /DifferentialQuery | Contém a classe DifferentialQueryService que é usada para enviar uma consulta diferencial e analisar o resultado. |
| /Extensions | Contém muitos métodos de extensão que simplificam a leitura de códigos. /Utils | Contém a grande classe usada AuthenticationHelper.cs |

**Microsoft.Education**

Esse projeto encapsula o cliente**API REST [Escolas](https://msdn.microsoft.com/en-us/office/office365/api/school-rest-operations)**. A classe principal neste projeto é o **EducationServiceClient**.

### Modelo de dados e acesso a dados

A identidade do ASP.NET usa [Entity Framework Code First](https://msdn.microsoft.com/en-us/library/jj193542(v=vs.113).aspx) para implementar todos os seus mecanismos de persistência. O pacote [Microsoft.AspNet.Identity.EntityFramework](https://www.nuget.org/packages/Microsoft.AspNet.Identity.EntityFramework/) é consumido para isso. 

Neste exemplo, **ApplicationDbContext** é criado para acessar o banco de dados. Ele herda da **IdentityDbContext** que é definida no pacote NuGet mencionado acima.

Veja a seguir os modelos de dados importantes (e suas propriedades importantes) que costumam usar neste exemplo:

**ApplicationUsers**

Herdadas de **IdentityUser**. 

| Propriedade | Descrição |
| ------------- | ---------------------------------------- |
| Organização | O locatário do usuário. Para o usuário local não vinculado, seu valor é nulo |
| O365UserId | Usado para vincular a uma conta do Office 365 |
| O365Email | O e-mail da conta do Office 365 vinculada |
| JobTitle | Usado para demonstrar a consulta diferencial |
| Department | Usado para demonstrar a consulta diferencial |
| Mobile | Usado para demonstrar a consulta diferencial |
| FavoriteColor | Usado para demonstrar dados locais |

**Organizações**

Uma linha nessa tabela representa um locatário no AAD.

| Propriedade | Descrição |
| ---------------- | ------------------------------------ |
| Locatárioid | GUID do locatário |
| Nome | Nome do locatário |
| IsAdminConsented | É o locatário consentido por qualquer administrador |

### Fluxo de consentimento do administrador

As permissões somente do aplicativo sempre exigem o consentimento do administrador de locatários. Se o aplicativo solicitar uma permissão somente do aplicativo e um usuário tentar entrar no aplicativo, uma mensagem de erro será exibida informando que o usuário não pode fornecer o consentimento.

Algumas permissões delegadas também exigem o consentimento do administrador de locatários. Por exemplo, a capacidade de gravar no Azure AD como o usuário conectado requer o consentimento de um administrador de locatários. Semelhante às permissões somente do aplicativo, se um usuário comum tenta entrar em um aplicativo que solicita uma permissão delegada que exige o consentimento do administrador, seu aplicativo recebe um erro. Se uma permissão exige ou não o consentimento do administrador é determinado pelo desenvolvedor que publicou o recurso e pode ser encontrado na documentação do recurso.

Se o aplicativo usar permissões que exigem o consentimento do administrador, você precisará ter um gesto como um botão ou link, em que o administrador pode iniciar a ação. A solicitação que seu aplicativo envia para essa ação é uma solicitação de autorização do OAuth2/OpenID Connect normal, que também inclui o parâmetro de cadeia de caracteres de consulta `prompt=admin_consent`. Depois que o administrador fornecer seu consentimento e a entidade de serviço for criada no locatário do cliente, as próximas solicitações de conexão não precisarão do parâmetro `prompt=admin_consent`. Uma vez que o administrador tiver decidido que as permissões solicitadas forem aceitáveis, não será solicitado o consentimento de nenhum outro usuário no locatário daquele ponto em diante.



### Fluxos de autenticação

Há quatro fluxos de autenticação neste projeto.

Os 2 primeiros fluxos (logon local/O365) permitem que os usuários entrem com uma conta local ou com uma conta do Office 365 e se vinculam a outra conta de tipo. Esse procedimento é implementado na LinkController.

**Fluxo de autenticação de logon local**

![](Images/auth-flow-local-login.png)

**Fluxo de autenticação de logon do O365**

![](Images/auth-flow-o365-login.png)

**Fluxo de autenticação de logon de administrador**

Esse fluxo mostra como um administrador faz logon no sistema e realiza operações administrativas.

Depois de entrar no aplicativo com uma conta do Office 365, o administrador será solicitado a criar um link para a conta local. Esta etapa não é necessária e pode ser ignorada. 

Como mencionamos anteriormente, o aplicativo Web é um aplicativo multilocatário que usa algumas permissões; portanto, um administrador de inquilino deve ter primeiro o consentimento do inquilino.  

Esse fluxo é implementado em AdminController. 

![](Images/auth-flow-admin-login.png)

**Fluxo de autenticação do aplicativo**

Esse fluxo é implementado no WebJob da SyncData.

![](Images/auth-flow-app-login.png)

Um certificado X509 é usado. Para obter mais detalhes, confira os links a seguir:

* [Servidor ou aplicativo Daemon para API da Web](https://docs.microsoft.com/en-us/azure/active-directory/active-directory-authentication-scenarios#daemon-or-server-application-to-web-api)
* [Autenticando no Microsoft Azure Active Directory em aplicativos daemon com certificados](https://azure.microsoft.com/en-us/resources/samples/active-directory-dotnet-daemon-certificate-credential/).
* [Criar aplicativos de serviço e daemon no Office 365](https://msdn.microsoft.com/en-us/office/office365/howto/building-service-apps-in-office-365)

### Dois tipos de API de gráfico

Há duas APIs de gráfico diferentes usadas neste exemplo:

|
| [API do Azure AD Graph](https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-graph-api) | [API do Microsoft Graph](https://graph.microsoft.io/) |
| ------------ | ---------------------------------------- | ---------------------------------------- |
| Descrição | A API do Graph do Azure Active Directory fornece acesso programático para o Azure Active Directory por meio de pontos de extremidade da API REST. Os aplicativos podem usar a API do Azure AD Graph para executar operações de criação, leitura, atualização e exclusão (CRUD) em dados de diretório e objetos de diretório, como usuários, grupos e contatos organizacionais | Uma API unificada que inclui APIs de outros serviços da Microsoft, como o Outlook, o OneDrive, o OneNote, o Planner e o Office Graph, todas acessadas por meio de um único ponto de extremidade com um único token de acesso. |
| Cliente | Install-Package [Microsoft. Azure. ActiveDirectory. GraphClient](https://www.nuget.org/packages/Microsoft.Azure.ActiveDirectory.GraphClient/) | Install-Package [Microsoft. Graph](https://www.nuget.org/packages/Microsoft.Graph/) |
| Ponto de extremidade | https://graph.windows.net | https://graph.microsoft.com |
| Gerenciador de API | https://graphexplorer.cloudapp.net/| https://graph.microsoft.io/graph-explorer |

Neste exemplo, usamos as classes abaixo, que se baseiam em uma interface comum, para demonstrar como as APIs estão relacionadas:  

![](Images/class-diagram-graphs.png)

A interface **IGraphClient** define dois métodos: **GeCurrentUserAsync** e **GetTenantAsync**.

**MSGraphClient** implementar a interface **IGraphClient**com bibliotecas de cliente do Microsoft Graph.

A interface e a classe de cliente de gráfico estão localizadas na pasta **/Services/GraphClients** do aplicativo Web. Alguns códigos são realçados abaixo para mostrar como obter o usuário e o locatário.

**Microsoft Graph** – MSGraphClient.cs

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

Observe que, em configurações de registro do aplicativo, as permissões de cada API de gráfico são configuradas separadamente:

![](/Images/aad-create-app-06.png) 

### Office 365 Education API

[APIs do Office 365 Education](https://msdn.microsoft.com/office/office365/api/school-rest-operations) ajuda a extrair dados do seu locatário do Office 365 que foi sincronizado com a nuvem pelo Microsoft School Data Sync. Esses resultados fornecem informações sobre escolas, seções, professores, estudantes e listamentos. A API REST Escolas fornece acesso a entidades de ensino no Office 365 para locatários Education.

No exemplo, o projeto **Microsoft.Education** Class Library foi criado para encapsular a API do Office 365 Education. 

**EducationServiceClient** é a principal classe da biblioteca. Com TI, podemos obter dados educativos facilmente.

**Obtenha escolas**

~~~c#
public async Task<School[]> GetSchoolsAsync()
{
    var schools = await HttpGetArrayAsync<EducationSchool>("education/schools");
    return schools.ToArray();

}
~~~



**GET Sections**

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


Veja a seguir algumas capturas de tela do aplicativo de exemplo que mostram os dados de formação educacional.

![](Images/edu-schools.png)



![](Images/edu-classes.png)

![](Images/edu-class.png)

Em **EducationServiceClient**, três métodos privados com HttpGet foram criados para simplificar a invocação das APIs REST.

* **HttpGetAsync**: envia uma solicitação HTTP GET para o ponto de extremidade de destino e retorna a cadeia de caracteres de resposta JSON. Um token de acesso é incluído no cabeçalho de autenticação do portador.
* **HttpGetObjectAsync<T>**: desserializa a sequência JSON retornada por HttpGetAsync para o tipo de destino T e retorna o objeto de resultado.
* **HttpGetArrayAsync<T>**: desserializa a cadeia de caracteres JSON retornada por HttpGetAsync para a matriz de destino Type T [] e retorna a matriz.

### Consulta diferencial

Uma solicitação de [consulta diferencial](https://msdn.microsoft.com/en-us/Library/Azure/Ad/Graph/howto/azure-ad-graph-api-differential-query) retorna todas as alterações feitas em entidades especificadas durante o tempo entre duas solicitações consecutivas. Por exemplo, se você fizer uma solicitação de consulta diferencial após a solicitação de consulta diferencial anterior, apenas as alterações feitas durante essa hora serão retornadas. Essa funcionalidade é especialmente útil ao sincronizar dados do diretório de locatários com o armazenamento de dados de um aplicativo.

O código relacionado se encontra nas duas pastas a seguir do projeto **EDUGraphAPI.Common**:

* **/DifferentialQuery**: contém classes de envio de consulta diferencial e resultado de análise diferencial.
* **/DataSync**: contém classes que são usadas para demonstrar como sincronizar os usuários.

> Observe que as classes na pasta **DifferentialQuery** usam algumas tecnologias avançadas .NET. Em vez disso, destaque o uso dessas classes, em vez de detalhes da implementação.

Para sincronizar usuários, definimos a classe User:

~~~c#
public class User
{
    public string ObjectId { get; set; }
    public virtual string JobTitle { get; set; }
    public virtual string Department { get; set; }
    public virtual string Mobile { get; set; }
}
~~~

Observe que as propriedades mutáveis *JobTitle*, *Department*, *Mobile* são virtuais. As classes na pasta **DifferentialQuery** criarão um tipo de proxy para o tipo de usuário e substituirão essas propriedades virtuais para controlar alterações.

Na classe **UserSyncService**, demonstramos como usar **DifferentialQueryService** para enviar uma consulta diferencial e obter um resultado diferencial.

```c#
var differentialQueryService = new DifferentialQueryService(/**/);
DeltaResult<Delta<User>> result = await differentialQueryService.QueryAsync<User>(url);
```
And how to update (or delete) users in local database with the delta result:

~~~c#
foreach (var differentialUser in result.Items)
    await UpdateUserAsync(differentialUser);
//...
private async Task UpdateUserAsync(Delta<User> differentialUser) { /**/ }
~~~

O modelo de dados **DataSyncRecord** é usado para a deltaLinks persistente.

A seguir, o log é gerado pelo WebJob SyncData:

![](Images/sync-data-web-job-log.png) 

### Filtros

Na pasta **/Infrastructure** do projeto da Web, há vários FilterAttributes.

**EduAuthorizeAttribute**

Este é um filtro de autorização herdado de AuthorizeAttribute.

Ele foi criado para permitir que o aplicativo web redirecione os usuários para a página de logon adequada em nosso cenário de método de múltipla autenticação.

Substituímos o método **HandleUnauthorizedRequest** para redirecionar o usuário para/Account/Login:

~~~c#
protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
{
    filterContext.Result = new RedirectResult("/Account/Login");
}
~~~

**HandleAdalExceptionAttribute**

A classe **AuthenticationHelper** expõe muitos métodos que retornam tokens de acesso ou instância de um cliente da API. A maioria desses métodos invoca **[AuthenticationContext. AcquireTokenSilentAsync](https://msdn.microsoft.com/en-us/library/mt473642.aspx)** internamente. Geralmente, **AcquireTokenSilentAsync** obtém o token de acesso corretamente, pois os tokens são armazenados em cache por **ADALTokenCache**. 

Em algumas situações, como token em cache expirado ou um novo token de recurso solicitado, **AcquireTokenSilentAsync** lançará****AdalException.**HandleAdalExceptionAttribute** é necessário para processar **AdalException** e navegar pelo usuário para o ponto de extremidade de autenticação para obter um novo token.

Em alguns casos, redirecionaremos o usuário diretamente para o ponto de extremidade de autenticação chamando:

~~~c#
filterContext.HttpContext.GetOwinContext().Authentication.Challenge(
   new AuthenticationProperties { RedirectUri = requestUrl },
   OpenIdConnectAuthenticationDefaults.AuthenticationType);
~~~

E, em outros casos, queremos mostrar ao usuário a página abaixo para informar ao usuário o motivo pelo qual ele foi redirecionado, especialmente para um usuário que se conectou com uma conta local.

![](Images/web-app-login-o365-required.png)

Usamos uma opção para controlar isso. O valor da opção é recuperado por:

~~~c#
//public static readonly string ChallengeImmediatelyTempDataKey = "ChallengeImmediately";
var challengeImmediately = filterContext.Controller.TempData[ChallengeImmediatelyTempDataKey];
~~~

Se o valor for verdadeiro, redirecionaremos o usuário para o ponto de extremidade de autenticação imediatamente. Caso contrário, a página acima será exibida primeiro e o usuário clicará no botão de logon para prosseguir.

**LinkedOrO365UsersOnlyAttribute**

Este é outro filtro de autorização. Com ela, somente os usuários vinculados ou os usuários do Office 365 poderão visitar os controladores/ações protegidas.

~~~c#
protected override bool AuthorizeCore(HttpContextBase httpContext)
{
    var applicationService = DependencyResolver.Current.GetService<ApplicationService>();
    var user = applicationService.GetUserContext();
    return user.AreAccountsLinked || user.IsO365Account;
}
~~~

Para o usuário não autorizado, vamos mostrá-los para a página NoAccess:

~~~c#
protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
{
    filterContext.Result = new ViewResult { ViewName = "NoAccess" };
}
~~~

Até o momento, ele só é usado na **SchoolsController**.

### Classes principais

**Microsoft.Education**

* `EducationServiceClient`: uma instância da classe manipula as solicitações, enviando-as à API do Office 365 Education e processando as respostas.

  | Método | Descrição |
| ------------------- | ---------------------------------------- |
| GetSchoolsAsync | Obtenha todas as escolas existentes no locatário do Azure Active Directory |
| GetSchoolAsync | Obter uma escola usando a ID do objeto |
| GetAllSectionsAsync | Obter seções dentro de uma escola |
| GetMySectionsAsync | Obter minhas seções dentro de uma escola |
| GetSectionAsync | Obter uma seção usando a ID do objeto |
| GetMembersAsync | Obter Membros em uma escola |
| GetStudentAsync | Obter o usuário atual conectado como um aluno |
| GetTeacherAsync | Obter o usuário conectado no momento como professor |

**EDUGraphAPI.Common**

* **`Data.ApplicationUser`**: uma instância da classe representa um usuário.

* **`Data.Organization`**: uma instância da classe representa um locatário no Azure AD. 

* **`Data.ApplicationDbContext`**: Classe DbContext usada pela Entity Framework, herdada de `IdentityDbContext < ApplicationUser >`.

* **`DataSync.User`**: uma instância da classe representa um usuário no Azure AD. Observe que as propriedades usadas para controlar alterações são virtuais.

* **`DataSync.UserSyncService`**: uma instância da classe manipula a sincronização de usuários no banco de dados local com consulta diferencial. Invoque o método `SyncAsync` para iniciar os usuários de sincronização.

* **`DifferentialQuery.DifferentialQueryService`**: Uma instância da classe manipula a solicitação de construção, enviando-a para o terminal em serviço e processando as respostas. Invoque o método `QueryAsync` com um deltaLink para iniciar uma consulta diferencial. O resultado diferencial será convertido em `DeltaResult < Delta < > >` pela classe `DeltaResultParser`.

* **`utils. AuthenticationHelper`**: uma classe auxiliar estática usada para obter token de acesso, resultado de autenticação, contexto de autenticação e instâncias do cliente de serviços.

  | Método | Descrição |
| -------------------------------------- | ---------------------------------------- |
| GetActiveDirectoryClientAsync | Obter uma instância de ActiveDirectoryClient |
| GetGraphServiceClientAsync | Obter uma instância de GraphServiceClient |
| GetEducationServiceClientAsync | Obter uma instância de EducationServiceClient |
| GetActiveDirectoryClient | Obter uma instância de ActiveDirectoryClient no AuthenticationResult especificado |
| GetGraphServiceClient | Obter uma instância de GraphServiceClient no AuthenticationResult especificado |
| GetAccessTokenAsync | Obter um token de acesso do recurso especificado |
| GetAuthenticationResult | Obter uma AuthenticationResult do recurso especificado |
| GetAuthenticationContext | Obter uma instância de AuthenticationContext |
| GetAuthenticationResultAsync | Obter AuthenticationResult do código de autorização especificado |
| GetAppOnlyAccessTokenForDaemonAppAsync | Obter um token de acesso somente para o aplicativo de um aplicativo daemon |

  A maioria dos métodos acima têm um argumento chamado permissão. Seu tipo é `Permissões`, um tipo de enumeração com dois valores definidos:

  * `Delegado`: o cliente acessa a API da Web como o usuário conectado.
  * `Aplicativo`: o cliente acessa a API da Web diretamente (sem contexto de usuário). Esse tipo de permissão exige o consentimento do administrador.

* **`Utils.AuthenticationHelper`**: uma classe estática usada para construir a URL de autorização. `GetUrl` é o único método definido na classe.

* **`Constantes `**: uma classe estática contém valores de configurações do aplicativo e outros valores constantes.

**EDUGraphAPI.Web**

* **`Controllers.AccountController`**: contém ações para o usuário registrar, fazer logon e alterar a senha.

* **`Controllers.AdminController`**: Implementa o ** Fluxo de autenticação do Login de Administração**. Verifique a seção [Fluxos de Autenticação](#authentication-flows) para obter mais detalhes.

* **`Controllers.LinkController`**: Implementa o **Logon Local/O365 do Fluxo de autenticação**. Verifique a seção [Fluxos de Autenticação](#authentication-flows) para obter mais detalhes.

* **`Controllers.SchoolsController`**: contém ações para mostrar escolas e aulas. A classe `SchoolsService` é usada principalmente por esse controlador. Marque a seção [Office 365 Education API](#office-365-education-api) para obter mais detalhes.

* **`Infrastructure.EduAuthorizeAttribute`**:permita que o aplicativo Web redirecione o usuário atual para a página de logon adequada no cenário de vários métodos de autenticação. Marque a seção [Filtros](#filters) para obter mais detalhes.

* **`Infrastructure.HandleAdalExceptionAttribute`**: manipula AdalException e navega o usuário até o ponto de extremidade ou / Link / LoginO365Required. Marque a seção [Filtros](#filters) para obter mais detalhes.

* **`Infrastructure.LinkedOrO365UsersOnlyAttribute`**:permite que somente usuários vinculados ou usuários do Office 365 visitem os controladores/ações protegidas. Marque a seção [Filtros](#filters) para obter mais detalhes.

* **`Models.UserContext`**: contexto para o usuário conectado.

* **`Services.GraphClients.AADGraphClient`**: implementa a interface `IGraphClient` com Azure AD Graph API. Marque a seção[Dois tipos de API de gráfico](#two-kinds-of-graph-api) para obter mais detalhes.

* **`Services.GraphClients.MSGraphClient`**: implementa a interface `IGraphClient` com Microsoft Graph API. Marque a seção[Dois tipos de API de gráfico](#two-kinds-of-graph-api) para obter mais detalhes.

* **`Services. ApplicationService.`**: uma instância da classe que tem/atualiza usuário/organização.

  | Método | Descrição |
| ------------------------------- | ---------------------------------------- |
| CreateOrUpdateOrganizationAsync | Criar ou atualizar a organização |
| GetAdminContextAsync | Obter contexto do administrador atual |
| Getcurrentuser | Obter o usuário atual |
| GetCurrentUserAsync | Obter o usuário atual |
| GetUserAsync | Obter usuário por ID |
| GetContext | Obter contexto do usuário atual |
| GetUserContextAsync | Obter contexto do usuário atual |
| GetLinkedUsers | Obter usuários vinculados com o filtro especificado |
| IsO365AccountLinkedAsync | A conta do O365 está associada a uma conta local |
| SaveSeatingArrangements | Salvar disposições de assentos |
| UnlinkAccountsAsync | Desvincular a conta especificada |
| UnlinkAllAccounts | Desvincular todas as contas no locatário especificado |
| UpdateLocalUserAsync | Atualizar o usuário local com as informações do usuário e do locatário do O365 |
| UpdateOrganizationAsync | Atualizar organização |
| UpdateUserFavoriteColor | Atualizar a cor favorita do usuário atual |

* **`Services.SchoolsService`**: uma classe de serviço usada para obter dados de formação educacional.

  | Método | Descrição |
| ------------------------------- | ---------------------------------------- |
| GetSchoolsViewModelAsync | Obter SchoolsViewModel |
| GetSchoolUsersAsync | Obter professores e alunos da escola especificada |
| GetSectionsViewModelAsync | Obter SectionsViewModel de ensino especificado |
| GetSectionDetailsViewModelAsync | Obter SectionDetailsViewModel da seção especificada |
| Getmyclasss | Obter minhas classes |

**EDUGraphAPI.SyncData**

* **`Funções`**: contém o método `SyncUsersAsync`que é executado regularmente para sincronizar os dados dos usuários.
* **`Program`**: contém o método `Main` que configura e inicia o host do WebJob.

## [Opcional] Criar e depurar o WebJob local

Depurar o **EDUGraphAPI.SyncData**:

1. Criar uma conta de armazenamento no Azure e obter a cadeia de conexão.
   > Observação: a depuração local com o emulador de armazenamento do Azure será suportada depois do [Azure WebJobs SDK](https://github.com/Azure/azure-webjobs-sdk) V2 relacionado. Confira [Suporte ao Emulador de armazenamento do Azure](https://github.com/Azure/azure-webjobs-sdk/issues/53) para obter mais detalhes.
   – **Não é** recomendado para a depuração local enquanto o trabalho web publicado está sendo executado no Azure com a mesma conta de armazenamento. Para obter mais detalhes, confira [esta pergunta](http://stackoverflow.com/questions/42020647/what-happened-when-using-same-storage-account-for-multiple-azure-webjobs-dev-li).

2. Configure **App.config**:

   ![](Images/webjob-app-config.png)

   - **Cadeias de Conexão**:
     - **AzureWebJobsDashboard**: Usa a cadeia de conexão obtida na etapa anterior.
     - **AzureWebJobsStorage**: Usa a cadeia de conexão obtida na etapa anterior.
   - **Configurações de Aplicativo**:
     - *ida:ClientId**: usa a ID do cliente do registro do aplicativo criado anteriormente.

3. Defina **EDUGraphAPI. SyncData** como projeto inicial e pressione F5. 

## Perguntas e comentários

* Se você tiver problemas para executar este exemplo, [relate um problema](https://github.com/OfficeDev/O365-EDU-AspNetMVC-Samples/issues).
* Perguntas sobre o desenvolvimento do GraphAPI em geral devem ser postadas no [Stack Overflow](http://stackoverflow.com/questions/tagged/office-addins). Não deixe de marcar as perguntas ou comentários com [ms-graph-api]. 

## Colaboração

Recomendamos que você contribua para nossos exemplos. Para obter diretrizes sobre como proceder, confira [nosso guia de contribuição](/Contributing.md).

Este projeto adotou o [Código de Conduta de Código Aberto da Microsoft](https://opensource.microsoft.com/codeofconduct/).  Para saber mais, confira as [Perguntas frequentes sobre o Código de Conduta](https://opensource.microsoft.com/codeofconduct/faq/) ou entre em contato pelo [opencode@microsoft.com](mailto:opencode@microsoft.com) se tiver outras dúvidas ou comentários.



**Copyright (c) 2017 Microsoft. Todos os direitos reservados.**
