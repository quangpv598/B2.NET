namespace FileExplorer.ViewModels.TreeView {
	using FileExplorer.ViewModels.TreeView.Interfaces;
	using GalaSoft.MvvmLight.CommandWpf;
	using System.Windows.Input;

	internal class CreateFolderViewModel : ViewModelBase , ICreateFolderViewModel {
		private string _folderName;
		public string FolderName {
			get => _folderName;
			set {
				_folderName = value;
				NotifyOfPropertyChange(() => FolderName);
			}
		}
		
		public ICommand SaveCommand { get; set; }
		public ICommand CancelCommand { get; set; }

		public CreateFolderViewModel() {
			SaveCommand = new RelayCommand(Save);
			CancelCommand = new RelayCommand(Cancel);
		}

		private void Save() {
			FolderName = string.Empty;
		}

		private void Cancel() {
			FolderName = string.Empty;
		}
	}
}
