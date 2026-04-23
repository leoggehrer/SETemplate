//@BaseCode
#if ACCOUNT_ON
namespace SETemplate.WebApi.Models.Account
{
    /// <summary>
    /// This part of the class contains the derivation for the 'Role'.
    /// </summary>
    partial class Role
    {
        /// <summary>
        /// Creates a new instance of the 'Identity' class.
        /// </summary>
        public static Role Create()
        {
            BeforeCreate();
            var result = new Role();
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
        static partial void AfterCreate(Role model);
        /// <summary>
        /// Creates a new instance of the 'Identity' class based on the provided entity.
        /// </summary>
        public static Role Create(Logic.Entities.Account.Role entity)
        {
            var handled = false;
            var result = default(Role);
            BeforeCreate(entity, ref result, ref handled);
            if (handled == false || result is null)
            {
                var visitedEntities = new List<object> { entity };
                result = new Role();
                result.CopyFrom(entity);
            }
            AfterCreate(entity, result);
            return result;
        }
        /// <summary>
        /// Creates a new instance of the 'Identity' class based on the provided entity, while keeping track of visited entities to avoid circular references.
        /// </summary>
        internal static Role? Create(List<object> visitedEntities, Logic.Entities.Account.Role entity)
        {
            var result = default(Role);
            if (visitedEntities.Contains(entity) == false)
            {
                visitedEntities.Add(entity);
                result = new Role();
                result.CopyFrom(entity);
            }
            return result;
        }
        /// <summary>
        /// Called before creating a new instance of the 'Role' class based on the provided entity.
        /// </summary>
        static partial void BeforeCreate(Logic.Entities.Account.Role entity, ref Role? model, ref bool handled);
        /// <summary>
        /// Called after creating a new instance of the 'Role' class based on the provided entity.
        /// </summary>
        static partial void AfterCreate(Logic.Entities.Account.Role entity, Role model);
    }
}
#endif
