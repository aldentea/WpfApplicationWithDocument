using System;
using System.Collections.Generic;
using System.Linq;
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

		void TextDocument_PropertyChanged(object sender, PropertyChangedEventArgs e)
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

		// 10/22/2014 by aldentea : 返値を追加．
		#region *ドキュメントを読み込む(LoadDocument)
		protected override bool LoadDocument(string fileName)
		{
			using (StreamReader reader = new StreamReader(fileName, Encoding.GetEncoding("UTF-8")))
			{
				this.Body = reader.ReadToEnd();
			}
			return true;
		}
		#endregion

		// 10/22/2014 by aldentea : 返値を追加．
		#region *ドキュメントを保存する(SaveDocument)
		protected override bool SaveDocument(string fileName)
		{
			using (StreamWriter writer = new StreamWriter(fileName, false, Encoding.GetEncoding("UTF-8")))
			{
				writer.Write(_body);
			}
			return true;
		}
		#endregion

		#endregion


	}
}
