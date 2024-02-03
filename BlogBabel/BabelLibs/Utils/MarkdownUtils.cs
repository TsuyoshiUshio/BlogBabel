using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BabelLibs.Utils
{
    public class MarkdownUtils
    {
        private const string pattern = @"\!\[.*?\]\((.*?)\)";
        public static string ConvertPath(string sourceContent, string sourcePath, string basePath)
        {
            var evaluator = new MatchEvaluator(m =>
            {                
                string originalHeader = m.Groups[0].Value;
                int index = originalHeader.IndexOf("](");
                string sourceFullPath = Path.GetFullPath(sourcePath);
                
                string currentPath = Directory.GetCurrentDirectory();
                string projectRoot = GitHubUtils.GetGitRoot(sourceFullPath);
                
                string imageRelativePath = m.Groups[1].Value;

                // Finding the full path of the Source.md file.
                var sourceFileInfo = new FileInfo(sourceFullPath);
                string sourceFileDirectory = sourceFileInfo.DirectoryName;
                
                // Finding the full path of the image file.
                string imageFullPath = Path.Combine(sourceFileDirectory, imageRelativePath);
                int sentenceIndex = imageFullPath.IndexOf(projectRoot);

                // Finding the relative path from the project root.
                string relativePath = imageFullPath.Substring(sentenceIndex + projectRoot.Length + 1);
                string imageFullPathOnGitHub = $"{basePath}{relativePath}".Replace("\\", "/");

                string header = originalHeader.Substring(0, index + 1);
                string replacement = $"{header}({imageFullPathOnGitHub})";
                return replacement;
            });
            return Regex.Replace(sourceContent, pattern, evaluator);
        }
    }
}
