using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Linq;
using System.Text;

namespace Dazzler
{
   public class Utility
   {
      #region Bool convertion
      static public bool ToBool(object value)
      {
         if (value is bool b) return b;
         else if (value is string s && s == "1") return true;

         bool.TryParse(Convert.ToString(value), out bool result);
         return result;
      }
      #endregion
      #region Numeric convertion
      static public int ToInt(object value)
      {
         if (value == null || value == DBNull.Value) return 0;
         else if (value is int i) return i;
         else if (value is bool b) return b ? 1 : 0;

         int.TryParse(Convert.ToString(value), System.Globalization.NumberStyles.Any, null, out int result);
         return result;
      }
      static public long ToLong(object value)
      {
         if (value == null || value == DBNull.Value) return 0;
         else if (value is long l) return l;
         else if (value is bool b) return b ? 1 : 0;

         long.TryParse(Convert.ToString(value), System.Globalization.NumberStyles.Any, null, out long result);
         return result;
      }
      static public double ToDouble(object value)
      {
         if (value == null || value == DBNull.Value) return 0;
         else if (value is double d) return d;
         else if (value is bool b) return b ? 1.0D : 0.0D;

         double.TryParse(Convert.ToString(value), System.Globalization.NumberStyles.Any, null, out double result);
         return result;
      }
      static public decimal ToDecimal(object value)
      {
         if (value == null || value == DBNull.Value) return 0.0M;
         else if (value is decimal d) return d;
         else if (value is bool b) return b ? 1.0M : 0.0M;

         decimal.TryParse(Convert.ToString(value), System.Globalization.NumberStyles.Any, null, out decimal result);
         return result;
      }
      #endregion
      #region Text convertion
      static public string ToStr(object value)
      {
         if (value == null || Convert.IsDBNull(value)) return null;
         else return Convert.ToString(value);
      }
      static public string SubStr(string text, int index, int length)
      {
         if (string.IsNullOrEmpty(text)) return null;
         string s = string.Empty;
         int len = text.Length - index;

         if (len >= length) s = text.Substring(index, length);
         else if (len > 0) s = text.Substring(index, text.Length - index);

         return s;
      }
      static public string SubStr(string sourceText, int startIndex)
      {
         return SubStr(sourceText, startIndex, sourceText == null ? 0 : sourceText.Length);
      }

      static public string Left(string text, int length)
      {
         if (string.IsNullOrEmpty(text) || text.Length <= 0) return null;
         if (text.Length > length) return text.Substring(0, length);
         else return text;
      }
      static public string Right(string text, int length)
      {
         if (string.IsNullOrEmpty(text) || text.Length <= 0) return null;
         if (text.Length > length) return text.Substring(text.Length - length, length);
         else return text;
      }
      public static string PadRight(string sourceText, int length, char paddingChar = ' ')
      {
         if (sourceText == null) return null;
         if (sourceText.Length >= length) return sourceText.Substring(0, length);
         else return sourceText.PadRight(length, paddingChar);
      }
      public static string PadLeft(string sourceText, int length, char paddingChar = ' ')
      {
         if (sourceText == null) return null;
         if (sourceText.Length >= length) return sourceText.Substring(0, length);
         else return sourceText.PadLeft(length, paddingChar);
      }
      public static string Trim(string sourceText, params char[] trimChars)
      {
         if (string.IsNullOrEmpty(sourceText)) return null;
         return sourceText.Trim(trimChars);
      }
      public static bool FindText(string source, int startIndex, string beginsWith, string endsWith, out int position, out int length, out string text)
      {
         position = -1; length = 0; text = null;
         if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(beginsWith) || string.IsNullOrEmpty(endsWith)) return false;
         if (startIndex < 0 || startIndex >= source.Length) return false;

         int posHead = source.IndexOf(beginsWith, startIndex);
         if (posHead < 0) return false;

         int posTail = source.IndexOf(endsWith, posHead + beginsWith.Length);
         if (posTail < 0) return false;

         position = posHead;
         length = posTail - posHead + 1;

         int posText = posHead + beginsWith.Length;
         text = source.Substring(posText, posTail - posText);

         return position >= 0;
      }
      #endregion
      #region Date convertion

      static public DateTime ToDate(object value, string format)
      {
         return ToDateTime(value, format).Date;
      }
      static public DateTime ToDateTime(object value, string format)
      {
         if (value is DateTime) return (DateTime)value;
         DateTime.TryParseExact(Convert.ToString(value), format, null, System.Globalization.DateTimeStyles.None, out DateTime result);
         return result;
      }

