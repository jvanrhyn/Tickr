using System.Threading.Tasks;

namespace Tickr.Client.Helpers
{
    public interface IAuthorizationHelper
    {
        Task<string> GetAccessToken();
    }
}