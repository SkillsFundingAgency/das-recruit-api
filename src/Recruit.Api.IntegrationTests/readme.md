# Configuration the integration tests

## User Secrets

You will need to setup some additional configuration in the user-secrets store
to run the integration tests.

From a command prompt, navigate to the `src/SAF.DAS.Recruit.IntegrationTests` folder
and run the following commands, inserting the appropriate values for the settings first:

```
dotnet user-secrets init
dotnet user-secrets set ConnectionStrings:Integration "<Your connection string>"
dotnet user-secrets set IntegrationTestsDbSchemaName "<Your preferred db schema name>"
```