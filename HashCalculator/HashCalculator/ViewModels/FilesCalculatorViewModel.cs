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

namespace HashCalculator.ViewModels
{
    public class FilesCalculatorViewModel : INotifyPropertyChanged
    {       
        private string[] _filePaths;
	    private ConcurrentQueue<FileInformation> _cq;

        private List<FileInformation> _filesInfo;
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



        public FilesCalculatorViewModel()
        {
            FilesInfo= new List<FileInformation>();

			_cq = new ConcurrentQueue<FileInformation>(); 
		}
        public void ConfigureFileInfo(string path)
        {
            FilesInfo.Clear();

            _cq = new ConcurrentQueue<FileInformation>();

            ProgressValue = 0;

            _filePaths = Directory.GetFiles(path, "*", SearchOption.AllDirectories).OrderBy(p => p).ToArray();

            ProgressMax = _filePaths.Length;

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
                if (_cq.Count != 0)
                {
                    FilesInfo = _cq.ToList();                                  
                }
            });

            return task;
        }

        private Task RecordResultsInAnXmlFile()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//SerializationOverview.xml";

            return Serialize(path);
        }

	    private Task SerializeAppend(string path)
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

	    private Task Serialize(string path)
        {

            var writer = new XmlSerializer(typeof(List<FileInformation>));

            var task = Task.Run(() =>
            {
                using (var file = new StreamWriter(path))
                {
                    writer.Serialize(file, _cq.ToList());
                }
            });

            return task;
        }

        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
