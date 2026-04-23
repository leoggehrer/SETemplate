//@BaseCode
#if ACCOUNT_ON
namespace SETemplate.WebApi.Models.Account
{
    /// <summary>
    /// This part of the class contains the derivation for the 'Identity'.
    /// </summary>
    partial class Identity
    {
        /// <summary>
        /// Creates a new instance of the 'Identity' class.
        /// </summary>
        public static Identity Create()
        {
            BeforeCreate();
            var result = new Identity();
            AfterCreate(result);
            return result;
        }
        /// <summary>
        /// Called before creating a new instance of the 'Identity' class.
        /// </summary>
        static partial void BeforeCreate();
        /// <summary>
        /// Called after creating a new instance of the 'Identity' class.
        /// </summary>
        static partial void AfterCreate(Identity model);
        /// <summary>
        /// Creates a new instance of the 'Identity' class based on the provided entity.
        /// </summary>
        public static Identity Create(Logic.Entities.Account.Identity entity)
        {
            var handled = false;
            var result = default(Identity);
            BeforeCreate(entity, ref result, ref handled);
            if (handled == false || result is null)
            {
                var visitedEntities = new List<object> { entity };
                result = new Identity();
                result.CopyFrom(entity);
            }
            AfterCreate(entity, result);
            return result;
        }
        /// <summary>
        /// Creates a new instance of the 'Identity' class based on the provided entity, while keeping track of visited entities to avoid circular references.
        /// </summary>
        internal static Identity? Create(List<object> visitedEntities, Logic.Entities.Account.Identity entity)
        {
            var result = default(Identity);
            if (visitedEntities.Contains(entity) == false)
            {
                visitedEntities.Add(entity);
                result = new Identity();
                result.CopyFrom(entity);
            }
            return result;
        }
        /// <summary>
        /// Called before creating a new instance of the 'Identity' class based on the provided entity.
        /// </summary>
        static partial void BeforeCreate(Logic.Entities.Account.Identity entity, ref Identity? model, ref bool handled);
        /// <summary>
        /// Called after creating a new instance of the 'Identity' class based on the provided entity.
        /// </summary>
        static partial void AfterCreate(Logic.Entities.Account.Identity entity, Identity model);
    }
}
#endif
