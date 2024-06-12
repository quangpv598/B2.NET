namespace FileExplorer.Factories.Interfaces
{
    using System.IO;
	using B2Net.Models;
	using FileExplorer.ViewModels.ListView.Interfaces;
    using FileExplorer.ViewModels.TreeView.Interfaces;

    using IListViewFolder = FileExplorer.ViewModels.ListView.Interfaces.IFolderViewModel;
    using ITreeViewFolder = FileExplorer.ViewModels.TreeView.Interfaces.IFolderViewModel;

    internal interface IFileSystemFactory
    {
        #region TreeView

        IDriveViewModel MakeDrive(B2Bucket driveInfo);

        ITreeViewFolder MakeTreeViewFolder(string path);

        #endregion TreeView

        #region ListView

        IFileViewModel MakeFile(B2File file);

        IListViewFolder MakeListViewFolder(string path);

        #endregion ListView
    }
}
