namespace FileExplorer.Models {
	using B2Net.Models;
	using System.IO;

	using IOPath = System.IO.Path;

	internal abstract class FileSystemObject {
		protected FileSystemObject(B2File file) {
		}

		protected FileSystemObject(string path) {
		}

		protected FileSystemObject(string name, string path) {
			Name = name;
			Path = path;
			IsHidden = new DirectoryInfo(path).Attributes.HasFlag(FileAttributes.Hidden);
		}

		public string Name { get; }

		public string Path { get; }

		public bool IsHidden { get; }
	}
}
