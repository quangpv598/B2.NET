﻿namespace FileExplorer.ViewModels {
	using FileExplorer.ViewModels.Interfaces;
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

		public MainViewModel(
			IFileSystemStructureViewModel fileSystemStructureViewModel,
			IFolderContentViewModel folderContentViewModel,
			IAuthenticationViewModel authenticationViewModel) {

			AuthenticationViewModel = authenticationViewModel;
			FileSystemStructureViewModel = fileSystemStructureViewModel;
			FolderContentViewModel = folderContentViewModel;

			UploadFileDiablogVisibility = Visibility.Collapsed;

			FolderContentViewModel.OnUploadButtonClickEvent += HandleUploadFileDiablogShow;
		}

		public IFileSystemStructureViewModel FileSystemStructureViewModel { get; }

		public IFolderContentViewModel FolderContentViewModel { get; }

		public IAuthenticationViewModel AuthenticationViewModel { get; }

		private void HandleUploadFileDiablogShow(object sender, EventArgs e) {
			UploadFileDiablogVisibility = UploadFileDiablogVisibility is Visibility.Visible
				? Visibility.Collapsed 
				: Visibility.Visible;
		}

		public void Dispose() {
			FolderContentViewModel.OnUploadButtonClickEvent -= HandleUploadFileDiablogShow;
		}
	}
}
