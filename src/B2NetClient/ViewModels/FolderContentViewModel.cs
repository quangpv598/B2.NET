namespace FileExplorer.ViewModels
{
    using Caliburn.Micro;
	using FileExplorer.Help;
	using FileExplorer.Models;
    using FileExplorer.Services.Interfaces;
    using FileExplorer.ViewModels.Interfaces;
    using FileExplorer.ViewModels.ListView.Interfaces;
	using GalaSoft.MvvmLight.CommandWpf;
	using System;
	using System.Linq;
	using System.Windows.Input;

	internal class FolderContentViewModel : ViewModelBase, IFolderContentViewModel, IHandle<Folder>
    {
        private readonly IFileSystemService fileSystemService;

		public ICommand UploadCommand { get; set; }
		public ICommand DownloadCommand { get; set; }
		public ICommand AddFolderCommand { get; set; }

		public event EventHandler OnUploadButtonClickEvent;
		public event EventHandler OnCreateFolderButtonClickEvent;

		public FolderContentViewModel(IEventAggregator eventAggreagtor, IFileSystemService fileSystemService)
        {
            eventAggreagtor.Subscribe(this);

            this.fileSystemService = fileSystemService;

			UploadCommand = new RelayCommand(Upload);
			DownloadCommand = new RelayCommand(Download);
			AddFolderCommand = new RelayCommand(AddFolder);
		}

		public IObservableCollection<IFileSystemObjectViewModel> Entries { get; } = new BindableCollection<IFileSystemObjectViewModel>();

        private string path;
        public string Path
        {
            get => path;

            set
            {
                if (path == value) return;

                path = value;
                NotifyOfPropertyChange(() => Path);

				Utils.InvokeIfNeed(async () => {
					Entries.Clear();
					Entries.AddRange(await fileSystemService.GetFileSystemObjects(path));
				});
            }
        }



        public void Handle(Folder message)
        {
            Path = message.Path;
        }

		private void Upload() {
			OnUploadButtonClickEvent?.Invoke(this, EventArgs.Empty);
		}

		private void Download() {
			var selectedItems = Entries.Where(item => item.IsSelected);

			foreach (var item in selectedItems) {

			}
		}

		private void AddFolder() {
			OnCreateFolderButtonClickEvent?.Invoke(this, EventArgs.Empty);
		}
    }
}
