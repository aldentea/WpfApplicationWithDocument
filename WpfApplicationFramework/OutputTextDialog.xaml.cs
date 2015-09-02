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
//using System.Windows.Shapes;
using System.ComponentModel;
using System.IO;

namespace Aldentea.Wpf.Application
{
	/// <summary>
	/// OutputTextDialog.xaml の相互作用ロジック
	/// </summary>
	public partial class OutputTextDialog : Window
	{
		public OutputTextDialog()
		{
			InitializeComponent();
		}
/*
		public MutusIntroDocument MyDocument
		{
			get
			{
				return this.DataContext as MutusIntroDocument;
			}
		}
		*/
		public OutputTextConfig MyConfig
		{
			get { return (OutputTextConfig)mainGrid.DataContext; }
		}

		// 06/18/2014 by aldentea 
		public OutputTextDialogConfig DialogConfig { get; set; }


		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			switch (MyConfig.Separator)
			{
				case OutputTextConfig.ColumnSeparator.Tab:
					radioButtonTab.IsChecked = true;
					break;
				case OutputTextConfig.ColumnSeparator.Comma:
					radioButtonComma.IsChecked = true;
					break;
				case OutputTextConfig.ColumnSeparator.Space:
					radioButtonSpace.IsChecked = true;
					break;
			}

			Commands.SelectFileCommand.Execute(null, this);
		}

		private void groupBoxSeparator_Checked(object sender, RoutedEventArgs e)
		{
			if (e.OriginalSource == radioButtonTab)
			{
				MyConfig.Separator = OutputTextConfig.ColumnSeparator.Tab;
			}
			else if (e.OriginalSource == radioButtonComma)
			{
				MyConfig.Separator = OutputTextConfig.ColumnSeparator.Comma;
			}
			else if (e.OriginalSource == radioButtonSpace)
			{
				MyConfig.Separator = OutputTextConfig.ColumnSeparator.Space;
			}
		}

		#region SelectFileコマンドハンドラ

    private void SelectFile_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			var dialog = new Microsoft.Win32.SaveFileDialog();
			if (DialogConfig != null && Path.IsPathRooted(DialogConfig.InitialDirectory))
			{
				dialog.InitialDirectory = DialogConfig.InitialDirectory;
			}
			dialog.Filter = "テキストファイル(*.txt;*.csv)|*.txt;*.csv|すべてのファイル|*";
			
			if (dialog.ShowDialog() == true)
			{
				MyConfig.Destination = dialog.FileName;
			}
		}

		private void SelectFile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}
    
    #endregion

		#region OutputTextコマンドハンドラ

    private void OutputText_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			this.DialogResult = true;
			/*
			try
			{
				using (FileStream stream = new FileStream(MyConfig.Destination, FileMode.Create))
				{
					using (StreamWriter writer = new StreamWriter(stream, MyConfig.Encoding))
					{
						writer.NewLine = MyConfig.LineBreakString;

						// アウトプット開始．
						foreach (var order in MyDocument.Orders)
						{
							var memo = order.Memos.SingleOrDefault(m => { return m.Code == "setname"; });
							if (memo != null)
							{
								writer.WriteLine(memo.Value);
							}

							var question = (IntroQuestion)MyDocument.GetQuestion(order.QuestionID);
							writer.WriteLine(string.Format("{0}{2}{1}", question.Song.Title, question.Song.Artist, MyConfig.SeparatorString));
						}

					}
				}
				this.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "エラー");
			}
			 * */
		}

		private void OutputText_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = MyConfig.IsReady;
		}
    
    #endregion

	}


	// 06/17/2014 by aldentea
	#region OutputTextConfigクラス
	public class OutputTextConfig : INotifyPropertyChanged
	{

		#region *Destinationプロパティ
		/// <summary>
		/// 出力先のファイル名を取得／設定します．
		/// </summary>
		public string Destination
		{
			get
			{
				return _destination;
			}
			set
			{
				if (_destination != value)
				{
					_destination = value;
					NotifyPropertyChanged("Destination");
					NotifyPropertyChanged("IsReady");
				}
			}
		}
		string _destination = string.Empty;
		#endregion

		#region *IsReadyプロパティ
		public bool IsReady
		{
			get
			{
				return Path.IsPathRooted(Destination);
			}
		}
		#endregion

		#region *Separatorプロパティ
		public ColumnSeparator Separator
		{
			get
			{
				return _separator;
			}
			set
			{
				if (_separator != value)
				{
					_separator = value;
					NotifyPropertyChanged("Separator");
				}
			}
		}
		ColumnSeparator _separator = ColumnSeparator.Comma;
		#endregion

		public Encoding Encoding
		{
			get
			{
				return Encoding.GetEncoding(932);	// shift_jis
			}
		}

		public LineBreak Break
		{
			get
			{
				return LineBreak.CRLF;
			}
		}

		public string LineBreakString
		{
			get
			{
				switch (Break)
				{
					case OutputTextConfig.LineBreak.CRLF:
						return "\r\n";
					case OutputTextConfig.LineBreak.LF:
						return "\n";
					case OutputTextConfig.LineBreak.CR:
						return "\r";
					default:
						throw new InvalidOperationException();
				}
			}
		}

		public string SeparatorString
		{
			get
			{
				switch (Separator)
				{
					case ColumnSeparator.Tab:
						return "\t";
					case ColumnSeparator.Comma:
						return ",";
					case ColumnSeparator.Space:
						return " ";
					default:
						return string.Empty;
				}
			}
		}


		#region INotifyPropertyChanged実装
		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		protected void NotifyPropertyChanged(string propertyName)
		{
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion


		public enum ColumnSeparator
		{
			Tab,
			Comma,
			Space
		}
		/*
		public enum JapaneseEncoding
		{
			UTF8,
			EUCJP,
			ShiftJIS
		}
		*/
		public enum LineBreak
		{
			CRLF,
			LF,
			CR
		}

	}
	#endregion

	// 06/18/2014 by aldentea : とりあえず。
	public class OutputTextDialogConfig
	{
		public string InitialDirectory { get; set; } 
	}


}
