using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Dijkstra
{
    public static List<Tuple<int, Status>> DijkstraAlgorithm(int start, int V, Graph graph)
    {
        List<Tuple<int, Status>> distWithStatus = new List<Tuple<int, Status>>();
        for (int i = 0; i < V; i++)
            distWithStatus.Add(new Tuple<int, Status>(int.MaxValue, new Status()));

        distWithStatus[start] = new Tuple<int, Status>(0, new Status());

        PriorityQueue<Tuple<int, int>> q = new PriorityQueue<Tuple<int, int>>((x, y) => x.Item1.CompareTo(y.Item1));
        q.Enqueue(new Tuple<int, int>(0, start));

        while (q.Count != 0)
        {
            int cost = -q.Peek().Item1;
            int from = q.Peek().Item2;
            q.Dequeue();

            if (distWithStatus[from].Item1 < cost)
                continue;

            foreach (var tuple in graph.list[from])
            {
                int to = tuple.Item1;
                int distFromTo = cost + 1;
                if (distFromTo < distWithStatus[to].Item1)
                {
                    distWithStatus[to] = new Tuple<int, Status>(distFromTo, distWithStatus[from].Item2 + tuple.Item2);
                    q.Enqueue(new Tuple<int, int>(-distFromTo, to));
                }
            }
        }
        return distWithStatus;
    }
}

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


public class LocationList : MonoBehaviour
{
    [SerializeField] private MoveCommand parentLocation;

    private Graph graph;
    private int locationCount;

    private void Awake()
    {
        locationCount = CountLocations(parentLocation);
        graph = new Graph(locationCount);
        PopulateGraph(parentLocation);
    }

    private int CountLocations(MoveCommand parent)
    {
        int count = 1;

        foreach (MoveCommand child in parent.ChildLocations)
            count += CountLocations(child); 

        return count;
    }

    private void PopulateGraph(MoveCommand parent)
    {
        foreach (MoveCommand child in parent.ChildLocations)
        {
            graph.AddEdge(parent, child);
            PopulateGraph(child);
        }
    }

    public List<Tuple<MoveCommand, Status>> CaculateAllPathsStatusFromName(string fromLocation)
    {
        int currentVertex = graph.GetVertexFromName(fromLocation);
        List<Tuple<int, Status>> shortestPaths = Dijkstra.DijkstraAlgorithm(currentVertex, locationCount, graph);

        List<Tuple<MoveCommand, Status>> result = new List<Tuple<MoveCommand, Status>>();
        for (int i = 0; i < shortestPaths.Count; i++)
        {
            if (shortestPaths[i].Item1 != int.MaxValue)
            {
                MoveCommand location = graph.locations[i];
                Status status = shortestPaths[i].Item2;
                result.Add(new Tuple<MoveCommand, Status>(location, status));
            }
            else
                Debug.LogError("찾지 못하는 경로가 존재하는데?");
        }

        return result;
    }


    public List<string> SearchNearLocationFromName(string fromLocation)
    {
        int currentVertex = graph.GetVertexFromName(fromLocation);
        List<string> nearLocations = new List<string>();

        foreach (var edge in graph.list[currentVertex])
        {
            nearLocations.Add(graph.locations[edge.Item1].CommandName);
        }

        return nearLocations;
    }
}