using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathFindScript
{
	public static int[] Pathfind(MapScript map, int startX, int startY, int endX, int endY, bool useBuildings, bool startOnRoad = false, bool ignoreRoads = false) {
        //Debug.Log(string.Format("Pathfinding from ({0},{1}) to ({2},{3})", startX, startY, endX, endY));
        int startIndex = startX + startY * map.sizeX;
        int endIndex = endX + endY * map.sizeX;
		GameObject startBuilding = null; GameObject endBuilding = null;
		if(useBuildings) { startBuilding = map.GetTile(startX, startY).building; endBuilding = map.GetTile(endX, endY).building; }

        Dictionary<int, PathfindNode> nodes = new Dictionary<int, PathfindNode>();
        nodes.Add(startIndex, new PathfindNode(startIndex, 0, null, startOnRoad));

        Queue<int> frontier = new Queue<int>();
        frontier.Enqueue(startX + startY * map.sizeX);
        while (frontier.Count > 0)
        {
            int currentLocation = frontier.Dequeue();
			bool leftStart = false;
			//LEFT
			if (currentLocation % map.sizeX != 0) {
                int i = currentLocation - 1;
                if (nodes[currentLocation].leftStart && (i == endIndex || (useBuildings && map.GetTile(i % map.sizeX, i / map.sizeX).building == endBuilding))) {
                    PathfindNode endNode = new PathfindNode(i, nodes[currentLocation].distance + 1, nodes[currentLocation], true);
                    return endNode.ReturnPath(useBuildings);
                } else if (!nodes.ContainsKey(i) && (ignoreRoads || IsRoad(map, i, startBuilding, endBuilding, nodes[currentLocation].leftStart, out leftStart))) {  //NEED TO UPDATE FOR COSTS LATER
                    nodes[i] = new PathfindNode(i, nodes[currentLocation].distance + 1, nodes[currentLocation], leftStart);
                    frontier.Enqueue(i);
                }
            }
            //Right
            if (currentLocation % map.sizeX != map.sizeX - 1)
            {
                int i = currentLocation + 1;
				if (nodes[currentLocation].leftStart && (i == endIndex || (useBuildings && map.GetTile(i % map.sizeX, i / map.sizeX).building == endBuilding))) {
					PathfindNode endNode = new PathfindNode(i, nodes[currentLocation].distance + 1, nodes[currentLocation], true);
                    return endNode.ReturnPath(useBuildings);
                } else if (!nodes.ContainsKey(i) && (ignoreRoads || IsRoad(map, i, startBuilding, endBuilding, nodes[currentLocation].leftStart, out leftStart))) {  //NEED TO UPDATE FOR COSTS LATER
                    nodes[i] = new PathfindNode(i, nodes[currentLocation].distance + 1, nodes[currentLocation], leftStart);
                    frontier.Enqueue(i);
                }
            }
            //DOWN
            if (currentLocation / map.sizeX != 0)
            {
                int i = currentLocation - map.sizeY;
				if (nodes[currentLocation].leftStart && (i == endIndex || (useBuildings && map.GetTile(i % map.sizeX, i / map.sizeX).building == endBuilding))) {
					PathfindNode endNode = new PathfindNode(i, nodes[currentLocation].distance + 1, nodes[currentLocation], true);
                    return endNode.ReturnPath(useBuildings);
                } else if (!nodes.ContainsKey(i) && (ignoreRoads || IsRoad(map, i, startBuilding, endBuilding, nodes[currentLocation].leftStart, out leftStart))) {  //NEED TO UPDATE FOR COSTS LATER
                    nodes[i] = new PathfindNode(i, nodes[currentLocation].distance + 1, nodes[currentLocation], leftStart);
                    frontier.Enqueue(i);
                }
            }
            //UP
            if (currentLocation / map.sizeX != map.sizeY - 1)
            {
                int i = currentLocation + map.sizeX;
				if (nodes[currentLocation].leftStart && (i == endIndex || (useBuildings && map.GetTile(i % map.sizeX, i / map.sizeX).building == endBuilding))) {
					PathfindNode endNode = new PathfindNode(i, nodes[currentLocation].distance + 1, nodes[currentLocation], true);
                    return endNode.ReturnPath(useBuildings);
                } else if (!nodes.ContainsKey(i) && (ignoreRoads || IsRoad(map, i, startBuilding, endBuilding, nodes[currentLocation].leftStart, out leftStart))) {  //NEED TO UPDATE FOR COSTS LATER
                    nodes[i] = new PathfindNode(i, nodes[currentLocation].distance + 1, nodes[currentLocation], leftStart);
                    frontier.Enqueue(i);
                }
            }
            nodes[currentLocation].expanded = true;
        }
        //Debug.Log("Couldn't find path");
        return null;
    }

    /*
    static bool Expand(int i, int endIndex, int currentLocation, Dictionary<int, PathfindNode> nodes) {
        if (i == endIndex) {
            PathfindNode endNode = new PathfindNode(i, nodes[currentLocation].distance + 1, nodes[currentLocation], true);
            return endNode.ReturnPath(false);
        } else if (!nodes.ContainsKey(i)) {  //NEED TO UPDATE FOR COSTS LATER
            nodes[i] = new PathfindNode(i, nodes[currentLocation].distance + 1, nodes[currentLocation], leftStart);
            frontier.Enqueue(i);
        }
    }
    */



    public static int[] PathfindNew(MapScript map, int startX, int startY, int endX, int endY) {
        //Debug.Log(System.String.Format("Pathfinding from {0},{1} to {2},{3}", startX, startY, endX, endY));
        return PathfindNew(map, startX + startY * map.sizeX, endX + endY * map.sizeX);
    }

    public static int[] PathfindNew(MapScript map, int startIndex, int endIndex) {
        int[] parent = new int[map.sizeX * map.sizeY];
        Queue<int> frontier = new Queue<int>();
        frontier.Enqueue(startIndex);
        while(frontier.Count > 0) {
            int i = frontier.Dequeue();
            if (i == endIndex) {
                //Debug.Log("found path");
                List<int> path = new List<int>();
                path.Add(i);
                while(i != startIndex) {
                    i = parent[i];
                    path.Add(i);
                }
                path.Reverse();
                return path.ToArray();
            }
            if (i % map.sizeX != map.sizeX - 1) ExpandNew(i + 1, i, parent, frontier);
            if (i % map.sizeX != 0)             ExpandNew(i - 1, i, parent, frontier);
            if (i / map.sizeX != map.sizeY - 1) ExpandNew(i + map.sizeX, i, parent, frontier);
            if (i / map.sizeX != 0)             ExpandNew(i - map.sizeX, i, parent, frontier);
        }
        return new int[0];
    }

    public static void ExpandNew(int i, int oldI, int[] parent, Queue<int> frontier) {
        if (parent[i] != 0) return;
        parent[i] = oldI;
        frontier.Enqueue(i);
    }



    /*
    static int CheckRoad(MapScript map, int i, int endIndex, Dictionary<int, PathfindNode> nodes)
    {
        if (i == endIndex)
        {
            return 2; //FOUND EXIT
        }
        else if (map.GetTiles(i % map.sizeX, i / map.sizeX].gameObject.tag == "Road" && nodes[i) == null)
        {  //NEED TO UPDATE FOR COSTS LATER
            return 1; //FOUND ROAD
        }
        return 0;//FOUND ??
    }
    */

    static bool IsRoad(MapScript map, int i, GameObject startBuilding, GameObject endBuilding, bool parentLeftStart, out bool leftStart) {
		GameObject building = map.GetTile(i % map.sizeX, i / map.sizeX).building;
		leftStart = parentLeftStart;
		if (building != null) {
			if (building.CompareTag("Road")) { leftStart = true; return true; }
			if (!parentLeftStart && building == startBuilding) return true;
			if (parentLeftStart && building == endBuilding) return true;
		}
		//if (building != null && (building.tag == "Road" || building == startBuilding || building == endBuilding)) return true;
		return false;
    }

    class PathfindNode {
        int index;
        public int distance;
        public bool expanded;
		public bool leftStart;
        PathfindNode parent;
        public PathfindNode(int index, int distance, PathfindNode parent, bool leftStart) {
            this.index = index; this.distance = distance; this.parent = parent; this.expanded = false; this.leftStart = leftStart;
        }

        public int[] ReturnPath(bool useBuildings){

            List<int> returnList = new List<int>();
			returnList.Add(index);
			PathfindNode next = parent;
			while (next != null) {
				returnList.Add(next.index);
				if (useBuildings && !next.leftStart) next = null; else next = next.parent;
			}
			returnList.Reverse();
			return returnList.ToArray();
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
