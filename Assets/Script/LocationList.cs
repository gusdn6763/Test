using System;
using System.Collections.Generic;
using UnityEngine;

public class LocationList : MonoBehaviour
{
    [SerializeField] private MoveCommand parentLocation;

    //장소 고유 행동 ex) 아이템, Npc
    [SerializeField] private List<LocationCommandList> locationCommandLists = new List<LocationCommandList>();

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
            LocationCommandList parentLocationCommand = locationCommandLists.Find(x => x.moveCommand == parent);
            LocationCommandList childLocationCommand = locationCommandLists.Find(x => x.moveCommand == child);

            if (parentLocationCommand != null && childLocationCommand != null)
                graph.AddEdge(parent, child, parentLocationCommand.status, childLocationCommand.status);

            PopulateGraph(child);
        }
    }

    public void CaculateAllMoveCommandStatus(MoveCommand command)
    {
        List<Tuple<MoveCommand, Status>> status = CaculateAllPathsStatusFromName(command.CommandName);

        for (int i = 0; i < locationCommandLists.Count; i++)
        {
            MoveCommand childCommand = locationCommandLists[i].moveCommand;
            for (int j = 0; j < status.Count; j++)
            {
                MoveCommand location = status[j].Item1;
                Status locationStatus = status[j].Item2;
                if (childCommand.CommandName == location.CommandName)
                {
                    childCommand.MyStatus = locationStatus;
                    break;
                }
            }
        }
        SearchNearLocation(command);
    }

    public void SearchNearLocation(MoveCommand command)
    {
        List<string> listTmp = new List<string>();
        List<string> list = FindNearLocationFromName(command.CommandName);
        for (int j = 0; j < list.Count; j++)
        {
            if (FindLocation(list[j]))
                listTmp.Add("경로 발견 : " + list[j]);
        }

        if (listTmp.Count > 0)
            Player.instance.ShowIntroduce(listTmp);
    }

    public List<string> FindNearLocationFromName(string fromLocation)
    {
        int currentVertex = graph.GetVertexFromName(fromLocation);
        List<string> nearLocations = new List<string>();

        foreach (var edge in graph.list[currentVertex])
        {
            nearLocations.Add(graph.locations[edge.Item1].command2);
        }

        return nearLocations;
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

    public bool FindLocation(string locationName)
    {
        foreach (LocationCommandList childCommand in locationCommandLists)
        {
            if (childCommand.moveCommand.command2 == locationName)
            {
                if (childCommand.moveCommand.Found)
                    return false;

                childCommand.moveCommand.Found = true;
                return true;
            }
        }
        return false;
    }

    public LocationCommandList GetLocationCommandList(MoveCommand moveCommand)
    {
        return locationCommandLists.Find(x => x.moveCommand == moveCommand);
    }
}