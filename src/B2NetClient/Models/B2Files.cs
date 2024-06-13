using B2Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Models {
	public class B2Files {
		public B2FileList B2FileList { get; set; }
		public Dictionary<string, List<string>> DicFolder { get; set; } = new Dictionary<string, List<string>>();
	}
}
