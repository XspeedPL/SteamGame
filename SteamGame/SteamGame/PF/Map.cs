using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace XnaPathfidingLibrary
{
    /// <summary>
    /// This class defines a map, i.e, a search space for search algorithms.
    /// The map is a MxN 2D matrix, where each cell is a MapNode instance.
    /// </summary>
    public class Map
    {
        // i,j {y,x}
        /// <summary>
        /// A MxN 2D matrix representing the map.
        /// The first index represents a line and the second a column.
        /// </summary>
        private MapNode[,] nodes;

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        public Map() { }

        public bool Contains(MapNode node)
        {
            foreach (MapNode nod in nodes)
                if (nod.Equals(node)) return true;
            return false;
        }

        /// <summary>
        /// Initializes a new instance of this class.
        /// All the nodes here are considered navigable, by default.
        /// </summary>
        /// <param name="lines">Amount of lines.</param>
        /// <param name="columns">Amount of columns.</param>       
        public Map(int columns, int lines)
        {
            nodes = new MapNode[columns, lines];
            for (int i = 0; i < nodes.GetLength(0); i++)
            {
                for (int j = 0; j < nodes.GetLength(1); j++)
                {
                    nodes[i, j] = new MapNode(false, new Point(i, j));
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of this class.
        /// This builds a map from existing map nodes.
        /// Important: these nodes are copied into new nodes, so no reference to the original node is stored. 
        /// Changes on the original nodes are not reflected here.
        /// </summary>
        /// <param name="nodes">A MxN 2D matrix containing all the map nodes.</param>
        public Map(MapNode[,] nodes) 
        {
            this.nodes = new MapNode[nodes.GetLength(0), nodes.GetLength(1)];

            for (int i = 0; i < nodes.GetLength(0); i++)
            {
                for (int j = 0; j < nodes.GetLength(1); j++)
                {
                    MapNode n = nodes[i, j];
                    this.nodes[i, j] = new MapNode(n.Navigable, n.Position);
                    this.nodes[i, j].NodeCost = n.NodeCost;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of this class.
        /// This builds a map from a boolean matrix, where true means navigable and false means not navigable. 
        /// </summary>
        /// <param name="nodes">A MxN 2D boolean matrix defining whether the node is navigable or not.</param>
        public Map(bool[,] nodes)
        {
            this.nodes = new MapNode[nodes.GetLength(0), nodes.GetLength(1)];

            for (int i = 0; i < nodes.GetLength(0); i++)
            {
                for (int j = 0; j < nodes.GetLength(1); j++)
                {
                    this.nodes[i, j] = new MapNode(nodes[i, j], new Point(i, j));
                }
            }
        }

        /// <summary>
        /// Gets or sets the map nodes.
        /// </summary>
        public MapNode[,] Nodes
        {
            get { return nodes; }
            set { nodes = value; }
        }

        public MapNode this[int x, int y]
        {
            get { return Nodes[x, y]; }
            set { Nodes[x, y] = value; }
        }

        /// <summary>
        /// Gets the neightborhood for a given map node.
        /// These are the contiguous nodes taken from the map matrix.
        /// </summary>
        /// <param name="node">The given node. This method will find it's neighbors.</param>
        /// <param name="type">Defines if diagonals nodes should be considered neighbors or not.</param>
        /// <param name="orientation">The search order. Clockwise or Counterclockwise.</param>
        /// <returns>Returs a list of neighbors for a given node.</returns>
        public List<MapNode> GetNeighborhood(MapNode node, SearchType type, SearchOrientation orientation)
        {
            List<MapNode> list = new List<MapNode>();
            Point currentPoint = node.Position;

            // If type is four way, only the nodes marked with a 'X' will be considered neighbors. Not the ones marked as '0'.
            // If type is eight way, both nodes marked as 'X' or '0' are considered neighbors.
            // N is the given node:
            //
            // 0 X 0
            // X N X
            // 0 X 0            

            if (type == SearchType.FourWay)
            {
                if (orientation == SearchOrientation.Clockwise)
                {
                    if (currentPoint.Y > 0)
                    {
                        list.Add(nodes[currentPoint.X, currentPoint.Y - 1]);
                    }

                    if (currentPoint.X < nodes.GetLength(0) - 1)
                    {
                        list.Add(nodes[currentPoint.X + 1, currentPoint.Y]);
                    }

                    if (currentPoint.Y < nodes.GetLength(1) - 1)
                    {
                        list.Add(nodes[currentPoint.X, currentPoint.Y + 1]);
                    }

                    if (currentPoint.X > 0)
                    {
                        list.Add(nodes[currentPoint.X - 1, currentPoint.Y]);
                    }
                }
                else if (orientation == SearchOrientation.Counterclockwise)
                {
                    if (currentPoint.Y > 0)
                    {
                        list.Add(nodes[currentPoint.X, currentPoint.Y - 1]);
                    }

                    if (currentPoint.X > 0)
                    {
                        list.Add(nodes[currentPoint.X - 1, currentPoint.Y]);
                    }

                    if (currentPoint.Y < nodes.GetLength(1) - 1)
                    {
                        list.Add(nodes[currentPoint.X, currentPoint.Y + 1]);
                    }

                    if (currentPoint.X < nodes.GetLength(0) - 1)
                    {
                        list.Add(nodes[currentPoint.X + 1, currentPoint.Y]);
                    }
                }
            }
            else if (type == SearchType.EightWay)
            {
                if (orientation == SearchOrientation.Clockwise)
                {
                    if (currentPoint.Y > 0)
                    {
                        list.Add(nodes[currentPoint.X, currentPoint.Y - 1]);
                    }

                    if (currentPoint.X < nodes.GetLength(0) - 1)
                    {
                        list.Add(nodes[currentPoint.X + 1, currentPoint.Y]);
                    }

                    if (currentPoint.Y < nodes.GetLength(1) - 1)
                    {
                        list.Add(nodes[currentPoint.X, currentPoint.Y + 1]);
                    }

                    if (currentPoint.X > 0)
                    {
                        list.Add(nodes[currentPoint.X - 1, currentPoint.Y]);
                    }

                    if (currentPoint.Y > 0 && currentPoint.X < nodes.GetLength(0) - 1)
                    {
                        list.Add(nodes[currentPoint.X + 1, currentPoint.Y - 1]);
                    }

                    if (currentPoint.Y < nodes.GetLength(1) - 1 && currentPoint.X < nodes.GetLength(0) - 1)
                    {
                        list.Add(nodes[currentPoint.X + 1, currentPoint.Y + 1]);
                    }

                    if (currentPoint.Y < nodes.GetLength(1) - 1 && currentPoint.X > 0)
                    {
                        list.Add(nodes[currentPoint.X - 1, currentPoint.Y + 1]);
                    }

                    if (currentPoint.Y > 0 && currentPoint.X > 0)
                    {
                        list.Add(nodes[currentPoint.X - 1, currentPoint.Y - 1]);
                    }
                }
                else if (orientation == SearchOrientation.Counterclockwise)
                {
                    if (currentPoint.Y > 0)
                    {
                        list.Add(nodes[currentPoint.X, currentPoint.Y - 1]);
                    }

                    if (currentPoint.Y > 0 && currentPoint.X > 0)
                    {
                        list.Add(nodes[currentPoint.X - 1, currentPoint.Y - 1]);
                    }

                    if (currentPoint.X > 0)
                    {
                        list.Add(nodes[currentPoint.X - 1, currentPoint.Y]);
                    }

                    if (currentPoint.Y < nodes.GetLength(1) - 1 && currentPoint.X > 0)
                    {
                        list.Add(nodes[currentPoint.X - 1, currentPoint.Y + 1]);
                    }

                    if (currentPoint.Y < nodes.GetLength(1) - 1)
                    {
                        list.Add(nodes[currentPoint.X, currentPoint.Y + 1]);
                    }

                    if (currentPoint.Y < nodes.GetLength(1) - 1 && currentPoint.X < nodes.GetLength(0) - 1)
                    {
                        list.Add(nodes[currentPoint.X + 1, currentPoint.Y + 1]);
                    }

                    if (currentPoint.X < nodes.GetLength(0) - 1)
                    {
                        list.Add(nodes[currentPoint.X + 1, currentPoint.Y]);
                    }

                    if (currentPoint.Y > 0 && currentPoint.X < nodes.GetLength(0) - 1)
                    {
                        list.Add(nodes[currentPoint.X + 1, currentPoint.Y - 1]);
                    }
                }
            }


            return list;
        }
    }
}
