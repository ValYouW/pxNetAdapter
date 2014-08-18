using System.Collections.Generic;
using pxNetAdapter.Response.Assets;
using pxNetAdapter.Response.MarketData;
using pxNetAdapter.Response.Trading;

namespace pxNetAdapter.Response.TradingApp
{
	public class InitialAppDataResponseData : IResponseData
	{
		public InitialAppDataResponseData(IDictionary<string, object> data)
		{
			Positions = new List<Position>();
			Orders = new List<Position>();
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

			if (data.ContainsKey("positions"))
			{
				IDictionary<string, object> psts = data["positions"] as IDictionary<string, object>;
				if (psts != null)
				{
					foreach (object p in psts.Values)
					{
						Positions.Add(new Position(p as IDictionary<string, object>));
					}
				}
			}

			if (data.ContainsKey("orders"))
			{
				IDictionary<string, object> ords = data["orders"] as IDictionary<string, object>;
				if (ords != null)
				{
					foreach (object o in ords.Values)
					{
						Orders.Add(new Position(o as IDictionary<string, object>));
					}
				}
			}
		}

		public IList<Position> Positions { get; private set; }
		public IList<Position> Orders { get; private set; }
		public IDictionary<string, Quote> Quotes { get; private set; }
		public IDictionary<string, Asset> Assets { get; private set; }
	}
}
