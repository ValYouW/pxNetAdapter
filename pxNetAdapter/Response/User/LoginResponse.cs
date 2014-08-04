using System.Collections;
using System.Collections.Generic;

namespace pxNetAdapter.Response.User
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

	public class UserInfo
	{
		public UserInfo(IDictionary<string, object> data)
		{
			GUID = Utils.GetValue(data, "GUID", "");
			BusinessUnitId = Utils.GetValue(data, "businessUnitId", "");
			FirstName = Utils.GetValue(data, "firstName", "");
			LastName = Utils.GetValue(data, "lastName", "");
			IsRegulated = Utils.GetValue(data, "isRegualted", false);
			NeedRegulationInfo = Utils.GetValue(data, "needRegulationInfo", false);

			Accounts = new List<Account>();
			IEnumerable accts = data["accounts"] as IEnumerable;
			if (accts == null)
				return;
			
			foreach (object acct in accts)
			{
				Accounts.Add(new Account(acct as IDictionary<string, object>));
			}
		}

		public string GUID { get; private set; }
		public string BusinessUnitId { get; private set; }
		public string FirstName { get; private set; }
		public string LastName { get; private set; }
		public bool IsRegulated { get; private set; }
		public bool NeedRegulationInfo { get; private set; }
		public IList<Account> Accounts { get; private set; }
	}

	public class Account
	{
		public Account(IDictionary<string, object> data)
		{
			if (data == null)
				return;

			GUID = Utils.GetValue(data, "GUID", "");
			Type = Utils.GetValue(data, "type", "");
			Balance = Utils.GetValue<decimal>(data, "balance", 0);
			Currency = Utils.GetValue(data, "abc", "");
			CurrencySymbol = Utils.GetValue(data, "abcSign", "");
		}

		public string GUID { get; protected set; }
		public string Type { get; protected set; }
		public decimal Balance { get; protected set; }
		public string Currency { get; protected set; }
		public string CurrencySymbol { get; protected set; }
	}
}
