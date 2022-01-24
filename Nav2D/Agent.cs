using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nav2D
{
    public class Agent : MonoBehaviour
    {
        public GameObject gridGameObject;
        public PathRequestManager reqManager;
        public Rigidbody2D rb;
        public Transform target;
        public Vector3[] path = null;
		public int i = 0;
        public bool autoFollowPath = true;
        public bool autoRequestPath = true;
        public float speed;

        public System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

        void Start()
        {
            reqManager = gridGameObject.GetComponent<PathRequestManager>();
            rb = GetComponent<Rigidbody2D>();
            timer.Start();
            reqManager.RequestPath(this);
        }

        public void OnPathFound(Vector3[] path)
        {
            timer.Stop();
            //Debug.Log("TimeTaken: " + timer.Elapsed + " Result: " + ((path != null) ? "true" : "false"));
            if (path != null)
            {
				// merge path
				//List<Vector3> listpath = path.ToList();
				//if (listpath.Contains(this.path[i]))
				//{
				//	int index = listpath.IndexOf()
				//}

                this.path = path;
                if (autoFollowPath)
                {
                    StopCoroutine("FollowPath");
                    StartCoroutine("FollowPath");
                }
            }
            
            // I do not know why but the following code slows down the fps
            // but sometimes, it does not
            else StopCoroutine("FollowPath");
            if (autoRequestPath)
            {
                timer.Start();
                reqManager.RequestPath(this);
            }
        }

        IEnumerator FollowPath()
        {
            for (i = 0; i < path.Length; i++)
            {
                while (true)
                {
                    if ((transform.position - path[i]).magnitude < 0.4f) break;
                    Vector3 direction = path[i] - transform.position; direction.Normalize();
                    rb.velocity = direction * speed;
                    yield return null;
                }
                yield return null;
            }
            rb.velocity = Vector3.zero;
        }

        void Update()
        {

        }

        private void OnDrawGizmos()
        {
            if (path != null && path.Length != 0)
            {
                // draw path
                Gizmos.color = Color.yellow;
                for (int i = 0; i < path.Length - 1; i++)
                    Gizmos.DrawLine(path[i], path[i + 1]);
            }
        }
    }

}