using System;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using BorBaNetCore.Models;
using BorBaNetCore.DataModel;
using System.Drawing.Imaging;
using System.Reflection;
using System.Globalization;
using BorBaNetCore.Classes;
using System.Text.RegularExpressions;

namespace BorBaNetCore.Extensions
{
	public static class Extensions
	{
        public static CurrentUser ToCurrentUser(this Users admin)
        {
            if (admin == null)
                return null;

            return new CurrentUser()
            {
                Id = admin.UserId,
                UserName = admin.UserName,
                FirstName = string.Empty,
                LastName = string.Empty
            };
        }
        /// <summary>
        /// Shorthand method to get service of a specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static T GetService<T>(this IServiceProvider serviceProvider) where T : class
		{
			return serviceProvider.GetService(typeof(T)) as T;
		}

		/// <summary>
		/// Generates an MD5 hash of the tring.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string MD5Hash(this string str)
		{
			using (MD5 md5 = MD5.Create())
			{
				return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(str))).Replace("-", string.Empty);
			}
		}

		/// <summary>
		/// Get bytes from a stream
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static byte[] GetBytes(this Stream stream)
		{
			if (stream is MemoryStream)
			{
				(stream as MemoryStream).ToArray();
			}
			using (MemoryStream memory = new MemoryStream())
			{
				stream.CopyTo(memory);
				return memory.ToArray();
			}
		}

		public static byte[] ResizeImage(this byte[] bytes, int width, int height)
		{
			using (MemoryStream input = new MemoryStream(bytes))
			using (MemoryStream output = new MemoryStream())
			using (System.Drawing.Image image = System.Drawing.Image.FromStream(input))
			using (Bitmap newBitMapt = new Bitmap(image, new Size(width, height)))
			{
				newBitMapt.Save(output, ImageFormat.Jpeg);
				return output.ToArray();
			}
		}

		public static void Remove<T, I>(this ICollection<T> items, I[] removeIDs, Func<T, I> idGetter)
		{
			IList<T> toRemove = new List<T>();
			foreach (T item in items)
			{
				if (removeIDs.Contains(idGetter(item)))
				{
					toRemove.Add(item);
				}
			}

			foreach (T item in toRemove)
			{
				items.Remove(item);
			}
		}

		public static IList<T> Keep<T, I>(this ICollection<T> items, I[] keepIDs, Func<T, I> idGetter)
		{
			IList<T> toRemove;
			if (keepIDs == null)
			{
				// remove all
				toRemove = new List<T>(items);
				items.Clear();
			}
			else
			{
				toRemove = new List<T>();
				foreach (T item in items)
				{
					if (!keepIDs.Contains(idGetter(item)))
					{
						toRemove.Add(item);
					}
				}

				foreach (T item in toRemove)
				{
					items.Remove(item);
				}
			}
			return toRemove;
		}	

		public static IList<Roles> GetRoles(this UnitOfWork unitOfwork)
		{
			return unitOfwork.Set<Roles>().OrderBy(s => s.Name).ToList();
		}

        static readonly Regex _reHissing = new Regex(".*([szx]|ch|sh)$", RegexOptions.IgnoreCase);
        static readonly Regex _reVowelAndY = new Regex(".*[aeiou]y$", RegexOptions.IgnoreCase);
        static readonly Regex _reEndsWithY = new Regex(".*y$", RegexOptions.IgnoreCase);
        static readonly Regex _reEndsWithIS = new Regex(".*is$", RegexOptions.IgnoreCase);
        //	NB: the two below are not always true
        static readonly Regex _reEndsWithIFE = new Regex(".*ife$", RegexOptions.IgnoreCase);
        static readonly Regex _reEndsWithLF = new Regex(".*lf", RegexOptions.IgnoreCase);

        #region ' Is(Not)Empty functions
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
        public static bool IsEmpty<T>(this IEnumerable<T> value)
        {
            return value == null || value.Count() == 0;
        }
        public static bool IsEmpty(this DateTime value)
        {
            return value == DateTime.MinValue;
        }
        public static bool IsEmpty(this DateTime? value)
        {
            return value == null || value.Value.IsEmpty();
        }

        public static bool IsNotEmpty(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
        public static bool IsNotEmpty(this char value)
        {
            return value.ToString().IsNotEmpty();
        }
        public static bool IsNotEmpty<T>(this IEnumerable<T> value)
        {
            return value != null && value.Count() != 0;
        }
        public static bool IsNotEmpty(this DateTime value)
        {
            return value != DateTime.MinValue;
        }
        public static bool IsNotEmpty(this DateTime? value)
        {
            return value != null && value.Value.IsNotEmpty();
        }

        public static bool AreAllEmpty(this IEnumerable<string> values)
        {
            return values.IsEmpty() || values.FirstOrDefault(s => s.IsNotEmpty()) == null;
        }

        public static bool OneNotEmpty(this IEnumerable<string> values)
        {
            return values.IsNotEmpty() && values.FirstOrDefault(s => s.IsNotEmpty()) != null;
        }
        #endregion

        #region '	Is(Not)In
        public static bool IsIn<T>(this T value, params T[] values) where T : struct
        {
            return values.IsNotEmpty() && Array.IndexOf(values, value) != -1;
        }
        public static bool IsIn<T>(this T? value, params T[] values) where T : struct
        {
            return value != null && value.Value.IsIn(values);
        }
        public static bool IsIn(this string value, bool ignoreCase, params string[] values)
        {
            bool result = false;
            if (values.IsNotEmpty())
                foreach (string item in values)
                    if (item.IsSameAs(value, ignoreCase))
                    {
                        result = true;
                        break;
                    }
            return result;
        }
        /// <summary>
        /// Provides case-insensitive comparison
        /// </summary>
        public static bool IsIn(this string value, params string[] values)
        {
            return value.IsIn(true, values);
        }

        public static bool IsNotIn<T>(this T value, params T[] values) where T : struct
        {
            return values.IsEmpty() || Array.IndexOf(values, value) == -1;
        }
        public static bool IsNotIn<T>(this T? value, params T[] values) where T : struct
        {
            return value == null || value.Value.IsNotIn(values);
        }
        public static bool IsNotIn(this string value, bool ignoreCase, params string[] values)
        {
            bool result = true;
            if (values.IsNotEmpty())
                foreach (string item in values)
                    if (item.IsSameAs(value, ignoreCase))
                    {
                        result = false;
                        break;
                    }
            return result;
        }
        /// <summary>
        /// Provides case-insensitive comparison
        /// </summary>
        public static bool IsNotIn(this string value, params string[] values)
        {
            return value.IsNotIn(true, values);
        }
        #endregion

        #region ' Is(Not)Between functions
        public static bool IsBetween(this int value, int start, int finish, bool excludeBoundaries = false)
        {
            if (start > finish)
            {
                int tmp = start;
                start = finish;
                finish = tmp;
            }
            return excludeBoundaries
                ? value > start && value < finish
                : value >= start && value <= finish;
        }
        public static bool IsBetween(this float value, float start, float finish, bool excludeBoundaries = false)
        {
            if (start > finish)
            {
                float tmp = start;
                start = finish;
                finish = tmp;
            }
            return excludeBoundaries
                ? value > start && value < finish
                : value >= start && value <= finish;
        }
        public static bool IsBetween(this double value, double start, double finish, bool excludeBoundaries = false)
        {
            if (start > finish)
            {
                double tmp = start;
                start = finish;
                finish = tmp;
            }
            return excludeBoundaries
                ? value > start && value < finish
                : value >= start && value <= finish;
        }
        public static bool IsBetween(this DateTime value, DateTime start, DateTime finish, bool excludeBoundaries = false)
        {
            if (start > finish)
            {
                DateTime tmp = start;
                start = finish;
                finish = tmp;
            }
            return excludeBoundaries
                ? value > start && value < finish
                : value >= start && value <= finish;
        }

        public static bool IsNotBetween(this int value, int start, int finish, bool excludeBoundaries = false)
        {
            if (start > finish)
            {
                int tmp = start;
                start = finish;
                finish = tmp;
            }
            return excludeBoundaries
                ? value <= start || value >= finish
                : value < start || value > finish;
        }
        public static bool IsNotBetween(this float value, float start, float finish, bool excludeBoundaries = false)
        {
            if (start > finish)
            {
                float tmp = start;
                start = finish;
                finish = tmp;
            }
            return excludeBoundaries
                ? value <= start || value >= finish
                : value < start || value > finish;
        }
        public static bool IsNotBetween(this double value, double start, double finish, bool excludeBoundaries = false)
        {
            if (start > finish)
            {
                double tmp = start;
                start = finish;
                finish = tmp;
            }
            return excludeBoundaries
                ? value <= start || value >= finish
                : value < start || value > finish;
        }
        public static bool IsNotBetween(this DateTime value, DateTime start, DateTime finish, bool excludeBoundaries = false)
        {
            if (start > finish)
            {
                DateTime tmp = start;
                start = finish;
                finish = tmp;
            }
            return excludeBoundaries
                ? value <= start || value >= finish
                : value < start || value > finish;
        }
        #endregion

        #region ' String functions
        public static string ToProperCase(this string value, bool caseLastName = false)
        {
            string result;
            if (value.IsNotEmpty())
            {
                var builder = new StringBuilder();
                string[] tokens = value.Split('\'', '-', ' ');
                TextInfo info = CultureInfo.CurrentCulture.TextInfo;
                if (caseLastName)
                    foreach (string token in tokens)
                    {
                        if (token.Length > 2 && token.Substring(0, 2).IsSameAs("mc", true))
                            builder.Append(info.ToTitleCase(token.Substring(0, 2)) + info.ToTitleCase(token.Substring(2)));
                        else
                            builder.Append(info.ToTitleCase(token));
                        int resLength = builder.Length;
                        if (resLength < value.Length)
                            builder.Append(value.Substring(resLength, 1));
                    }
                else
                    foreach (string token in tokens)
                    {
                        builder.Append(info.ToTitleCase(token));
                        int resLength = builder.Length;
                        if (resLength < value.Length)
                            builder.Append(value.Substring(resLength, 1));
                    }
                result = builder.ToString();
            }
            else
                result = value;
            return result;
        }

        public static string ToFirstUpper(this string value)
        {
            return value.IsNotEmpty() ? value.Substring(0, 1).ToUpper() + value.Substring(1) : value;
        }

        public static string ToFirstLower(this string value)
        {
            return value.IsNotEmpty() ? value.Substring(0, 1).ToLower() + value.Substring(1) : value;
        }

        public static string Left(this string value, int length)
        {
            return value != null ? value.Substring(0, Math.Min(length < 0 ? 0 : length, value.Length)) : null;
        }

        public static bool Contains(this string value, string searchFor, bool ignoreCase)
        {
            return value != null
                ? ignoreCase ? value.IndexOf(searchFor, StringComparison.CurrentCultureIgnoreCase) > -1 : value.Contains(searchFor)
                : false;
        }

        public static string Replicate(this string value, int number)
        {
            string result = null;
            if (number > 0 && !string.IsNullOrEmpty(value))
            {
                var builder = new StringBuilder(number);
                for (int i = 0; i < number; i++)
                    builder.Append(value);
                result = builder.ToString();
            }
            else if (number == 0 || value == "")
                result = "";
            return result;
        }
        public static string Replicate(this char value, int number)
        {
            return value.ToString().Replicate(number);
        }

        public static string GetPlural(this string value)
        {
            string result;
            if (value.IsNotEmpty())
            {
                if (_reHissing.IsMatch(value))
                    result = "{0}es".Frmt(value);
                else if (_reVowelAndY.IsMatch(value))
                    result = "{0}s".Frmt(value);
                else if (_reEndsWithY.IsMatch(value))
                    result = "{0}ies".Frmt(value.Substring(0, value.Length - 1));
                else if (_reEndsWithIS.IsMatch(value))
                    result = Regex.Replace(value, "is$", "es");
                else if (_reEndsWithIFE.IsMatch(value))
                    result = Regex.Replace(value, "ife$", "ives");
                else if (_reEndsWithLF.IsMatch(value))
                    result = Regex.Replace(value, "lf$", "lves");
                else    //MOST other words are formed like this:
                    result = "{0}s".Frmt(value);
            }
            else
                result = null;
            return result;
        }

        /// <summary>
        ///  adds space(s) to Pascal Case strings - eg: "CandidateName" will become "Candidate Name".
        /// </summary>
        public static string SeparateWords(this string value)
        {
            return value != null ? Regex.Replace(value, "([a-z])([A-Z])", "$1 $2", RegexOptions.Compiled) : null;
        }

        public static string Frmt(this string value, params object[] values)
        {
            return string.Format(value, values);
        }

        public static bool IsSameAs(this string value, string compareTo, bool ignoreCase = false, bool trimValue = false, bool trimCompareTo = false)
        {
            return string.Compare(
                trimValue ? value.NzTrim() : value,
                trimCompareTo ? compareTo.NzTrim() : compareTo,
                ignoreCase
                ) == 0;
        }

        public static bool IsNotSameAs(this string value, string compareTo, bool ignoreCase = false, bool trimValue = false, bool trimCompareTo = false)
        {
            return string.Compare(
                trimValue ? value.NzTrim() : value,
                trimCompareTo ? compareTo.NzTrim() : compareTo,
                ignoreCase
                ) != 0;
        }
        #endregion

        #region ' Nz functions
        public static T Nz<T>(this T? value, T defaultValue = default(T)) where T : struct
        {
            return value.HasValue ? value.Value : defaultValue;
        }
        public static DateTime Nz(this DateTime value, DateTime defaultValue)
        {
            return value.IsNotEmpty() ? value : defaultValue;
        }
        public static string Nz(this string value, string defaultValue = "")
        {
            return value.IsNotEmpty() ? value : defaultValue;
        }

        public static string NzTrim(this string value, string defaultValue = "")
        {
            return (value.IsEmpty() ? defaultValue : value).Trim();
        }
        #endregion

        #region ' Zn functions
        /// <summary>
        /// Returns Value of the <paramref name="value" /> parameter if not null (Nothing in VB), otherwise - <c>DBNull.Value</c>
        /// </summary>
        public static object ZnParameter<T>(this T? value) where T : struct
        {
            return value.HasValue ? (object)value.Value : DBNull.Value;
        }
        /// <summary>
        /// Returns <paramref name="value" /> parameter if it does not Equal <paramref name="defaultValue" />, otherwise - <c>DBNull.Value</c>
        /// </summary>
        public static object ZnParameter<T>(this T? value, T defaultValue = default(T)) where T : struct
        {
            return value.Equals(defaultValue) ? (object)value.Value.Zn(defaultValue) : DBNull.Value;
        }
        /// <summary>
        /// Returns Value of the <paramref name="value" /> parameter if not null (Nothing in VB) and not equal to the defaultValue, otherwise - null (Nothing in VB)
        /// </summary>
        public static T? Zn<T>(this T? value, T defaultValue = default(T)) where T : struct
        {
            return value.HasValue ? value.Value.Zn(defaultValue) : null;
        }
        /// <summary>
        /// Returns <paramref name="value" /> if it does not equal to the <paramref name="defaultValue" />, otherwise - null (Nothing in VB) 
        /// </summary>
        public static T? Zn<T>(this T value, T defaultValue = default(T)) where T : struct
        {
            return !value.Equals(defaultValue) ? new T?(value) : null;
        }
        /// <summary>
        /// Returns <paramref name="value" /> if <paramref name="valueSpecified" /> is true, otherwise - null (Nothing in VB)
        /// </summary>
        public static T? Zn<T>(this T value, bool valueSpecified) where T : struct
        {
            return valueSpecified ? new T?(value) : null;
        }
        public static string Zn(this string value)
        {
            return value.IsNotEmpty() ? value : null;
        }

        public static string ZnString<T>(this T value, T defaultValue = default(T)) where T : struct
        {
            return !value.Equals(defaultValue) ? value.ToString() : null;
        }
        public static string ZnString<T>(this T? value, T defaultValue = default(T)) where T : struct
        {
            return value.HasValue ? value.ZnString(defaultValue) : null;
        }
        public static string ZnString(this bool? value, string trueValue, string falseValue)
        {
            return value.HasValue ? value.Value ? trueValue : falseValue : null;
        }
        #endregion

        #region '	To(Type) string functions
        /// <summary>
        /// Converts string to Boolean value or defaultValue if value cannot be parsed as Boolean
        /// </summary>
        public static bool ToBoolean(this string value, bool defaultValue = false)
        {
            bool result;
            return bool.TryParse(value, out result) ? result : defaultValue;
        }
        /// <summary>
        /// Converts string to Byte value or defaultValue if value cannot be parsed as Byte
        /// </summary>
        public static byte ToByte(this string value, byte defaultValue = 0)
        {
            byte result;
            return byte.TryParse(value, out result) ? result : defaultValue;
        }
        /// <summary>
        /// Converts string to Int16 value or defaultValue if value cannot be parsed as Int16
        /// </summary>
        public static short ToInt16(this string value, short defaultValue = 0)
        {
            short result;
            return short.TryParse(value, out result) ? result : defaultValue;
        }
        /// <summary>
        /// Converts string to Int32 value or defaultValue if value cannot be parsed as Int32
        /// </summary>
        public static int ToInt32(this string value, int defaultValue = 0)
        {
            int result;
            return int.TryParse(value, out result) ? result : defaultValue;
        }
        /// <summary>
        /// Converts string to Int64 value or defaultValue if value cannot be parsed as Int64
        /// </summary>
        public static long ToInt64(this string value, long defaultValue = 0)
        {
            long result;
            return long.TryParse(value, out result) ? result : defaultValue;
        }
        /// <summary>
        /// Converts string to Single value or defaultValue if value cannot be parsed as Single
        /// </summary>
        public static float ToSingle(this string value, float defaultValue = 0)
        {
            float result;
            return float.TryParse(value, NumberStyles.AllowExponent | NumberStyles.Number | NumberStyles.Currency, null, out result) ? result : defaultValue;
        }
        /// <summary>
        /// Converts string to Double value or defaultValue if value cannot be parsed as Double
        /// </summary>
        public static double ToDouble(this string value, double defaultValue = 0)
        {
            double result;
            return double.TryParse(value, NumberStyles.AllowExponent | NumberStyles.Number | NumberStyles.Currency, null, out result) ? result : defaultValue;
        }
        /// <summary>
        /// Converts string to Decimal value or defaultValue if value cannot be parsed as Decimal
        /// </summary>
        public static decimal ToDecimal(this string value, decimal defaultValue = 0)
        {
            decimal result;
            return decimal.TryParse(value, NumberStyles.AllowExponent | NumberStyles.Number | NumberStyles.Currency, null, out result) ? result : defaultValue;
        }
        /// <summary>
        /// Converts string to Date value or Date.MinValue if value cannot be parsed as Date
        /// </summary>
        public static DateTime ToDate(this string value, string parseFormat = null)
        {
            DateTime result;
            return parseFormat.IsEmpty()
                ? (DateTime.TryParse(value, out result) ? result : DateTime.MinValue)
                : DateTime.TryParseExact(value, parseFormat, null, DateTimeStyles.None, out result) ? result : DateTime.MinValue;
        }
        #endregion

        #region '	ToN(Type) string functions
        /// <summary>
        /// Converts string to Boolean? value or defaultValue if value cannot be parsed as Boolean
        /// </summary>
        public static bool? ToNBoolean(this string value, bool? defaultValue = null)
        {
            bool result;
            return bool.TryParse(value, out result) ? result : defaultValue;
        }
        /// <summary>
        /// Converts string to Byte? value or defaultValue if value cannot be parsed as Byte
        /// </summary>
        public static byte? ToNByte(this string value, byte? defaultValue = null)
        {
            byte result;
            return byte.TryParse(value, out result) ? result : defaultValue;
        }
        /// <summary>
        /// Converts string to Int16? value or defaultValue if value cannot be parsed as Int16
        /// </summary>
        public static short? ToNInt16(this string value, short? defaultValue = null)
        {
            short result;
            return short.TryParse(value, out result) ? result : defaultValue;
        }
        /// <summary>
        /// Converts string to Int32? value or defaultValue if value cannot be parsed as Int32
        /// </summary>
        public static int? ToNInt32(this string value, int? defaultValue = null)
        {
            int result;
            return int.TryParse(value, out result) ? result : defaultValue;
        }
        /// <summary>
        /// Converts string to Int64? value or defaultValue if value cannot be parsed as Int64
        /// </summary>
        public static long? ToNInt64(this string value, long? defaultValue = null)
        {
            long result;
            return long.TryParse(value, out result) ? result : defaultValue;
        }
        /// <summary>
        /// Converts string to Single? value or defaultValue if value cannot be parsed as Single
        /// </summary>
        public static float? ToNSingle(this string value, float? defaultValue = null)
        {
            float result;
            return float.TryParse(value, NumberStyles.AllowExponent | NumberStyles.Number | NumberStyles.Currency, null, out result) ? result : defaultValue;
        }
        /// <summary>
        /// Converts string to Double? value or defaultValue if value cannot be parsed as Double
        /// </summary>
        public static double? ToNDouble(this string value, double? defaultValue = null)
        {
            double result;
            return double.TryParse(value, NumberStyles.AllowExponent | NumberStyles.Number | NumberStyles.Currency, null, out result) ? result : defaultValue;
        }
        /// <summary>
        /// Converts string to Decimal? value or defaultValue if value cannot be parsed as Decimal
        /// </summary>
        public static decimal? ToNDecimal(this string value, decimal? defaultValue = null)
        {
            decimal result;
            return decimal.TryParse(value, NumberStyles.AllowExponent | NumberStyles.Number | NumberStyles.Currency, null, out result) ? result : defaultValue;
        }
        /// <summary>
        /// Converts string to Date? value or defaultValue if value cannot be parsed as Date
        /// </summary>
        public static DateTime? ToNDate(this string value, string parseFormat = null, DateTime? defaultValue = null)
        {
            DateTime result;
            return parseFormat.IsEmpty()
                ? (DateTime.TryParse(value, out result) ? result : defaultValue)
                : DateTime.TryParseExact(value, parseFormat, null, DateTimeStyles.None, out result) ? result : defaultValue;
        }
        #endregion

        public static DateTime WithTime(this DateTime value, TimeSpan? time)
        {
            return time.HasValue ? value.Date.Add(time.Value) : value;
        }

        public static DateTime FirstOfTheMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }

        public static DateTime LastOfTheMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, DateTime.DaysInMonth(value.Year, value.Month));
        }

        public static IEnumerable<int> ToEnumInt32(this string value, char separator = ',', bool distinctValues = true)
        {
            IEnumerable<int> result;
            if (value.IsNotEmpty())
            {
                result = value.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries).Select(v => v.ToInt32());
                if (distinctValues)
                    result = result.Distinct();
            }
            else
                result = new List<int> { };
            return result;
        }

        public static bool None<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate = null)
        {
            return predicate != null
                ? !source.Any(predicate)
                : !source.Any();
        }

        public static int SafeCount<T>(this IEnumerable<T> source)
        {
            return source != null ? source.Count() : 0;
        }

        #region ' Encryption
        private static readonly byte[] _key = { 181, 222, 92, 49, 176, 89, 28, 226, 196, 64, 24, 163, 118, 217, 169, 151 };
        private static readonly byte[] _iv = { 184, 177, 211, 145, 66, 234, 173, 167 };

        private static byte[] getKey(string key)
        {
            if (key == null || key.Length < 8)
            {
                Assembly asm = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
                key = "{0}{1}12345678".Frmt(key, General.GetCompanyName(asm)).Substring(0, 8);
            }
            byte[] arr = Encoding.Unicode.GetBytes(key);
            return arr.Take(Math.Min(_key.Length, arr.Length)).ToArray();
        }

        public static string Encrypt(this string value, string key)
        {
            ICryptoTransform encryptor = new RC2CryptoServiceProvider().CreateEncryptor(getKey(key), _iv);
            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                byte[] to = Encoding.Unicode.GetBytes(value);
                cs.Write(to, 0, to.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }

        }

        public static string Decrypt(this string value, string key)
        {
            byte[] encrypted = Convert.FromBase64String(value);
            ICryptoTransform decryptor = new RC2CryptoServiceProvider().CreateDecryptor(getKey(key), _iv);
            using (var ms = new MemoryStream(encrypted))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            {
                const int max_LENGTH = 5000;
                var from = new byte[max_LENGTH];
                int count = cs.Read(from, 0, max_LENGTH) / 2;
                return Encoding.Unicode.GetString(from).Substring(0, count);
            }
        }
        #endregion

        #region ' Time

        private static readonly string _ZERO_WIDTH_SPACE = ((char)8203).ToString();

        public static string ToHoursString(this decimal? value, bool encodeColon = false, bool insertZeroWidthSpace = false)
        {
            var result = "";
            if (value.HasValue)
            {
                return ToHoursString(value.Value, encodeColon, insertZeroWidthSpace);
            }
            return result;
        }

        public static string ToHoursString(this decimal value, bool encodeColon = false, bool insertZeroWidthSpace = false)
        {
            var result = "00:00";
            if (value > 0)
            {
                int hours = (int)value;
                int minutes = (int)Math.Round((value - hours) * 60);
                if (minutes == 60)
                {
                    hours++;
                    minutes = 0;
                }
                if (minutes > 0)
                {
                    result = string.Format("{0:00}:{1:00}", hours, minutes);
                }
                else
                {
                    result = string.Format("{0:00}:00", hours);
                }
            }

            return (insertZeroWidthSpace ? _ZERO_WIDTH_SPACE : "")  //	to right-align the Html output - see frontend.js - tooltip initialtion
                 + (encodeColon ? result.Replace(":", "&#58;") : result);
        }

        public static string ToHoursString(this double? value, bool encodeColon = false, bool insertZeroWidthSpace = false)
        {
            return ToHoursString((decimal?)value, encodeColon, insertZeroWidthSpace);
        }

        public static string ToHoursString(this double value, bool encodeColon = false, bool insertZeroWidthSpace = false)
        {
            return ToHoursString((decimal)value, encodeColon, insertZeroWidthSpace);
        }

        public static decimal HoursToDecimal(this string value)
        {
            decimal result = 0;
            if (value.IsNotEmpty() && value.Contains(":"))
            {
                var valueParts = value.Split(':');
                var hours = int.Parse(valueParts[0]);
                decimal minutes = Math.Round((decimal)(int.Parse(valueParts[1]) / 60.0));
                if (minutes > 0)
                {
                    result = hours + minutes;
                }
                else
                {
                    result = (decimal)hours;
                }
            }
            if (result == 0)
            {
                decimal.TryParse(value, out result);
            }
            return result;
        }

        #endregion
    }
}
