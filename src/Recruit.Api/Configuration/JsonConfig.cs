﻿using System.Text.Json;

namespace SFA.DAS.Recruit.Api.Configuration;

public static class JsonConfig
{
    public static readonly JsonSerializerOptions Options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
}