//@BaseCode
#if EXTERNALGUID_OFF
namespace SETemplate.Common.Contracts
{
    partial interface IIdentifiable
    {
        #region properties
        /// <summary>
        /// Gets the unique identifier for the entity.
        /// </summary>
        IdType Id { get; protected set; }
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
                Id = other.Id;
            }

            // Allows custom logic to be executed after copying properties.
            AfterCopyProperties(other);
        }
        #endregion methods
    }
}
#endif
