# Create Project by using Sample template

Sample project template contains the following job tasks:

```
GENERATE --> PUSH --> BUILD --> DEPLOYDB --> DEPLOY
```

_GENERATE_ task will generate an ASP.NET Core MVC application.

_PUSH_ task will push the generated code to GitHub repository.

_BUILD_ task will build the code and produce an artifact.

_DEPLOYDB_ task will apply generated Entity Framework Core migrations into a SQL Server database.

_DEPLOY_ task will deploy the artifact to Azure App service.

## Pre-requisites

Before generating a project with Sample template, you need to prepare some pre-requisites to make connection to external services.

**GitHub**

In order to connect to GitHub, you can choose to provide `username/password` or `personal access token`. If you choose the latest, you can find how to get it at https://help.github.com/articles/creating-a-personal-access-token-for-the-command-line/.

You need to create a GitHub repository manually for now. In the future, this procedure will be taken care automatically by `OpenCatapult`.

**Azure App Service**

You need to create an Azure App Service manually for now to host the generated application. In the future, this procedure will be taken care automatically by `OpenCatapult`.

After creating the App Service, you need to make note of `Application ID`, `Authentication Key`, and `Tenant ID` values. You can get them by following the steps explained at https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-create-service-principal-portal#get-application-id-and-authentication-key.

**SQL Server Database**

Basically you can host the database anywhere as long as it is reachable by the application. We recommend to use Azure SQL in the same region as the App Service to make them closer.

You need to create the database manually in the server for now. In the future, this procedure will be taken care automatically by `OpenCatapult`. Please make a note for the database connection string.

## Create required external services

After you have all of the required values above, next is to store them securely in one of `OpenCatapult` feature called `External Service`. By creating this, you don't have to enter sensitive information directly in Job Task definition. You just need to include the External Service name in the task, and let the system fetch the secret values in secure way.

Let's create an External Service for `GitHub`:

```sh
dotnet PC.dll service add --name github-default --type GitHub
```

Then you will be prompted to enter the required details for `GitHub`.

And now, let's create External Service for `Azure App Service`:

```sh
dotnet PC.dll service add --name azure-default --type AzureAppService
```

Then you will be prompted to enter the required details for `AzureAppService`.

## Create project

After all required components in place, now you can start to create the project by using Sample template.

```sh
dotnet PC.dll project create --name SampleProject --client Polyrific --template sample
```

You will be prompted to enter some additional configurations.

After your project is created, you can check what you have in it, e.g. models, jobs, etc.

One important thing to do before doing further action is that you need to register an engine instance to allow it picking up any jobs from your projects. To do it, let's register the Engine and generate an access token:

```sh
dotnet PC.dll engine register --name Engine01
dotnet PC.dll engine token --name Engine01
```

Copy the engine access token, then open a new CLI and go to the catapult directory. Run the following command to set authorization token of the engine:

```sh
dotnet .\src\Engine\Polyrific.Catapult.Engine\bin\Release\PCEngine.dll config set -n AuthorizationToken -v <paste the token here>
```

Start the engine to make it ready to execute any queued jobs from your projects:

```sh
dotnet .\src\Engine\Polyrific.Catapult.Engine\bin\Release\PCEngine.dll start
```

At this state, you should be able to direclty send the job from your created project to the queue:

```sh
dotnet PC.dll queue add --project SampleProject --job Default
```