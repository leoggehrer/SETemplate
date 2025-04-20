//@BaseCode
namespace SETemplate.Common.Contracts
{
    /// <summary>
    /// Represents an identifiable in the company manager.
    /// </summary>
    public partial interface IIdentifiable
    {
        #region partial methods
        partial void BeforeCopyProperties(IIdentifiable other, ref bool handled);
        partial void AfterCopyProperties(IIdentifiable other);
        #endregion partial methods
    }
}
