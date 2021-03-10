namespace Tickr.Tests.Integration
{
    using System.Net.Http;
    using Xunit;

    public class GetAllItemsTest : IClassFixture<TestFixture>
    {
        private readonly HttpClient _httpClient;
        
        public GetAllItemsTest(TestFixture testFixture)
        {
            _httpClient = testFixture.CreateClient();
        }
    } 

}