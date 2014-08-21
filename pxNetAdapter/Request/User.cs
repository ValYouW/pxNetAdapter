using pxNetAdapter.Request.Data;
using System.Collections.Generic;

namespace pxNetAdapter.Request
{
	public static class User
	{
		public static IRequest Login(string user, string password)
		{
			IRequest req = new Request("user/login");
			req.Data = new LoginRequestData(user, password);
			return req;
		}
	}
}
