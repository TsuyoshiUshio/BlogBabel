using BabelLibs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabelLibs.Tests
{
    public class GitHubUtilsTests
    {
        [Fact]
        public void GetProfile()
        {
            var gitProfile = GitHubUtils.ParseGitConfig(Path.Combine("fixture", nameof(GitHubUtilsTests), nameof(GetProfile), "config"));
            Assert.Equal("refs/heads/main", gitProfile.Branch.Merge);
            Assert.Equal("https://github.com/TsuyoshiUshio/BlogBabel.git", gitProfile.Remote.Url);
        }
    }
}
