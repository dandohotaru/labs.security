using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Labs.Security.Domain.Adfs.Shared.Extensions;
using Labs.Security.Domain.Features.Profiles.Providers;
using Labs.Security.Domain.Shared.Exceptions;
using Labs.Security.Domain.Shared.Extensions;

namespace Labs.Security.Domain.Adfs.Profiles
{
    public class DirectoryIdentityProvider : IIdentityProvider
    {
        public Task<IdentityData[]> Search(AliasesCriterion criterion)
        {
            if (criterion == null)
                throw new GuardException("The criterion cannot be null");
            if (criterion.Aliases == null)
                throw new GuardException("The criterion names cannot be null");
            if (criterion.Aliases.Empty())
                throw new GuardException("The criterion names cannot be empty");

            // ToDo: Encapsulate setup at construction level [DanD]

            // Setup
            var identity = WindowsIdentity.GetCurrent();
            var user = identity.User;
            if (user == null)
                throw new Exception("The user could not be retrieved from identity");

            var context = new PrincipalContext(ContextType.Domain);
            var principal = UserPrincipal.FindByIdentity(context, IdentityType.Sid, user.Value);
            if (principal == null)
                throw new Exception("The principal could not be retrieved from identity");

            var node = principal.GetUnderlyingObject() as DirectoryEntry;
            if (node == null)
                throw new Exception("The underlying object for principal is not a directory entry");

            while (node.SchemaClassName != "domainDNS")
            {
                node = node.Parent;
            }

            // Search
            var names = from name in criterion.Aliases
                        where !string.IsNullOrEmpty(name)
                        select $"(name={name})";
            var filter = $"(|{names.Collate()})";

            var attributes = new[]
            {
                "cn",
                "sn",
                "name",
                "givenname",
                "displayname",
                "mail",
                "company",
                "department",
                "description",
                "memberof",
                "department",
                "extensionattribute9",
                "extensionattribute10",
                "extensionattribute11",
                "physicaldeliveryofficename",
                "msexchuserculture",
                "physicaldeliveryofficename",
                "telephonenumber",
                "homedirectory",
            };

            var searcher = new DirectorySearcher(node, filter, attributes)
            {
                SearchScope = SearchScope.Subtree,
                Sort = new SortOption("cn", SortDirection.Ascending),
            };
            var results = searcher.FindAll();

            // Projection
            var profiles = from result in results.OfType<SearchResult>()
                           select new IdentityData
                           {
                               AliasName = result.Parse<string>("cn"),
                               FirstName = result.Parse<string>("givenname"),
                               LastName = result.Parse<string>("sn"),
                               FullName = result.Parse<string>("displayname"),
                               EmailAddress = result.Parse<string>("mail"),
                               PhoneNumber = result.Parse<string>("telephonenumber"),
                               CompanyName = result.Parse<string>("company"),
                               OrganizationPath = result.Parse<string>("extensionattribute11"),
                               DepartmentName = result.Parse<string>("department"),
                               FunctionName = result.Parse<string>("extensionattribute9"),
                               OfficeName = result.Parse<string>("physicaldeliveryofficename"),
                               ContractType = result.Parse<string>("description"),
                               CultureCode = result.Parse<string>("msexchuserculture"),
                               PicturePath = result.Parse<string>("extensionattribute10"),
                           };

            return Task.FromResult(profiles.ToArray());
        }
    }
}