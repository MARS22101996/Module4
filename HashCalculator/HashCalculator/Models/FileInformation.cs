using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashCalculator.Models
{
    public class FileInformation
    {
        public string Path { get; set; }
        public double Length { get; set; }
        public string Hash { get; set; }
    }
}
