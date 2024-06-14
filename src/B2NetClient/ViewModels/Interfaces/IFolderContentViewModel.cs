using Caliburn.Micro;
using FileExplorer.ViewModels.ListView.Interfaces;
using System;
using System.Windows.Input;

namespace FileExplorer.ViewModels.Interfaces
{
    internal interface IFolderContentViewModel : IViewModelBase
    {
		public event EventHandler OnUploadButtonClickEvent;
		IObservableCollection<IFileSystemObjectViewModel> Entries { get; }
	}
}
