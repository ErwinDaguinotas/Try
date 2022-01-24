using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nav2D
{
    public class Node
    {
        public Vector2Int position;
        public Node parent;
        public bool walkable;
        public int gcost;
        public int hcost;
        public int weight;
        public string msg = "hello";

        public Node(Vector2Int position, Node parent =null, bool walkable=false, int gcost=0, int hcost=0, int weight=0)
        {
            this.position = position;
            this.parent = parent;
            this.walkable = walkable;
            this.gcost = gcost;
            this.hcost = hcost;
        }

        public int fcost { get { return gcost + hcost + weight; } }

        public Node Duplicate
        {
            get
            {
                return new Node(position, parent, walkable, gcost, hcost, weight);
            }
        }

        public bool IsCostLessThan(Node a)
        {
            if (fcost == a.fcost)
                if (hcost == a.hcost) return gcost < a.gcost;
                else return hcost < a.hcost;
            else return fcost < a.fcost;
        }

        public static int Cost(Node from, Node to)
        {
            int x = Mathf.Abs(to.position.x - from.position.x);
            int y = Mathf.Abs(to.position.y - from.position.y);
            int cost = Mathf.Max(x, y) * 10 + Mathf.Min(x, y) * (14 - 10);
            return cost;
        }
    }
}
