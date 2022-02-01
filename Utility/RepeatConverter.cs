using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Threading.Tasks;

namespace Aldentea.Wpf.Utility
{
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
