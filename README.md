[![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status/das-recruit-api)](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_build/latest?definitionId=1631)


# SFA.DAS.Recruit.Api

This repository represents the codebase for an API designed for internal use within the Digital Apprenticeship Service. The API will be used by other Digital Apprenticeship Service systems to retrieve data related to apprenticeship vacancies sourced from the system known as [Recruit an Apprentice](https://github.com/SkillsFundingAgency/das-recruit). Recruit an Apprentice is used by employers and approved training providers to post apprenticeship opportunities across England. The code in this repository is to be maintained by the ESFA Vacancy Services team.

&nbsp;

## Contents

* [Endpoints](#endpoints)
    * [Sample API Responses](#sampleApiResponses)
        * [Vacancy Summaries](#vacancySummariesSample)
        * [Applicant Summaries](#applicantSummariesSample)
        * [Employer Account Summary](#employerAccountSummarySample)
    * [Authorization](#apiAuth)
    * [Known consumers of this API](#knownApiConsumers)
* [Development](#development)
    * [Requirements](#devReqs)
    * [Add configuration to Azure Storage Emulator](#localConfig)
    * [Logging](#logging)
    * [Running](#runningLocally)
* [License](#license)

&nbsp;

<a id="endpoints"></a>
## Endpoints

Initially there are 3 endpoints that this API provides and they are all for retrieving data.

If you work on the Digital Apprenticeship Service, you can view the following [Environments](https://skillsfundingagency.atlassian.net/wiki/spaces/RAAV2/pages/200245289/Environments) page for the Recruit system to find the environment url's for the Recruit API:

https://skillsfundingagency.atlassian.net/wiki/spaces/RAAV2/pages/200245289/Environments

The URIs in the table below are relative to *https://localhost* for example purposes.

Endpoint | HTTP request | Description
------------ | ------------- | -------------
*Vacancy Summaries* | **GET**<br>/api/vacancies/?employerAccountId=?&legalEntityId=?&ukprn=?pageSize=25&pageNo=1 | EmployerAccountId is required, other parameters are optional. PageSize defaults to 25 and PageNo to 1.<br><br>Gets all summaries (paged where applicable) of Employer created apprenticeship vacancies for the Employer Account Id provided, optionally filtered by Legal Entity Id.<br><br>If specifying Ukprn then returns Provider created vacancies associated with the Provider and for the Employer Account Id specified, optionally filtered by Legal Entity Id.<br><br>The vacancies returned from this endpoint are ordered from oldest first.
*Applicant Summaries* | **GET**<br>/api/vacancies/{vacancyReference}/applicants?outcome=? | VacancyReference is required. Outcome is optional and possible valid values for that parameter are 'Successful' or 'Unsuccessful'.<br><br>Gets all applicant summaries for the vacancy specified, optionally filtered by outcome. If no outcome is specified and there exists an applicant where the outcome is still to be decided, 'N/A' is returned as part of the `status` field.
*Employer Summary* | **GET**<br>/api/employers/{employerAccountId} | EmployerAccountId is required.<br><br>Gets a breakdown of apprenticeship vacancies created by the Employer Account Id specified, broken down with a summary status count per legal entity.

<a id="sampleApiResponses"></a>
### Sample API Responses (not real data)

<a id="vacancySummariesSample"></a>
#### Vacancy Summaries
```json
{
    "vacancies": [
        {
            "employerAccountId": "ABC1D3",
            "title": "Seafarer apprenticeship",
            "vacancyReference": 1000004431,
            "legalEntityId": 1234,
            "legalEntityName": "Rosie's Boats",
            "employerName": "Rosie's Boats Company",
            "ukprn": 12345678,
            "createdDate": "2019-06-12T10:35:10.457Z",
            "status": "Live",
            "closingDate": "2020-10-10T00:00:00Z",
            "duration": 2,
            "durationUnit": "Year",
            "applicationMethod": "ThroughFindAnApprenticeship",
            "programmeId": "34",
            "startDate": "2020-11-10T00:00:00Z",
            "trainingTitle": "Able seafarer (deck)",
            "trainingType": "Standard",
            "trainingLevel": "Intermediate",
            "noOfNewApplications": 0,
            "noOfSuccessfulApplications": 1,
            "noOfUnsuccessfulApplications": 0,
            "faaVacancyDetailUrl": "https://findapprenticeship.service.gov.uk/apprenticeship/1000004431",
            "raaManageVacancyUrl": "https://recruit.apprenticeships.education.gov.uk/12345678/vacancies/eb0d5d5b-6cb9-469e-9423-bdc9db1ef5b9/manage/"
        }
    ],
    "pageSize": 25,
    "pageNo": 1,
    "totalResults": 1,
    "totalPages": 1
}
```

<a id="applicantSummariesSample"></a>
#### Applicant Summaries
```json
[
    {
        "applicantId": "a148b1ee-8949-4ae4-9b4d-c687753a5058",
        "firstName": "Billy",
        "lastName": "Dante",
        "dateOfBirth": "1998-01-03T00:00:00Z",
        "applicationStatus": "Unsuccessful"
    },
    {
        "applicantId": "569dfc5a-7292-45bf-b792-5cc7f26683cc",
        "firstName": "John",
        "lastName": "Tree",
        "dateOfBirth": "1988-01-01T00:00:00Z",
        "applicationStatus": "Successful"
    },
    {
        "applicantId": "da90181c-333e-4159-8ee5-65c9b99c161d",
        "firstName": "Jill",
        "lastName": "Hargreave",
        "dateOfBirth": "1986-11-11T00:00:00Z",
        "applicationStatus": "N/A"
    }
]
```

<a id="employerAccountSummarySample"></a>
#### Employer Account Summary
```json
{
    "employerAccountId": "ABC1D3",
    "totalNoOfVacancies": 5,
    "totalVacancyStatusCounts": {
        "closed": 1,
        "referred": 1,
        "live": 1,
        "draft": 2
    },
    "legalEntityVacancySummaries": [
        {
            "legalEntityId": 1234,
            "legalEntityName": "Rosie's Boats",
            "vacancyStatusCounts": {
                "closed": 1,
                "referred": 1,
                "live": 1
            }
        },
        {
            "legalEntityId": 9876,
            "legalEntityName": "ASCOT LTD",
            "vacancyStatusCounts": {
                "draft": 1
            }
        }
    ]
}
```

<a id="apiAuth"></a>
### Authorization

Requests to the API need to be authenticated and authorization is proved by submitting an OAuth Bearer Token in the HTTP header of the request. If you work on the Digital Apprenticeship Service you can request access keys from the Operations team for the environment you need.

<a id="knownApiConsumers"></a>
### Known consumers of this API:
 - [Manage Apprenticeships](https://github.com/SkillsFundingAgency/das-employerapprenticeshipsservice)
 - [Provider Apprenticeships](https://github.com/SkillsFundingAgency/das-providercommitments)

&nbsp;

<a id="development"></a>
## Development

Note that for local running of the API, the authorization is disabled so no authorization token header needs to be submitted.

<a id="devReqs"></a>
### Requirements

In order to run this project locally you will need the following:

* [.NET Core SDK >= 2.2](https://www.microsoft.com/net/download/)
* (VS Code Only) [C# Extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp)
* Access to an Azure Cosmos or MongoDb server (emulated or hosted) hosting the DAS Recruit database.
* Azure Storage Emulator (or [Azurite](https://github.com/Azure/Azurite) as an alternative)
* Azure Storage Explorer (Optional)

<a id="localConfig"></a>
### Add configuration to Azure Storage Emulator

1. Clone the [das-employer-config](https://github.com/SkillsFundingAgency/das-employer-config) repository.
2. For the above repository create your own branch based off of master. You should be rebasing this new branch on the origin master regularly as other teams within DAS will be updating the repository and configuration files contained within. **DO NOT** push this branch onto Github, it is for local use to your machine only.
3. Depending on if you are using the Cosmos emulator or Mongo to host the Recruit database you may need to change the connection string for the database in the file `/das-recruit-api/SFA.DAS.Recruit.Api.json`, once again on the branch you created in the previous step.
4. Clone the [das-employer-config-updater](https://github.com/SkillsFundingAgency/das-employer-config-updater) repository.
5. Run your local Azure Storage Emulator or [Azurite](https://github.com/Azure/Azurite) (in a container or natively) if you are using macOS.
6. Navigate to the `/das-employer-config-updater/das-employer-config-updater` directory in a new terminal session and run the command `dotnet run`.
7. Follow the instructions to import the config from the directory that you cloned the `das-employer-config repository` to and set your environment to `Development`.

> The two repositories above are private. If the links appear to be dead make sure that you are logged into GitHub with an account that has access to these i.e. that you are part of the [Skills Funding Agency Team](https://github.com/SkillsFundingAgency) organization.

Note that if you have used Azurite v2.7.0 or below, there is an issue in that you will be unable to edit values in a TableStorage row unless you update the row using code. It is for this reason that

<a id="logging"></a>
### Logging

The API logs messages to multiple targets. The primary target being logging to a local ELK stack via Redis configured via the `appSettings.Development.json`, `appSettings.json` and `nlog.config` files. The secondary target is to local file which will appear dated in the `bin/Debug/netcoreapp2.2/logs` directory after running.

<a id="runningLocally"></a>
### Running

There are various ways of running the project. Here are with instructions per platform below:

macOS
```
ASPNETCORE_ENVIRONMENT=Development ConfigurationStorageConnectionString=UseDevelopmentStorage=true dotnet run
```

Windows cmd
```
set ASPNETCORE_ENVIRONMENT=Development
set ConfigurationStorageConnectionString=UseDevelopmentStorage=true
dotnet run
```

If using `launchSettings.json` file from Visual Studio or VS Code the following would do:

```json
{
  "profiles": {
    "Recruit.Api": {
      "commandName": "Project",
      "launchBrowser": false,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "APPSETTING_ASPNETCORE_ENVIRONMENT": "Development",
        "APPSETTING_ConfigurationStorageConnectionString": "UseDevelopmentStorage=true;"
      },
      "applicationUrl": "http://localhost:5040/"
    }
  }
}
```

Then make sure you select the Recruit.Api selection from the debug drop down as show below:

![launchDebugOption](docs/img/vsLaunchDebugOptionScreenshot_982x482.png)

The browser will not launch but the app should be running and you can start making requests to the API using [cURL](https://curl.haxx.se/), [Postman](https://www.getpostman.com/), web browser or any other http request making tools. If you want a browser to launch you will need to modify your `launchSettings.json` file to do so.

<a id="license"></a>
## License

Licensed under the [MIT license](LICENSE)