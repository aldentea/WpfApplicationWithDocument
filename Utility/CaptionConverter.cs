using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Data;

namespace Aldentea.Wpf.Utility
{
	// 06/02/2014 by aldentea : いろいろ乱立していたが，CaptionConverterクラスに統一．

	// 05/28/2014 by aldentea : (いくつかクラスがあるけど，たぶんこれが現役)readonlyな状態を表示するように変更．
	// 02/09/2012 by aldentea : OBSOLETEを解除(^-^;
	// 02/06/2012 by aldentea : OBSOLETE
	#region CaptionConverterクラス
	public class CaptionConverter : IMultiValueConverter
	{

		#region *ApplicationNameプロパティ
		public string ApplicationName
		{
			get
			{
				return _applicationName;
			}
			set
			{
				_applicationName = value;
			}
		}
		string _applicationName = "てすとあぷりけーしょん";
		#endregion

		// 06/18/2014 by aldentea
		// デザイン時に例外が発生したため，values[0]がstringであることを確認．
		// (values[0]にMS.Internal.NamedObjectが渡されたらしい．)
		// 05/28/2014 by aldentea
		// それまで，valuesは[ファイル名, IsModified]な配列だったが，
		// [ファイル名，IsModified，IsReadOnly]な配列に変更．一応互換性を考慮．
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo cultureInfo)
		{
			if (values.Length > 0 && values[0] is string)
			{
				var fileName = (string)values[0];
				var display_fileName = string.IsNullOrEmpty(fileName) ? "無題" : System.IO.Path.GetFileName(fileName);

				bool is_modified = values.Length > 1 ? (bool)values[1] : false;
				bool is_readonly = values.Length > 2 ? (bool)values[2] : false;

				return string.Format(
							"{1}{0} - {2}",
							display_fileName,
							is_readonly ? "[読み取り専用]" : (is_modified ? "*" : string.Empty),
							ApplicationName);
			}
			else // if values.Length = 1
			{
				return ApplicationName;
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo cultureInfo)
		{
			throw new NotImplementedException();
		}
	}
	#endregion


}
