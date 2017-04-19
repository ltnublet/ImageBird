using ImageBird.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBird.Frontend.Shared
{
    /// <summary>
    /// Represents an image in the index.
    /// </summary>
    public class IndexObject
    {
        private List<Category> categories;

        /// <summary>
        /// Instantiates a new <see cref="IndexObject"/> using the supplied path. This method will compute the hash of the file - this is an expensive operation.
        /// </summary>
        /// <param name="path">
        /// The path to the image.
        /// </param>
        public IndexObject(string path)
        {
            this.Path = path;
            this.Hash = FastBitmap.FromFile(path).GetPerceptualHash();
            this.categories = new List<Category>();
        }

        /// <summary>
        /// Instantiate a new <see cref="IndexObject"/> using the supplied parameters.
        /// </summary>
        /// <param name="path">
        /// The path to the image.
        /// </param>
        /// <param name="hash">
        /// The hash of the image.
        /// </param>
        /// <param name="categories">
        /// The <see cref="Category"/>s this <see cref="IndexObject"/> belongs to, if any.
        /// </param>
        public IndexObject(string path, string hash, params Category[] categories)
        {
            this.Path = path;
            this.Hash = hash;
            this.categories = new List<Category>(categories);
        }

        /// <summary>
        /// The path this <see cref="IndexObject"/> was instantiated with.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// The hash this <see cref="IndexObject"/> was instantiated with.
        /// </summary>
        public string Hash { get; private set; }

        /// <summary>
        /// The categories this <see cref="IndexObject"/> belongs to.
        /// </summary>
        public IReadOnlyCollection<Category> Categories
        {
            get
            {
                return this.categories;
            }
        }

        /// <summary>
        /// Checks if the supplied <see cref="IndexObject"/>s have the same path and hash.
        /// </summary>
        /// <param name="left">
        /// The first <see cref="IndexObject"/>.
        /// </param>
        /// <param name="right">
        /// The second <see cref="IndexObject"/>.
        /// </param>
        /// <returns>
        /// True if the supplied <see cref="IndexObject"/>s have the same path and hash, and false otherwise.
        /// </returns>
        public static bool PathAndHashMatch(IndexObject left, IndexObject right)
        {
            return left.Path == right.Path && left.Hash == right.Hash;
        }

        /// <summary>
        /// Checks if the supplied <see cref="IndexObject"/> has the same path and hash as this <see cref="IndexObject"/>.
        /// </summary>
        /// <param name="item">
        /// The <see cref="IndexObject"/> to compare the path and hash with.
        /// </param>
        /// <returns>
        /// True if the supplied <see cref="IndexObject"/> has the same path and hash, and false otherwise.
        /// </returns>
        public bool PathAndHashMatch(IndexObject item)
        {
            return IndexObject.PathAndHashMatch(this, item);
        }

        /// <summary>
        /// Adds the specified <see cref="Category"/> <paramref name="category"/> to this <see cref="IndexObject"/>'s categories if it was not already in the category.
        /// </summary>
        /// <param name="category">
        /// The category to add.
        /// </param>
        internal void AddCategory(Category category)
        {
            lock (this.categories)
            {
                if (!this.categories.Contains(category))
                {
                    this.categories.Add(category);
                }
            }
        }

        /// <summary>
        /// Removes this <see cref="IndexObject"/> from the specified <see cref="Category"/> <paramref name="category"/>, or throws <see cref="KeyNotFoundException"/> if the <see cref="IndexObject"/> was not in that category.
        /// </summary>
        /// <param name="category">
        /// The category to remove.
        /// </param>
        internal void RemoveCategory(Category category)
        {
            lock (this.categories)
            {
                if (this.categories.Contains(category))
                {
                    this.categories.Remove(category);
                }
                else
                {
                    throw new KeyNotFoundException(
                        string.Format(
                            CultureInfo.InvariantCulture, 
                            Properties.Resources.IndexObjectNotInCategory,
                            category.Name,
                            this.Hash));
                }
            }
        }
    }
}
