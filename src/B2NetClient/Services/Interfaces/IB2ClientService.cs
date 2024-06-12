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
		Task<B2FileList> FetchFilesBaseOnBucketIdAsync(string bucketId);
	}
}
