using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using HashCalculator.Models;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using HashCalculator.Commands;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace HashCalculator.ViewModels
{
	public class FilesCalculatorViewModel : INotifyPropertyChanged
	{
		private string[] _filePaths;

		private ObservableCollection<FileInformation> _concurrentQueue;

		private ICommand _calculateCommand;

		private ICommand _cancelCommand;

		private List<FileInformation> _filesInfo;

		private CancellationTokenSource _cancellationTokenSource;

		private readonly object _lockObject = new object();

		private const string XmlFileName = "\\SerializationOverview.xml";

		public List<FileInformation> FilesInfo
		{
			get { return _filesInfo; }
			set
			{
				_filesInfo = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FilesInfo"));
			}
		}

		private int _progressValue;

		public int ProgressValue
		{
			get { return _progressValue; }
			set
			{
				if (value == _progressValue)
					return;

				_progressValue = value;
				OnPropertyChanged();
			}
		}

		private int _progressMax = 100;

		public int ProgressMax
		{
			get { return _progressMax; }
			set
			{
				if (value == _progressMax)
					return;

				_progressMax = value;
				OnPropertyChanged();
			}
		}

		public ICommand CalculateCommand => _calculateCommand ?? (_calculateCommand = new Command(parameter =>
		{
			RestoreToken();

			var path = OpenFileDialog();

			if (!string.IsNullOrEmpty(path))
			{
				Task.Run(() => ConfigureFileInfo(path));
			}
		}));

		public ICommand CancelCommand => _cancelCommand ?? (_cancelCommand = new Command(parameter => { Cancel(); }));


		public FilesCalculatorViewModel()
		{
			FilesInfo = new List<FileInformation>();

			_concurrentQueue = new ObservableCollection<FileInformation>();

			_cancellationTokenSource = new CancellationTokenSource();
		}

		private void Cancel()
		{
			_cancellationTokenSource.Cancel();
		}

		private void ConfigureFileInfo(string path)
		{
			ConfigureStartData();

			RestoreToken();

			_filePaths = Directory.GetFiles(path, "*", SearchOption.AllDirectories).OrderBy(p => p).ToArray();

			ProgressMax = _filePaths.Length;

			foreach (var filePath in _filePaths)
			{
				using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
				{
					var info = GetFileInfo(stream, filePath);

					InputOfResultsIntoTheControl(info, _cancellationTokenSource.Token);

					RecordResultsInAnXmlFile(_cancellationTokenSource.Token);

					WriteToTheProgressBar(_cancellationTokenSource.Token);
				}
			}
		}

		private void ConfigureStartData()
		{
			FilesInfo.Clear();

			_concurrentQueue = new ObservableCollection<FileInformation>();

			ProgressValue = 0;
		}

		private string OpenFileDialog()
		{
			var openFileDialog = new CommonOpenFileDialog();

			ConfigureFileDialog(openFileDialog);

			var path = openFileDialog.FileName;

			return path;
		}

		private void ConfigureFileDialog(CommonOpenFileDialog openFileDialog)
		{
			openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

			openFileDialog.IsFolderPicker = true;

			openFileDialog.ShowDialog();
		}

		private FileInformation GetFileInfo(Stream stream, string filePath)
		{
			var info = new FileInformation();

			using (var hashAlgorithm = MD5.Create())
			{
				info.Path = filePath;

				info.Hash = Encoding.Default.GetString(hashAlgorithm.ComputeHash(stream));

				info.Length = stream.Length;

				stream.Close();

				hashAlgorithm.Clear();
			}

			return info;
		}

		private Task InputOfResultsIntoTheControl(FileInformation file, CancellationToken cancellationToken)
		{
			var task = Task.Run(() =>
			{
				_concurrentQueue.Add(file);

				FilesInfo = _concurrentQueue.ToList();
			}, cancellationToken);


			return task;
		}

		private Task RecordResultsInAnXmlFile(CancellationToken cancellationToken)
		{
			var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

			var path = folder + XmlFileName;

			return Serialize(path, cancellationToken);
		}

		private Task Serialize(string path, CancellationToken cancellationToken)
		{
			var writer = new XmlSerializer(typeof(List<FileInformation>));

			var task = Task.Run(() =>
			{
				lock (_lockObject)
				{
					using (var file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
					{
						writer.Serialize(file, _concurrentQueue.ToList());
					}
				}
			}, cancellationToken);

			return task;
		}

		private Task WriteToTheProgressBar(CancellationToken cancellationToken)
		{
			var task = Task.Run(() => ProgressValue++, cancellationToken);

			return task;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void RestoreToken()
		{
			if (_cancellationTokenSource.IsCancellationRequested)
			{
				_cancellationTokenSource = new CancellationTokenSource();
			}
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
