﻿using B2Net;
using FileExplorer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Services {
	public class B2ClientStateManager : IB2ClientStateManager {
		public B2Client CurrentB2Client { get; private set; }

		public void SetCurrentB2Client(B2Client client) {
			CurrentB2Client = client;
		}
	}
}
