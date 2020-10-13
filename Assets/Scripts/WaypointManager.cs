using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using UnityEngine.Experimental.Rendering;

namespace TankPathingSystem
{
    [ExecuteInEditMode]
    public class WaypointManager : MonoBehaviour
    {
        public static WaypointManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        //Set in inspector or hit button to get children
        public List<Waypoint> allWaypoints;


        #region access
        public Waypoint GetRandomWaypoint(Waypoint exclude = null)
        {
            Waypoint random;
            do
            {
                random = allWaypoints[Random.Range(0, allWaypoints.Count)];
            }
            while (random.WaypointID == exclude.WaypointID || random.CompareTag("Offduty Area") || random.CompareTag("Repair Area"));

            return random;
        }

        public Waypoint GetClosestWaypoint(Vector3 position)
        {
            Waypoint[] points;
            points = Object.FindObjectsOfType<Waypoint>();
            Waypoint closest = null;
            float closestdist = float.PositiveInfinity;

            foreach (Waypoint wayp in points)
            {
                float dist = Vector3.Distance(wayp.transform.position, position);
                RaycastHit hit = new RaycastHit();
                if (dist < closestdist && Physics.Raycast(wayp.transform.position, position - wayp.transform.position, out hit, wayp.neighborSearchRadius, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                {
                    if (Vector3.Distance(hit.transform.position, position) < 5f)
                    {
                        closest = wayp;
                        closestdist = dist;
                    }
                }
            }

            if (closest == null) //If none are found just give the closest waypoint
            {
                foreach (Waypoint wayp in points)
                {
                    float dist = Vector3.Distance(wayp.transform.position, position);
                    if (dist < closestdist)
                    {
                        closest = wayp;
                        closestdist = dist;
                    }
                }
            }
            return closest;
        }

        #endregion

        #region pathing

        public void AddChildrenWaypoints()
        {
            allWaypoints = new List<Waypoint>();
            Waypoint[] list = this.transform.GetComponentsInChildren<Waypoint>();
            foreach (Waypoint w in list)
            {
                allWaypoints.Add(w);
                w.WaypointID = allWaypoints.IndexOf(w);
            }
        }
        public void RecalculateWaypointWeights()
        {
            foreach (Waypoint wayp in allWaypoints)
            {
                wayp.RecalculateWeights();
            }
        }
        public void FindAllNeighbors()
        {
            foreach (Waypoint wayp in allWaypoints)
            {
                wayp.ResetNeighbors();
                wayp.FindNeighbors();
            }
        }

        public Stack<Waypoint> Path(Vector3 start, Vector3 end)
        {
            return Path(GetClosestWaypoint(start), GetClosestWaypoint(end));
        }
        public Stack<Waypoint> Path(Waypoint start, Waypoint end)
        {
            FastPriorityQueue<WaypointQueueable> queue = new FastPriorityQueue<WaypointQueueable>(128);
            //Debug.Log("Starting path algorithm from " + start.hashname + " to " + end.hashname);
            //Debug.Log("Enqueue start of " + start.hashname);
            enqueue(queue, start);
            List<int> alreadyAddedWaypoints = new List<int>();
            alreadyAddedWaypoints.Add(start.WaypointID);

            WaypointQueueable endqueueable = null;

            bool endFound = false;
            while (queue.Count > 0 && !endFound)
            {
                if (queue.Count > 127)
                {
                    Debug.LogError("Catastrophic pathing falure: Queue length exceeded maximum. Aborting loop!");
                    break;
                }
                WaypointQueueable headqueueable = dequeue(queue);
                Waypoint head = headqueueable.self;
                //Debug.Log("Dequeue " + head.hashname);
                //Debug.Log("Number of neighbors: " + head.neighbors.Count);

                for (int i = 0; i < head.neighbors.Count; i++)
                {
                    NeighborConnection neighbor = head.neighbors[i];//This cant be foreach because you can't (break;) foreach *grumble grumble*
                    //Debug.Log("Checking neighbor " + neighbor.neighbor.hashname);
                    WaypointQueueable neighborqueueable = null;

                    foreach (WaypointQueueable waypq in queue)
                    {
                        if (waypq.self.WaypointID == neighbor.neighbor.WaypointID)
                        {
                            //Debug.Log("Neighbor already found in queue");
                            neighborqueueable = waypq;
                            break;//techincally I think this doesnt do anything
                        }
                    }

                    bool addedBefore = false;
                    foreach (int addedID in alreadyAddedWaypoints) // alreadyAddedWaypoints.Contains was not functioning for some reason so I just did it manually
                    {
                        if (addedID == neighbor.neighborID)
                            addedBefore = true;
                    }

                    if (neighborqueueable == null && !addedBefore)
                    {
                        //Debug.Log("New neighbor, enqueueing " + neighbor.neighbor.name);
                        neighborqueueable = enqueue(queue, neighbor.neighbor, headqueueable);
                        alreadyAddedWaypoints.Add(neighbor.neighborID);
                    }
                    else if (neighborqueueable != null)
                    {
                        //Debug.Log("Already exists; Checking if should update weight");
                        float newdist = totaldistance(neighbor.neighbor, headqueueable);
                        if (newdist < neighborqueueable.disttostart)
                        {
                            //Debug.Log("Updating weight");
                            queue.UpdatePriority(neighborqueueable, newdist);
                        }
                    }
                    else
                    {
                        //Debug.Log("This waypoint has already been added at some point; will not add");
                    }


                    //If the end point is the next point, a path is found
                    if (neighbor.neighbor.WaypointID == end.WaypointID)
                    {
                        //Debug.Log("End detected");
                        endqueueable = neighborqueueable; //Should always be the first time this node is found so this should always be set to something
                        endFound = true;
                        break;
                    }
                }
            }

            if (endqueueable == null)
            {
                Debug.Log("No path found between " + start.hashname + " and " + end.hashname);
                Stack<Waypoint> stack = new Stack<Waypoint>();
                stack.Push(start);
                queue.Clear();
                alreadyAddedWaypoints.Clear();
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
                //Debug.Log("Path found. Returning path " + stack.ToArray());
                queue.Clear();
                alreadyAddedWaypoints.Clear();
                return stack;
            }
        }


        private WaypointQueueable dequeue(FastPriorityQueue<WaypointQueueable> q)
        {
            return q.Dequeue();
        }
        private WaypointQueueable enqueue(FastPriorityQueue<WaypointQueueable> q, Waypoint self, WaypointQueueable from = null)
        {
            float dist = 0;
            if (from != null)
                dist = totaldistance(self, from); //the weight to the neighbor self from the waypoint from.self

            WaypointQueueable newwayp = new WaypointQueueable()
            {
                self = self,
                from = from,
                disttostart = dist
            };
            //Debug.Log("Enqueue " + self.hashname + " with distance " + dist);
            q.Enqueue(newwayp, dist);
            return newwayp;
        }

        private float totaldistance(Waypoint self, WaypointQueueable from)
        {
            float distbetween = Vector3.Distance(self.transform.position, from.self.transform.position);
            //The distance to a node is equal to the total distance of the node it came from plus the distance from that node to the current node
            return from.disttostart + distbetween;
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


