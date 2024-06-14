using FileExplorer.ViewModels.ListView.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.ViewModels.ListView {
	internal class FileUploadViewModel : IFileUploadViewModel, IFileDragDropTarget {
		public void OnFileDrop(string[] filepaths) {
		}
	}
}
