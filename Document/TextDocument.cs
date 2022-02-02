using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.IO;

using System.ComponentModel;

namespace Aldentea.Wpf.Document
{
	public class TextDocument : DocumentBase
	{

		#region *コンストラクタ(TextDocument)
		public TextDocument()
		{
			this.PropertyChanged += new PropertyChangedEventHandler(TextDocument_PropertyChanged);
		}
		#endregion

		void TextDocument_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "Body":
					SetDirty();
					break;
			}

		}


		#region *Bodyプロパティ
		public string Body
		{
			get
			{
				return this._body;
			}
			set
			{
				if (this._body != value)
				{
					this._body = value;
					NotifyPropertyChanged("Body");
				}
			}
		}
		string _body = string.Empty;
		#endregion

		// 07/13/2014 by aldentea : virtual→abstractとなったのに対応。
		#region *初期化(Initialize)
		protected override void InitializeDocument()
		{
			this.Body = string.Empty;
		}
		#endregion


		#region abstaract実装

		// (4.0.0)async化。
		// 10/22/2014 by aldentea : 返値を追加．
		#region *ドキュメントを読み込む(LoadDocument)
		protected async override Task<bool> LoadDocument(string fileName)
		{
			using (StreamReader reader = new StreamReader(fileName, Encoding.GetEncoding("UTF-8")))
			{
				this.Body = await reader.ReadToEndAsync();
			}
			return true;
		}
		#endregion

		// (4.0.0)async化。
		// 10/22/2014 by aldentea : 返値を追加．
		#region *ドキュメントを保存する(SaveDocument)
		protected async override Task<bool> SaveDocument(string fileName)
		{
			using (StreamWriter writer = new StreamWriter(fileName, false, Encoding.GetEncoding("UTF-8")))
			{
				await writer.WriteAsync(_body);
			}
			return true;
		}
		#endregion

		#endregion


	}
}
