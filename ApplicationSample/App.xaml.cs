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

	}
}
