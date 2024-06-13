namespace FileExplorer.ViewModels
{
    using FileExplorer.ViewModels.Interfaces;

    internal class MainViewModel : ViewModelBase, IMainViewModel
    {
        public MainViewModel(
			IFileSystemStructureViewModel fileSystemStructureViewModel, 
			IFolderContentViewModel folderContentViewModel,
			IAuthenticationViewModel authenticationViewModel)
        {
			AuthenticationViewModel = authenticationViewModel;
            FileSystemStructureViewModel = fileSystemStructureViewModel;
            FolderContentViewModel = folderContentViewModel;
        }

        public IFileSystemStructureViewModel FileSystemStructureViewModel { get; }

        public IFolderContentViewModel FolderContentViewModel { get; }

		public IAuthenticationViewModel AuthenticationViewModel { get; }
	}
}
