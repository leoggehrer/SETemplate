//@BaseCode
namespace TemplateTools.ConApp.Modules
{
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text;
    public partial class CodeCreator
    {
        /// <summary>
        /// Imports C# class code and CSV data into the target solution's ConApp project.
        /// For each namespace block in the import code, extracts the class code, determines the target file path,
        /// and writes the class to a file if allowed (based on the force flag and file existence).
        /// The class code is wrapped in a conditional compilation block and the appropriate namespace.
        /// For each #DataSetBegin block, extracts the CSV data and writes it to a .csv file in the data folder
        /// if allowed (based on the force flag and file existence).
        /// All generated files are prefixed with a ChatGptCodeLabel for identification.
        /// </summary>
        /// <param name="codeSolutionPath">The root path of the target code solution.</param>
        /// <param name="importCode">The code string containing one or more class and/or CSV data definitions to import.</param>
        /// <param name="force">
        /// If true, allows overwriting existing files that contain the ChatGptCodeLabel in the first line;
        /// otherwise, only writes files that do not already exist.
        /// </param>
        public static void CreateImportWithData(string codeSolutionPath, string importCode, bool force)
        {
            var sourceSolutionName = TemplatePath.GetSolutionName(codeSolutionPath);

            if (sourceSolutionName.HasContent() && importCode.HasContent())
            {
                var codeTags = importCode.GetAllTags("namespace ", "{");

                foreach (var tag in codeTags)
                {
                    var conAppProject = $"{sourceSolutionName}.ConApp";
                    var importSolutionName = CreateParentNamespace(tag.FullText, "ConApp");
                    var classCode = importCode.ExtractBetween('{', '}', tag.StartTagIndex).Replace(importSolutionName, sourceSolutionName);
                    var className = classCode.ExtractBetween("partial class ", ":").RemoveLeftAndRight(' ');
                    var fileName = classCode.ExtractBetween("Filename:", ".cs").RemoveLeftAndRight(' ');
                    var targetPath = Path.Combine(codeSolutionPath, conAppProject);
                    var targetFilePath = Path.Combine(targetPath, $"{(fileName.HasContent() ? fileName : className)}.cs");
                    var fullSourceCode = $"//{Common.StaticLiterals.AiCodeLabel}{Environment.NewLine}";

                    fullSourceCode += $"#if GENERATEDCODE_ON{Environment.NewLine}";
                    fullSourceCode += $"namespace {conAppProject}{Environment.NewLine}" + '{' + $"{classCode.NormalizeLineEndings()}" + '}' + $"{Environment.NewLine}#endif";

                    if (CanWriteFile(targetFilePath, force))
                    {
                        WriteAllTextToFile(targetFilePath, fullSourceCode);
                    }
                }

                var dataSetTags = importCode.GetAllTags("#DataSetBegin", "#DataSetEnd");

                foreach (var tag in dataSetTags)
                {
                    var dataSetTag = tag.InnerText.NormalizeLineEndings().GetAllTags($"#CsvBegin{Environment.NewLine}", "#CsvEnd").FirstOrDefault();

                    if (dataSetTag != default)
                    {
                        var conAppProject = $"{sourceSolutionName}.ConApp";
                        var fileName = tag.InnerText.ExtractBetween("Filename:", ".csv").RemoveLeftAndRight(' ');
                        var targetPath = Path.Combine(codeSolutionPath, conAppProject);
                        var targetFilePath = Path.Combine(targetPath, "data", $"{fileName}.csv");
                        var fullCsvData = $"#{Common.StaticLiterals.AiCodeLabel}{Environment.NewLine}";

                        fullCsvData += dataSetTag.InnerText.NormalizeLineEndings();

                        if (CanWriteFile(targetFilePath, force))
                        {
                            WriteAllTextToFile(targetFilePath, fullCsvData);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generates a temporary HTML preview file from the provided import code and opens it in the default browser.
        /// The method extracts all HTML form blocks (delimited by &lt;!--HtmlBegin--&gt; and &lt;!--HtmlEnd--&gt;) from the import code,
        /// collects their form content (between &lt;!--FormBegin--&gt; and &lt;!--FormEnd--&gt;), and inserts them into a Bootstrap-based HTML template.
        /// The resulting HTML is written to a temporary file and opened using the system's default browser.
        /// </summary>
        /// <param name="importCode">The code string containing one or more HTML form definitions to preview.</param>
        public static void CreateAndOpenHtmlPreview(string importCode)
        {
            string previewHtml = @"
            <!DOCTYPE html>
            <html lang=""de"">
            <head>
                <meta charset=""UTF-8"">
                <title>Bootstrap Demo</title>
                <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
                          
                <!-- Bootstrap CSS -->
                <link href=""https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"" rel=""stylesheet"">
                <!-- Bootstrap Icons -->
                <link href=""https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css"" rel=""stylesheet"">
            </head>
            <body class=""bg-light"">

                <div class=""container my-5"">
                <!-- Hier dein Formular oder deine Tabelle einfügen -->
                </div>

                <!-- Bootstrap JS -->
                <script src=""https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js""></script>
            </body>
            </html>
            ";

            var htmlCode = string.Empty;

            if (importCode.HasContent())
            {
                var tags = importCode.GetAllTags("<!--HtmlBegin-->", "<!--HtmlEnd-->");

                foreach (var tag in tags)
                {
                    var fileTag = tag.InnerText.GetAllTags("<!--Filename:", "-->").FirstOrDefault();
                    var fileName = ConvertFileItem(fileTag?.InnerText ?? string.Empty);
                    var formTag = tag.InnerText.GetAllTags("<!--FormBegin-->", "<!--FormEnd-->").FirstOrDefault();

                    if (formTag != null)
                    {
                        htmlCode += $"<h2>{fileName}</h2>{Environment.NewLine}";
                        htmlCode += $"<hr >{Environment.NewLine}";
                        htmlCode += $"<div style=\"margin-top: 50px; margin-bottom: 40px;\">{Environment.NewLine}";
                        htmlCode += formTag.InnerText.NormalizeLineEndings();
                        htmlCode += $"</div>{Environment.NewLine}";
                    }
                }
            }
            previewHtml = previewHtml.Replace("<!-- Hier dein Formular oder deine Tabelle einfügen -->", htmlCode);

            var tempPath = Path.GetTempPath();
            var outputFilePath = Path.Combine(tempPath, "preview.html");
            var utf8OhneBOM = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
            File.WriteAllText(outputFilePath, previewHtml, utf8OhneBOM);

            OpenBrowser(outputFilePath);
        }

        #region helpers
        /// <summary>
        /// Opens a html file in the default browser for the current operating system.
        /// </summary>
        /// <param name="filePath">The full path to the html file to open.</param>
        /// <exception cref="FileNotFoundException">Thrown if the specified file does not exist.</exception>
        /// <exception cref="PlatformNotSupportedException">Thrown if the operating system is not supported.</exception>
        public static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    Process.Start("xdg-open", url);
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    Process.Start("open", url);
                else
                    throw;
            }
        }

        /// <summary>
        /// Creates the parent namespace string from the given full namespace, stopping at the specified item.
        /// Splits the full namespace by '.', and collects each item until <paramref name="toItem"/> is found.
        /// Each item is cleaned of non-letter/digit characters.
        /// </summary>
        /// <param name="fullNamespace">The full namespace string to process.</param>
        /// <param name="toItem">The namespace item at which to stop collecting parent namespace items.</param>
        /// <returns>The parent namespace string up to (but not including) <paramref name="toItem"/>.</returns>
        public static string CreateParentNamespace(string fullNamespace, string toItem)
        {
            var start = true;
            var result = new List<string>();
            var items = fullNamespace.Replace("namespace", string.Empty)
                                     .Split('.', StringSplitOptions.RemoveEmptyEntries)
                                     .Select(ni => ClearNamespaceItem(ni));

            foreach (var item in items)
            {
                if (start && item == toItem)
                {
                    start = false;
                }
                if (start)
                {
                    result.Add(item);
                }
            }
            return string.Join(".", result);
        }
        /// <summary>
        /// Removes all non-letter and non-digit characters from the given namespace item string.
        /// </summary>
        /// <param name="item">The namespace item to clean.</param>
        /// <returns>A string containing only letters and digits from the input.</returns>
        public static string ClearNamespaceItem(string item)
        {
            return string.Concat(item.RemoveLeftAndRight(' ').Where(char.IsLetterOrDigit));
        }

        /// <summary>
        /// Determines whether a file can be written to, based on its existence and a force flag.
        /// If the file does not exist, returns true. If the file exists and <paramref name="force"/> is true,
        /// checks if the first line contains the ChatGptCodeLabel and allows overwrite if so.
        /// </summary>
        /// <param name="filePath">The path of the file to check.</param>
        /// <param name="force">If true, allows overwriting files containing the ChatGptCodeLabel.</param>
        /// <returns>True if the file can be written to; otherwise, false.</returns>
        public static bool CanWriteFile(string filePath, bool force)
        {
            var result = File.Exists(filePath) == false;

            if (result == false && force)
            {
                var lines = File.ReadAllLines(filePath, Encoding.Default);

                if (lines.Length > 0
                    && (lines.First().Contains(Common.StaticLiterals.AiCodeLabel)
                        || lines.First().Contains(Common.StaticLiterals.GeneratedCodeLabel)))
                {
                    result = true;
                }
            }
            return result;
        }
        /// <summary>
        /// Writes the specified text to a file at the given file path using UTF-8 encoding without BOM.
        /// If the directory does not exist, it is created.
        /// </summary>
        /// <param name="filePath">The full path of the file to write to.</param>
        /// <param name="text">The text content to write to the file.</param>
        public static void WriteAllTextToFile(string filePath, string text)
        {
            var path = Path.GetDirectoryName(filePath);

            if (string.IsNullOrEmpty(path) == false)
            {
                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }
                var utf8OhneBOM = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

                File.WriteAllText(filePath, text, utf8OhneBOM);
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
        #endregion helpers
    }
}
