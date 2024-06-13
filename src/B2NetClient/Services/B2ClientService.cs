using B2Net;
using B2Net.Models;
using FileExplorer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Services {
	public class B2ClientService : IB2ClientService {
		B2Client _client = null;
		public B2ClientService() {
			Task.Run(async () => {
				_client = new B2Client("0027035975c38f50000000001", "K002RtNQQAdqS7adRUSqVXv5rJbeTCs");
				var bucketList = await _client.Buckets.GetList();

				OnBucketsFetched?.Invoke(this, bucketList);
			});
		}

		public event EventHandler<List<B2Bucket>> OnBucketsFetched;

		public async Task<B2FileList> FetchFilesBaseOnBucketIdAsync(string bucketId) {
			return await _client.Files.GetList(bucketId: bucketId);
		}
	}
}
