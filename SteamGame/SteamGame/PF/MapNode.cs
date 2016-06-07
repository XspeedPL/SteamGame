using Microsoft.Xna.Framework;

namespace XnaPathfidingLibrary
{
    public class MapNode
    {
        private bool navigable;
        private bool visited;
        private bool added;
        private Point position;
        private MapNode parent;
        private int totalCost;
        private int heuristicCost;
        private int nodeCost;
        private object data;

        public MapNode(bool Navigable, Point Position)
        {
            navigable = Navigable;
            position = Position;
            visited = false;
            added = false;
            parent = null;
            totalCost = 0;
            heuristicCost = 0;
            nodeCost = 0;
        }

        public Point Position
        {
            get { return position; }
        }

        public bool Navigable
        {
            get { return navigable; }
            set { navigable = value;}
        }

        public void SetNav()
        {
            navigable = true;
        }

        public void ResetNav()
        {
            navigable = false;
        }

        public bool Visited
        {
            get { return visited; }
            set { visited = value; }
        }

        public bool Added
        {
            get { return added; }
            set { added = value; }
        }

        public MapNode Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public int TotalCost
        {
            get { return totalCost; }
            set { totalCost = value; }
        }

        public int HeuristicCost
        {
            get { return heuristicCost; }
            set { heuristicCost = value; }
        }

        public int NodeCost
        {
            get { return nodeCost; }
            set { nodeCost = value; }
        }

        public object Data
        {
            get { return data; }
            set { data = value; }
        }
    }
}
