using B2Net;
using B2Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Services.Interfaces {
	public interface IB2ClientService {
		event EventHandler<List<B2Bucket>> OnBucketsFetched;
		Task<B2Client> Connect(string appId, string appKey);
		Task<List<B2Bucket>> FetchB2Buckets(B2Client client);
		Task<B2FileList> FetchFilesBaseOnBucketIdAsync(B2Client client, string bucketId);
		Task<B2File> DownloadFileById(B2Client client, string fileId);
		Task<B2File> UploadFile(B2Client client, string bucketId, string folderName, string filePath);
		Task<B2File> DeleteFileById(B2Client client, string fileId, string fileName);
	}
}
