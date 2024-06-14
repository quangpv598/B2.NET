using FileExplorer.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.ViewModels.TreeView.Interfaces {
	internal interface ICreateFolderViewModel : IViewModelBase{
		event EventHandler<CreateFolderViewModel> OnRequestViewClosed;
	}
}
