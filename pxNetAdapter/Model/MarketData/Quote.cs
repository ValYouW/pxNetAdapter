using System.Collections.Generic;

namespace pxNetAdapter.Model.MarketData
{
	public class Quote
	{
		public Quote(IDictionary<string, object> data)
		{
			if (data == null)
				return;

			Symbol = Utils.GetValue(data, "symbol", "");
			GUID = Utils.GetValue(data, "GUID", "");
			Mid = double.Parse(Utils.GetValue(data, "mid", "0"));
			Bid = double.Parse(Utils.GetValue(data, "bid", "0"));
			Ask = double.Parse(Utils.GetValue(data, "ask", "0"));
			Open = double.Parse(Utils.GetValue(data, "open", "0"));
			High = double.Parse(Utils.GetValue(data, "high", "0"));
			Low = double.Parse(Utils.GetValue(data, "low", "0"));
			PctChange = Utils.GetValue(data, "pctChange", 0.0);
		}

		public string Symbol { get; set; }
		public string GUID { get; set; }
		public double Mid { get; set; }
		public double Bid { get; set; }
		public double Ask { get; set; }
		public double Open { get; set; }
		public double High { get; set; }
		public double Low { get; set; }
		public double PctChange { get; set; }
	}
}
