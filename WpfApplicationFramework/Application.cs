using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Aldentea.Wpf.Document;

namespace Aldentea.Wpf.Application
{
	// 08/27/2015 by aldentea : abstractにして，ファイル履歴関係をこちらに移す．
	// 06/18/2014 by aldentea
	// 新しいApplicationクラスを導入し，そこでuser.configやDocumentを保持するように考えてみる．
	public abstract class Application : System.Windows.Application
	{
		#region *Documentプロパティ
		/// <summary>
		/// ここで設定した値は，BasicWindow(DocumentWithWindow)側のDataContextに設定されます．
		/// また，NewDocumentとして参照できます．適宜キャストして使って下さい．
		/// </summary>
		public DocumentBase Document
		{
			get;
			set;
		}
		#endregion

		// 06/18/2014 by aldentea : WindowWithDocumentクラスで参照．
		public new static Application Current
		{
			get
			{
				return (Application)System.Windows.Application.Current;
			}
		}

		/// <summary>
		/// ファイル履歴を表す文字列のコレクションです．
		/// </summary>
		public abstract System.Collections.Specialized.StringCollection FileHistory { get; set; }

		/// <summary>
		/// ファイル履歴の保持数を取得します．
		/// </summary>
		public abstract byte FileHistoryCount { get; }

		/// <summary>
		/// ファイル履歴の表示数を取得します．
		/// </summary>
		public abstract byte FileHistoryDisplayCount { get; }

		public void AddToFileHistory(string fileName)
		{     // ※そもそもこんな↓心配する必要アルの？
					// nullになりうるのは開発時だけじゃない？
					// ※考えられるのは，NullReferenceException飛ばして再試行とか．
			if (FileHistory == null)
			{
				// これどうする？
				FileHistory = new System.Collections.Specialized.StringCollection();
			}

			FileHistory.Remove(fileName);
			FileHistory.Insert(0, fileName);

			byte max = FileHistoryCount;
			while (FileHistory.Count > max)
			{
				FileHistory.RemoveAt(max);
			}
		}

	}


}
