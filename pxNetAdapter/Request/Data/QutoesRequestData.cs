using System.Collections.Generic;

namespace pxNetAdapter.Request.Data
{
	public class QuotesRequestData : IRequestData
	{
		private string m_token;
		private string m_userGUID;

		public QuotesRequestData(string token, string userGUID)
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
