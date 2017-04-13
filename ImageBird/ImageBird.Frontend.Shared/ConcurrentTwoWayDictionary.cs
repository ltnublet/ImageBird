using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBird.Frontend.Shared
{
    /// <summary>
    /// Theadsafe two-way dictionary implementation.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ConcurrentTwoWayDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> keyMapping;
        private Dictionary<TValue, TKey> valueMapping;

        /// <summary>
        /// Instantiates a new <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/>.
        /// </summary>
        public ConcurrentTwoWayDictionary()
        {
            this.keyMapping = new Dictionary<TKey, TValue>();
            this.valueMapping = new Dictionary<TValue, TKey>();
        }

        /// <summary>
        /// The <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/>'s keys.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get
            {
                lock (this.keyMapping)
                {
                    return this.keyMapping.Keys;
                }
            }
        }

        /// <summary>
        /// The <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/>'s values.
        /// </summary>
        public ICollection<TValue> Values
        {
            get
            {
                lock (this.valueMapping)
                {
                    return this.valueMapping.Keys;
                }
            }
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/>.
        /// </summary>
        public int Count
        {
            get
            {
                lock (this.keyMapping)
                {
                    return this.keyMapping.Count;
                }
            }
        }

        /// <summary>
        /// Returns the <see cref="TValue"/> associated with the specified <see cref="TKey"/> <paramref name="key"/>.
        /// </summary>
        /// <param name="key">
        /// The key to look up the associated value of.
        /// </param>
        /// <returns>
        /// The value associated with the key.
        /// </returns>
        public TValue this[TKey key]
        {
            get
            {
                lock (this.keyMapping)
                {
                    return this.keyMapping[key];
                }
            }

            set
            {
                lock (this.keyMapping)
                {
                    lock (this.valueMapping)
                    {
                        if (this.keyMapping.ContainsKey(key))
                        {
                            TValue oldValue = this.keyMapping[key];
                            this.keyMapping[key] = value;
                            this.valueMapping.Remove(oldValue);
                            this.valueMapping.Add(value, key);
                        }
                        else
                        {
                            this.keyMapping[key] = value;
                            this.valueMapping[value] = key;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the <see cref="TKey"/> associated with the specified <see cref="TValue"/> <paramref name="key"/>.
        /// </summary>
        /// <param name="key">
        /// The key to look up the associated value of.
        /// </param>
        /// <returns>
        /// The value associated with the key.
        /// </returns>
        public TKey this[TValue key]
        {
            get
            {
                return this.valueMapping[key];
            }

            set
            {
                lock (this.keyMapping)
                {
                    lock (this.valueMapping)
                    {
                        if (this.valueMapping.ContainsKey(key))
                        {
                            TKey oldValue = this.valueMapping[key];
                            this.valueMapping[key] = value;
                            this.keyMapping.Remove(oldValue);
                            this.keyMapping.Add(value, key);
                        }
                        else
                        {
                            this.valueMapping[key] = value;
                            this.keyMapping[value] = key;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key">
        /// The key of the element to add.
        /// </param>
        /// <param name="value">
        /// The value of the element to add.
        /// </param>
        public void Add(TKey key, TValue value)
        {
            lock (this.keyMapping)
            {
                lock (this.valueMapping)
                {
                    this.keyMapping.Add(key, value);
                    this.valueMapping.Add(value, key);
                }
            }
        }
        
        /// <summary>
        /// Adds the specified item to the dictionary.
        /// </summary>
        /// <param name="item">
        /// The item to add.
        /// </param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add((TKey)item.Key, (TValue)item.Value);
        }
        
        /// <summary>
        /// Adds the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key">
        /// The key of the element to add.
        /// </param>
        /// <param name="value">
        /// The value of the element to add.
        /// </param>
        public void Add(TValue key, TKey value)
        {
            this.Add(value, key);
        }

        /// <summary>
        /// Adds the specified item to the dictionary.
        /// </summary>
        /// <param name="item">
        /// The item to add.
        /// </param>
        public void Add(KeyValuePair<TValue, TKey> item)
        {
            this.Add((TValue)item.Key, (TKey)item.Value);
        }

        /// <summary>
        /// Removes all keys and values from the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/>.
        /// </summary>
        public void Clear()
        {
            lock (this.keyMapping)
            {
                lock (this.valueMapping)
                {
                    this.keyMapping.Clear();
                    this.valueMapping.Clear();
                }
            }
        }

        /// <summary>
        /// Determines whether a sequence contains a specified element using the default equality comparer.
        /// </summary>
        /// <param name="item">
        /// The value to locate in the sequence.
        /// </param>
        /// <returns>
        /// True if the sequence contained the element, and false otherwise.
        /// </returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            lock (this.keyMapping)
            {
                return this.keyMapping.Contains(item);
            }
        }

        /// <summary>
        /// Determines whether a sequence contains a specified element using the default equality comparer.
        /// </summary>
        /// <param name="item">
        /// The value to locate in the sequence.
        /// </param>
        /// <returns>
        /// True if the sequence contained the element, and false otherwise.
        /// </returns>
        public bool Contains(KeyValuePair<TValue, TKey> item)
        {
            lock (this.valueMapping)
            {
                return this.valueMapping.Contains(item);
            }
        }

        /// <summary>
        /// Determines whether the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/> contains the specified key.
        /// </summary>
        /// <param name="key">
        /// The key to locate in the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/>.
        /// </param>
        /// <returns>
        /// True if the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/> contained the specified key, and false otherwise.
        /// </returns>
        public bool ContainsKey(TKey key)
        {
            lock (this.keyMapping)
            {
                return this.keyMapping.ContainsKey(key);
            }
        }

        /// <summary>
        /// Determines whether the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/> contains the specified key.
        /// </summary>
        /// <param name="key">
        /// The key to locate in the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/>.
        /// </param>
        /// <returns>
        /// True if the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/> contained the specified key, and false otherwise.
        /// </returns>
        public bool ContainsKey(TValue key)
        {
            lock (this.valueMapping)
            {
                return this.valueMapping.ContainsKey(key);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <returns>
        /// An enumerator that iterates through the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/>.
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetKeyValueEnumerator()
        {
            lock (this.keyMapping)
            {
                return this.keyMapping.GetEnumerator();
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <returns>
        /// An enumerator that iterates through the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/>.
        /// </returns>
        public IEnumerator<KeyValuePair<TValue, TKey>> GetValueKeyEnumerator()
        {
            lock (this.valueMapping)
            {
                return this.valueMapping.GetEnumerator();
            }
        }

        /// <summary>
        /// Attempts to remove the specified key from the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">
        /// The key to remove from the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/>.
        /// </param>
        /// <returns>
        /// True if the specified <paramref name="key"/> was removed from the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/>, and false otherwise.
        /// </returns>
        public bool Remove(TKey key)
        {
            lock (this.keyMapping)
            {
                lock (this.valueMapping)
                {
                    if (this.keyMapping.ContainsKey(key))
                    {
                        TValue oldValue = this.keyMapping[key];
                        this.keyMapping.Remove(key);
                        this.valueMapping.Remove(oldValue);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Attempts to remove the specified item from the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="item">
        /// The item to remove from the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/>.
        /// </param>
        /// <returns>
        /// True if the specified <paramref name="item"/> was removed from the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/>, and false otherwise.
        /// </returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            lock (this.keyMapping)
            {
                lock (this.valueMapping)
                {
                    if (this.keyMapping.ContainsKey(item.Key) 
                        && this.keyMapping[item.Key].Equals(item.Value))
                    {
                        return this.Remove((TKey)item.Key);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Attempts to remove the specified key from the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">
        /// The key to remove from the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/>.
        /// </param>
        /// <returns>
        /// True if the specified <paramref name="key"/> was removed from the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/>, and false otherwise.
        /// </returns>
        public bool Remove(TValue key)
        {
            lock (this.keyMapping)
            {
                lock (this.valueMapping)
                {
                    if (this.valueMapping.ContainsKey(key))
                    {
                        TKey oldKey = this.valueMapping[key];
                        this.valueMapping.Remove(key);
                        this.keyMapping.Remove(oldKey);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Attempts to remove the specified item from the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="item">
        /// The item to remove from the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/>.
        /// </param>
        /// <returns>
        /// True if the specified <paramref name="item"/> was removed from the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/>, and false otherwise.
        /// </returns>
        public bool Remove(KeyValuePair<TValue, TKey> item)
        {
            lock (this.keyMapping)
            {
                lock (this.valueMapping)
                {
                    if (this.valueMapping.ContainsKey(item.Key)
                        && this.valueMapping[item.Key].Equals(item.Value))
                    {
                        return this.Remove((TValue)item.Key);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Tries to get the specified <paramref name="key"/> from the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/>, and stores the result in <paramref name="value"/>.
        /// </summary>
        /// <param name="key">
        /// The key to index into the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/> on.
        /// </param>
        /// <param name="value">
        /// An out variable to store the result in. If the key did not exist in the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/>, it is set to <see cref="default{TValue}"/> instead.
        /// </param>
        /// <returns>
        /// True if a value was successfully retrieved, and false otherwise.
        /// </returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (this.keyMapping)
            {
                if (this.keyMapping.ContainsKey(key))
                {
                    value = this.keyMapping[key];
                    return true;
                }
                else
                {
                    value = default(TValue);
                    return false;
                }
            }
        }

        /// <summary>
        /// Tries to get the specified <paramref name="key"/> from the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/>, and stores the result in <paramref name="value"/>.
        /// </summary>
        /// <param name="key">
        /// The key to index into the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/> on.
        /// </param>
        /// <param name="value">
        /// An out variable to store the result in. If the key did not exist in the <see cref="ConcurrentTwoWayDictionary{TKey, TValue}"/>, it is set to <see cref="default{TKey}"/> instead.
        /// </param>
        /// <returns>
        /// True if a value was successfully retrieved, and false otherwise.
        /// </returns>
        public bool TryGetValue(TValue key, out TKey value)
        {
            lock (this.valueMapping)
            {
                if (this.valueMapping.ContainsKey(key))
                {
                    value = this.valueMapping[key];
                    return true;
                }
                else
                {
                    value = default(TKey);
                    return false;
                }
            }
        }
    }
}
