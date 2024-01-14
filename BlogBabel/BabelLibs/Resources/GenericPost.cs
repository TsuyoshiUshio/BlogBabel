using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabelLibs.Resources
{
    public class GenericPost
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public IList<string> Tags { get; set; } = new List<string>();
    }
}
