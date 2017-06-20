namespace Labs.Security.Domain.Features.Users
{
    public interface IClaimMapper
    {
        bool Contains(string type);

        string Fetch(string type);
    }
}