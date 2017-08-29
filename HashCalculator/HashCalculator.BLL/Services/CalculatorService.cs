using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using HashCalculator.BLL.Interfaces;
using HashCalculator.BLL.Models;

namespace HashCalculator.BLL.Services
{
	public class CalculatorService : ICalculatorService
	{
		private ObservableCollection<FileInformation> _filesCollection;

		//private List<FileInformation> _filesInfo;

		private CancellationTokenSource _cancellationTokenSource;

		private readonly object _lockObject = new object();

		private const string XmlFileName = "\\SerializationOverview.xml";

		public CalculatorService(CancellationTokenSource cancellationTokenSource)
		{
			_filesCollection = new ObservableCollection<FileInformation>();		
			_cancellationTokenSource = cancellationTokenSource;
		}

		public void RestoreToken()
		{
			if (_cancellationTokenSource.IsCancellationRequested)
			{
				_cancellationTokenSource = new CancellationTokenSource();
			}
		}

		public FileInformation GetFileInfo(Stream stream, string filePath)
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

		//public Task InputOfResultsIntoTheControl(FileInformation file, CancellationToken cancellationToken)
		//{
		//	var task = Task.Run(() =>
		//	{
		//		_filesCollection.Add(file);

		//		_filesInfo = _filesCollection.ToList();

		//	}, cancellationToken);


		//	return task;
		//}

		public Task RecordResultsInAnXmlFile(CancellationToken cancellationToken)
		{
			var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

			var path = folder + XmlFileName;

			return Serialize(path, cancellationToken);
		}


		public Task Serialize(string path, CancellationToken cancellationToken)
		{
			var writer = new XmlSerializer(typeof(List<FileInformation>));

			var task = Task.Run(() =>
			{
				lock (_lockObject)
				{
					using (var file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
					{
						writer.Serialize(file, _filesCollection.ToList());
					}
				}
			}, cancellationToken);

			return task;
		}

		private void Cancel()
		{
			_cancellationTokenSource.Cancel();
		}

		public void Add(FileInformation file)
		{
			_filesCollection.Add(file);
		}


		public ObservableCollection<FileInformation> GetFiles()
		{
			return _filesCollection;
		}

		public void ResetCollection()
		{
			_filesCollection = new ObservableCollection<FileInformation>();
		}

	}
}