using System.Collections.Generic;
using UnityEngine;
using System;

namespace TankPathingSystem
{

    /// <summary>
    /// Beefed up waypoint that can detect neighbors within a certain radius, and then recalculate wieghts further away
    /// <para>Because find-neighbors adds to a list it can be used repeatedly to find multiple sets of neighbors</para>
    /// </summary>

    //Enable this once the neighbor auto-find works in play mode
    [ExecuteInEditMode]
    public class Waypoint : MonoBehaviour
    {
        //A more descriptive name for a potentially nondescriptive waypoint names
        [DisplayWithoutEdit()]
        public string hashname = "";
        public int WaypointID;
        public float neighborSearchRadius = 10f;
        public bool showNeighborRadius = false;
        public bool showNeighborConenction = false;
        //A dictionary of waypoints sorted by waypointID
        //Unneccessary and nonfunctional in certain cases
        //public Dictionary<int, NeighborConnection> neighbors = new Dictionary<int, NeighborConnection>();
        [SerializeField]
        public List<NeighborConnection> neighbors = new List<NeighborConnection>();


        private void Start()
        {
            hashname = this.name + this.GetHashCode();

        }

        ///<summary>
        ///Ideally this reference to all waypoints is only done in the editor when the waypoint doesn't know who it's neighbors are
        ///</summary>
        public void FindNeighbors()
        {
            Debug.Log("Waypoint " + this.hashname + " has begun looking for nearby neighbors");
            Collider[] potentialNearbyObjects = Physics.OverlapSphere(this.transform.position, neighborSearchRadius, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);
            List<Collider> nearbyObjects = new List<Collider>();
            this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            foreach (Collider c in potentialNearbyObjects)
            {
                RaycastHit hit;
                if(Physics.Raycast(this.transform.position, c.transform.position - this.transform.position, out hit, neighborSearchRadius, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
                {
                    if (hit.collider == c)
                    {
                        Debug.Log("Raycast to collider " + c.name + " succesful; collider visible.");
                        nearbyObjects.Add(c);
                    }
                    else
                    {
                        Debug.Log("Raycast to collider " + c.name + " failed; cannot make conneciton.");
                    }
                }
            }
            this.gameObject.layer = LayerMask.NameToLayer("Default");

            for (int i = 0; i < nearbyObjects.Count; i++)
            {
                Debug.Log("Potential neighbor " + nearbyObjects[i].name + nearbyObjects[i].GetHashCode() + " detected");
                Waypoint neighbor;
                if (nearbyObjects[i].gameObject != this.gameObject &&
                    nearbyObjects[i].TryGetComponent<Waypoint>(out neighbor) &&
                    (!neighbor.CompareTag("Offduty Area") && !neighbor.CompareTag("Repair Area"))
                    )
                {
                    Debug.Log("Neighbor confirmed. Adding neighbor " + neighbor.hashname + " ID " + neighbor.WaypointID);
                    NeighborConnection connection = new NeighborConnection(neighbor);
                    connection.disttoneighbor = Vector3.Distance(this.transform.position, neighbor.transform.position);
                    AddNeighborUnique(connection);
                    Debug.Log("Adding self as neighbor to the possibly new neighbor.");
                    NeighborConnection connectionBack = new NeighborConnection(this);
                    connectionBack.disttoneighbor = connection.disttoneighbor;
                    neighbor.AddNeighborUnique(connectionBack);
                }
                else
                    Debug.Log("Neighbor rejected");
            }
        }

        public void AddNeighborUnique(NeighborConnection neighbor)
        {
            bool contains = false;
            foreach(NeighborConnection addedneighbor in neighbors)
            {
                if (addedneighbor.neighborID == neighbor.neighborID)
                {
                    contains = true;
                    break;
                }
            }


            Debug.Log("Checking uniqueness for " + neighbor.neighborName);
            if (!contains)
            {
                Debug.Log("Neighbor has not been added yet, adding");
                //this.neighbors.Add(neighbor.neighborID, neighbor);
                this.neighbors.Add(neighbor);
            }
            else
                Debug.Log("Neighbor has already been added. Doing nothing");
        }

        /// <summary>
        /// Also should be editor-only. Hit this button if you move the waypoint
        /// </summary>
        public void RecalculateWeights()
        {
            Debug.Log("Recalculating weights");
            for (int i = 0; i < neighbors.Count; i++)
            {
                neighbors[i].disttoneighbor = Vector3.Distance(this.transform.position, neighbors[i].neighbor.transform.position);
                //neighbors[InspectorNeighbors[i].neighborID].disttoneighbor = InspectorNeighbors[i].disttoneighbor;
            }
        }

        ///<summary>
        ///Another function that should only be used by the editor
        ///</summary>
        public void ResetNeighbors()
        {
            //neighbors = new Dictionary<int, NeighborConnection>();
            neighbors.Clear();
        }

        private void OnDrawGizmos()
        {
            if (showNeighborRadius)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(this.transform.position, neighborSearchRadius);
            }
            if (showNeighborConenction && neighbors != null)
            {
                foreach(NeighborConnection n in neighbors)
                {
                    Vector3 pos = this.transform.position;
                    Gizmos.color = new Color(pos.x, pos.y, pos.z);
                    Gizmos.DrawLine(pos, n.neighbor.transform.position);
                }
            }
        }
    }

    [Serializable]
    public class NeighborConnection
    {
        public int neighborID;
        public Waypoint neighbor;
        public string neighborName;
        [DisplayWithoutEdit()]
        public float disttoneighbor;
        public NeighborConnection(Waypoint neighbor, float dist = float.PositiveInfinity)
        {
            this.neighbor = neighbor;
            neighborID = neighbor.WaypointID;
            neighborName = neighbor.hashname;
            this.disttoneighbor = dist;
        }

        public override int GetHashCode()
        {
            return neighborID.GetHashCode();
        }
    }

 
}

