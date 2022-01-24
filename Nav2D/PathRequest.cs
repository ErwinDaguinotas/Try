using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nav2D
{
    public class PathRequest
    {
        public Agent agent;
        public Vector3 from;
        public Vector3 to;
        public Vector3[] path;
        
        public PathRequest(Agent _agent, Vector3 _from, Vector3 _to)
        {
            agent = _agent;
            from = _from;
            to = _to;
        }
    }

    public class PathFindResult
    {
        public Agent agent;
        public Vector3[] path;

        public PathFindResult(Agent _agent, Vector3[] _path)
        {
            agent = _agent;
            path = _path;
        }
    }
}

