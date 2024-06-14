

using FileExplorer.ViewModels.Clients;
using System;

namespace FileExplorer.ViewModels.Interfaces {
	internal interface IAuthenticationViewModel {
		event EventHandler<ApplicationKeysViewModel> OnApplicationKeysSelected;
		void LoadData();
	}
}
