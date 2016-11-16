using System;
using System.Text;

namespace Arkivverket.Arkade.Util
{
    public class NorwegianOrganizationNumber
    {
        private static Random _random = new Random();
        private readonly string _organizationNumber;

        private NorwegianOrganizationNumber(string örganizationNumber)
        {
            _organizationNumber = örganizationNumber
                .Replace(".", "")
                .Replace(" ", "");
        }

        public static NorwegianOrganizationNumber Create(string organizationNumber)
        {
            return new NorwegianOrganizationNumber(organizationNumber);
        }

        public static NorwegianOrganizationNumber CreateRandom()
        {
            string organizationNumberWithoutChecksum;
            string checksum;
            do
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < 8; i++)
                {
                    sb.Append(_random.Next(0, 10));
                }
                organizationNumberWithoutChecksum = sb.ToString();
                checksum = CalculateChecksumPart(organizationNumberWithoutChecksum);
            } while (checksum.Length != 1);
            // If checkum were calculated to something other than one digit, the organization number is invalid, try again!

            return Create(organizationNumberWithoutChecksum + checksum);
        }

        public static NorwegianOrganizationNumber CreateRandom(string seed)
        {
            _random = new Random(seed.GetHashCode());
            return CreateRandom();
        }

        // https://no.wikipedia.org/wiki/MOD11
        private static string CalculateChecksumPart(string organizationNumberWithoutChecksum)
        {
            int[] b = StringUtil.ToIntArray(organizationNumberWithoutChecksum);

            int checksumDigit = 11 -
                                (((3*b[0]) + (2*b[1]) + (7*b[2]) + (6*b[3]) + (5*b[4]) + (4*b[5]) + (3*b[6]) + (2*b[7]))%
                                 11);
            checksumDigit = (checksumDigit == 11 ? 0 : checksumDigit);

            return checksumDigit.ToString();
        }

        public bool Verify()
        {
            string actualChecksum = _organizationNumber.Substring(8, 1);
            string calculatedChecksum = CalculateChecksumPart(_organizationNumber.Substring(0, 8));

            return actualChecksum == calculatedChecksum;
        }

        public override string ToString()
        {
            return _organizationNumber;
        }

        public string ToString(string separator)
        {
            return _organizationNumber.Substring(0, 3) + separator + _organizationNumber.Substring(3, 3) + separator +
                   _organizationNumber.Substring(6, 3);
        }

        protected bool Equals(NorwegianOrganizationNumber other)
        {
            return string.Equals(_organizationNumber, other._organizationNumber);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((NorwegianOrganizationNumber) obj);
        }

        public override int GetHashCode()
        {
            return (_organizationNumber != null ? _organizationNumber.GetHashCode() : 0);
        }
    }
}