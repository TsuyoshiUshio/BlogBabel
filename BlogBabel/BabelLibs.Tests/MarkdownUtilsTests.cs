using BabelLibs.Utils;

namespace BabelLibs.Tests
{
    public class MarkdownUtilsTests
    {
        // We can fetch the project User/Project/branch name from the GitHub .git files.
        private const string BASE_URL = "https://raw.githubusercontent.com/TsuyoshiUshio/BlogBabel/main/";
        [Fact]
        public void ConvertPathNormalCase()
        {
            string scenario = nameof(MarkdownUtilsTests);
            string testcase = nameof(ConvertPathNormalCase);
            string unitTestProjectRoot = GetUnitTestProjectRoot();

            string SourceContent = ReadAllText(scenario, testcase, "Source.md");
            string sourcePath = Path.Combine(unitTestProjectRoot, "fixture", scenario, testcase, "Source.md");
            string ExpectedContent = ReadAllText(scenario, testcase, "Expected.md");

            // sourcePath can be relative/absolute path. The test runner runs in the debug directory, so we need to use the absolute path.
            string acttualContent = MarkdownUtils.ConvertPath(SourceContent, sourcePath, $"{BASE_URL}");
            Assert.Equal(ExpectedContent, acttualContent);
        }

        private static string ReadAllText(string scenario, string testcase, string targetPath)
        {

            string unitTestProjectRoot = GetUnitTestProjectRoot();
            string path = Path.Combine(unitTestProjectRoot, "fixture", scenario, testcase, targetPath);
            return File.ReadAllText(path);
        }

        private static string GetUnitTestProjectRoot()
        {
            string projectRoot = MarkdownUtils.GetGitRoot(Directory.GetCurrentDirectory());
            return Path.Combine(projectRoot, "BlogBabel", "BabelLibs.Tests");
        }
    }
}