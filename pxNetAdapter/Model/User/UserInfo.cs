using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pxNetAdapter.Model.User
{
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
}
