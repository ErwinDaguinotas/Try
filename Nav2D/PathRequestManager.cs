using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

namespace Nav2D
{
    public class PathRequestManager : MonoBehaviour
    {
        public NodeGrid grid = new NodeGrid();

        public Queue<PathRequest> pathRequests = new Queue<PathRequest>();
        public Queue<PathRequest> results = new Queue<PathRequest>();
        public int runningRequests = 0;
        public int maxRunningRequests = 5;

        [Header("DEBUGGING")]
        public bool autoCreateGrid = false;
        public bool alwaysShowGizmos = false;

        public bool findingpath = false;


        private void Awake()
        {
            grid.CreateGrid();
        }

        void Start()
        {
        }

        // ===PATH REQUEST SYSTEM===
        public void RequestPath(Agent agent)
        {
            // call to register a request
            PathRequest req = new PathRequest(agent, agent.transform.position, agent.target.position);
            lock (pathRequests)
            {
                pathRequests.Enqueue(req);
            }
        }

        public void DoPathRequests()
        {
            // will be called every frame
            // creates a new thread where the request will be processed
            if (pathRequests.Count > 0 && runningRequests < maxRunningRequests)
            {
                PathRequest req;
                lock (pathRequests)
                {
                    req = pathRequests.Dequeue();
                }
                Vector3 from = req.agent.transform.position;
                Vector3 to = req.agent.target.transform.position;

                Thread t = new Thread(() =>
                {
                    PathFinder.GetPath(new NodeGrid(grid), req, QueueResult);
                });
                t.Start();
                runningRequests++;
            }
        }

        public void QueueResult(PathRequest req)
        {
            lock(results)
            {
                results.Enqueue(req);
            }
            runningRequests--;
        }

        public void DistributeResults()
        {
            while (results.Count > 0)
            {
                PathRequest req;
                lock(results)
                {
                    req = results.Dequeue();
                }
                req.agent.OnPathFound(req.path);
            }
        }

        void Update()
        {
            DoPathRequests();
            DistributeResults();
        }

        private void OnDrawGizmos()
        {
            if (autoCreateGrid) grid.CreateGrid();
            if (alwaysShowGizmos || UnityEditor.Selection.activeGameObject == this.gameObject)
                for (int x = 0; x < grid.dimension.x; x++)
                {
                    for (int y = 0; y < grid.dimension.y; y++)
                    {
                        Vector3 worldPos = grid.GridToWorldPosition(new Vector2Int(x, y));
                        if ((autoCreateGrid || grid.grid != null) && !grid.grid[x, y].walkable)
                        {
                            Gizmos.color = new Color(1, 0, 0, 0.5f);
                            Gizmos.DrawCube(worldPos, grid.nodesize);
                        }
                        Gizmos.color = new Color(1f, 1f, 1f, 0.1f);
                        Gizmos.DrawWireCube(worldPos, grid.nodesize);
                    }
                }
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(grid.position, grid.size);
        }
    }
}
