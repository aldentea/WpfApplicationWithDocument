
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

namespace Aldentea.Wpf.Document
{
	// Document.Document�N���X��Aldentea.Wpf.Document.DocumentBase�N���X�ɕύX�D


	// (2.4.0)IsModified��Converted�Ɩ��֌W�ɂȂ�܂����D
	// Ver.1.1 (08/22/2013 by aldentea)
	// �]����IsModified�v���p�e�B�̎�����IsDirty�v���p�e�B�ɕύX�ɂȂ�܂����D
	// �ǂݍ��ݎ��ɕϊ������ꂽ���Ƃ�����Converted�v���p�e�B���V�݂���C
	// IsModified�v���p�e�B�́CIsDirty��Converted��OR��Ԃ��悤�ɂȂ�܂����D
	// ����ɂƂ��Ȃ��C�ȑOvirtual������IsModified�v���p�e�B�͔�virtual�ɂȂ�C
	// �����IsDirty��virtual�ɂȂ��Ă��܂��̂ł����ӂ��������D
	// ���łɁCConfirm�f���Q�[�g�̖��O��Confirmer�ɕύX�ɂȂ��Ă��܂��D


	#region [abstract]DocumentBase�N���X
	/// <summary>
	/// LoadDocument, SaveDocument��abstract�D
	/// </summary>
	public abstract class DocumentBase : INotifyPropertyChanged
	{
		#region INotifyPropertyChanged����

		/// <summary>
		/// �v���p�e�B���ύX���ꂽ���Ƃ�ʒm���܂��D
		/// �������q�N���X����ʒm����ۂɂ́C���̃C�x���g�ł͂Ȃ��C
		/// NotifyPropertyChanged���\�b�h���g�p���ĉ������D
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		#endregion

		#region �v���p�e�B�ύX�ʒm�֘A


		// 02/14/2012 by aldentea
		#region *NowLoading�v���p�e�B
		protected bool NowLoading
		{
			get
			{
				return this._nowLoading;
			}
			set
			{
				if (this._nowLoading && !value)
				{
					//�ۗ����Ă�������ʒm����D
					while (changedPropertyQueue.Count > 0)
					{
						PropertyChanged(this, new PropertyChangedEventArgs(changedPropertyQueue.Dequeue()));
					}
				}
				this._nowLoading = value;
			}
		}
		bool _nowLoading;
		#endregion


		Queue<string> changedPropertyQueue = new Queue<string>();

