using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HockeyStats
{
    // Inspired by and borrows from https://stackoverflow.com/questions/25369446/dictionary-with-item-limit
    public class RollingDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> dictionary;
        private Queue<TKey> keys;
        private int capacity;

        public RollingDictionary(int capacity)
        {
            dictionary = new Dictionary<TKey, TValue>(capacity);
            keys = new Queue<TKey>(capacity);
            this.capacity = capacity;
        }

        public void Add(TKey key, TValue value)
        {
            if (dictionary.Count == capacity)
            {
                TKey oldestKey = keys.Dequeue();
                dictionary.Remove(oldestKey);
            }

            dictionary.Add(key, value);
            keys.Enqueue(key);
        }

        public TValue GetValue(TKey key)
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }
            else
            {
                return default(TValue);
            }
        }

        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }
    }
}
