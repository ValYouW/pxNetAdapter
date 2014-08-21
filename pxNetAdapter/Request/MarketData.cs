using pxNetAdapter.Request.Data;
using System.Collections.Generic;

namespace pxNetAdapter.Request
{
	public static class MarketData
	{
		public static IRequest SubscribeForQuotes(string token, string userGUID)
		{
			IRequest req = new Request("marketData/subscribeForQuotes");
			req.Data = new QuotesRequestData(token, userGUID);
			return req;
		}
	}
}
