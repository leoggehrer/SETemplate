//@BaseCode
namespace SETemplate.Common.Contracts
{
    /// <summary>
    /// An interface representing versionable entities.
    /// </summary>
    public partial interface IVersionable : IIdentifiable
    {
#if ROWVERSION_ON

#if POSTGRES_ON
        /// <summary>
        /// Gets the row version associated with the entity.
        /// </summary>
        uint RowVersion { get; protected set; }
#else
        /// <summary>
        /// Gets the row version associated with the entity.
        /// </summary>
        /// <remarks>
        /// The row version is a byte array that represents the version of the entity's data.
        /// It can be used for optimistic concurrency control in data storage and retrieval.
        /// </remarks>
        byte[]? RowVersion { get; protected set; }
#endif

        void CopyProperties(IVersionable other)
        {
            bool handled = false;
            // Allows custom logic to be executed before copying properties.
            BeforeCopyProperties(other, ref handled);
            if (handled == false)
            {
                ((IIdentifiable)this).CopyProperties(other);
                // Copies the RowVersion property from the other instance.
#if SQLITE_OFF
                RowVersion = other.RowVersion;
#endif
            }
            // Allows custom logic to be executed after copying properties.
            AfterCopyProperties(other);
        }

        partial void BeforeCopyProperties(IVersionable other, ref bool handled);
        partial void AfterCopyProperties(IVersionable other);

#endif
    }
}
