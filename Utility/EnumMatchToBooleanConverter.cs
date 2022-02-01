using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Data;


// 06/02/2014 by aldentea : さらにAldentea.Wpf.Utility名前空間に移動．
// 06/02/2014 by aldentea : Document名前空間からAldentea.Wpf.Application名前空間に変更．
namespace Aldentea.Wpf.Utility
{

	#region EnumMatchToBooleanConverterクラス
	public class EnumMatchToBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
															object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null || parameter == null)
				return false;

			string? checkValue = value.ToString();
			string? targetValue = parameter.ToString();
			return checkValue != null && checkValue.Equals(targetValue,
							 StringComparison.InvariantCultureIgnoreCase);
		}

		public object? ConvertBack(object value, Type targetType,
															object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null || parameter == null)
				return null;

			bool useValue = (bool)value;
			string? targetValue = parameter.ToString();
			if (useValue && targetValue != null)
				return Enum.Parse(targetType, targetValue);

			return null;
		}

	}
	#endregion

}
