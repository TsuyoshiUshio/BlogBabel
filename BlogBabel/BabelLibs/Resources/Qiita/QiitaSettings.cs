using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabelLibs.Resources.Qiita
{
    public class QiitaSettings
    {
        public const string SectionName = "Qiita";

        public string PersonalAccessToken { get; set; }

        public string Language { get; set; } = "Japanese";

        public string Sentiment { get; set; } = "Techinical Blog";
    }
}
