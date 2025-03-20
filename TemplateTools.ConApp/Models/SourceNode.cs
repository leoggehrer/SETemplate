//@BaseCode
//MdStart

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TemplateTools.Logic.Models
{
    /// <summary>
    /// This class forwards the corresponding queries to the CSharpSyntaxNode class.
    /// </summary>
    internal class SourceNode
    {
        /// <summary>
        /// Gets or sets the syntax node associated with the C# syntax.
        /// </summary>
        /// <value>
        /// The syntax node representing the C# syntax.
        /// </value>
        public CSharpSyntaxNode SyntaxNode { get; init; }
        /// <summary>
        /// Gets the syntax kind of the current syntax node.
        /// </summary>
        /// <returns>The syntax kind of the current syntax node.</returns>
        public SyntaxKind Kind => SyntaxNode.Kind();
        /// <summary>
        /// Retrieves the name of the specified SyntaxNode.
        /// </summary>
        /// <param name="node">The SyntaxNode to retrieve the name from.</param>
        /// <returns>The name of the SyntaxNode.</returns>
        public string Name => GetName(SyntaxNode);
        /// <summary>
        /// Gets the full name of the SyntaxNode and trims any leading or trailing whitespaces.
        /// </summary>
        /// <param name="syntaxNode">The SyntaxNode from which to get the full name.</param>
        /// <returns>The full name as a trimmed string.</returns>
        public string FullName => GetFullName(SyntaxNode).Trim();
        /// <summary>
        /// Gets the documentation associated with the SyntaxNode.
        /// </summary>
        /// <returns>The documentation as a string.</returns>
        public string Documentation
        {
            get
            {
                var result = new List<string>();
                var docuLines = SyntaxNode.GetLeadingTrivia()
                                          .Where(trivial => trivial.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                                          .FirstOrDefault()
                                          .ToString()
                                          .ToLines();
                
                foreach (var line in docuLines)
                {
                    if (line.Equals("<summary>"))
                    {
                        result.Add($"/// {line.Trim()}");
                    }
                    else if (line.IsNullOrWhiteSpace() == false)
                    {
                        result.Add(line.Trim());
                    }
                }
                return result.ToText();
            }
        }
        /// <summary>
        /// Gets or sets the parent SourceNode of the current SourceNode instance.
        /// </summary>
        /// <value>
        /// The parent SourceNode of the current SourceNode instance.
        /// </value>
        public SourceNode? Parent { get; init; }
        
        /// <summary>
        /// Returns an enumerable collection of <see cref="SourceNode"/> objects that represent the enum declarations
        /// contained within the <see cref="SyntaxNode"/>.
        /// </summary>
        /// <returns>An enumerable collection of <see cref="SourceNode"/> objects.</returns>
        public IEnumerable<SourceNode> Enums
        {
            get
            {
                return SyntaxNode.DescendantNodes().OfType<EnumDeclarationSyntax>()
                                 .Select(n => new SourceNode(n, this));
            }
        }
        /// <summary>
        /// Retrieves the collection of syntax nodes that represent the properties within the syntax node.
        /// </summary>
        /// <returns>
        /// An enumerable collection of <see cref="SourceNode"/> objects representing the properties within the syntax node.
        /// </returns>
        public IEnumerable<SourceNode> Properties
        {
            get
            {
                return SyntaxNode.DescendantNodes().OfType<PropertyDeclarationSyntax>()
                                 .Select(n => new SourceNode(n, this));
            }
        }
        /// <summary>
        /// Returns an enumerable collection of source nodes representing the constructors within the syntax node.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerable{SourceNode}"/> containing the constructors found within the <see cref="SyntaxNode"/>.
        /// </returns>
        public IEnumerable<SourceNode> Constructors
        {
            get
            {
                return SyntaxNode.DescendantNodes().OfType<ConstructorDeclarationSyntax>()
                                 .Select(n => new SourceNode(n, this));
            }
        }
        /// <summary>
        /// Gets all methods within the SyntaxNode.
        /// </summary>
        /// <returns>
        /// An IEnumerable of SourceNode representing the methods found.
        /// </returns>
        public IEnumerable<SourceNode> Methods
        {
            get
            {
                return SyntaxNode.DescendantNodes().OfType<MethodDeclarationSyntax>()
                                 .Select(n => new SourceNode(n, this));
            }
        }
        
        /// <summary>
        /// Initializes a new instance of the SourceNode class with the specified syntax node.
        /// </summary>
        /// <param name="syntaxNode">The CSharpSyntaxNode representing the syntax of the source node.</param>
        public SourceNode(CSharpSyntaxNode syntaxNode)
        {
            SyntaxNode = syntaxNode;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceNode"/> class with the specified <paramref name="syntaxNode"/> and <paramref name="parent"/>.
        /// </summary>
        /// <param name="syntaxNode">The CSharpSyntaxNode associated with the SourceNode.</param>
        /// <param name="parent">The parent SourceNode of the current SourceNode.</param>
        private SourceNode(CSharpSyntaxNode syntaxNode, SourceNode parent)
        {
            SyntaxNode = syntaxNode;
            Parent = parent;
        }
        
        /// <summary>
        /// Returns a string representation of the current instance.
        /// </summary>
        /// <returns>A string representation of the current instance's SyntaxNode.</returns>
        public override string ToString()
        {
            return SyntaxNode.ToString();
        }
        
        /// <summary>
        /// Gets the name of the given <see cref="CSharpSyntaxNode"/>.
        /// </summary>
        /// <param name="syntaxNode">The syntax node to get the name from.</param>
        /// <returns>The name of the syntax node.</returns>
        public static string GetName(CSharpSyntaxNode syntaxNode)
        {
            string result;
            
            // Check the specific type of the member and extract the name accordingly
            if (syntaxNode is ClassDeclarationSyntax classDeclaration)
            {
                result = classDeclaration.Identifier.ValueText;
            }
            else if (syntaxNode is EnumDeclarationSyntax enumDeclaration)
            {
                result = enumDeclaration.Identifier.ValueText;
            }
            else if (syntaxNode is FieldDeclarationSyntax fieldDeclaration)
            {
                // For fields, there can be multiple variables declared, so we take the name of the first one
                result = fieldDeclaration.Declaration.Variables.First().Identifier.ValueText;
            }
            else if (syntaxNode is ConstructorDeclarationSyntax constructorDeclaration)
            {
                result = constructorDeclaration.Identifier.ValueText;
            }
            else if (syntaxNode is PropertyDeclarationSyntax propertyDeclaration)
            {
                result = propertyDeclaration.Identifier.ValueText;
            }
            else if (syntaxNode is MethodDeclarationSyntax methodDeclaration)
            {
                result = methodDeclaration.Identifier.ValueText;
            }
            else if (syntaxNode is OperatorDeclarationSyntax operatorDeclaration)
            {
                result = operatorDeclaration.OperatorToken.ToString();
            }
            else if (syntaxNode is IndexerDeclarationSyntax indexDeclaration)
            {
                result = indexDeclaration.GetType().Name.Replace("Syntax", string.Empty);
            }
            else if (syntaxNode is DelegateDeclarationSyntax delegateDeclaration)
            {
                result = delegateDeclaration.Identifier.ValueText;
            }
            else if (syntaxNode is EventFieldDeclarationSyntax eventFieldDeclaration)
            {
                result = eventFieldDeclaration.GetType().Name.Replace("Syntax", string.Empty);
            }
            // Add more cases for other types of members as needed
            else
            {
                result = syntaxNode.GetType().Name.Replace("Syntax", string.Empty);
            }
            return result;
        }
        /// <summary>
        /// Gets the full name of a C# syntax node by extracting the name based on its type.
        /// </summary>
        /// <param name="syntaxNode">The C# syntax node.</param>
        /// <returns>The full name of the syntax node.</returns>
        public static string GetFullName(CSharpSyntaxNode syntaxNode)
        {
            string result;
            
            // Check the specific type of the member and extract the name accordingly
            if (syntaxNode is ClassDeclarationSyntax classDeclaration)
            {
                result = classDeclaration.Identifier.ValueText;
            }
            else if (syntaxNode is EnumDeclarationSyntax enumDeclaration)
            {
                result = enumDeclaration.Identifier.ValueText;
            }
            else if (syntaxNode is FieldDeclarationSyntax fieldDeclaration)
            {
                // For fields, there can be multiple variables declared, so we take the name of the first one
                result = fieldDeclaration.Declaration.Variables.First().Identifier.ValueText;
            }
            else if (syntaxNode is ConstructorDeclarationSyntax constructorDeclaration)
            {
                result = constructorDeclaration.Body != null ? constructorDeclaration.ToString().Replace(constructorDeclaration.Body.ToString(), string.Empty) : GetName(syntaxNode);
            }
            else if (syntaxNode is PropertyDeclarationSyntax propertyDeclaration)
            {
                if (propertyDeclaration.ExpressionBody != null)
                {
                    result = propertyDeclaration.ToString().Replace(propertyDeclaration.ExpressionBody.ToString(), string.Empty);
                }
                else if (propertyDeclaration.AccessorList != null)
                {
                    result = propertyDeclaration.ToString().Replace(propertyDeclaration.AccessorList.ToString(), string.Empty);
                }
                else
                {
                    result = GetName(syntaxNode);
                }
            }
            else if (syntaxNode is MethodDeclarationSyntax methodDeclaration)
            {
                result = methodDeclaration.Body != null ? methodDeclaration.ToString().Replace(methodDeclaration.Body.ToString(), string.Empty) : GetName(syntaxNode);
            }
            else if (syntaxNode is OperatorDeclarationSyntax operatorDeclaration)
            {
                result = operatorDeclaration.OperatorToken.ToString();
            }
            else if (syntaxNode is IndexerDeclarationSyntax indexDeclaration)
            {
                result = indexDeclaration.GetType().Name.Replace("Syntax", string.Empty);
            }
            else if (syntaxNode is DelegateDeclarationSyntax delegateDeclaration)
            {
                result = delegateDeclaration.Identifier.ValueText;
            }
            else if (syntaxNode is EventFieldDeclarationSyntax eventFieldDeclaration)
            {
                result = eventFieldDeclaration.GetType().Name.Replace("Syntax", string.Empty);
            }
            // Add more cases for other types of members as needed
            else
            {
                result = syntaxNode.GetType().Name.Replace("Syntax", string.Empty);
            }
            return result;
        }
    }
}
//MdEnd