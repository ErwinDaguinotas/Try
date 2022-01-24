using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nav2D
{
    public static class PathFinder
    {
        public static void GetPath(NodeGrid grid, PathRequest req, Action<PathRequest> callback)
        {
            //Debug.Log(req.from + " " + req.to);
            Node start = grid.NodeAt(grid.WorldToGridPosition(req.from));
            Node end = grid.NodeAt(grid.WorldToGridPosition(req.to));
            if ((!start.walkable || !end.walkable) || (start == null || end == null))
            {
                req.path = null;
                callback(req);
                return;
            }
            start.parent = null;
            start.gcost = 0;
            start.hcost = Node.Cost(start, end);

            List<Node> open = new List<Node>();
            List<Node> closed = new List<Node>();
            open.Add(start);

            while (true)
            {
                Node cnode = open[0];
                foreach (Node n in open) if (n.IsCostLessThan(cnode)) cnode = n;
                open.Remove(cnode);
                closed.Add(cnode);

                if (cnode.position == end.position)
                {
                    //return null;
                    req.path = RetracePath(grid, start, end);
                    callback(req);
                    return;
                }

                foreach (Node neighbour in grid.GetNeighbours(cnode))
                {
                    if (closed.Contains(neighbour)) continue;

                    int offerG = cnode.gcost + Node.Cost(cnode, neighbour);
                    if (open.Contains(neighbour))
                    {
                        if (offerG < neighbour.gcost)
                        {
                            neighbour.gcost = offerG;
                            neighbour.parent = cnode;
                        }
                    }
                    else
                    {
                        neighbour.gcost = offerG;
                        neighbour.hcost = Node.Cost(neighbour, end);
                        neighbour.parent = cnode;
                        open.Add(neighbour);
                    }
                }

                if (open.Count == 0)
                {
                    req.path = null;
                    callback(req);
                    return;
                }
            }
        }

        public static Vector3[] RetracePath(NodeGrid grid, Node start, Node end)
        {
            List<Vector3> pathList = new List<Vector3>();
            while (true)
            {
                pathList.Add(grid.GridToWorldPosition(end.position));
                if (end.parent == null) break;
                else end = end.parent;
            }
            Vector3[] path = pathList.ToArray();
            Array.Reverse(path);
            return path;
        }
    }
}