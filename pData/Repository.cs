﻿using System;
using System.Collections.Generic;
using System.Text;

namespace pData
{
    public class Repository
    {
        string _Name, _Url, _Owner;
        public string Name { get { return _Name; } }
        public string Owner { get { return _Owner; } }

        public Repository(string owner, string name, string url)
        {
            _Owner = owner;
            _Name = name;
            _Url = url;
        }
    }
}