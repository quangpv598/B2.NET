namespace FileExplorer.Services {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Windows.Shapes;
	using B2Net;
	using B2Net.Models;
	using FileExplorer.Factories.Interfaces;
	using FileExplorer.Models;
	using FileExplorer.Services.Interfaces;
	using FileExplorer.ViewModels.Interfaces;
	using FileExplorer.ViewModels.ListView.Interfaces;
	using FileExplorer.ViewModels.TreeView.Interfaces;

	using IFolderViewModel = FileExplorer.ViewModels.TreeView.Interfaces.IFolderViewModel;

	internal class FileSystemService : IFileSystemService {
		private Dictionary<string, B2Files> dicB2Buckets = new Dictionary<string, B2Files>();
		private B2Client b2Client;

		private readonly IB2ClientService b2ClientService;
		private readonly IFileSystemFactory fileSystemFactory;

		public IB2ClientService B2ClientService => b2ClientService;

		public FileSystemService(IFileSystemFactory fileSystemFactory, IB2ClientService b2ClientService) {
			this.fileSystemFactory = fileSystemFactory;
			this.b2ClientService = b2ClientService;
		}

		public async Task<IEnumerable<IDriveViewModel>> GetDrives(List<B2Bucket> buckets) {
			foreach (var bucket in buckets) {
				if (!dicB2Buckets.ContainsKey(bucket.BucketId)) {
					dicB2Buckets.Add(bucket.BucketId, null);
				}
			}
			return buckets.Select(fileSystemFactory.MakeDrive);
		}

		public async Task<IEnumerable<IFolderViewModel>> GetFolders(string path) {
			return await GetFileSystemObjectsHelper(path, fileSystemFactory.MakeTreeViewFolder);
		}

		public async Task<IEnumerable<IFileSystemObjectViewModel>> GetFileSystemObjects(string path) {
			return await GetFileSystemObjectsHelper<IFileSystemObjectViewModel>(path, fileSystemFactory.MakeListViewFolder, fileSystemFactory.MakeFile);
		}

		private async Task<IEnumerable<T>> GetFileSystemObjectsHelper<T>(
			string path,
			Func<string, T> folderSelector,
			Func<B2File, T> fileSelector = null) {
			B2Files b2Files = await GetB2FilesAsync(path);

			if (b2Files != null && b2Files.B2FileList != null) {
				bool isRootPath = dicB2Buckets.Keys.Contains(path);
				string bucketId = isRootPath ? path : path.Split('/').FirstOrDefault();

				if (isRootPath) {
					var directoriesInRootBucket = b2Files.DicFolder
						.Where(d => !d.Key.Replace($"{bucketId}/", "").Contains("/"))
						.Select(d => folderSelector(d.Key));

					if (fileSelector != null) {
						var fileInRootBucket = b2Files.B2FileList.Files
						.Where(f => !f.FileName.Contains("/"))
						.Select(f => fileSelector!.Invoke(f));

						return directoriesInRootBucket.Concat(fileInRootBucket.Cast<T>());
					}
					else {
						return directoriesInRootBucket.Cast<T>();
					}

				}
				else {
					var filesAndFoldersInSubfolder = b2Files.DicFolder[path];

					if (fileSelector != null) {
						var filesSubfolder = b2Files.B2FileList.Files
						.Where(f => filesAndFoldersInSubfolder.Contains($"{bucketId}/{f.FileName}"))
						.Select(f => fileSelector!.Invoke(f))
						.Cast<IFileViewModel>();

						var directories = filesAndFoldersInSubfolder
							.Where(d => !filesSubfolder.Select(f => $"{bucketId}/{f.Model.Path}").Contains(d))
							.Select(d => folderSelector(d));

						return directories.Concat(filesSubfolder.Cast<T>());
					}
					else {
						var fileArr = b2Files.B2FileList.Files.Select(f => $"{bucketId}/{f.FileName}");
						var directories = filesAndFoldersInSubfolder
							.Where(d => !fileArr.Contains(d))
							.Select(d => folderSelector(d));

						return directories.Cast<T>();
					}
				}
			}

			return null;
		}

		private async Task<B2Files> GetB2FilesAsync(string path) {
			string bucketId = dicB2Buckets.Keys.Contains(path) ? path : path.Split('/').FirstOrDefault();
			B2Files b2Files = dicB2Buckets.ContainsKey(bucketId) ? dicB2Buckets[bucketId] : null;

			if (b2Files == null) {
				var files = await b2ClientService.FetchFilesBaseOnBucketIdAsync(b2Client, path);
				if (files == null) return null;
				b2Files = ConvertFilesToDictionary(path, files);
			}

			return b2Files;
		}

		private B2Files ConvertFilesToDictionary(string bucketId, B2FileList files) {

			dicB2Buckets[bucketId] = new B2Files {
				B2FileList = files
			};
			var b2File = dicB2Buckets[bucketId];
			foreach (var filePath in files.Files.Select(f => $"{f.FileName}").ToArray()) {
				var parts = filePath.Split('/').ToList();
				parts.Insert(0, bucketId);
				for (int i = 0; i < parts.Count - 2; i++) {
					string key = string.Join("/", parts.Take(i + 2));
					string value = string.Join("/", parts.Take(i + 3));

					if (!b2File.DicFolder.ContainsKey(key)) {
						b2File.DicFolder[key] = new List<string>();
					}

					if (!b2File.DicFolder[key].Contains(value)) {
						b2File.DicFolder[key].Add(value);
					}
				}
			}
			return b2File;
		}

		public async Task FetchBuckets(B2Client b2Client) {
			this.b2Client = b2Client;
			dicB2Buckets.Clear();
			await b2ClientService.FetchB2Buckets(this.b2Client);
		}
	}
}
