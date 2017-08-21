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

namespace HashCalculator.ViewModels
{
    public class FilesCalculatorViewModel : INotifyPropertyChanged
    {       
        private string[] _filePaths;
        private List<FileInformation> _files;
        private static object _thLockMe = new object();
	    private readonly ConcurrentQueue<FileInformation> _cq;

        private List<FileInformation> _filesInfo;
        public List<FileInformation> FilesInfo
        {
            get { return _filesInfo; }
            set
            {
                if (value == _filesInfo)
                    return;

                _filesInfo = value;
                OnPropertyChanged("FilesInfo");
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

        public FilesCalculatorViewModel()
        {           
            _files = new List<FileInformation>();
			_cq = new ConcurrentQueue<FileInformation>();
		}
        public void ConfigureFileInfo(string path)
        {
            _filePaths = Directory.GetFiles(path, "*", SearchOption.AllDirectories).OrderBy(p => p).ToArray();

            var tasks = new List<Task>();

            using (var md5 = MD5.Create())
            {
                foreach (var filePath in _filePaths)
                {
                    tasks.Add(CollectInformation(filePath, md5));

                    tasks.Add(RecordResultsInAnXmlFile());

                    tasks.Add(InputOfResultsIntoTheControl());

                    Task.WaitAll(tasks.ToArray());

                    tasks.Clear();

                    ProgressValue++;
                }

                tasks.Add(RecordResultsInAnXmlFile());

                tasks.Add(InputOfResultsIntoTheControl());

                Task.WaitAll(tasks.ToArray());
            }

        }

        private Task CollectInformation(string filePath, MD5 md5)
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
            });

            return task;
        }

        private Task InputOfResultsIntoTheControl()
        {
            var task = Task.Run(() =>
            {
                FilesInfo = _cq.ToList();
            });

            return task;
        }

        private Task RecordResultsInAnXmlFile()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//SerializationOverview.xml";

            return Serialize(path);
        }

        public Task SerializeAppend(string path)
        {
            var writer = new XmlSerializer(typeof(FileInformation));

            var task = Task.Run(() =>
            {
                FileStream file = File.Open(path, FileMode.Append, FileAccess.Write);

                writer.Serialize(file, _cq.ToList());

                file.Close();
            });

            return task;
        }

        public Task Serialize(string path)
        {

            var writer = new XmlSerializer(typeof(List<FileInformation>));

            var task = Task.Run(() =>
            {
                using (var file = new StreamWriter(path))
                {
                    var i = _cq.ToList();

                    writer.Serialize(file, i);
                }
            });

            return task;
        }

        public event PropertyChangedEventHandler PropertyChanged;

	    private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
