using System.Collections.Generic;
using System;

public class Graph
{
    public List<Tuple<int, Status>>[] list;
    public List<VillageMoveData> locations;

    public Graph(int vertexCount)
    {
        list = new List<Tuple<int, Status>>[vertexCount];
        locations = new List<VillageMoveData>(vertexCount);

        for (int i = 0; i < vertexCount; i++)
            list[i] = new List<Tuple<int, Status>>();
    }

    public void AddEdge(VillageMoveData from, VillageMoveData to)
    {
        if (locations.Contains(from) == false)
            locations.Add(from);

        if (locations.Contains(to) == false)
            locations.Add(to);

        list[from.vertex].Add(new Tuple<int, Status>(to.vertex, new Status(to.status)));
        list[to.vertex].Add(new Tuple<int, Status>(from.vertex, new Status(to.status)));
    }

    public int GetVertexFromName(string locationName)
    {
        for (int i = 0; i < locations.Count; i++)
        {
            if (locations[i].commandName == locationName)
                return locations[i].vertex;
        }

        return -1;
    }
}