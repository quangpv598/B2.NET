using B2Net;
using B2Net.Models;
using FileExplorer.Services.Interfaces;
using FileExplorer.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileExplorer.Services {
	internal class B2ClientService : IB2ClientService {

		private readonly ILogViewModel _logViewModel;
		public static readonly long MIN_PART_SIZE = 1024 * (5 * 1024);

		public B2ClientService(ILogViewModel logViewModel) {
			_logViewModel = logViewModel;
		}

		public event EventHandler<List<B2Bucket>> OnBucketsFetched;

		public async Task<B2Client> Connect(string appId, string appKey) {
			return await Task.Run(() => {
				_logViewModel.WriteLog($"Connect to {appId}...");
				var options = new B2Options() {
					KeyId = appId,
					ApplicationKey = appKey,
					PersistBucket = false
				};
				var client = new B2Client(B2Client.Authorize(options));

				_logViewModel.WriteLog($"[{appId}]-Connected");
				return client;
			});
		}

		public async Task<List<B2Bucket>> FetchB2Buckets(B2Client client) {
			return await Task.Run(async () => {
				_logViewModel.WriteLog($"Fetching buckets...");
				List<B2Bucket> buckets = null;
				if (client.Capabilities.BucketId == null) {
					buckets = await client.Buckets.GetList();
				}
				else {
					buckets = new List<B2Bucket>() {
						new B2Bucket {
							BucketId = client.Capabilities.BucketId,
							BucketName = client.Capabilities.BucketName,
						}
					};
				}
				_logViewModel.WriteLog($"Fetch buckets completed!");
				OnBucketsFetched?.Invoke(this, buckets);
				return buckets;
			});
		}

		public async Task<B2FileList> FetchFilesBaseOnBucketIdAsync(B2Client client, string bucketId) {
			_logViewModel.WriteLog($"Fetching files in buckets {bucketId}...");
			var _ = await client.Files.GetList(bucketId: bucketId);
			_logViewModel.WriteLog($"Fetch files completed.");
			return _;
		}

		public async Task<B2File> DownloadFileById(B2Client client, string fileId) {
			_logViewModel.WriteLog($"Downloading file by id {fileId}...");
			var _ = await client.Files.DownloadById(fileId);
			_logViewModel.WriteLog($"Download file completed.");
			return _;
		}

		public async Task<B2File> UploadFile(B2Client client, string bucketId, string folderName, string filePath) {
			var fileData = File.ReadAllBytes(filePath);
			_logViewModel.WriteLog($"Uploading file {filePath}...");
			const long minPartLength = 2;
			B2File _;
			if (fileData.Length < MIN_PART_SIZE * minPartLength) {
				_ = await UploadSmallFile(client, bucketId, folderName, filePath);
			}
			else {
				_ = await UploadLargeFile(client, bucketId, folderName, filePath);
			}
			_logViewModel.WriteLog($"Upload file completed.");
			return _;
		}

		private async Task<B2File> UploadLargeFile(B2Client client, string bucketId, string folderName, string filePath) {
			var fileName = $"{folderName}/{Path.GetFileName(filePath)}";
			FileStream fileStream = File.OpenRead(filePath);
			byte[] c = null;
			List<byte[]> parts = new List<byte[]>();
			var shas = new List<string>();
			long fileSize = fileStream.Length;
			long totalBytesParted = 0;

			while (totalBytesParted < fileSize) {
				var partSize = MIN_PART_SIZE;
				// If last part is less than min part size, get that length
				if (fileSize - totalBytesParted < MIN_PART_SIZE) {
					partSize = fileSize - totalBytesParted;
				}

				c = new byte[partSize];
				fileStream.Seek(totalBytesParted, SeekOrigin.Begin);
				fileStream.Read(c, 0, c.Length);

				parts.Add(c);
				totalBytesParted += partSize;
			}

			foreach (var part in parts) {
				string hash = Utilities.GetSHA1Hash(part);
				shas.Add(hash);
			}

			B2File start = null;
			B2File finish = null;
			try {
				start = await client.LargeFiles.StartLargeFile(fileName, "", bucketId);

				for (int i = 0; i < parts.Count; i++) {
					var uploadUrl = await client.LargeFiles.GetUploadPartUrl(start.FileId);
					var part = await client.LargeFiles.UploadPart(parts[i], i + 1, uploadUrl);
				}

				finish = await client.LargeFiles.FinishLargeFile(start.FileId, shas.ToArray());
			}
			catch (Exception e) {
				await client.LargeFiles.CancelLargeFile(start.FileId);
				Console.WriteLine(e);
				throw;
			}

			return finish;
		}

		private async Task<B2File> UploadSmallFile(B2Client client, string bucketId, string folderName, string filePath) {
			var fileName = $"{folderName}/{Path.GetFileName(filePath)}";
			var fileData = File.ReadAllBytes(filePath);
			var file = await client.Files.Upload(fileData, fileName, bucketId);
			return file;
		}

		public async Task<B2File> DeleteFileById(B2Client client, string fileId, string fileName) {
			_logViewModel.WriteLog($"Delete file by id {fileId}...");
			var _ = await client.Files.Delete(fileId, fileName);
			_logViewModel.WriteLog($"Delete file completed.");
			return _;
		}
	}
}
