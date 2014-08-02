using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pxNetAdapter.Response
{
	public class LoginResponse : Response
	{
		public LoginResponse(IDictionary<string, object> data) : base(data)
		{
			if (data == null)
				return;

			if (data.ContainsKey("data"))
				Data = new LoginResponseData(data["data"] as IDictionary<string, object>);
		}

		public LoginResponseData Data { get; private set; }
	}

	public class LoginResponseData : IResponseData
	{
		public LoginResponseData(IDictionary<string, object> data)
		{
			if (data == null)
				return;

			Token = Utils.GetValue<string>(data, "token", "");
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
			GUID = Utils.GetValue<string>(data, "GUID", "");
			BusinessUnitId = Utils.GetValue<string>(data, "businessUnitId", "");
			FirstName = Utils.GetValue<string>(data, "firstName", "");
			LastName = Utils.GetValue<string>(data, "lastName", "");
			IsRegulated = Utils.GetValue<bool>(data, "isRegualted", false);
			NeedRegulationInfo = Utils.GetValue<bool>(data, "needRegulationInfo", false);

			Accounts = new List<Account>();
			System.Collections.IEnumerable accts = data["accounts"] as System.Collections.IEnumerable;
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

			GUID = Utils.GetValue<string>(data, "GUID", "");
			Type = Utils.GetValue<string>(data, "type", "");
			Balance = Utils.GetValue<decimal>(data, "balance", 0);
			Currency = Utils.GetValue<string>(data, "currency", "");
			CurrencySymbol = Utils.GetValue<string>(data, "currencySymbol", "");
		}

		public string GUID { get; protected set; }
		public string Type { get; protected set; }
		public decimal Balance { get; protected set; }
		public string Currency { get; protected set; }
		public string CurrencySymbol { get; protected set; }
	}
}
