//@BaseCode
#if ACCOUNT_ON
using SETemplate.WebApi.Contracts;

namespace SETemplate.WebApi.Controllers
{
    partial class GenericViewController<TModel, TView, TContract>
    {
        #region partial methods
        partial void OnReadContextAccessor(IContextAccessor contextAccessor)
        {
            contextAccessor.SessionToken = SessionToken;
        }
        #endregion partial methods
    }
}
#endif
