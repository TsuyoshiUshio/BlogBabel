using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabelLibs.Utils
{
    public class GitConfig
    {
        public CoreSection Core { get; set; }
        public IDictionary<string, RemoteSection> Remote { get; set; }
        public IDictionary<string, BranchSection> Branch { get; set; }

        public class CoreSection
        {
            public int RepositoryFormatVersion { get; set; }
            public bool FileMode { get; set; }
            public bool Bare { get; set; }
            public bool LogAllRefUpdates { get; set; }
            public bool Symlinks { get; set; }
            public bool IgnoreCase { get; set; }
        }

        public class RemoteSection
        {
            public string Url { get; set; }
            public string Fetch { get; set; }
        }

        public class BranchSection
        {
            public string Remote { get; set; }
            public string Merge { get; set; }
        }
    }
}
