using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pxNetAdapter.Request.Data
{
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
