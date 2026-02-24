//@BaseCode
namespace TemplateTools.ConApp.Modules
{
    using System.Text;
    using TemplateTools.Logic;

    internal class CodeImporter
    {
        /// <summary>
        /// Creates C# class files from the provided import code into the target solution.
        /// For each namespace found in the import code, extracts the class code, determines the target namespace and file path,
        /// and writes the class to a file if allowed (based on the force flag and file existence).
        /// The imported class code is normalized and wrapped in the appropriate namespace.
        /// </summary>
        /// <param name="codeSolutionPath">The root path of the target code solution.</param>
        /// <param name="importCode">The code string containing one or more class definitions to import.</param>
        /// <param name="force">
        /// If true, allows overwriting existing files that contain the ChatGptCodeLabel in the first line;
        /// otherwise, only writes files that do not already exist.
        /// </param>
        public static void ImportCSharpItems(string codeSolutionPath, string importCode, bool force)
        {
            var sourceSolutionName = TemplatePath.GetSolutionName(codeSolutionPath);

            if (sourceSolutionName.HasContent() && importCode.HasContent())
            {
                var itemTags = importCode.GetAllTags("//<CSharpItem>", "//</CSharpItem>");

                foreach (var tag in itemTags)
                {
                    var headerTags = tag.InnerText.GetAllTags("<Header>", "</Header>");
                    var fileNameTag = tag.InnerText.GetAllTags("<Filename>", "</Filename").FirstOrDefault();
                    var codeTag = tag.InnerText.GetAllTags("//<Code>", "//</Code>").FirstOrDefault();

                    if (codeTag != default)
                    {
                        var sourceCode = new StringBuilder();
                        var importNamespace = codeTag.FullText.Betweenstring("namespace", "{").RemoveLeftAndRight(' ');
                        var importSoltionName = CodeBase.GetSolutionNameFromNamespace(importNamespace);
                        var projectNamespace = CodeBase.ConvertToSourceProjectNamespace(importNamespace, sourceSolutionName);
                        var subNamespace = CodeBase.GetSubNamespace(importNamespace);
                        var className = CodeBase.ClearClassName(codeTag.InnerText.ExtractBetween(" class ", "{"));
                        var fileName = fileNameTag != default ? CodeBase.ClearFileName(fileNameTag.InnerText) : $"{className}{StaticLiterals.CSharpFileExtension}";
                        var targetPath = Path.Combine(codeSolutionPath, projectNamespace, subNamespace.Replace('.', Path.DirectorySeparatorChar));
                        var targetFilePath = Path.Combine(targetPath, fileName);

                        foreach (var item in headerTags)
                        {
                            sourceCode.AppendLine(item.InnerText);
                        }

                        sourceCode.Append(codeTag.InnerText);
                        sourceCode.Replace($"{importSoltionName}.", $"{sourceSolutionName}.");

                        if (File.Exists(targetFilePath) == false || force)
                        {
                            CodeBase.WriteAllTextToFile(targetFilePath, sourceCode.ToString());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates or updates HTML form files in the Angular project based on the provided import code.
        /// For each HTML form block found in the import code (delimited by <!--HtmlBegin--> and <!--HtmlEnd-->),
        /// extracts the target file name and form content, locates the corresponding file in the Angular project,
        /// and writes the form content to the file if allowed (based on the force flag and file existence).
        /// The written file is prefixed with a ChatGptCodeLabel comment for identification.
        /// </summary>
        /// <param name="codeSolutionPath">The root path of the target code solution.</param>
        /// <param name="importCode">The code string containing one or more HTML form definitions to import.</param>
        /// <param name="force">
        /// If true, allows overwriting existing files that contain the ChatGptCodeLabel in the first line;
        /// otherwise, only writes files that do not already exist.
        /// </param>
        public static void ImportHtmlForms(string codeSolutionPath, string importCode, bool force)
        {
            var sourceSolutionName = TemplatePath.GetSolutionName(codeSolutionPath);

            if (sourceSolutionName.HasContent() && importCode.HasContent())
            {
                var tags = importCode.GetAllTags("<!--HtmlBegin-->", "<!--HtmlEnd-->");
                var projectPath = Path.Combine(codeSolutionPath, $"{sourceSolutionName}{Common.StaticLiterals.AngularExtension}");

                foreach (var tag in tags)
                {
                    var fileTag = tag.InnerText.GetAllTags("<!--Filename:", "-->").FirstOrDefault();
                    var fileName = ConvertFileItem(fileTag?.InnerText ?? string.Empty);
                    var filePath = FindFileInPath(projectPath, fileName);

                    if (string.IsNullOrEmpty(filePath) == false)
                    {
                        var formTag = tag.InnerText.GetAllTags("<!--FormBegin-->", "<!--FormEnd-->").FirstOrDefault();

                        if (formTag != null && CodeBase.CanWriteFile(filePath, force))
                        {
                            CodeBase.WriteAllTextToFile(filePath, formTag.InnerText);
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"File {fileName} not found in path {projectPath}.");
                    }
                }
            }
        }

        /// <summary>
        /// Creates a CSV data file from the provided import data.
        /// The method expects the importData string to contain a line starting with "#Filename:"
        /// which specifies the name of the CSV file to be created. The method extracts the filename,
        /// determines the target directory based on the solution path and solution name, and writes
        /// all lines except those starting with "#Filename:", "#CsvBegin", or "#CsvEnd" to the file
        /// in UTF-8 encoding without BOM. If the target directory does not exist, it is created.
        /// </summary>
        /// <param name="codeSolutionPath">The path to the code solution directory.</param>
        /// <param name="importData">The import data string containing the CSV content and filename tag.</param>
        public static void ImportCsvData(string codeSolutionPath, string importData)
        {
            var fileNameTag = "#Filename:";
            var lines = importData.NormalizeLineEndings().Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries);
            var fileNameLine = lines.FirstOrDefault(l => l.StartsWith(fileNameTag));
            var fileName = fileNameLine != default ? CodeBase.ClearFileName(fileNameLine.Replace(fileNameTag, string.Empty)) : string.Empty;

            if (fileName.HasContent())
            {
                var sourceSolutionName = TemplatePath.GetSolutionName(codeSolutionPath);
                var path = Path.Combine(codeSolutionPath, $"{sourceSolutionName}{CommonStaticLiterals.ConsoleExtension}", "data");
                var filePath = Path.Combine(path, fileName);

                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }
                var utf8OhneBOM = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
                var dataLines = lines.Where(l => l.StartsWith("#DataSetBegin") == false
                                              && l.StartsWith(fileNameTag) == false
                                              && l.StartsWith("#CsvBegin") == false
                                              && l.StartsWith("#CsvEnd") == false
                                              && l.StartsWith("#DataSetEnd") == false);

                File.WriteAllLines(filePath, dataLines, utf8OhneBOM);
            }
        }
        /// <summary>
        /// Imports file data from a string into a specified directory within the code solution path.
        /// The method expects the importData string to contain a line starting with "Filename:"
        /// which specifies the name of the file to be created. It extracts the filename, determines
        /// the target directory based on the provided codeSolutionPath, and writes all lines except
        /// those starting with "Filename:" to the file in UTF-8 encoding without BOM.
        /// If the target directory does not exist, it is created.
        /// </summary>
        /// <param name="codeSolutionPath">The path to the code solution directory.</param>
        /// <param name="importData">The import data string containing the file content and filename
        public static void ImportFileData(string codeSolutionPath, string importData)
        {
            var fileNameTag = "Filename:";
            var lines = importData.NormalizeLineEndings().Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries);
            var fileNameLine = lines.FirstOrDefault(l => l.Contains(fileNameTag));
            var fileName = string.Empty;

            if (fileNameLine != null)
            {
                var startIdx = fileNameLine.IndexOf(fileNameTag);

                startIdx = startIdx != -1 ? startIdx + fileNameTag.Length : startIdx;
                fileName = fileNameLine.Partialstring(startIdx, fileNameLine.Length);
                fileName = CodeBase.ClearFileName(fileName);
            }

            if (fileName.HasContent())
            {
                var path = Path.Combine(codeSolutionPath);
                var filePath = Path.Combine(path, fileName);

                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }
                var utf8OhneBOM = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
                var dataLines = lines.Where(l => l.Contains(fileNameTag) == false);

                File.WriteAllLines(filePath, dataLines, utf8OhneBOM);
            }
        }

        /// <summary>
        /// Converts a file item into a normalized format.
        /// </summary>
        /// <param name="fileItem">The file item to be converted.</param>
        /// <returns>A string representing the normalized file item.</returns>
        public static string ConvertFileItem(string fileItem)
        {
            var result = new StringBuilder();

            foreach (var item in fileItem)
            {
                if (result.Length == 0)
                {
                    result.Append(char.ToLower(item));
                }
                else if (item == '\\')
                {
                    result.Append('/');
                }
                else if (char.IsUpper(item))
                {
                    if (result[^1] != '-' && result[^1] != '/' && result[^1] != '\\')
                    {
                        result.Append('-');
                    }
                    result.Append(char.ToLower(item));
                }
                else
                {
                    result.Append(char.ToLower(item));
                }
            }
            return result.ToString();
        }
        /// <summary>
        /// Searches for the first occurrence of a file with the specified name within the given directory path and all its subdirectories.
        /// </summary>
        /// <param name="pfad">The root directory path in which to search for the file.</param>
        /// <param name="fileName">The name of the file to search for.</param>
        /// <returns>
        /// The full path to the first file found with the specified name, or <c>null</c> if no such file exists in the directory tree.
        /// </returns>
        public static string? FindFileInPath(string pfad, string fileName)
        {
            // Gibt das erste Vorkommen zurück (oder null, falls nicht gefunden)
            var dateien = Directory.GetFiles(pfad, fileName, SearchOption.AllDirectories);

            return dateien.Length > 0 ? dateien[0] : null;
        }
    }
}
