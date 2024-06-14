namespace FileExplorer.ViewModels.ListView {
	using FileExplorer.ViewModels.ListView.Interfaces;
	using System.Windows;

	internal class FileUploadViewModel : ViewModelBase, IFileUploadViewModel, IFileDragDropTarget {
		private Visibility _uploadVisibility;
		public Visibility UploadVisibility { 
			get => _uploadVisibility; 
			set { 
				_uploadVisibility = value; 
				NotifyOfPropertyChange(() => UploadVisibility); 
			} 
		}

		public FileUploadViewModel() {
			UploadVisibility = Visibility.Visible;
		}

		public void OnFileDrop(string[] filepaths) {
			UploadVisibility = Visibility.Collapsed;
		}
	}
}
