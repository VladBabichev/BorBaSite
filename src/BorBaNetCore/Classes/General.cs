using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BorBaNetCore.Classes
{
	public static class General
	{
		public static string GetCompanyName(Assembly asm)
		{
			return asm != null
				? "Compros"// ((AssemblyCompanyAttribute)asm.GetCustomAttribute(typeof(AssemblyCompanyAttribute), false)[0]).Company
				: null;
		}

		public static List<T> ListEnumValues<T>(params T[] excludeValues) where T : struct, IFormattable, IComparable
		{
			Type enumType = typeof(T);
			if (!enumType.IsEnum)
				throw new ArgumentException("Invalid type - must be enum!");

			return Enum.GetValues(enumType)
						.OfType<T>()
						.Except(excludeValues ?? new T[] { })
						.ToList();
		}

//		public static double CalculateDistanceInMeters(double startLatitude, double 
//startLongitude, double endLatitude, double endLongitude)
//		{
//			var start = new GeoCoordinate(startLatitude, startLongitude);
//			var end = new GeoCoordinate(endLatitude, endLongitude);
//			return start.GetDistanceTo(end);
//		}
	}
}
