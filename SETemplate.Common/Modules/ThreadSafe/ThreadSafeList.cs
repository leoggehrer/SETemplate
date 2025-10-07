//@BaseCode
namespace SETemplate.Common.Modules.ThreadSafe
{
    using System.Collections;

    /// <summary>
    /// A thread-safe implementation of the IList&lt;T&gt; interface.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public partial class ThreadSafeList<T> : IList<T>
    {
        #region fields
        private static readonly object _lock = new();
        protected readonly List<T> internalList = [];
        #endregion fields

        #region properties
        public T this[int index]
        {
            get
            {
                var result = default(T);
                
                lock (_lock)
                {
                    result = internalList[index];
                }
                return result;
            }
            set
            {
                lock (_lock)
                {
                    internalList[index] = value;
                }
            }
        }
        
        /// <summary>
        /// Gets the number of elements in the internal list.
        /// </summary>
        /// <returns>The number of elements in the internal list.</returns>
        public int Count
        {
            get
            {
                var result = 0;
                
                lock (_lock)
                {
                    result = internalList.Count;
                }
                return result;
            }
        }
        
        /// <summary>
        /// Determines if the object is read-only.
        /// </summary>
        /// <returns>A boolean value indicating if the object is read-only.</returns>
        public bool IsReadOnly => false;
        #endregion properties

        /// <summary>
        /// Adds an item to the internal list.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        public void Add(T item)
        {
            lock (_lock)
            {
                internalList.Add(item);
            }
        }
        
        /// <summary>
        /// Clears the internal list by removing all its elements.
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                internalList.Clear();
            }
        }
        
        /// <summary>
        /// Determines whether the internalList contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the internalList.</param>
        /// <returns>true if the internalList contains the specified item; otherwise, false.</returns>
        public bool Contains(T item)
        {
            var result = false;
            
            lock (_lock)
            {
                result = internalList.Contains(item);
            }
            return result;
        }
        
        /// <summary>
        /// Copies the elements of the internal list to an array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional target array to copy the elements to.</param>
        /// <param name="arrayIndex">The index in the target array at which copying begins.</param>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_lock)
            {
                internalList.CopyTo(array, arrayIndex);
            }
        }
        
        /// <summary>
        /// Returns an enumerator that iterates through a copy of the collection.
        /// </summary>
        /// <returns>
        /// An enumerator object that can be used to iterate through the collection.
        /// </returns>
        /// <remarks>
        /// This method returns an enumerator that allows sequential iteration over a copy of the elements in the collection.
        /// The copy of the collection is created by invoking the <see cref="Clone"/> method.
        /// </remarks>
        public IEnumerator<T> GetEnumerator()
        {
            return Clone().GetEnumerator();
        }
        
        /// Returns the index of the specified item in the list.
        /// @param item The item to locate in the list.
        /// @return The index of the item in the list, or -1 if the item is not found.
        public int IndexOf(T item)
        {
            var result = -1;
            
            lock (_lock)
            {
                result = internalList.IndexOf(item);
            }
            return result;
        }
        
        /// <summary>
        /// Inserts an element into the <see cref="List{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="List{T}"/>.</param>
        public void Insert(int index, T item)
        {
            lock (_lock)
            {
                internalList.Insert(index, item);
            }
        }
        
        /// <summary>
        /// Removes the specified item from the internal list and returns a value indicating whether the operation was successful.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="item">The item to be removed.</param>
        /// <returns>true if the item was successfully removed; otherwise, false.</returns>
        public bool Remove(T item)
        {
            var result = false;
            
            lock (_lock)
            {
                result = internalList.Remove(item);
            }
            return result;
        }
        
        /// <summary>
        /// Removes the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        public void RemoveAt(int index)
        {
            lock (_lock)
            {
                internalList.RemoveAt(index);
            }
        }
        
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An IEnumerator object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Clone().GetEnumerator();
        }
        
        /// <summary>
        /// Returns a new list that contains the elements of the current list.
        /// </summary>
        /// <returns>A new list containing the elements of the current list.</returns>
        /// <remarks>
        /// This method creates a new list and copies all elements from the current list to the new list.
        /// The new list is independent of the original list and can be modified without affecting the original list.
        /// </remarks>
        public List<T> Clone()
        {
            var newList = new List<T>();
            
            lock (_lock)
            {
                internalList.ForEach(x => newList.Add(x));
            }
            return newList;
        }
    }
}
