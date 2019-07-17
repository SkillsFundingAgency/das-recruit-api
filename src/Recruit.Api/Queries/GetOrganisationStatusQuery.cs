using MediatR;

namespace SFA.DAS.Recruit.Api.Queries
{
    public class GetOrganisationStatusQuery : IRequest<GetOrganisationStatusResponse>
    {
        public long Ukprn { get; }

        public GetOrganisationStatusQuery(long ukprn)
        {
            Ukprn = ukprn;
        }
    }
}