using System;
using System.Text;

namespace Arkivverket.Arkade.Util
{
    public class NorwegianAccountNumber
    {
        private static Random _random = new Random();
        private readonly string _accountNumber;

        private NorwegianAccountNumber(string accountNumber)
        {
            _accountNumber = StripSpacesAndDots(accountNumber);

            if (!Verify(_accountNumber))
            {
                throw new ArgumentException("Illegal account number: " + _accountNumber);
            }
        }

        private static string StripSpacesAndDots(string s)
        {
            return s
                .Replace(".", "")
                .Replace(" ", "");
        }

        public static NorwegianAccountNumber Create(string accountNumber)
        {
            return new NorwegianAccountNumber(accountNumber);
        }

        public static NorwegianAccountNumber CreateRandom()
        {
            string accountNumberWithoutChecksum;
            string checksum;
            do
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < 10; i++)
                {
                    sb.Append(_random.Next(0, 10));
                }
                accountNumberWithoutChecksum = sb.ToString();
                checksum = CalculateChecksumPart(accountNumberWithoutChecksum);
            } while (checksum.Length != 1);
            // If checkum were calculated to something other than one digit, the account numnber is invalid, try again!

            return Create(accountNumberWithoutChecksum + checksum);
        }

        public static NorwegianAccountNumber CreateRandom(string seed)
        {
            _random = new Random(seed.GetHashCode());
            return CreateRandom();
        }

        // https://no.wikipedia.org/wiki/Kontonummer
        // https://no.wikipedia.org/wiki/MOD11
        private static string CalculateChecksumPart(string accountNumberWithoutChecksum)
        {
            int[] b = StringUtil.ToIntArray(accountNumberWithoutChecksum);

            int checksumDigit = 11 -
                                (((5*b[0]) + (4*b[1]) + (3*b[2]) + (2*b[3]) + (7*b[4]) + (6*b[5]) + (5*b[6]) + (4*b[7]) +
                                  (3*b[8]) + (2*b[9]))
                                 %11);
            checksumDigit = (checksumDigit == 11 ? 0 : checksumDigit);

            return checksumDigit.ToString();
        }

        public static bool Verify(string accountNumber)
        {
            accountNumber = StripSpacesAndDots(accountNumber);

            if (accountNumber.Length != 11)
            {
                return false;
            }

            if (!StringUtil.IsOnlyDigits(accountNumber))
            {
                return false;
            }

            string actualChecksum = accountNumber.Substring(10, 1);
            string calculatedChecksum = CalculateChecksumPart(accountNumber.Substring(0, 10));

            return actualChecksum == calculatedChecksum;
        }

        public override string ToString()
        {
            return _accountNumber;
        }

        public string ToString(string separator)
        {
            return _accountNumber.Substring(0, 4) + separator + _accountNumber.Substring(4, 2) + separator +
                   _accountNumber.Substring(6, 5);
        }

        protected bool Equals(NorwegianAccountNumber other)
        {
            return string.Equals(_accountNumber, other._accountNumber);
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
            return Equals((NorwegianAccountNumber) obj);
        }

        public override int GetHashCode()
        {
            return _accountNumber?.GetHashCode() ?? 0;
        }
    }
}