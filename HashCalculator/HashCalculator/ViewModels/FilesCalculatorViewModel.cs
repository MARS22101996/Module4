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

            using (var md5 = MD5.Create())
            {
                //foreach (var filePath in _filePaths)
                //{
                //    var file = CollectInformation(filePath, md5);

                //    RecordResultsInAnXMLFile(file);

                //    InputOfResultsIntoTheControl(file);
                //}          

                CancellationTokenSource cts = new CancellationTokenSource();
                var paralelOptions = new ParallelOptions
                {
                    MaxDegreeOfParallelism = 3,
                    CancellationToken = cts.Token
                };
                Parallel.ForEach(_filePaths, paralelOptions,
                    filePath => 
                        {
                            var file = CollectInformation(filePath, md5);

                            RecordResultsInAnXMLFile(file);

                            InputOfResultsIntoTheControl(file);
                        }
                        );
            }
        }

        private FileInformation CollectInformation(string filePath, MD5 md5)
        {
            var info = new FileInformation();
            lock (thLockMe)
            {
                using (var stream = File.OpenRead(filePath))
                {
                    info.Path = filePath;

                    info.Hash = Encoding.Default.GetString(md5.ComputeHash(stream));

                    info.Length = stream.Length;
                }
            }
           
            return info;
        }

        private void InputOfResultsIntoTheControl(FileInformation file)
        {
            _files.Add(file);

            FilesInfo = _files;
        }

        private void RecordResultsInAnXMLFile(FileInformation fileInfo)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//SerializationOverview.xml";

            if (!File.Exists(path))
            {
                Serialize(fileInfo, path);
            }
            else
            {
                SerializeAppend(fileInfo, path);
            }
        }

        public static void SerializeAppend<FileInformation>(FileInformation obj, string path)
        {
            var writer = new XmlSerializer(typeof(FileInformation));
            lock (thLockMe)
            {
                FileStream file = File.Open(path, FileMode.Append, FileAccess.Write);

                writer.Serialize(file, obj);

                file.Close();
            }
        }

        public static void Serialize<FileInformation>(FileInformation obj, string path)
        {
            var writer = new XmlSerializer(typeof(FileInformation));
            lock (thLockMe)
            {
                using (var file = new StreamWriter(path))
                {
                    writer.Serialize(file, obj);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
