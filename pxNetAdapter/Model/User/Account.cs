using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pxNetAdapter.Model.User
{
	public class Account
	{
		public Account(IDictionary<string, object> data)
		{
			if (data == null)
				return;

			GUID = Utils.GetValue(data, "GUID", "");
			Type = Utils.GetValue(data, "type", "");
			Balance = Utils.GetValue<decimal>(data, "balance", 0);
			Currency = Utils.GetValue(data, "abc", "");
			CurrencySymbol = Utils.GetValue(data, "abcSign", "");
		}

		public string GUID { get; protected set; }
		public string Type { get; protected set; }
		public decimal Balance { get; protected set; }
		public string Currency { get; protected set; }
		public string CurrencySymbol { get; protected set; }
	}
}
