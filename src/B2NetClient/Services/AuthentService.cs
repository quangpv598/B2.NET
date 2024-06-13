using B2Net;
using FileExplorer.Services.Interfaces;
using FileExplorer.Models;
using System;
using System.Threading.Tasks;

namespace FileExplorer.Services {
	internal class AuthentService : IAuthentService {
		public AuthentService() { }

		public async Task<Client> Authent(string appId, string appKey) {
			try {
				return await Task.Run(() => {
					B2Client client = null;

					client = new B2Client(appId, appKey);

					return new Client {
						AppId = appId,
						AppKey = appKey
					};
				});

			}
			catch (Exception e) {
				throw new Exception("AppID or AppKey is invalid!");
			}
		}
	}
}
