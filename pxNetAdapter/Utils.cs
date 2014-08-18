using System;
using System.Collections.Generic;

namespace pxNetAdapter
{
	public class GenericEventArgs<T> : EventArgs
	{
		public GenericEventArgs(T args)
		{
			Args = args;
		}

		public T Args { get; private set; }
	}

	public static class Utils
	{
		public static T GetValue<T>(IDictionary<string, object> data, string key, T defaultValue)
		{
			if (!data.ContainsKey(key))
				return defaultValue;

			T res;

			if (typeof(T) == typeof(DateTime))
			{
				string exp = GetValue(data, key, "");
				if (string.IsNullOrEmpty(exp))
					return defaultValue;

				DateTime tmp;
				if (DateTime.TryParse(exp, out tmp))
					return (T)Convert.ChangeType(tmp, typeof(DateTime));

				return defaultValue;
			}

			try
			{
				res = (T)Convert.ChangeType(data[key], typeof(T));
			}
			catch
			{
				res = defaultValue;
			}

			return res;
		}

		public static T GetEnumValue<T>(IDictionary<string, object> data, string key, T defaultValue) where T : struct
		{
			string val = GetValue(data, key, "");
			if (string.IsNullOrEmpty(val))
				return defaultValue;

			T tmp;
			try
			{
				return (T)Enum.Parse(typeof(T), val, true);
			}
			catch
			{
				return defaultValue;
			}
		}
	}
}
