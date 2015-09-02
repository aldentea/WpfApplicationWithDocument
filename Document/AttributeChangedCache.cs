using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;


// 06/02/2014 by aldentea : namespaceをAldentea.Wpf.Document.Xmlに移動．
// 01/04/2012 by aldentea : プロジェクトを移動．それに伴い，namespaceをSPP.mutus.DataからDocumentに変更．
namespace Aldentea.Wpf.Document.Xml
{
	// 10/05/2011 by aldentea : 属性名が変更になった場合には対応していません．
	#region AttributeChangedCacheクラス
	public class AttributeChangedCache : IOperationCache
	{

		/// <summary>
		/// 追加された属性名を取得します．
		/// </summary>
		private XName AttributeName { get; set; }

		/// <summary>
		/// 属性の以前の値を取得します．
		/// 新規に追加された場合は，この値はnullです．
		/// </summary>
		private string PreviousValue { get; set; }

		/// <summary>
		/// 属性の現在の値(変更後の値)を取得します．
		/// 属性が削除された場合は，この値はnullです．
		/// </summary>
		private string CurrentValue { get; set; }

		/// <summary>
		/// 追加された要素の親要素を取得します．
		/// </summary>
		private XElement Parent { get; set; }

		static XElement currentParent = null;
		static string currentOldValue = null;

		public static void SetOldValue(XAttribute attribute)
		{
			currentParent = attribute.Parent;
			currentOldValue = attribute.Value;
		}

		// 10/5/2011 by aldentea
		#region *コンストラクタ(AttributeChangedCache)
		public AttributeChangedCache(XAttribute newAttribute)
		{
			this.AttributeName = newAttribute.Name;
			this.PreviousValue = currentOldValue;
			if (newAttribute.Parent == null)
			{
				// 削除された場合．
				this.CurrentValue = null;
				this.Parent = currentParent;
			}
			else
			{
				this.CurrentValue = newAttribute.Value;
				this.Parent = newAttribute.Parent;
			}

			// キャッシュをクリアする．
			currentParent = null;
			currentOldValue = null;
		}
		#endregion

		#region IOperationCache実装

		public void Do()
		{
			Parent.SetAttributeValue(AttributeName, CurrentValue);
		}

		public void Reverse()
		{
			Parent.SetAttributeValue(AttributeName, PreviousValue);
		}

		#endregion

	}
	#endregion

}
