
namespace FileExplorer.ViewModels {
	using Caliburn.Micro;
	using FileExplorer.Help;
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

		private IB2ClientService _b2ClientService;

		private SettingsService _settingsService;

		private ApplicationKeysViewModel _selectedClient;
		public ApplicationKeysViewModel SelectedClient {
			get { return _selectedClient; }
			set {
				_selectedClient = value;
				NotifyOfPropertyChange(() => SelectedClient);

				_selectedClient.ClickCommand.Execute(null);
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

		public event EventHandler<ApplicationKeysViewModel> OnApplicationKeysSelected;

		public ObservableCollection<ApplicationKeysViewModel> ApplicationKeys { get; set; }

		public AuthenticationViewModel(SettingsService settingsService, IB2ClientService b2ClientService) {
			_settingsService = settingsService;
			_b2ClientService = b2ClientService;

			ApplicationKeys = new ObservableCollection<ApplicationKeysViewModel>();

			var settings = settingsService.LoadSettings();

			if (settings.ApplicationKeys is null || !settings.ApplicationKeys.Any()) return;

			foreach (var client in settings.ApplicationKeys) {
				var clientVM = new ApplicationKeysViewModel(client);

				clientVM.EditEvent += EditAccount;
				clientVM.DeleteEvent += DeleteAccount;
				clientVM.SelectEvent += (s, e) => Task.Run(() => {
					OnApplicationKeysSelected?.Invoke(s, e);
				});

				ApplicationKeys.Add(clientVM);
			}
		}

		public async Task SaveAccountCommand() {
			try {
				var client = await _b2ClientService.Connect(AppId, AppKey);				
				if (!IsEditting) {
					if (ApplicationKeys.Any(c => Equals(AppId, c.AppId))) {
						MessageBox.Show("The AppId is duplicated with previous one", "Warning", MessageBoxButton.OK);
						return;
					}

					var newApplications = new ApplicationKeys {
						AppId = AppId,
						AppKey = AppKey
					};

					_settingsService.ApplicationSettings.ApplicationKeys.Add(newApplications);
					_settingsService.SaveChanges();

					var applicationKeyVM = new ApplicationKeysViewModel(newApplications);
					applicationKeyVM.B2Client = client;

					applicationKeyVM.EditEvent += EditAccount;
					applicationKeyVM.DeleteEvent += DeleteAccount;
					applicationKeyVM.SelectEvent += (s, e) => Task.Run(() => {
						OnApplicationKeysSelected?.Invoke(s, e);
					});

					ApplicationKeys.Add(applicationKeyVM);
				}
				else {
					if (ApplicationKeys.Any(c => Equals(AppId, c.AppId) && !Equals(SelectedClient.Id, c.Id))) {
						MessageBox.Show("The AppId is duplicated with previous one", "Warning", MessageBoxButton.OK);
						return;
					}

					var currentClient = _settingsService.ApplicationSettings.ApplicationKeys.FirstOrDefault(c => Equals(c.Id, SelectedClient.Id));

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

		public void EditAccount(object sender, ApplicationKeysViewModel applicationKey) {
			IsEditting = true;

			AppId = applicationKey.AppId;
			AppKey = applicationKey.AppKey;

			SelectedClient = applicationKey;
		}

		public void DeleteAccount(object sender, ApplicationKeysViewModel applicationKey) {
			var res = MessageBox.Show("Are you sure to delete this account?", "Warning", MessageBoxButton.YesNo);

			if (res == MessageBoxResult.Yes) {
				ApplicationKeys.Remove(applicationKey);

				_settingsService.ApplicationSettings.ApplicationKeys.Remove(applicationKey.Model);
				_settingsService.SaveChanges();
			}
		}
	}
}
