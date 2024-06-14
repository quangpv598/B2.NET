using FileExplorer.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.ViewModels {
	internal class LogViewModel : ViewModelBase, ILogViewModel {
		public string Log { get; set; } = "";

		public LogViewModel() {
			Log = "Initialize";
		}

		public void WriteLog(string log) {
			Log = log;
			NotifyOfPropertyChange(() => Log);
		}
	}
}
