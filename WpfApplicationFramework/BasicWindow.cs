using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using Aldentea.Wpf.Document;

namespace Aldentea.Wpf.Application
{
	// 09/26/2014 by aldentea : 履歴メニューの表示数の上限を設定できるように改良．
	// 06/02/2014 by aldentea : BasicWindowに改名．
	// 09/04/2013 by aldentea
	// (FileHistoryShortcutSeparatorの後ろにある)FileShortcutParentの子のMenuItemで，
	// Tagにファイル名(絶対パス)が設定されているものは全てショートカットメニューと解釈します．
	// 06/21/2011 by aldentea : MySettingsに依存する部分をabstractプロパティにして分離．
	#region BasicWindowクラス

	/// <summary>
	/// ファイル履歴をハンドルするWindowクラスです．
	/// </summary>
	public abstract class BasicWindow : WindowWithDocument
	{


		#region *FileHistoryShortcutSeparatorプロパティ
		/// <summary>
		/// ショートカットメニューの直前のメニュー項目を取得／設定します(この項目の直後にショートカットメニューが入る)．
		/// あるメニュー項目の子要素を全てショートカットメニューにする場合は，nullを設定してください．
		/// </summary>
		public Control FileHistoryShortcutSeparator
		{
			get
			{
				return (Control)GetValue(FileHistoryShortcutSeparatorProperty);
			}
			set
			{

				SetValue(FileHistoryShortcutSeparatorProperty, value);
			}
		}


		public static readonly DependencyProperty FileHistoryShortcutSeparatorProperty
			= DependencyProperty.Register(
					"FileHistoryShortcutSeparator",
					typeof(Control),
					typeof(BasicWindow),
					new PropertyMetadata(
							null,
							(d, e) => { ((BasicWindow)d).FileHistoryShortcutParent = ((Control)e.NewValue).Parent as MenuItem; }
					),
					(value) => { return value == null || ((Control)value).Parent is MenuItem; }
			// ↑value == nullを入れ忘れると，XamlParseExceptionでアプリケーションが立ち上がらなかった．しかもデバッグ困難．
				);

		#endregion

		#region *FileShortcutParentプロパティ
		/// <summary>
		/// ショートカットメニューの親にあたるMenuItemを取得／設定します．
		/// FileHistoryShortcutSeparatorプロパティを設定すると，その後ろにショートカットメニューが挿入されていきます．
		/// FileHistoryShortcutSeparatorプロパティの値がnullであれば，FileShortcutParentの子が全てショートカットメニューであると解釈します．
		/// </summary>
		public MenuItem FileHistoryShortcutParent
		{
			get
			{
				return (MenuItem)GetValue(FileHistoryShortcutParentProperty);
			}
			protected set
			{
				SetValue(FileHistoryShortcutParentPropertyKey, value);
			}
		}

		static readonly DependencyPropertyKey FileHistoryShortcutParentPropertyKey
			= DependencyProperty.RegisterReadOnly("FileHistoryShortcutParent", typeof(MenuItem), typeof(BasicWindow),
					new PropertyMetadata());
		public static readonly DependencyProperty FileHistoryShortcutParentProperty = FileHistoryShortcutParentPropertyKey.DependencyProperty;
		#endregion

