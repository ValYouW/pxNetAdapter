using System.Collections.Generic;

namespace pxNetAdapter.Request
{
	public static class TradingApp
	{
		public static IRequest GetInitialAppData(string token, string userGUID)
		{
			IRequest req = new Request("tradingApp/getInitialAppData");
			req.Data = new InitialAppDataRequestData(token, userGUID);
			return req;
		}
	}

	public class InitialAppDataRequestData : IRequestData
	{
		private string m_token;
		private string m_userGUID;

		public InitialAppDataRequestData(string token, string userGUID)
		{
			m_token = token;
			m_userGUID = userGUID;
		}

		public void FillData(IDictionary<string, object> data)
		{
			data["token"] = m_token;
			data["userGUID"] = m_userGUID;
		}
	}
}
