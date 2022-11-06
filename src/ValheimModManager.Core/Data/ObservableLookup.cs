using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace ValheimModManager.Core.Data
{
    public class ObservableLookup<TKey, TValue> : IDictionary<TKey, ObservableCollection<TValue>>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly Dictionary<TKey, ObservableCollection<TValue>> _data;

        public ObservableLookup()
        {
            _data = new Dictionary<TKey, ObservableCollection<TValue>>();
        }

        public ObservableLookup(TKey key)
            : this()
        {
            _data.Add(key, new ObservableCollection<TValue>());
        }

        public ObservableLookup(TKey key, IEnumerable<TValue> value)
            : this()
        {
            _data.Add(key, new ObservableCollection<TValue>(value));
        }

        public int Count
        {
            get { return _data.Count; }
        }

        public ICollection<TKey> Keys
        {
            get { return _data.Keys; }
        }

        public ICollection<ObservableCollection<TValue>> Values
        {
            get { return _data.Values; }
        }

        public ObservableCollection<TValue> this[TKey key]
        {
            get { return _data[key]; }
            set
            {
                if (!_data.TryGetValue(key, out var existingValue))
                {
                    OnCollectionAdded();
                }
                else
                {
                    var newItems = new KeyValuePair<TKey, ObservableCollection<TValue>>(key, value);
                    var oldItems = new KeyValuePair<TKey, ObservableCollection<TValue>>(key, existingValue);

                    OnCollectionChanged(newItems, oldItems);
                }
            }
        }

        public void Add(TKey key, ObservableCollection<TValue> value)
        {
            _data.Add(key, value);
            OnCollectionAdded();
        }

        public void Clear()
        {
            _data.Clear();
            OnCollectionCleared();
        }

        public bool Remove(TKey key)
        {
            if (!_data.TryGetValue(key, out var value))
            {
                return false;
            }

            if (!_data.Remove(key))
            {
                return false;
            }

            OnCollectionRemoved(new KeyValuePair<TKey, ObservableCollection<TValue>>(key, value));

            return true;
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_data.Count)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_data.Keys)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_data.Values)));
        }

        private void OnCollectionChanged(KeyValuePair<TKey, ObservableCollection<TValue>> newItems, KeyValuePair<TKey, ObservableCollection<TValue>> oldItems)
        {
            var eventArgs =
                new NotifyCollectionChangedEventArgs
                (
                    NotifyCollectionChangedAction.Replace,
                    newItems,
                    oldItems
                );

            CollectionChanged?.Invoke(this, eventArgs);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Values)));
        }

        private void OnCollectionAdded()
        {
            var eventArgs =
                new NotifyCollectionChangedEventArgs
                (
                    NotifyCollectionChangedAction.Add,
                    this.Last()
                );

            OnCollectionChanged(eventArgs);
        }

        private void OnCollectionCleared()
        {
            var eventArgs =
                new NotifyCollectionChangedEventArgs
                (
                    NotifyCollectionChangedAction.Reset
                );

            OnCollectionChanged(eventArgs);
        }

        private void OnCollectionRemoved(KeyValuePair<TKey, ObservableCollection<TValue>> item)
        {
            var eventArgs =
                new NotifyCollectionChangedEventArgs
                (
                    NotifyCollectionChangedAction.Remove,
                    item
                );

            OnCollectionChanged(eventArgs);
        }

        IEnumerator<KeyValuePair<TKey, ObservableCollection<TValue>>> IEnumerable<KeyValuePair<TKey, ObservableCollection<TValue>>>.GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        void ICollection<KeyValuePair<TKey, ObservableCollection<TValue>>>.Add(KeyValuePair<TKey, ObservableCollection<TValue>> item)
        {
            Add(item.Key, item.Value);
        }

        void ICollection<KeyValuePair<TKey, ObservableCollection<TValue>>>.Clear()
        {
            Clear();
        }

        bool ICollection<KeyValuePair<TKey, ObservableCollection<TValue>>>.Contains(KeyValuePair<TKey, ObservableCollection<TValue>> item)
        {
            return _data.ContainsKey(item.Key) && _data.ContainsValue(item.Value);
        }

        void ICollection<KeyValuePair<TKey, ObservableCollection<TValue>>>.CopyTo(KeyValuePair<TKey, ObservableCollection<TValue>>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        bool ICollection<KeyValuePair<TKey, ObservableCollection<TValue>>>.Remove(KeyValuePair<TKey, ObservableCollection<TValue>> item)
        {
            return Remove(item.Key);
        }

        bool ICollection<KeyValuePair<TKey, ObservableCollection<TValue>>>.IsReadOnly
        {
            get { return false; }
        }

        bool IDictionary<TKey, ObservableCollection<TValue>>.ContainsKey(TKey key)
        {
            return _data.ContainsKey(key);
        }

        bool IDictionary<TKey, ObservableCollection<TValue>>.TryGetValue(TKey key, out ObservableCollection<TValue> value)
        {
            return _data.TryGetValue(key, out value);
        }
    }
}