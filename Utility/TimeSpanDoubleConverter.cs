using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Threading.Tasks;

namespace Aldentea.Wpf.Utility
{
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
}
