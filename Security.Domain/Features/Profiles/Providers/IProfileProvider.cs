using System.Threading.Tasks;
using Infsys.Security.Auth.Core.Features.Profiles.Models;

namespace Infsys.Security.Auth.Core.Features.Profiles.Providers
{
    public interface IProfileProvider
    {
        Task<ProfileModel[]> Search(IProfileCriterion criterion);
    }
}