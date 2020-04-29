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
# EDUGraphAPI — пример кода Office 365 для образования (ASP.NET MVC)

В этом примере показано, как выполнить интеграцию с учебными ролями или данными списков, а также службами Office 365, доступными с помощью API Graph. 

Данные учебного заведения синхронизируются в клиентах Office 365 для образования с помощью [синхронизации сведений о школе (Майкрософт)](http://sds.microsoft.com).  

**Оглавление**
* [Примеры целей](#sample-goals)
* [Предварительные требования](#prerequisites)
* [Регистрация приложения в Azure Active Directory](#register-the-application-in-azure-active-directory)
* [Сборка и отладка в локальной среде](#build-and-debug-locally)
* [Развертывание примера в Azure](#deploy-the-sample-to-azure)
* [Разбор кода](#understand-the-code)
* [[Необязательно] Сборка и отладка веб-задания в локальной среде](#optional-build-and-debug-the-webjob-locally)
* [Вопросы и комментарии](#questions-and-comments)
* [Участие](#contributing)

## Примеры целей

В примере показано:

* Вызов API Graph, включая:

  * [API Graph Microsoft Azure Active Directory](https://www.nuget.org/packages/Microsoft.Azure.ActiveDirectory.GraphClient/)
  * [API Microsoft Graph](https://www.nuget.org/packages/Microsoft.Graph/)

* Связывание локально управляемых учетных записей пользователей и учетных записей Office 365 (Azure Active Directory). 

  После связывания учетных записей пользователи могут использовать локальные учетные записи или учетные записи Office 365 для входа в пример веб-сайта и его использования.

* Получение учебных заведений, разделов, преподавателей и учащихся из Office 365 для образования:

  * [Справочник по API REST учебных заведений для Office 365](https://msdn.microsoft.com/office/office365/api/school-rest-operations)
  * Для синхронизации данных, кэшированных в локальной базе данных с помощью веб-задания SyncData, используется [разностный запрос](https://msdn.microsoft.com/en-us/library/azure/ad/graph/howto/azure-ad-graph-api-differential-query).

EDUGraphAPI основывается на ASP.NET MVC. В этом проекте также используется [ASP.NET Identity](https://www.asp.net/identity).

## Предварительные требования

**Для развертывания и работы этого примера требуется следующее**:
* Подписка на Azure с разрешениями на регистрацию нового приложения и развертывание веб-приложения.
* Клиент Office 365 для образования с поддержкой синхронизации сведений о школе (Майкрософт).
* Один из следующих
браузеров: Microsoft Edge, Internet Explorer 9, Safari 5.0.6, Firefox 5, Chrome 13 или более поздние версии этих браузеров. Кроме того, для разработки и работы этого примера в локальной среде требуется следующее:  
    * Visual Studio 2017 (любой выпуск).
	* Знакомство с C#, веб-приложениями .NET, программированием JavaScript и веб-службами.

## Регистрация приложения в Azure Active Directory

1. Войдите на новый портал Azure по адресу [https://portal.azure.com](https://portal.azure.com/).

2. Выберите клиент Azure AD, щелкнув имя своей учетной записи в правом верхнем углу страницы:

   ![](Images/aad-select-directory.png)

3. Щелкните **Azure Active Directory** -> **Регистрация приложений** -> **+Добавить**.

   ![](Images/aad-create-app-01.png)

4. Введите **Имя** и выберите **Веб-приложение или API** в качестве **типа приложения**.

   Укажите **URL-адрес входа**: https://localhost:44311/

   ![](Images/aad-create-app-02.png)

   Нажмите **Создать**.

5. После завершения приложение отобразится в списке.

   ![](/Images/aad-create-app-03.png)

6. Щелкните его, чтобы просмотреть сведения о нем. 

   ![](/Images/aad-create-app-04.png)

7. Щелкните **Все параметры**, если окно параметров не отобразилось.

   * Нажмите **Свойства** и присвойте параметру **Мультитенантный** значение **Да**.

     ![](/Images/aad-create-app-05.png)

     Скопируйте **Идентификатор приложения** и нажмите кнопку **Сохранить**.

   * Щелкните **Необходимые разрешения**. Добавьте указанные ниже разрешения.

     | API | Разрешения приложений | Делегированные разрешения |
| ------------------------------ | ---------------------------------------- | ---------------------------------------- |
| Microsoft Graph | Чтение полных профилей всех пользователей<br> Чтение данных каталога<br> Чтение всех групп | Чтение данных каталога<br>Доступ к каталогу, аналогичный доступу вошедшего пользователя<br>Вход пользователей<br> Полный доступ ко всем файлам, доступным пользователю<br> Полный доступ к файлам пользователя<br> Чтение заданий для курсов пользователей без оценок<br> Чтение и запись заданий для курсов пользователей без оценок<br> Чтение заданий для курсов пользователей и соответствующих оценок<br> Чтение и запись заданий для курсов пользователей и соответствующих оценок |
| Windows Azure Active Directory | Чтение данных каталога | Вход и чтение профиля пользователя<br>Чтение и запись данных каталога |

     ![](/Images/aad-create-app-06.png)

     ​

     **Разрешения приложений**

     | Разрешение | Описание | Требуется ли согласие администратора |
| ----------------------------- | ---------------------------------------- | ---------------------- |
| Чтение полных профилей всех пользователей | Приложение сможет просматривать полный набор свойств профиля, данные об участии в группах, сведения о подчиненных и руководителях других пользователей в организации без входа пользователя. | Да |
| Чтение данных каталога | Позволяет приложению читать данные в каталоге вашей организации, такие как группы, пользователи и приложения без необходимости входа пользователя. | Да |

     **Делегированные разрешения**

     | Разрешение | Описание | Требуется ли согласие администратора |
| -------------------------------------- | ---------------------------------------- | ---------------------- |
| Чтение данных каталога | Позволяет приложению читать данные в каталоге вашей организации, такие как сведения о пользователях, группах и приложениях. | Да |
| Доступ к каталогу, аналогичный доступу вошедшего пользователя. | Предоставляет приложению такой же доступ к информации в каталоге, как у вошедшего пользователя. | Да |
| Вход пользователей | Пользователи смогут входить в приложение с помощью своей рабочей или учебной учетной записи, а приложение сможет просматривать основные данные профилей пользователей. | Нет |
| Вход и чтение профиля пользователя | Позволяет пользователям входить в приложение, а приложению — просматривать профили вошедших пользователей. Кроме того, позволяет приложению считывать основные сведения о компании вошедших пользователей. | Нет |
| Чтение и запись данных каталога | Приложение сможет просматривать и записывать данные в каталоге организации, например данные пользователей и групп. Не позволяет приложению удалять пользователей и группы или сбрасывать пароли пользователей. | Да |

     ​

   * Щелкните пункт **Ключи** и добавьте новый ключ:

     ![](Images/aad-create-app-07.png)

     Нажмите **Сохранить**, затем скопируйте **ЗНАЧЕНИЕ** ключа. 

   Закройте окно параметров.

8. Щелкните **Манифест**.

   ![](Images/aad-create-app-08.png)

   Вставьте следующий код JSON в массив **keyCredentials**.

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

   Нажмите кнопку **Сохранить**.

   > Примечание. На этом этапе настраивается сертификация, используемая веб-заданием. Дополнительные сведения см. в разделе **Поток проверки подлинности приложения**.

## Сборка и отладка в локальной среде

Этот проект можно открыть с помощью выпуска Visual Studio 2017, который у вас уже есть, или скачать и установить выпуск Community, чтобы запускать, создавать или разрабатывать это приложение в локальной среде.



Выполните отладку **EDUGraphAPI.Web**:

1. Настройте **appSettings** в **Web.config**. 

   ![](Images/web-app-config.png)

   - **ida:ClientId**: используйте идентификатор клиента из регистрации приложения, созданный ранее.
   - **ida:ClientSecret**: используйте значение ключа из регистрации приложения, созданное ранее.
   - **SourceCodeRepositoryURL**: используйте URL-адрес репозитория вашего разветвления.


2. Настройте **EDUGraphAPI.Web** в качестве начального проекта и нажмите клавишу F5. 

## Развертывание примера в Azure

**Авторизация GitHub**

1. Создание маркера

   - Откройте страницу https://github.com/settings/tokens в веб-браузере.
   - Войдите в свою учетную запись GitHub, где находится разветвление этого репозитория.
   - Нажмите кнопку **Создать маркер**
   - Введите значение в текстовое поле **Описание маркера**
   - Выберите следующее (выбранные параметры должны соответствовать снимку экрана ниже):
        - repo (all) -> repo:status, repo\_deployment, public\_repo
        - admin:repo\_hook -> read:repo\_hook

   ![](Images/github-new-personal-access-token.png)

   - Нажмите кнопку **Создать маркер**
   - Скопируйте маркер

2. Добавление маркера GitHub в Azure в обозревателе ресурсов Azure

   - Откройте страницу https://resources.azure.com/providers/Microsoft.Web/sourcecontrols/GitHub в веб-браузере.
   - Войдите с помощью учетной записи Azure.
   - Выберите правильную подписку Azure.
   - Выберите режим **Чтение и запись**.
   - Нажмите кнопку **Изменить**.
   - Вставьте маркер в **параметр маркера**.

   ![](Images/update-github-token-in-azure-resource-explorer.png)

   - Щелкните **PUT**

**Развертывание компонентов Azure из GitHub**

1. Убедитесь, что сборка передает сборку VSTS.

2. Создайте разветвление этого репозитория в свою учетную запись GitHub.

3. Нажмите кнопку "Развернуть в Azure":

   [![Развернуть в Azure](http://azuredeploy.net/deploybutton.png)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FOfficeDev%2FO365-EDU-AspNetMVC-Samples%2Fmaster%2Fazuredeploy.json)

4. Заполните значения на странице развертывания и установите флажок **Я принимаю указанные выше условия**.

   ![](Images/azure-auto-deploy.png)

   * **Группа ресурсов**: рекомендуется создать новую группу.

   * **Имя сайта**: укажите имя. Например: EDUGraphAPICanviz или EDUGraphAPI993.

     > Примечание. Если указанное имя занято, возникнут ошибки проверки:
     >
     > ![](Images/azure-auto-deploy-validation-errors-01.png)
     >
     > Щелкнув по этому сообщению, вы получите дополнительные сведения, например, что учетная запись хранения уже используется в другой группе ресурсов или подписке.
     >
     > В таком случае используйте другое имя.

   * **URL-адрес репозитория исходного кода**: замените <YOUR REPOSITORY> именем репозитория вашего разветвления.

   * **Ручная интеграция исходного кода**: выберите **false**, так как вы развертываете из собственного разветвления.

   * **Идентификатор клиента**: используйте идентификатор клиента из регистрации приложения, созданный ранее.

   * **Секрет клиента**: используйте значение ключа из регистрации приложения, созданное ранее.

   * Установите флажок **Я принимаю указанные выше условия**.

5. Нажмите **Купить**.

**Добавление URL-адреса ответа в регистрацию приложения**

1. После развертывания откройте группу ресурсов:

   ![](Images/azure-resource-group.png)

2. Щелкните веб-приложение.

   ![](Images/azure-web-app.png)

   Скопируйте URL-адрес и измените схему на **https**. Это URL-адрес ответа, который будет использоваться на следующем шаге.

3. Перейдите к регистрации приложения на новом портале Azure и откройте окно параметров.

   Добавьте URL-адрес ответа:

   ![](Images/aad-add-reply-url.png)

   > Примечание. Чтобы выполнить отладку примера локально, укажите https://localhost:44311/ в URL-адресах ответа.

4. Нажмите кнопку **Сохранить**.

## Разбор кода

### Введение

**Схема компонентов решения**

![](Images/solution-component-diagram.png)

Верхний уровень решения содержит веб-приложение и консольное приложение веб-заданий.

Средний уровень содержит два проекта библиотеки классов. 

Нижний уровень содержит три источника данных.

**EDUGraphAPI.Web**

Это веб-приложение основано на шаблоне проекта ASP.NET MVC с выбранным параметром **Учетные записи отдельных пользователей**. 

![](Images/mvc-auth-individual-user-accounts.png)

Следующие файлы был созданы с помощью шаблона MVC с внесением небольших изменений:

1. **/App\_Start/Startup.Auth.Identity.cs** (исходное имя — Startup.Auth.cs)
2. **/Controllers/AccountController.cs**

В этом примере проекта используется **[ASP.NET Identity](https://www.asp.net/identity)** и **[Owin](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/owin)**. Эти две технологии упрощают сосуществование различных методов проверки подлинности. Знакомство с этими компонентами, в дополнение к ASP.NET MVC, очень важно для понимания примера.

Ниже приведены важные файлы классов, используемые в этом веб-проекте:

| Файл | Описание |
| --------------------------------- | ---------------------------------------- |
| /App\_Start/Startup.Auth.AAD.cs | Интеграция с проверкой подлинности Azure Active Directory |
| /Controllers/AdminController.cs | Содержит административные действия: <br>согласие администратора, управление связанными учетными записями и установка приложения. |
| /Controllers/LinkController.cs | Содержит действия для связывания AD и учетных записей локальных пользователей |
| /Controllers/SchoolsController.cs | Содержит действия для представления образовательных данных |

Это веб-приложение является **мультитенантным**. В AAD включен параметр:

![](Images/app-is-multi-tenant.png)

Доступ к этому приложению могут получить пользователи из любого клиента Azure Active Directory. Так как это приложение использует разрешения приложений, сначала следует зарегистрировать администратора клиента (получить его согласие). В противном случае у пользователей возникнет ошибка:

![](Images/app-requires-admin-to-consent.png)

Дополнительные сведения см. в статье [Создание мультитенантного веб-приложения SaaS с помощью Azure AD и OpenID Connect](https://azure.microsoft.com/en-us/resources/samples/active-directory-dotnet-webapp-multitenant-openidconnect/).

**EDUGraphAPI.SyncData**

Это веб-задание, используемое для синхронизации данных пользователя. В методе **Functions.SyncUsersAsync** применяется **UserSyncService** из проекта EDUGraphAPI.Common.

Проект был создан для демонстрации разностного запроса. Дополнительные сведения см. в разделе [Разностный запрос](#differential-query).

**EDUGraphAPI.Common**

В проекте библиотеки классов используются **EDUGraphAPI.Web** и **EDUGraphAPI.SyncData**. 

В таблице ниже показаны папки проекта:

| Папка | Описание |
| ------------------ | ---------------------------------------- |
| /Data | Содержит ApplicationDbContext и классы объекта |
| /DataSync | Содержит класс UserSyncSerextensionsvice, применяемый в веб-задании EDUGraphAPI.SyncData |
| /DifferentialQuery | Содержит класс DifferentialQueryService, используемый для отправки разностного запроса и анализа результата. |
| /Extensions | Содержит большое число методов расширений, упрощающих создание понятного кода |
| /Utils | Содержит распространенный класс AuthenticationHelper.cs |

**Microsoft.Education**

Этот проект объединяет клиент **[API REST учебных заведений](https://msdn.microsoft.com/en-us/office/office365/api/school-rest-operations)**. Основным классом этого проекта является **EducationServiceClient**.

### Доступ к данным и модели данных

ASP.NET Identity использует [Entity Framework Code First](https://msdn.microsoft.com/en-us/library/jj193542(v=vs.113).aspx) для реализации всех механизмов сохранения. Для этого применяется пакет [Microsoft.AspNet.Identity.EntityFramework](https://www.nuget.org/packages/Microsoft.AspNet.Identity.EntityFramework/). 

В этом примере создается класс **ApplicationDbContext** для доступа к базе данных. Он наследуется из класса **IdentityDbContext**, определенного в пакете NuGet, упомянутом выше.

Ниже приведены важные модели данных (и их важные свойства), использованные в этом примере:

**ApplicationUsers**

Унаследовано от **IdentityUser**. 

| Свойство | Описание |
| ------------- | ---------------------------------------- |
| Organization | Клиент пользователя. Для локального несвязанного пользователя его значение равно null |
| O365UserId | Используется для связи с учетной записью Office 365 |
| O365Email | Электронная почта связанной учетной записи Office 365 |
| JobTitle | Используется для демонстрации разностного запроса |
| Department | Используется для демонстрации разностного запроса |
| Mobile | Используется для демонстрации разностного запроса |
| FavoriteColor | Используется для демонстрации локальных данных |

**Организации**

Строка в этой таблице представляет клиента в AAD.

| Свойство | Описание |
| ---------------- | ------------------------------------ |
| TenantId | GUID клиента |
| Name | Имя клиента |
| IsAdminConsented | Получено ли для клиента согласие одного из администраторов |

### Поток получения согласия администратора

Для разрешений "только для приложения" всегда требуется согласие администратора клиента. Если приложение запрашивает разрешение "только для приложения" и пользователь пытается войти в приложение, появится сообщение об ошибке. Это сообщение говорит о том, что пользователь не может принять это разрешение.

Для некоторых делегированных разрешений также требуется согласие администратора клиента. Например, оно требуется для возможности обратной записи в Azure AD в качестве выполнившего вход пользователя. Как и в случае с разрешениями "только для приложения", если обычный пользователь пытается войти в приложение, запрашивающее делегированное разрешение, для которого требуется согласие администратора, в приложении появится сообщение об ошибке. Требует ли разрешение согласия администратора определяется разработчиком, опубликовавшим ресурс. Эти сведения можно найти в документации по данному ресурсу.

Если приложение использует разрешения, требующие согласия администратора, в приложении должен быть элемент, такой как кнопка или ссылка, с помощью которого администратор может инициировать действие. Запрос, отправляемый приложением для этого действия, является обычным запросом авторизации OAuth2 или OpenID Connect, но он также включает в себя параметр строки запроса `prompt=admin_consent`. После того как администратор предоставит свое согласие, а в клиенте пользователя будет создан субъект-служба, в последующих запросах входа не нужно будет указывать параметр `prompt=admin_consent`. После того как администратор решил, что запрошенные разрешения являются приемлемыми, у других пользователей клиента согласие запрашиваться не будет.



### Потоки проверки подлинности

В этом проекте существует 4 потока проверки подлинности.

Первые 2 потока (локальный вход/вход в O365) позволяют пользователям входить в систему с помощью локальной учетной записи или учетной записи Office 365, а затем привязывать учетную запись другого типа. Эта процедура реализована в LinkController.

**Поток проверки подлинности локального входа**

![](Images/auth-flow-local-login.png)

**Поток проверки подлинности входа в O365**

![](Images/auth-flow-o365-login.png)

**Поток проверки подлинности входа администратора**

В этом потоке показано, как администратор входит в систему и выполняет административные операции.

После входа в приложение с помощью учетной записи Office 365 администратору будет предложено подключиться к локальной учетной записи. Это необязательный шаг, который можно пропустить. 

Как указано выше, веб-приложение является мультитенантным и использует некоторые разрешения приложений, поэтому администратор клиента должен сначала согласиться на использование клиента.  

Этот поток реализован в AdminController. 

![](Images/auth-flow-admin-login.png)

**Поток проверки подлинности приложения**

Это поток реализован в веб-задании SyncData.

![](Images/auth-flow-app-login.png)

Используется сертификат X509. Дополнительные сведения доступны по следующим ссылкам:

* [Из управляющей программы или серверного приложения в веб-интерфейс API](https://docs.microsoft.com/en-us/azure/active-directory/active-directory-authentication-scenarios#daemon-or-server-application-to-web-api)
* [Аутентификация в Azure AD из управляющих программ с помощью сертификатов](https://azure.microsoft.com/en-us/resources/samples/active-directory-dotnet-daemon-certificate-credential/)
* [Создание приложений службы и управляющих программ в Office 365](https://msdn.microsoft.com/en-us/office/office365/howto/building-service-apps-in-office-365)

### Два вида API Graph

В этом примере используются два отдельных API Graph:

|
| [API Graph Azure AD](https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-graph-api) | [API Microsoft Graph](https://graph.microsoft.io/) |
| ------------ | ---------------------------------------- | ---------------------------------------- |
| Описание | API Graph Azure Active Directory обеспечивает программный доступ к Azure Active Directory через конечные точки API REST. Приложения могут использовать API Graph Azure AD для выполнения операций создания, чтения, обновления и удаления (CRUD) с данными и объектами каталогов, таких как пользователи, группы и контакты организации | Единый API, также включающий API из других служб Майкрософт, таких как Outlook, OneDrive, OneNote, Планировщик и Office Graph, доступен через одну конечную точку с помощью одного маркера доступа. |
| Клиент | Пакет установки [Microsoft.Azure.ActiveDirectory.GraphClient](https://www.nuget.org/packages/Microsoft.Azure.ActiveDirectory.GraphClient/) | Пакет установки [Microsoft.Graph](https://www.nuget.org/packages/Microsoft.Graph/) |
| Конечная точка | https://graph.windows.net | https://graph.microsoft.com |
| Обозреватель API | https://graphexplorer.cloudapp.net/| https://graph.microsoft.io/graph-explorer |

В этом примере используются указанные ниже классы, основанные на общем интерфейсе, чтобы продемонстрировать связи API:  

![](Images/class-diagram-graphs.png)

Интерфейс **IGraphClient** определяет два метода: **GeCurrentUserAsync** и **GetTenantAsync**.

**MSGraphClient** реализует интерфейс **IGraphClient** с помощью клиентских библиотек Microsoft Graph.

Интерфейс и класс клиента Graph расположены в папке **/Services/GraphClients** веб-приложения. Ниже выделены фрагменты кода для демонстрации получения пользователя и клиента.

**Microsoft Graph** — MSGraphClient.cs

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

Обратите внимание, что в параметрах регистрации приложений разрешения для каждого API Graph настраиваются отдельно:

![](/Images/aad-create-app-06.png) 

### API Office 365 для образования

[API Office 365 для образования](https://msdn.microsoft.com/office/office365/api/school-rest-operations) помогают извлекать данные из клиента Office 365, синхронизированного с облаком с помощью службы синхронизации сведений о школе (Майкрософт). Эти результаты содержат сведения об учебных заведениях, разделах, преподавателях, учащихся и списках. API REST учебных заведений предоставляет доступ к объектам учебных заведений в клиентах Office 365 для образования.

В этом примере проект библиотеки классов **Microsoft.Education** создан для воплощения API Office 365 для образования. 

**EducationServiceClient** — это основной класс библиотеки. С его помощью можно легко получать данные образовательных учреждений.

**Получение учебных заведений**

~~~c#
public async Task<School[]> GetSchoolsAsync()
{
    var schools = await HttpGetArrayAsync<EducationSchool>("education/schools");
    return schools.ToArray();

}
~~~



**Получение разделов**

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


Ниже представлены некоторые снимки экрана с примером приложения, демонстрирующие образовательные данные.

![](Images/edu-schools.png)



![](Images/edu-classes.png)

![](Images/edu-class.png)

В **EducationServiceClient** были созданы три частных метода с префиксами HttpGet, чтобы упростить вызов API REST.

* **HttpGetAsync**. Отправляет HTTP-запрос GET в целевую конечную точку и возвращает строку ответа JSON. В заголовок проверки подлинности носителя добавлен маркер доступа.
* **HttpGetObjectAsync<T>**. Выполняет десериализацию строки JSON, возвращаемой методом HttpGetAsync для целевого типа T, и возвращает итоговый объект.
* **HttpGetArrayAsync<T>**. Выполняет десериализацию строки JSON, возвращаемой методом HttpGetAsync для целевого массива типа T[], и возвращает массив.

### Разностный запрос

[Разностный запрос](https://msdn.microsoft.com/en-us/Library/Azure/Ad/Graph/howto/azure-ad-graph-api-differential-query) требует возвращения всех изменений, внесенных в указанные объекты между моментами двух последовательных запросов. Например, если разностный запрос выполняется через час после предыдущего разностного запроса, возвращаются изменения, внесенные только в течение этого часа. Эта функция особенно полезна при синхронизации данных каталога клиента с хранилищем данных приложения.

Соответствующий код находится в следующих двух папках проекта **EDUGraphAPI.Common**:

* **/DifferentialQuery**. Содержит классы для отправки разностного запроса и анализа разностного результата.
* **/DataSync**. Содержит классы, используемые для демонстрации способов синхронизации пользователей.

> Обратите внимание, что классы в папке **DifferentialQuery** используют некоторые дополнительные технологии .NET. Сосредоточьтесь на использовании этих классов, а не на подробностях их реализации.

Чтобы синхронизировать пользователей, мы определили класс User:

~~~c#
public class User
{
    public string ObjectId { get; set; }
    public virtual string JobTitle { get; set; }
    public virtual string Department { get; set; }
    public virtual string Mobile { get; set; }
}
~~~

Обратите внимание, что изменяемые свойства *JobTitle*, *Department*, *Mobile* являются виртуальными. Классы в папке **DifferentialQuery** создают прокси-тип для типа User и переопределяют эти виртуальные свойства для отслеживания изменений.

В классе **UserSyncService** демонстрируется способ применения **DifferentialQueryService** для отправки разностного запроса и получения разностного результата.

```c#
var differentialQueryService = new DifferentialQueryService(/**/);
DeltaResult<Delta<User>> result = await differentialQueryService.QueryAsync<User>(url);
```
Также показано, как обновлять (или удалять) пользователей в локальной базе данных с разностным результатом:

~~~c#
foreach (var differentialUser in result.Items)
    await UpdateUserAsync(differentialUser);
//...
private async Task UpdateUserAsync(Delta<User> differentialUser) { /**/ }
~~~

Модель данных **DataSyncRecord** используется для постоянных объектов deltaLink.

Ниже показан журнал, созданный с помощью веб-задания SyncData:

![](Images/sync-data-web-job-log.png) 

### Фильтры

В папке **/Infrastructure** веб-проекта находится несколько объектов FilterAttribute.

**EduAuthorizeAttribute**

Это фильтр авторизации, унаследованный от AuthorizeAttribute.

Он создан, чтобы разрешать веб-приложению перенаправлять пользователей на соответствующую страницу входа в рамках сценария с многофакторной проверкой подлинности.

Мы переопределили метод **HandleUnauthorizedRequest**, чтобы перенаправлять пользователя в /Account/Login:

~~~c#
protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
{
    filterContext.Result = new RedirectResult("/Account/Login");
}
~~~

**HandleAdalExceptionAttribute**

Класс **AuthenticationHelper** предоставляет множество методов, возвращающих маркеры доступа или экземпляр клиента API. Большинство этих методов вызывают **[AuthenticationContext.AcquireTokenSilentAsync](https://msdn.microsoft.com/en-us/library/mt473642.aspx)** внутренним способом. Обычно **AcquireTokenSilentAsync** успешно получает маркер доступа, так как маркеры кэшируются в базе данных с помощью **ADALTokenCache**. 

В некоторых случаях, например при истечении срока действия кэшированного маркера или при запросе маркера нового ресурса, метод **AcquireTokenSilentAsync** выдает сообщение о необходимости **AdalException**.**HandleAdalExceptionAttribute** для обработки **AdalException** и направляет пользователя к конечной точке проверки подлинности, чтобы получить новый маркер.

В некоторых случаях пользователь перенаправляется непосредственно к конечной точке проверки подлинности путем вызова:

~~~c#
filterContext.HttpContext.GetOwinContext().Authentication.Challenge(
   new AuthenticationProperties { RedirectUri = requestUrl },
   OpenIdConnectAuthenticationDefaults.AuthenticationType);
~~~

В других случаях мы хотим показать пользователю указанную ниже страницу, чтобы сообщить ему о причине перенаправления, в частности для пользователя, вошедшего с помощью локальной учетной записи.

![](Images/web-app-login-o365-required.png)

Для этого используется переключатель. Значение переключателя извлекается следующим образом:

~~~c#
//public static readonly string ChallengeImmediatelyTempDataKey = "ChallengeImmediately";
var challengeImmediately = filterContext.Controller.TempData[ChallengeImmediatelyTempDataKey];
~~~

Если присвоено значение true, пользователь будет сразу перенаправлен к конечной точке проверки подлинности. В противном случае сначала отобразится представленная выше страница, а пользователь нажмет кнопку входа для продолжения.

**LinkedOrO365UsersOnlyAttribute**

Это другой фильтр авторизации. С его помощью можно разрешить только связанным пользователям или пользователям Office 365 использовать защищенные контроллеры и действия.

~~~c#
protected override bool AuthorizeCore(HttpContextBase httpContext)
{
    var applicationService = DependencyResolver.Current.GetService<ApplicationService>();
    var user = applicationService.GetUserContext();
    return user.AreAccountsLinked || user.IsO365Account;
}
~~~

Для неавторизованных пользователей отображается страница об отсутствии доступа:

~~~c#
protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
{
    filterContext.Result = new ViewResult { ViewName = "NoAccess" };
}
~~~

Пока она используется только в **SchoolsController**.

### Основные классы

**Microsoft.Education**

* `EducationServiceClient`. Экземпляр этого класса обрабатывает запросы на создание, отправляя их в API Office 365 для образования и обрабатывая ответы.

  | Метод | Описание |
| ------------------- | ---------------------------------------- |
| GetSchoolsAsync | Получение всех учебных заведений, существующих в клиенте Azure Active Directory |
| GetSchoolAsync | Получение учебного заведения с помощью идентификатора объекта |
| GetAllSectionsAsync | Получение разделов в учебном заведении l |
| GetMySectionsAsync | Получение своих разделов в учебном заведении |
| GetSectionAsync | Получение раздела с помощью идентификатора объекта |
| GetMembersAsync | Получение участников в учебном заведении |
| GetStudentAsync | Получение текущего пользователя, вошедшего как учащийся |
| GetTeacherAsync | Получение текущего пользователя, вошедшего как преподаватель |

**EDUGraphAPI.Common**

* **`Data.ApplicationUser`**. Экземпляр класса представляет пользователя.

* **`Data.Organization`**. Экземпляр класса представляет клиента в Azure AD. 

* **`Data.ApplicationDbContext`**. Класс DbContext, используемый Entity Framework, который наследуется от `IdentityDbContext<ApplicationUser>`.

* **`DataSync.User`**. Экземпляр класса представляет пользователя в Azure AD. Обратите внимание, что свойства, используемые для отслеживания изменений, являются виртуальными.

* **`DataSync.UserSyncService`**. Экземпляр класса обрабатывает синхронизацию пользователей в локальной базе данных с помощью разностного запроса. Чтобы начать синхронизацию пользователей, вызовите метод `SyncAsync`.

* **`DifferentialQuery.DifferentialQueryService`**. Экземпляр класса обрабатывает запрос на создание, его отправку в конечную точку службы и обработку ответов. Чтобы создать разностный запрос, вызовите метод `QueryAsync` с использованием deltaLink. Разностный результат будет преобразован в `DeltaResult<Delta<TEntity>>` с помощью класса `DeltaResultParser`.

* **`Utils.AuthenticationHelper`**. Статический класс поддержки, используемый для получения маркера доступа, результата проверки подлинности, контекста проверки подлинности и экземпляров клиента службы.

  | Метод | Описание |
| -------------------------------------- | ---------------------------------------- |
| GetActiveDirectoryClientAsync | Получение экземпляра ActiveDirectoryClient |
| GetGraphServiceClientAsync | Получение экземпляра GraphServiceClient |
| GetEducationServiceClientAsync | Получение экземпляра EducationServiceClient |
| GetActiveDirectoryClient | Получение экземпляра ActiveDirectoryClient из указанного объекта AuthenticationResult |
| GetGraphServiceClient | Получение экземпляра GraphServiceClient из указанного объекта AuthenticationResult |
| GetAccessTokenAsync | Получение маркера доступа указанного ресурса |
| GetAuthenticationResult | Получение AuthenticationResult указанного ресурса |
| GetAuthenticationContext | Получение экземпляра AuthenticationContext |
| GetAuthenticationResultAsync | Получение AuthenticationResult из указанного кода авторизации |
| GetAppOnlyAccessTokenForDaemonAppAsync | Получение маркера доступа "только для приложений" для управляющей программы |

  У большинства методов выше есть аргумент с названием permission. Он относится к типу `Permissions` (тип перечисления) с двумя определенными значениями:

  * `Delegated`: клиент обращается к веб-API как вошедший пользователь.
  * `Application`: клиент напрямую обращается к веб-API от своего имени (без контекста пользователя). Этот тип разрешения требует согласия администратора.

* **`Utils.AuthenticationHelper`**. Статический класс, используемый для создания URL-адреса авторизации. `GetUrl` — единственный метод, определенный в классе.

* **`Constants`**. Статический класс, содержащий постоянные значения параметров приложения и другие постоянные значения.

**EDUGraphAPI.Web**

* **`Controllers.AccountController`**. Содержит действия для регистрации, входа и смены пароля пользователя.

* **`Controllers.AdminController`**. Реализует **поток проверки подлинности входа администратора**. Дополнительные сведения см. в разделе [Потоки проверки подлинности](#authentication-flows).

* **`Controllers.LinkController`**. Реализует **поток проверки подлинности локального входа или входа в O365**. Дополнительные сведения см. в разделе [Потоки проверки подлинности](#authentication-flows).

* **`Controllers.SchoolsController`**. Содержит действия для отображения учебных заведений и классов. Этот контроллер в основном использует класс `SchoolsService`. Дополнительные сведения см. в разделе [API Office 365 для образования](#office-365-education-api).

* **`Infrastructure.EduAuthorizeAttribute`**. Разрешает веб-приложению перенаправлять текущего пользователя на соответствующую страницу входа в рамках сценария с многофакторной проверкой подлинности. Дополнительные сведения см. в разделе [Фильтры](#filters).

* **`Infrastructure.HandleAdalExceptionAttribute`**. Обрабатывает AdalException и направляет пользователя в конечную точку авторизации или в /Link/LoginO365Required. Дополнительные сведения см. в разделе [Фильтры](#filters).

* **`Infrastructure.LinkedOrO365UsersOnlyAttribute`**. Разрешает только связанным пользователям или пользователям Office 365 использовать защищенные контроллеры и действия. Дополнительные сведения см. в разделе [Фильтры](#filters).

* **`Models.UserContext`**. Контекст вошедшего пользователя.

* **`Services.GraphClients.AADGraphClient`**. Реализует интерфейс `IGraphClient` с помощью API Graph Azure AD. Дополнительные сведения см. в разделе [Два вида API Graph](#two-kinds-of-graph-api).

* **`Services.GraphClients.MSGraphClient`**. Реализует интерфейс `IGraphClient` с помощью API Microsoft Graph. Дополнительные сведения см. в разделе [Два вида API Graph](#two-kinds-of-graph-api).

* **`Services.ApplicationService.`**. Экземпляр класса обрабатывает получение и обновление пользователя или организации.

  | Метод | Описание |
| ------------------------------- | ---------------------------------------- |
| CreateOrUpdateOrganizationAsync | Создание или обновление организации |
| GetAdminContextAsync | Получение контекста текущего администратора |
| GetCurrentUser | Получение текущего пользователя |
| GetCurrentUserAsync | Получение текущего пользователя |
| GetUserAsync | Получение пользователя по идентификатору |
| GetUserContext | Получение контекста текущего пользователя |
| GetUserContextAsync | Получение контекста текущего пользователя |
| GetLinkedUsers | Получение связанных пользователей с помощью определенного фильтра |
| IsO365AccountLinkedAsync | Связана ли указанная учетная запись O365 с локальной учетной записью |
| SaveSeatingArrangements | Сохранение схемы рассадки |
| UnlinkAccountsAsync | Удаление связи с указанной учетной записью |
| UnlinkAllAccounts | Удаление связей всех учетных записей в указанном клиенте |
| UpdateLocalUserAsync | Обновление локального пользователя с использованием сведений пользователя O365 и клиента |
| UpdateOrganizationAsync | Обновление организации |
| UpdateUserFavoriteColor | Обновление избранного цвета текущего пользователя |

* **`Services.SchoolsService`**. Класс службы, используемый для получения образовательных данных.

  | Метод | Описание |
| ------------------------------- | ---------------------------------------- |
| GetSchoolsViewModelAsync | Получение SchoolsViewModel |
| GetSchoolUsersAsync | Получение преподавателей и учащихся указанного учебного заведения |
| GetSectionsViewModelAsync | Получение SectionsViewModel указанного учебного заведения |
| GetSectionDetailsViewModelAsync | Получение SectionDetailsViewModel указанного раздела |
| GetMyClasses | Получение своих классов |

**EDUGraphAPI.SyncData**

* **`Functions`**. Содержит метод `SyncUsersAsync`, регулярно выполняемый для синхронизации данных пользователей.
* **`Program`**. Содержит метод `Main`, настраивающий и запускающий узел веб-задания.

## [Необязательно] Сборка и отладка веб-задания в локальной среде

Отладка **EDUGraphAPI.SyncData**:

1. Создайте учетную запись хранения в Azure и получите строку подключения.
   > Примечание.
   - Локальная отладка с использованием эмулятора хранения Azure будет поддерживаться после выпуска соответствующего пакета [SDK веб-заданий Azure](https://github.com/Azure/azure-webjobs-sdk) версии 2. Дополнительные сведения см. в статье [Поддержка эмулятора хранения Azure](https://github.com/Azure/azure-webjobs-sdk/issues/53). - **Не** рекомендуется выполнять локальную отладку при работе опубликованного веб-задания в Azure с использованием той же учетной записи хранения. Дополнительные сведения см. в [этом вопросе](http://stackoverflow.com/questions/42020647/what-happened-when-using-same-storage-account-for-multiple-azure-webjobs-dev-li).

2. Настройте **App.config**:

   ![](Images/webjob-app-config.png)

   - **Строки подключения**:
     - **AzureWebJobsDashboard**: используйте строку подключения, полученную на предыдущем шаге.
     - **AzureWebJobsStorage**: используйте строку подключения, полученную на предыдущем шаге.
   - **Параметры приложения**:
     - *ida:ClientId*\*: используйте идентификатор клиента из регистрации приложения, созданный ранее.

3. Настройте **EDUGraphAPI.SyncData** в качестве начального проекта и нажмите клавишу F5. 

## Вопросы и комментарии

* Если у вас возникли проблемы с запуском этого примера, [сообщите о неполадке](https://github.com/OfficeDev/O365-EDU-AspNetMVC-Samples/issues).
* Общие вопросы о разработке GraphAPI следует задавать на сайте [Stack Overflow](http://stackoverflow.com/questions/tagged/office-addins). Добавляйте тег [ms-graph-api] к своим вопросам или комментариям. 

## Участие

Мы приветствуем ваше участие в создании примеров. Сведения о дальнейших действиях см. в [руководстве по участию](/Contributing.md).

Этот проект соответствует [правилам поведения разработчиков открытого кода Майкрософт](https://opensource.microsoft.com/codeofconduct/). Дополнительные сведения см. в разделе [часто задаваемых вопросов о правилах поведения](https://opensource.microsoft.com/codeofconduct/faq/). Если у вас возникли вопросы или замечания, напишите нам по адресу [opencode@microsoft.com](mailto:opencode@microsoft.com).



**(c) Корпорация Майкрософт (Microsoft Corporation), 2017. Все права защищены.**
