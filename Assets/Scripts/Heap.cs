using System.Collections;
using System.Collections.Generic;
using System;

public class Heap<T> where T : IHeapItem<T>
{
    T[] Items;
    int m_currentItemCount;
    

    public Heap(int maxSize)
    {
        Items = new T[maxSize + 1];
    }

    public void Add(T item)
    {
        item.Index = m_currentItemCount;
        Items[m_currentItemCount] = item;
        HeapSortUp(item);
        m_currentItemCount++;
    }

    public T RemoveFirst()
    {
        T _firstItem = Items[0];
        m_currentItemCount--;
        Items[0] = Items[m_currentItemCount];
        Items[0].Index = 0;
        HeapSortDown(Items[0]);
        return _firstItem;
    }
    public bool Contains(T item)
    {
        return Equals(Items[item.Index], item);
    }

    public void UpdateItem(T item)
    {
        HeapSortUp(item);
        
    }

    public int Count
    {
        get
        {
            return m_currentItemCount;
        }
    }

    void HeapSortDown(T item)
    {
        while (true)
        {
            int _leftIndex = item.Index * 2 + 1;
            int _rightIndex = item.Index * 2 + 2;
            int _swapIndex = 0;

            if (_leftIndex < Count)
            {
                _swapIndex = _leftIndex;

                if (_rightIndex < Count)
                {
                    if (Items[_leftIndex].CompareTo(Items[_rightIndex]) < 0)
                    {
                        _swapIndex = _rightIndex;
                    }
                }

            if (item.CompareTo(Items[_swapIndex]) < 0)
            {
                Swap(item, Items[_swapIndex]);
            }
            else
                return;
            }
            else return;
        }
    }

    void HeapSortUp(T item)
    {
        int _parentIndex = (item.Index - 1) / 2;

        while (true)
        {
            T _parentItem = Items[_parentIndex];
            //If the item value is higher
            if (item.CompareTo(_parentItem) > 0)
            {
                //Move it up in the heap
                Swap(item, _parentItem);
            }
            else
                break;

            _parentIndex = (item.Index - 1) / 2;
        }
    }

    void Swap(T itemA, T itemB)
    {
        Items[itemA.Index] = itemB;
        Items[itemB.Index] = itemA;
        int _itemAIndex = itemA.Index;
        itemA.Index = itemB.Index;
        itemB.Index = _itemAIndex;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int Index
    {
        get;
        set;
    }
}
