using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Aldentea.Wpf.Document;

namespace Aldentea.Wpf.Application
{

	// 06/18/2014 by aldentea
	// 新しいApplicationクラスを導入し，そこでuser.configやDocumentを保持するように考えてみる．
	public class Application : System.Windows.Application
	{
		#region *Documentプロパティ
		public DocumentBase Document
		{
			get;
			set;
			//get
			//{
			//	return  as DocumentBase;
			//}
		}
		#endregion

		// 06/18/2014 by aldentea : WindowWithDocumentクラスで参照．
		public new static Application Current
		{
			get
			{
				return System.Windows.Application.Current as Application;
			}
		}

	}


}
