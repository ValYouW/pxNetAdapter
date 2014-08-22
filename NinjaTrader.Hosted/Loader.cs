using NinjaTrader.Adapter;
using NinjaTrader.Cbi;
using System;

namespace NinjaTrader.Hosted
{
    [Serializable]
    public class Loader : ILoader
    {
        public IAdapter Create(Connection connection)
        {
            return new PXAdapter(connection);
        }
    }
}