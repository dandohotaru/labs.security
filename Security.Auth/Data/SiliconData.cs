using System.Collections.Generic;
using IdentityServer4.Models;

namespace Labs.Security.Auth.Data
{
    public class SiliconData
    {
        public IEnumerable<ApiResource> Load()
        {
            yield return Apione();
            yield return Apitwo();
            yield return Apithree();
            yield return Apifour();
        }

        protected ApiResource Apione()
        {
            return new ApiResource("api1", "Api #1");
        }

        protected ApiResource Apitwo()
        {
            return new ApiResource("api2", "Api #2");
        }

        protected ApiResource Apithree()
        {
            return new ApiResource
            {
                Name = "api3",
                DisplayName = "Api #3",
                Description = "Api #3",
                Enabled = true,
                UserClaims = new List<string>
                {
                    "role",
                }
            };
        }

        protected ApiResource Apifour()
        {
            return new ApiResource
            {
                Name = "api4",
                DisplayName = "Api #4",
                Description = "Api #4",
                Enabled = true,
                UserClaims = new List<string>
                {
                    "role",
                    "grant",
                }
            };
        }
    }
}