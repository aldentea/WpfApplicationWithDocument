using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Aldentea.Wpf.Application;

namespace Aldentea.Wpf.ApplicationSample
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : BasicWindow
	{

		// 08/08/2014 by aldentea : BasicWindowと重複する処理を削除．
		public MainWindow() : base()
		{

			this.Loaded += (sender, e) =>
			{
				BuildHistoryShortcut();
			};

			
			//FileHistory = MySettings.FileHistory;
			//FileHistoryCount = MySettings.FileHistoryCount;

			InitializeComponent();

			// ※とりあえずコードでの指定にのみ対応．
			FileHistoryShortcutSeparator = fileHistorySeparator;
		}

		private Properties.Settings MySettings
		{
			get { return App.Current.MySettings; }
		}

		// 06/18/2014 by aldentea : empty
		private void WindowWithDocument_Closed(object sender, EventArgs e)
		{
			//MySettings.Save();
		}



	}
}
