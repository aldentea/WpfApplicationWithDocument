using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Threading.Tasks;

namespace Aldentea.Wpf.Utility
{
	// (1.1.0)
	#region ProposalConverterクラス
	/// <summary>
	/// 定数をかけるだけのConverterです。
	/// </summary>
	public class ProposalConveter : IValueConverter
	{
		#region IValueConverter メンバー

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			double gain = parameter == null ? 1.0 : System.Convert.ToDouble(parameter);
			return System.Convert.ToDouble(value) * gain;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			double gain = parameter == null ? 1.0 : System.Convert.ToDouble(parameter);
			return System.Convert.ToDouble(value) / gain;
		}

		#endregion
	}
	#endregion
}
