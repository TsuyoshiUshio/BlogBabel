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
        [Theory]
        [InlineData("normal_config")]
        [InlineData("multiple_config")]
        public void GetProfile(string configName)
        {
            var gitProfile = GitHubUtils.ParseGitConfig(Path.Combine("fixture", nameof(GitHubUtilsTests), nameof(GetProfile), configName));
            Assert.Equal("refs/heads/main", gitProfile.Branch["main"].Merge);
            Assert.Equal("https://github.com/TsuyoshiUshio/BlogBabel.git", gitProfile.Remote["origin"].Url);
        }

        [Fact]
        public void GetCurrentBranch()
        {
            var currentBranch = GitHubUtils.GetCurrentBranch(Path.Combine("fixture", nameof(GitHubUtilsTests), nameof(GetCurrentBranch), "HEAD"));
            Assert.Equal("main", currentBranch);
        }

        [Fact]
        public void GetGitHubRoot()
        {
            var gitHubRoot = GitHubUtils.GetGitHubRoot(Path.Combine("fixture", nameof(GitHubUtilsTests), nameof(GetGitHubRoot)));
            Assert.Equal("https://raw.githubusercontent.com/TsuyoshiUshio/BlogBabel/main/", gitHubRoot);
        }
    }
}
