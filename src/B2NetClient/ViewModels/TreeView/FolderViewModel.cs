namespace FileExplorer.ViewModels.TreeView
{
    using Caliburn.Micro;
	using FileExplorer.Help;
	using FileExplorer.Models;
	using FileExplorer.Services.Interfaces;
    using FileExplorer.ViewModels.TreeView.Interfaces;
	using System.Windows;

    internal class FolderViewModel : ViewModelBase, IFolderViewModel
    {
		private readonly IB2ClientStateManager _clientStateManager;
        private readonly IEventAggregator eventAggregator;

        private readonly IFileSystemService fileSystemService;

		public FolderViewModel(IEventAggregator eventAggregator, IFileSystemService fileSystemService, IB2ClientStateManager clientStateManager) {
			this.eventAggregator = eventAggregator;
			this.fileSystemService = fileSystemService;
			_clientStateManager = clientStateManager;
		}

		public override string DisplayName => Folder.Name;

        public IObservableCollection<IFolderViewModel> Folders { get; } = new BindableCollection<IFolderViewModel>();

        private Folder folder;
        public virtual Folder Folder
        {
            get => folder;

            set
            {
                if (folder == value) return;

                folder = value;
                NotifyOfPropertyChange(() => Folder);

                AddPlaceholderFolder();
            }
        }

        private bool isExpanded;
        public bool IsExpanded
        {
            get => isExpanded;

            set
            {
                if (isExpanded == value) return;

                isExpanded = value;
                NotifyOfPropertyChange(() => IsExpanded);

                Folders.Clear();

                if (isExpanded)
                {
					Utils.InvokeIfNeed(async () => {
						Folders.AddRange(await fileSystemService.GetFolders(Folder.Path));
					});
                }
                else
                {
                    AddPlaceholderFolder();
                }
            }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;

            set
            {
                if (isSelected == value) return;


				//if (_clientStateManager.IsFetchingBucket) {
				//	//MessageBox.Show("Please wait to fetch the previous buckets until they have been completed.");
				//	value = false;
				//}

				isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);

                if (isSelected)
                {
                    eventAggregator.BeginPublishOnUIThread(Folder);
                }
            }
        }

        private void AddPlaceholderFolder()
        {
			//if (fileSystemService.GetDirectoryLength(folder.Path) > 0) {
			//	Folders.Add(null);
			//}
		}
    }
}
