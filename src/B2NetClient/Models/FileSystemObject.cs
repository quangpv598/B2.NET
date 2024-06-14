namespace FileExplorer.Models {
	using B2Net.Models;
	using FileExplorer.Factories.Interfaces;
	using System.IO;
	using System.Linq;
	using IOPath = System.IO.Path;

	internal abstract class FileSystemObject {
		protected FileSystemObject(B2File file) {
			Name = file.FileName.Split('/').ToArray().Last().ToString();
			Path = file.FileName;//.Replace($"/{Name}", "");
		}

		protected FileSystemObject(string path) {
			Name = path.Split('/').ToArray().Last().ToString();
			Path = path;
		}

		protected FileSystemObject(string name, string path) {
			Name = name;
			Path = path;
			IsHidden = false;// new DirectoryInfo(path).Attributes.HasFlag(FileAttributes.Hidden);
		}

		public string Name { get; }

		public string Path { get; }

		public bool IsHidden { get; }
	}
}
