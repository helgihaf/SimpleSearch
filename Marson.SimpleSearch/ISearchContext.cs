using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marson.SimpleSearch
{
    public interface ISearchContext
    {
        string SearchForDirectoryPath { get; }
        string SearchForFileName { get; }
        string[] Texts { get; }

        bool IsCancelPending { get; }

        void SearchingPath(string path);
        void SearchFound(SearchFileInfo fileInfo);
    }
}
