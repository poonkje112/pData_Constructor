using System;
using System.Collections.Generic;
using System.Text;

namespace pData
{
    public class Repository
    {
        string _Name, _FullName, _Url, _Owner, _RepoUrl;
        bool _IsPrivate;

        public string Name { get { return _Name; } }
        public string FullName { get { return _FullName; } }
        public string Owner { get { return _Owner; } }
        public string Url { get { return _Url; } }
        public string RepoUrl {  get { return _RepoUrl;  } }
        public bool IsPrivate { get { return _IsPrivate; } }

        public Repository(string owner, string name, string url, string repoUrl, bool isPrivate)
        {
            _Owner = owner;
            _Name = name;
            _FullName = $"{owner}/{name}";
            _Url = url;
            _RepoUrl = repoUrl;
            _IsPrivate = isPrivate;
        }
    }
}
