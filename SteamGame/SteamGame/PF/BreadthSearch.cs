using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XnaPathfidingLibrary
{
    /// <summary>
    /// This class defines a Breadth Search search method.
    /// </summary>
    public class BreadthSearch : AbstractSearch
    {
        /// <summary>
        /// A queue used to store the nodes that will be visited next.
        /// </summary>
        private Queue<MapNode> queue;

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="game">Game that the game component should be attached to.</param>
        public BreadthSearch(Game game)
            : base(game)
        {
            queue = new Queue<MapNode>();
        }

        /// <summary>
        /// Starts the search algorithm.
        /// </summary>
        public override void Start()
        {
            queue.Clear();
            base.Start();
        }

        /// <summary>
        /// Gets the next node to be visited. 
        /// The node is the first one in the queue.
        /// </summary>
        /// <returns>Returns the next node to be visited.</returns>
        public override MapNode GetNode()
        {
            MapNode node;

            try
            {
                node = queue.Dequeue();
            }
            catch (InvalidOperationException)
            {
                node = null;
            }

            return node;
        }

        /// <summary>
        /// Adds a node to the wait list. This node can be visited in the future. 
        /// The node is added to a queue where it can be grabbed, after the nodes already added.
        /// </summary>
        /// <param name="parent">The node's parent. Null if node is root.</param>
        /// <param name="node">The node.</param>
        public override void AddNode(MapNode parent, MapNode node)
        {
            if (node != null)
            {
                // If node has been enqueued, do nothing.
                if (!node.Added)
                {
                    if (node.Navigable && !node.Visited)
                    {
                        node.Parent = parent;
                        node.Added = true;
                        queue.Enqueue(node);
                        if (OnNodeAdded != null)
                        {
                            OnNodeAdded(this, new NodeAddedEventArgs(node));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets all the nodes currently in the queue (added nodes to be visited).
        /// </summary>
        /// <returns>Returns the nodes on the wait list.</returns>
        public override MapNode[] GetAddedNodes()
        {
            return queue.ToArray();
        }
    }
}
