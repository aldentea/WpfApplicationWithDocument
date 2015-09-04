using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;

namespace Aldentea.Wpf.Document
{
	// Document.DocumentクラスをAldentea.Wpf.Document.DocumentBaseクラスに変更．


	// Ver.1.1 (08/22/2013 by aldentea)
	// 従来のIsModifiedプロパティの実装はIsDirtyプロパティに変更になりました．
	// 読み込み時に変換がされたことを示すConvertedプロパティが新設され，
	// IsModifiedプロパティは，IsDirtyとConvertedのORを返すようになりました．
	// それにともない，以前virtualだったIsModifiedプロパティは非virtualになり，
	// 代わりにIsDirtyがvirtualになっていますのでご注意ください．
	// ついでに，Confirmデリゲートの名前がConfirmerに変更になっています．


	#region [abstract]DocumentBaseクラス
	/// <summary>
	/// LoadDocument, SaveDocumentがabstract．
	/// </summary>
	public abstract class DocumentBase : INotifyPropertyChanged
	{
		#region INotifyPropertyChanged実装

		/// <summary>
		/// プロパティが変更されたことを通知します．
		/// ただし子クラスから通知する際には，このイベントではなく，
		/// NotifyPropertyChangedメソッドを使用して下さい．
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		#endregion

		#region プロパティ変更通知関連


		// 02/14/2012 by aldentea
		#region *NowLoadingプロパティ
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
					// 保留していた分を通知する．
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

		// 02/14/2012 by aldentea : ロード時フラグが立っていれば，通知を保留するように変更．
		protected void NotifyPropertyChanged(string propertyName)
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

		#endregion

		/// <summary>
		/// FileNameプロパティの名前を表す文字列です．
		/// </summary>
		public const string FILENAME_PROPERTY = "FileName";

		/// <summary>
		/// IsModifiedプロパティの名前を表す文字列です．
		/// </summary>
		public const string IS_MODIFIED_PROPERTY = "IsModified";

		/// <summary>
		/// IsReadOnlyプロパティの名前を表す文字列です．
		/// </summary>
		public const string IS_READ_ONLY_PROPERTY = "IsReadOnly";

		#region *FileNameプロパティ
		/// <summary>
		/// ファイル名を取得／設定します．
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

		// 01/14/2014 by aldentea : IsReadOnlyプロパティがtrueの場合には，このフラグが立たないように変更．
		// 08/22/2013 by aldentea : 従来の定義から大幅に変更．非virtual化(overrideすべき部分はIsDirtyプロパティに切り出し)．
		#region *IsModifiedプロパティ
		/// <summary>
		/// ドキュメントが変更されたか否かの値を取得します．IsReadOnlyがtrueの場合は，この値は常にfalseです(すなわち上書き保存ができない)．
		/// </summary>
		public bool IsModified
		{
			get
			{
				return IsReadOnly ? false : Converted || IsDirty;
			}
		}
		#endregion

		// 08/22/2013 by aldentea : 従来のIsModifiedプロパティをIsDirtyプロパティに変更．IsModifiedプロパティは別に新設．
		#region *[virtual]IsDirtyプロパティ
		/// <summary>
		/// ドキュメントを読み込んだ後に変更されたかどうかを取得します．
		/// 一応publicにしていますが，将来的にprotectedになるかもしれません．
		/// </summary>
		public virtual bool IsDirty {
			get
			{
				return this._isDirty;
			}
		}
		bool _isDirty = false;
		#endregion

		// 03/17/2015 by aldentea : なぜか_isDirtyをtrueにしていたのを，falseに修正．
		// 08/22/2013 by aldentea : IsDirtyプロパティの変更通知を追加(これ必要かな？外部に通知するのはIsModifiedプロパティだけでいいような気もする)．
		#region *[virtual]ダーティフラグをクリア(ClearDirty)
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

		// 08/22/2013 by aldentea : IsDirtyプロパティの変更通知を追加(これ必要かな？外部に通知するのはIsModifiedプロパティだけでいいような気もする)．
		#region *[virtual]ダーティフラグをセット(SetDirty)
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

