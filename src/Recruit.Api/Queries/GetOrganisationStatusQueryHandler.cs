using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Recruit.Api.Helpers;
using SFA.DAS.Recruit.Api.Models;
using SFA.DAS.Recruit.Api.Services;

namespace SFA.DAS.Recruit.Api.Queries
{
    public class GetOrganisationStatusQueryHandler : IRequestHandler<GetOrganisationStatusQuery, GetOrganisationStatusResponse>
    {
        private readonly ILogger<GetOrganisationStatusQueryHandler> _logger;
        private readonly IQueryStoreReader _queryStoreReader;

        public GetOrganisationStatusQueryHandler(ILogger<GetOrganisationStatusQueryHandler> logger, IQueryStoreReader queryStoreReader)
        {
            _logger = logger;
            _queryStoreReader = queryStoreReader;
        }

        public async Task<GetOrganisationStatusResponse> Handle(GetOrganisationStatusQuery request, CancellationToken cancellationToken)
        {
            var validationErrors = ValidateRequest(request);

            if (validationErrors.Any())
            {
                return new GetOrganisationStatusResponse { ResultCode = ResponseCode.InvalidRequest, ValidationErrors = validationErrors };
            }

            var blockedProviders = await _queryStoreReader.GetBlockedProviders();

            if (blockedProviders == null)
            {
                return new GetOrganisationStatusResponse { ResultCode = ResponseCode.NotFound };
            }

            var status = blockedProviders.Data.Contains(request.Ukprn) ? "Blocked" : "Unblocked";
            return new GetOrganisationStatusResponse { ResultCode = ResponseCode.Success, Data = new { Status = status } };
        }

        private List<string> ValidateRequest(GetOrganisationStatusQuery request)
        {
            const string ukprnFieldName = nameof(request.Ukprn);
            const string ukprnRegex = @"^\d{8}$";
            var validationErrors = new List<string>();

            if (Regex.IsMatch(request.Ukprn.ToString(), ukprnRegex) == false)
            {
                validationErrors.Add($"Invalid {FieldNameHelper.ToCamelCasePropertyName(ukprnFieldName)}");
            }

            return validationErrors;
        }
    }
}