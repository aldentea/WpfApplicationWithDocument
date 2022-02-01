using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Threading.Tasks;

namespace Aldentea.Wpf.Utility
{
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
}
