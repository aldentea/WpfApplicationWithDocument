using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

using Aldentea.Wpf.Document;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace Aldentea.Wpf.Application
{

	#region WindowWithDocumentクラス
	public class WindowWithDocument : Window
	{

		// (4.0.0)async化。
		#region *コンストラクタ(WindowWithDocument)
		/// <summary>
		/// NOSSコマンドバインディングを設定し，Closing時に保存を確認するイベントを設定しています．
		/// </summary>
		public WindowWithDocument()
			: base()
		{
			// DataContextの設定．
			this.DataContext = NewDocument;

			this.Closing += async (sender, e) =>
			{
				e.Cancel = !await CloseDocument();
			};

			// コマンドバインディングの設定．
			this.CommandBindings.Add(
				new CommandBinding(System.Windows.Input.ApplicationCommands.New, New_Executed)
			);
			this.CommandBindings.Add(
				new CommandBinding(System.Windows.Input.ApplicationCommands.Open, Open_Executed)
			);
			this.CommandBindings.Add(
				new CommandBinding(System.Windows.Input.ApplicationCommands.Save, Save_Executed, Save_CanExecute)
			);
			this.CommandBindings.Add(
				new CommandBinding(System.Windows.Input.ApplicationCommands.SaveAs, SaveAs_Executed)
			);

		}
		#endregion


		// 06/18/2014 by aldentea : NewDocumentを実装．
		#region *Documentプロパティ

		public DocumentBase NewDocument
		{
			get
			{
				return Application.Current.Document;
				//return this.DataContext as DocumentBase;
			}
		}

		// 旧実装．
		/// <summary>
		/// DataContextをDocumentBaseにキャストしたものを返します．
		/// 設定にはDataContextプロパティを使って下さい．
		/// </summary>
		public DocumentBase Document
		{
			get
			{
				return this.DataContext as DocumentBase;
			}
		}
		#endregion

		#region *[dependency]OpenFileDialogFilterプロパティ

		/// <summary>
		/// ドキュメントを開くダイアログのFilterを取得／設定します．
		/// </summary>
		public string OpenFileDialogFilter
		{
			get
			{
				return (string)GetValue(OpenFileDialogFilterProperty);
			}
			set
			{
				SetValue(OpenFileDialogFilterProperty, value);
			}
		}

		public static readonly DependencyProperty SaveFileDialogFilterProperty
				= DependencyProperty.Register(
						"SaveFileDialogFilter",
						typeof(string),
						typeof(WindowWithDocument),
						new PropertyMetadata("すべてのファイル(*.*)|*.*")
					);
		#endregion

		#region *[dependency]SaveFileDialogFilterプロパティ
		/// <summary>
		/// ドキュメントを保存するダイアログのFilterを取得／設定します．
		/// </summary>
		public string SaveFileDialogFilter
		{
			get
			{
				return (string)GetValue(SaveFileDialogFilterProperty);
			}
			set
			{
				SetValue(SaveFileDialogFilterProperty, value);
			}
		}

		public static readonly DependencyProperty OpenFileDialogFilterProperty 
				= DependencyProperty.Register(
						"OpenFileDialogFilter",
						typeof(string),
						typeof(WindowWithDocument),
						new PropertyMetadata("すべてのファイル(*.*)|*.*")
					);
		#endregion


		#region ドキュメント操作関連

		// ※保存系の返り値の仕様ってどうよ？

		// ※とりあえずこのあたりのレイヤーで例外処理を行っておく．

		// (4.0.0)async化。
		// 03/19/2012 by aldentea : IOExceptionのハンドルを追加．
		#region *ドキュメントを保存(SaveDocument)
		/// <summary>
		/// ドキュメントを保存します．ファイル名がついていなければ，ファイルダイアログでファイル名を指定します．
		/// 結果的にModifiedフラグが消えたならばtrueを，Modifiedフラグが残っているならばfalseを返します．
		/// </summary>
		/// <returns></returns>
		private async Task<bool> SaveDocument()
		{
			try
			{
				switch (await Document.Save())
				{
					case SaveResult.RequireSaveAs:
						// try SaveAs
						return await SaveAsDocument();
					case SaveResult.Succeed:
						//AddToFileHistory(document.FileName);
						return true;
					case SaveResult.NotModified:
					default:
						return true;

				}
			}
			// こんなところでcatchするの？
			catch (System.IO.IOException ex)
			{
				MessageBox.Show("保存に失敗しました．\n" + ex.Message, "保存失敗", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}

		}
		#endregion

		// (4.0.0)async化。
		// 03/19/2012 by aldentea : IOExceptionのハンドルを追加．※SaveDocumentメソッドからのコピペ．
		#region *ドキュメントに名前をつけて保存(SaveAsDocument)
		/// <summary>
		/// ドキュメントに名前をつけて保存します．
		/// 結果的にModifiedフラグが消えたならばtrueを，Modifiedフラグが残っているならばfalseを返します．
		/// </summary>
		/// <returns></returns>
		private async Task<bool> SaveAsDocument()
		{
			// try SaveAs
			Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog { Filter = this.SaveFileDialogFilter };
			if (dialog.ShowDialog() == true)
			{
				try
				{
					var result = await Document.SaveAs(dialog.FileName);
					if (result == SaveResult.Succeed)
					{
						//AddToFileHistory(document.FileName);
					}
				}

				// ↓SaveDocumentメソッドからのコピペ．
				// こんなところでcatchするの？
				catch (System.IO.IOException ex)
				{
					MessageBox.Show("保存に失敗しました．\n" + ex.Message, "保存失敗", MessageBoxButton.OK, MessageBoxImage.Error);
					return false;
				}

				// anyway, Document.IsModified should be false.
				return true;
			}
			// canceled
			return false;
		}
		#endregion

		// (4.0.0)async化。
		#region *ドキュメントをクローズ(CloseDocument)
		/// <summary>
		/// ドキュメントを閉じます．必要に応じて保存確認のメッセージボックスが表示されます．
		/// 
		/// </summary>
		/// <returns></returns>
		protected async Task<bool> CloseDocument()
		{
			if (await ConfirmToClose())
			{
				Document.Close();
				return true;
			}
			else
			{
				return false;
			}
		}
		#endregion

		// (4.0.0)async化。
		// 07/10/2012 by aldentea : CloseDocumentメソッドから分離．
		#region *閉じる前に確認する(ConfirmToClose)
		protected async Task<bool> ConfirmToClose()
		{
			bool ready = !Document.IsModified;

			if (Document.IsModified)
			{
				switch (MessageBox.Show("保存しますか？", "保存確認", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation))
				{
					case MessageBoxResult.Yes:
						return await SaveDocument();
					case MessageBoxResult.No:
						return true;
					default:
						return false;
				}
			}
			else
			{
				return true;
			}
		}
		#endregion


		// (4.0.0)async化。
		// 01/14/2014 by aldentea : ファイルをReadOnlyで開けるように変更．
		// 07/10/2012 by aldentea : 開くファイル名の選択時にキャンセルされた場合は，現在のドキュメントを閉じないように変更．
		// 03/19/2012 by aldentea : 返値をboolからOpenDocumentResult列挙体に変更．
		// 03/19/2012 by aldentea : ↓やっぱりprotectedに変更．OpenCommandでは不都合な場合がある(ショートカットメニューから開くときとか)．
		// 03/16/2012 by aldentea : private化．継承先からの呼び出しには，OpenCommandを実行するようにしましょう．
		#region *ドキュメントをオープン(OpenDocument)
		/*
				/// <summary>
				/// ファイルダイアログを表示し，選択されたファイルのドキュメントを開きます．
				/// 実際にファイルを開いた場合はtrueが，そうでない場合はfalseが返ります．
				/// このメソッド内では例外処理を行っていませんので，呼び出し側でFileNotFoundExceptionなどの処理をしましょう．
				/// </summary>
				/// <returns></returns>
				protected OpenDocumentResult OpenDocument()
				{
					return OpenDocument(string.Empty);
				}
		*/
		/// <summary>
		/// 指定されたファイルのドキュメントを開きます．
		/// 空文字列が渡された場合は，ファイルダイアログが表示されます．
		/// 実際にファイルを開いた場合はtrueが，そうでない場合はfalseが返ります．
		/// このメソッド内では例外処理を行っていませんので，呼び出し側でFileNotFoundExceptionなどの処理をしましょう．
		/// </summary>
		/// <param name="fileName">開くファイル名を与えます．null(またはstring.Empty)を与えると，ファイルダイアログを表示します．</param>
		/// <param name="showReadOnly">trueの場合，ファイルダイアログを表示するときに，「読み取り専用」のチェックボックスを表示します．
		/// ファイル名を指定したときは何の効果もありません．</param>
		/// <param name="checkReadOnly">trueの場合，ファイルダイアログを表示するときに，デフォルトで「読み取り専用」を選択します．
		/// ファイル名を指定したときは，「読み取り専用」として開きます．</param>
		/// <returns></returns>
		protected async Task<OpenDocumentResult> OpenDocument(string fileName = null, bool showReadOnly = false, bool checkReadOnly = false)
		{
			// 1.現在のドキュメントを閉じていいか確認． 
			if (!await ConfirmToClose())
			{
				return OpenDocumentResult.Canceled;
			}

			bool is_read_only = false;

			// 2.開くファイル名を取得．
			if (string.IsNullOrEmpty(fileName))
			{
				var dialog = new Microsoft.Win32.OpenFileDialog
													{
														Filter = this.OpenFileDialogFilter,
														ShowReadOnly = showReadOnly,
														ReadOnlyChecked = checkReadOnly
													};
				// よくわからないが，ShowReadOnlyをtrueにしてもダイアログの外観に変化がない．
				if (dialog.ShowDialog() == true)
				{
					fileName = dialog.FileName;
					is_read_only = dialog.ReadOnlyChecked;
				}
				else
				{
					return OpenDocumentResult.Canceled;
				}
			}
			else
			{
				is_read_only = checkReadOnly;
			}

			// 3.現在のドキュメントをクローズ．
			Document.Close();

			// 4.ファイルオープン．
			while (true)
			{
				try
				{
					await Document.Open(fileName, is_read_only);
					return OpenDocumentResult.Opened;
				}
				catch (System.IO.IOException ex)
				{
					var message = ex.Message + "\nリトライしますか？";
					switch (MessageBox.Show(message, "IOException", MessageBoxButton.YesNo, MessageBoxImage.Error))
					{
						case MessageBoxResult.Yes:
							break;
						case MessageBoxResult.No:
							return OpenDocumentResult.Failed;
					}
				}
				catch (Document.DocumentFormatException ex)
				{
					MessageBox.Show("フォーマットが不適です．\n" + ex.Message, "フォーマットエラー", MessageBoxButton.OK, MessageBoxImage.Error);
					Document.Close();
					return OpenDocumentResult.Failed;
				}
			}
		}
		#endregion

		// 03/19/2012 by aldentea
		#region OpenDocumentResult列挙体
		public enum OpenDocumentResult
		{
			/// <summary>
			/// ファイルオープンに成功しました．
			/// </summary>
			Opened,
			/// <summary>
			/// ファイルオープンがキャンセルされました．
			/// </summary>
			Canceled,
			/// <summary>
			/// ファイルオープンに失敗しました．
			/// </summary>
			Failed
		}
		#endregion

		#endregion

		// 06/21/2011 by aldentea
		public static void ShowFileNotFoundMessageBox(string fileName)
		{
			var msg = string.Format("以下のファイルが見つかりませんでした．ファイル名を確認してください．\n{0}", fileName);
			MessageBox.Show(msg, "ファイルが見つかりませんでした", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		// 06/22/2014 by aldentea
		#region *保存前に確認する(ConfirmToSave)
		/// <summary>
		/// 保存するにあたっての注意事項を確認します。
		/// </summary>
		/// <returns>注意事項を承諾するか否か。falseが返った場合は、処理を中止します。</returns>
		protected virtual bool ConfirmToSave()
		{
			return true;
		}
		#endregion

		// 01/14/2014 by aldentea : ハンドラの名前を変更(新しい命名規約に従い，ハンドラ名から"Command"を削除)．
		#region コマンドハンドラ

		// ↓いつの間にかそうなっていた。
		// というか，外から直接OpenDocumentメソッドなどを呼び出すのではなく，
		// コマンドハンドラを通してのみ処理を実行するようにしますか？

		// (4.0.0)async化。
		protected async void New_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			await CloseDocument();
		}

		// (4.0.0)async化。
		// 01/14/2014 by aldentea : パラメータとして，stringの他にOpenDocumentParameterをとれるように変更．
		// 03/19/2012 by aldentea : 例外処理をOpenDocumentメソッドに移動．
		// 06/21/2011 by aldentea : FileNotFoundExceptionの処理を追加．
		protected async void Open_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (e.Parameter is OpenDocumentParameter)
			{
				var parameter = (OpenDocumentParameter)e.Parameter;
				await OpenDocument(
					fileName: parameter.FileName,
					showReadOnly: parameter.EnableReadOnly,
					checkReadOnly: parameter.IsReadOnly);
			}
			else
			{
				await OpenDocument(e.Parameter as string);
			}
		}

		// (4.0.0)async化。
		// 06/22/2014 by aldentea : ConfirmToSaveを追加。
		protected async void Save_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (ConfirmToSave())
			{
				await SaveDocument();
			}
		}

		// (4.0.0)async化。
		// 06/22/2014 by aldentea : ConfirmToSaveを追加。
		protected async void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (ConfirmToSave())
			{
				await SaveAsDocument();
			}
		}

		protected void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = Document.IsModified;
		}

		/// <summary>
		/// 汎用のCanExecuteハンドラ．
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void Always_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		#endregion

	}
	#endregion

}
