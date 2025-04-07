//@BaseCode
#if ACCOUNT_ON
namespace SETemplate.WebApi.Contracts
{
    partial interface IContextAccessor
    {
        string SessionToken { set; }
    }
}
#endif
