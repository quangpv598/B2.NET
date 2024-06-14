namespace FileExplorer.ViewModels.TreeView {
	internal class CreateFolderViewModel : ViewModelBase {
		private string _folderName;
		public string FolderName {
			get => _folderName;
			set {
				_folderName = value;
				NotifyOfPropertyChange(() => FolderName);
			}
		}

	}
}
