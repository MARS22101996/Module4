using HashCalculator.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace HashCalculator.Calculator.Concrete
{
    public class FilesCalculatorViewModel : INotifyPropertyChanged
    {       
        private string[] _filePaths;
        private List<FileInformation> _files;
        private static object thLockMe = new object();
        private FileInformation _file;

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

        public FilesCalculatorViewModel()
        {           
            _files = new List<FileInformation>();
        }
        public void ConfigureFileInfo(string path)
        {
            _filePaths = Directory.GetFiles(path, "*", SearchOption.AllDirectories).OrderBy(p => p).ToArray();

            var tasks = new List<Task>();

            using (var md5 = MD5.Create())
            {
                foreach (var filePath in _filePaths)
                {
                    //var task = Task.Run(() =>
                    //{
                    //    lock (thLockMe)
                    //    {
                    //        _file = CollectInformation(filePath, md5);
                    //    }
                    //});

                    //tasks.Add(task);


                    _file = CollectInformation(filePath, md5);

                    tasks.Add(RecordResultsInAnXMLFile(_file));

                    tasks.Add(InputOfResultsIntoTheControl(_file));

                    Task.WaitAll(tasks.ToArray());
                }
               
            }
        }

        private FileInformation CollectInformation(string filePath, MD5 md5)
        {

            var info = new FileInformation();

                using (var stream = File.OpenRead(filePath))
                {
                    info.Path = filePath;

                    info.Hash = Encoding.Default.GetString(md5.ComputeHash(stream));

                    info.Length = stream.Length;
                }
          
            return info;
        }

        private Task InputOfResultsIntoTheControl(FileInformation file)
        {
            var task = Task.Run(() =>
            {
                _files.Add(file);

                FilesInfo = _files;
            });

            return task;
        }

        private Task RecordResultsInAnXMLFile(FileInformation fileInfo)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//SerializationOverview.xml";

            if (!File.Exists(path))
            {
               return Serialize(fileInfo, path);
            }
            else
            {
               return SerializeAppend(fileInfo, path);
            }
        }

        public Task SerializeAppend<FileInformation>(FileInformation obj, string path)
        {
            var writer = new XmlSerializer(typeof(FileInformation));

            var task = Task.Run(() =>
            {
                FileStream file = File.Open(path, FileMode.Append, FileAccess.Write);

                writer.Serialize(file, obj);

                file.Close();
            });

            return task;
        }

        public Task Serialize<FileInformation>(FileInformation obj, string path)
        {

            var writer = new XmlSerializer(typeof(FileInformation));

            var task = Task.Run(() =>
            {
                using (var file = new StreamWriter(path))
                {
                    writer.Serialize(file, obj);
                }
            });

            return task;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
