using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Aldentea.Wpf.ApplicationSample
{
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App : Aldentea.Wpf.Application.Application
	{
		protected App() : base()
		{
			// ★Documentをここで設定する！
			Document = new Aldentea.Wpf.Document.TextDocument();
			this.Exit += new ExitEventHandler(App_Exit);
		}

		// ※注意！↓
		//アプリケーションで SessionEnding イベントを処理した後にこれをキャンセルすると、
		// Exit が発生せず、アプリケーションはシャットダウン モードに従って実行を続けます。

		void App_Exit(object sender, ExitEventArgs e)
		{
			MySettings.Save();
		}

		// 06/13/2014 by aldentea
		#region *MySettingsプロパティ
		/// <summary>
		/// アプリケーションの設定を取得します．
		/// </summary>
		public Properties.Settings MySettings
		{
			get
			{
				return Aldentea.Wpf.ApplicationSample.Properties.Settings.Default;
			}
		}
		#endregion

		// 06/13/2014 by aldentea : これはその都度実装する必要がありますかねぇ．
		public static App Current
		{
			get
			{
				return System.Windows.Application.Current as App;
			}
		}

		// 08/27/2015 by aldentea : MainWindowからAppに移動．
		#region ファイルショートカットメニュー関連

		// 06/21/2011 by aldentea : プロパティ化．
		#region *[override]FileHistoryプロパティ
		/// <summary>
		/// ファイル履歴を取得します．
		/// </summary>
		public override System.Collections.Specialized.StringCollection FileHistory
		{
			get
			{
				return MySettings.FileHistory;
			}
			set
			{
				MySettings.FileHistory = value;
			}
		}
		#endregion

		#region *[override]FileHistoryCountプロパティ
		public override byte FileHistoryCount
		{
			get { return MySettings.FileHistoryCount; }
		}
		#endregion


		// 08/26/2015 by aldentea
		public override byte FileHistoryDisplayCount
		{
			get { return MySettings.FileHistoryDisplayCount; }
		}
		//private void WindowWithDocumentAndFileHistory_Loaded(object sender, RoutedEventArgs e)
		//{

		//}

		#endregion

	}
}
