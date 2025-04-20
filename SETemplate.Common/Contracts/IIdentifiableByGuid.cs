//@BaseCode
#if EXTERNALGUID_ON
namespace SETemplate.Common.Contracts
{
    /// <summary>
    /// Represents an identifiable in the company manager.
    /// </summary>
    partial interface IIdentifiable
    {
        #region properties
        /// <summary>
        /// Gets the unique identifier for the entity.
        /// </summary>
        Guid Guid { get; protected set; }
        #endregion properties

        #region methods
        /// <summary>
        /// Copies the properties from another <see cref="IIdentifiable"/> instance.
        /// </summary>
        /// <param name="other">The other <see cref="IIdentifiable"/> instance to copy properties from.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="other"/> is null.</exception>
        void CopyProperties(IIdentifiable other)
        {
            bool handled = false;

            // Allows custom logic to be executed before copying properties.
            BeforeCopyProperties(other, ref handled);
            if (handled == false)
            {
                other.CheckArgument(nameof(other));
                // Copies the Guid property from the other instance.
                Guid = other.Guid;
            }

            // Allows custom logic to be executed after copying properties.
            AfterCopyProperties(other);
        }
        #endregion methods
    }
}
#endif