		// 02/14/2012 by aldentea : ���[�h���t���O�������Ă���΁C�ʒm��ۗ�����悤�ɕύX�D
		protected void NotifyPropertyChanged(string propertyName)
		{
			// (6.0.0)NowLoading�v���p�e�B���̂̕ύX�͒ʒm���Ȃ��B
			// ����ƁANowLoading���̒ʒm����Œm�点��Ӗ����ĂȂ��̂ł́H
			// Load���̕ύX�ɂ��Ă͎����Ő���ł���̂�����B�Ƃ������ƂŁA���̃R�[�h���폜����B
			if (propertyName != "NowLoading")
			{
			if (NowLoading)
			{
				if (!changedPropertyQueue.Contains(propertyName))
				{
					changedPropertyQueue.Enqueue(propertyName);
				}
			}
			else
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		}

		#endregion

		/// <summary>
		/// FileName�v���p�e�B�̖��O��\��������ł��D
		/// </summary>
		public const string FILENAME_PROPERTY = "FileName";

		/// <summary>
		/// IsModified�v���p�e�B�̖��O��\��������ł��D
		/// </summary>
		public const string IS_MODIFIED_PROPERTY = "IsModified";

		/// <summary>
		/// IsReadOnly�v���p�e�B�̖��O��\��������ł��D
		/// </summary>
		public const string IS_READ_ONLY_PROPERTY = "IsReadOnly";

		#region *FileName�v���p�e�B
		/// <summary>
		/// �t�@�C�������擾�^�ݒ肵�܂��D
		/// </summary>
		public string FileName
		{
			get
			{
				return this._fileName;
			}
			protected set
			{
				if (this._fileName != value)
				{
					this._fileName = value;
					NotifyPropertyChanged(FILENAME_PROPERTY);
				}
			}
		}
		string _fileName = string.Empty;
		#endregion

		// (2.4.0)Converted��true(����IsReadOnly��false)�̏ꍇ�ɂ��ꂪ���true�ɂȂ�d�l��p�~���܂����D
		// 01/14/2014 by aldentea : IsReadOnly�v���p�e�B��true�̏ꍇ�ɂ́C���̃t���O�������Ȃ��悤�ɕύX�D
		// 08/22/2013 by aldentea : �]���̒�`����啝�ɕύX�D��virtual��(override���ׂ�������IsDirty�v���p�e�B�ɐ؂�o��)�D
		#region *IsModified�v���p�e�B
		/// <summary>
		/// �h�L�������g���ύX���ꂽ���ۂ��̒l���擾���܂��DIsReadOnly��true�̏ꍇ�́C���̒l�͏��false�ł�(���Ȃ킿�㏑���ۑ����ł��Ȃ�)�D
		/// </summary>
		public bool IsModified
		{
			get
			{
				return IsReadOnly ? false : IsDirty;
			}
		}
		#endregion

		// 08/22/2013 by aldentea : �]����IsModified�v���p�e�B��IsDirty�v���p�e�B�ɕύX�DIsModified�v���p�e�B�͕ʂɐV�݁D
		#region *[virtual]IsDirty�v���p�e�B
		/// <summary>
		/// �h�L�������g��ǂݍ��񂾌�ɕύX���ꂽ���ǂ������擾���܂��D
		/// �ꉞpublic�ɂ��Ă��܂����C�����I��protected�ɂȂ邩������܂���D
		/// </summary>
		public virtual bool IsDirty
		{
			get
			{
				return this._isDirty;
			}
		}
		bool _isDirty = false;
		#endregion

		// 03/17/2015 by aldentea : �Ȃ���_isDirty��true�ɂ��Ă����̂��Cfalse�ɏC���D
		// 08/22/2013 by aldentea : IsDirty�v���p�e�B�̕ύX�ʒm��ǉ�(����K�v���ȁH�O���ɒʒm����̂�IsModified�v���p�e�B�����ł����悤�ȋC������)�D
		#region *[virtual]�_�[�e�B�t���O���N���A(ClearDirty)
		public virtual void ClearDirty()
		{
			if (this.IsDirty)
			{
				this._isDirty = false;
				NotifyPropertyChanged("IsDirty");
				NotifyPropertyChanged(IS_MODIFIED_PROPERTY);
			}
		}
		#endregion

		// 08/22/2013 by aldentea : IsDirty�v���p�e�B�̕ύX�ʒm��ǉ�(����K�v���ȁH�O���ɒʒm����̂�IsModified�v���p�e�B�����ł����悤�ȋC������)�D
		#region *[virtual]�_�[�e�B�t���O���Z�b�g(SetDirty)
		public virtual void SetDirty()
		{
			if (!this.IsDirty)
			{
				this._isDirty = true;
				NotifyPropertyChanged("IsDirty");
				NotifyPropertyChanged(IS_MODIFIED_PROPERTY);
			}
		}
		#endregion

		// (3.0.0)Convert����IsConverted�ɉ����D
		// (2.4.0)IsModified�v���p�e�B�Ƃ͖��֌W�ɂȂ�܂����D
		// 08/22/2013 by aldentea
		#region *IsConverted�v���p�e�B
		/// <summary>
		/// �ǂݍ��݂ɂ������ăh�L�������g��ϊ������Ƃ�(�����Ƃ̂܂܂ŕۑ��ł��Ȃ��Ƃ�)��
		/// true�ɂȂ�܂��D���̃v���p�e�B��true���ƁCSave���\�b�h��SaveAs���\�b�h�Ƀ��_�C���N�g����܂��D
		/// </summary>
		protected bool IsConverted
		{
			get
			{
				return _isConverted;
			}
			set
			{
				if (IsConverted != value)
				{
					_isConverted = value;
					NotifyPropertyChanged("Converted");
					//NotifyPropertyChanged("IsModified");
				}
			}
		}
		bool _isConverted = false;
		#endregion

		// 01/14/2014 by aldentea : setter�͂Ƃ肠����protected�ɂ��Ă����D
		#region *IsReadOnly�v���p�e�B
		public bool IsReadOnly
		{
			get
			{
				return _isReadOnly;
			}
			protected set
			{
				if (_isReadOnly != value)
				{
					_isReadOnly = value;
					NotifyPropertyChanged(IS_READ_ONLY_PROPERTY);
					NotifyPropertyChanged(IS_MODIFIED_PROPERTY);
				}
			}
		}
		bool _isReadOnly = false;
		#endregion

		// 08/08/2014 by aldentea : �v���p�e�B�̊���l��ݒ�D
		public DocumentBase()
		{
			this.ClearReadOnlyAfterSaveAs = true;
		}

		// 10/22/2014 by aldentea : �����̃��\�b�h�̕Ԓl��bool�ɕύX�I
		//protected abstract bool LoadDocument(string fileName);
		//protected abstract bool SaveDocument(string fileName);
		protected abstract Task<bool> LoadDocument(string fileName);
		protected abstract Task<bool> SaveDocument(string fileName);

		// 07/13/2014 by aldentea
		protected abstract void InitializeDocument();

		// 08/08/2014 by aldentea
		// Saved�̌^���CEventHandler����EventHandler<SavedEventHandler>�ɕύX�D
		#region �C�x���g

		/// <summary>
		/// �h�L�������g�����������ꂽ���ɔ������܂��D
		/// </summary>
		public event EventHandler Initialized = delegate { };
		/// <summary>
		/// �h�L�������g���J�������ɔ������܂��D
		/// </summary>
		public event EventHandler Opened = delegate { };
		/// <summary>
		/// �h�L�������g�̕ۑ�(Save/SaveAs)�ɐ����������ɔ������܂��D
		/// </summary>
		public event EventHandler<SavedEventArgs> Saved = delegate { };

		// �����̖ړI�ł�NowLoading�v���p�e�B���g�p����悤�ɂ��܂����B
		/// <summary>
		/// �h�L�������g�̏��������J�n����Ƃ��ɔ������܂��D
		/// (�n���h���Ńf�[�^�o�C���f�B���O��}�~����̂��Ӑ}�D)
		/// </summary>
		//public event EventHandler BeginInitializing = delegate { };



		#endregion


		// (3.0.0)InitializeDocument����Initialize�ɉ���(���\�b�h���̓���ւ�)�D
		// 07/13/2014 by aldentea
		// NowLoading�t���O�����Ă�B���\�b�h����InitializeDocument�ɕύX�B�ʂ̏�����Initialize���\�b�h�ɐ؂�o���B
		// 01/14/2014 by aldentea : IsReadOnly�v���p�e�B�̏�������ǉ��D
		// 01/07/2014 by aldentea : Initialized�C�x���g�𔭐�������悤�ɕύX�D
		// 08/22/2013 by aldentea : Converted�v���p�e�B�̏�������ǉ��D
		#region *������(Initialize)
		/// <summary>
		/// �h�L�������g�����������܂��D
		/// �I�[�o�[���C�h���鎞�́C�h���N���X�Ǝ������̌��base.Initialize()���Ăяo���܂��傤�D
		/// </summary>
		private void Initialize()
		{
			NowLoading = true;
			try
			{
				// (6.0.0)�ꉞtry�u���b�N���c�������ǁA���s����󋵂Ȃ�Ă���̂��H
				// ����ȂƂ���NowLoading�v���p�e�B�ɈӖ��͂���̂��H
				InitializeDocument();
			}
			finally
			{
				NowLoading = false;
			}
			this.FileName = string.Empty;
			IsConverted = false;
			IsReadOnly = false;
			ClearDirty();
			this.Initialized(this, EventArgs.Empty);
		}
		#endregion

		// (4.0.0)async���B
		// (2.3.2)LoadDocument���\�b�h���ŁCIsReadOnly�v���p�e�B��true�ɂł���悤�ɉ���
		// (Converted�v���p�e�B�ƈꏏ��true�ɂ���P�[�X��z��)�D
		// (2.3.1)�ǂݎ���p�t�@�C�����J���Ƃ��ɂ͎����I��IsReadOnly�v���p�e�B��true�ɂ���D
		// 10/22/2014 by aldentea : LoadDocument�̕Ԓl�̃`�F�b�N��ǉ��D
		// 01/14/2014 by aldentea : isReadOnly������ǉ��D
		// 02/14/2012 by aldentea : NowLoading�v���p�e�B��set��ǉ��D
		#region *�J��(Open)
		/// <summary>
		/// �w�肳�ꂽ�t�@�C�����J���܂��D
		/// �t�@�C��������ɊJ�����ꍇ�́COpened�C�x���g���������܂��D
		/// </summary>
		/// <param name="fileName">�J���t�@�C�������t���p�X�łǂ����D</param>
		public async Task Open(string fileName, bool isReadOnly = false)
		{
			//Initialize();	// �����ꂢ��H
			System.IO.FileInfo info = new System.IO.FileInfo(fileName);
			isReadOnly = isReadOnly || info.IsReadOnly;

			this.NowLoading = true;
			try
			{
				if (!await LoadDocument(fileName))
				{
					return;
				}
			}
			finally
			{
				this.NowLoading = false;
			}
			this.FileName = fileName;
			this.IsReadOnly = this.IsReadOnly || isReadOnly;
			ClearDirty(); // �������ł�Converted���N���A���Ȃ��I
			Opened(this, EventArgs.Empty);
		}
		#endregion

		// �������ꂢ��́H
		#region *����(Close)
		/// <summary>
		/// ���܂̂Ƃ���CInitialize���\�b�h�̃G�C���A�X�ł��D
		/// </summary>
		public void Close()
		{
			Initialize();
		}
		#endregion


		#region �ۑ��֘A�v���p�e�B

		// 08/08/2014 by aldentea
		/// <summary>
		/// [�ǂݎ���p]�̃h�L�������g�ɑ΂���[���O�����ĕۑ�]�����Ƃ��ɁC
		/// [�ǂݎ���p]���������邩�ǂ����̒l���擾�^�ݒ肵�܂��D
		/// �����true�ł��D
		/// </summary>
		public bool ClearReadOnlyAfterSaveAs { get; set; }

		// 08/08/2014 by aldentea
		/// <summary>
		/// [�R�s�[��ۑ�]�����Ƃ���Saved�C�x���g�𔭐������邩�ǂ����̒l���擾�^�ݒ肵�܂��D
		/// �����false�ł��D
		/// </summary>
		public bool RaiseSavedEventAfterSaveCopyAs { get; set; }

		#endregion


		// Save��SaveAs�ŏ������d�����Ă���̂��C�ɓ���Ȃ�����...

		// (4.0.0)async���B
		// 10/22/2014 by aldentea : SaveDocument�̕Ԓl�̏�����ǉ��D(�K�v���ǂ����͂킩��܂���)
		// 08/08/2014 by aldentea : Saved�C�x���g�̈����^�̕ύX�ɑΉ��D
		// 06/17/2014 by aldentea : Converted�ł���΁CRequireSaveAs��Ԃ��悤�ɕύX�D
		// 01/08/2013 by aldentea : IsDirty�̊m�F���ɁCConverted���m�F����悤�ɏC���D
		// 08/22/2013 by aldentea : Converted�v���p�e�B�̃N���A��ǉ��D
		#region *�ۑ�(Save)
		public async Task<SaveResult> Save()
		{
			// readonly���Ə��false��Ԃ��D
			if (this.IsModified)
			{
				if (this.IsConverted || (string.IsNullOrEmpty(this.FileName)))
				{
			//if (this.IsDirty || this.Converted)
			//{
			//  if (this.FileName == string.Empty)
			//  {
					return SaveResult.RequireSaveAs;
				}
				else
				{
					if (await SaveDocument(this.FileName))
					{
						ClearDirty();
						IsConverted = false;
						Saved(this, new SavedEventArgs(FileName));
						return SaveResult.Succeed;
					}
					else
					{
						// ���������P�[�X������̂��킩��Ȃ����ǁC�Ƃ肠�����������Ă����D
						return SaveResult.Cancelled;
					}
				}
			}
			else
			{
				return SaveResult.NotModified;
			}
		}
		#endregion

		// (4.0.0)async���B
		// 08/08/2014 by aldentea : Saved�C�x���g�̈����^�̕ύX�ɑΉ��D
		// 08/08/2014 by aldentea : IsReadOnly���N���A���鏈����ǉ��D
		// 08/22/2013 by aldentea : Converted�v���p�e�B�̃N���A��ǉ��D
		// 06/14/2011 by aldentea : IsModified�̃`�F�b�N���폜�D(�X�V����Ă��Ȃ��Ă����O��ς��ĕۑ��������ꍇ�������ˁI)
		// 04/26/2011 by aldentea : FileName�v���p�e�B��ۑ������̑O�ɐݒ肷��悤�ɕύX�D
		#region *���O�����ĕۑ�(SaveAs)
		public async Task<SaveResult> SaveAs(string fileName)
		{
			// ��IsReadOnly�������Ă���ƁCSave�͂ł��Ȃ����CSaveAs���瓯���ŕۑ����邱�Ƃ�(���݂̎����ł�)�\�D
			// ������ǂ��l���邩�H
			// ���܂��CIsReadOnly�������Ă���h�L�������g��SaveAs����ƁC
			// (���Ȃ��Ƃ�FileName���ς�����ꍇ�ɂ�)IsReadOnly�͉��낵�Ă����̂ł͂Ȃ����H

			string oldFileName = this.FileName;
			this.FileName = fileName;
			try
			{
				await SaveDocument(fileName);
			}
			catch (Exception)
			{
				this.FileName = oldFileName;
				throw;
			}

			ClearDirty();
			IsConverted = false;
			if (ClearReadOnlyAfterSaveAs)
			{
				IsReadOnly = false;
			}
			Saved(this, new SavedEventArgs(FileName));
			return SaveResult.Succeed;
		}
		#endregion

		// (4.0.0)async���B
		// 08/08/2014 by aldentea : Saved�C�x���g�𔭐�������I������p�ӁD
		#region *�R�s�[��ۑ�(SaveCopyAs)
		/// <summary>
		/// �t�@�C���̃R�s�[��ۑ����܂��D
		/// Saved�C�x���g�͔������܂���I(�t�@�C�������ɂ��e�����܂���I)
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public async Task<SaveResult> SaveCopyAs(string fileName)
		{
			//string oldFileName = this.FileName;
			await SaveDocument(fileName);

			if (RaiseSavedEventAfterSaveCopyAs)
			{
				Saved(this, new SavedEventArgs(fileName));
			}
			return SaveResult.Succeed;
		}
		#endregion


		#region ���[�U�ւ̊m�F

		// 08/22/2013 by aldentea : Confirm����Confirmer�ɖ��O��ύX�D
		// 01/18/2012 by aldentea
		#region *Confirmer�f���Q�[�g
		/// <summary>
		/// �h�L�������g���烆�[�U�Ɋm�F�����߂鎞�Ɏg����f���Q�[�g�ł��D
		/// ��������s���Ă悯���true�C���~���������false��Ԃ��܂��D
		/// �f�t�H���g�ł͏��true��Ԃ��܂��D
		/// �܂��CConfirmer.Invoke��������Confirm���\�b�h���g���ƕ֗��ł��D
		/// </summary>
		public Predicate<string> Confirmer
		{
			get
			{
				return _confirm ?? new Predicate<string>((message) => { return true; });
			}
			set
			{
				_confirm = value;
			}
		}
		Predicate<string>? _confirm = null;
		#endregion

		// 08/22/2013 by aldentea
		#region *���[�U�Ɋm�F���Ƃ�(Confirm)
		/// <summary>
		/// ���[�U�ɑ΂���Yes/No�`���̊m�F���s���܂��D
		/// �m�F���@��Confirmer�f���Q�[�g�ɐݒ肵�܂��D
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public bool Confirm(string message)
		{
			return Confirmer.Invoke(message);
		}
		#endregion


		// 10/27/2014 by aldentea
		public enum ConfirmCollectivelyAnswer
		{
			Yes,
			No,
			All,
			Cancel
		}

		// 10/27/2014 by aldentea
		public Func<string, ConfirmCollectivelyAnswer> CollectiveConfirmer
		{
			get
			{
				return _collective_confirmer ??
					new Func<string, ConfirmCollectivelyAnswer>((message) => { return ConfirmCollectivelyAnswer.All; });
			}
			set
			{
				_collective_confirmer = value;
			}
		}
		Func<string, ConfirmCollectivelyAnswer>? _collective_confirmer = null;

		// 10/27/2014 by aldentea
		#region *���[�U�Ɋm�F���Ƃ�(ConfirmCollectively)
		/// <summary>
		/// ���[�U�ɑ΂���Yes/No�`���̊m�F���s���܂��D
		/// �m�F���@��Confirmer�f���Q�[�g�ɐݒ肵�܂��D
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public ConfirmCollectivelyAnswer ConfirmCollectively(string message)
		{
			return CollectiveConfirmer.Invoke(message);
		}
		#endregion

		#endregion

	}
	#endregion

