using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBird.Frontend.Shared
{
    public class Index : IIndex
    {
        private Dictionary<string, IndexObject> byPath;
        private Dictionary<string, IndexObject> byHash;
        private Dictionary<Category, List<IndexObject>> byCategory;
        private Dictionary<string, Category> categories;
        private List<string> directories;
        private HashComparer comparer;
        
        /// <summary>
        /// Instantiates a <see cref="Index"/> using the supplied parameters.
        /// </summary>
        public Index()
        {
            this.byPath = new Dictionary<string, IndexObject>();
            this.byHash = new Dictionary<string, IndexObject>();
            this.byCategory = new Dictionary<Category, List<IndexObject>>();
            this.categories = new Dictionary<string, Category>();
            this.directories = new List<string>();
            this.comparer = new HashComparer(-1);
        }

        public IReadOnlyCollection<string> Directories
        {
            get
            {
                lock (this.directories)
                {
                    return this.directories;
                }
            }
        }

        public IReadOnlyCollection<string> Files
        {
            get
            {
                lock (this.byPath)
                {
                    return this.byPath.Keys;
                }
            }
        }

        public IReadOnlyCollection<string> Categories
        {
            get
            {
                lock (this.byCategory)
                {
                    return this.byCategory.Keys.Select(x => x.Name).ToList();
                }
            }
        }

        public void AddCategory(string category)
        {
            lock (this.categories)
            {
                if (!this.categories.ContainsKey(category))
                {
                    lock (this.byCategory)
                    {
                        Category created = new Category(category);
                        this.byCategory.Add(created, new List<IndexObject>());
                        this.categories.Add(category, created);
                    }
                }
            }
        }

        public void AddDirectory(string path)
        {
            lock (this.directories)
            {
                if (!this.directories.Contains(path))
                {
                    this.directories.Add(path);
                }
            }
        }

        public IReadOnlyCollection<IndexObject> AddFile(string path)
        {
            path = Path.GetFullPath(path);
            List<IndexObject> collisions = null;

            lock (this.byPath)
            {
                if (!this.byPath.ContainsKey(path))
                {
                    IndexObject created = new IndexObject(path);

                    lock (this.byHash)
                    {
                        collisions = 
                            this.byHash
                                .Where(x => this.comparer.Compare(x.Key, created.Hash))
                                .Select(x => x.Value)
                                .ToList();

                        if (collisions.Count == 0)
                        {
                            this.byPath.Add(path, created);
                            this.byHash.Add(created.Hash, created);
                        }
                    }
                }
            }

            return (IReadOnlyCollection<IndexObject>)collisions ?? new IndexObject[0];
        }

        public IReadOnlyCollection<IndexObject> GetByCategory(string category)
        {
            return this.categories[category].Items;
        }

        public IndexObject GetByPath(string path)
        {
            return this.byPath[Path.GetFullPath(path)];
        }

        public IndexObject GetByPerceptualHash(string hash)
        {
            return this.byHash[hash];
        }

        public Category GetCategory(string category)
        {
            return this.categories[category];
        }

        public void Load(string path)
        {
            throw new NotImplementedException();
        }

        public void RemoveCategory(string category)
        {
            lock (this.categories)
            {
                if (this.categories.ContainsKey(category))
                {
                    lock (this.byCategory)
                    {
                        Category cache = this.categories[category];

                        lock (cache)
                        {
                            foreach (IndexObject item in cache.Items)
                            {
                                lock (item)
                                {
                                    item.RemoveCategory(cache);
                                }
                            }
                        }

                        this.categories.Remove(category);
                    }
                }
            }
        }

        public void RemoveDirectory(string path)
        {
            path = Path.GetFullPath(path);

            lock (this.directories)
            {
                this.directories.Remove(path);
            }

            throw new NotImplementedException(
                "Need to remove all IndexObjects in this.byPath that belonged to that path.");
        }

        public void RemoveFile(string path)
        {
            path = Path.GetFullPath(path);

            lock (this.byPath)
            {
                if (this.byPath.ContainsKey(path))
                {
                    lock (this.byHash)
                    {
                        IndexObject cache = this.byPath[path];

                        this.byHash.Remove(cache.Hash);

                        lock (cache)
                        {
                            foreach (Category category in cache.Categories)
                            {
                                lock (category)
                                {
                                    category.RemoveItem(cache);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Save(string path)
        {
            throw new NotImplementedException();
        }
    }
}
