using System.Collections;
using System.Collections.Generic;
using pxNetAdapter.Model.User;

namespace pxNetAdapter.Response.Data
{
	public class LoginResponseData : IResponseData
	{
		public LoginResponseData(IDictionary<string, object> data)
		{
			if (data == null)
				return;

			Token = Utils.GetValue(data, "token", "");
			if (data.ContainsKey("userInfo"))
				UserInfo = new UserInfo(data["userInfo"] as IDictionary<string, object>);
		}

		public string Token { get; private set; }
		public UserInfo UserInfo { get; private set; }
	}
}
