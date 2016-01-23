using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marson.SimpleSearch
{
    public class SearchFileInfo
    {
        public string Path { get; set; }
        public DateTime LastWriteTime { get; set; }
        public long Length { get; set; }
        public string SearchTexts { get; set; }
    }
}