      static public DateTime ToDate(object value)
      {
         return ToDateTime(value).Date;
      }
      static public DateTime ToDateTime(object value)
      {
         if (value is DateTime) return (DateTime)value;

         DateTime.TryParseExact(Convert.ToString(value)
             , new string[] {
                "G",
                "yyyyMMdd HH:mm:ss",
                "yyyy/M/d HH:mm:ss",
                "yyyy-M-d HH:mm:ss",
                "yyyy.M.d HH:mm:ss",
                "yyyyMMdd",
                "yyyy/M/d",
                "yyyy-M-d",
                "yyyy.M.d"}
             , null, System.Globalization.DateTimeStyles.None, out DateTime result);

         return result;
      }

      static public string NormalizeLongDate(string dateString)
      {
         // it should always return mm/dd/ccyy !!!

         if (dateString == null) return dateString;

         dateString = dateString.Replace("/", "");
         dateString = dateString.Replace("-", "");

         string mm = null, dd = null, cc = null, yy = null;

         if (dateString.Length == 4)
         {
            mm = dateString.Substring(0, 2);
            dd = dateString.Substring(2, 2);
            cc = (DateTime.Today.Year / 100).ToString("00");
            yy = (DateTime.Today.Year % 100).ToString("00");
         }
         else if (dateString.Length == 6)
         {
            mm = dateString.Substring(0, 2);

            if (Convert.ToInt32(mm) > 12)
            {
               // YYMMDD
               yy = dateString.Substring(0, 2);
               mm = dateString.Substring(2, 2);
               dd = dateString.Substring(4, 2);
            }
            else
            {
               // MMDDYY
               dd = dateString.Substring(2, 2);
               yy = dateString.Substring(4, 2);
            }

            #region determine CC
            /*****************************************************
            * 1. If the specified two-digit year is 00 to 49, then
            * a. If the last two digits of the current year are 00 to 49, then the returned year has the same first two digits as the current year.
            * b. If the last two digits of the current year are 50 to 99, then the first 2 digits of the returned year are 1 greater than the first 2 digits of the current year.
            * 
            * 2. If the specified two-digit year is 50 to 99, then
            * a. If the last two digits of the current year are 00 to 49, then the first 2 digits of the returned year are 1 less than the first 2 digits of the current year.
            * b. If the last two digits of the current year are 50 to 99, then the returned year has the same first two digits as the current year.
            ******************************************************/

            int _cc = DateTime.Today.Year / 100;
            int _yy = DateTime.Today.Year % 100;

            if (Convert.ToInt32(yy) < 50) cc = (_yy < 50 ? _cc : _cc + 1).ToString("00");
            else cc = (_yy < 50 ? _cc - 1 : _cc).ToString("00");

            #endregion
         }
         else if (dateString.Length == 8)
         {
            mm = dateString.Substring(0, 2);

            if (Convert.ToInt32(mm) > 12)
            {
               // CCYYMMDD
               cc = dateString.Substring(0, 2);
               yy = dateString.Substring(2, 2);
               mm = dateString.Substring(4, 2);
               dd = dateString.Substring(6, 2);
            }
            else
            {
               // MMDDCCYY
               dd = dateString.Substring(2, 2);
               cc = dateString.Substring(4, 2);
               yy = dateString.Substring(6, 2);
            }
         }
         else
         {
            return dateString;
         }

         dateString = mm + "/" + dd + "/" + cc + yy;
         return dateString;
      }
      static public string NormalizeShortDate(string dateString)
      {
         // it should always return mm/dd/yy !!!

         if (dateString == null) return dateString;

         dateString = dateString.Replace("/", "");
         dateString = dateString.Replace("-", "");

         string mm = null, dd = null, yy = null;

         if (dateString.Length == 4)
         {
            mm = dateString.Substring(0, 2);
            dd = dateString.Substring(2, 2);
            yy = (DateTime.Today.Year % 100).ToString("00");
         }
         else if (dateString.Length == 6)
         {
            mm = dateString.Substring(0, 2);

            if (Convert.ToInt32(mm) > 12)
            {
               // YYMMDD
               yy = dateString.Substring(0, 2);
               mm = dateString.Substring(2, 2);
               dd = dateString.Substring(4, 2);
            }
            else
            {
               // MMDDYY
               dd = dateString.Substring(2, 2);
               yy = dateString.Substring(4, 2);
            }
         }
         else if (dateString.Length == 8)
         {
            mm = dateString.Substring(0, 2);

            if (Convert.ToInt32(mm) > 12)
            {
               // CCYYMMDD
               //cc = dateString.Substring(0, 2);
               yy = dateString.Substring(2, 2);
               mm = dateString.Substring(4, 2);
               dd = dateString.Substring(6, 2);
            }
            else
            {
               // MMDDCCYY
               dd = dateString.Substring(2, 2);
               //cc = dateString.Substring(4, 2);
               yy = dateString.Substring(6, 2);
            }
         }
         else
         {
            return dateString;
         }

         dateString = mm + "/" + dd + "/" + yy;
         return dateString;
      }

