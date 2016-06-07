using System;

namespace XnaPathfidingLibrary
{
    /// <summary>
    /// This class defines an event triggered when a node is visited.
    /// </summary>
    public class NodeVisitedEventArgs : EventArgs
    {
        /// <summary>
        /// The node.
        /// </summary>
        private MapNode node;

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="node">The visited node.</param>
        public NodeVisitedEventArgs(MapNode node)
        {
            this.node = node;
        }

        /// <summary>
        /// Gets the visited node.
        /// </summary>
        public MapNode Node
        {
            get { return node; }
        }
    }
}
