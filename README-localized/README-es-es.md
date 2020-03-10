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
# EDUGraphAPI: ejemplo de código de Office 365 Educación (ASP.NET MVC)

En este ejemplo, le mostramos cómo integrar con datos de lista o roles del centro educativo, así como servicios de Office 365 disponibles mediante la API de Graph. 

[Microsoft School Data Sync](http://sds.microsoft.com) mantiene sincronizados los datos académicos en los inquilinos de Office 365 Educación.  

**Tabla de contenido**
* [Objetivos de ejemplo](#sample-goals)
* [Requisitos previos](#prerequisites)
* [Registrar la aplicación en Azure Active Directory](#register-the-application-in-azure-active-directory)
* [Compilar y depurar localmente](#build-and-debug-locally)
* [Implementar el ejemplo en Azure](#deploy-the-sample-to-azure)
* [Entender el código](#understand-the-code)
* [[Opcional] Compilar y depurar el WebJob localmente](#optional-build-and-debug-the-webjob-locally)
* [Preguntas y comentarios](#questions-and-comments)
* [Colaboradores](#contributing)

## Objetivos del ejemplo

En el ejemplo se muestra:

* Cómo llamar a las API de Graph, incluyendo:

  * [API de Microsoft Azure Active Directory Graph](https://www.nuget.org/packages/Microsoft.Azure.ActiveDirectory.GraphClient/)
  * [API de Microsoft Graph](https://www.nuget.org/packages/Microsoft.Graph/)

* Cómo vincular las cuentas de usuario administradas localmente y las cuentas de usuario de Office 365 (Azure Active Directory). 

  Tras vincular las cuentas, los usuarios pueden usar cuentas locales o cuentas de Office 365 para iniciar sesión en el sitio web de ejemplo y usarlo.

* Cómo obtener centros educativos, secciones, profesores y estudiantes de Office 365 Educación:

  * [Referencia de la API de REST en centros educativos de Office 365](https://msdn.microsoft.com/office/office365/api/school-rest-operations)
  * Se usa una [consulta diferencial](https://msdn.microsoft.com/en-us/library/azure/ad/graph/howto/azure-ad-graph-api-differential-query) para sincronizar los datos que el trabajo web SyncData almacena en caché en una base de datos local.

EDUGraphAPI se basa en ASP.NET MVC y en este proyecto también se usa [ASP.NET Identity](https://www.asp.net/identity).

## Requisitos previos

**Para implementar y ejecutar este ejemplo se requiere**:
* Una suscripción de Azure con permisos para registrar una nueva aplicación y para implementar la aplicación web.
* Un inquilino de Office 365 Educación con Microsoft School Data Sync activado
* Uno de los siguientes exploradores: Microsoft Edge, Internet Explorer 9, Safari 5.0.6, Firefox 5, Chrome 13 o una versión posterior de alguno de estos exploradores. Además: Para desarrollar y ejecutar este ejemplo, se necesita lo siguiente:  
    * Visual Studio 2017 (cualquier edición).
	* Conocimientos de C#, aplicaciones web .NET, servicios web y programación JavaScript.

## Registrar la aplicación en Azure Active Directory

1. Inicie sesión en el nuevo portal de Azure: [https://portal.azure.com/](https://portal.azure.com/).

2. Para elegir el inquilino de Azure AD, seleccione una cuenta en la esquina superior derecha de la página:

   ![](Images/aad-select-directory.png)

3. Haga clic en **Azure Active Directory** -> **Registros de aplicaciones** -> **+Agregar**.

   ![](Images/aad-create-app-01.png)

4. Escriba un **nombre** y seleccione **Aplicación web/API** como **tipo de aplicación**.

   Introduzca la **URL de inicio de sesión**: https://localhost:44311/

   ![](Images/aad-create-app-02.png)

   Haga clic en **Crear**.

5. Cuando haya finalizado, se mostrará la aplicación en la lista.

   ![](/Images/aad-create-app-03.png)

6. Haga clic en ella para ver los detalles. 

   ![](/Images/aad-create-app-04.png)

7. Haga clic en **Toda la configuración**, si no se muestra la ventana de configuración.

   * Haga clic en **Propiedades** y luego establezca el campo **Multiinquilino** en **Sí**.

     ![](/Images/aad-create-app-05.png)

     Copie aparte el **Id. de aplicación** y después, haga clic en **Guardar**.

   * Haga clic en **Permisos necesarios**. Agregue los permisos siguientes:

     | API | Permisos de aplicación | Permisos delegados |
| ------------------------------ | ---------------------------------------- | ---------------------------------------- |
| Microsoft Graph | Leer los perfiles completos de todos los usuarios<br> Leer datos del directorio<br> Leer todos los grupos | Leer datos del directorio<br>Obtener acceso al directorio como usuario que ha iniciado sesión<br>Inicio de sesión de los usuarios<br> Obtener acceso total a todos los archivos a los que puede tener acceso el usuario<br> Obtener acceso total a los archivos de usuario<br> Leer las tareas de clase de los usuarios sin calificaciones<br> Leer y escribir las tareas de clase de los usuarios sin calificaciones<br> Leer las tareas de clase de los usuarios y sus calificaciones<br> Leer y escribir las tareas de clase de los usuarios y sus calificaciones |
| Windows Azure Active Directory | Leer datos del directorio | Iniciar sesión y leer el perfil de usuario<br>Leer y escribir datos en el directorio

     ![](/Images/aad-create-app-06.png)

     ​

     **Permisos de la aplicación**

     | Permiso | Descripción | Se requiere el consentimiento del administrador |
| ----------------------------- | ---------------------------------------- | ---------------------- |
| Leer los perfiles completos de todos los usuarios | Permite que la aplicación lea el conjunto completo de las propiedades de perfil, la pertenencia a grupos, los informes y los administradores de otros usuarios de la organización, sin necesidad de que un usuario haya iniciado sesión. | Sí |
| Leer datos del directorio | Permite que la aplicación lea datos en el directorio de la organización, como usuarios, grupos y aplicaciones, sin necesidad de que un usuario haya iniciado sesión. | Sí |

     **Permisos delegados**

     | Permiso | Descripción | Se requiere el consentimiento del administrador |
| -------------------------------------- | ---------------------------------------- | ---------------------- |
| Leer datos del directorio | Permite que la aplicación lea datos en el directorio de la organización, como usuarios, grupos y aplicaciones. | Sí |
| Acceso al directorio como el usuario que ha iniciado sesión | Permite a la aplicación tener el mismo acceso a la información del directorio que el usuario que ha iniciado sesión. | Sí |
| Inicio de sesión de los usuarios | Permite que los usuarios inicien sesión en la aplicación con sus cuentas profesionales o educativas y permite que la aplicación vea la información básica del perfil del usuario. | No |
| Iniciar sesión y leer el perfil del usuario | Permite que los usuarios inicien sesión en la aplicación y permite que la aplicación lea el perfil de los usuarios que hayan iniciado sesión. También permite que la aplicación lea la información básica de la empresa de los usuarios que hayan iniciado sesión. | No |
| Leer y escribir datos en el directorio | Permite que la aplicación lea y escriba datos en el directorio de la organización, como usuarios y grupos. No permite que la aplicación elimine usuarios o grupos ni que restablezca contraseñas de usuario. | Sí |

     ​

   * Haga clic en **Claves**y agregue una nueva clave:

     ![](Images/aad-create-app-07.png)

     Haga clic en **Guardar** y copie aparte el **VALOR** de la clave. 

   Cierre la ventana de configuración.

8. Haga clic en **Manifiesto**.

   ![](Images/aad-create-app-08.png)

   Inserte el siguiente código JSON en la matriz de **keyCredentials**.

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

   Haga clic en **Guardar**.

   > Nota: en este paso se configura la certificación usada por un trabajo web. Consulte la sección **Flujo de autenticación de aplicaciones** para obtener más información.

## Compilar y depurar localmente

Puede abrir este proyecto con la edición de Visual Studio 2017 que ya tiene o puede descargar e instalar la edición de la comunidad para ejecutar, compilar y desarrollar esta aplicación localmente.



Depurar **EDUGraphAPI.Web**:

1. Configure **appSettings** en **Web.config**. 

   ![](Images/web-app-config.png)

   - **ida:ClientId**: use el Id. de cliente del registro de la aplicación que creó anteriormente.
   - **ida:ClientSecret**: use el valor de la clave del registro de la aplicación que creó anteriormente.
   - **SourceCodeRepositoryURL**: use la URL del repositorio de la bifurcación.


2. Establezca **EDUGraphAPI.Web** como proyecto de inicio y presione F5. 

## Implementar el ejemplo en Azure

**Autorización de GitHub**

1. Generar el token

   - Abra https://github.com/settings/tokens en el explorador web.
   - Inicie sesión en la cuenta de GitHub donde haya bifurcado este repositorio.
   - Haga clic en **Generar token**
   - Escriba un valor en el cuadro de texto **Descripción del token**
   - Seleccione lo siguiente (las selecciones deben coincidir con la siguiente captura de pantalla):
        - repo (todas) -> repo:status, repo_deployment, public_repo
        - admin:repo_hook -> read:repo_hook

   ![](Images/github-new-personal-access-token.png)

   - Haga clic en **Generar token**
   - Copie el token

2. Agregar el token de GitHub a Azure en Azure Resource Explorer

   - Abra https://resources.azure.com/providers/Microsoft.Web/sourcecontrols/GitHub en el explorador web.
   - Inicie sesión con su cuenta de Azure.
   - Seleccionada la suscripción de Azure correcta.
   - Seleccione el modo **Lectura y escritura**.
   - Haga clic en **Editar**.
   - Pegue el token en el **parámetro de token**.

   ![](Images/update-github-token-in-azure-resource-explorer.png)

   - Haga clic en **COLOCAR**.

**Implementar los componentes de Azure desde GitHub**

1. Asegúrese de que la compilación pasa la compilación de VSTS.

2. Bifurque este repositorio en su cuenta de GitHub.

3. Haga clic en el botón Implementar en Azure:

   [![Implementar en Azure](http://azuredeploy.net/deploybutton.png)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FOfficeDev%2FO365-EDU-AspNetMVC-Samples%2Fmaster%2Fazuredeploy.json)

4. Rellene los valores de la página de implementación y seleccione la casilla de verificación **Acepto los términos y condiciones indicados anteriormente**.

   ![](Images/azure-auto-deploy.png)

   * **Grupo de recursos**: le recomendamos que cree un nuevo grupo.

   * **Nombre del sitio**: escriba un nombre. Por ejemplo, EDUGraphAPICanviz o EDUGraphAPI993.

     > Nota: Si el nombre que ha escrito está ocupado, obtendrá errores de validación:
     >
     > ![](Images/azure-auto-deploy-validation-errors-01.png)
     >
     > Si hace clic en él obtendrá más información, como, por ejemplo, que la cuenta de almacenamiento ya se encuentra en otro grupo de recursos o en otra suscripción.
     >
     > En ese caso, use otro nombre.

   * **URL del repositorio de códigos fuente**: reemplace <YOUR REPOSITORY> por el nombre del repositorio de la bifurcación.

   * **Integración manual del código fuente**: seleccione **falso**, ya que va a hacer la implantación desde su propia bifurcación.

   * **Id. de cliente**: use el Id. de cliente del registro de la aplicación que creó anteriormente.

   * **Secreto de cliente**: use el valor de la clave del registro de la aplicación que creó anteriormente.

   * Marque **Acepto los términos y condiciones indicados anteriormente**.

5. Haga clic en **Comprar**.

**Agregar una URL de respuesta al registro de la aplicación**

1. Después de la implementación, abra el grupo de recursos:

   ![](Images/azure-resource-group.png)

2. Haga clic en la aplicación web.

   ![](Images/azure-web-app.png)

   Copie la URL aparte y cambie el esquema a **https**. Esta es la URL de respuesta y se utilizará en el siguiente paso.

3. Vaya al registro de aplicaciones del nuevo portal de Azure y abra las ventanas de configuración.

   Agregue la URL de respuesta:

   ![](Images/aad-add-reply-url.png)

   > Nota: para depurar el ejemplo localmente, asegúrese de que https://localhost:44311/se encuentra en las URL de respuesta.

4. Haga clic en **GUARDAR**.

## Entender el código

### Introducción

**Diagrama de componentes de la solución**

![](Images/solution-component-diagram.png)

La capa superior de la solución contiene una aplicación web y una aplicación de consola de WebJob.

La capa del medio contiene dos proyectos de biblioteca de clases. 

Las capas inferiores contienen los tres orígenes de datos.

**EDUGraphAPI.Web**

Esta aplicación web está basada en una plantilla de proyecto de MVC de ASP.NET con la opción **Cuentas de usuario individuales** seleccionada. 

![](Images/mvc-auth-individual-user-accounts.png)

Los siguientes archivos se crearon con la plantilla MVC y solo se realizaron cambios secundarios:

1. **/App_Start/Startup.Auth.Identity.cs** (El nombre original es Startup.Auth.cs)
2. **/Controllers/AccountController.cs**

En este proyecto de ejemplo se usa**[ASP.NET Identity](https://www.asp.net/identity)** y **[Owin](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/owin)**. Estas dos tecnologías facilitan que puedan coexistir distintos métodos de autenticación. Para comprender este ejemplo, es importante estar familiarizado con estos componentes y tener conocimientos de ASP.NET MVC.

A continuación se muestran los archivos de clase importantes que se han usado en este proyecto web:

| Archivo | Descripción |
| --------------------------------- | ---------------------------------------- |
| /App_Start/Startup.Auth.AAD.cs | Se integra con la autenticación de Azure Active Directory |
| /Controllers/AdminController.cs | Contiene las acciones administrativas: <br>consentimiento del administrador, administrar cuentas vinculadas e instalar la aplicación. |
| /Controllers/LinkController.cs | Contiene las acciones para vincular cuentas de AD y de usuarios locales |
| /Controllers/SchoolsController.cs | Contiene las acciones para presentar datos educativos |

Esta aplicación web es una **aplicación multiinquilino**.  En AAD, hemos habilitado la opción:

![](Images/app-is-multi-tenant.png)

Los usuarios de cualquier inquilino de Azure Active Directory pueden obtener acceso a esta aplicación. Como esta aplicación usa algunos permisos de aplicación, primero debe iniciar sesión (otorgar consentimiento) un administrador del inquilino. De lo contrario, los usuarios tendrían un error:

![](Images/app-requires-admin-to-consent.png)

Para obtener más información, consulte [Crear una aplicación web SaaS multiinquilino con Azure AD y OpenID Connect](https://azure.microsoft.com/en-us/resources/samples/active-directory-dotnet-webapp-multitenant-openidconnect/).

**EDUGraphAPI.SyncData**

Este es el WebJob que se usa para sincronizar los datos de usuario. En el método **Functions.SyncUsersAsync**, se usa **UserSyncService** del proyecto EDUGraphAPI.Common.

El proyecto se creó para mostrar una consulta diferencial. Consulte la sección [Consulta diferencial](#differential-query) para obtener más información.

**EDUGraphAPI.Common**

En el proyecto de la biblioteca de clases se usa tanto **EDUGraphAPI.Web** como **EDUGraphAPI.SyncData**. 

En la siguiente tabla se muestran las carpetas del proyecto:

| Carpeta | Descripción |
| ------------------ | ---------------------------------------- |
| /Data | Contiene ApplicationDbContext y clases de entidad |
| /DataSync | Contiene la clase UserSyncSerextensionsvice usada por el WebJob EDUGraphAPI.SyncData |
| /DifferentialQuery | Contiene la clase DifferentialQueryService, que se usa para enviar una consulta diferencial y analizar el resultado. |
| /Extensions | Contiene una amplia variedad de métodos de extensión que simplifican la codificación para facilitar la lectura del código. | /Utils | Contiene la clase de amplio uso AuthenticationHelper.cs |

**Microsoft.Education**

Este proyecto contiene el cliente de la **[API de REST de centros educativos](https://msdn.microsoft.com/en-us/office/office365/api/school-rest-operations)**. La clase principal en este proyecto es **EducationServiceClient**.

### Acceso a los datos y modelos de datos

ASP.NET Identity usa [Entity Framework Code First](https://msdn.microsoft.com/en-us/library/jj193542(v=vs.113).aspx) para implementar todos sus mecanismos de persistencia. Para ello se consume el paquete [Microsoft.AspNet.Identity.EntityFramework](https://www.nuget.org/packages/Microsoft.AspNet.Identity.EntityFramework/). 

En este ejemplo, se ha creado **ApplicationDbContext** para obtener acceso a la base de datos. Se ha heredado de **IdentityDbContext**, que se ha definido en el paquete de NuGet mencionado anteriormente.

A continuación se muestran los modelos de datos importantes (y las propiedades importantes) que se usan en este ejemplo:

**ApplicationUsers**

Se ha heredado de **IdentityUser**. 

| Propiedad | Descripción |
| ------------- | ---------------------------------------- |
| Organización | El inquilino del usuario. Para un usuario local no vinculado, su valor es null |
| O365UserId | Se usa para vincular a una cuenta de Office 365 |
| O365Email | El correo electrónico de la cuenta de Office 365 vinculada |
| JobTitle | Se usa para mostrar la consulta diferencial |
| Departamento | Se usa para mostrar la consulta diferencial |
| Móvil | Se usa para mostrar la consulta diferencial |
| FavoriteColor | Se usa para mostrar datos locales |

**Organizaciones**

Una fila de esta tabla representa un espacio empresarial en AAD.

| Propiedad | Descripción |
| ---------------- | ------------------------------------ |
| TenantId | GUID del inquilino |
| Nombre | Nombre del inquilino |
| IsAdminConsented | Es el inquilino aceptado por un administrador |

### Flujo de consentimiento del administrador

Los permisos de solo aplicación siempre requieren el consentimiento del administrador de inquilinos. Si la aplicación solicita un permiso de solo aplicación y un usuario intenta iniciar sesión en la aplicación, aparecerá un mensaje de error que indica que el usuario no puede dar su consentimiento.

Algunos permisos delegados también requieren el consentimiento del administrador de inquilinos. Por ejemplo, la posibilidad de reescribir en Azure AD como el usuario que ha iniciado la sesión requiere el consentimiento del administrador de inquilinos. Al igual que los permisos de solo aplicación, si un usuario ordinario intenta iniciar sesión en una aplicación que solicita un permiso delegado que requiere el consentimiento del administrador, la aplicación recibirá un error. Que un permiso requiera o no el consentimiento del administrador viene determinado por el desarrollador que publica el recurso, y se puede encontrar en la documentación del recurso.

Si la aplicación usa permisos que requieren el consentimiento del administrador, necesita tener un gesto, como un botón o un vínculo donde el administrador pueda iniciar la acción. La solicitud que la aplicación envía para esta acción es una solicitud de autorización habitual de OAuth2 u OpenID Connect que también incluye el parámetro de cadena de consulta `prompt=admin_consent`. Una vez que el administrador ha dado su consentimiento y la entidad de servicio se crea en el inquilino del cliente, las posteriores solicitudes de inicio de sesión no necesitan el parámetro `prompt=admin_consent`. Dado que el administrador ha decidido que los permisos solicitados son aceptables, en adelante no se solicitará consentimiento a ningún otro usuario.



### Flujos de autenticación

Hay cuatro flujos de autenticación en este proyecto.

Los primeros 2 flujos (Inicio de sesión local/Inicio de sesión de Office 365) permiten a los usuarios iniciar sesión con una cuenta local o con una cuenta de Office 365 y, después, establecer un vínculo a la cuenta del otro tipo. Este procedimiento se implementa en LinkController.

**Flujo de autenticación de inicio de sesión local**

![](Images/auth-flow-local-login.png)

**Flujo de autenticación de inicio de sesión de Office 365**

![](Images/auth-flow-o365-login.png)

**Flujo de autenticación de inicio de sesión del administrador**

Este flujo muestra cómo inicia sesión un administrador en el sistema y cómo realiza operaciones administrativas.

Cuando el administrador inicie sesión en la aplicación con una cuenta de Office 365, se le pedirá que se vincule a la cuenta local. Este paso no es obligatorio y puede omitirse. 

Como mencionamos anteriormente, la aplicación web es una aplicación multiinquilino que usa algunos permisos de aplicación, por lo que es necesario un administrador del inquilino acepte primero el espacio empresarial.  

Este flujo se implementa en AdminController. 

![](Images/auth-flow-admin-login.png)

**Flujo de autenticación de la aplicación**

Este flujo se implementa en el WebJob de SyncData.

![](Images/auth-flow-app-login.png)

Se usa un certificado X509. Para obtener más información, consulte los siguientes vínculos:

* [Aplicación de servidor o de Daemon en API web](https://docs.microsoft.com/en-us/azure/active-directory/active-directory-authentication-scenarios#daemon-or-server-application-to-web-api)
* [Autenticación en Azure AD en aplicaciones Daemon con certificados](https://azure.microsoft.com/en-us/resources/samples/active-directory-dotnet-daemon-certificate-credential/)
* [Crear aplicaciones de servicio y aplicaciones Daemon en Office 365](https://msdn.microsoft.com/en-us/office/office365/howto/building-service-apps-in-office-365)

### Dos tipos de API de Graph

En este ejemplo se usan dos API de Graph distintas:

|
| [API de Azure AD Graph](https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-graph-api) | [API de Microsoft Graph](https://graph.microsoft.io/) |
| ------------ | ---------------------------------------- | ---------------------------------------- |
| Descripción | La API de Azure Active Directory Graph proporciona acceso mediante programación a Azure Active Directory a través de los puntos de conexión de la API de REST. Las aplicaciones pueden usar la API de Azure AD Graph para realizar operaciones de creación, lectura, actualización y eliminación (CRUD) en datos del directorio y objetos del directorio, como usuarios, grupos y contactos de la organización | Una API unificada que también incluye las API de otros servicios de Microsoft, como Outlook, OneDrive, OneNote, Planner y Office Graph, a los que se accede a través de un único punto de conexión con un único token de acceso. |
| Cliente | Install-Package [Microsoft.Azure.ActiveDirectory.GraphClient](https://www.nuget.org/packages/Microsoft.Azure.ActiveDirectory.GraphClient/) | Install-Package [Microsoft.Graph](https://www.nuget.org/packages/Microsoft.Graph/) |
| Punto de conexión | https://graph.windows.net | https://graph.microsoft.com |
| API Explorer | https://graphexplorer.cloudapp.net/| https://graph.microsoft.io/graph-explorer |

En este ejemplo se usan las siguientes clases, que se basan en una interfaz común, para demostrar cómo se relacionan las API:  

![](Images/class-diagram-graphs.png)

La interfaz **IGraphClient** define dos métodos: **GeCurrentUserAsync** y **GetTenantAsync**.

**MSGraphClient** implementa la interfaz **IGraphClient** con las bibliotecas de cliente de Microsoft Graph.

La interfaz y la clase de cliente de Graph se encuentran en la carpeta **Services/GraphClients** de la aplicación web. A continuación se resalta parte del código para mostrar cómo obtener el usuario y el inquilino.

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

Tenga en cuenta que, en la configuración de registro de la aplicación, los permisos para cada API de Graph se configuran por separado:

![](/Images/aad-create-app-06.png) 

### API de Office 365 Educación

Las [API de Office 365 Educación](https://msdn.microsoft.com/office/office365/api/school-rest-operations) ayudan a extraer datos del inquilino de Office 365, que Microsoft School Data Sync ha sincronizado con la nube. Estos resultados proporcionan información sobre centros educativos, secciones, profesores, alumnos y listas. La API de REST de centros educativos proporciona acceso a entidades escolares en Office 365 para los inquilinos de educación.

En el ejemplo, el proyecto de biblioteca de clases **Microsoft.Education** se ha creado para contener la API de Office 365 Educación. 

**EducationServiceClient** es la clase principal de la biblioteca. Con ella podemos obtener datos académicos con facilidad.

**Obtener centros educativos**

~~~c#
public async Task<School[]> GetSchoolsAsync()
{
    var schools = await HttpGetArrayAsync<EducationSchool>("education/schools");
    return schools.ToArray();

}
~~~



**Obtener secciones

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



A continuación, se muestran algunas capturas de pantalla de la aplicación de ejemplo donde se muestran datos académicos.

![](Images/edu-schools.png)



![](Images/edu-classes.png)

![](Images/edu-class.png)

En **EducationServiceClient**, se han creado tres métodos privados con el prefijo HttpGet para simplificar la llamada a las API de REST.

* **HttpGetAsync**: envía una solicitud HTTP GET al extremo de destino y devuelve la cadena de respuesta JSON. Se incluye un token de acceso en el encabezado de autenticación de portador.
* **HttpGetObjectAsync<T>**: deserializa la cadena JSON devuelta por HttpGetAsync al tipo de destino T y devuelve el objeto de resultado.
* **HttpGetObjectAsync<T>**: deserializa la cadena JSON devuelta por HttpGetAsync al tipo de matriz de destino T[] y devuelve la matriz.

### Consulta diferencial

Una solicitud de [consulta diferencial](https://msdn.microsoft.com/en-us/Library/Azure/Ad/Graph/howto/azure-ad-graph-api-differential-query) devuelve todos los cambios realizados en las entidades especificadas durante el tiempo entre dos solicitudes consecutivas. Por ejemplo, si hace una solicitud de consulta diferencial una hora después de la solicitud de consulta diferencial anterior, solo se devolverán los cambios realizados durante esa hora. Esta funcionalidad es especialmente útil al sincronizar los datos del directorio del inquilino con el almacén de datos de una aplicación.

El código relacionado está en las dos siguientes carpetas del proyecto **EDUGraphAPI.Common**:

* **/DifferentialQuery**: contiene clases para enviar una consulta diferencial y analizar el resultado diferencial.
* **/DataSync**: contiene clases que sirven para mostrar cómo se sincronizan los usuarios.

> Tenga en cuenta que las clases de la carpeta **DifferentialQuery** usan tecnologías .NET avanzadas. Céntrese en el uso de estas clases y no en los detalles de implementación.

Para sincronizar usuarios, hemos definido la clase de usuario:

~~~c#
public class User
{
    public string ObjectId { get; set; }
    public virtual string JobTitle { get; set; }
    public virtual string Department { get; set; }
    public virtual string Mobile { get; set; }
}
~~~

Observe que las propiedades modificables *JobTitle*, *Department* y *Mobile* son virtuales. Las clases de la carpeta **DifferentialQuery** crearán un tipo de proxy para el tipo de usuario y reemplazarán estas propiedades virtuales para hacer el seguimiento de cambios.

En la clase **UserSyncService**, mostramos cómo usar el **DifferentialQueryService** para enviar una consulta diferencial y obtener un resultado diferencial.

```c#
var differentialQueryService = new DifferentialQueryService(/**/);
DeltaResult<Delta<User>> result = await differentialQueryService.QueryAsync<User>(url);
```
Y cómo actualizar (o eliminar) usuarios en la base de datos local con el resultado diferencial:

~~~c#
foreach (var differentialUser in result.Items)
    await UpdateUserAsync(differentialUser);
//...
private async Task UpdateUserAsync(Delta<User> differentialUser) { /**/ }
~~~

El modelo de datos **DataSyncRecord** se usa para los vínculos diferenciales persistentes.

A continuación se muestra el registro generado por el WebJob de SyncData:

![](Images/sync-data-web-job-log.png) 

### Filtros

En la carpeta **/Infrastructure** del proyecto web, hay varios atributos de filtro.

**EduAuthorizeAttribute**

Este es un filtro de autorización heredado de AuthorizeAttribute.

Se ha creado para permitir que la aplicación web redirija a los usuarios a la página de inicio de sesión adecuada en nuestro escenario de varios métodos de autenticación.

Hemos invalidado el método **HandleUnauthorizedRequest** para redirigir al usuario a /Account/Login:

~~~c#
protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
{
    filterContext.Result = new RedirectResult("/Account/Login");
}
~~~

**HandleAdalExceptionAttribute**

La clase **AuthenticationHelper** muestra una gran cantidad de métodos que devuelven los tokens de acceso o la instancia de un cliente de API. La mayoría de estos métodos invocan **[AuthenticationContext.AcquireTokenSilentAsync](https://msdn.microsoft.com/en-us/library/mt473642.aspx)** de forma interna. Por lo general, **AcquireTokenSilentAsync** obtiene el token de acceso correctamente, ya que **ADALTokenCache** almacena los tokens en caché en la base de datos. 

En algunas situaciones, como, por ejemplo, cuando el token almacenado en caché ha caducado o cuando se solicita un nuevo token de recurso, **AcquireTokenSilentAsync** producirá **AdalException**. Se requiere **HandleAdalExceptionAttribute** para administrar **AdalException** y es necesario dirigir al usuario hasta el punto de conexión de autenticación para obtener un nuevo token.

En algunos casos, se redirigirá al usuario directamente al punto de conexión de autenticación mediante la siguiente invocación:

~~~c#
filterContext.HttpContext.GetOwinContext().Authentication.Challenge(
   new AuthenticationProperties { RedirectUri = requestUrl },
   OpenIdConnectAuthenticationDefaults.AuthenticationType);
~~~

Y, en otros casos, queremos mostrar al usuario la siguiente página para decirle al usuario el motivo por el que se le ha redirigido, sobre todo si se trata de un usuario que haya iniciado sesión con una cuenta local.

![](Images/web-app-login-o365-required.png)

Se usa un modificador para controlar esto. El valor del modificador lo recupera:

~~~c#
//public static readonly string ChallengeImmediatelyTempDataKey = "ChallengeImmediately";
var challengeImmediately = filterContext.Controller.TempData[ChallengeImmediatelyTempDataKey];
~~~

Si el valor es verdadero, se redirigirá al usuario al punto de conexión de autenticación inmediatamente. De lo contrario, se mostrará en primer lugar la página anterior y el usuario hará clic en el botón de inicio de sesión para continuar.

**LinkedOrO365UsersOnlyAttribute**

Este es otro filtro de autorización. Con él podemos permitir solo a los usuarios vinculados y los usuarios de Office 365 que visiten las acciones y los controladores protegidos.

~~~c#
protected override bool AuthorizeCore(HttpContextBase httpContext)
{
    var applicationService = DependencyResolver.Current.GetService<ApplicationService>();
    var user = applicationService.GetUserContext();
    return user.AreAccountsLinked || user.IsO365Account;
}
~~~

A los usuarios no autorizados se les mostrará la página NoAccess:

~~~c#
protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
{
    filterContext.Result = new ViewResult { ViewName = "NoAccess" };
}
~~~

Por el momento, solo se usa en **SchoolsController**.

### Clases principales

**Microsoft.Education**

* `EducationServiceClient`: una instancia de la clase controla las solicitudes de creación, su envío a la API de Office 365 Educación y el procesamiento de las respuestas.

  | Método | Descripción |
| ------------------- | ---------------------------------------- |
| GetSchoolsAsync | Obtener todos los centros educativos que existen en el inquilino de Azure Active Directory |
| GetSchoolAsync | Obtener un centro educativo con el Id. de objeto |
| GetAllSectionsAsync | Obtener secciones de un centro educativo |
| GetMySectionsAsync | Obtener mis secciones de un centro educativo|
| GetSectionAsync | Obtener una sección con el Id. de objeto |
| GetMembersAsync | Obtener miembros de un centro educativo |
| GetStudentAsync | Obtener el usuario que ha iniciado sesión como estudiante |
| GetTeacherAsync | Obtener el usuario que ha iniciado sesión como profesor |

**EDUGraphAPI.Common**

* **`Data.ApplicationUser`**: una instancia de la clase representa un usuario.

* **`Data.Organization`**: una instancia de la clase representa un inquilino en Azure AD. 

* **`Data.ApplicationDbContext`**: clase DbContext usada por Entity Framework, heredada de `IdentityDbContext<ApplicationUser>`.

* **`DataSync.User`**: na instancia de la clase representa un usuario en Azure AD. Observe que las propiedades que se usan para realizar el seguimiento de los cambios son virtuales.

* **`DataSync.UserSyncService`**: una instancia de la clase controla la sincronización de los usuarios de la base de datos local con la consulta diferencial. Invoque el método `SyncAsync` para empezar a sincronizar usuarios.

* **`DifferentialQuery.DifferentialQueryService`**: una instancia de la clase controla la solicitud de compilación, la envía al punto de conexión de servicio y procesa las respuestas. Invocar el método `QueryAsync` con un deltaLink para iniciar una consulta diferencial. La clase `DeltaResultParser` convertirá el resultado diferencial en `DeltaResult<Delta<TEntity>>`.

* **`Utils.AuthenticationHelper`**: clase auxiliar estática que se usa para obtener el token de acceso, el resultado de la autenticación, el contexto de autenticación y las instancias del cliente de servicio.

  | Method | Description |
| -------------------------------------- | ---------------------------------------- |
| GetActiveDirectoryClientAsync | Obtener una instancia de ActiveDirectoryClient |
| GetGraphServiceClientAsync | Obtener una instancia de GraphServiceClient |
| GetEducationServiceClientAsync | Obtener una instancia de EducationServiceClient |
| GetActiveDirectoryClient | Obtener una instancia de ActiveDirectoryClient desde el AuthenticationResult especificado |
| GetGraphServiceClient | Obtener una instancia de GraphServiceClient desde el AuthenticationResult especificado |
| GetAccessTokenAsync | Obtener un token de acceso desde el recurso especificado |
| GetAuthenticationResult | Obtener un AuthenticationResult desde el recurso especificado |
| GetAuthenticationContext | Obtener una instancia de AuthenticationContext |
| GetAuthenticationResultAsync | Obtener un AuthenticationResult desde el código de autorización especificado |
| GetAppOnlyAccessTokenForDaemonAppAsync | Obtener un token de acceso de solo aplicaciones para una aplicación Daemon |

  La mayoría de los métodos anteriores tienen un argumento denominado permiso. El tipo es `Permissions`, un tipo de enumeración con dos valores definidos:

  * `Delegated`: el cliente accede a la API web como el usuario que ha iniciado sesión.
  * `Application`: el cliente accede a la API web directamente como sí mismo (sin contexto de usuario). Este tipo de permiso requiere el consentimiento del administrador.

* **`Utils.AuthenticationHelper`**: una clase estática que se usa para crear una URL de autorización. `GetUrl` es el único método definido en la clase.

* **`Constants`**: una clase estática contiene valores de configuración de la aplicación y otros valores constantes.

**EDUGraphAPI.Web**

* **`Controllers.AccountController`**: contiene acciones para que el usuario se registre, inicie sesión y cambie la contraseña.

* **`Controllers.AdminController`**: implementa el **flujo de autenticación de inicio de sesión del administrador**. Consulte la sección [Flujos de autenticación](#authentication-flows) para obtener más información.

* **`Controllers.LinkController`**: implementa el **flujo de autenticación de inicio de sesión local y de Office 365**. Consulte la sección [Flujos de autenticación](#authentication-flows) para obtener más información.

* **`Controllers.SchoolsController`**: contiene acciones para mostrar centros educativos y clases. Este controlador usa principalmente la clase `SchoolsService`. Consulte la sección [API de Office 365 Educación](#office-365-education-api) para obtener más información.

* **`Infrastructure.EduAuthorizeAttribute`**: permite que la aplicación web redirija al usuario actual a la página de inicio de sesión adecuada en nuestro escenario de varios métodos de autenticación. Consulte la sección [Filtros](#filters) para obtener más información.

* **`Infrastructure.HandleAdalExceptionAttribute`**: controla AdalException y dirige al usuario al punto de conexión autorizado o /Link/LoginO365Required. Consulte la sección [Filtros](#filters) para obtener más información.

* **`Infrastructure.LinkedOrO365UsersOnlyAttribute`**: permite solo a los usuarios vinculados y los usuarios de Office 365 que visiten las acciones y los controladores protegidos. Consulte la sección [Filtros](#filters) para obtener más información.

* **`Models.UserContext`**: contexto para el usuario que ha iniciado sesión.

* **`Services.GraphClients.AADGraphClient`**: implementa la interfaz `IGraphClient` con la API de Azure AD Graph. Consulte la sección [Dos tipos de API de Graph](#two-kinds-of-graph-api) para obtener más información.

* **`Services.GraphClients.MSGraphClient`**: implementa la interfaz `IGraphClient` con la API de Microsoft Graph. Consulte la sección [Dos tipos de API de Graph](#two-kinds-of-graph-api) para obtener más información.

* **`Services.ApplicationService.`**: una instancia de la clase controla la obtención y actualización de usuarios y organizaciones.

  | Método | Descripción |
| ------------------------------- | ---------------------------------------- |
| CreateOrUpdateOrganizationAsync | Crear o actualizar la organización |
| GetAdminContextAsync | Obtener el contexto del administrador actual |
| GetCurrentUser | Obtener el usuario actual |
| GetCurrentUserAsync | Obtener el usuario actual |
| GetUserAsync | Obtener el usuario por Id. |
| GetUserContext | Obtener el contexto del usuario actual |
| GetUserContextAsync | Obtener el contexto del usuario actual |
| GetLinkedUsers | Obtener los usuarios vinculados con el filtro especificado |
| IsO365AccountLinkedAsync | Es la cuenta de Office 365 especificada que se ha vinculado a una cuenta local |
| SaveSeatingArrangements | Guardar disposiciones de sala |
| UnlinkAccountsAsync | Desvincular la cuenta especificada |
| UnlinkAllAccounts | Desvincular todas las cuentas del inquilino especificado |
| UpdateLocalUserAsync | Actualizar el usuario local con la información del inquilino y el usuario de Office 365 |
| UpdateOrganizationAsync | Actualizar la organización |
| UpdateUserFavoriteColor | Actualizar el color favorito del usuario actual |

* **`Services.SchoolsService`**: una clase de servicio que se usa para obtener datos académicos.

  | Método | Descripción |
| ------------------------------- | ---------------------------------------- |
| GetSchoolsViewModelAsync | Obtener SchoolsViewModel |
| GetSchoolUsersAsync | Obtener los profesores y alumnos del centro educativo especificado |
| GetSectionsViewModelAsync | Obtener SectionsViewModel del centro educativo especificado|
| GetSectionDetailsViewModelAsync | Obtener SectionDetailsViewModel de la sección especificada |
| GetMyClasses | Obtener mis clases |

**EDUGraphAPI.SyncData**

* **`Functions`**: contiene el método `SyncUsersAsync` que se ejecuta regularmente par sincronizar los datos de usuario.
* **`Program`**: contiene el método `Main` que configura e inicia el host de WebJob.

## [Opcional] Compilar y depurar el WebJob localmente.

Depurar **EDUGraphAPI.SyncData**:

1. Cree una cuenta de almacenamiento en Azure y obtenga la cadena de conexión.
   > Nota:
   El depurado local con Azure Storage Emulator se admitirá después de [Azure WebJobs SDK](https://github.com/Azure/azure-webjobs-sdk) V2. Para obtener más información, consulte [Soporte de Azure Storage Emulator](https://github.com/Azure/azure-webjobs-sdk/issues/53).
   **No** se recomienda para la depuración local cuando el trabajo web publicado se ejecuta en Azure con la misma cuenta de almacenamiento. Consulte [esta pregunta](http://stackoverflow.com/questions/42020647/what-happened-when-using-same-storage-account-for-multiple-azure-webjobs-dev-li) para obtener más información.

2. Configure **App.config**:

   ![](Images/webjob-app-config.png)

   - **Cadenas de conexión**:
     - **AzureWebJobsDashboard**: use la cadena de conexión que obtuvo en el paso anterior.
     - **AzureWebJobsStorage**: use la cadena de conexión que obtuvo en el paso anterior.
   - **Configuración de la aplicación**:
     - *ida:ClientId**: use el Id. de cliente del registro de la aplicación que creó anteriormente.

3. Establezca **EDUGraphAPI.SyncData** como proyecto de inicio y presione F5. 

## Preguntas y comentarios

* Si tiene algún problema para ejecutar este ejemplo, [registre un problema](https://github.com/OfficeDev/O365-EDU-AspNetMVC-Samples/issues).
* Las preguntas generales sobre el desarrollo de GraphAPI deben publicarse en [Stack Overflow](http://stackoverflow.com/questions/tagged/office-addins). Asegúrese de que sus preguntas o comentarios se etiquetan con [ms-graph-api]. 

## Colaboradores

Le animamos a contribuir a nuestros ejemplos. Para obtener instrucciones sobre cómo continuar, consulte [nuestra guía de contribución](/Contributing.md)

Este proyecto ha adoptado el [Código de conducta de código abierto de Microsoft](https://opensource.microsoft.com/codeofconduct/). Para obtener más información, vea [Preguntas frecuentes sobre el código de conducta](https://opensource.microsoft.com/codeofconduct/faq/) o póngase en contacto con [opencode@microsoft.com](mailto:opencode@microsoft.com) si tiene otras preguntas o comentarios.



**Copyright (c) 2017 Microsoft. Reservados todos los derechos.**
