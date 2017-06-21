using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Labs.Security.Domain.Features.Users
{
    public interface IUserStore
    {
        Task<bool> ValidateCredentials(string username, string password);

        Task<UserData> FindBySubjectId(string subjectId);

        Task<UserData> FindByUsername(string username);

        Task<UserData> FindByProvider(string provider, string subjectId, string connectId);

        Task<UserData> ProvisionUser(string provider, string userId, string connectId, List<Claim> claims);
    }
}