		// 08/22/2013 by aldentea
		#region *Convertedプロパティ
		/// <summary>
		/// 読み込みにあたってドキュメントを変換したとき(＝もとのままで保存できないとき)に
		/// trueになります．
		/// </summary>
		protected bool Converted
		{
			get
			{
				return _converted;
			}
			set
			{
				if (Converted != value)
				{
					_converted = value;
					NotifyPropertyChanged("Converted");
					NotifyPropertyChanged("IsModified");
				}
			}
		}
		bool _converted = false;
		#endregion

		// 01/14/2014 by aldentea : setterはとりあえずprotectedにしておく．
		#region *IsReadOnlyプロパティ
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

		// 08/08/2014 by aldentea : プロパティの既定値を設定．
		public DocumentBase()
		{
			this.ClearReadOnlyAfterSaveAs = true;
		}

		// 10/22/2014 by aldentea : これらのメソッドの返値をboolに変更！
		protected abstract bool LoadDocument(string fileName);
		protected abstract bool SaveDocument(string fileName);

		// 08/08/2014 by aldentea
		// Savedの型を，EventHandlerからEventHandler<SavedEventHandler>に変更．
		#region イベント

		/// <summary>
		/// ドキュメントが初期化された時に発生します．
		/// </summary>
		public event EventHandler Initialized = delegate { };
		/// <summary>
		/// ドキュメントを開いた時に発生します．
		/// </summary>
		public event EventHandler Opened = delegate { };
		/// <summary>
		/// ドキュメントの保存(Save/SaveAs)に成功した時に発生します．
		/// </summary>
		public event EventHandler<SavedEventArgs> Saved = delegate { };

		// ↓その目的ではNowLoadingプロパティを使用するようにしました。
		/// <summary>
		/// ドキュメントの初期化を開始するときに発生します．
		/// (ハンドラでデータバインディングを抑止するのを意図．)
		/// </summary>
		//public event EventHandler BeginInitializing = delegate { };



		#endregion

		// 07/13/2014 by aldentea
		// NowLoadingフラグをたてる。メソッド名をInitializeDocumentに変更。個別の処理をInitializeメソッドに切り出し。
		// 01/14/2014 by aldentea : IsReadOnlyプロパティの初期化を追加．
		// 01/07/2014 by aldentea : Initializedイベントを発生させるように変更．
		// 08/22/2013 by aldentea : Convertedプロパティの初期化を追加．
		#region *初期化(InitializeDocument)
		/// <summary>
		/// ドキュメントを初期化します．
		/// オーバーライドする時は，派生クラス独自処理の後にbase.Initialize()を呼び出しましょう．
		/// </summary>
		private void InitializeDocument()
		{
			NowLoading = true;
			try
			{
				Initialize();

				this.FileName = string.Empty;
				ClearDirty();
				Converted = false;
				IsReadOnly = false;
				this.Initialized(this, EventArgs.Empty);
			}
			finally
			{
				NowLoading = false;
			}
		}
		#endregion

		// 07/13/2014 by aldentea
		protected abstract void Initialize();

		// (2.3.1)読み取り専用ファイルを開くときには自動的にIsReadOnlyプロパティをtrueにする．
		// 10/22/2014 by aldentea : LoadDocumentの返値のチェックを追加．
		// 01/14/2014 by aldentea : isReadOnly引数を追加．
		// 02/14/2012 by aldentea : NowLoadingプロパティのsetを追加．
		#region *開く(Open)
		/// <summary>
		/// 指定されたファイルを開きます．
		/// ファイルが正常に開いた場合は，Openedイベントが発生します．
		/// </summary>
		/// <param name="fileName">開くファイル名をフルパスでどうぞ．</param>
		public void Open(string fileName, bool isReadOnly = false)
		{
			//Initialize();	// ←これいる？
			System.IO.FileInfo info = new System.IO.FileInfo(fileName);
			isReadOnly = isReadOnly || info.IsReadOnly;

			this.NowLoading = true;
			try
			{
				if (!LoadDocument(fileName))
				{
					return;
				}
			}
			finally
			{
				this.NowLoading = false;
			}
			this.FileName = fileName;
			this.IsReadOnly = isReadOnly;
			ClearDirty();	// ※ここではConvertedをクリアしない！
			Opened(this, EventArgs.Empty);
		}
		#endregion

