using FileExplorer.Models;
using FileExplorer.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace FileExplorer.Services {
	public interface IFileService {
		string GetFile(string path);
		bool SaveFile(string path, string contents);
	}

	public class LocalFileService : IFileService {
		public string GetFile(string path) {
			if (!System.IO.File.Exists(path)) {
				throw new FileNotFoundException();
			}

			return System.IO.File.ReadAllText(path);
		}

		public bool SaveFile(string path, string contents) {
			try {
				System.IO.File.WriteAllText(path, contents);
				return true;
			}
			catch (Exception ex) {
				return false;
			}
		}
	}

	public interface IJsonSettingsLocalFileService<T> {
		T LoadInstance(string path);

		bool SaveInstance(string path, T settings);
	}


	public class JsonSettingsLocalFileService<T> : IJsonSettingsLocalFileService<T> where T : class {
		private readonly IFileService _fileService;

		public JsonSettingsLocalFileService(IFileService fileService) {
			_fileService = fileService;
		}

		public T LoadInstance(string path) {
			T settings = null;
			string settingsString = _fileService.GetFile(path);
			settings = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(settingsString);
			return settings;
		}

		public bool SaveInstance(string path, T settings) {
			try {
				string content = Newtonsoft.Json.JsonConvert.SerializeObject(settings, Newtonsoft.Json.Formatting.Indented);
				return _fileService.SaveFile(path, content);
			}
			catch {
				return false;
			}
		}
	}

	public class SettingsService {
		private static readonly SemaphoreLocker _locker = new SemaphoreLocker();
		private Queue<ApplicationSettings> SaveSettingQueue = new Queue<ApplicationSettings>();
		private bool _isInitialApplicationSetting = false;
		private readonly string _path;
		private readonly IJsonSettingsLocalFileService<ApplicationSettings> _jsonSettingsLocalFileService;
		public ApplicationSettings ApplicationSettings { get; set; } = new ApplicationSettings();

		public SettingsService(IJsonSettingsLocalFileService<ApplicationSettings> jsonSettingsLocalFileService,
			string path) {
			try {
				var dir = System.IO.Path.GetDirectoryName(path);
				if (!Directory.Exists(dir)) {
					Directory.CreateDirectory(dir);
				}
			}
			catch { }

			_jsonSettingsLocalFileService = jsonSettingsLocalFileService;
			_path = path;
			try {
				LoadSettings();
			}
			catch (Exception ex) { }
		}

		public ApplicationSettings LoadSettings() {
			if (!System.IO.File.Exists(_path)) {
				return new ApplicationSettings();
			}

			if (_isInitialApplicationSetting) {
				return ApplicationSettings;
			}

			_isInitialApplicationSetting = true;
			ApplicationSettings = _jsonSettingsLocalFileService.LoadInstance(_path);

			return ApplicationSettings;
		}

		private bool _isSavingFile = false;

		private async Task NotifyToSaveSettingAsync() {
			await _locker.LockAsync(async () => {
				await WaitUntil(() => !_isSavingFile);

				_isSavingFile = true;

				try {
					while (SaveSettingQueue.Count > 0) {
						try {
							SaveSettingQueue.Dequeue();

							// Maximum number of attempts to save the file
							int maxAttempts = 5;

							for (int attempt = 1; attempt <= maxAttempts; attempt++) {
								// Save the settings to the file
								bool isSuccess = _jsonSettingsLocalFileService.SaveInstance(_path, ApplicationSettings);

								// Check if the file content has changed
								if (isSuccess) {
									//Debug.WriteLine("Write file success!");
									break; // The file has been changed, exit the loop
								}
								else if (attempt < maxAttempts) {
									Debug.WriteLine("Retry write file: " + attempt);
									// If the file has not changed and maximum attempts are not reached, try again
									//Thread.Sleep(100); // Wait for 1 second before retrying
									await Task.Delay(100);
								}
								else {
									// Max attempts reached and the file has not changed
									// You can log an error or handle it as needed
									Debug.WriteLine("Fail to write file");
								}
							}
						}
						catch (Exception ex) {
							string message = ex.Message;
						}
					}
				}
				finally {
					_isSavingFile = false;
				}
			});
		}

		public void SaveChanges() {
			SaveSettingQueue.Enqueue(ApplicationSettings);
			NotifyToSaveSettingAsync();
		}

		public static async Task WaitUntil(Func<bool> condition, int frequency = 25, int timeout = -1) {
			var waitTask = Task.Run(async () => {
				while (!condition()) await Task.Delay(frequency);
			});

			if (waitTask != await Task.WhenAny(waitTask,
					Task.Delay(timeout)))
				throw new TimeoutException();
		}
	}

	public class ApplicationSettings {
		public IList<Client> Clients { get; set; }	= new List<Client>();
	}
}
