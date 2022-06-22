﻿using System;

namespace Arkivverket.Arkade.Core.Util
{
    public class NorwegianBirthNumber : NorwegianIdNumberBase
    {
        private static Random _random = new Random();

        public NorwegianBirthNumber(string id)
        {
            _id = StripSpace(id);
            if (!Verify(_id))
            {
                throw new ArgumentException("Illegal birth number: " + _id);
            }
        }

        public static NorwegianBirthNumber Create(string birthNumber)
        {
            return new NorwegianBirthNumber(birthNumber);
        }

        public static bool Verify(string id)
        {
            id = StripSpace(id);

            if (id.Length != 11)
            {
                return false;
            }

            if (!StringUtil.IsOnlyDigits(id))
            {
                return false;
            }

            string actualChecksum = id.Substring(9, 2);
            string calculatedChecksum = CalculateChecksumPart(id.Substring(0, 9));

            return actualChecksum == calculatedChecksum;
        }
        protected bool Equals(NorwegianBirthNumber other)
        {
            return string.Equals(_id, other._id);
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
            return Equals((NorwegianBirthNumber)obj);
        }
        public static NorwegianBirthNumber CreateRandom()
        {
            string datePart;
            do
            {
                datePart = "";
                datePart += _random.Next(1, 32).ToString("D2");
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

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}