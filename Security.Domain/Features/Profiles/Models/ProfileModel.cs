﻿using Infsys.Security.Auth.Core.Shared.Messages;

namespace Infsys.Security.Auth.Core.Features.Profiles.Models
{
    public class ProfileModel : IModel
    {
        public string AliasName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public string EmailAddress { get; set; }

        public string PhoneNumber { get; set; }

        public string CompanyName { get; set; }

        public string OrganizationPath { get; set; }

        public string DepartmentName { get; set; }

        public string FunctionName { get; set; }

        public string OfficeName { get; set; }

        public string ContractType { get; set; }

        public string CultureCode { get; set; }

        public string PicturePath { get; set; }
    }
}