      #endregion
      #region Object convertion

      /// <summary>
      /// It converts the value to underlying type.
      /// </summary>
      /// <param name="type"></param>
      /// <param name="value"></param>
      /// <returns></returns>
      static public object To(Type type, object value) => To(type, value, out Type underlyingType, null);

      /// <summary>
      /// It converts the value to underlying type.
      /// </summary>
      /// <param name="type"></param>
      /// <param name="value"></param>
      /// <param name="underlyingType"></param>
      /// <param name="nullValue"></param>
      /// <returns></returns>
      static public object To(Type type, object value, out Type underlyingType, object nullValue = null)
      {
         bool isNullable = false;
         bool isEnum = false;

         // gets inner type if nullable.
         if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
         {
            isNullable = true;
            type = Nullable.GetUnderlyingType(type);
         }
         else if (!type.IsValueType)
         {
            isNullable = true;
         }
         if (type.IsEnum)
         {
            isEnum = true;
            type = Enum.GetUnderlyingType(type);
         }

         underlyingType = type;

         // returns for the nullable value.
         if ((value == null || value == DBNull.Value) && isNullable) return nullValue;


         // converts into target type.
         if (isEnum)
         {
            if (value == null || value == DBNull.Value) value = Convert.ChangeType(0, type);
            else value = Convert.ChangeType(value, type);
         }
         else if (type == typeof(string)) value = Utility.ToStr(value);
         else if (type == typeof(byte) || type == typeof(sbyte)) value = (byte)Utility.ToInt(value);
         else if (type == typeof(Int16) || type == typeof(UInt16)) value = (Int16)Utility.ToInt(value);
         else if (type == typeof(Int32) || type == typeof(UInt32)) value = (Int32)Utility.ToInt(value);
         else if (type == typeof(Int64) || type == typeof(UInt64)) value = Utility.ToLong(value);
         else if (type == typeof(DateTime)) value = Utility.ToDateTime(value);
         else if (type == typeof(Decimal)) value = Utility.ToDecimal(value);
         else if (type == typeof(Double)) value = Utility.ToDouble(value);
         else if (type == typeof(float)) value = (float)Utility.ToDouble(value);
         else if (type == typeof(bool)) value = Utility.ToBool(value);
         else if (type == typeof(Byte[])) value = (value is byte[]? (byte[])value : null);
         else if (type == typeof(Guid))
         {
            if (value == null || value == DBNull.Value) value = Guid.Empty;
            else value = new Guid(Utility.ToStr(value));
         }

         return value;
      }

      /// <summary>
      /// It converts the value from underlying type to desired type.
      /// </summary>
      /// <param name="type"></param>
      /// <param name="value"></param>
      /// <param name="underlyingType"></param>
      /// <param name="nullValue"></param>
      /// <returns></returns>
      static public object From(Type type, object value)
      {
         bool isNullable = false;
         Type underlyingType = type;

         // gets inner type if nullable.
         if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
         {
            isNullable = true;
            underlyingType = Nullable.GetUnderlyingType(type);
         }

         // returns for the nullable value.
         if ((value == null || value == DBNull.Value) && isNullable) return null;


