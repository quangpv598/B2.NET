using B2Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Services.Interfaces {
	public interface IB2ClientStateManager {
		B2Client CurrentB2Client { get; }
		void SetCurrentB2Client(B2Client client);
	}
}
