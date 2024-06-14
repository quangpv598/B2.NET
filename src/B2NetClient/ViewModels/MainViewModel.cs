namespace FileExplorer.ViewModels {
	using FileExplorer.ViewModels.Interfaces;
	using FileExplorer.ViewModels.ListView.Interfaces;
	using FileExplorer.ViewModels.TreeView.Interfaces;
	using System;
	using System.Windows;

	internal class MainViewModel : ViewModelBase, IMainViewModel, IDisposable {
		private Visibility _uploadFileDiablogVisibility;

		public Visibility UploadFileDiablogVisibility {
			get => _uploadFileDiablogVisibility; 
			set {
				_uploadFileDiablogVisibility = value;
				NotifyOfPropertyChange(() => UploadFileDiablogVisibility);
			}
		}

		private object _currentDialog;
		public object CurrentDialog {
			get => _currentDialog;
			set {
				_currentDialog = value;
				NotifyOfPropertyChange(() => CurrentDialog);
				UploadFileDiablogVisibility = Visibility.Visible;
			}
		}

		public MainViewModel(
			IFileSystemStructureViewModel fileSystemStructureViewModel,
			IFolderContentViewModel folderContentViewModel,
			IAuthenticationViewModel authenticationViewModel,
			IFileUploadViewModel fileUploadViewModel,
			ICreateFolderViewModel createFolderViewModel) {

			AuthenticationViewModel = authenticationViewModel;
			FileSystemStructureViewModel = fileSystemStructureViewModel;
			FolderContentViewModel = folderContentViewModel;
			FileUploadViewModel = fileUploadViewModel;
			CreateFolderViewModel = createFolderViewModel;

			UploadFileDiablogVisibility = Visibility.Collapsed;

			FolderContentViewModel.OnUploadButtonClickEvent += HandleUploadFileDiablogShow;
			FolderContentViewModel.OnCreateFolderButtonClickEvent += HandleCreateFolderButtonClick;
		}

		public IFileSystemStructureViewModel FileSystemStructureViewModel { get; }

		public IFolderContentViewModel FolderContentViewModel { get; }

		public IAuthenticationViewModel AuthenticationViewModel { get; }

		public IFileUploadViewModel FileUploadViewModel { get; }

		public ICreateFolderViewModel CreateFolderViewModel { get; }

		private void HandleUploadFileDiablogShow(object sender, EventArgs e) {
			CurrentDialog = FileUploadViewModel;
		}

		private void HandleCreateFolderButtonClick(object sender, EventArgs e) {
			CurrentDialog = CreateFolderViewModel;
		}

		public void Dispose() {
			FolderContentViewModel.OnUploadButtonClickEvent -= HandleUploadFileDiablogShow;
			FolderContentViewModel.OnCreateFolderButtonClickEvent -= HandleCreateFolderButtonClick;
		}
	}
}
