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

            string SourceContent = ReadAllText(scenario, testcase, "Source.md");
            string sourcePath = Path.Combine("fixture", scenario, testcase, "Source.md");
            string ExpectedContent = ReadAllText(scenario, testcase, "Expected.md");
            string acttualContent = MarkdownUtils.ConvertPath(SourceContent, sourcePath, $"{BASE_URL}");
            Assert.Equal(ExpectedContent, acttualContent);
        }

        private static string ReadAllText(string scenario, string testcase, string targetPath)
        {
            string path = Path.Combine("fixture", scenario, testcase, targetPath);
            return File.ReadAllText(path);
        }
    }
}