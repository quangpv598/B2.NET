namespace FileExplorer.ViewModels.ListView.Interfaces {
	/// <summary>
	/// IFileDragDropTarget Interface
	/// </summary>
	public interface IFileDragDropTarget {
		void OnFileDrop(string[] filepaths);
	}
}
