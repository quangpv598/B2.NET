namespace FileExplorer.Views
{
    public partial class MainView
    {
        public MainView() => InitializeComponent();

		private void Border_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			if (contentControl.IsMouseOver) return;

			PopupBorder.Visibility = System.Windows.Visibility.Collapsed;

		}
    }
}
