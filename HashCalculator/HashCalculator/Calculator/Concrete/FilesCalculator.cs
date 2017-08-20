using HashCalculator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HashCalculator.Calculator.Concrete
{
    public class FilesCalculator
    {
        private readonly string _directoryPath;
        private string[] _filePaths;
        private List<FileInformation> _files;

        public FilesCalculator(string path)
        {
            _directoryPath = path;
            _filePaths = Directory.GetFiles(_directoryPath, "*", SearchOption.AllDirectories).OrderBy(p => p).ToArray();
            _files = new List<FileInformation>();
        }
        public List<FileInformation> ConfigureFileInfo()
        {         
            using (var md5 = MD5.Create())
            {
                foreach (var filePath in _filePaths)
                {
                    CollectInformation(filePath, md5);
                }

                return _files;
            }
        }

        private void CollectInformation(string filePath, MD5 md5)
        {
            var info = new FileInformation();

            using (var stream = File.OpenRead(filePath))
            {
                info.Path = filePath;

                info.Hash = Encoding.Default.GetString(md5.ComputeHash(stream));

                info.Length = stream.Length;

                _files.Add(info);
            }
        }
    }
}
