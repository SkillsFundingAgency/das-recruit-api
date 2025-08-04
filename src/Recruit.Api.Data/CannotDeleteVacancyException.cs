namespace SFA.DAS.Recruit.Api.Data;

public sealed class CannotDeleteVacancyException() : RecruitDataException(
    message: "Cannot delete vacancy",
    detail: "The vacancy is either already deleted or has a status which prevents it from being deleted");