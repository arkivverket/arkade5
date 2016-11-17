using System;

namespace Arkivverket.Arkade.Util
{
    public class NorwegianBirthNumber
    {
        private static Random _random = new Random();

        private readonly string _birthNumber;

        private NorwegianBirthNumber(string birthNumber)
        {
            _birthNumber = StripSpace(birthNumber);
            if (!Verify(_birthNumber))
            {
                throw new ArgumentException("Illegal birth number: " + _birthNumber);
            }
        }

        private static string StripSpace(string s)
        {
            return s.Replace(" ", "");
        }

        public static NorwegianBirthNumber Create(string birthNumber)
        {
            return new NorwegianBirthNumber(birthNumber);
        }

        public static NorwegianBirthNumber CreateRandom()
        {
            string datePart;
            do
            {
                datePart = "";
                datePart += _random.Next(1, 31).ToString("D2");
                datePart += _random.Next(1, 13).ToString("D2");
                datePart += _random.Next(0, 100).ToString("D2");
            } while (!DateExists(datePart)); // If we created a date which does not exist, try again!

            string dateAndPersonNumberPart;
            string checksumPart;
            do
            {
                string personNumberPart = _random.Next(0, 999).ToString("D3");
                dateAndPersonNumberPart = datePart + personNumberPart;
                checksumPart = CalculateChecksumPart(dateAndPersonNumberPart);
            } while (checksumPart.Length != 2);
            // If checkum were calculated to something other than two digits, the birth number is invalid, try again!

            return new NorwegianBirthNumber(dateAndPersonNumberPart + checksumPart);
        }

        public static NorwegianBirthNumber CreateRandom(string seed)
        {
            _random = new Random(seed.GetHashCode());
            return CreateRandom();
        }

        private static string CalculateChecksumPart(string dateAndPersonNumberPart)
        {
            int[] b = StringUtil.ToIntArray(dateAndPersonNumberPart);

            int first = 11 -
                        (((3*b[0]) + (7*b[1]) + (6*b[2]) + (1*b[3]) + (8*b[4]) + (9*b[5]) + (4*b[6]) + (5*b[7]) +
                          (2*b[8]))
                         %11);
            first = (first == 11 ? 0 : first);


            int second = 11 -
                         (((5*b[0]) + (4*b[1]) + (3*b[2]) + (2*b[3]) + (7*b[4]) + (6*b[5]) + (5*b[6]) + (4*b[7]) +
                           (3*b[8]) +
                           (2*first))%11);
            second = (second == 11 ? 0 : second);

            return first.ToString() + second.ToString();
        }

        private static bool DateExists(string date)
        {
            try
            {
                DateTime.ParseExact(date, "ddMMyy", System.Globalization.CultureInfo.InvariantCulture);
                return true;
            }
            catch (FormatException e)
            {
                return false;
            }
        }

        public static bool Verify(string birthNumber)
        {
            birthNumber = StripSpace(birthNumber);

            if (birthNumber.Length != 11)
            {
                return false;
            }

            if (!StringUtil.IsOnlyDigits(birthNumber))
            {
                return false;
            }

            string actualChecksum = birthNumber.Substring(9, 2);
            string calculatedChecksum = CalculateChecksumPart(birthNumber.Substring(0, 9));

            return actualChecksum == calculatedChecksum;
        }


        protected bool Equals(NorwegianBirthNumber other)
        {
            return string.Equals(_birthNumber, other._birthNumber);
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
            return Equals((NorwegianBirthNumber) obj);
        }

        public override int GetHashCode()
        {
            return _birthNumber?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return _birthNumber;
        }
    }
}