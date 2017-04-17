using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBird.Frontend.Shared
{
    public class IndexObject
    {
        private ConcurrentList<Category> innerCategories;

        public IndexObject(string path, string hash, params Category[] categories)
        {
            this.Path = path;
            this.Hash = hash;
            this.innerCategories = new ConcurrentList<Category>(categories);
        }

        public string Path { get; private set; }

        public string Hash { get; private set; }

        public IReadOnlyCollection<Category> Categories
        {
            get
            {
                return this.innerCategories;
            }
        }

        public void AddCategory(Category category)
        {
            if (!this.innerCategories.Contains(category))
            {
                this.innerCategories.Add(category);
            }
        }

        public void RemoveCategory(Category category)
        {
            if (this.innerCategories.Contains(category))
            {
                this.innerCategories.Remove(category);
            }
        }
    }
}
