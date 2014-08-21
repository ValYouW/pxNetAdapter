using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pxNetAdapter.Model.Trading
{
	public class Order
	{
		public string Symbol { get; set; }
		public string AccountGUID { get; set; }
		public TypeEnum Type { get; set; }
		public SideEnum Side { get; set; }
		public double Price { get; set; }
		public double Quantity { get; set; }
		public string QuoteGUID { get; set; }
		public double StopLossPrice { get; set; }
		public double StopLossAmount { get; set; }
		public double TakeProfitPrice { get; set; }
		public double TakeProfitAmount { get; set; }
		public string SLTPCurrency { get; set; }
		public double PNL { get; set; }
		public string PNLCurrency { get; set; }
	}
}
