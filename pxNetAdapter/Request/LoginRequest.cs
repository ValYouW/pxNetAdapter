using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pxNetAdapter.Request
{
	public class LoginRequest : Request
	{
		public LoginRequest(string user, string pwd)
			: base("user/login")
		{
			Data = new LoginRequestData(user, pwd);
		}

		public LoginRequestData Data { get; private set; }

		public override IDictionary<string, object> ToDictionary()
		{
			IDictionary<string, object> result = base.ToDictionary();
			IDictionary<string, object> data = new Dictionary<string, object>();
			result["data"] = data;
			Data.FillData(data);
			return result;
		}
	}

	public class LoginRequestData
	{
		public LoginRequestData(string user, string pwd)
		{
			Username = user;
			Password = pwd;
		}

		public string Username { get; private set; }
		public string Password { get; private set; }

		public void FillData(IDictionary<string, object> data)
		{
			data["user"] = Username;
			data["password"] = Password;
		}
	}
}
