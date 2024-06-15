namespace FileExplorer.ViewModels {
	using Caliburn.Micro;
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

		private readonly IB2ClientService _b2ClientService;
		private readonly IB2ClientStateManager _clientStateManager;
		private readonly IFileSystemService fileSystemService;

		public ICommand UploadCommand { get; set; }
		public ICommand DownloadCommand { get; set; }
		public ICommand AddFolderCommand { get; set; }

		public ICommand DeleteCommand { get; set; }

		public event EventHandler OnUploadButtonClickEvent;
		public event EventHandler OnCreateFolderButtonClickEvent;

		public FolderContentViewModel(IEventAggregator eventAggreagtor, IFileSystemService fileSystemService, IB2ClientStateManager clientStateManager, IB2ClientService b2ClientService) {
			eventAggreagtor.Subscribe(this);

			this.fileSystemService = fileSystemService;
			_clientStateManager = clientStateManager;
			_b2ClientService = b2ClientService;

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

				Utils.InvokeIfNeed(async () => {
					Entries.Clear();
					Entries.AddRange(await fileSystemService.GetFileSystemObjects(path));
				});
			}
		}



		public void Handle(Folder message) {
			Path = message.Path;
		}

		private void Upload() {
			OnUploadButtonClickEvent?.Invoke(this, EventArgs.Empty);
		}

		private async void Download() {

			var dialog = new VistaFolderBrowserDialog();
			dialog.Description = "Please select a folder to save files.";
			dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.

			if ((bool)dialog.ShowDialog()) {
				var selectedItems = Entries.Where(item => item.IsSelected);
				var numberOfItems = selectedItems.Count();
				for (int i = 0; i < numberOfItems; i++) {

					var item = selectedItems.ElementAt(i);
					var file = await _b2ClientService.DownloadFileById(_clientStateManager.CurrentB2Client, item.Model.FileId);
					try {
						System.IO.File.WriteAllBytes(System.IO.Path.Combine(_downloadFilesPath, file.FileName), file.FileData);
					}
					catch (Exception ex) { }
				}
			}
		}

		private void AddFolder() {
			OnCreateFolderButtonClickEvent?.Invoke(this, EventArgs.Empty);
		}

		private async void Delete() {

			if (MessageBox.Show("Are you sure to delete files?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No){
				return;
			}

			var selectedItems = Entries.Where(item => item.IsSelected);
			var numberOfItems = selectedItems.Count();
			for (int i = 0; i < numberOfItems; i++) {

				var item = selectedItems.ElementAt(i);

				if (item is FileViewModel) {
					var file = await _b2ClientService.DeleteFileById(_clientStateManager.CurrentB2Client, item.Model.FileId, item.Model.Path);
					Utils.InvokeIfNeed(() => {
						Entries.Remove(item);
					});
				}
				else if (item is FolderViewModel) {
					var bucketId = item.Model.Path.Split('/').FirstOrDefault();
					if (bucketId != null) {
						if (_clientStateManager.DicB2Buckets.ContainsKey(bucketId)) {
							var files = _clientStateManager.DicB2Buckets[bucketId].B2FileList.Files.Where(f => $"{bucketId}/{f.FileName}".StartsWith($"{item.Model.Path}/"));
							foreach (var file in files) {
								await _b2ClientService.DeleteFileById(_clientStateManager.CurrentB2Client, file.FileId, file.FileName);
								Utils.InvokeIfNeed(() => {
									Entries.Remove(item);
								});
							}
						}
					}
				}

			}
		}

	}
}
