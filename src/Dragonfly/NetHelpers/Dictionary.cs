namespace Dragonfly.NetHelpers;

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

/// <summary>
/// Serializable Dictionary
/// </summary>

[Serializable]
public static class Dictionary
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="Jobject"></param>
	/// <returns></returns>
	public static IDictionary<string, string> JobjectToDictionary(dynamic Jobject)
	{
		var dict = new Dictionary<string, string>();

		JObject cfg = Jobject;
		if (cfg != null)
		{
			if (cfg.Properties() != null)
			{
				foreach (JProperty jprop in cfg.Properties())
				{
					dict.Add(jprop.Name, jprop.Value.ToString());
				}
			}
		}

		return dict;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="Dict1"></param>
	/// <param name="Dict2"></param>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="S"></typeparam>
	/// <returns></returns>
	public static IDictionary<T, S> CombineDictionaries<T, S>(IDictionary<T, S> Dict1, IDictionary<T, S> Dict2)
	{
		if (Dict1 == null)
		{
			return Dict2;
		}

		if (Dict2 == null)
		{
			return Dict1;
		}

		foreach (var item in Dict2)
		{
			if (!Dict1.ContainsKey(item.Key))
			{
				Dict1.Add(item.Key, item.Value);
			}
			else
			{
				// handle duplicate key issue here
			}
		}

		return Dict1;
	}
}

