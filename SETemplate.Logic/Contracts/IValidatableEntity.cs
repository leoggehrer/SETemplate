//@BaseCode
namespace SETemplate.Logic.Contracts
{
    public partial interface IValidatableEntity
    {
        void Validate(IContext context);
    }
}
