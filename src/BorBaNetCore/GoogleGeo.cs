using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using BorBaNetCore.Extensions;
using System.Linq;

namespace BorBaNetCore
{
	public static class GoogleGeo
	{
		public static void GetAddressCoordinates(string urlPart, string apiKey, IEnumerable<string> addressTokens, out string latitude, out string longitude, bool toExactAddress = false)
		{
			//TODO temporary
			try
			{
				string[] addressParts = addressTokens.IsNotEmpty() ? addressTokens.First().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : null;
				var geoGoogleUrl = "{0}?address={1}&key={2}".Frmt(urlPart, string.Join("+", addressTokens), apiKey);
				var webClient = new WebClient();

				var webResponse = webClient.DownloadString(geoGoogleUrl);
				var xmlDoc = new XmlDocument();
				xmlDoc.LoadXml(webResponse);

				latitude = "";
				longitude = "";
				XmlNode response = xmlDoc.SelectSingleNode("/GeocodeResponse");
				if (response != null && response.SelectSingleNode("status").InnerText.IsSameAs("OK", ignoreCase: true, trimValue: true))
				{
					XmlNodeList nodes = response.SelectNodes("result");

					foreach (XmlNode node in nodes)
					{
						string address = node.SelectSingleNode("formatted_address").InnerText;
						XmlNode loc = node.SelectSingleNode("geometry/location");
						latitude = loc.SelectSingleNode("lat").InnerText;
						longitude = loc.SelectSingleNode("lng").InnerText;

						if (latitude.IsNotEmpty() && longitude.IsNotEmpty())
						{
							if (addressParts != null && addressParts.None(a => address.Contains(a)))
							{
								// not all address parts are matched - so, don't return incorrect params
								latitude = "";
								longitude = "";
							}
							break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				string err = ex.Message;
				latitude = "";
				longitude = "";
			}
		}
	}
}
