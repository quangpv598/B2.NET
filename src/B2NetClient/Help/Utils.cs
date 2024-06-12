using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Help {
	public static class Utils {
		public static void InvokeIfNeed(Action ac) {
			App.Current.Dispatcher.BeginInvoke(ac);
		}
	}
}
