using System;
using System.Windows.Input;

namespace FileExplorer.ViewModels.Interfaces
{
    internal interface IFolderContentViewModel : IViewModelBase
    {
		public event EventHandler OnUploadButtonClickEvent;
	}
}
