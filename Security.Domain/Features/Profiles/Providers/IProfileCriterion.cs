namespace Infsys.Security.Auth.Core.Features.Profiles.Providers
{
    public interface IProfileCriterion
    {
    }

    public class AliasesCriterion : IProfileCriterion
    {
        public string[] Names { get; set; }
    }
}