using System;
using System.Collections.Generic;
using UnityEngine;

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

    public void CaculateAllMoveCommandStatus(List<MoveCommand> moveCommands, MoveCommand command)
    {
        List<Tuple<MoveCommand, Status>> status = CaculateAllPathsStatusFromName(command.CommandName);

        for (int i = 0; i < moveCommands.Count; i++)
        {
            MoveCommand childCommand = moveCommands[i];

            if (childCommand.Found == false)
                continue;

            foreach (var tuple in status)
            {
                MoveCommand location = tuple.Item1;
                Status locationStatus = tuple.Item2;

                if (childCommand.CommandName == location.CommandName)
                {
                    childCommand.MyStatus = locationStatus;
                    break;
                }
            }
        }

        List<string> listTmp = new List<string>();
        List<string> list = SearchNearLocationFromName(command.CommandName);
        for (int j = 0; j < list.Count; j++)
        {
            if (FindLocation(list[j], moveCommands))
                listTmp.Add("경로 발견 : " + list[j]);
        }

        if (listTmp.Count > 0)
            Player.instance.ShowIntroduce(listTmp);
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

    public bool FindLocation(string locationName, List<MoveCommand> moveCommands)
    {
        foreach (MoveCommand childCommand in moveCommands)
        {
            if (childCommand.CommandName == locationName)
            {
                if (childCommand.Found)
                    return false;

                childCommand.Found = true;
                return true;
            }
        }
        return false;
    }
}