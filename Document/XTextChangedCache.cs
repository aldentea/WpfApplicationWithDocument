using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;

// 06/02/2014 by aldentea : namespaceをAldentea.Wpf.Document.Xmlに移動．
// 01/04/2012 by aldentea : プロジェクトを移動．それに伴い，namespaceをSPP.mutus.DataからDocumentに変更．
namespace Aldentea.Wpf.Document
{
	namespace Xml
	{
		#region XTextChangedCacheクラス
		public class XTextChangedCache : Legacy.IOperationCache
		{
			/// <summary>
			/// 変更によって削除されたXTextを取得します．
			/// </summary>
			private string OldValue { get; set; }

			/// <summary>
			/// 変更によって設定されたXTextを取得します．
			/// </summary>
			private string NewValue { get; set; }

			/// <summary>
			/// 変更されたXTextの親要素を取得します．
			/// </summary>
			private XElement Parent { get; set; }

			static XElement? currentParent = null;
			static string currentOldValue = String.Empty;

			public static void SetOldValue(XText oldText)
			{
				currentParent = oldText.Parent;
				currentOldValue = oldText.Value;
			}

			#region *コンストラクタ(ElementAddedCache)
			public XTextChangedCache(XText newText)
			{
				// 一致するはず．
				if (newText.Parent != null && newText.Parent == currentParent)
				{
					this.OldValue = currentOldValue;
					this.Parent = newText.Parent;
					this.NewValue = newText.Value;

					currentParent = null;
					currentOldValue = string.Empty;
				}
				else
				{
					throw new Exception("変更が適切に記録されませんでした。");
				}
			}
			#endregion


			#region IOperationCache実装

			#region *順方向実装(Do)
			public void Do()
			{
				if (NewValue != null)
				{
					Parent.Value = NewValue;
				}
				else
				{
					throw new InvalidOperationException();
				}
			}
			#endregion

			#region *逆方向実装(Reverse)
			public void Reverse()
			{
				if (NewValue != null)
				{
					Parent.Value = OldValue;
				}
				else
				{
					throw new InvalidOperationException();
				}
			}
			#endregion

			#endregion

		}
		#endregion
	}
}
