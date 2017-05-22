using System;
using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Util
{
    public class Assert
    {

        public static void AssertNotNull(string variableName, object o)
        {
            if (o == null)
            {
                throw new ArkadeException(string.Format(Resources.AddmlMessages.AssertNotNull, variableName));
            }
        }
        public static void AssertNotNullOrEmpty(string variableName, string o)
        {
            if (string.IsNullOrEmpty(o))
            {
                throw new ArkadeException(string.Format(Resources.AddmlMessages.AssertNotNullOrEmpty, variableName));
            }
        }

    }
}
