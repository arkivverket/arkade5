using System;

namespace Arkivverket.Arkade.Util
{
    public class Assert
    {

        public static void AssertNotNull(string variableName, object o)
        {
            if (o == null)
            {
                throw new Exception(variableName + " cannot be null");
            }
        }
        public static void AssertNotNullOrEmpty(string variableName, string o)
        {
            if (string.IsNullOrEmpty(o))
            {
                throw new Exception(variableName + " cannot be null or empty");
            }
        }

    }
}
