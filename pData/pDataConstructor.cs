using System;
using System.Collections.Generic;
using System.Text;

namespace pData
{
    public struct pDataConstructor
    {
        public string Name;
        public string ShortDescription;
        public string ProjectDescription;
        public string ProjectGoal;
        public string ProjectStarted;
        public string YouTubeVideo;

        public Dictionary<string, User> TeamMembers;
        public string[] LangAndInfo;
        public string Demo;
        public string GitSource;

        public string Card;
        public Dictionary<string, string> Images;
    }
}
