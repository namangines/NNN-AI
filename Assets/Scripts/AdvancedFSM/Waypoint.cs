using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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
        public float neighborSearchRadius = 10f;
        public bool showNeighborRadius = false;
        //Waypoints and weights to get to them
        public HashSet<NeighborConnection> neighbors = new HashSet<NeighborConnection>();

        //Is called once by the system when the instance of the script is first loaded
        private void Awake()
        {
            hashname = this.name + this.GetHashCode();

        }

        ///<summary>
        ///Ideally this reference to all waypoints is only done in the editor when the waypoint doesn't know who it's neighbors are
        ///</summary>
        public void FindNeighbors()
        {
            Debug.Log("Waypoint " + this.hashname + " has begun looking for nearby neighbors");
            Collider[] nearbyObjects = Physics.OverlapSphere(this.transform.position, neighborSearchRadius, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);

            foreach(Collider obj in nearbyObjects)
            {
                Debug.Log("Potential neighbor detected");
                Waypoint neighbor;
                if (obj.gameObject != this.gameObject && obj.TryGetComponent<Waypoint>(out neighbor))
                {
                    Debug.Log("Neighbor confirmed. Adding neighbor " + neighbor.hashname);
                    NeighborConnection connection = new NeighborConnection(neighbor);
                    connection.weight = Vector3.Distance(this.transform.position, neighbor.transform.position);
                    connection.neighborName = neighbor.hashname;
                    neighbors.Add(connection);
                }
            }
        }

        /// <summary>
        /// Also should be editor-only. Hit this button if you move the waypoint
        /// </summary>
        public void RecalculateWeights()
        {
            foreach(NeighborConnection connection in neighbors)
            {
                connection.weight = Vector3.Distance(this.transform.position, connection.neighbor.transform.position);
            }
        }

        ///<summary>
        ///Another function that should only be used by the editor
        ///</summary>
        public void ResetNeighbors()
        {
            neighbors = new HashSet<NeighborConnection>();
        }

        private void OnDrawGizmos()
        {
            if (showNeighborRadius)
            {
                Gizmos.DrawWireSphere(this.transform.position, neighborSearchRadius);
            }
        }
    }

    [Serializable]
    public class NeighborConnection
    {
        private Waypoint _neighbor;
        public Waypoint neighbor { get { return _neighbor; } }
        [DisplayWithoutEdit()]
        public string neighborName = "uninitialized name";
        [DisplayWithoutEdit()]
        public float weight;
        public NeighborConnection(Waypoint neighbor, float weight = float.PositiveInfinity)
        {
            _neighbor = neighbor;
            this.weight = weight;
        }

        public override int GetHashCode()
        {
            return _neighbor.GetHashCode();
        }
    }
}

