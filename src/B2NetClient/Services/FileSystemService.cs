namespace FileExplorer.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
	using System.Threading.Tasks;
	using B2Net.Models;
	using FileExplorer.Factories.Interfaces;
    using FileExplorer.Services.Interfaces;
    using FileExplorer.ViewModels.ListView.Interfaces;
    using FileExplorer.ViewModels.TreeView.Interfaces;

    using IFolderViewModel = FileExplorer.ViewModels.TreeView.Interfaces.IFolderViewModel;

    internal class FileSystemService : IFileSystemService
    {
		private readonly IB2ClientService b2ClientService;
        private readonly IFileSystemFactory fileSystemFactory;

		public IB2ClientService B2ClientService => b2ClientService;

		public FileSystemService(IFileSystemFactory fileSystemFactory, IB2ClientService b2ClientService) {
			this.fileSystemFactory = fileSystemFactory;
			this.b2ClientService = b2ClientService;
		}

		public async Task<IEnumerable<IDriveViewModel>> GetDrives(List<B2Bucket> buckets)
        {
			return buckets.Select(fileSystemFactory.MakeDrive);
        }

        public async Task<IEnumerable<IFolderViewModel>> GetFolders(string path)
        {
			return null;
            //return GetDirectories(path).Select(fileSystemFactory.MakeTreeViewFolder);
        }

        public async Task<IEnumerable<IFileSystemObjectViewModel>> GetFileSystemObjects(string path)
        {
			var files = await b2ClientService.FetchFilesBaseOnBucketIdAsync(path);
			return null;

			//return GetDirectories(files).Select(fileSystemFactory.MakeListViewFolder)
			//	.Concat<IFileSystemObjectViewModel>(GetFiles(files).Select(fileSystemFactory.MakeFile));
        }

        //public int GetDirectoryLength(string path)
        //{
        //    return GetDirectories(path).Length;
        //}

   //     private static string[] GetDirectories(B2FileList path)
   //     {
			//return null;
   //         //return GetFileSystemObjects(path, Directory.GetDirectories);
   //     }

  //      private static string[] GetFiles(B2FileList path)
  //      {
		//	return path;
		//	//return GetFileSystemObjects(path, Directory.GetFiles);
		//}
	}
}
