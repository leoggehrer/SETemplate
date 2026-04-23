//@BaseCode
#if ACCOUNT_ON
using System.Reflection;

namespace SETemplate.Logic.DataContext
{
    /// <summary>
    /// Represents a view set with security features.
    /// </summary>
    partial class ViewSet<TView>
    {
        #region methods
        /// <summary>
        /// Executes logic before accessing a method, including authorization checks.
        /// </summary>
        /// <param name="methodBase">The method being accessed.</param>
        partial void BeforeAccessing(MethodBase methodBase)
        {
            if (!BeforeAccessingHandler(methodBase))
            {
                ExecuteBeforeAccessing(methodBase);
            }
        }
        #endregion methods
    }
}
#endif
