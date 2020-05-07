using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public static class PathFindScript
{
    static int[] parent; //-1 = start, -2 = end, -3 unvisited, -4 impassable
    static Queue<int> frontier;

    public static int[] Pathfind(MapScript map, int startX, int startY, int endX, int endY) {
        return Pathfind(map, startIndex:startX + startY * map.sizeX, endIndex:endX + endY * map.sizeX);
    }

    public static int[] Pathfind(MapScript map, int startX, int startY, GameObject endBuilding) {
        return Pathfind(map, startIndex: startX + startY * map.sizeX, endBuilding: endBuilding);
    }

    public static int[] Pathfind(MapScript map, int startIndex = -1, GameObject startBuilding = null, int endIndex = -1, GameObject endBuilding = null, bool useRoads = true) {

        //setup
        if (parent == null) parent = new int[map.sizeX * map.sizeY];
        if(!useRoads) for (int i = 0; i < parent.Length; i++) parent[i] = -3;
        else for (int i = 0; i < parent.Length; i++) parent[i] = map.tiles[i].road ? -3 : -4;
        if (frontier == null) frontier = new Queue<int>();
        else frontier.Clear();
        int found = -1;

        //starting points
        if (startBuilding == null) {
            parent[startIndex] = -1;
            frontier.Enqueue(startIndex);
        } else {
            BuildingScript buildingScript = startBuilding.GetComponent<BuildingScript>();
            for(int y = buildingScript.y; y < buildingScript.y + buildingScript.sizeY; y++) {
                for(int x = buildingScript.x; x < buildingScript.x + buildingScript.sizeX; x++) {
                    parent[x + y * map.sizeX] = -1;
                    frontier.Enqueue(x + y * map.sizeX);
                }
            }
        }

        //end points
        if (endBuilding == null) {
            parent[endIndex] = -2;
        } else {
            BuildingScript buildingScript = endBuilding.GetComponent<BuildingScript>();
            for (int y = buildingScript.y; y < buildingScript.y + buildingScript.sizeY; y++) {
                for (int x = buildingScript.x; x < buildingScript.x + buildingScript.sizeX; x++) {
                    parent[x + y * map.sizeX] = -2;
                }
            }
        }


        while (frontier.Count > 0) {
            int index = frontier.Dequeue();
            if (index % map.sizeX != map.sizeX - 1) ExpandNew(ref found, useRoads, index, index + 1);
            if (index % map.sizeX != 0)             ExpandNew(ref found, useRoads, index, index - 1);
            if (index / map.sizeX != map.sizeY - 1) ExpandNew(ref found, useRoads, index, index + map.sizeX);
            if (index / map.sizeX != 0)             ExpandNew(ref found, useRoads, index, index - map.sizeX);

            if (found != -1) {
                //Debug.Log("found path");

                //Get length of path
                int count = 1;
                int j = found;
                while (parent[j] != -1) {
                    j = parent[j];
                    count++;
                }
                int[] path = new int[count];
                //fill path
                for (int i = path.Length - 1; i >= 0; i--) {
                    path[i] = found;
                    found = parent[found];
                }
                return path;
            }
        }
        return null;
    }

    public static void ExpandNew(ref int found, bool useRoads, int newParent, int i) {
        if (parent[i] > -2) return;
        else if (parent[i] == -2) {
            found = i;
            parent[i] = newParent;
        } else if (parent[i] == -3) {
            parent[i] = newParent;
            frontier.Enqueue(i);
        }
    }

    public static GameObject DrawPath(MapScript region, GameObject parent, int[] path) {
        Vector3[] positions = new Vector3[path.Length];
        for (int i = 0; i < path.Length; i++)
        {
            int x = path[i] % region.sizeX; int y = path[i] / region.sizeX;
            positions[i] = new Vector3((x - y) * 0.5f, (x + y) * .25f +.25f, 0);
        }
        GameObject line = new GameObject("path");
        line.transform.SetParent(parent.transform);
        line.transform.localPosition = Vector3.zero;
        LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.positionCount = path.Length;
        lineRenderer.widthMultiplier = .1f;
        lineRenderer.SetPositions(positions);
        return line;
    }
       
    
}
