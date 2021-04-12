using System.Collections.Generic;
using System.Numerics;

namespace Arkivverket.Arkade.Core.Base.Addml.Processes
{
    public class FrequencyList
    {
        private readonly Dictionary<string, BigInteger> _frequencyList = new Dictionary<string, BigInteger>();

        public Dictionary<string, BigInteger> Get()
        {
            return _frequencyList;
        }

        public void Add(string value)
        {
            if (!_frequencyList.ContainsKey(value))
            {
                _frequencyList.Add(value, new BigInteger(0));
            }

            _frequencyList[value]++;
        }
    }
}
