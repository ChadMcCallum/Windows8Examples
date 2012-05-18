using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Foundation.Collections;
using System.Linq;

namespace W8Shared
{
    /// <summary>
    /// This is a class that wraps the ObservableCollection we all know and love to use the new 
    /// IObservableVector interface in WinRT
    /// 
    /// Thanks to Avi Pilosof for the code (http://blogs.msdn.com/b/avip/archive/2011/09/18/windows-8-development-tidbits-observablecollection-doesn-t-work.aspx)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BindableObservableCollection<T> : ObservableCollection<T>, IObservableVector<object>
    {
        public event VectorChangedEventHandler<object> VectorChanged;

        public int IndexOf(object item)
        {
            return base.IndexOf((T)item);
        }

        public void Insert(int index, object item)
        {
            base.Insert(index, (T)item);
            RaiseChange(CollectionChange.ItemInserted, (uint)index);
        }

        private void RaiseChange(CollectionChange type, uint index)
        {
            if (VectorChanged != null)
                VectorChanged(this, new VectorChangedEventArgs(type, index));
        }

        public new object this[int index]
        {
            get
            {
                return (object)base[index];
            }
            set
            {
                base[index] = (T)value;
                RaiseChange(CollectionChange.ItemChanged, (uint)index);
            }
        }

        public void Add(object item)
        {
            base.Add((T)item);
            RaiseChange(CollectionChange.ItemInserted, (uint)(base.Count - 1));
        }

        public bool Contains(object item)
        {
            return base.Contains((T)item);
        }

        public void CopyTo(object[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(object item)
        {
            int index = base.IndexOf((T)item);
            if (index >= 0)
            {
                var res = base.Remove((T)item);
                RaiseChange(CollectionChange.ItemRemoved, (uint)index);
                return res;
            }

            return false;
        }

        private VectorEnumerator<T> _Enum;
        public new IEnumerator<object> GetEnumerator()
        {
            if (_Enum == null)
            {
                _Enum = new VectorEnumerator<T>(this);
            }

            return _Enum;
        }

        internal bool Any(Func<T, bool> func)
        {
            return base.Items.Any(func);
        }
    }

    public class VectorChangedEventArgs : IVectorChangedEventArgs
    {
        public VectorChangedEventArgs(CollectionChange type, uint index)
        {
            CollectionChange = type;
            Index = index;
        }
        public CollectionChange CollectionChange { get; set; }
        public uint Index { get; set; }
    }

    public class VectorEnumerator<T> : IEnumerator<object>
    {
        private IEnumerable<T> _Coll;
        private IEnumerator<T> _Enum;

        public VectorEnumerator(IEnumerable<T> internalColl)
        {
            _Coll = internalColl;
            _Enum = _Coll.GetEnumerator();
        }

        public object Current
        {
            get { return (object)_Enum.Current; }
        }


        public bool MoveNext()
        {
            return _Enum.MoveNext();
        }

        public void Reset()
        {
            _Enum.Reset();
        }

        public void Dispose()
        {
            _Enum.Dispose();
        }
    }
}
