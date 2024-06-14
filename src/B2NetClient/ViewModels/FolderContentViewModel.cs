namespace FileExplorer.ViewModels {
	using Caliburn.Micro;
	using FileExplorer.Factories;
	using FileExplorer.Factories.Interfaces;
	using FileExplorer.Help;
	using FileExplorer.Models;
	using FileExplorer.Services.Interfaces;
	using FileExplorer.ViewModels.Interfaces;
	using FileExplorer.ViewModels.ListView;
	using FileExplorer.ViewModels.ListView.Interfaces;
	using GalaSoft.MvvmLight.CommandWpf;
	using Ookii.Dialogs.Wpf;
	using System;
	using System.ComponentModel;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows;
	using System.Windows.Input;

	internal class FolderContentViewModel : ViewModelBase, IFolderContentViewModel, IHandle<Folder> {

		private string _downloadFilesPath = string.Empty;

		private readonly IFileSystemFactory fileSystemFactory;
		private readonly IB2ClientService _b2ClientService;
		private readonly IFileSystemService fileSystemService;

		public ICommand UploadCommand { get; set; }
		public ICommand DownloadCommand { get; set; }
		public ICommand AddFolderCommand { get; set; }

		public ICommand DeleteCommand { get; set; }

		public event EventHandler OnUploadButtonClickEvent;
		public event EventHandler OnCreateFolderButtonClickEvent;

		public string CurrentBucketId { get; set; }
		public string CurrentFolder { get; set; }

		public IB2ClientStateManager B2ClientStateManager { get; }

		public FolderContentViewModel(IEventAggregator eventAggreagtor, IFileSystemService fileSystemService, IB2ClientStateManager clientStateManager, IB2ClientService b2ClientService, IFileSystemFactory fileSystemFactory) {
			eventAggreagtor.Subscribe(this);

			this.fileSystemService = fileSystemService;
			B2ClientStateManager = clientStateManager;
			_b2ClientService = b2ClientService;
			this.fileSystemFactory = fileSystemFactory;

			UploadCommand = new RelayCommand(Upload);
			DownloadCommand = new RelayCommand(Download);
			AddFolderCommand = new RelayCommand(AddFolder);
			DeleteCommand = new RelayCommand(Delete);
		}

		public IObservableCollection<IFileSystemObjectViewModel> Entries { get; } = new BindableCollection<IFileSystemObjectViewModel>();

		private string path;
		public string Path {
			get => path;

			set {
				if (path == value) return;

				path = value;
				NotifyOfPropertyChange(() => Path);

				CurrentBucketId = B2ClientStateManager.CurrentBucketId;
				CurrentFolder = B2ClientStateManager.CurrentFolder;
				NotifyOfPropertyChange(() => CurrentBucketId);
				NotifyOfPropertyChange(() => CurrentFolder);

				Utils.InvokeIfNeed(async () => {
					Entries.Clear();
					Entries.AddRange(await fileSystemService.GetFileSystemObjects(path));
				});
			}
		}



		public void Handle(Folder message) {
			Path = message.Path;
		}

		private bool IsCheck() {
			if (string.IsNullOrEmpty(B2ClientStateManager.CurrentBucketId)) {
				MessageBox.Show("Please select a application key");
				return false;
			}

			//if (string.IsNullOrEmpty(_clientStateManager.CurrentFolder)) {
			//	MessageBox.Show("Please select a bucket");
			//	return false;
			//}

			return true;
		}

		private void Upload() {
			//OnUploadButtonClickEvent?.Invoke(this, EventArgs.Empty);

			if (!IsCheck()) {
				return;
			}


			Task.Run((Func<Task>)(async () => {
				VistaOpenFileDialog dialog = new VistaOpenFileDialog();
				dialog.Filter = "All files (*.*)|*.*";
				dialog.Multiselect = true;
				if ((bool)dialog.ShowDialog()) {
					foreach (var file in dialog.FileNames) {
						var b2File = await _b2ClientService.UploadFile((B2Net.B2Client)this.B2ClientStateManager.CurrentB2Client,
							bucketId: (string)this.B2ClientStateManager.CurrentBucketId,
							folderName: (string)this.B2ClientStateManager.CurrentFolder.Replace($"{this.B2ClientStateManager.CurrentBucketId}/", ""),
							filePath: file);

						if (b2File.FileId != null) {
							Entries.Add(fileSystemFactory.MakeFile((B2Net.Models.B2File)b2File));
						}
					}
				}
			}));

		}

		private void Download() {

			if (!IsCheck()) {
				return;
			}

			Task.Run((Func<Task>)(async () => {
				var dialog = new VistaFolderBrowserDialog();
				dialog.Description = "Please select a folder to save files.";
				dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.

				if ((bool)dialog.ShowDialog()) {
					var selectedItems = Entries.Where(item => item.IsSelected);
					var numberOfItems = selectedItems.Count();
					for (int i = 0; i < numberOfItems; i++) {

						var item = selectedItems.ElementAt(i);
						var file = await _b2ClientService.DownloadFileById((B2Net.B2Client)this.B2ClientStateManager.CurrentB2Client, item.Model.FileId);
						try {
							System.IO.File.WriteAllBytes(System.IO.Path.Combine(_downloadFilesPath, (string)file.FileName), (byte[])file.FileData);
						}
						catch (Exception ex) { }
					}
				}
			}));
		}

		private void AddFolder() {

			if (!IsCheck()) {
				return;
			}

			OnCreateFolderButtonClickEvent?.Invoke(this, EventArgs.Empty);
		}

		private void Delete() {

			if (!IsCheck()) {
				return;
			}

			Task.Run((Func<Task>)(async () => {
				if (MessageBox.Show("Are you sure to delete files?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) {
					return;
				}

				var selectedItems = Entries.Where(item => item.IsSelected);
				var numberOfItems = selectedItems.Count();
				for (int i = 0; i < numberOfItems; i++) {

					var item = selectedItems.ElementAt(i);

					if (item is FileViewModel) {
						var file = await _b2ClientService.DeleteFileById((B2Net.B2Client)this.B2ClientStateManager.CurrentB2Client, item.Model.FileId, item.Model.Path);
						Utils.InvokeIfNeed(() => {
							Entries.Remove(item);
						});
					}
					else if (item is FolderViewModel) {
						var bucketId = item.Model.Path.Split('/').FirstOrDefault();
						if (bucketId != null) {
							if (this.B2ClientStateManager.DicB2Buckets.ContainsKey(bucketId)) {
								var files = Enumerable.Where<B2Net.Models.B2File>(this.B2ClientStateManager.DicB2Buckets[bucketId].B2FileList.Files, (Func<B2Net.Models.B2File, bool>)(f => $"{bucketId}/{f.FileName}".StartsWith($"{item.Model.Path}/")));
								foreach (var file in files) {
									await _b2ClientService.DeleteFileById((B2Net.B2Client)this.B2ClientStateManager.CurrentB2Client, file.FileId, file.FileName);
									Utils.InvokeIfNeed(() => {
										Entries.Remove(item);
									});
								}
							}
						}
					}

				}
			}));
		}

	}
}
