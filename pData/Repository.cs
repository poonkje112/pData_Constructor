using System;
using System.Collections.Generic;
using System.Text;

namespace pData
{
    public class Repository
    {
        string _Name, _Url, _Owner;
        bool _IsPrivate;

        public string Name { get { return _Name; } }
        public string Owner { get { return _Owner; } }
        public string Url { get { return _Url; } }
        public bool IsPrivate { get { return _IsPrivate; } }

        public Repository(string owner, string name, string url, bool isPrivate)
        {
            _Owner = owner;
            _Name = name;
            _Url = url;
            _IsPrivate = isPrivate;
        }
    }
}
