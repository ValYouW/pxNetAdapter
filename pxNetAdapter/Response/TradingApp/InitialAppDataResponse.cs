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
			Quotes = new List<Quote>();
			Assets = new List<Asset>();

			if (data == null)
				return;

			if (data.ContainsKey("quotes"))
			{
				IDictionary<string, object> qts = data["quotes"] as IDictionary<string, object>;
				if (qts != null)
				{
					foreach (object q in qts.Values)
					{
						Quotes.Add(new Quote(q as IDictionary<string, object>));
					}
				}
			}

			if (data.ContainsKey("assets"))
			{
				IDictionary<string, object> asts = data["assets"] as IDictionary<string, object>;
				if (asts != null)
				{
					foreach (object a in asts.Values)
					{
						Assets.Add(new Asset(a as IDictionary<string, object>));
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
						Positions.Add(new Position(o as IDictionary<string, object>));
					}
				}
			}
		}

		public IList<Position> Positions { get; private set; }
		public IList<Quote> Quotes { get; private set; }
		public IList<Asset> Assets { get; private set; }
	}
}
