//@BaseCode
namespace TemplateTools.ConApp.Modules
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System.Text;

    internal static class CodeExporter
    {
        /// <summary>
        /// Creates CSharpParseOptions with predefined preprocessor symbols for parsing C# code.
        /// </summary>
        /// <returns>A configured <see cref="CSharpParseOptions"/> instance.</returns>
        private static CSharpParseOptions CreateParseOptions()
        {
            var defines = "ACCOUNT_ON;IDINT_ON;IDLONG_OFF;IDGUID_OFF;ROWVERSION_OFF;EXTERNALGUID_OFF;POSTGRES_OFF;SQLSERVER_OFF;SQLITE_ON;DOCKER_OFF;DEVELOP_ON;DBOPERATION_ON;GENERATEDCODE_ON"
                          .Split(';')
                          .Select(i => i.Replace("_OFF", "_ON"))
                          .ToArray();

            return new CSharpParseOptions(preprocessorSymbols: defines);
        }
        /// <summary>
        /// Prepares a C# code file for standardized export format.
        /// Includes namespace wrapping, usings, filename comment, and preserves preprocessor directives.
        /// </summary>
        /// <param name="filePath">The full path to the source file (used to extract the filename).</param>
        /// <param name="itemCode">The raw C# code content of the file.</param>
        /// <returns>A formatted string in the export structure.</returns>
        public static string PrepareCSharpItemForExport(string filePath, string itemCode)
        {
            var fileName = Path.GetFileName(filePath);
            var headerItems = new List<string>();
            var footerItems = new List<string>();

            // Parse syntax tree with preprocessor symbols enabled
            var tree = CSharpSyntaxTree.ParseText(itemCode, CreateParseOptions());
            var root = tree.GetCompilationUnitRoot();

            // Collect all using directives (global and within namespaces)
            var usingDirectives = root.Usings
                .Concat(root.DescendantNodes()
                            .OfType<NamespaceDeclarationSyntax>()
                            .SelectMany(ns => ns.Usings))
                .Distinct(new UsingComparer())
                .ToList();

            // Detect the first namespace declaration
            var namespaceNode = root.DescendantNodes()
                                    .OfType<NamespaceDeclarationSyntax>()
                                    .FirstOrDefault();
            var namespaceName = namespaceNode?.Name.ToString() ?? "Default.Namespace";

            // Extract the inner members (class, struct, records, etc.)
            var memberSource = new StringBuilder();
            var members = namespaceNode?.Members.Count > 0 ? namespaceNode.Members : root.Members;

            foreach (var member in members)
            {
                memberSource.AppendLine(member.ToFullString().TrimEnd());
            }

            // Compose final formatted output
            var sb = new StringBuilder();

            headerItems.Add("//<CSharpItem>");
            headerItems.Add($"//<Filename>{fileName}</Filename>");

            sb.AppendLine("//<code>");
            sb.AppendLine($"namespace {namespaceName}");
            sb.AppendLine("{");

            foreach (var usingItem in usingDirectives)
            {
                var lines = usingItem.ToFullString().ToLines()
                                            .Select(l => l.RemoveLeftAndRight(' '))
                                            .Where(l => string.IsNullOrWhiteSpace(l) == false);
                foreach (var line in lines)
                {
                    if (CommonStaticLiterals.FirstLineLabels.Any(e => line.StartsWith($"//{e}")))
                    {
                        headerItems.Add($"//<Header>{line}</Header>");
                    }
                    else if (line.StartsWith("using "))
                    {
                        var normalizeline = line.NormalizeLineEndings();

                        if (normalizeline.EndsWith(Environment.NewLine))
                            sb.Append(normalizeline.SetIndent());
                        else
                            sb.AppendLine(normalizeline.SetIndent());
                    }
                    else
                    {
                        sb.AppendLine(line);
                    }
                }
            }

            sb.AppendLine();

            foreach (var line in memberSource.ToString().Split('\n'))
            {
                sb.AppendLine($"{line.TrimEnd()}");
            }

            sb.AppendLine("}");
            sb.AppendLine("//</Code>");

            footerItems.Add("//</CSharpItem>");

            headerItems.Reverse();
            foreach (var item in headerItems)
            {
                sb.Insert(0, $"{item}{Environment.NewLine}");
            }

            foreach (var item in footerItems)
            {
                sb.AppendLine(item);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Comparer for eliminating duplicate using directives.
        /// </summary>
        private class UsingComparer : IEqualityComparer<UsingDirectiveSyntax>
        {
            public bool Equals(UsingDirectiveSyntax? x, UsingDirectiveSyntax? y)
                => x?.ToString() == y?.ToString();

            public int GetHashCode(UsingDirectiveSyntax obj)
                => obj.ToString().GetHashCode();
        }
    }
}
