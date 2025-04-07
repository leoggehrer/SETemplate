//@BaseCode
using SETemplate.Logic.Contracts;

namespace SETemplate.Logic.DataContext
{
    partial class ProjectDbContext
    {
        /// <summary>
        /// Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>The number of state entries written to the underlying database.</returns>
        internal int ExecuteSaveChanges()
        {
            // Vor dem Speichern alle Entitäten validieren
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is IValidatableEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var validatableEntity = (IValidatableEntity)entry.Entity;

                validatableEntity.Validate(this);
            }

            return base.SaveChanges();
        }

        /// <summary>
        /// Asynchronously saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the underlying database.</returns>
        internal Task<int> ExecuteSaveChangesAsync()
        {
            // Vor dem Speichern alle Entitäten validieren
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is IValidatableEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var validatableEntity = (IValidatableEntity)entry.Entity;

                validatableEntity.Validate(this);
            }

            return base.SaveChangesAsync();
        }
    }
}
