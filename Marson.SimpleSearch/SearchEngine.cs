using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Marson.SimpleSearch
{
    public class SearchEngine : ISearchContext
    {
        private string searchForDirectoryPath;
        private string searchForFileName;
        private List<SearchFileInfo> results;

        public List<string> SearchDirectories { get; } = new List<string>();

        public string DirectoryPath { get; set; }
        public string FileName { get; set; }
        public string[] Texts { get; set; }

        public List<SearchFileInfo> Results
        {
            get { return results; }
        }

        public List<SearchHit> HitList { get; private set; }

        public bool IsCancelPending { get; set; }

        public event EventHandler<SearchEventArgs> SearchingPath;
        public event EventHandler<SearchFoundEventArgs> SearchFound;

        public void Search()
        {
            if (SearchDirectories.Count == 0)
                throw new InvalidOperationException("Directory must be set.");

            if (!string.IsNullOrEmpty(DirectoryPath))
                searchForDirectoryPath = DirectoryPath;
            else
                searchForDirectoryPath = null;

            if (!string.IsNullOrEmpty(FileName))
                searchForFileName = FileName;
            else
                searchForFileName = null;

            if (searchForDirectoryPath == null && searchForFileName == null && (Texts == null || Texts.Length > 0))
                throw new InvalidOperationException("At least one of DirectoryPath, FileName or Texts must be set.");

            var listSeparator = Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator;
            string missingDirectories = string.Join(listSeparator, SearchDirectories.Where(s => !Directory.Exists(s)));
            if (!string.IsNullOrEmpty(missingDirectories))
            {
                throw new InvalidOperationException(string.Format("The following directories do not exist: {0}", missingDirectories));
            }

            IsCancelPending = false;
            results = new List<SearchFileInfo>();
            HitList = null;
            var resultsLock = new object();

            Parallel.ForEach(SearchDirectories,
                s =>
                {
                    var search = new Search { Context = this };
                    var singleResult = search.Run(s);
                    lock (resultsLock)
                    {
                        results.AddRange(singleResult);
                        var searchHitList = search.HitList;
                        if (searchHitList != null)
                        {
                            if (HitList == null)
                            {
                                HitList = new List<SearchHit>();
                            }
                            HitList.AddRange(searchHitList);
                        }
                    }
                }
            );
        }
    



		private void OnSearchingPath(string path)
		{
			if (SearchingPath != null)
			{
				SearchingPath(this, new SearchEventArgs { Path = path });
			}
		}

		private void OnSearchFound(SearchFileInfo fileInfo)
		{
			if (SearchFound != null)
			{
				SearchFound(this, new SearchFoundEventArgs { FileInfo = fileInfo });
			}
		}

		public void Cancel()
		{
			IsCancelPending = true;
		}

        string ISearchContext.SearchForDirectoryPath
        {
            get
            {
                return searchForDirectoryPath;
            }
        }

        string ISearchContext.SearchForFileName
        {
            get
            {
                return searchForFileName;
            }
        }

        string[] ISearchContext.Texts
        {
            get
            {
                return Texts;
            }
        }

        void ISearchContext.SearchingPath(string path)
        {
            OnSearchingPath(path);
        }

        void ISearchContext.SearchFound(SearchFileInfo fileInfo)
        {
            OnSearchFound(fileInfo);
        }
    }
	
	
	public class SearchEventArgs : EventArgs
	{
		public string Path { get; set; }
	}


	public class SearchFoundEventArgs : EventArgs
	{
		public SearchFileInfo FileInfo { get; set; }
	}

}
