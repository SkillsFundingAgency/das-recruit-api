using System.Net;

namespace SFA.DAS.Recruit.Api.IntegrationTests;

internal class HttpResponseAssertions(HttpResponseMessage message)
{
    private void AssertHttpStatusCode(HttpStatusCode statusCode) => Assert.AreEqual(statusCode, message.StatusCode);
    public void NotFound() => AssertHttpStatusCode(HttpStatusCode.NotFound);
    public void Ok() => AssertHttpStatusCode(HttpStatusCode.OK);
    public void NoContent() => AssertHttpStatusCode(HttpStatusCode.NoContent);
    public void BadRequest() => AssertHttpStatusCode(HttpStatusCode.BadRequest);
    public void InternalServerError() => AssertHttpStatusCode(HttpStatusCode.InternalServerError);
}