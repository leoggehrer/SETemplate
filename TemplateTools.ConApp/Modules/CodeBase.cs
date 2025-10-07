//@BaseCode
namespace TemplateTools.ConApp.Modules
{
    using System.Text;
    internal static partial class CodeBase
    {
        /// <summary>
        /// A dictionary mapping file extensions to their respective single-line comment symbols.
        /// </summary>
        public static readonly Dictionary<string, string> LineCommentSymbolsByExtension = new(StringComparer.OrdinalIgnoreCase)
            {
                { ".cs",   "//" },
                { ".java", "//" },
                { ".js",   "//" },
                { ".ts",   "//" },
                { ".cpp",  "//" },
                { ".c",    "//" },
                { ".h",    "//" },
                { ".py",   "#" },
                { ".sh",   "#" },
                { ".rb",   "#" },
                { ".ps1",  "#" },
                { ".bat",  "REM" },
                { ".sql",  "--" },
                { ".ini",  ";" },
                { ".toml", "#" },
                { ".yaml", "#" },
                { ".yml",  "#" },
                { ".html", "<!-- -->" },    // HTML doesn't support single-line comments
                { ".xml",  "<!-- -->" },    // Same for XML
            };

        /// <summary>
        /// Indents the given text by a specified number of indentation levels (4 spaces per level).
        /// </summary>
        public static Func<string, int, string> setIndent = (t, c) => $"{new string(' ', 4 * c)}{t}";
        /// <summary>
        /// Prepends a line comment symbol to the given text and indents it by one level.
        /// </summary>
        public static Func<string, string, string> setComment = (lc, t) => setIndent($"{lc}{t}", 1);

