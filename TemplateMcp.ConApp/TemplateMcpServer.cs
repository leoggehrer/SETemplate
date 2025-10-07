//@BaseCode
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace TemplateMcp.ConApp
{
    [McpServerToolType]
    [Description("""
                    TemplateMcpServer is a special MCP server tool interface for exploring and modifying the developer template for data-centric applications “SETemplate”.

                    It provides a structured and semantic way to:
                    - retrieve the structure and layout of an entity in the system (GetEntityTemplate())
                    - Retrieve the structure and layout of a validation for an entity (GetEntityValidationTemplate())
                    - Retrieve the structure and layout of a data import interface for CSV data (GetCsvImportTemplate())
                                                        - query the structure of a list view in HTML for an Angular app (GetListHtmlTemplate())
                    - query the structure of an edit view in HTML for an Angular app (GetEditHtmlTemplate())
                    - execute commands for code generation

                    All operations are tailored exclusively to the functions of SETemplate.
                    Any request that is outside the capabilities of SETemplate will be handled politely.
                                                        Use this server if you want to do the following:
                    - Create entities for the application
                    - Create an import of csv data
                    - Create HTML forms for lists and edit functions

                    Translated with DeepL.com (free version)
                """)]
    public static class TemplateMcpServer
    {
        #region class members
        private static readonly string _entities_templates;
        private static readonly string _import_templates;
        private static readonly string _forms_templates;
        #endregion class members

        static TemplateMcpServer()
        {
            if (File.Exists("entities_template.txt"))
            {
                _entities_templates = File.ReadAllText("entities_template.txt");
            }
            else
            {
                _entities_templates = string.Empty;
            }

            if (File.Exists("forms_template.txt"))
            {
                _forms_templates = File.ReadAllText("forms_template.txt");
            }
            else
            {
                _forms_templates = string.Empty;
            }

            if (File.Exists("import_template.txt"))
            {
                _import_templates = File.ReadAllText("import_template.txt");
            }
            else
            {
                _import_templates = string.Empty;
            }
        }

        [McpServerTool]
        [Description("""
        Provides a complete general structure of an entity in the system.
        Useful when the AI creates an entity.
        """)]
        public static string GetEntityTemplate()
        {
            return $"""{_entities_templates}""";
        }

        [McpServerTool]
        [Description("""
        Provides a complete general structure for an import in the system.
        Useful when the AI creates an import.
        """)]
        public static string GetCsvImportTemplate()
        {
            return $"""{_import_templates}""";
        }
        [McpServerTool]
        [Description("""
        Provides a complete general structure for an HTML view in the system.
        Useful when the AI creates an import.
        """)]
        public static string GetHtmlTemplate()
        {
            return $"""{_forms_templates}""";
        }
    }
}
