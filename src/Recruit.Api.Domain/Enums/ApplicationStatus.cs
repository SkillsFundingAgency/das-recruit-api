﻿using System.Text.Json.Serialization;

namespace SFA.DAS.Recruit.Api.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ApplicationStatus
    {
        Draft = 0,
        Submitted = 1,
        Withdrawn = 2,
        Successful = 3,
        UnSuccessful = 4,
        Expired = 5
    }
}