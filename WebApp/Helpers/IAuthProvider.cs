using System.Threading.Tasks;

namespace WebApp.Helpers
{
    public interface IAuthProvider
    {
        Task<string> GetUserAccessTokenAsync();
    }
}
