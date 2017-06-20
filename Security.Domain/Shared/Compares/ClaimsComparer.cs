using System.Collections.Generic;
using System.Security.Claims;

namespace Labs.Security.Domain.Shared.Compares
{
    public class ClaimsComparer : IEqualityComparer<Claim>
    {
        public ClaimsComparer()
        {
            ValueAndTypeOnly = false;
        }

        public ClaimsComparer(bool compareValueAndTypeOnly)
        {
            ValueAndTypeOnly = compareValueAndTypeOnly;
        }

        protected bool ValueAndTypeOnly { get; }

        public bool Equals(Claim x, Claim y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;

            if (ValueAndTypeOnly)
            {
                if (x.Type == y.Type)
                {
                    return x.Value == y.Value;
                }

                return false;
            }

            if (x.Type == y.Type && x.Value == y.Value && x.Issuer == y.Issuer)
            {
                return x.ValueType == y.ValueType;
            }

            return false;
        }

        public int GetHashCode(Claim claim)
        {
            if (claim == null)
                return 0;
            var type = claim.Type;
            var num1 = type != null ? type.GetHashCode() : 0;
            var str = claim.Value;
            var num2 = str != null ? str.GetHashCode() : 0;
            return num1 ^ num2;
        }
    }
}