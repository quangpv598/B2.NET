using FileExplorer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Services.Interfaces {
	internal interface IAuthentService {
		Task<Client> Authent(string appId, string appKey);
	}
}
