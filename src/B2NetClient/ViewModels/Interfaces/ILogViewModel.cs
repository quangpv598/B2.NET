using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.ViewModels.Interfaces {
	internal interface ILogViewModel : IViewModelBase {
		void WriteLog(string log);
	}
}
