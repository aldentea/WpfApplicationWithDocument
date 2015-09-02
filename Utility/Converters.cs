using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Data;

namespace Aldentea.Wpf.Utility
{
	// 06/02/2014 by aldentea : MutusBaseからWpfUtilityに移動．

	// IntroMutusで使うのに作成したconverterのうち，汎用っぽいものをここで列挙．

	// 06/02/2014 by aldentea : doubleだけでなくintにも対応．
	// 09/14/2011 by aldentea
	#region FractionConverterクラス
	public class FractionConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (values.Length > 1)
			{
				if (values[0] is double && values[1] is double)
				{
					return string.Format("{0:F0} / {1:F0}", values[0], values[1]);
				}
				else if (values[0] is int && values[1] is int)
				{
					return string.Format("{0} / {1}", values[0], values[1]);
				}
			}
			// ※とりあえず手抜き．
			return "--- / ---";
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	#endregion

	// 09/14/2011 by aldentea
	#region TimeSpanDoubleConverterクラス
	public class TimeSpanDoubleConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			TimeSpan timeSpan = (TimeSpan)value;
			return timeSpan.TotalSeconds;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			double seconds = (double)value;
			return TimeSpan.FromSeconds(seconds);
		}
	}
	#endregion

	// 01/07/2014 by aldentea
	#region SwitchConverterクラス
	public class SwitchConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is bool)
			{
				if (parameter is System.Collections.IList)
				{
					var values = (System.Collections.IList)parameter;
					if (values.Count >= 2)
					{
						return (bool)value ? values[1] : values[0];
					}
					else
					{
						return values[0];
					}
				}
				else
				{
					return parameter;
				}
			}
			else
			{
				throw new ArgumentException("valueはbool型でなければなりません．", "value");
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	#endregion

	#region NotNullConverterクラス
	public class NotNullConverter : IValueConverter
	{
		public object Convert(object value, Type type, object parameter, System.Globalization.CultureInfo cultureInfo)
		{
			Type source_type = Type.GetType((string)parameter ?? string.Empty);
			return source_type == null ? value != null : source_type.IsAssignableFrom(value.GetType());
		}

		public object ConvertBack(object value, Type type, object parameter, System.Globalization.CultureInfo cultureInfo)
		{
			throw new NotImplementedException();
		}

	}
	#endregion

	#region NotNullOrEmptyConverterクラス
	public class NotNullOrEmptyConverter : IValueConverter
	{
		public object Convert(object value, Type type, object parameter, System.Globalization.CultureInfo cultureInfo)
		{
			string source = (string)value;
			return !string.IsNullOrEmpty(source.Trim());
		}

		public object ConvertBack(object value, Type type, object parameter, System.Globalization.CultureInfo cultureInfo)
		{
			throw new NotImplementedException();
		}
	}
	#endregion

	// 09/30/2014 by aldentea : MutusDataからここに移動．
	#region RepeatConverterクラス
	public class RepeatConverter : IValueConverter
	{
		// パラメータに与えた文字をvalueの回数だけ表示します．
		// valueはint，parameterはcharかstringであることを要求します．

		public object Convert(object value, Type type, object parameter, System.Globalization.CultureInfo cultureInfo)
		{
			int count = System.Convert.ToInt32(value);
			string ret = string.Empty;
			char mark = parameter is char ? (char)parameter : (parameter is string && !string.IsNullOrEmpty((string)parameter) ? ((string)parameter)[0] : '○');
			for (int i = 0; i < count; i++)
			{
				ret += mark;
			}
			return ret;
		}

		public object ConvertBack(object value, Type type, object parameter, System.Globalization.CultureInfo cultureInfo)
		{
			throw new NotImplementedException();
		}
	}
	#endregion

}
