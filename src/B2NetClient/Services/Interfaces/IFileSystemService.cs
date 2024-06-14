namespace FileExplorer.Services.Interfaces {
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using B2Net;
	using B2Net.Models;
	using FileExplorer.ViewModels.ListView.Interfaces;
	using FileExplorer.ViewModels.TreeView.Interfaces;

	using IFolderViewModel = FileExplorer.ViewModels.TreeView.Interfaces.IFolderViewModel;

	internal interface IFileSystemService {
		IB2ClientService B2ClientService { get; }
		Task<IEnumerable<IDriveViewModel>> GetDrives(List<B2Bucket> buckets);

		Task<IEnumerable<IFolderViewModel>> GetFolders(string path);

		Task<IEnumerable<IFileSystemObjectViewModel>> GetFileSystemObjects(string path);

		//int GetDirectoryLength(string path);

		Task FetchBuckets(B2Client b2Client);
	}
}
