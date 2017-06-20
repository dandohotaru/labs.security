using System.Threading.Tasks;

namespace Labs.Security.Domain.Features.Profiles.Providers
{
    public interface IIdentityProvider
    {
        Task<IdentityData[]> Search(AliasesCriterion criterion);
    }

    public interface IProfileCriterion
    {
    }

    public class AliasesCriterion : IProfileCriterion
    {
        public string[] Aliases { get; set; }
    }
}