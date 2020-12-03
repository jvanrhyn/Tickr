namespace Tickr.Client.Helpers
{
    using System.Threading.Tasks;

    public interface IAuthorizationHelper
    {
        Task<string> GetAccessToken();
    }
}
