//@BaseCode
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
            var changedEntries = ChangeTracker.Entries()
                                              .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                                              .Select(e => new ChangedEntry(e))
                                              .ToList();
            var entitySets = GetType().GetProperties()
                                      .Where(p => p.PropertyType.GetInterfaces()
                                                 .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntitySet<>)))
                                      .Select(p => p.GetValue(this))
                                      .Where(es => es != null)
                                      .Where(es =>
                                      {
                                          var entitySetInterface = es!.GetType().GetInterfaces()
                                              .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntitySet<>));
                                          if (entitySetInterface != null)
                                          {
                                              var genericArg = entitySetInterface.GetGenericArguments()[0];
                                              return genericArg.IsSubclassOf(typeof(Entities.EntityObject)) && !genericArg.IsAbstract;
                                          }
                                          return false;
                                      })
                                      .ToList();

            foreach (var entitySet in entitySets)
            {
                var method = entitySet?.GetType().GetMethod("BeforeSaveChanges",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                    null,
                    [typeof(IReadOnlyList<ChangedEntry>)],
                    null);

                method?.Invoke(entitySet, [changedEntries]);
            }

            foreach (var entry in changedEntries)
            {
                BeforeSaveChanges(entry);
            }

            // Separate validatable entries (subset of changedEntries)
            foreach (var changedEntry in changedEntries.Where(e => e.Entry.Entity is IValidatableEntity))
            {
                var entity = (IValidatableEntity)changedEntry.Entry.Entity;
                entity.Validate(this, changedEntry.State);
            }

            var result = base.SaveChanges();

            foreach (var entry in changedEntries)
            {
                AfterSaveChanges(entry);
            }

            foreach (var entitySet in entitySets)
            {
                var method = entitySet?.GetType().GetMethod("AfterSaveChanges",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                    null,
                    [typeof(IReadOnlyList<ChangedEntry>)],
                    null);

                method?.Invoke(entitySet, [changedEntries]);
            }
            return result;
        }

        /// <summary>
        /// Asynchronously saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the underlying database.</returns>
        internal async Task<int> ExecuteSaveChangesAsync()
        {
            var changedEntries = ChangeTracker.Entries()
                                              .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                                              .Select(e => new ChangedEntry(e))
                                              .ToList();
            // Query all IEntitySet<T> Properties
            var entitySets = GetType().GetProperties()
                                      .Where(p => p.PropertyType.GetInterfaces()
                                                   .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntitySet<>)))
                                      .Select(p => p.GetValue(this))
                                      .Where(es => es != null)
                                      .Where(es =>
                                      {
                                          var entitySetInterface = es!.GetType().GetInterfaces()
                                              .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntitySet<>));
                                          if (entitySetInterface != null)
                                          {
                                              var genericArg = entitySetInterface.GetGenericArguments()[0];
                                              return genericArg.IsSubclassOf(typeof(Entities.EntityObject)) && !genericArg.IsAbstract;
                                          }
                                          return false;
                                      })
                                      .ToList();

            foreach (var entitySet in entitySets)
            {
                var method = entitySet?.GetType().GetMethod("BeforeSaveChanges",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                    null,
                    [typeof(IReadOnlyList<ChangedEntry>)],
                    null);

                method?.Invoke(entitySet, [changedEntries]);
            }

            foreach (var entry in changedEntries)
            {
                BeforeSaveChanges(entry);
            }

            // Validate all entities before saving
            var validatableEntries = ChangeTracker.Entries()
                                                  .Where(e => e.Entity is IValidatableEntity
                                                           && (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted));

            foreach (var entry in validatableEntries)
            {
                var validatableEntity = (IValidatableEntity)entry.Entity;

                validatableEntity.Validate(this, entry.State);
            }

            var result = await base.SaveChangesAsync().ConfigureAwait(false);

            foreach (var entry in changedEntries)
            {
                AfterSaveChanges(entry);
            }

            foreach (var entitySet in entitySets)
            {
                var method = entitySet?.GetType().GetMethod("AfterSaveChanges",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                    null,
                    [typeof(IReadOnlyList<ChangedEntry>)],
                    null);

                method?.Invoke(entitySet, [changedEntries]);
            }
            return result;
        }

        #region partial methods
        /// <summary>
        /// Partial method invoked before saving changes for the specified entity entry.
        /// Allows custom logic to be executed prior to persisting changes to the database.
        /// </summary>
        /// <param name="changedEntry">The entity entry that is about to be saved.</param>
        partial void BeforeSaveChanges(ChangedEntry changedEntry);

        /// <summary>
        /// Partial method invoked after saving changes for the specified entity entry.
        /// Allows custom logic to be executed after changes have been persisted to the database.
        /// </summary>
        /// <param name="changedEntry">The entity entry that has been saved.</param>
        partial void AfterSaveChanges(ChangedEntry changedEntry);
        #endregion partial methods
    }
}
