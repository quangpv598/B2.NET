namespace FileExplorer.ViewModels {
	using Caliburn.Micro;
	using FileExplorer.Help;
	using FileExplorer.Services;
	using FileExplorer.Services.Interfaces;
	using FileExplorer.ViewModels.Interfaces;
	using FileExplorer.ViewModels.TreeView.Interfaces;
	using System;

	internal class FileSystemStructureViewModel : ViewModelBase, IFileSystemStructureViewModel {
		private readonly IFileSystemService _fileSystemService;
		public FileSystemStructureViewModel(IFileSystemService fileSystemService) {
			_fileSystemService = fileSystemService;
			fileSystemService.B2ClientService.OnBucketsFetched += B2ClientService_OnBucketsFetched;
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
