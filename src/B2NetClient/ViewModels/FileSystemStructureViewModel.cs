namespace FileExplorer.ViewModels {
	using Caliburn.Micro;
	using FileExplorer.Help;
	using FileExplorer.Models;
	using FileExplorer.Services;
	using FileExplorer.Services.Interfaces;
	using FileExplorer.ViewModels.Interfaces;
	using FileExplorer.ViewModels.TreeView.Interfaces;
	using System;
	using System.Threading.Tasks;
	using System.Windows.Input;

	internal class FileSystemStructureViewModel : ViewModelBase, IFileSystemStructureViewModel {
		private readonly IAuthenticationViewModel _authenticationViewModel;
		private readonly IFileSystemService _fileSystemService;
		private readonly IB2ClientService _b2ClientService;
		public FileSystemStructureViewModel(IFileSystemService fileSystemService, IAuthenticationViewModel authenticationViewModel, IB2ClientService b2ClientService) {
			_fileSystemService = fileSystemService;
			_authenticationViewModel = authenticationViewModel;
			_b2ClientService = b2ClientService;

			_authenticationViewModel.OnApplicationKeysSelected += _authenticationViewModel_OnApplicationKeysSelected;
			fileSystemService.B2ClientService.OnBucketsFetched += B2ClientService_OnBucketsFetched;
		}

		private void _authenticationViewModel_OnApplicationKeysSelected(object sender, Clients.ApplicationKeysViewModel e) {
			Utils.InvokeIfNeed(async () => {
				Drives?.Clear();
				if (e.B2Client == null) {
					e.B2Client = await _b2ClientService.Connect(e.AppId, e.AppKey);
				}
				await _fileSystemService.FetchBuckets(e.B2Client);
			});
		}

		private void B2ClientService_OnBucketsFetched(object sender, System.Collections.Generic.List<B2Net.Models.B2Bucket> buckets) {
			Utils.InvokeIfNeed(async () => {
				Drives = new BindableCollection<IDriveViewModel>(await _fileSystemService.GetDrives(buckets));
				NotifyOfPropertyChange(nameof(Drives));
			});
		}
		public IObservableCollection<IDriveViewModel> Drives { get; private set; }

	}
}
