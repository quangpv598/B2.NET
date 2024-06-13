
namespace FileExplorer.ViewModels {
	using Caliburn.Micro;
	using FileExplorer.Models;
	using FileExplorer.Services;
	using FileExplorer.Services.Interfaces;
	using FileExplorer.ViewModels.Clients;
	using FileExplorer.ViewModels.Interfaces;
	using GalaSoft.MvvmLight.Command;
	using System;
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Windows;
	using System.Windows.Input;

	internal class AuthenticationViewModel : ViewModelBase, IAuthenticationViewModel {
		private IAuthentService _authentService;
		private SettingsService _settingsService;

		private ClientViewModel _selectedClient;
		public ClientViewModel SelectedClient {
			get { return _selectedClient; }
			set {
				_selectedClient = value;
				NotifyOfPropertyChange(() => SelectedClient);
			}
		}

		private bool _isEditting;
		public bool IsEditting {
			get { return _isEditting; }
			set {
				_isEditting = value;
				NotifyOfPropertyChange(() => IsEditting);
			}
		}

		private string _appId;
		public string AppId {
			get { return _appId; }
			set {
				_appId = value;
				NotifyOfPropertyChange(() => AppId);
			}
		}

		private string _appKey;
		public string AppKey {
			get { return _appKey; }
			set {
				_appKey = value;
				NotifyOfPropertyChange(() => AppKey);
			}
		}

		public ObservableCollection<ClientViewModel> Clients { get; set; }

		public AuthenticationViewModel(IAuthentService authentService, SettingsService settingsService) {
			_authentService = authentService;
			_settingsService = settingsService;

			Clients = new ObservableCollection<ClientViewModel>();

			var settings = settingsService.LoadSettings();

			if (settings.Clients is null || !settings.Clients.Any()) return;

			foreach (var client in settings.Clients) {
				var clientVM = new ClientViewModel(client);

				clientVM.EditEvent += EditAccount;
				clientVM.DeleteEvent += DeleteAccount;

				Clients.Add(clientVM);
			}
		}

		public async Task SaveAccountCommand() {
			try {
				var client = await _authentService.Authent(AppId, AppKey);


				if (!IsEditting) {
					if (Clients.Any(c => Equals(AppId, c.AppId))){
						MessageBox.Show("The AppId is duplicated with previous one", "Warning", MessageBoxButton.OK);
						return;
					}

					_settingsService.ApplicationSettings.Clients.Add(client);
					//	_settingsService.SaveChanges();

					var clientVM = new ClientViewModel(client);

					clientVM.EditEvent += EditAccount;
					clientVM.DeleteEvent += DeleteAccount;

					Clients.Add(clientVM);
				}
				else {
					if (Clients.Any(c => Equals(AppId, c.AppId) && !Equals(SelectedClient.Id, c.Id))) {
						MessageBox.Show("The AppId is duplicated with previous one", "Warning", MessageBoxButton.OK);
						return;
					}

					var currentClient = _settingsService.ApplicationSettings.Clients.FirstOrDefault(c => Equals(c.Id, SelectedClient.Id));

					currentClient.AppId = AppId;
					currentClient.AppKey = AppKey;

					SelectedClient.AppId = AppId;
					SelectedClient.AppKey = AppKey;

					_settingsService.SaveChanges();

					IsEditting = false;
				}

				AppKey = string.Empty;
				AppId = string.Empty;

			}
			catch (Exception e) {
				MessageBox.Show(e.Message);
			}
		}

		public void EditAccount(object sender, ClientViewModel client) {
			IsEditting = true;

			AppId = client.AppId;
			AppKey = client.AppKey;

			SelectedClient = client;
		}

		public void DeleteAccount(object sender, ClientViewModel client) {
			var res = MessageBox.Show("Are you sure to delete this account?", "Warning", MessageBoxButton.YesNo);

			if (res == MessageBoxResult.Yes) {
				Clients.Remove(client);

				_settingsService.ApplicationSettings.Clients.Remove(client.Model);
				_settingsService.SaveChanges();
			}
		}
	}
}
