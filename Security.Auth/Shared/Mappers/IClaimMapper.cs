namespace Labs.Security.Auth.Shared.Mappers
{
    public interface IClaimMapper
    {
        bool Contains(string type);

        string Fetch(string type);
    }
}