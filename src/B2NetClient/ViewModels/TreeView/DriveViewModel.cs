namespace FileExplorer.ViewModels.TreeView
{
    using Caliburn.Micro;

    using FileExplorer.Services.Interfaces;
    using FileExplorer.ViewModels.TreeView.Interfaces;

    internal class DriveViewModel : FolderViewModel, IDriveViewModel
    {
        public DriveViewModel(IEventAggregator eventAggregator, IFileSystemService fileSystemService, IB2ClientStateManager b2ClientStateManager) : base(eventAggregator, fileSystemService, b2ClientStateManager)
        {
        }
    }
}
