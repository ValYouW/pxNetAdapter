using System;
using System.Collections.Generic;

namespace pxNetAdapter.Model.Assets
{
	public class Asset
	{
		public Asset(IDictionary<string, object> data)
		{
			if (data == null)
				return;

			Symbol = Utils.GetValue(data, "symbol", "");
			Name = Utils.GetValue(data, "assetName", "");
			Description = Utils.GetValue(data, "description", "");
			ExchangeCode = Utils.GetValue(data, "exchangeCode", "");
			Type = Utils.GetValue(data, "type", "");
			Category = Utils.GetValue(data, "category", "");
			Active = Utils.GetValue(data, "isActive", false);
			Tradable = Utils.GetValue(data, "isTradable", false);
			AllowedForTrading = Utils.GetValue(data, "isAllowedForTrading", false);
			Leverage = Utils.GetValue(data, "leverage", 0);

			Expiration = Utils.GetValue(data, "contractExpiration", DateTime.MinValue);
		}

		public string Symbol { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string ExchangeCode { get; set; }
		public string Type { get; set; }
		public string Category { get; set; }
		public bool Active { get; set; }
		public bool AllowedForTrading { get; set; }
		public bool Tradable { get; set; }
		public int Leverage { get; set; }
		public DateTime Expiration { get; set; }
	}
}
