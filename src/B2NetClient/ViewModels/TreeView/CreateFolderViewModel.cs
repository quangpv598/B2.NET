namespace FileExplorer.ViewModels.TreeView {
	using FileExplorer.Services;
	using FileExplorer.Services.Interfaces;
	using FileExplorer.ViewModels.TreeView.Interfaces;
	using GalaSoft.MvvmLight.CommandWpf;
	using System;
	using System.Threading.Tasks;
	using System.Windows;
	using System.Windows.Input;

	internal class CreateFolderViewModel : ViewModelBase , ICreateFolderViewModel {

		private readonly IB2ClientService _b2ClientService;
		private readonly IB2ClientStateManager _b2ClientStateManager;

		private string _folderName;

		public event EventHandler<CreateFolderViewModel> OnRequestViewClosed;

		public string FolderName {
			get => _folderName;
			set {
				_folderName = value;
				NotifyOfPropertyChange(() => FolderName);
			}
		}
		
		public ICommand SaveCommand { get; set; }
		public ICommand CancelCommand { get; set; }

		public CreateFolderViewModel(IB2ClientStateManager b2ClientStateManager, IB2ClientService b2ClientService) {
			_b2ClientStateManager = b2ClientStateManager;
			_b2ClientService = b2ClientService;
			SaveCommand = new RelayCommand(Save);
			CancelCommand = new RelayCommand(Cancel);
		}

		private void Save() {
			Task.Run(async () => {
				OnRequestViewClosed?.Invoke(this, null);

				string currentFolder = string.IsNullOrEmpty(_b2ClientStateManager.CurrentFolder) ? FolderName : $"{_b2ClientStateManager.CurrentFolder.Replace($"{_b2ClientStateManager.CurrentBucketId}/", "")}/{FolderName}";

				var file = await _b2ClientService.AddFolder(_b2ClientStateManager.CurrentB2Client,
					_b2ClientStateManager.CurrentBucketId,
					currentFolder);

				FolderName = string.Empty;

				MessageBox.Show("If the viewfinder was not updated, please click Refresh Buckets.");
			});
		}

		private void Cancel() {
			FolderName = string.Empty;
			OnRequestViewClosed?.Invoke(this, null);
		}
	}
}
