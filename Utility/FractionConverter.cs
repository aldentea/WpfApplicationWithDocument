using System;
using System.Windows.Data;

namespace Aldentea.Wpf.Utility
{
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

}

