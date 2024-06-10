using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heap<T> where T : IHeapItem<T> {
    T[] items;
    int currentItemCount;
    public int Count =>
        currentItemCount;

    public Heap(int maxHeapSize)
        => items = new T[maxHeapSize];

    public void Add(T item) {
        item.heapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    public T RemoveFirst() {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].heapIndex = 0;
        SortDown(items[0]);

        return firstItem;
    }

    public void UpdateItem(T item)
        => SortUp(item);

    public bool Contains(T item)
        => Equals(items[item.heapIndex], item);

    private void SortUp(T item) {
        int parentIndex = (item.heapIndex - 1) / 2;

        while (true) {
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) <= 0)
                return;

            Swap(item, parentItem);
            parentIndex = ( item.heapIndex - 1 ) / 2;
        }
    }

    private void SortDown(T item) {
        while (true) {
            int childIndexL = (item.heapIndex * 2) + 1;
            int childIndexR = (item.heapIndex * 2) + 2;
            int swapIndex = 0;

            if (childIndexL >= currentItemCount)
                return;

            swapIndex = childIndexL;

            if (childIndexR < currentItemCount && items[childIndexL].CompareTo(items[childIndexR]) < 0)
                swapIndex = childIndexR;

            if (item.CompareTo(items[swapIndex]) >= 0)
                return;

            Swap(item, items[swapIndex]);
        }
    }

    private void Swap(T itemA, T itemB) {
        items[itemA.heapIndex] = itemB;
        items[itemB.heapIndex] = itemA;

        int itemAIndex = itemA.heapIndex;
        itemA.heapIndex = itemB.heapIndex;
        itemB.heapIndex = itemAIndex;
    }
}

public interface IHeapItem<T> : IComparable<T> {
    int heapIndex { get; set; }
}
