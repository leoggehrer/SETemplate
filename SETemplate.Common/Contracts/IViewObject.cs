//@BaseCode
namespace SETemplate.Common.Contracts
{
    public partial interface IViewObject
    {
        #region methods
        /// <summary>
        /// Copies the properties from another <see cref="IIdentifiable"/> instance.
        /// </summary>
        /// <param name="other">The other <see cref="IIdentifiable"/> instance to copy properties from.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="other"/> is null.</exception>
        void CopyProperties(IViewObject other)
        {
            bool handled = false;

            // Allows custom logic to be executed before copying properties.
            BeforeCopyProperties(other, ref handled);
            if (handled == false)
            {
                other.CheckArgument(nameof(other));
                // Copies the Guid property from the other instance.
            }

            // Allows custom logic to be executed after copying properties.
            AfterCopyProperties(other);
        }
        #endregion methods

        #region partial methods
        partial void BeforeCopyProperties(IViewObject other, ref bool handled);
        partial void AfterCopyProperties(IViewObject other);
        #endregion partial methods
    }
}
