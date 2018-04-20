using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace BorBaWebSite.Views
{
	public class CriteriaContractResolver : DefaultContractResolver
	{
		public CriteriaContractResolver()
		{
		}

		protected override IList<JsonProperty> CreateProperties(
			Type type, MemberSerialization memberSerialization)
		{
			IList<JsonProperty> filtered = new List<JsonProperty>();

			foreach (JsonProperty p in base.CreateProperties(type, memberSerialization))
			{
				if (!p.PropertyType.FullName.Contains("EFModel.")) // excluding complex types
				{
					filtered.Add(p);
				}
			}

			return filtered;
		}
	}
	// converting from Json formatted string to an object and vice versa
	public static class JSONExtension
	{
		public const string SINGLE_QUOTE_MARKER = "|||";
		public const string DOUBLE_QUOTE_MARKER = "@@@";

		public static string WrapQuotes(string val)
		{
			return val.Replace("'", SINGLE_QUOTE_MARKER).Replace("\"", DOUBLE_QUOTE_MARKER);
		}
		public static string ExtractWrappedQuotes(string val)
		{
			return val.Replace(SINGLE_QUOTE_MARKER, "'").Replace(DOUBLE_QUOTE_MARKER, "\"");
		}

		public static string ToJSON(this object obj)
		{
			return JsonConvert.SerializeObject(
				obj,
				Formatting.Indented,
				new JsonSerializerSettings { ContractResolver = new CriteriaContractResolver() }
				);
		}

		public static T ToObject<T>(this string value)
		{
			return JsonConvert.DeserializeObject<T>(
				value,
				new JsonSerializerSettings { ContractResolver = new CriteriaContractResolver() });
		}

	}
}
