using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Esfa.Recruit.Vacancies.Client.Infrastructure.OuterApi;
using Esfa.Recruit.Vacancies.Client.Infrastructure.OuterApi.Requests;
using Esfa.Recruit.Vacancies.Client.Infrastructure.OuterApi.Responses;
using Microsoft.Extensions.Logging;
using SFA.DAS.Encoding;

namespace Esfa.Recruit.Vacancies.Client.Infrastructure.Services.EmployerAccount;

public class EmployerAccountProvider : IEmployerAccountProvider
{
    private readonly ILogger<EmployerAccountProvider> _logger;
    private readonly IOuterApiClient _outerApiClient;
    private readonly IEncodingService _encodingService;

    public EmployerAccountProvider(ILogger<EmployerAccountProvider> logger, IOuterApiClient outerApiClient, IEncodingService encodingService)
    {
        _logger = logger;
        _outerApiClient = outerApiClient;
        _encodingService = encodingService;
    }
    
    public async Task<IEnumerable<AccountLegalEntity>> GetLegalEntitiesConnectedToAccountAsync(string hashedAccountId)
    {
        try
        {
            var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);
            var legalEntities =
                await _outerApiClient.Get<GetAccountLegalEntitiesResponse>(
                    new GetAccountLegalEntitiesRequest(accountId));
                
            return legalEntities.AccountLegalEntities;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to retrieve account information for account Id: {hashedAccountId}");
            throw;
        }
    }
}