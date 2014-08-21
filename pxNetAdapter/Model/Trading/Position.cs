using System;
using System.Collections.Generic;

namespace pxNetAdapter.Model.Trading
{
	public class Position
	{
		public Position(IDictionary<string, object> data)
		{
			if (data == null)
				return;

			Symbol = Utils.GetValue(data, "symbol", "");
			GUID = Utils.GetValue(data, "GUID", "");
			AccountGUID = Utils.GetValue(data, "accountGUID", "");
			DisplayId = Utils.GetValue(data, "displayId", "");
			OpenTimestamp = Utils.GetValue(data, "openTimestamp", DateTime.MinValue);
			ExpirationTimestamp = Utils.GetValue(data, "expirationTimestamp", DateTime.MinValue);
			CloseTimestamp = Utils.GetValue(data, "closeTimestamp", DateTime.MinValue);
			Status = Utils.GetEnumValue(data, "status", StatusEnum.None);
			Type = Utils.GetEnumValue(data, "type", TypeEnum.None);
			Side = Utils.GetEnumValue(data, "side", SideEnum.None);
			Price = Utils.GetValue(data, "price", 0.0);
			BuyQty = Utils.GetValue(data, "buyQuantity", 0.0);
			BuyAsset = Utils.GetValue(data, "buyAsset", "");
			SellQty = Utils.GetValue(data, "sellQuantity", 0.0);
			SellAsset = Utils.GetValue(data, "sellAsset", "");
			ClosePrice = Utils.GetValue(data, "closePrice", 0.0);
			PNL = Utils.GetValue(data, "pnl", 0.0);
		}

		public string Symbol { get; set; }
		public string GUID { get; set; }
		public string AccountGUID { get; set; }
		public string DisplayId { get; set; }
		public DateTime OpenTimestamp { get; set; }
		public DateTime ExpirationTimestamp { get; set; }
		public DateTime CloseTimestamp { get; set; }
		public StatusEnum Status { get; set; }
		public TypeEnum Type { get; set; }
		public SideEnum Side { get; set; }
		public double Price { get; set; }
		public double BuyQty { get; set; }
		public string BuyAsset { get; set; }
		public double SellQty { get; set; }
		public string SellAsset { get; set; }
		public double ClosePrice { get; set; }
		public double PNL { get; set; }
	}
}
