//@BaseCode
#if EXTERNALGUID_ON
namespace SETemplate.Common.Models
{
    /// <summary>
    /// Represents a model object with a unique identifier.
    /// </summary>
    partial class ModelObject
    {
        #region properties
        /// <summary>
        /// Gets or sets the unique identifier for the model object.
        /// </summary>
        public Guid Guid { get; set; }
        #endregion properties

        #region overrides
        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// <c>true</c> if the specified object is a <see cref="ModelObject"/> and has the same <see cref="Guid"/> as the current object; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object? obj)
        {
            var result = false;

            if (obj is ModelObject modelObject)
            {
                result = IsEqualsWith(Guid, modelObject.Guid);
            }
            return result;
        }
        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            return this.CalculateHashCode(Guid);
        }
        #endregion overrides
    }
}
#endif
