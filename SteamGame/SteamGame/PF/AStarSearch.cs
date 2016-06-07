using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XnaPathfidingLibrary
{
    /// <summary>
    /// This class defines a A* (A star) search method.
    /// </summary>
    public class AStarSearch : AbstractSearch
    {
        /// <summary>
        /// This list stores nodes that should be visited.
        /// </summary>
        private List<MapNode> openList;

        /// <summary>
        /// This list stores nodes that have been visited already.
        /// </summary>
        private List<MapNode> closedList;

        /// <summary>
        /// Heuristic used to measure which node is (or can be) the best next step.
        /// Initially euclidean distance and manhattan distance are planned, but at this point only the first is supported.
        /// </summary>
        private Heuristic heuristic;

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="game">Game that the game component should be attached to.</param>
        public AStarSearch(Game game)
            : base(game)
        {
            openList = new List<MapNode>();
            closedList = new List<MapNode>();
            heuristic = Heuristic.Euclidean;
        }

        /// <summary>
        /// Starts the search algorithm.
        /// </summary>
        public override void Start()
        {
            openList.Clear();
            closedList.Clear();

            base.Start();
        }

        /// <summary>
        /// Gets the next node to be visited. 
        /// The node is choosen from the open list and is the one with the lower total cost.
        /// </summary>
        /// <returns>Returns the next node to be visited.</returns>
        public override MapNode GetNode()
        {
            MapNode node;
                        
            // If there are nodes in the open list, get the best one, remove it from the open list and return.
            if (openList.Count > 0)
            {
                node = GetBestNode();
                openList.Remove(node);
            }
            else
            {
                node = null;
            }

            return node;
        }

        /// <summary>
        /// Adds a node to the wait list. This node can be visited in the future. 
        /// The node is added to an open list where it can be grabbed, if it has the best cost.
        /// </summary>
        /// <param name="parent">The node's parent. Null if node is root.</param>
        /// <param name="node">The node.</param>
        public override void AddNode(MapNode parent, MapNode node)
        {
            if (node != null)
            {
                // The node is added only if it's navigable and not visited before.
                Point p = node.Position;
                if (node.Navigable && !node.Visited)
                {
                    if (!openList.Contains(node))
                    {
                        // If the node is not in the open list.

                        node.Parent = parent;

                        // Find it's heuristic cost based on the current heuristic.
                        switch (heuristic)
                        {
                            case Heuristic.Manhattan:
                                throw new Exception("Currently only Euclidean distance heuristic is supported. Try using Heuristic.Euclidean.");

                            // Euclidean distance from current and the end node. |V1 - v2|
                            case Heuristic.Euclidean:
                                Vector2 end = new Vector2(EndNode.Position.X, EndNode.Position.Y);
                                Vector2 v = new Vector2(node.Position.X, node.Position.Y);
                                float distance = Math.Abs(Vector2.Distance(v, end));
                                node.HeuristicCost = (int)distance;
                                break;
                            default:
                                break;
                        }                        
                        
                        // Finds it's total cost. If the node has a parent, it's parent total cost is added.
                        if (parent != null)
                        {
                            node.TotalCost = node.HeuristicCost + node.NodeCost + node.Parent.TotalCost;
                        }
                        else
                        {
                            node.TotalCost = node.HeuristicCost + node.NodeCost;
                        }

                        // Adds the node to the open list.
                        openList.Add(node);

                        // Calls the OnNodeAdded event (if it exists).
                        if (OnNodeAdded != null)
                        {
                            OnNodeAdded(this, new NodeAddedEventArgs(node));
                        }
                    }
                    else
                    {
                        // If the added node is already in the open list, it's needed to check if the current node is a better way,
                        // i.e., it's cost is lower than the previous parent. If so, th parent is changed and it's total cost is
                        // calculated again.
                        if (CurrentNode.TotalCost < node.TotalCost)
                        {
                            node.Parent = CurrentNode;
                            node.TotalCost = node.HeuristicCost + node.NodeCost + CurrentNode.TotalCost;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets all the nodes currently in the open list (added nodes to be visited).
        /// </summary>
        /// <returns>Returns the nodes on the wait list.</returns>
        public override MapNode[] GetAddedNodes()
        {
            return openList.ToArray();
        }

        /// <summary>
        /// Searches the openlist and finds the next node to be visited. This node is the one with the lower cost.
        /// </summary>
        /// <returns>An node from the open list with the lower cost.</returns>
        private MapNode GetBestNode()
        {
            MapNode node;

            int index = -1;
            int minCost = int.MaxValue;
            for (int i = 0;
                i < openList.Count;
                i++)
            {
                if(openList[i].TotalCost < minCost)
                {
                    minCost = openList[i].TotalCost;
                    index = i;
                }
            }

            if (index != -1)
            {
                node = openList[index];
            }
            else
            {
                node = null;
            }

            return node;
        }

        /// <summary>
        /// Visits a node in order to check if it's the goal. If not, it's neighbors are added to the wait list.
        /// This fully overrides AbstractSearch.VisitNode.
        /// </summary>
        /// <param name="node">The node to be visited.</param>
        public override void VisitNode(MapNode node)
        {
            // Sets the current node.
            CurrentNode = node;

            if (node != null)
            {
                // If node hasn't been visited.
                if (!node.Visited)
                {
                    // Marks it as visited and add it to the closed list.
                    node.Visited = true;
                    // This is the only thing that requires this method override.
                    closedList.Add(node);

                    // Calls the OnNodeVisited event (if it exists).
                    if (OnNodeVisited != null)
                    {
                        OnNodeVisited(this, new NodeVisitedEventArgs(node));
                    }

                    // Checks if the current node is the end node.
                    if (node.Position == EndNode.Position)
                    {
                        // Makes the path from start to end node.
                        MakePath(node);
                        // Finishes the search.
                        State = SearchState.FinishedFound;

                        // Calls the OnSearchFinished event (if it exists).
                        // True means the goal was found.
                        if (OnSearchFinished != null)
                        {
                            OnSearchFinished(this, new SearchFinishedEventArgs(true));
                        }
                    }
                    else
                    {
                        // Gets the current node neighbors.
                        List<MapNode> neighbors = Map.GetNeighborhood(node, Type, Orientation);

                        // Adds all of them to the visit list.
                        // This could be a queue, a stack or anything, depending on the search algorithm.
                        foreach (MapNode m in neighbors)
                        {
                            AddNode(CurrentNode, m);                                                                
                        }
                    }
                }
            }
            else
            {
                //   If the current node is null, this means the search is finished and the goal hasn't been found.
                State = SearchState.FinishedNotFound;
                // Calls the OnSearchFinished event (if it exists).
                // False means the goal was not found.
                if (OnSearchFinished != null) OnSearchFinished(this, new SearchFinishedEventArgs(false));
            }
        }

        /// <summary>
        /// Gets or sets the A* heuristic.
        /// </summary>
        public Heuristic Heuristic
        {
            get { return heuristic; }
            set
            {
                heuristic = value;
                if (heuristic != Heuristic.Euclidean)
                {
                    throw new Exception("Currently only Euclidean distance heuristic is supported. Try using Heuristic.Euclidean.");
                }
            }
        }
    }
}