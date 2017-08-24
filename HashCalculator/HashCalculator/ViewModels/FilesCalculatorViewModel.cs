using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
using Microsoft.WindowsAPICodePack.Dialogs;
using FileProcessor.Commands;

namespace HashCalculator.ViewModels
{
    public class FilesCalculatorViewModel : INotifyPropertyChanged
    {       
        private string[] _filePaths;
	    private ConcurrentQueue<FileInformation> _cq;

        private ICommand _scanCommand;

        private ICommand _cancelCommand;

        private List<FileInformation> _filesInfo;

        private CancellationTokenSource _cancellationTokenSource;
        public List<FileInformation> FilesInfo
        {
            get { return _filesInfo; }
	        set
            {
                _filesInfo = value;
                if (null != this.PropertyChanged)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("FilesInfo"));
                }
            }
        }

        private int _progressValue = 0;
        public int ProgressValue
        {
            get { return _progressValue; }
	        set
            {
                if (value == _progressValue)
                    return;

                _progressValue = value;
                OnPropertyChanged("ProgressValue");
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

                _progressMax= value;
                OnPropertyChanged("ProgressMax");
            }
        }

        public ICommand ScanCommand => _scanCommand ?? (_scanCommand = new Command(parameter =>
        {
            RestoreCancellationToken();
            var path = OpenDirectory();
            if (!string.IsNullOrEmpty(path))
            {
                Task.Run(() => ConfigureFileInfo(path));
            }
        }));

        public ICommand CancelCommand => _cancelCommand ?? (_cancelCommand = new Command(parameter =>
        {
            _cancellationTokenSource.Cancel();
        }));


        public FilesCalculatorViewModel()
        {
            FilesInfo= new List<FileInformation>();

			_cq = new ConcurrentQueue<FileInformation>();

            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }
        public void ConfigureFileInfo(string path)
        {
            FilesInfo.Clear();

            _cq = new ConcurrentQueue<FileInformation>();

            ProgressValue = 0;

            RestoreCancellationToken();

            _filePaths = Directory.GetFiles(path, "*", SearchOption.AllDirectories).OrderBy(p => p).ToArray();

            ProgressMax = _filePaths.Length;

            var tasks = new List<Task>();

            using (var md5 = MD5.Create())
            {
                foreach (var filePath in _filePaths)
                {

                    tasks.Add(CollectInformation(filePath, md5, _cancellationTokenSource.Token));

                    tasks.Add(RecordResultsInAnXmlFile(_cancellationTokenSource.Token));

                    tasks.Add(InputOfResultsIntoTheControl(_cancellationTokenSource.Token));

                    Task.WaitAll(tasks.ToArray());

                    tasks.Clear();

                    ProgressValue++;
                }

                tasks.Add(RecordResultsInAnXmlFile(_cancellationTokenSource.Token));

                tasks.Add(InputOfResultsIntoTheControl(_cancellationTokenSource.Token));

                Task.WaitAll(tasks.ToArray());
            }

        }

        private string OpenDirectory()
        {
            string path = null;

            var openFileDialog = new CommonOpenFileDialog();

            ConfigureFileDialog(openFileDialog);

            path = openFileDialog.FileName;

            return path;
        }

        private void ConfigureFileDialog(CommonOpenFileDialog openFileDialog)
        {
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            openFileDialog.IsFolderPicker = true;

            openFileDialog.ShowDialog();
        }
        private Task CollectInformation(string filePath, MD5 md5,CancellationToken cancellationToken)
        {
            var task = Task.Run(() =>
            {
                var info = new FileInformation();

                using (var stream = File.OpenRead(filePath))
                {
                    info.Path = filePath;

                    info.Hash = Encoding.Default.GetString(md5.ComputeHash(stream));

                    info.Length = stream.Length;

                    _cq.Enqueue(info);
                }
            }, cancellationToken);

            return task;
        }

        private Task InputOfResultsIntoTheControl(CancellationToken cancellationToken)
        {
            var task = Task.Run(() =>
            {
                if (_cq.Count != 0)
                {
                    FilesInfo = _cq.ToList();                                  
                }
            }, cancellationToken);

            return task;
        }

        private Task RecordResultsInAnXmlFile(CancellationToken cancellationToken)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//SerializationOverview.xml";

            return Serialize(path, cancellationToken);
        }

	    private Task SerializeAppend(string path, CancellationToken cancellationToken)
        {
            var writer = new XmlSerializer(typeof(FileInformation));

            var task = Task.Run(() =>
            {
                FileStream file = File.Open(path, FileMode.Append, FileAccess.Write);

                writer.Serialize(file, _cq.ToList());

                file.Close();
            }, cancellationToken);

            return task;
        }

	    private Task Serialize(string path,CancellationToken cancellationToken)
        {

            var writer = new XmlSerializer(typeof(List<FileInformation>));

            var task = Task.Run(() =>
            {
                using (var file = new StreamWriter(path))
                {
                    writer.Serialize(file, _cq.ToList());
                }
            }, cancellationToken);

            return task;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RestoreCancellationToken()
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
