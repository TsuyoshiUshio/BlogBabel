using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static BabelLibs.Utils.GitConfig;

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
                Remote = GetRemoteSections(data),
                Branch = GetBranchSections(data)
            };
        }

        private static Dictionary<string, RemoteSection> GetRemoteSections(IniData data)
        {
            var remoteSections = new Dictionary<string, RemoteSection>();
            foreach (var section in data.Sections)
            {
                if (section.SectionName.StartsWith("remote"))
                {
                    var match = Regex.Match(section.SectionName, "remote \"(.*)\"");
                    var remoteName = match.Groups[1].Value;
                    remoteSections.Add(remoteName, new RemoteSection
                    {
                        Url = data[section.SectionName]["url"],
                        Fetch = data[section.SectionName]["fetch"]
                    });
                }
            }
            return remoteSections;
        }

        private static Dictionary<string, BranchSection> GetBranchSections(IniData data)
        {
            var branchSections = new Dictionary<string, BranchSection>();
            foreach (var section in data.Sections)
            {
                if (section.SectionName.StartsWith("branch"))
                {
                    var match = Regex.Match(section.SectionName, "branch \"(.*)\"");
                    var branchName = match.Groups[1].Value;
                    branchSections.Add(branchName, new BranchSection
                    {
                        Remote = data[section.SectionName]["remote"],
                        Merge = data[section.SectionName]["merge"]
                    });
                }
            }
            return branchSections;
        }
    }
}
