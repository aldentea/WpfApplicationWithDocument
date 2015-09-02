using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Aldentea.Wpf.Application
{
	public class Commands
	{
		// 06/17/2014 by aldentea : もっと汎用的なレイヤーにあるべきコマンド．
		/// <summary>
		/// ファイルを選択します．
		/// </summary>
		public static RoutedCommand SelectFileCommand = new RoutedCommand();

		public static RoutedCommand OutputTextCommand = new RoutedCommand();

		// 09/02/2014 by aldentea : いわゆるAboutダイアログを表示します．
		public static RoutedCommand AboutCommand = new RoutedCommand();

		// 09/17/2014 by aldentea : アプリケーションの設定を行います．
		public static RoutedCommand ConfigCommand = new RoutedCommand();

	}
}
