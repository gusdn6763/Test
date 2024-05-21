using System.Collections.Generic;
using System;

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