using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Marson.SimpleSearch
{
    public class Search
    {
        private string startingSearchDir;
        private List<SearchHit> searchTexts;

        public ISearchContext Context { get; set; }

        public ReadOnlyCollection<SearchHit> HitList
        {
            get
            {
                if (searchTexts != null)
                {
                    return searchTexts.AsReadOnly();
                }
                else
                {
                    return null;
                }
            }
        }

        public List<SearchFileInfo> Run(string searchDir)
        {
            if (Context.Texts != null && Context.Texts.Length > 0)
            {
                searchTexts = new List<SearchHit>();
                foreach (string text in Context.Texts)
                {
                    searchTexts.Add(new SearchHit { Text = text, Hits = 0 });
                }
            }
            else
                searchTexts = null;

            startingSearchDir = searchDir;
            return DoSearch(searchDir);
        }

        public List<SearchFileInfo> DoSearch(string currentSearchDir)
        {
            Context.SearchingPath(currentSearchDir);
            List<SearchFileInfo> foundFiles = new List<SearchFileInfo>();

            bool checkFiles;

            if (Context.SearchForDirectoryPath != null)
            {
                // Check if directory path below the initial search path is found
                string subPath = currentSearchDir.Substring(startingSearchDir.Length);
                checkFiles = Wildcard.Match(Context.SearchForDirectoryPath, subPath);
            }
            else
            {
                checkFiles = true;
            }

            if (checkFiles)
            {
                string[] files = null;

                // Files in this directory
                try
                {
                    if (Context.SearchForFileName != null)
                        files = Directory.GetFiles(currentSearchDir, Context.SearchForFileName);
                    else
                        files = Directory.GetFiles(currentSearchDir);
                }
                catch (UnauthorizedAccessException)
                {
                }

                if (files != null)
                {
                    foreach (string file in files)
                    {
                        if (Context.IsCancelPending)
                            return foundFiles;

                        string searchTextHits;
                        if (searchTexts != null)
                        {
                            Context.SearchingPath(file);
                            if (SearchForTextInFile(file, searchTexts.Count > 1, out searchTextHits))
                            {
                                SearchFileInfo fileInfo = CreateSearchFileInfo(file, searchTextHits);
                                foundFiles.Add(fileInfo);
                                Context.SearchFound(fileInfo);
                            }
                        }
                        else
                        {
                            SearchFileInfo fileInfo = CreateSearchFileInfo(file, null);
                            foundFiles.Add(fileInfo);
                            Context.SearchFound(fileInfo);
                        }
                    }
                }
            }

            // Directories in this directory
            string[] dirs = null;
            try
            {
                dirs = Directory.GetDirectories(currentSearchDir);
            }
            catch (UnauthorizedAccessException)
            {
            }

            if (dirs != null)
            {
                foreach (string dir in dirs)
                {
                    if (Context.IsCancelPending)
                        return foundFiles;

                    foundFiles.AddRange(DoSearch(dir));
                }
            }

            return foundFiles;
        }

        private bool SearchForTextInFile(string filePath, bool gatherHits, out string searchTextHits)
        {
            searchTextHits = null;
            Dictionary<string, string> searchTextHitsDictionary = new Dictionary<string, string>();

            bool found = false;
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        foreach (SearchHit hit in searchTexts)
                        {
                            if (line.IndexOf(hit.Text, StringComparison.CurrentCultureIgnoreCase) >= 0)
                            {
                                if (!gatherHits)
                                    return true;

                                hit.Hits++;
                                searchTextHitsDictionary[hit.Text] = null;
                                found = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is IOException || ex is OutOfMemoryException)
                {
                }
                else
                    throw;
            }

            if (searchTextHitsDictionary.Count > 0)
            {
                searchTextHits = (new MultilineText(searchTextHitsDictionary.Keys.ToArray())).Text;
            }
            return found;
        }

        private SearchFileInfo CreateSearchFileInfo(string file, string searchTextHits)
        {
            FileInfo fileInfo = new FileInfo(file);
            SearchFileInfo searchFileInfo = new SearchFileInfo();
            searchFileInfo.Path = file;
            searchFileInfo.LastWriteTime = fileInfo.LastWriteTime;
            searchFileInfo.Length = fileInfo.Length;
            searchFileInfo.SearchTexts = searchTextHits;

            return searchFileInfo;
        }

    }
}
