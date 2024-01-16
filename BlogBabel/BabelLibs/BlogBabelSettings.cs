using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabelLibs
{
    public class BlogBabelSettings
    {
        public int TokenLimit { get; set; } = 2000;
        public int MaxTokenLimit { get; set; } = 3000;

        public string Model { get; set; } = "gpt3.5-turbo";
    }
}