         // converts into target type.
         if (underlyingType.IsEnum)
         {
            if (value == null || value == DBNull.Value) value = Enum.ToObject(underlyingType, 0);
            else value = Enum.ToObject(underlyingType, value);
         }
         else if (type == typeof(string)) value = Utility.ToStr(value);
         else if (type == typeof(byte) || type == typeof(sbyte)) value = (byte)Utility.ToInt(value);
         else if (type == typeof(Int16) || type == typeof(UInt16)) value = (Int16)Utility.ToInt(value);
         else if (type == typeof(Int32) || type == typeof(UInt32)) value = (Int32)Utility.ToInt(value);
         else if (type == typeof(Int64) || type == typeof(UInt64)) value = Utility.ToLong(value);
         else if (type == typeof(DateTime)) value = Utility.ToDateTime(value);
         else if (type == typeof(Decimal)) value = Utility.ToDecimal(value);
         else if (type == typeof(Double)) value = Utility.ToDouble(value);
         else if (type == typeof(float)) value = (float)Utility.ToDouble(value);
         else if (type == typeof(bool)) value = Utility.ToBool(value);
         else if (type == typeof(Byte[])) value = (value is byte[]? (byte[])value : null);
         else if (type == typeof(Guid))
         {
            if (value == null || value == DBNull.Value) value = Guid.Empty;
            else value = new Guid(Utility.ToStr(value));
         }

         return value;
      }

      static public object GetDefault(Type type)
      {
         // return null for any reference types
         if (!type.IsValueType) return null;

         // find base type of nullable value type
         Type basetype = Nullable.GetUnderlyingType(type);
         if (basetype != null) type = basetype;
         return Activator.CreateInstance(type);
      }

      static public void Copy(object source, object dest, bool skipNull)
      {
         foreach (PropertyInfo prop in source.GetType().GetProperties())
         {
            PropertyInfo pi2 = dest.GetType().GetProperty(prop.Name);
            if (pi2 == null) continue;

            object value = prop.GetValue(source, null);
            if (skipNull && value == null) continue;
            pi2.SetValue(dest, value, null);
         }
      }
      static public void Copy(object source, object dest) => Copy(source, dest, false);

      static public T Copy<T>(object source)
      {
         T instance = Activator.CreateInstance<T>();
         Copy(source, instance, false);
         return instance;
      }

      static public string FastSerialize(string title, object source)
      {
         if (title == null && source == null) return null;

         StringBuilder sb = new StringBuilder(title, 1024);
         if (source != null)
         {
            var type = source.GetType();
            sb.Append('\x0').Append(type.FullName);

            // insert values of the properties.
            foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
               sb.Append('\x0').Append(prop.GetValue(source, null));
            }
         }

         return sb.ToString();
      }

      #endregion


      #region database related

      static public IDataAdapter GetAdapter(IDbConnection connection)
      {
         var assembly = connection.GetType().Assembly;
         var @namespace = connection.GetType().Namespace;

         // Assumes the factory is in the same namespace
         var factoryType = assembly.GetTypes()
                             .Where(x => x.Namespace == @namespace)
                             .Where(x => x.IsSubclassOf(typeof(DbProviderFactory)))
                             .Single();

         // SqlClientFactory and OleDbFactory both have an Instance field.
         var instanceFieldInfo = factoryType.GetField("Instance", BindingFlags.Static | BindingFlags.Public);
         var factory = (DbProviderFactory)instanceFieldInfo.GetValue(null);

         return factory.CreateDataAdapter();
      }

      static public int GetErrorCode(DbException e)
      {
         // 'Number' for SqlException and MySqlException, 'Code' for Oracle.
         var pi = GetProperty(e, "Number") ?? GetProperty(e, "Code");
         return ToInt(pi?.GetValue(e) ?? -1);
      }

      #endregion

      #region Property Info

      static public PropertyInfo GetProperty(object instance, string name) => instance.GetType().GetProperty(name);
      static public void SetPropertyValue(object instance, string name, object value) => GetProperty(instance, name)?.SetValue(instance, value);

      static public FieldInfo GetBackingFieldInfo(object instance, string name)
      {
         // <>f__AnonymousType5`2            Anonymous object type name.
         // <FirstName>i__Field              Anonymous backing field name.
         // <FirstName>k__BackingField       Normal class's backing field name.

         const BindingFlags FieldFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase;
         string[] dackingFieldFormats = { "<{0}>i__Field", "<{0}>k__BackingField", "<{0}>" };
         var backingFieldNames = dackingFieldFormats.Select(x => string.Format(x, name)).ToList();
         var fi = instance.GetType()
             .GetFields(FieldFlags)
             .FirstOrDefault(f => backingFieldNames.Contains(f.Name));

         return fi;
      }

      static public bool SetBackiingFieldValue(object instance, string name, object value)
      {
         FieldInfo fi = GetBackingFieldInfo(instance, name);
         if (fi == null) return false;
         fi.SetValue(instance, value);
         return true;
      }

      #endregion
   }
}



