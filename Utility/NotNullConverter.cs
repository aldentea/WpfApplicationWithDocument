using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Threading.Tasks;

namespace Aldentea.Wpf.Utility
{
	#region NotNullConverterクラス
	public class NotNullConverter : IValueConverter
	{
		public object Convert(object value, Type type, object parameter, System.Globalization.CultureInfo cultureInfo)
		{
			Type? source_type = Type.GetType((string)parameter ?? string.Empty);
			return source_type == null ? value != null : source_type.IsAssignableFrom(value.GetType());
		}

		public object ConvertBack(object value, Type type, object parameter, System.Globalization.CultureInfo cultureInfo)
		{
			throw new NotImplementedException();
		}

	}
	#endregion
}
