//@BaseCode
using System.ComponentModel.DataAnnotations;

namespace SETemplate.Common.Models
{
    /// <summary>
    /// Represents an abstract partial class for version management.
    /// </summary>
    public abstract partial class VersionModelObject : ModelObject, Contracts.IVersionable
    {
        #region properties
#if ROWVERSION_ON

#if POSTGRES_ON
        /// <summary>
        /// Gets or sets the row version of the entity.
        /// </summary>
        [Timestamp]
        public uint RowVersion { get; set; }
#else
        /// <summary>
        /// Gets or sets the row version of the entity.
        /// </summary>
        [Timestamp]
        public byte[]? RowVersion { get; set; } = [];
#endif

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

#if POSTGRES_ON
            values.Add(RowVersion);
#else
            if (RowVersion != null)
            {
                values.Add(RowVersion);
            }
#endif

#endif
            return base.GetHashCode(values);
        }
        #endregion methods
    }
}
