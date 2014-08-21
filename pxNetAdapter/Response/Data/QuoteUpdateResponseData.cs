using System.Collections.Generic;
using pxNetAdapter.Model.Assets;
using pxNetAdapter.Model.MarketData;

namespace pxNetAdapter.Response.Data
{
	public class QuoteUpdateResponseData : IResponseData
	{
		public QuoteUpdateResponseData(IDictionary<string, object> data)
		{
			Quotes = new Dictionary<string, Quote>();
			Assets = new Dictionary<string, Asset>();
			if (data == null)
				return;

			if (data.ContainsKey("quotes"))
			{
				IDictionary<string, object> qts = data["quotes"] as IDictionary<string, object>;
				if (qts != null)
				{
					Quote quote;
					foreach (object q in qts.Values)
					{
						quote = new Quote(q as IDictionary<string, object>);
						if (!string.IsNullOrEmpty(quote.Symbol))
							Quotes[quote.Symbol] = quote;
					}
				}
			}

			if (data.ContainsKey("assets"))
			{
				IDictionary<string, object> asts = data["assets"] as IDictionary<string, object>;
				if (asts != null)
				{
					Asset asset;
					foreach (object a in asts.Values)
					{
						asset = new Asset(a as IDictionary<string, object>);
						if (!string.IsNullOrEmpty(asset.Symbol))
							Assets[asset.Symbol] = asset;
					}
				}
			}

		}

		public IDictionary<string, Quote> Quotes { get; private set; }
		public IDictionary<string, Asset> Assets { get; private set; }
	}
}
