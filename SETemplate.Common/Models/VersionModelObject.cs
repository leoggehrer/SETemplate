//@BaseCode
namespace SETemplate.Common.Models
{
    /// <summary>
    /// Represents an abstract partial class for version management.
    /// </summary>
    public abstract partial class VersionModelObject : ModelObject, Contracts.IVersionable
    {
        #region properties
#if ROWVERSION_ON
        /// <summary>
        /// Row version of the entity.
        /// </summary>
        public virtual byte[]? RowVersion { get; set; }
#endif
        #endregion properties

        #region methods
        /// <summary>
        /// Computes the hash code for the specified list of objects.
        /// </summary>
        /// <param name="values">A list of objects.</param>
        /// <returns>The computed hash code.</returns>
        protected override int GetHashCode(List<object?> values)
        {
#if ROWVERSION_ON
            if (RowVersion != null)
            {
                values.Add(RowVersion);
            }
#endif
            return base.GetHashCode(values);
        }
        #endregion methods
    }
}