		// ※↓これいるの？
		#region *閉じる(Close)
		/// <summary>
		/// いまのところ，Initializeメソッドのエイリアスです．
		/// </summary>
		public void Close()
		{
			InitializeDocument();
		}
		#endregion


		#region 保存関連プロパティ

		// 08/08/2014 by aldentea
		/// <summary>
		/// [読み取り専用]のドキュメントに対して[名前をつけて保存]したときに，
		/// [読み取り専用]を解除するかどうかの値を取得／設定します．
		/// 既定はtrueです．
		/// </summary>
		public bool ClearReadOnlyAfterSaveAs { get; set; }

		// 08/08/2014 by aldentea
		/// <summary>
		/// [コピーを保存]したときにSavedイベントを発生させるかどうかの値を取得／設定します．
		/// 既定はfalseです．
		/// </summary>
		public bool RaiseSavedEventAfterSaveCopyAs { get; set; }

		#endregion


		// SaveとSaveAsで処理が重複しているのが気に入らないけど...

		// 10/22/2014 by aldentea : SaveDocumentの返値の処理を追加．(必要かどうかはわかりませんｗ)
		// 08/08/2014 by aldentea : Savedイベントの引数型の変更に対応．
		// 06/17/2014 by aldentea : Convertedであれば，RequireSaveAsを返すように変更．
		// 01/08/2013 by aldentea : IsDirtyの確認時に，Convertedも確認するように修正．
		// 08/22/2013 by aldentea : Convertedプロパティのクリアを追加．
		#region *保存(Save)
		public SaveResult Save()
		{
			// readonlyだと常にfalseを返す．
			if (this.IsModified)
			{
				if (this.Converted || (string.IsNullOrEmpty(this.FileName))) {
			//if (this.IsDirty || this.Converted)
			//{
			//  if (this.FileName == string.Empty)
			//  {
					return SaveResult.RequireSaveAs;
				}
				else
				{
					if (SaveDocument(this.FileName))
					{
						ClearDirty();
						Converted = false;
						Saved(this, new SavedEventArgs(FileName));
						return SaveResult.Succeed;
					}
					else
					{
						// こういうケースがあるのかわからないけど，とりあえず実装しておく．
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

		// 08/08/2014 by aldentea : Savedイベントの引数型の変更に対応．
		// 08/08/2014 by aldentea : IsReadOnlyをクリアする処理を追加．
		// 08/22/2013 by aldentea : Convertedプロパティのクリアを追加．
		// 06/14/2011 by aldentea : IsModifiedのチェックを削除．(更新されていなくても名前を変えて保存したい場合があるよね！)
		// 04/26/2011 by aldentea : FileNameプロパティを保存処理の前に設定するように変更．
		#region *名前をつけて保存(SaveAs)
		public SaveResult SaveAs(string fileName)
		{
			// ☆IsReadOnlyが立っていると，Saveはできないが，SaveAsから同名で保存することは(現在の実装では)可能．
			// それをどう考えるか？
			// ☆また，IsReadOnlyが立っているドキュメントをSaveAsすると，
			// (少なくともFileNameが変わった場合には)IsReadOnlyは下ろしていいのではないか？

			string oldFileName = this.FileName;
			this.FileName = fileName;
			try
			{
				SaveDocument(fileName);
			}
			catch (Exception ex)
			{
				this.FileName = oldFileName;
				throw ex;
			}

			ClearDirty();
			Converted = false;
			if (ClearReadOnlyAfterSaveAs)
			{
				IsReadOnly = false;
			}
			Saved(this, new SavedEventArgs(FileName));
			return SaveResult.Succeed;
		}
		#endregion

		// 08/08/2014 by aldentea : Savedイベントを発生させる選択肢を用意．
		#region *コピーを保存(SaveCopyAs)
		/// <summary>
		/// ファイルのコピーを保存します．
		/// Savedイベントは発生しません！(ファイル履歴にも影響しません！)
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public SaveResult SaveCopyAs(string fileName)
		{
			//string oldFileName = this.FileName;
			SaveDocument(fileName);

			if (RaiseSavedEventAfterSaveCopyAs)
			{
				Saved(this, new SavedEventArgs(fileName));
			}
			return SaveResult.Succeed;
		}
		#endregion

		// 08/22/2013 by aldentea : ConfirmからConfirmerに名前を変更．
		// 01/18/2012 by aldentea
		#region *Confirmerデリゲート
		/// <summary>
		/// ドキュメントからユーザに確認を求める時に使われるデリゲートです．
		/// 操作を実行してよければtrue，中止したければfalseを返します．
		/// デフォルトでは常にtrueを返します．
		/// また，Confirmer.Invokeする代わりにConfirmメソッドを使うと便利です．
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
		Predicate<string> _confirm = null;
		#endregion

		// 08/22/2013 by aldentea
		#region *ユーザに確認をとる(Confirm)
		/// <summary>
		/// ユーザに対してYes/No形式の確認を行います．
		/// 確認方法はConfirmerデリゲートに設定します．
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
		Func<string, ConfirmCollectivelyAnswer> _collective_confirmer = null;

		// 10/27/2014 by aldentea
		#region *ユーザに確認をとる(ConfirmCollectively)
		/// <summary>
		/// ユーザに対してYes/No形式の確認を行います．
		/// 確認方法はConfirmerデリゲートに設定します．
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public ConfirmCollectivelyAnswer ConfirmCollectively(string message)
		{
			return CollectiveConfirmer.Invoke(message);
		}
		#endregion

	}
	#endregion

	// 10/22/2014 by aldentea : Cancelledを追加．
	#region SaveResult列挙体
	/// <summary>
	/// ドキュメントの保存結果として返す識別子を指定します．
	/// </summary>
	public enum SaveResult
	{
		/// <summary>
		/// ファイルに保存しました
		/// </summary>
		Succeed,
		/// <summary>
		/// ドキュメントが更新されていません，あるいは読み取り専用です．
		/// </summary>
		NotModified,
		/// <summary>
		/// 名前をつけて保存して下さい
		/// </summary>
		RequireSaveAs,
		/// <summary>
		/// キャンセルされました．
		/// </summary>
		Cancelled

	}
	#endregion

	// 08/08/2014 by aldentea
	#region SavedEventArgsクラス
	public class SavedEventArgs : EventArgs
	{
		/// <summary>
		/// 保存されたファイルの名前を取得します．
		/// </summary>
		public string FileName { get; private set; }

		public SavedEventArgs(string fileName)
		{
			this.FileName = fileName;
		}
	}
	#endregion

	// 06/22/2011 by aldentea
	#region DocumentFormatExceptionクラス
	public class DocumentFormatException : Exception, System.Runtime.Serialization.ISerializable
	{

		#region *コンストラクタ(DocumentFormatException)
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
	#region OpenDocumentParameter構造体
	public struct OpenDocumentParameter
	{
		/// <summary>
		/// 開くファイル名を指定します．
		/// string.IsNullOrEmptyがこの値についてtrueを返す場合は，対話的にファイル名を指定させます(ex.ファイルダイアログを表示)．
		/// </summary>
		public string FileName
		{
			get { return _fileName; }
			set { _fileName = value; }
		}

		/// <summary>
		/// この値がtrueの場合，読み取り専用でファイルを開きます．
		/// 対話的にファイルを指定する場合は，デフォルト値にこの値を用います．
		/// </summary>
		public bool IsReadOnly
		{
			get { return _isReadOnly; }
			set { _isReadOnly = value; }
		}

		/// <summary>
		/// 対話的にファイルを指定する場合に，「読み取り専用」を選択可能にします．
		/// FileNameプロパティにファイル名を指定した場合は，この値は意味がありません．
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
