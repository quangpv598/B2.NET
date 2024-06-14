namespace FileExplorer.ViewModels
{
    using Caliburn.Micro;
	using FileExplorer.Help;
	using FileExplorer.Models;
    using FileExplorer.Services.Interfaces;
    using FileExplorer.ViewModels.Interfaces;
    using FileExplorer.ViewModels.ListView.Interfaces;
	using GalaSoft.MvvmLight.CommandWpf;
	using System.Windows.Input;

	internal class FolderContentViewModel : ViewModelBase, IFolderContentViewModel, IHandle<Folder>
    {
        private readonly IFileSystemService fileSystemService;

		public ICommand UploadCommand { get; set; }
		public ICommand RefreshCommand { get; set; }


		public FolderContentViewModel(IEventAggregator eventAggreagtor, IFileSystemService fileSystemService)
        {
            eventAggreagtor.Subscribe(this);

            this.fileSystemService = fileSystemService;

			UploadCommand = new RelayCommand(Upload);
			RefreshCommand = new RelayCommand(Refresh);
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

		}

		private void Refresh() {

		}
    }
}
