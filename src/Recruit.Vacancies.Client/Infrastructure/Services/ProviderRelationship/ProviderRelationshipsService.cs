using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Esfa.Recruit.Vacancies.Client.Domain.Models;
using Esfa.Recruit.Vacancies.Client.Infrastructure.QueryStore.Projections.EditVacancyInfo;
using Esfa.Recruit.Vacancies.Client.Infrastructure.Services.EmployerAccount;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Esfa.Recruit.Vacancies.Client.Infrastructure.Services.ProviderRelationship
{
    public class ProviderRelationshipsService : IProviderRelationshipsService
    {
        private readonly ILogger<ProviderRelationshipsService> _logger;
        private readonly IEmployerAccountProvider _employerAccountProvider;
        private readonly HttpClient _httpClient;

        public ProviderRelationshipsService(
            ILogger<ProviderRelationshipsService> logger,
            IEmployerAccountProvider employerAccountProvider,
            HttpClient httpClient)
        {
            _logger = logger;
            _employerAccountProvider = employerAccountProvider;
            _httpClient = httpClient;

        }

        public async Task<bool> HasProviderGotEmployersPermissionAsync(long ukprn, string accountPublicHashedId, string accountLegalEntityPublicHashedId, OperationType operationType)
        {
            var permittedLegalEntities = await GetProviderPermissionsforEmployer(ukprn, accountPublicHashedId, operationType);

            if (permittedLegalEntities.Count == 0) return false;

            var accountId = permittedLegalEntities[0].AccountHashedId;
            var allLegalEntities = (await _employerAccountProvider.GetLegalEntitiesConnectedToAccountAsync(accountId)).ToList();

            bool hasPermission = permittedLegalEntities
                .Join(allLegalEntities,
                    ple => ple.AccountLegalEntityPublicHashedId,
                    ale => ale.AccountLegalEntityPublicHashedId,
                    (ple, ale) => ale)
                .Any(l => l.AccountLegalEntityPublicHashedId == accountLegalEntityPublicHashedId);

            return hasPermission;
        }

        private async Task<List<LegalEntityDto>> GetProviderPermissionsforEmployer(long ukprn, string accountHashedId, OperationType operationType)
        {
            var providerPermissions = await GetProviderPermissionsByUkprn(ukprn, operationType);

            var permittedLegalEntities = providerPermissions.AccountProviderLegalEntities
                .Where(l => l.AccountHashedId == accountHashedId)
                .ToList();

            return permittedLegalEntities;
        }

        private async Task<ProviderPermissions> GetProviderPermissionsByUkprn(long ukprn, OperationType operationType)
        {
            int operation = ConvertOperation(operationType);
            var queryData = new { Ukprn = ukprn, Operations = operation.ToString() };
            return await GetProviderPermissions(queryData);
        }

        private static int ConvertOperation(OperationType operationType)
        {
            // In PR API the operations are expected as int and the enum is incorrect in here
            // hence we have convert to correct ints expected by PR
            return operationType switch
            {
                OperationType.Recruitment => 1,
                OperationType.RecruitmentRequiresReview => 2,
                _ => -1
            };
        }

        private async Task<ProviderPermissions> GetProviderPermissions(object queryData)
        {
            var uri = new Uri(AddQueryString("/accountproviderlegalentities", queryData), UriKind.Relative);

            try
            {
                var response = await _httpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var providerPermissions = JsonConvert.DeserializeObject<ProviderPermissions>(content);
                    return providerPermissions;
                }

                _logger.LogError("An invalid response received when trying to get provider relationships. Status:{StatusCode} Reason:{ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error trying to retrieve legal entities.");
            }
            catch (JsonReaderException ex)
            {
                _logger.LogError(ex, "Couldn't deserialise ProviderPermissions.");
            }

            return new ProviderPermissions { AccountProviderLegalEntities = Enumerable.Empty<LegalEntityDto>() };
        }

        private string AddQueryString(string uri, object queryData)
        {
            var queryDataDictionary = queryData.GetType().GetProperties().ToDictionary(x => x.Name, x => x.GetValue(queryData)?.ToString() ?? string.Empty);
            return QueryHelpers.AddQueryString(uri, queryDataDictionary);
        }
    }
}
