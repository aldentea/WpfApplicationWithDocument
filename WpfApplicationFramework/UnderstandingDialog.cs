using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Aldentea.Wpf.Utility
{
	public class UnderstandingDialog : Window
	{
		public static readonly DependencyProperty UnderstoodProperty =
			DependencyProperty.Register("Understood", typeof(bool), typeof(UnderstandingDialog), new PropertyMetadata(false));

		public bool Understood
		{
			get
			{
				return (bool)GetValue(UnderstoodProperty);
			}
			set
			{
				SetValue(UnderstoodProperty, value);
			}
		}

		
	}
}
