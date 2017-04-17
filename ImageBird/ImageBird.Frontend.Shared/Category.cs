using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBird.Frontend.Shared
{
    public class Category
    {
        private ConcurrentList<IndexObject> innerItems;

        public Category(string name, params IndexObject[] initial)
        {
            this.Name = name;
            this.innerItems = new ConcurrentList<IndexObject>(initial);
        }

        public IReadOnlyCollection<IndexObject> Items
        {
            get
            {
                return this.innerItems;
            }
        }

        public string Name { get; private set; }

        public void AddItem(IndexObject item)
        {
            this.innerItems.Add(item);
        }

        public void RemoveItem(IndexObject item)
        {
            this.innerItems.Remove(item);
        }
    }
}
