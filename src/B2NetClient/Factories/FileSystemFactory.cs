namespace FileExplorer.Factories {
	using System.IO;
	using B2Net.Models;
	using Caliburn.Micro;

	using FileExplorer.Factories.Interfaces;
	using FileExplorer.Models;
	using FileExplorer.ViewModels.ListView.Interfaces;
	using FileExplorer.ViewModels.TreeView.Interfaces;

	using File = FileExplorer.Models.File;
	using IListViewFolder = FileExplorer.ViewModels.ListView.Interfaces.IFolderViewModel;
	using ITreeViewFolder = FileExplorer.ViewModels.TreeView.Interfaces.IFolderViewModel;

	internal class FileSystemFactory : IFileSystemFactory {
		public IDriveViewModel MakeDrive(B2Bucket bucket) {
			IDriveViewModel driveViewModel = IoC.Get<IDriveViewModel>();
			driveViewModel.Folder = new Drive(bucket);

			return driveViewModel;
		}

		public ITreeViewFolder MakeTreeViewFolder(string path) {
			ITreeViewFolder folderViewModel = IoC.Get<ITreeViewFolder>();
			folderViewModel.Folder = new Folder(path);

			return folderViewModel;
		}

		public IFileViewModel MakeFile(B2File file) {
			IFileViewModel fileViewModel = IoC.Get<IFileViewModel>();
			fileViewModel.Model = new File(file);

			return fileViewModel;
		}

		public IListViewFolder MakeListViewFolder(string path) {
			IListViewFolder folderViewModel = IoC.Get<IListViewFolder>();
			folderViewModel.Model = new Folder(path);

			return folderViewModel;
		}
	}
}
