using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBird.Frontend.Shared
{
    public class ConcurrentList<T> : IList<T>, IReadOnlyList<T>
    {
        private List<T> innerList;

        public ConcurrentList(params T[] initial)
        {
            this.innerList = new List<T>(initial);
        }

        public int Count
        {
            get
            {
                lock (this.innerList)
                {
                    return this.innerList.Count;
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public T this[int index]
        {
            get
            {
                lock (this.innerList)
                {
                    return this.innerList[index];
                }
            }

            set
            {
                lock (this.innerList)
                {
                    this.innerList[index] = value;
                }
            }
        }

        public void Add(T item)
        {
            lock (this.innerList)
            {
                this.innerList.Add(item);
            }
        }

        public void Clear()
        {
            lock (this.innerList)
            {
                this.innerList.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (this.innerList)
            {
                return this.innerList.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (this.innerList)
            {
                this.innerList.CopyTo(array, arrayIndex);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (this.innerList)
            {
                return this.innerList.ToList().GetEnumerator();
            }
        }

        public int IndexOf(T item)
        {
            lock (this.innerList)
            {
                return this.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            lock (this.innerList)
            {
                this.innerList.Insert(index, item);
            }
        }

        public bool Remove(T item)
        {
            lock (this.innerList)
            {
                if (this.innerList.Contains(item))
                {
                    this.innerList.Remove(item);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void RemoveAt(int index)
        {
            lock (this.innerList)
            {
                this.innerList.RemoveAt(index);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
