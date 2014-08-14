using System.Collections.Generic;

namespace pxNetAdapter.Response.MarketData
{
	public class QuoteUpdateResponseData : IResponseData
	{
		public QuoteUpdateResponseData(IDictionary<string, object> data)
		{
			Quotes = new Dictionary<string, object>();
			if (data == null || !data.ContainsKey("quotes"))
				return;

			data = data["quotes"] as IDictionary<string, object>;
			if (data == null)
				return;

			Quote q;
			IDictionary<string, object> rawQ;
			foreach (string symbol in data.Keys)
			{
				rawQ = data[symbol] as IDictionary<string, object>;
				if (rawQ == null)
					continue;
				
				q = new Quote(rawQ);
				Quotes[symbol] = q;
			}
		}

		public IDictionary<string, object> Quotes { get; private set; }
	}
}
