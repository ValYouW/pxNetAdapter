using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using pxNetAdapter.Response;
using pxNetAdapter.Response.Data;

namespace pxNetAdapter
{
	public class ReqResConverter : JavaScriptConverter
	{
		public override IEnumerable<Type> SupportedTypes
		{
			get { return new[] { typeof(Response.Response), typeof(Request.Request) }; }
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

			IResponse response = new Response.Response(dictionary);
			IDictionary<string, object> data = null;
			if (dictionary.ContainsKey("data"))
				data = dictionary["data"] as IDictionary<string, object>;

			switch (response.Qualifier)
			{
				case ResponseTypeEnum.LoginResponse:
					response.Data = new LoginResponseData(data);
					break;
				case ResponseTypeEnum.QuoteUpdateResponse:
					response.Data = new QuoteUpdateResponseData(data);
					break;
				case ResponseTypeEnum.InitialAppDataResponse:
					response.Data = new InitialAppDataResponseData(data);
					break;
				case ResponseTypeEnum.OpenPositionResponse:
				case ResponseTypeEnum.ModifyPositionResponse:
				case ResponseTypeEnum.ClosePositionResponse:
					response.Data = new PositionResponseData(data);
					break;
			}

			return response;
		}
	}
}
