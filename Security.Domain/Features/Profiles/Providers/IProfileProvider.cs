using System.Threading.Tasks;
using Labs.Security.Domain.Features.Profiles.Models;

namespace Labs.Security.Domain.Features.Profiles.Providers
{
    public interface IProfileProvider
    {
        Task<ProfileModel[]> Search(AliasesCriterion criterion);
    }

    public interface IProfileCriterion
    {
    }

    public class AliasesCriterion : IProfileCriterion
    {
        public string[] Names { get; set; }
    }
}