using B2Net;
using FileExplorer.Models;
using FileExplorer.ViewModels.Clients.Interfaces;
using FileExplorer.ViewModels.ListView.Interfaces;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileExplorer.ViewModels.Clients {
	internal class ClientViewModel : ViewModelBase, IClientViewModel {
		public Client Model { get; set; }

		public Guid Id { get; set; }

		public string AppId { get; set; }

		public string AppKey { get; set; }

		public B2Client B2Client { get; set; }

		public ICommand DeleteCommand { get; set; }

		public ICommand EditCommand { get; set; }

		public event EventHandler<ClientViewModel> DeleteEvent;
		public event EventHandler<ClientViewModel> EditEvent;

		public ClientViewModel(Client model) { 
			Model = model;
			AppId = model.AppId;
			AppKey = model.AppKey;
			Id = model.Id;

			EditCommand = new RelayCommand(Edit);
			DeleteCommand = new RelayCommand(Delete);
		}

		private void Delete() {
			DeleteEvent?.Invoke(this, this);
		}

		public void Edit() {
			EditEvent?.Invoke(this, this);
		}
	}
}