        /// <summary>
        /// Extracts the solution name from a given namespace string.
        /// Splits the namespace by '.', cleans each item, and returns the first item as the solution name.
        /// </summary>
        /// <param name="nameSpace">The namespace string to extract the solution name from.</param>
        /// <returns>The solution name as a string, or an empty string if not found.</returns>
        public static string GetSolutionNameFromNamespace(string nameSpace)
        {
            var result = string.Empty;
            var namespaceItems = nameSpace.Split('.', StringSplitOptions.RemoveEmptyEntries).Select(i => CodeBase.ClearNamespaceItem(i)).ToArray();

            if (namespaceItems.Length > 0)
            {
                result = namespaceItems[0];
            }
            return result;
        }
        /// <summary>
        /// Converts a given namespace to use the specified source solution name as its root.
        /// Splits the namespace by '.', cleans each item, and replaces the first item (solution name)
        /// with <paramref name="sourceSolutionName"/>. The rest of the namespace is preserved.
        /// </summary>
        /// <param name="nameSpace">The original namespace string to convert.</param>
        /// <param name="sourceSolutionName">The solution name to use as the new root of the namespace.</param>
        /// <returns>
        /// The converted namespace string with the new solution name as the root,
        /// or an empty string if the original namespace does not have more than one item.
        /// </returns>
        public static string ConvertToSourceNamespace(string nameSpace, string sourceSolutionName)
        {
            var result = string.Empty;
            var namespaceItems = nameSpace.Split('.', StringSplitOptions.RemoveEmptyEntries).Select(i => CodeBase.ClearNamespaceItem(i)).ToArray();

            if (namespaceItems.Length > 1)
            {
                result = sourceSolutionName + "." + namespaceItems.Skip(1).Aggregate((a, b) => $"{a}.{b}");// importProjectName.Replace(importSolutionName, sourceSolutionName);
            }
            return result;
        }
        /// <summary>
        /// Converts a given namespace to use the specified source solution name as its root,
        /// and returns only the solution and project part of the namespace.
        /// Splits the namespace by '.', cleans each item, and replaces the first item (solution name)
        /// with <paramref name="sourceSolutionName"/>. The result will be the new solution name and the next item (project).
        /// </summary>
        /// <param name="nameSpace">The original namespace string to convert.</param>
        /// <param name="sourceSolutionName">The solution name to use as the new root of the namespace.</param>
        /// <returns>
        /// The converted namespace string with the new solution name as the root and the next item as the project,
        /// or an empty string if the original namespace does not have more than one item.
        /// </returns>
        public static string ConvertToSourceProjectNamespace(string nameSpace, string sourceSolutionName)
        {
            var result = string.Empty;
            var namespaceItems = nameSpace.Split('.', StringSplitOptions.RemoveEmptyEntries).Select(i => CodeBase.ClearNamespaceItem(i)).ToArray();

            if (namespaceItems.Length > 1)
            {
                result = sourceSolutionName + "." + namespaceItems.Skip(1).Take(1).Aggregate((a, b) => $"{a}.{b}");// importProjectName.Replace(importSolutionName, sourceSolutionName);
            }
            return result;
        }
        /// <summary>
        /// Gets the sub-namespace from a given namespace string, skipping the solution and project parts.
        /// Splits the namespace by '.', cleans each item, and returns the portion after the first two items (solution and project).
        /// If the namespace has two or fewer items, returns an empty string.
        /// </summary>
        /// <param name="nameSpace">The full namespace string to process.</param>
        /// <returns>The sub-namespace string after the solution and project, or an empty string if not present.</returns>
        public static string GetSubNamespace(string nameSpace)
        {
            var result = string.Empty;
            var namespaceItems = nameSpace.Split('.', StringSplitOptions.RemoveEmptyEntries).Select(i => CodeBase.ClearNamespaceItem(i)).ToArray();

            if (namespaceItems.Length > 2)
            {
                result = namespaceItems.Skip(2).Aggregate((a, b) => $"{a}.{b}");
            }
            return result;
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
        /// Creates a sub-namespace string from the given full namespace, starting from the specified item.
        /// Splits the full namespace by '.', finds the first occurrence of <paramref name="startItem"/>,
        /// and returns the sub-namespace including and after that item, with each item cleaned of non-letter/digit characters.
        /// </summary>
        /// <param name="fullNamespace">The full namespace string to process.</param>
        /// <param name="startItem">The namespace item to start the sub-namespace from.</param>
        /// <returns>The sub-namespace string starting from <paramref name="startItem"/>.</returns>
        public static string CreateSubNamespace(string fullNamespace, string startItem)
        {
            var start = false;
            var result = new List<string>();
            var items = fullNamespace.Split('.', StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in items)
            {
                if (start == false && item == startItem)
                {
                    start = true;
                }
                if (start)
                {
                    result.Add(ClearNamespaceItem(item));
                }
            }
            return string.Join(".", result);
        }
        /// <summary>
        /// Removes all non-letter and non-digit characters from the given namespace item string.
        /// </summary>
        /// <param name="item">The namespace to clean.</param>
        /// <returns>A string containing only letters and digits from the input.</returns>
        public static string ClearNamespace(string item)
        {
            return string.Concat(item.RemoveLeftAndRight(' ')
                         .Where(c => char.IsLetterOrDigit(c) || c == '.'));
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
        /// Removes all non-letter and non-digit characters from the given class name string.
        /// </summary>
        /// <param name="className">The class name to clean.</param>
        /// <returns>A string containing only letters and digits from the input.</returns>
        public static string ClearClassName(string className)
        {
            var finished = false;
            var result = string.Empty;

            className = className.RemoveLeftAndRight(' ');

            for (int i = 0; i < className.Length && finished == false; i++)
            {
                if (char.IsLetterOrDigit(className[i]) || className[i] == '_')
                {
                    result += className[i];
                }
                else
                {
                    finished = true;
                }
            }
            return result;
        }
        /// <summary>
        /// Removes leading and trailing spaces from the given file name string.
        /// </summary>
        /// <param name="fileName">The file name to clean.</param>
        /// <returns>The file name with leading and trailing spaces removed.</returns>
        public static string ClearFileName(string fileName)
        {
            var result = RemoveUnknownComment(fileName.RemoveLeftAndRight(' '));

            // Ungültige Zeichen vom Betriebssystem abfragen
            char[] invalidChars = Path.GetInvalidFileNameChars();
            // Alle ungültigen Zeichen entfernen
            string cleanName = new([.. result.Where(ch => !invalidChars.Contains(ch))]);

            // Optional: Mehrfache Leerzeichen oder Punkte am Ende entfernen
            cleanName = cleanName.Trim().TrimEnd('.');

            // Sicherstellen, dass der Name nicht leer wird
            if (string.IsNullOrWhiteSpace(cleanName))
                cleanName = "Unbenannt";

            return cleanName;
        }
        /// <summary>
        /// Removes known single-line comment symbols from the start of the given line.
        /// Handles comment symbols for various file types (e.g., //, #, REM, --, ;).
        /// For HTML/XML comments, removes &lt;!-- and --&gt; from the line.
        /// Returns the cleaned line, or an empty string if the input is null or whitespace.
        /// </summary>
        /// <param name="line">The line of text to process.</param>
        /// <returns>The line with known comment symbols removed.</returns>
        public static string RemoveUnknownComment(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return string.Empty;

            line = line.Trim();

            // 1) Alle anderen Kommentarzeichen testen
            foreach (var commentSymbol in LineCommentSymbolsByExtension.Values.Distinct())
            {
                if (commentSymbol == "<!-- -->")
                    continue; // Wird spaeter behandelt

                if (line.StartsWith(commentSymbol, StringComparison.OrdinalIgnoreCase))
                {
                    return line.Substring(commentSymbol.Length).Trim();
                }
            }

            // 2) HTML/XML Kommentare
            line = line.Replace("<!--", string.Empty);
            line = line.Replace("-->", string.Empty);
            return line;
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
                var lines = File.ReadAllLines(filePath, Encoding.Default)
                                .SkipWhile(string.IsNullOrWhiteSpace)
                                .ToArray();

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

                File.WriteAllText(filePath, text.NormalizeLineEndings(), utf8OhneBOM);
            }
        }
    }
}
