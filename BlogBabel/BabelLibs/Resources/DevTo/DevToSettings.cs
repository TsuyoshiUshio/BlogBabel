using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabelLibs.Resources.DevTo
{
    public class DevToSettings
    {
        public const string SectionName = "DevTo";
        public string ApiKey { get; set; }
        public string Language { get; set; } = "English";

        public string Sentiment { get; set; } = "Techinical Blog";
    }
}
