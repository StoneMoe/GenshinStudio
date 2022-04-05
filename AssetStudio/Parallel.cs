using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AssetStudio
{
    /// <summary>
    /// https://stackoverflow.com/questions/18922985/concurrent-hashsett-in-net-framework
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConcurrentHashSet<T> : IDisposable
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly HashSet<T> _hashSet;

        public ConcurrentHashSet(IEqualityComparer<T> comparer)
        {
            this._hashSet = new HashSet<T>(comparer);
        }

        #region Implementation of ICollection<T> ...ish
        public bool Add(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                return _hashSet.Add(item);
            }
            finally
            {
                if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
            }
        }

        public void Clear()
        {
            _lock.EnterWriteLock();
            try
            {
                _hashSet.Clear();
            }
            finally
            {
                if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
            }
        }

        public bool Contains(T item)
        {
            _lock.EnterReadLock();
            try
            {
                return _hashSet.Contains(item);
            }
            finally
            {
                if (_lock.IsReadLockHeld) _lock.ExitReadLock();
            }
        }

        public bool Remove(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                return _hashSet.Remove(item);
            }
            finally
            {
                if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
            }
        }

        public int Count
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _hashSet.Count;
                }
                finally
                {
                    if (_lock.IsReadLockHeld) _lock.ExitReadLock();
                }
            }
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                if (_lock != null)
                    _lock.Dispose();
        }
        ~ConcurrentHashSet()
        {
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// https://github.com/dtao/ConcurrentList/blob/master/ConcurrentList/SynchronizedList.cs
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ThreadSafeList<T> : IList<T>, IList
    {
        public abstract T this[int index] { get; }

        public abstract int Count { get; }

        public abstract void Add(T item);

        public virtual int IndexOf(T item)
        {
            IEqualityComparer<T> comparer = EqualityComparer<T>.Default;

            int count = Count;
            for (int i = 0; i < count; i++)
            {
                if (comparer.Equals(item, this[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        public virtual bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }

        public abstract void CopyTo(T[] array, int arrayIndex);

        public IEnumerator<T> GetEnumerator()
        {
            int count = Count;
            for (int i = 0; i < count; i++)
            {
                yield return this[i];
            }
        }

        #region "Protected methods"

        protected abstract bool IsSynchronizedBase { get; }

        protected virtual void CopyToBase(Array array, int arrayIndex)
        {
            for (int i = 0; i < this.Count; ++i)
            {
                array.SetValue(this[i], arrayIndex + i);
            }
        }

        protected virtual int AddBase(object value)
        {
            Add((T)value);
            return Count - 1;
        }

        #endregion

        #region "Explicit interface implementations"

        T IList<T>.this[int index]
        {
            get { return this[index]; }
            set { throw new NotSupportedException(); }
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return false; }
        }

        object IList.this[int index]
        {
            get { return this[index]; }
            set { throw new NotSupportedException(); }
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        void IList.Remove(object value)
        {
            throw new NotSupportedException();
        }

        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((T)value);
        }

        void IList.Clear()
        {
            throw new NotSupportedException();
        }

        bool IList.Contains(object value)
        {
            return ((IList)this).IndexOf(value) != -1;
        }

        int IList.Add(object value)
        {
            return AddBase(value);
        }

        bool ICollection.IsSynchronized
        {
            get { return IsSynchronizedBase; }
        }

        object ICollection.SyncRoot
        {
            get { return null; }
        }

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            CopyToBase(array, arrayIndex);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

    public sealed class SynchronizedList<T> : ThreadSafeList<T>
    {
        List<T> m_list;
        int m_count;

        public SynchronizedList()
        {
            m_list = new List<T>();
        }

        public override T this[int index]
        {
            get
            {
                int count = m_count;
                if (index < 0 || index >= count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }

                return m_list[index];
            }
        }

        public override int Count
        {
            get { return m_count; }
        }

        public override void Add(T element)
        {
            lock (m_list)
            {
                m_list.Add(element);
                ++m_count;
            }
        }
        public void Clear() {
            lock (m_list)
            {
                m_list.Clear();
                m_count = 0;
            }
        }

        public override void CopyTo(T[] array, int arrayIndex)
        {
            int count = m_count;
            m_list.CopyTo(0, array, arrayIndex, count);
        }

        internal int FindIndex(Predicate<T> match)
        {
            return m_list.FindIndex(match);
        }

        #region "Protected methods"

        protected override bool IsSynchronizedBase
        {
            get { return true; }
        }


        #endregion
    }
}
