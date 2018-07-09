using System.Numerics;

namespace Arkivverket.Arkade.Core.Base.Addml.Processes
{
    public class MinAndMax
    {
        private BigInteger? _max;
        private BigInteger? _min;

        public BigInteger? GetMin()
        {
            return _min;
        }

        public BigInteger? GetMax()
        {
            return _max;
        }

        public void NewValue(BigInteger value)
        {
            if (!_max.HasValue || value > _max)
            {
                _max = value;
            }

            if (!_min.HasValue || value < _min)
            {
                _min = value;
            }
        }
    }
}