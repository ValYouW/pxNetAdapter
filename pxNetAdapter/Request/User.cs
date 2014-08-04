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

	public class LoginRequestData : IRequestData
	{
		private string m_user;
		private string m_password;

		public LoginRequestData(string user, string pwd)
		{
			m_user = user;
			m_password = pwd;
		}

		public void FillData(IDictionary<string, object> data)
		{
			data["user"] = m_user;
			data["password"] = m_password;
		}
	}
}
