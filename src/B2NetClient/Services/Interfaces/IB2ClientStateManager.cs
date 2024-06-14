using B2Net;
using FileExplorer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Services.Interfaces {
	public interface IB2ClientStateManager {
		Dictionary<string, B2Files> DicB2Buckets { get; set; }
		B2Client CurrentB2Client { get; }
		string CurrentBucketId { get; set; }
		string CurrentFolder { get; set; }
		void SetCurrentB2Client(B2Client client);
	}
}
