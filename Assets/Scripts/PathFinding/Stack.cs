using System;
using System.Collections.Generic;

public class Stack<T> {
    T[] items;
    int currentItemCount;

    public int Count => currentItemCount;

    public Stack(int maxStackSize) {
        items = new T[maxStackSize];
        currentItemCount = 0;
    }

    public void Push(T item) {
        if (currentItemCount >= items.Length) {
            throw new InvalidOperationException("Stack overflow");
        }
        items[currentItemCount] = item;
        currentItemCount++;
    }

    public T Pop() {
        if (currentItemCount == 0) {
            throw new InvalidOperationException("Stack underflow");
        }
        currentItemCount--;
        return items[currentItemCount];
    }

    public T Peek() {
        if (currentItemCount == 0) {
            throw new InvalidOperationException("Stack is empty");
        }
        return items[currentItemCount - 1];
    }

    public bool Contains(T item) {
        for (int i = 0; i < currentItemCount; i++) {
            if (EqualityComparer<T>.Default.Equals(items[i], item)) {
                return true;
            }
        }
        return false;
    }

}
