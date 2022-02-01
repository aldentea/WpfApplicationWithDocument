using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Threading.Tasks;

namespace Aldentea.Wpf.Utility
{
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
}
