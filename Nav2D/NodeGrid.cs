using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Nav2D
{
    [Serializable]
    public class NodeGrid
    {
        public Vector2 position = Vector2.zero;
        public Vector2 size = Vector2.one;
        public Vector2 nodesize = Vector2.one;
        public Vector2Int dimension;

        public Node[,] grid = null;

        public LayerMask nonWalkableMask;

        // ===CONSTRUCTORS===
        public NodeGrid() 
        {
        }

        public NodeGrid(NodeGrid nodegrid)
        {
            position = nodegrid.position;
            size = nodegrid.size;
            nodesize = nodegrid.nodesize;
            dimension = nodegrid.dimension;

            nonWalkableMask = nodegrid.nonWalkableMask;

            grid = new Node[dimension.x, dimension.y];

            for (int i=0; i<dimension.x; i++)
            {
                for (int j=0; j<dimension.y; j++)
                {
                    grid[i, j] = nodegrid.grid[i, j].Duplicate;
                }
            }
        }


        // ===FUNCTIONS===
        public void CreateGrid()
        {
            dimension = Vector2Int.FloorToInt(size / nodesize);
            dimension.x -= (dimension.x % 2 == 0) ? 0 : 1;
            dimension.y -= (dimension.y % 2 == 0) ? 0 : 1;
            Node[,] newgrid = new Node[dimension.x, dimension.y];
            for (int x = 0; x < dimension.x; x++)
            {
                for (int y = 0; y < dimension.y; y++)
                {
                    Vector3 worldPos = GridToWorldPosition(new Vector2Int(x, y));
                    Node node = new Node(new Vector2Int(x, y));

                    Collider2D hit = Physics2D.OverlapBox(worldPos, nodesize, 0, nonWalkableMask);

                    if (hit != null)
                    {
                        node.walkable = false;
                    }
                    else node.walkable = true;

                    newgrid[x, y] = node;
                }
            }
            grid = newgrid;
        }

        public Node NodeAt(Vector2Int position) 
        {
            if (!PositionInsideGrid(position)) return null;
            return grid[position.x, position.y]; 
        }

        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (i == 0 && j == 0) continue;

                    int x = i + node.position.x;
                    int y = j + node.position.y;

                    if (!PositionInsideGrid(new Vector2Int(x, y))) continue;
                    Node neighbour = NodeAt(new Vector2Int(x, y));
                    if (!neighbour.walkable) continue;

                    neighbours.Add(neighbour);
                }
            }
            return neighbours;
        }

        public bool PositionInsideGrid(Vector2Int position)
        {
            return (position.x >= 0 && position.x < dimension.x && position.y >= 0 && position.y < dimension.y);
        }

        public Vector3 GridToWorldPosition(Vector2Int gridPosition, int getCenter = 1)
        {
            Vector3 pos = (Vector2)(gridPosition - dimension / 2);
            pos.x = pos.x * nodesize.x + nodesize.x * getCenter / 2;
            pos.y = pos.y * nodesize.y + nodesize.y * getCenter / 2;
            pos = pos + (Vector3) position;
            return pos;
        }

        public Vector2Int WorldToGridPosition(Vector3 worldPosition)
        {
            Vector2 pos = (Vector2) worldPosition - position;
            pos.x = pos.x / nodesize.x;
            pos.y = pos.y / nodesize.y;
            pos += dimension / 2;
            return Vector2Int.FloorToInt(pos);
        }


        // ===GIZMOS===
        public void DrawGizmos()
        {
            for (int x = 0; x < dimension.x; x++)
            {
                for (int y = 0; y < dimension.y; y++)
                {
                    Vector3 worldPos = GridToWorldPosition(new Vector2Int(x, y));
                    if (grid != null && !grid[x, y].walkable)
                    {
                        Gizmos.color = new Color(1, 0, 0, 0.5f);
                        Gizmos.DrawCube(worldPos, nodesize);
                    }
                    Gizmos.color = new Color(1f, 1f, 1f, 0.1f);
                    Gizmos.DrawWireCube(worldPos, nodesize);
                }
            }
        }
    }

}