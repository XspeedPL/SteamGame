using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XnaPathfidingLibrary
{
    /// <summary>
    /// This class defines a Depth Search search method.
    /// </summary>
    public class DepthSearch : AbstractSearch
    {
        /// <summary>
        /// A stack used to store the nodes that will be visited next.
        /// </summary>
        private Stack<MapNode> stack;

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="game">Game that the game component should be attached to.</param>
        public DepthSearch(Game game)
            : base(game)
        {
            stack = new Stack<MapNode>();
        }

        /// <summary>
        /// Starts the search algorithm.
        /// </summary>
        public override void Start()
        {
            stack.Clear();
            base.Start();
        }

        /// <summary>
        /// Gets the next node to be visited. 
        /// The node is the one on the to of the stack.
        /// </summary>
        /// <returns>Returns the next node to be visited.</returns>
        public override MapNode GetNode()
        {
            MapNode node;

            try
            {
                node = stack.Pop();
            }
            catch (InvalidOperationException)
            {
                node = null;
            }

            return node;
        }

        /// <summary>
        /// Adds a node to the wait list. This node can be visited in the future. 
        /// The node is added to a stack. This will be the next node to be visited, unless other nodes are stacked after this call.
        /// </summary>
        /// <param name="parent">The node's parent. Null if node is root.</param>
        /// <param name="node">The node.</param>
        public override void AddNode(MapNode parent, MapNode node)
        {
            if (node != null)
            {
                // If node has been stacked, do nothing.
                if (!node.Added)
                {
                    if (node.Navigable && !node.Visited)
                    {
                        node.Parent = parent;
                        node.Added = true;
                        stack.Push(node);
                        if (OnNodeAdded != null)
                        {
                            OnNodeAdded(this, new NodeAddedEventArgs(node));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets all the nodes currently in the stack (added nodes to be visited).
        /// </summary>
        /// <returns>Returns the nodes on the wait list.</returns>
        public override MapNode[] GetAddedNodes()
        {
            return stack.ToArray();
        }
    }
}
