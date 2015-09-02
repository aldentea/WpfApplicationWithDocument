using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;

// 06/02/2014 by aldentea : namespaceをAldentea.Wpf.Document.Xmlに移動．
// 01/04/2012 by aldentea : プロジェクトを移動．それに伴い，namespaceをSPP.mutus.DataからDocumentに変更．
namespace Aldentea.Wpf.Document.Xml
{
	// 10/06/2011 by aldentea
	#region ElementRemovedCacheクラス
	public class ElementRemovedCache : IOperationCache
	{
		/// <summary>
		/// 追加された要素を取得します．
		/// </summary>
		private XElement Element { get; set; }

		/// <summary>
		/// 追加された要素の親要素を取得します．
		/// </summary>
		private XElement Parent { get; set; }

		#region *コンストラクタ(ElementRemovedCache)
		/// <summary>
		/// 内部でelement.Parentにアクセスしますので，
		///	Removeされる前のハンドラで呼び出して下さい．
		/// </summary>
		/// <param name="element"></param>
		public ElementRemovedCache(XElement element)
		{
			this.Element = element;
			this.Parent = element.Parent;
		}
		#endregion

		#region IOperationCache実装

		public void Do()
		{
			Element.Remove();
		}

		public void Reverse()
		{
			Parent.Add(Element);
		}

		#endregion
	}
	#endregion

}
