using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace pxNetAdapter
{
	public class ReqResConverter : JavaScriptConverter
	{
		public override IEnumerable<Type> SupportedTypes
		{
			get { return new Type[] { typeof(Response.Response), typeof(Request.Request) }; }
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			Request.Request request = obj as Request.Request;
			if (request == null)
				return null;

			return request.ToDictionary();
		}

		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			if (dictionary == null)
				throw new ArgumentNullException("dictionary");

			if (type != typeof(Response.Response))
				return null;

			string resposeType = Utils.GetValue<string>(dictionary, "qualifier", "");
			if (string.IsNullOrEmpty(resposeType))
				return null;

			Response.IResponse response = null;
			switch (resposeType)
			{
				case "LoginResponse":
					response = new Response.LoginResponse(dictionary);
					break;
			}

			return response;
		}
	}
}
