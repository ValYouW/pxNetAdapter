using pxNetAdapter.Request.Data;
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
}
