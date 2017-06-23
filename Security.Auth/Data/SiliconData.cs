using System.Collections.Generic;
using IdentityServer4.Models;

namespace Labs.Security.Auth.Data
{
    public class SiliconData
    {
        public IEnumerable<ApiResource> Load()
        {
            yield return CompanionApi();
            yield return SandboxApi();
        }

        private static ApiResource CompanionApi()
        {
            return new ApiResource("permissions", "Permissions");
        }

        private static ApiResource SandboxApi()
        {
            return new ApiResource
            {
                Name = "sandbox",
                DisplayName = "Sandbox (resource scope)",
                Description = "Encapsulates permissions for the sandbox api",
                Enabled = true,
                UserClaims = new List<string>
                {
                    "userId",
                    "userName",
                    "userLabel",
                    "personId",
                    "firstName",
                    "lastName",
                    "fullName",
                    "aliasName",
                },
                Scopes = new List<Scope>
                {
                    new Scope
                    {
                        Name = "sandbox.full",
                        DisplayName = "Sandbox Permissions",
                        Description = "Grants full permissions for the sandbox api",
                    },
                    new Scope
                    {
                        Name = "sandbox.read",
                        DisplayName = "Sandbox Read",
                        Description = "Grants read permissions for the sandbox api",
                    },
                    new Scope
                    {
                        Name = "sandbox.write",
                        DisplayName = "Sandbox Write",
                        Description = "Grants write permissions for the sandbox api",
                    },
                }
            };
        }
    }
}