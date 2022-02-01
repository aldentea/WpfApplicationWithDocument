using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Aldentea.Wpf.Document;

namespace Aldentea.Wpf.Application
{
	// 08/27/2015 by aldentea : abstract�ɂ��āC�t�@�C�������֌W��������Ɉڂ��D
	// 06/18/2014 by aldentea
	// �V����Application�N���X�𓱓����C������user.config��Document��ێ�����悤�ɍl���Ă݂�D
	public abstract class Application : System.Windows.Application
	{
		#region *Document�v���p�e�B
		/// <summary>
		/// �����Őݒ肵���l�́CBasicWindow(DocumentWithWindow)����DataContext�ɐݒ肳��܂��D
		/// �܂��CNewDocument�Ƃ��ĎQ�Ƃł��܂��D�K�X�L���X�g���Ďg���ĉ������D
		/// </summary>
		public DocumentBase Document
		{
			get;
			set;
		}
		#endregion

		// 06/18/2014 by aldentea : WindowWithDocument�N���X�ŎQ�ƁD
		public new static Application Current
		{
			get
			{
				return (Application)System.Windows.Application.Current;
			}
		}

		/// <summary>
		/// �t�@�C��������\��������̃R���N�V�����ł��D
		/// </summary>
		public abstract System.Collections.Specialized.StringCollection FileHistory { get; set; }

		/// <summary>
		/// �t�@�C�������̕ێ������擾���܂��D
		/// </summary>
		public abstract byte FileHistoryCount { get; }

		/// <summary>
		/// �t�@�C�������̕\�������擾���܂��D
		/// </summary>
		public abstract byte FileHistoryDisplayCount { get; }

		public void AddToFileHistory(string fileName)
		{     // ��������������ȁ��S�z����K�v�A���́H
					// null�ɂȂ肤��̂͊J������������Ȃ��H
					// ���l������̂́CNullReferenceException��΂��čĎ��s�Ƃ��D
			if (FileHistory == null)
			{
				// ����ǂ�����H
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
