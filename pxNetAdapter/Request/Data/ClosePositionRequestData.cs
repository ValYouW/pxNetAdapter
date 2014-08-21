using pxNetAdapter.Model.Trading;
using System.Collections.Generic;

namespace pxNetAdapter.Request.Data
{
	public class ClosePositionRequestData : IRequestData
	{
		private string m_token;
		private string m_posGUID;
		private Order m_order;

		public ClosePositionRequestData(string token, string positionGUID, Order order)
		{
			m_token = token;
			m_posGUID = positionGUID;
			m_order = order;
		}

		public void FillData(IDictionary<string, object> data)
		{
			data["token"] = m_token;
			data["positionGUID"] = m_posGUID;
			
			IDictionary<string, object> order = new Dictionary<string, object>();
			order["accountGUID"] = m_order.AccountGUID;
			order["type"] = m_order.Type.ToString();
			order["side"] = m_order.Side.ToString();
			order["price"] = m_order.Price;
			order["quoteGUID"] = m_order.QuoteGUID;
			order["pnl"] = m_order.PNL;
			order["pnlCurrency"] = m_order.PNLCurrency;

			data["order"] = order;
		}
	}
}