	// 10/22/2014 by aldentea : Cancelled��ǉ��D
	#region SaveResult�񋓑�
	/// <summary>
	/// �h�L�������g�̕ۑ����ʂƂ��ĕԂ����ʎq���w�肵�܂��D
	/// </summary>
	public enum SaveResult
	{
		/// <summary>
		/// �t�@�C���ɕۑ����܂���
		/// </summary>
		Succeed,
		/// <summary>
		/// �h�L�������g���X�V����Ă��܂���C���邢�͓ǂݎ���p�ł��D
		/// </summary>
		NotModified,
		/// <summary>
		/// ���O�����ĕۑ����ĉ�����
		/// </summary>
		RequireSaveAs,
		/// <summary>
		/// �L�����Z������܂����D
		/// </summary>
		Cancelled

	}
	#endregion

	// 08/08/2014 by aldentea
	#region SavedEventArgs�N���X
	public class SavedEventArgs : EventArgs
	{
		/// <summary>
		/// �ۑ����ꂽ�t�@�C���̖��O���擾���܂��D
		/// </summary>
		public string FileName { get; private set; }

		public SavedEventArgs(string fileName)
		{
			this.FileName = fileName;
		}
	}
	#endregion

	// 06/22/2011 by aldentea
	#region DocumentFormatException�N���X
	public class DocumentFormatException : Exception, System.Runtime.Serialization.ISerializable
	{

