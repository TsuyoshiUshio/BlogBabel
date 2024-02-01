using IniParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabelLibs.Utils
{
    public class GitHubUtils
    {
        public static GitConfig ParseGitConfig(string filePath)
        {
            var parser = new FileIniDataParser();
            var data = parser.ReadFile(filePath);

            return new GitConfig
            {
                Core = new GitConfig.CoreSection
                {
                    RepositoryFormatVersion = int.Parse(data["core"]["repositoryformatversion"]),
                    FileMode = bool.Parse(data["core"]["filemode"]),
                    Bare = bool.Parse(data["core"]["bare"]),
                    LogAllRefUpdates = bool.Parse(data["core"]["logallrefupdates"]),
                    Symlinks = bool.Parse(data["core"]["symlinks"]),
                    IgnoreCase = bool.Parse(data["core"]["ignorecase"])
                },
                Remote = new GitConfig.RemoteSection
                {
                    Url = data["remote \"origin\""]["url"],
                    Fetch = data["remote \"origin\""]["fetch"]
                },
                Branch = new GitConfig.BranchSection
                {
                    Remote = data["branch \"main\""]["remote"],
                    Merge = data["branch \"main\""]["merge"]
                }
            };
        }
    }
}
