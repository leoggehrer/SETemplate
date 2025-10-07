//@BaseCode
namespace TemplateTools.Logic.Contracts
{
    /// <summary>
    /// Represents a generated item.
    /// </summary>
    public interface IGeneratedItem
    {
        /// <summary>
        /// Gets the unit type of the common instance.
        /// </summary>
        Common.UnitType UnitType { get; }
        /// <summary>
        /// Gets the type of the item.
        /// </summary>
        Common.ItemType ItemType { get; }
        /// <summary>
        /// Gets a value indicating whether the item has a label.
        /// </summary>
        bool HasDefaultLabel { get; }
        /// <summary>
        /// Gets the special label of the item.
        /// </summary>
        string SpecialLabel { get; }
        /// <summary>
        /// Gets the full name.
        /// </summary>
        string FullName { get; }
        /// <summary>
        /// Gets the sub file path.
        /// </summary>
        string SubFilePath { get; }
        ///<summary>
        /// Gets the file extension.
        ///</summary>
        string FileExtension { get; }
        /// <summary>
        /// Gets the source code.
        /// </summary>
        IEnumerable<string> SourceCode { get; }
    }
}

