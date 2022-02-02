using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Xml.Linq;

using Aldentea.Wpf.Document.Xml;

namespace Aldentea.Wpf.Document.Legacy
{

	#region [abstract]XmlDocumentクラス
	public abstract class XmlDocument : DocumentWithOperationHistory
	{

		// 初期化は継承先のコンストラクタで行う．
		readonly XDocument document;

		// 01/04/2012 by aldentea : MutusDocumentクラスからXmlDocumentクラスに移動．
		#region *Documentプロパティ
		protected XDocument Document
		{
			get
			{
				return document;
			}
		}
		#endregion

		// 10/06/2014 by aldentea
		// protectedからpublicにする．設計としてはprotectedの方が良いのだが...
		// 10/02/2014 by aldentea
		#region *UseAutoSaveOperationHistoryプロパティ
		/// <summary>
		/// 操作履歴を自動的に追加する仕組みを使用するかどうかの値を取得／設定します．デフォルト値はtrueです．
		/// </summary>
		public bool UseAutoSaveOperationHistory { get; set; }
		#endregion


		#region *コンストラクタ(XmlDocument)
		/// <summary>
		/// 引数に与えられたXDocumentオブジェクトをDocumentプロパティに設定します．
		/// </summary>
		/// <param name="xdoc"></param>
		public XmlDocument(XDocument xdoc)
			: base()
		{
			this.UseAutoSaveOperationHistory = true;
			this.WriterSettings = new XmlWriterSettings();
			this.document = xdoc;

			document.Changing += new EventHandler<XObjectChangeEventArgs>(document_Changing);
			document.Changed += new EventHandler<XObjectChangeEventArgs>(document_Changed);

		}
		#endregion

		// このあたりの挙動を継承先で抑止するにはどうすればいいか？

		// 10/02/2014 by aldentea : UseAutoSaveOperationHistoryプロパティの値を反映．
		// 01/04/2012 by aldentea : MutusDocumentクラスからXmlDocumentクラスに移動．
		// 10/06/2011 by aldentea : XElementがRemoveされる時の処理を追加．
		// 10/05/2011 by aldentea : XAttribute変更時についての処理を追加．
		#region *ドキュメント変更前(Document_Changing)
		void document_Changing(object sender, XObjectChangeEventArgs e)
		{
			if (this.UseAutoSaveOperationHistory)
			{
				XObject senderObject = (XObject)sender;

				switch (e.ObjectChange)
				{
					case XObjectChange.Remove:
						if (senderObject is XText)
						{
							// 削除されるオブジェクトをキャッシュしておく．
							XTextChangedCache.SetOldValue((XText)senderObject);
						}
						else if (senderObject is XAttribute)
						{
							AttributeChangedCache.SetOldValue((XAttribute)senderObject);
						}
						else if (sender is XElement)
						{
							AddOperationHistory(new ElementRemovedCache((XElement)sender));
						}

						break;
					case XObjectChange.Value:
						if (senderObject is XAttribute)
						{
							AttributeChangedCache.SetOldValue((XAttribute)senderObject);
						}
						break;
				}
			}
		}
		#endregion

		// 10/02/2014 by aldentea : UseAutoSaveOperationHistoryプロパティの値を反映．
		// 01/04/2012 by aldentea : MutusDocumentクラスからXmlDocumentクラスに移動．
		// 10/05/2011 by aldentea : XAttribute変更時についての処理を追加．
		#region *ドキュメント変更時(Document_Changed)
		void document_Changed(object sender, XObjectChangeEventArgs e)
		{
			if (UseAutoSaveOperationHistory)
			{
				XObject senderObject = (XObject)sender;

				if (senderObject is XAttribute)
				{
					switch (e.ObjectChange)
					{
						case XObjectChange.Add:
						case XObjectChange.Remove:
						case XObjectChange.Value:
							AddOperationHistory(new AttributeChangedCache((XAttribute)senderObject));
							break;
					}
				}
				else if (senderObject is XElement && e.ObjectChange == XObjectChange.Add)
				{
					AddOperationHistory(new ElementAddedCache((XElement)senderObject));
				}
				else if (senderObject is XText && e.ObjectChange == XObjectChange.Add)
				{
					AddOperationHistory(new XTextChangedCache((XText)senderObject));
				}
			}
		}
		#endregion


		// 01/04/2012 by aldentea : MutusDocumentクラスからXmlDocumentクラスに移動．
		#region *WriterSettingsプロパティ
		public XmlWriterSettings WriterSettings { get; set; }
		#endregion


		#region (参考)abstractメソッドの実装例

		// 04/15/2013 by aldentea : コードサンプルとして追加．
		//protected override void SaveDocument(string fileName)
		//{
		//	using (XmlWriter writer = XmlWriter.Create(fileName, this.WriterSettings))
		//	{
		//		Document.Save(writer);
		//	}
		//}

		// 04/15/2013 by aldentea : コードサンプルとして追加．
		//protected override void LoadDocument(string fileName)
		//{
		//  using (XmlReader reader = XmlReader.Create(fileName))
		//  {
		//    try
		//    {
		//      XElement source = XElement.Load(reader);
		//      foreach (var player in source.Element("players").Elements("player"))
		//      {
		//        // playerエレメントをthis.documentに追加する．
		//      }
		//    }
		//    catch (XmlException ex)
		//    {
		//      throw new DocumentFormatException(ex.Message, ex);
		//    }
		//  }
		//}

		#endregion

	}
	#endregion

}
