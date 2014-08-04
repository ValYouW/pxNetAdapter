using System.Collections.Generic;

namespace pxNetAdapter
{
	public static class Utils
	{
		public static T GetValue<T>(IDictionary<string, object> data, string key, T defaultValue)
		{
			if (!data.ContainsKey(key))
				return defaultValue;

			T res;
			try
			{
				res = (T)data[key];
			}
			catch
			{
				res = defaultValue;
			}

			return res;
		}
	}
}
