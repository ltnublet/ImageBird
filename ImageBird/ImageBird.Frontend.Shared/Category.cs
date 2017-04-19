using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBird.Frontend.Shared
{
    /// <summary>
    /// Represents a category in the index.
    /// </summary>
    public class Category
    {
        private List<IndexObject> innerItems;

        /// <summary>
        /// Instantiates a new <see cref="Category"/> using the supplied parameters.
        /// </summary>
        /// <param name="name">
        /// The name of the category.
        /// </param>
        /// <param name="initial">
        /// The <see cref="IndexObject"/>s this <see cref="Category"/> contains, if any.
        /// </param>
        public Category(string name, params IndexObject[] initial)
        {
            this.Name = name;
            this.innerItems = new List<IndexObject>(initial);
        }

        /// <summary>
        /// The <see cref="IndexObject"/>s this <see cref="Category"/> contains.
        /// </summary>
        public IReadOnlyCollection<IndexObject> Items
        {
            get
            {
                return this.innerItems;
            }
        }

        /// <summary>
        /// The name of the <see cref="Category"/>.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Adds the specified <see cref="IndexObject"/> <paramref name="item"/> to the category.
        /// </summary>
        /// <param name="item">
        /// The <see cref="IndexObject"/> to add to this category.
        /// </param>
        public void AddItem(IndexObject item)
        {
            lock (this.innerItems)
            {
                if (!this.innerItems.Contains(item))
                {
                    this.innerItems.Add(item);
                }
            }
        }

        /// <summary>
        /// Removes the specified <see cref="IndexObject"/> <paramref name="item"/> from the category,  or throws <see cref="KeyNotFoundException"/> if the <see cref="IndexObject"/> was not in the category.
        /// </summary>
        /// <param name="item"></param>
        public void RemoveItem(IndexObject item)
        {
            lock (this.innerItems)
            {
                if (this.innerItems.Contains(item))
                {
                    this.innerItems.Remove(item);
                }
                else
                {
                    throw new KeyNotFoundException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Properties.Resources.IndexObjectNotInCategory,
                            this.Name,
                            item.Hash));
                }
            }
        }
    }
}
