using B2Net;
using B2Net.Models;
using FileExplorer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileExplorer.Services {
	public class B2ClientService : IB2ClientService {
		public B2ClientService() {
		}

		public event EventHandler<List<B2Bucket>> OnBucketsFetched;

		public async Task<B2Client> Connect(string appId, string appKey) {
			return await Task.Run(() => {
				var options = new B2Options() {
					KeyId = appId,
					ApplicationKey = appKey,
					PersistBucket = false
				}; 
				var client = new B2Client(B2Client.Authorize(options));
				return client;
			});
		}

		public async Task<List<B2Bucket>> FetchB2Buckets(B2Client client) {
			return await Task.Run(async () => {
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
				OnBucketsFetched?.Invoke(this, buckets);
				return buckets;
			});
		}

		public async Task<B2FileList> FetchFilesBaseOnBucketIdAsync(B2Client client, string bucketId) {
			return await client.Files.GetList(bucketId: bucketId);
		}

		public async Task<B2File> DownloadFileById(B2Client client, string fileId) {
			return await client.Files.DownloadById(fileId);
		}

		public async Task UploadLargeFile(B2Client client, string bucketId, string filePath) {
			var fileName = Path.GetFileName(filePath);
			FileStream fileStream = File.OpenRead(filePath);
			byte[] c = null;
			List<byte[]> parts = new List<byte[]>();
			var shas = new List<string>();
			long fileSize = fileStream.Length;
			long totalBytesParted = 0;
			long minPartSize = 1024 * (5 * 1024);

			while (totalBytesParted < fileSize) {
				var partSize = minPartSize;
				// If last part is less than min part size, get that length
				if (fileSize - totalBytesParted < minPartSize) {
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
				start = client.LargeFiles.StartLargeFile(fileName, "", bucketId).Result;

				for (int i = 0; i < parts.Count; i++) {
					var uploadUrl = client.LargeFiles.GetUploadPartUrl(start.FileId).Result;
					var part = client.LargeFiles.UploadPart(parts[i], i + 1, uploadUrl).Result;
				}

				finish = client.LargeFiles.FinishLargeFile(start.FileId, shas.ToArray()).Result;
			}
			catch (Exception e) {
				await client.LargeFiles.CancelLargeFile(start.FileId);
				Console.WriteLine(e);
				throw;
			}

			// Clean up.
			var deletedFile = client.Files.Delete(start.FileId, start.FileName).Result;
		}

		public async Task<B2File> DeleteFileById(B2Client client, string fileId, string fileName) {

			return await client.Files.Delete(fileId, fileName);
		}
	}
}
