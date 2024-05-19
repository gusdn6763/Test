using System.Collections.Generic;
using System;

public class Graph
{
    public List<Tuple<int, Status>>[] list;
    public List<MoveCommand> locations;

    public Graph(int vertexCount)
    {
        list = new List<Tuple<int, Status>>[vertexCount];
        locations = new List<MoveCommand>(vertexCount);

        for (int i = 0; i < vertexCount; i++)
            list[i] = new List<Tuple<int, Status>>();
    }

    public void AddEdge(MoveCommand from, MoveCommand to)
    {
        if (locations.Contains(from) == false)
            locations.Add(from);

        if (locations.Contains(to) == false)
            locations.Add(to);

        list[from.Vertex].Add(new Tuple<int, Status>(to.Vertex, new Status(to.MyStatus)));
        list[to.Vertex].Add(new Tuple<int, Status>(from.Vertex, new Status(to.MyStatus)));
    }

    public int GetVertexFromName(string locationName)
    {
        for (int i = 0; i < locations.Count; i++)
        {
            if (locations[i].CommandName == locationName)
                return locations[i].Vertex;
        }

        return -1;
    }
}