		#region *�R���X�g���N�^(DocumentFormatException)
		public DocumentFormatException()
			: base()
		{
		}

		public DocumentFormatException(string message)
			: base(message)
		{
		}

		public DocumentFormatException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected DocumentFormatException(System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
		}
		#endregion

	}
	#endregion


	// 01/14/2014 by aldentea
	#region OpenDocumentParameter�\����
	public struct OpenDocumentParameter
	{
		/// <summary>
		/// �J���t�@�C�������w�肵�܂��D
		/// string.IsNullOrEmpty�����̒l�ɂ���true��Ԃ��ꍇ�́C�Θb�I�Ƀt�@�C�������w�肳���܂�(ex.�t�@�C���_�C�A���O��\��)�D
		/// </summary>
		public string FileName
		{
			get { return _fileName; }
			set { _fileName = value; }
		}

		/// <summary>
		/// ���̒l��true�̏ꍇ�C�ǂݎ���p�Ńt�@�C�����J���܂��D
		/// �Θb�I�Ƀt�@�C�����w�肷��ꍇ�́C�f�t�H���g�l�ɂ��̒l��p���܂��D
		/// </summary>
		public bool IsReadOnly
		{
			get { return _isReadOnly; }
			set { _isReadOnly = value; }
		}

		/// <summary>
		/// �Θb�I�Ƀt�@�C�����w�肷��ꍇ�ɁC�u�ǂݎ���p�v��I���\�ɂ��܂��D
		/// FileName�v���p�e�B�Ƀt�@�C�������w�肵���ꍇ�́C���̒l�͈Ӗ�������܂���D
		/// </summary>
		public bool EnableReadOnly
		{
			get { return _enableReadOnly; }
			set { _enableReadOnly = value; }
		}

		string _fileName;
		bool _isReadOnly;
		bool _enableReadOnly;
	}
	#endregion

}
