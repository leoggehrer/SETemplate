//@BaseCode
#if ACCOUNT_ON
namespace SETemplate.Logic.Contracts
{
    partial interface IContext
    {
        #region properties
        /// <summary>
        /// Sets the session token.
        /// </summary>
        string SessionToken { set; }
        #endregion properties
    }
}
#endif
