using System;

namespace XnaPathfidingLibrary
{
    /// <summary>
    /// This class defines an event triggered when a node is added to a wait list.
    /// </summary>
    public class NodeAddedEventArgs : EventArgs
    {
        /// <summary>
        /// The node.
        /// </summary>
        private MapNode node;

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="node">The added node.</param>
        public NodeAddedEventArgs(MapNode node)
        {
            this.node = node;
        }

        /// <summary>
        /// Gets the added node.
        /// </summary>
        public MapNode Node
        {
            get { return node; }
        }
    }
}
