using pxNetAdapter.Model.Trading;
using pxNetAdapter.Request.Data;

namespace pxNetAdapter.Request
{
	public static class Trading
	{
		public static IRequest OpenPosition(string token, Order order)
		{
			IRequest req = new Request("trading/openPosition");
			req.Data = new OpenPositionRequestData(token, order);
			return req;
		}

		public static IRequest ClosePosition(string token, string positionGUID, Order order)
		{
			IRequest req = new Request("trading/closePosition");
			req.Data = new ClosePositionRequestData(token, positionGUID, order);
			return req;
		}
	}
}
