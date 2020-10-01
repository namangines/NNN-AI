using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

namespace TankPathingSystem {
    public class WaypointManager : MonoBehaviour
    {
        public static WaypointManager Instance;

        private void Start()
        {
            Instance = this;
        }

        //Set in inspector
        public List<Waypoint> allWaypoints;

        private FastPriorityQueue<WaypointQueueable> queue = new FastPriorityQueue<WaypointQueueable>(20);

        #region access
        public Waypoint GetRandomWaypoint()
        {
            return allWaypoints[Random.Range(0, allWaypoints.Count)];
        }

        public Waypoint GetClosestWaypoint(Vector3 position)
        {
            Waypoint[] points;
            points = Object.FindObjectsOfType<Waypoint>();
            Waypoint closest = null;
            float closestdist = float.PositiveInfinity;

            foreach(Waypoint wayp in points)
            {
                float dist = Vector3.Distance(wayp.transform.position, position);
                if (dist < closestdist) 
                {
                    closest = wayp;
                    closestdist = dist;
                }
            }

            return closest;
        }

        #endregion

        #region pathing
        public void FindAllNeighbors()
        {
            foreach(Waypoint wayp in allWaypoints)
            {
                wayp.ResetNeighbors();
                wayp.FindNeighbors();
            }
        }

        public Stack<Waypoint> Path(Waypoint start, Waypoint end)
        {
            enqueue(start);
            WaypointQueueable endqueueable = null;

            while(queue.Count > 0)
            {
                WaypointQueueable headqueueable = dequeue();
                Waypoint head = headqueueable.self;

                foreach(NeighborConnection neighbor in head.neighbors.Values)
                {
                    WaypointQueueable neighborqueueable = null;
                    foreach(WaypointQueueable waypq in queue)
                    {
                        if(waypq.self = neighbor.neighbor)
                        {
                            neighborqueueable = waypq;
                            break;
                        }
                    }

                    if (neighborqueueable == null)
                    {
                        enqueue(neighbor.neighbor, headqueueable);
                    }
                    else
                    {
                        float newdist = totaldistance(neighbor.neighbor, headqueueable);
                        if (newdist < neighborqueueable.disttostart)
                        {
                            queue.UpdatePriority(neighborqueueable, newdist);
                        }
                    }

                    //If the end point is the next point, a path is found
                    if(neighbor.neighbor == end)
                    {
                        endqueueable = neighborqueueable;
                        break;
                    }
                }
            }

            if (endqueueable == null)
            {
                Debug.Log("No path found between " + start.hashname + " and " + end.hashname);
                Stack<Waypoint> stack = new Stack<Waypoint>();
                stack.Push(start);
                return stack;
            }
            else 
            {
                Stack<Waypoint> stack = new Stack<Waypoint>();
                stack.Push(end);

                WaypointQueueable next = endqueueable.from;
                while (next != null) //Will stop before pushing start
                {
                    stack.Push(next.self);
                    next = next.from;
                }

                stack.Push(start);
                return stack;
            }
        }


        private WaypointQueueable dequeue()
        {
            return queue.Dequeue();
        }
        private void enqueue(Waypoint self, WaypointQueueable from = null)
        {
            float dist = 0;
            if (from != null)
                dist = totaldistance(self, from); //the weight to the neighbor self from the waypoint from.self

            queue.Enqueue
            (
                new WaypointQueueable()
                {
                    self = self,
                    from = from,
                    disttostart = dist
                },
                dist
            );
        }

        private float totaldistance(Waypoint self, WaypointQueueable from)
        {
            return from.disttostart + from.self.neighbors[self].weight;
        }
        #endregion
    }


    public class WaypointQueueable : FastPriorityQueueNode
    {
        public Waypoint self;
        public WaypointQueueable from;
        public float disttostart;
    }
}


