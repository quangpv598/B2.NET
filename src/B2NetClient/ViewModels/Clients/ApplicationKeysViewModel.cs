namespace FileExplorer.ViewModels.Clients {
	using B2Net;
	using FileExplorer.Models;
	using FileExplorer.ViewModels.Clients.Interfaces;
	using GalaSoft.MvvmLight.Command;
	using System;
	using System.Windows.Input;

	internal class ApplicationKeysViewModel : ViewModelBase, IClientViewModel {
		public ApplicationKeys Model { get; set; }

		public Guid Id { get; set; }

		public string AppId { get; set; }

		public string AppKey { get; set; }

		public B2Client B2Client { get; set; }

		public ICommand DeleteCommand { get; set; }

		public ICommand EditCommand { get; set; }

		public ICommand ClickCommand { get; set; }

		public event EventHandler<ApplicationKeysViewModel> SelectEvent;
		public event EventHandler<ApplicationKeysViewModel> DeleteEvent;
		public event EventHandler<ApplicationKeysViewModel> EditEvent;

		public ApplicationKeysViewModel(ApplicationKeys model) { 
			Model = model;
			AppId = model.AppId;
			AppKey = model.AppKey;
			Id = model.Id;

			EditCommand = new RelayCommand(Edit);
			DeleteCommand = new RelayCommand(Delete);
			ClickCommand = new RelayCommand(Select);
		}

		private void Delete() {
			DeleteEvent?.Invoke(this, this);
		}

		public void Edit() {
			EditEvent?.Invoke(this, this);
		}

		public void Select() {
			SelectEvent?.Invoke(this, this);
		}
	}
}
