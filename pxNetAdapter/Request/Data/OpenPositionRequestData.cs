using pxNetAdapter.Model.Trading;
using System.Collections.Generic;

namespace pxNetAdapter.Request.Data
{
	public class OpenPositionRequestData : IRequestData
	{
		private string m_token;
		private Order m_order;

		public OpenPositionRequestData(string token, Order order)
		{
			m_token = token;
			m_order = order;
		}

		public void FillData(IDictionary<string, object> data)
		{
			data["token"] = m_token;

			IDictionary<string, object> order = new Dictionary<string, object>();
			order["symbol"] = m_order.Symbol;
			order["type"] = m_order.Type.ToString();
			order["side"] = m_order.Side.ToString();
			order["quantity"] = m_order.Quantity;
			order["price"] = m_order.Price;
			order["quoteGUID"] = m_order.QuoteGUID;
			order["accountGUID"] = m_order.AccountGUID;
			
			data["order"] = order;
		}
	}
}