		// 08/08/2014 by aldentea : Savedイベントの仕様変更を反映．
		// 08/30/2011 by aldentea : MutusApplicationSampleからLoadedハンドラの追加を移動．
		#region *コンストラクタ(BasicWindow)
		public BasicWindow()
			: base()
		{
			this.Loaded += (sender, e) =>
			{
				Document.Opened += (c_sender, c_e) => { AddToFileHistory(this.Document.FileName); };
				Document.Saved += (c_sender, c_e) => { AddToFileHistory(c_e.FileName); };
				BuildHistoryShortcut();
			};
			this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, Close_Executed, Always_CanExecute));
		}
		#endregion

		// 01/14/2014 by aldentea : 継承先のコンストラクタで(のみ)設定するものなので，あえてプロパティにはしないでおく．
		/// <summary>
		/// trueの場合，ファイル履歴から開いたドキュメントをReadOnlyで開きます．
		/// </summary>
		protected bool openAsReadOnlyFromFileHistory;

		// 01/14/2014 by aldentea : read-onlyで開くことができるように変更．
		// 03/19/2012 by aldentea : ↓やっぱりOpenDocumentメソッドの呼び出しに変更．
		// 03/16/2012 by aldentea : コマンドハンドラの中身をコピペしていた部分を，Executeメソッドの呼び出しに変更．
		// 06/21/2011 by aldentea : MySettingsに依存しない形で分離．
		#region *ファイル履歴のショートカットメニューを生成(GenerateFileHistoryShortcutMenuItem)
		protected MenuItem GenerateFileHistoryShortcutMenuItem(string fileName)
		{
			//var menuItem = new MenuItem
			//{
			//  Header = System.IO.Path.GetFileName(fileName).Replace("_", "__"),
			//  ToolTip = string.Format("{0}を開きます", fileName),
			//  Command = System.Windows.Input.ApplicationCommands.Open,
			//  CommandParameter = fileName
			//};
			// ※本当は↑のようにしたいのだが，メニュー項目全てにキージェスチャ(Crtl+O)が表示されてしまうのが気にくわない．
			// ※仕方がないので，以下の方法を採用している．

			var contextMenu = new ContextMenu();
			var removingMenuItem = new MenuItem();
			removingMenuItem.Header = "履歴から削除する";
			removingMenuItem.Click += (sender, e) => { RemoveFromFileHistory(fileName); };
			contextMenu.Items.Add(removingMenuItem);

			// 開くべきファイル名がTagプロパティに格納されている．
			var menuItem = new MenuItem
			{
				Header = System.IO.Path.GetFileName(fileName).Replace("_", "__"),
				ToolTip = string.Format("{0}を開きます", fileName),
				Tag = fileName,
			};
			menuItem.Click += async (sender, e) =>
			{
				var selectedMenuItem = (MenuItem)sender;

				//System.Windows.Input.ApplicationCommands.Open.Execute((string)selectedMenuItem.Tag, this);
				// 結果を取得する関係で，↑の代わりに↓のコードを採用．
				var fName = (string)selectedMenuItem.Tag;
				if (await OpenDocument(fName, checkReadOnly: this.openAsReadOnlyFromFileHistory) == OpenDocumentResult.Failed)
				{
					RemoveFromFileHistory(fName);
				}
			};
			menuItem.ContextMenu = contextMenu;
			return menuItem;
		}
		#endregion

		/// <summary>
		/// ショートカットメニューの直後に入れられるseparatorのTagプロパティに設定される文字列です．
		/// </summary>
		const string END_SEPARATOR = "fileHistoryShortcutEndSeparator";

		// 09/26/2014 by aldentea : FileHistoryDisplayCountの設定を反映(表示数の上限)．
		// 09/04/2013 by aldentea : fileHistoryShortcutSeparatorがnullの場合に対応．一部バグを修正．
		#region *ショートカットメニューを構築(BuildHistoryShortcut)
		/// <summary>
		/// ファイル履歴ショートカットメニューを構築します．
		/// </summary>
		protected void BuildHistoryShortcut()
		{
			if (FileHistoryShortcutParent != null)
			{
				int n;
				if (FileHistoryShortcutSeparator != null && FileHistoryShortcutSeparator.Parent == FileHistoryShortcutParent)
				{
				// FileHistoryShortcutSeparatorのインデックスに1を加えたもの．
				n = FileHistoryShortcutParent.Items.IndexOf(FileHistoryShortcutSeparator) + 1;
				}
				else {
					n = 0;
				}

				// 既存の履歴を削除．

				// FileHistoryShortcutSeparatorの直後に並んでいる以下のものを削除します．
				// - Tagにフルパスが設定されているもの
				// - END_SEPARATORが設定されているもの
				while (FileHistoryShortcutParent.Items.Count > n)
				{
					var item = FileHistoryShortcutParent.Items[n];
					if (item is Control && ((Control)item).Tag is string)
					{
						var tag = (string)((Control)item).Tag;
						if (tag == END_SEPARATOR || System.IO.Path.IsPathRooted(tag))
							{
							FileHistoryShortcutParent.Items.RemoveAt(n);
							continue;
						}
					}
					break;
				}

				if (Application.Current.FileHistory == null)
				{
					Application.Current.FileHistory = new System.Collections.Specialized.StringCollection();
				}
				// ファイル履歴を挿入．
				else if (Application.Current.FileHistory.Count > 0)
				{
					if (n > 0)
					{
						FileHistoryShortcutParent.Items.Insert(n, new Separator { Tag = END_SEPARATOR });	// 結局ショートカットメニューの直後に来る．
					}
					for (int i = 0; i < Application.Current.FileHistory.Count && i < Application.Current.FileHistoryDisplayCount; i++)
					{
						var menuItem = GenerateFileHistoryShortcutMenuItem(Application.Current.FileHistory[i]);
						FileHistoryShortcutParent.Items.Insert(n++, menuItem);
					}
				}
			}
		}
		#endregion

		#region *ファイル履歴に追加(AddToFileHistory)
		/// <summary>
		/// 指定したファイルを履歴に追加して，ショートカットメニューを再構築します．
		/// </summary>
		/// <param name="fileName"></param>
		protected void AddToFileHistory(string fileName)
		{
			Application.Current.AddToFileHistory(fileName);
			// メニューを再構築．
			BuildHistoryShortcut();
		}
		#endregion

		// 03/19/2012 by aldentea : アイコンを[？]から[！]に変更．
		// 06/23/2011 by aldentea
		#region *ファイル履歴から削除(RemoveFromFileHistory)
		/// <summary>
		/// 確認のメッセージボックスを表示した上で，指定されたファイルの履歴を削除します．
		/// </summary>
		/// <param name="fileName"></param>
		public void RemoveFromFileHistory(string fileName)
		{
			var msg = string.Format("{0}を履歴から削除しますか？", fileName);
			if (MessageBox.Show(msg, "履歴削除の確認", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.Yes)
			{
				Application.Current.FileHistory.Remove(fileName);
				BuildHistoryShortcut();
			}

		}
		#endregion

		#region コマンドハンドラ

		#region Close
		private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			this.Close();
		}
		#endregion

		#endregion

	}
	#endregion

}
