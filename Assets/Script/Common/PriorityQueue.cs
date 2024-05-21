using System.Collections.Generic;
using System;

public class PriorityQueue<T>
{
    private List<T> data;
    private Comparison<T> comparison;

    public PriorityQueue(Comparison<T> comparison)
    {
        this.data = new List<T>();
        this.comparison = comparison;
    }

    public void Enqueue(T item)
    {
        data.Add(item);
        int ci = data.Count - 1;
        while (ci > 0)
        {
            int pi = (ci - 1) / 2;
            if (comparison(data[ci], data[pi]) >= 0) break;
            T tmp = data[ci]; data[ci] = data[pi]; data[pi] = tmp;
            ci = pi;
        }
    }

    public T Dequeue()
    {
        if (data.Count == 0)
            throw new InvalidOperationException("Queue is empty.");

        T frontItem = data[0];
        data[0] = data[data.Count - 1];
        data.RemoveAt(data.Count - 1);

        int ci = 0;
        while (ci < data.Count)
        {
            int li = 2 * ci + 1, ri = 2 * ci + 2;
            if (li >= data.Count) break;
            int mi = ri >= data.Count || comparison(data[li], data[ri]) < 0 ? li : ri;
            if (comparison(data[ci], data[mi]) <= 0) break;
            T tmp = data[ci]; data[ci] = data[mi]; data[mi] = tmp;
            ci = mi;
        }
        return frontItem;
    }

    public T Peek()
    {
        if (data.Count == 0) throw new InvalidOperationException("Queue is empty.");
        return data[0];
    }

    public int Count
    {
        get { return data.Count; }
    }
}