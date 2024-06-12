namespace FileExplorer.Models {
	using B2Net.Models;
	using System.IO;

	internal class Drive : Folder {
		internal Drive(B2Bucket bucket) : base(name: $"[{bucket.BucketId}] {bucket.BucketName}",
			path: bucket.BucketId) {
		}
	}
}
