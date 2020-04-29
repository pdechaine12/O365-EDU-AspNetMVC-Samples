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
# EDUGraphAPI : exemple de code Office 365 pour l’éducation (ASP.NET MVC)

Dans cet exemple, nous vous indiquons comment intégrer des données de liste de présence/rôles d’établissement scolaire, ainsi que les services Office 365 disponibles via l’API Graph. 

Les données scolaires sont synchronisées dans les locataires Office 365 éducation par [Microsoft School Data Sync](http://sds.microsoft.com).  

**Table des matières**
* [Exemples d’objectifs](#sample-goals)
* [Conditions préalables](#prerequisites)
* [inscrire l’application dans Azure Active Directory](#register-the-application-in-azure-active-directory)
* [Créer et déboguer localement](#build-and-debug-locally)
* [Déploiement de l’exemple vers Azure](#deploy-the-sample-to-azure)
* [Comprendre le code](#understand-the-code)
* [[facultatif] Créer et déboguer le WebJob localement](#optional-build-and-debug-the-webjob-locally)
* [Questions et commentaires](#questions-and-comments)
* [Contribution](#contributing)

## Exemples de objectifs

Cet exemple illustre les éléments suivants :

* Appeler les API Graph, notamment :

  * [Microsoft Azure Active Directory Graph](https://www.nuget.org/packages/Microsoft.Azure.ActiveDirectory.GraphClient/)
  * [API Microsoft Graph](https://www.nuget.org/packages/Microsoft.Graph/)

* Liaison de comptes d’utilisateur gérés localement et de comptes d’utilisateurs Office 365 (Azure Active Directory). 

  Une fois les comptes liés, les utilisateurs peuvent utiliser des comptes locaux ou Office 365 pour se connecter au site web d’exemple et l’utiliser.

* Acquisition d’établissements scolaires, de sections, d’enseignants et d’étudiants à partir d’Office 365 Éducation :

  * [Informations de référence sur les API REST Office 365 Schools](https://msdn.microsoft.com/office/office365/api/school-rest-operations)
  * Une [Requête différentielle](https://msdn.microsoft.com/en-us/library/azure/ad/graph/howto/azure-ad-graph-api-differential-query) est utilisée pour synchroniser des données mises en cache dans une base de données locale par la tâche web SyncData.

EDUGraphAPI est basé sur ASP.NET MVC et [ASP.NET Identity](https://www.asp.net/identity) est également utilisé dans ce projet.

## Conditions préalables

**Déploiement et Exécution de cet exemple nécessite**:
* Un abonnement Azure disposant des autorisations pour inscrire une nouvelle application et déployer l’application web.
* Un client Office 365 pour l’éducation avec Microsoft School Data Sync activé
* Un des navigateurs suivants : Edge, Internet Explorer 9, Safari 5.0.6, Firefox 5, Chrome 13 ou une version ultérieure de l’un de ces navigateurs.
En outre : Le développement ou l’exécution de cet exemple localement nécessite les conditions suivantes :  
    * Visual Studio 2017 (toute édition).
	* Familiarisation avec le C#, les applications web.Net, la programmation JavaScript et les services Web.

## Inscription de l’application dans Azure Active Directory

1. Connectez-vous au nouveau Portail Azure : [https://portal.azure.com](https://portal.azure.com/).

2. Choisissez votre client Azure AD en sélectionnant votre compte dans le coin supérieur droit de la page :

   ![](Images/aad-select-directory.png)

3. Cliquez sur **Azure Active Directory**, -> **Inscriptions des applications** -> **+Ajouter**.

   ![](Images/aad-create-app-01.png)

4. Entrez un **Nom**, puis sélectionnez **Application web/API** en tant que**Type d’application**.

   **URL de connexion**: https://localhost :44311/

   ![](Images/aad-create-app-02.png)

   Cliquez sur **Créer**.

5. Une fois l’opération terminée, l’application s’affiche dans la liste.

   ![](/Images/aad-create-app-03.png)

6. Cliquez dessus pour afficher les détails associés. 

   ![](/Images/aad-create-app-04.png)

7. Cliquez sur **Tous les paramètres**, si la fenêtre de paramètres ne s’affiche pas.

   * Cliquez sur **Propriétés**, puis configurez **Mutualisé** à **Oui**.

     ![](/Images/aad-create-app-05.png)

     Copiez **ID d’application**, puis cliquez **Enregistrer**.

   * Cliquez sur **Autorisations requises**. Ajoutez les autorisations suivantes :

     | API | Autorisation d’application | Autorisations déléguées |
| ------------------------------ | ---------------------------------------- | ---------------------------------------- |
| Microsoft Graph | Lire les profils complets de tous les utilisateurs<br> Lire les données de l’annuaire<br> Lire tous les groupes | Lire les données de l’annuaire<br>Accéder à l’annuaire en tant qu’utilisateur connecté<br>Connecter des utilisateurs<br> Disposer d’un accès total à tous les fichiers accessibles par l’utilisateur<br> Disposer d’un accès total aux fichiers utilisateur<br> Lire les devoirs non notés des utilisateurs<br> Lire et écrire les devoirs non notés des utilisateurs<br> Lire les devoirs de cours des utilisateurs et de leurs notes<br> Lire et écrire les affectations de cours des utilisateurs et leurs notes |
| Windows Azure Active Directory | Lire les données d’annuaire | Se connecter et lire le profil de l’utilisateur<br>Lire et écrire les données de l’annuaire

     ![](/Images/aad-create-app-06.png)

     ​

     **Autorisations d’application**

     | Autorisation | Description | Autorisation d’administration requise |
| ----------------------------- | ---------------------------------------- | ---------------------- |
| Lire tous les profils complets des utilisateurs | Autorise l’application à lire l’ensemble complet des propriétés de profil, de l’appartenance aux groupes, des rapports et des gestionnaires d’autres utilisateurs au sein de votre organisation, sans utilisateur connecté. | Oui |
| Lire les données d’annuaire | Autorise l’application à lire les données de l’annuaire de votre organisation, telles que les utilisateurs, les groupes et les applications, sans utilisateur connecté. | Oui |

     **Autorisations déléguées**

     | Autorisation | Description | Autorisation d’administration requise |
| -------------------------------------- | ---------------------------------------- | ---------------------- |
| Lire les données d’annuaire | Autorise l’application à lire les données de l’annuaire de votre organisation, telles que les utilisateurs, les groupes et les applications. | Oui |
| Accéder au répertoire en tant qu’utilisateur connecté | Autorise l’application à accéder aux informations de l’annuaire comme étant l’utilisateur connecté. | Oui |
| Connecter des utilisateurs | Autorise les utilisateurs à se connecter à l’application à l’aide de leur compte professionnel ou scolaire et permet à l’application de voir les informations de profil utilisateur de base. | Non |
| Se connecter et lire le profil de l’utilisateur | Autorise les utilisateurs à se connecter à l’application et autorise l’application à lire le profil des utilisateurs connectés. Elle autorise également l’application à lire les informations de base sur l’entreprise d’utilisateurs connectés. | Non |
| Lire et écrire des données d’annuaire | Autorise l’application à lire et écrire des données dans l’annuaire de votre organisation, par exemple, les utilisateurs et les groupes. Elle n’autorise pas l’application à supprimer des utilisateurs ou des groupes ou à réinitialiser les mots de passe de l’utilisateur. | Oui |

     ​

   * Cliquez sur **Clés**, puis ajoutez une nouvelle clé :

     ![](Images/aad-create-app-07.png)

     Cliquez sur **Enregistrer**, puis copiez la **VALEUR** de la clé. 

   Fermez la fenêtre Paramètres.

8. Cliquez sur **Manifeste**.

   ![](Images/aad-create-app-08.png)

   Insérez le JSON suivant dans le tableau de **keyCredentials**.

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

   Cliquez sur **Enregistrer**.

   > Remarque : cette étape configure la certification utilisée par une tâche web. Pour plus d’informations, consultez la section **Flux d’authentification des applications**.

## Créer et déboguer localement

Vous pouvez ouvrir ce projet avec l’édition de Visual Studio 2017 que vous avez déjà, ou télécharger et installer l'édition de la Communauté pour exécuter, construire et/ou développer cette application localement.



Déboguez le **EDUGraphAPI.Web** :

1. Configurez **appSettings** dans le **Web.config**. 

   ![](Images/web-app-config.png)

   - **ida:ClientId** : utilisez l’ID client de l’inscription de l’application que vous avez créée précédemment.
   - **ida:ClientSecret**: utilisez la valeur clé de l’inscription de l’application que vous avez créée précédemment.
   - **SourceCodeRepositoryURL** : utilisez l’URL de référentiel de votre fourche.


2. Sélectionnez **EDUGraphAPI.Web** comme projet de démarrage, puis appuyez sur la touche F5. 

## Déployez l’exemple sur Azure

**Autorisation GitHub**

1. Générez un jeton

   - Ouvrez https://github.com/settings/tokens dans votre navigateur web.
   - Connectez-vous à votre compte GitHub où vous avez dupliqué ce référentiel.
   - Cliquez sur **Générer un jeton**.
   - Entrez une valeur dans la zone de texte **Description du jeton**
   - Sélectionnez les options suivantes (vos sélections doivent correspondre à la capture d’écran ci-dessous) :
        - repo (all) -> repo:status, repo_deployment, public_repo
        - admin:repo_hook -> read:repo_hook

   ![](Images/github-new-personal-access-token.png)

   - Cliquez sur **Générer un jeton**
   - Copiez le jeton

2. Ajoutez le jeton GitHub à Azure dans Azure Resource Explorer

   - Ouvrez https://resources.azure.com/providers/Microsoft.Web/sourcecontrols/GitHub dans votre navigateur web.
   - Connectez-vous à l’aide de votre compte Azure.
   - Sélectionnez l’abonnement Azure approprié.
   - Sélectionnez le mode**Lecture/écriture**.
   - Cliquez sur **Modifier**.
   - Collez le jeton dans le **paramètre de jeton**.

   ![](Images/update-github-token-in-azure-resource-explorer.png)

   - Cliquez sur **PUT**.

**Déployer les composants Azure à partir de GitHub**

1. Vérifiez que la création transmet la création VSTS.

2. Transférez ce référentiel sur votre compte GitHub.

3. Cliquez sur le bouton déployer vers Azure :

   [![Déployer dans Azure](http://azuredeploy.net/deploybutton.png)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FOfficeDev%2FO365-EDU-AspNetMVC-Samples%2Fmaster%2Fazuredeploy.json)

4. Renseignez les valeurs dans la page déploiement et sélectionnez le case à cocher **j’accepte les conditions générales mentionnées ci-dessus**.

   ![](Images/azure-auto-deploy.png)

   * **Groupe de ressources** : nous vous suggérons de créer un groupe.

   * **Nom du site**: veuillez entrer un nom. Tels que EDUGraphAPICanviz ou EDUGraphAPI993.

     > Remarque : Si le nom que vous entrez est utilisé, vous obtiendrez des erreurs de validation :
     >
     > ![](Images/azure-auto-deploy-validation-errors-01.png)
     >
     > Cliquez dessus pour obtenir davantage de détails tels que le compte de stockage est déjà dans un autre groupe de ressources/abonnement.
     >
     > Dans ce cas, veuillez utiliser un autre nom.

   * **URL du référentiel de code source**: remplacez <YOUR REPOSITORY> par le nom du référentiel de votre fourche.

   * **Intégration manuelle du code source** : sélectionnez **faux**, car vous déployez à partir de votre propre fourche.

   * **ID client** : utilisez l’ID client de l’inscription de l’application que vous avez créée précédemment.

   * **Secret client**: utilisez la valeur clé de l’inscription de l’application que vous avez créée précédemment.

   * Cliquez sur **J’accepte les termes et conditions mentionnés ci-dessus**.

5. Cliquez sur **Achat**.

**Ajouter une URL de réponse à l’inscription de l’application**

1. Après le déploiement, ouvrez le groupe de ressources :

   ![](Images/azure-resource-group.png)

2. Cliquez sur l’application web.

   ![](Images/azure-web-app.png)

   Copiez l’URL et modifiez le schéma en **https**. Il s’agit de l’URL de relecture qui sera utilisée à l’étape suivante.

3. Accédez à inscription de l’application dans le nouveau Portail Azure, puis ouvrez le paramètre Windows.

   Ajouter l’URL de réponse :

   ![](Images/aad-add-reply-url.png)

   > Remarque : pour déboguer l’exemple localement, assurez-vous que https://localhost:44311/ se trouve dans les URL de réponse.

4. Cliquez sur **ENREGISTRER**.

## Comprendre le code

### Introduction

**Diagramme de composant de solution**

![](Images/solution-component-diagram.png)

La couche supérieure de la solution contient une application web et une application console WebJob.

La couche intermédiaire contient deux projets de bibliothèque de cours. 

Les couches inférieures contiennent les trois sources de données.

**EDUGraphAPI.Web**

Cette application web est basée sur un modèle de projet ASP.NET MVC avec l’option **Comptes d’utilisateurs individuels** sélectionnée. 

![](Images/mvc-auth-individual-user-accounts.png)

Les fichiers suivants ont été créés par le modèle MVC et seules des modifications mineures ont été apportées :

1. **/App_Start/Startup.Auth.Identity.cs** (le nom d’origine est Startup.Auth.cs)
2. **/Controllers/AccountController.cs**

Cet exemple de projet utilise **[ASP.NET Identity](https://www.asp.net/identity)** et **[Owin](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/owin)**. Ces deux technologies font coexister facilement des méthodes d'authentification différentes.  La connaissance de ces composantes, en plus de ASP.NET MVC, est essentielle pour comprendre cet exemple.

Voici quelques fichiers de cours importants utilisés dans ce projet web :

| Fichier | Description |
| --------------------------------- | ---------------------------------------- |
| /App_Start/Startup.Auth.AAD.cs | Intégration à l’authentification Azure Active Directory |
| /Controllers/AdminController.cs | Contient les actions d’administration : <br>accord de l’administrateur, gérer les comptes liés et installer l’application. |
| /Controllers/LinkController.cs | Contient les actions permettant de lier les comptes d’utilisateurs AD et locaux |
| /Controllers/SchoolsController.cs | Contient les actions de présentation des données pour l’éducation |

Cette application web est une **application mutualisée**. Dans AAD, nous avons activé l’option :

![](Images/app-is-multi-tenant.png)

Les utilisateurs de n’importe quel client Azure Active Directory peuvent accéder à cette application. Dans la mesure où cette application utilise certaines autorisations d’application, un administrateur du locataire doit d’abord s’inscrire (consentement). Dans le cas contraire, les utilisateurs seraient une erreur :

![](Images/app-requires-admin-to-consent.png)

Pour plus d’informations, consultez[Créer une application web SaaS multi-locataire à l’aide d’Azure AD et OpenID Connect](https://azure.microsoft.com/en-us/resources/samples/active-directory-dotnet-webapp-multitenant-openidconnect/).

**EDUGraphAPI.SyncData**

Il s’agit de la WebJob utilisée pour synchroniser les données utilisateur. Dans la méthode **functions.SyncUsersAsync**, **UserSyncService** du projet e EDUGraphAPI.Common est utilisé.

Le projet a été créé pour illustrer une requête différentielle. Pour plus d’informations, consultez la section [Requête différentielle](#differential-query).

**EDUGraphAPI.Common**

Le projet bibliothèque de cours est utilisé à la fois sur **EDUGraphAPI.Web** et **EDUGraphAPI.SyncData**. 

Le tableau ci-dessous répertorie les dossiers du projet :

| Dossier | Description |
| ------------------ | ---------------------------------------- |
| /Data | Contient ApplicationDbContext et des cours d’entité |
| /DataSync | Contient le cours UserSyncSerextensionsvice utilisé par EDUGraphAPI. SyncData WebJob |
| /DifferentialQuery | Contient le cours DifferentialQueryService utilisé pour envoyer une requête différentielle et analyser le résultat. |
| /Extensions | Contient de nombreuses méthodes d’extension qui simplifient le codage du code facilitant la lecture du code |
| /Utils | Contient le cours utilisé large AuthenticationHelper.cs |

**Microsoft.Education**

Ce projet encapsule le client **[API REST Écoles](https://msdn.microsoft.com/en-us/office/office365/api/school-rest-operations)**. Le cours principal de ce projet est **EducationServiceClient**.

### Accès aux données et modèles de données

ASP.NET Identity utilise [Entity Framework Code First](https://msdn.microsoft.com/en-us/library/jj193542(v=vs.113).aspx) pour implémenter l’ensemble de ses mécanismes de persistance. Le package [Microsoft.AspNet.Identity.EntityFramework](https://www.nuget.org/packages/Microsoft.AspNet.Identity.EntityFramework/) est consommé pour ce problème. 

Dans cet exemple, **ApplicationDbContext** est créé pour l’accès à la base de données. Il hérité de **IdentityDbContext** qui est défini dans le package NuGet mentionné ci-dessus.

Voici les principaux modèles de données (et leurs propriétés importantes) qui ont été utilisés dans cet exemple :

**ApplicationUsers**

Hérité de **IdentityUser**. 

| Propriété | Description |
| ------------- | ---------------------------------------- |
| Organisation | Le locataire de l’utilisateur. Pour l’utilisateur local non lié, sa valeur est null |
| O365UserId | Utilisé pour créer un lien avec un compte Office 365 |
| O365Email | Adresse de courrier du compte Office 365 lié |
| JobTitle | Utilisé pour illustrer une requête différentielle |
| Service | Utilisé pour illustrer une requête différentielle |
| Mobile | Utilisé pour illustrer une requête différentielle |
| FavoriteColor | Utilisé pour illustrer des données locales |

**Organisations**

Une ligne dans ce tableau représente un locataire dans AAD.

| Propriété | Description |
| ---------------- | ------------------------------------ |
| IDClient | Guid du client |
| Nom | Nom du client |
| IsAdminConsented | Le client est-il envoyé par un administrateur ?

### Flux de consentement de l’administrateur

Les autorisations application seule nécessitent toujours le consentement de l’administrateur d’un client. Si votre application demande une autorisation application seule et qu’un utilisateur tente de se connecter à l’application, un message d’erreur indiquant que l’utilisateur n’est pas en mesure de donner son consentement s’affiche.

Certaines autorisations déléguées nécessitent également le consentement de l’administrateur d’un client. Par exemple, la possibilité de réécrire dans Azure AD en tant que l’utilisateur connecté requiert le consentement de l’administrateur d’un client. Comme pour les autorisations application seule, si un utilisateur standard tente de se connecter à une application qui demande une autorisation déléguée nécessitant le consentement de l’administrateur, votre application reçoit une erreur. Le fait qu’une autorisation nécessite ou non le consentement d’un administrateur est déterminé par le développeur qui a publié la ressource, et ces informations sont disponibles dans la documentation de cette ressource.

Si votre application utilise des autorisations qui nécessitent le consentement de l’administrateur, vous devez y intégrer une option comme un bouton ou un lien afin que l’administrateur puisse initier l’action. La demande envoyée par votre application pour cette action est la demande d’autorisation OAuth2/OpenID Connect ordinaire, mais qui comporte également le paramètre de chaîne de requête `prompt=admin_consent`. Une fois que l’administrateur a donné son consentement et que le principal de service est créé dans le locataire du client, les demandes de connexion ultérieures n’ont plus besoin du paramètre `prompt=admin_consent`. Comme l’administrateur a décidé que les autorisations demandées sont acceptables, les autres utilisateurs n’ont plus à donner leur consentement par la suite.



### Flux d’authentification

Ce projet comprend 4 flux d’authentification.

Les deux premiers flux (connexion locale/connexion Office 365) permettent aux utilisateurs de se connecter à l’aide d’un compte local ou d’un compte Office 365, puis de créer un lien vers le compte autre type. Cette procédure est implémentée dans la LinkController.

**Flux d’authentification de connexion locale**

![](Images/auth-flow-local-login.png)

**Flux d’authentification de connexion Office 365**

![](Images/auth-flow-o365-login.png)

**Flux d’authentification de connexion d’administrateur**

Ce flux montre comment un administrateur se connecte au système et effectue des opérations d’administration.

Une fois que vous êtes connecté à l’application à l’aide d’un compte Office 365, l’administrateur est invité à créer un lien vers un compte local. Cette étape n’est pas obligatoire et peut être ignorée. 

Comme indiqué précédemment, l’application web est une application mutualisée qui utilise des autorisations d’application, de sorte qu’un administrateur du locataire doit d’abord consentir le locataire.  

Ce flux est implémenté dans AdminController. 

![](Images/auth-flow-admin-login.png)

**Flux d’authentification des applications**

Ce flux est implémenté dans la SyncData WebJob.

![](Images/auth-flow-app-login.png)

Un certificat X509 est utilisé. Pour plus d’informations, consultez les liens suivants :

* [Application démon ou serveur vers API web](https://docs.microsoft.com/en-us/azure/active-directory/active-directory-authentication-scenarios#daemon-or-server-application-to-web-api)
* [Authentification auprès d’Azure AD dans les applications démon avec des certificats](https://azure.microsoft.com/en-us/resources/samples/active-directory-dotnet-daemon-certificate-credential/).
* [Créer des applications de service et de démon dans Office 365](https://msdn.microsoft.com/en-us/office/office365/howto/building-service-apps-in-office-365)

### Deux types d’API Graph

Il existe deux API Graph distinctes utilisées dans cet exemple :

| | [API Azure AD Graph](https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-graph-api) | [API Microsoft Graph](https://graph.microsoft.io/) |
| ------------ | ---------------------------------------- | ---------------------------------------- |
| Description | L’API Graph Azure Active Directory fournit un accès par programme à Azure Active Directory via les points de terminaison de l’API REST. Les applications peuvent utiliser l’API Azure AD Graph pour effectuer des opérations de création, lecture, mise à jour et suppression sur des données d’annuaire et des objets d’annuaire, tels que des utilisateurs, des groupes et des contacts d’organisation | API unifiée qui inclut également des API d’autres services Microsoft tels qu’Outlook, OneDrive, OneNote, Planner et Office Graph, accessibles via un point de terminaison unique avec un seul jeton d’accès. |
| Client | Installer-Package [Microsoft. Azure. ActiveDirectory. GraphClient](https://www.nuget.org/packages/Microsoft.Azure.ActiveDirectory.GraphClient/) | Installer-Package [Microsoft.Graph](https://www.nuget.org/packages/Microsoft.Graph/) |
| Point de terminaison | https://graph.windows.net | https://graph.microsoft.com |
| Explorateur d’API | https://graphexplorer.cloudapp.net/| https://graph.microsoft.io/graph-explorer |

Dans cet exemple, nous utilisons les cours suivants (basés sur une interface commune) pour illustrer la relation entre les API :  

![](Images/class-diagram-graphs.png)

L’interface **IGraphClient** définit deux méthodes : **GeCurrentUserAsync** et **GetTenantAsync**.

**MSGraphClient** implémente l’interface **IGraphClient** avec les bibliothèques clientes de Microsoft Graph.

L’interface et le cours client Graph résident dans le dossier **/Services/GraphClients** de l’application web. Certains codes sont mis en surbrillance ci-dessous pour montrer comment obtenir l’utilisateur et le client.

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

Notez que, dans les paramètres d’inscription de l’application AAD, les autorisations pour chaque API Graph sont configurées séparément :

![](/Images/aad-create-app-06.png) 

### API Office 365 Éducation

Les [API Office 365 Éducation](https://msdn.microsoft.com/office/office365/api/school-rest-operations) permettent d'extraire les données de votre locataire Office 365 qui ont été synchronisées vers le cloud par Microsoft School Data Sync. Ces résultats fournissent des informations sur les établissements scolaires, les sections, les enseignants, les étudiants et les listes de documents. L’API REST des établissements scolaires fournit l’accès aux entités scolaires dans Office 365 pour les locataires de l’éducation.

Dans l’exemple, le projet bibliothèque de cours de **Microsoft.Education** a été crée pour encapsuler l’API Office 365 Éducation. 

**EducationServiceClient** est le cours principal de la bibliothèque. Avec elle, nous pouvons obtenir facilement des données sur l'éducation.

**Obtenir des établissements scolaires**

~~~c#
public async Task<School[]> GetSchoolsAsync()
{
    var schools = await HttpGetArrayAsync<EducationSchool>("education/schools");
    return schools.ToArray();

}
~~~



**Obtenir des sections**

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


Voici quelques captures d’écran de l’exemple d’application qui illustre les données d’éducation.

![](Images/edu-schools.png)



![](Images/edu-classes.png)

![](Images/edu-class.png)

Dans **EducationServiceClient**, trois méthodes privées préfixées avec HttpGet ont été créées pour simplifier l’appel des API REST.

* **HttpGetAsync**: envoie une demande http GET au point de terminaison cible et renvoie la chaîne de réponse JSON. Un jeton d’accès est inclus dans l’en-tête d’authentification du support.
* **HttpGetObjectAsync<T>** : désérialise la chaîne JSON renvoyée par HttpGetAsync au type cible T et renvoie l’objet de résultat.
* **HttpGetArrayAsync<T>** : désérialise la chaîne JSON renvoyée par HttpGetAsync au type de tableau cible T[] et renvoie le tableau.

### Requête différentielle :

Une demande de [requête différentielle](https://msdn.microsoft.com/en-us/Library/Azure/Ad/Graph/howto/azure-ad-graph-api-differential-query) renvoie toutes les modifications apportées aux entités spécifiées pendant l’intervalle entre deux demandes consécutives. Par exemple, si vous créez une demande de requête différentielle une heure après la demande de requête différentielle précédente, seules les modifications apportées pendant cette heure sont renvoyées. Cette fonctionnalité est particulièrement utile lors de la synchronisation des données de l’annuaire client avec le magasin de données d’une application.

Le code associé se trouve dans les deux dossiers suivants du projet **EDUGraphAPI.Common** :

* **/DifferentialQuery**: contient des cours pour envoyer une requête différentielle et analyser le résultat différentiel.
* **/DataSync**: contient des cours utilisés pour montrer comment synchroniser des utilisateurs.

> Veuillez noter que les cours dans le dossier **DifferentialQuery** utilisent des technologies .NET avancées. Veuillez vous concentrer sur l'utilisation de ces cours plutôt que sur les détails d'implémentation.

Pour synchroniser des utilisateurs, nous avons défini le cours utilisateur :

~~~c#
public class User
{
    public string ObjectId { get; set; }
    public virtual string JobTitle { get; set; }
    public virtual string Department { get; set; }
    public virtual string Mobile { get; set; }
}
~~~

Notez que les propriétés modifiables *JobTitle*, *Department*, *Mobile* sont virtuelles. Les cours dans le dossier **DifferentialQuery** créent un type de proxy pour le type d’utilisateur et remplacent ces propriétés virtuelles pour le suivi des modifications.

Dans le cours **UserSyncService**, nous vous montrerons comment utiliser le **DifferentialQueryService** pour envoyer une requête différentielle et obtenir un résultat différentiel.

```c#
var differentialQueryService = new DifferentialQueryService(/**/);
DeltaResult<Delta<User>> result = await differentialQueryService.QueryAsync<User>(url);
```
Et comment mettre à jour (ou supprimer) des utilisateurs dans une base de données locale avec le résultat delta :

~~~c#
foreach (var differentialUser in result.Items)
    await UpdateUserAsync(differentialUser);
//...
private async Task UpdateUserAsync(Delta<User> differentialUser) { /**/ }
~~~

Le modèle de données **DataSyncRecord** est utilisé pour persister deltaLinks.

Voici le journal généré par le SyncData WebJob :

![](Images/sync-data-web-job-log.png) 

### Filtres

Dans le dossier **/infrastructure** du projet web, plusieurs FilterAttributes sont disponibles.

**EduAuthorizeAttribute**

Il s’agit d’un filtre d’autorisation hérité de AuthorizeAttribute.

Il a été créé pour autoriser l’application web à rediriger les utilisateurs vers la page de connexion appropriée dans le cadre de notre scénario de méthode de multi-authentification.

Nous avons remplacé la méthode **HandleUnauthorizedRequest** pour rediriger l’utilisateur vers /Account/Login :

~~~c#
protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
{
    filterContext.Result = new RedirectResult("/Account/Login");
}
~~~

**HandleAdalExceptionAttribute**

Le cours **AuthenticationHelper** expose de nombreuses méthodes qui renvoient des jetons d’accès ou une instance de client API. La plupart de ces méthodes appellent **[AuthenticationContext.AcquireTokenSilentAsync](https://msdn.microsoft.com/en-us/library/mt473642.aspx)** en interne. En règle générale, **AcquireTokenSilentAsync** obtient le jeton d’accès correctement, au fur et à mesure que les jetons sont mis en cache dans la base de données par **ADALTokenCache**. 

Dans certains cas, comme l’expiration du jeton mis en cache ou la demande d’un nouveau jeton de ressource, **AcquireTokenSilentAsync** génère **AdalException**.**HandleAdalExceptionAttribute** est requis pour gérer **AdalException**, et accéder au point de terminaison d’authentification pour obtenir un nouveau jeton.

Dans certains cas, nous redirigeons l’utilisateur directement vers le point de terminaison d’authentification en appelant :

~~~c#
filterContext.HttpContext.GetOwinContext().Authentication.Challenge(
   new AuthenticationProperties { RedirectUri = requestUrl },
   OpenIdConnectAuthenticationDefaults.AuthenticationType);
~~~

Dans d’autres cas, nous souhaitons afficher la page ci-dessous pour indiquer à l’utilisateur la raison pour laquelle il a été redirigé, en particulier pour les utilisateurs connectés avec un compte local.

![](Images/web-app-login-o365-required.png)

Nous utilisons un commutateur pour contrôler ce paramètre. La valeur du commutateur est récupérée par :

~~~c#
//public static readonly string ChallengeImmediatelyTempDataKey = "ChallengeImmediately";
var challengeImmediately = filterContext.Controller.TempData[ChallengeImmediatelyTempDataKey];
~~~

Si la valeur est true, l’utilisateur est redirigé vers le point de terminaison d’authentification immédiatement. Dans le cas contraire, la page ci-dessus s’affiche en premier, et l’utilisateur clique sur le bouton connexion pour continuer.

**LinkedOrO365UsersOnlyAttribute**

Il s’agit d’un autre filtre d’autorisation. Avec elle, nous pouvons uniquement autoriser les utilisateurs liés ou les utilisateurs Office 365 à visiter les contrôleurs/actions protégés.

~~~c#
protected override bool AuthorizeCore(HttpContextBase httpContext)
{
    var applicationService = DependencyResolver.Current.GetService<ApplicationService>();
    var user = applicationService.GetUserContext();
    return user.AreAccountsLinked || user.IsO365Account;
}
~~~

Pour les utilisateurs non autorisés, nous allons leur afficher la page NoAccess :

~~~c#
protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
{
    filterContext.Result = new ViewResult { ViewName = "NoAccess" };
}
~~~

Jusqu’à présent, il est uniquement utilisé sur **SchoolsController**.

### Cours principaux

**Microsoft.Education**

* `GraphServiceClient` : Une instance du cours gère la création de demandes en les envoyant vers l’API Office 365 Éducation et en traitant les réponses.

  | Méthode | Description |
| ------------------- | ---------------------------------------- |
| GetSchoolsAsync | Obtenir tous les établissements scolaires présents dans le client Azure Active Directory |
| GetSchoolAsync | Obtenir une école à l’aide de l’ID d’objet |
| GetAllSectionsAsync | Obtenir des sections au sein d’une école |
| GetMySectionsAsync | Utiliser mes sections au sein d’une école |
| GetSectionAsync | Obtenir une section à l’aide de l’ID d’objet |
| GetMembersAsync | Obtenir des membres au sein d’une école |
| GetStudentAsync | Obtenir l’utilisateur connecté en tant qu’étudiant |
| GetTeacherAsync | Obtenir l’utilisateur connecté en tant qu’enseignant |

**EDUGraphAPI.Common**

* **`Data.ApplicationUser`** : une instance du cours représente un utilisateur.

* **`Data.Organization`**: une instance du cours représente un client dans Azure AD. 

* **`Data.ApplicationDbContext`** : Cours DbContext utilisée par Entity Framework, héritée de `IdentityDbContext<ApplicationUser>`.

* **`DataSync.User`** : une instance du cours représente un utilisateur dans Azure AD. Notez que les propriétés utilisées pour suivre les modifications sont virtuelles.

* **`DataSync.UserSyncService`** : une instance du cours gère la synchronisation des utilisateurs dans la base de données locale avec une requête différentielle. Appelez la méthode `SyncAsync` pour commencer à synchroniser les utilisateurs.

* **`DifferentialQuery.DifferentialQueryService`**: Une instance du cours gère la création de la demande, en l’envoyant au point de terminaison du service et en traitant les réponses. Appelez la méthode `QueryAsync` avec un deltaLink pour démarrer une requête différentielle. Le résultat différentiel est converti en `DeltaResult<Delta<TEntity>>` par le cours `DeltaResultParser`.

* **`utils.AuthenticationHelper`**: un cours d’aide statique utilisée pour obtenir un jeton d’accès, le résultat de l’authentification, le contexte d’authentification et les instances de client de service.

  | Méthode | Description |
| -------------------------------------- | ---------------------------------------- |
| GetActiveDirectoryClientAsync | Obtenir une instance de ActiveDirectoryClient |
| GetGraphServiceClientAsync | Obtenir une instance de GraphServiceClient |
| GetEducationServiceClientAsync | Obtenir une instance de EducationServiceClient |
| GetActiveDirectoryClient | Obtenir une instance de ActiveDirectoryClient à partir de AuthenticationResult spécifié |
| GetGraphServiceClient | Obtenir une instance de GraphServiceClient à partir du AuthenticationResult spécifié |
| GetAccessTokenAsync | Obtenir un jeton d’accès de la ressource spécifiée |
| GetAuthenticationResult | Obtenir une AuthenticationResult de la ressource spécifiée |
| GetAuthenticationContext | Obtenir une instance de AuthenticationContext |
| GetAuthenticationResultAsync | Obtenir une AuthenticationResult à partir du code d’autorisation spécifié |
| GetAppOnlyAccessTokenForDaemonAppAsync | Obtenir un jeton d’accès réservé à l’application pour une application de démon |

  La plupart des méthodes ci-dessus ont un argument appelé autorisation. Son type est `Autorisations`, un enum type avec deux valeurs définies :

  * `Délégué`: le client accède à l’API web en tant qu’utilisateur connecté.
  * `Application` : le client accède directement à l’API web en tant que telle (aucun contexte utilisateur). Ce type d’autorisation nécessite l’accord de l’administrateur.

* **`utils.AuthenticationHelper`** : cours statique utilisé pour créer une URL d’autorisation. `GetUrl` est la seule méthode définie dans le cours.

* **`Constantes`**: un cours statique contient les valeurs des paramètres de l’application et d’autres valeurs constantes.

**EDUGraphAPI.Web**

* **`Controllers.AccountController`** : contient les actions que l’utilisateur doit enregistrer, de se connecter et de modifier le mot de passe.

* **`Controllers.AdminController`** : implémente la **flux d’authentification de connexion d’administrateur**. Pour plus d’informations, consultez la section [Flux d’authentification](#authentication-flows).

* **`Controllers.LinkController`** : implémente la **flux d’authentification de connexion locale/O365**. Pour plus d’informations, consultez la section [Flux d’authentification](#authentication-flows).

* **`Controllers.SchoolsController`** : contient des actions pour illustrer les établissements scolaires et les cours. Le cours `SchoolsService` est principalement utilisée par ce contrôleur. Pour plus d’informations, consultez la section [API Office 365 Éducation](#office-365-education-api).

* **`Infrastructure.EduAuthorizeAttribute`** : autoriser l’application web à rediriger l’utilisateur actuel vers la page de connexion appropriée dans le cadre de notre scénario de méthode de multi-authentification. Pour plus d’informations, consultez la section [Filtres](#filters).

* **`infrastructure.HandleAdalExceptionAttribute`** : gérer AdalException et parcourir l’utilisateur jusqu’au point de terminaison autorisé ou /Link/LoginO365Required. Pour plus d’informations, consultez la section [Filtres](#filters).

* **`Infrastructure.LinkedOrO365UsersOnlyAttribute`** : autoriser uniquement les utilisateurs liés ou les utilisateurs Office 365 à visiter les contrôleurs/actions protégés. Pour plus d’informations, consultez la section [Filtres](#filters).

* **`Models.UserContext`** : contexte pour l’utilisateur connecté.

* **`Services.GraphClients.AADGraphClient`** : implémente l’interface `IGraphClient` avec l’API Azure AD Graph. Pour plus d’informations, consultez la section [Deux types d’API Graph](#two-kinds-of-graph-api).

* **`Services.GraphClients.MSGraphClient`** : implémente l’interface `IGraphClient` avec l’API Microsoft Graph. Pour plus d’informations, consultez la section [Deux types d’API Graph](#two-kinds-of-graph-api).

* **`Services.ApplicationService.`**: une instance du cours gère l’affichage/la mise à jour de l’utilisateur ou de l’organisation.

  | Méthode | Description |
| ------------------------------- | ---------------------------------------- |
| CreateOrUpdateOrganizationAsync | Créer ou mettre à jour l’organisation |
| GetAdminContextAsync | Obtenir le contexte de l’administrateur actuel |
| GetCurrentUser | Obtenir l’utilisateur actuel |
| GetCurrentUserAsync | Obtenir l’utilisateur actuel |
| GetUserAsync | Obtenir l’utilisateur par ID |
| GetUserContext | Obtenir le contexte de l’utilisateur actuel |
| GetUserContextAsync | Obtenir le contexte de l’utilisateur actuel |
| GetLinkedUsers | Obtenir les utilisateurs liés avec le filtre spécifié |
| IsO365AccountLinkedAsync | Le compte Office 365 spécifié est-il lié avec un compte local |
| SaveSeatingArrangements | Enregistrer les aménagements du siège |
| UnlinkAccountsAsync | Dissocier le compte spécifié |
| UnlinkAllAccounts | Dissocier tous les comptes dans le client spécifié |
| UpdateLocalUserAsync | Mettre à jour l’utilisateur local avec les informations utilisateur et client Office 365 |
| UpdateOrganizationAsync | Mettre à jour l’organisation |
| UpdateUserFavoriteColor | Mettre à jour la couleur préférée de l’utilisateur actuel |

* **`Services.SchoolsService`** : cours de service utilisé pour obtenir des données d’éducation.

  | Méthode | Description |
| ------------------------------- | ---------------------------------------- |
| GetSchoolsViewModelAsync | Obtenir SchoolsViewModel |
| GetSchoolUsersAsync | Obtenir enseignants et étudiants de l’école spécifiée |
| GetSectionsViewModelAsync | Obtenir SectionsViewModel de l’établissement d’enseignement spécifié |
| GetSectionDetailsViewModelAsync | Obtenir SectionDetailsViewModel de la section spécifiée |
| GetMyClasses | Récupérer mes cours |

**EDUGraphAPI.SyncData**

* **`Fonctions`** : contient la méthode `SyncUsersAsync` exécutée régulièrement pour synchroniser les données des utilisateurs.
* **`Programme`**: contient la méthode `Principale` qui configure et démarre l’hôte WebJob.

## [Facultatif] Créer et déboguer le WebJob localement

Déboguez le **EDUGraphAPI.SyncData** :

1. Créez un compte de stockage dans Azure et obtenez la chaîne de connexion.
   > Remarque : - le débogage local avec Émulateur de stockage Azure sera pris en charge après [le kit de développement logiciel (SDK) Azure Webjobs](https://github.com/Azure/azure-webjobs-sdk) V2 associé. Pour plus d’informations, consultez [Prise en charge de l’émulateur de stockage Azure](https://github.com/Azure/azure-webjobs-sdk/issues/53). -Il **est pas** recommandé de déboguer en local pendant que la tâche web publiée est exécutée dans Azure avec le même compte de stockage. Pour plus d’informations, consultez [cette question](http://stackoverflow.com/questions/42020647/what-happened-when-using-same-storage-account-for-multiple-azure-webjobs-dev-li).

2. Configurer le **App.config** :

   ![](Images/webjob-app-config.png)

   - **Chaînes de connexion** :
     - **AzureWebJobsDashboard**: utilisez la chaîne de connexion que vous avez obtenue à l’étape précédente.
     - **AzureWebJobsStorage**: utilisez la chaîne de connexion que vous avez obtenue à l’étape précédente.
   - **Paramètres de l’application** :
     - **ida:ClientId**: utilisez l’ID client de l’inscription de l’application que vous avez créée précédemment.

3. Sélectionnez **EDUGraphAPI.SyncData** comme projet de démarrage, puis appuyez sur la touche F5. 

## Questions et commentaires

* Si vous rencontrez des difficultés pour exécuter cet exemple, veuillez [consigner un problème](https://github.com/OfficeDev/O365-EDU-AspNetMVC-Samples/issues).
* Si vous avez des questions sur le développement de GraphAPI en général, envoyez-les sur [Stack Overflow](http://stackoverflow.com/questions/tagged/office-addins). Posez vos questions avec les balises [ms-graph-api]. 

## Contribution

Nous vous invitons à contribuer à nos exemples. Pour obtenir des instructions sur la façon de procéder, consultez [notre guide de contribution](/Contributing.md).

Ce projet a adopté le [code de conduite Open Source de Microsoft](https://opensource.microsoft.com/codeofconduct/). Pour en savoir plus, reportez-vous à la [FAQ relative au code de conduite](https://opensource.microsoft.com/codeofconduct/faq/) ou contactez [opencode@microsoft.com](mailto:opencode@microsoft.com) pour toute question ou tout commentaire.



**Copyright (c) 2017 Microsoft. Tous droits réservés.**
