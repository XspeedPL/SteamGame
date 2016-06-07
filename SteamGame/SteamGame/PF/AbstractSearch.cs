using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace XnaPathfidingLibrary
{
    /// <summary>
    /// This is a base class for the search methods (such as breadth search or A*).
    /// </summary>
    public abstract class AbstractSearch : GameComponent
    {
        /// <summary>
        /// Current search state.
        /// </summary>
        private SearchState state;

        /// <summary>
        /// A LinkedList containing the path from start node to end node, if there is any.
        /// </summary>
        private LinkedList<MapNode> path;

        /// <summary>
        /// Defines search timing configuration (whether it's timed, iteration limited or not timed at all).
        /// </summary>
        private SearchConfiguration configuration;

        /// <summary>
        /// Defines de search orientation (clockwise ou counterclockwise).
        /// </summary>
        private SearchOrientation orientation;

        /// <summary>
        /// Defines the neighborhood type (fourway or eightway, i.e., diagonally neighbors should also be visited or not).
        /// </summary>
        private SearchType type;

        /// <summary>
        /// Search map indicating which cells are navigable.
        /// </summary>
        private Map map;

        /// <summary>
        /// Max time the search should take for frame, in milliseconds. Applicable only when configuration is Timed.
        /// </summary>
        private double timeLimit;

        /// <summary>
        /// Elapsed milliseconds in the current frame.
        /// </summary>
        private double elapsedTime; 

        /// <summary>
        /// Total search time since Start was called, in milliseconds. 
        /// </summary>
        private double totalTime;

        /// <summary>
        /// Max iterations the search should take for frame. Applicable only when configuration is IterationLimited.
        /// </summary>
        private int iterationLimit;

        /// <summary>
        /// Total iterations taken in the current frame.
        /// </summary>
        private int iterations;

        /// <summary>
        /// Total iterations taken since Start was called.
        /// </summary>
        private int totalIterations;

        /// <summary>
        /// Total Update calls the component has taken since Start was called, during the search process.
        /// </summary>
        private int updateCalls;

        /// <summary>
        /// Current visited node.
        /// </summary>
        private MapNode currentNode;

        /// <summary>
        /// Called when the search is finished. Raises OnSearchFinished event.
        /// </summary>
        private event EventHandler<SearchFinishedEventArgs> onSearchFinished;

        /// <summary>
        /// Called when a node is added to a wait list (where it can be visited in the future). Raises OnNodeAdded event.
        /// </summary>
        private event EventHandler<NodeAddedEventArgs> onNodeAdded;

        /// <summary>
        /// Called when a node is visited. Raises OnNodeVisited event.
        /// </summary>
        private event EventHandler<NodeVisitedEventArgs> onNodeVisited;

        /// <summary>
        /// Gets the next node to be visited. The way this node is choosen is defined by the search method, e.g., Depth Search 
        /// stacks the nodes to be visited, so the next node is always the one on the top of the stack.
        /// </summary>
        /// <returns>Returns the next node to be visited.</returns>
        public abstract MapNode GetNode();

        /// <summary>
        /// Adds a node to the wait list. This node can be visited in the future. The way this node is added is defined by the search
        /// methos, e.g., Breadth Search uses a queue to store it's wait list, so the node will be added at the end of the queue.
        /// </summary>
        /// <param name="parent">The node's parent. Null if node is root.</param>
        /// <param name="node">The node.</param>
        public abstract void AddNode(MapNode parent, MapNode node);

        /// <summary>
        /// Gets the nodes that are currently on the wait list.
        /// </summary>
        /// <returns>Returns the nodes on the wait list.</returns>
        public abstract MapNode[] GetAddedNodes();

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="game">Game that the game component should be attached to.</param>
        public AbstractSearch(Game game)
            : base(game)
        {
            state = SearchState.NotStarted;
            path = null;
            configuration = SearchConfiguration.NotTimed;
            orientation = SearchOrientation.Clockwise;
            type = SearchType.FourWay;
            map = null;
            timeLimit = (float)game.TargetElapsedTime.TotalMilliseconds;
            elapsedTime = 0.0f;
            totalTime = 0.0f;
            iterations = 0;
            totalIterations = 0;
            updateCalls = 0;
        }

        private MapNode startNode = null, endNode = null;
        public MapNode StartNode { get { return startNode; } set { startNode = value; } }
        public MapNode EndNode { get { return endNode; } set { endNode = value; } }

        /// <summary>
        /// Processes the search algorithm depending on it's configuration (timed, iteration limited, or not timed).
        /// If the search is not finished when this method ends, it will continue when it's called again by the game.
        /// </summary>
        /// <param name="gameTime">Time elapsed since the last call to Update.</param>
        public override void Update(GameTime gameTime)
        {
            // If search has started, process it.
            if (state == SearchState.Started)
            {
                // Count how many update calls the component will get until search is finished.
                updateCalls++;

                // Resets current elapsed time.
                elapsedTime = 0.0f;

                // High resolution timer used if the algorithm is time limited.
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();                

                // Iterations counter used if the algorithm is iteration limited.
                iterations = 0;

                // Process the search depending on it's configurations.                
                switch (configuration)
                {
                    // If the search is timed, do it until elapsed time is equal or greater the time limit.
                    case SearchConfiguration.Timed:
                        do
                        {
                            iterations++;
                            if (state == SearchState.Started)
                            {
                                // Process the current node.
                                VisitNode(GetNode());
                            }
                            elapsedTime = stopwatch.Elapsed.TotalMilliseconds;
                        } while (elapsedTime < timeLimit);
                        break;

                    // If the search is not timed, do it until it's finished (the component will be locked meanwhile).
                    case SearchConfiguration.NotTimed:
                        while (state == SearchState.Started)
                        {
                            iterations++;
                            // Process the current node.
                            VisitNode(GetNode());
                        }
                        elapsedTime = stopwatch.Elapsed.TotalMilliseconds;
                        break;

                    // If the search is iteration limited, do it until the amount of iterations is achieved.
                    case SearchConfiguration.IterationLimited:
                        while (iterations < iterationLimit && state == SearchState.Started)
                        {
                            iterations++;
                            // Process the current node.
                            VisitNode(GetNode());
                        } 
                        elapsedTime = stopwatch.Elapsed.TotalMilliseconds;
                        break;

                    default:
                        break;
                }

                totalTime += elapsedTime;
                totalIterations += iterations;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Starts the search algorithm.
        /// </summary>
        public virtual void Start()
        {
            // Resets to it's original state.
            Reset();

            // Make sure nodes are valid.
            if (StartNode == null || !map.Contains(StartNode))
            {
                throw new Exception("Start Node is invalid.");
            }
            if (EndNode == null || !map.Contains(EndNode))
            {
                throw new Exception("End Node is invalid.");
            }

            // Adds the start node to the search space.
            AddNode(null, StartNode);
            state = SearchState.Started;                      

            elapsedTime = 0.0f;
            totalTime = 0.0f;
            iterations = 0;
            totalIterations = 0;
            updateCalls = 0;
        }

        /// <summary>
        /// Pauses the search algorithm. It can be resumed at a later time.
        /// </summary>
        public void Pause()
        {
            if (state == SearchState.Started)
            {
                state = SearchState.Paused;
            }
        }

        /// <summary>
        /// Resumes the search algorithm.
        /// </summary>
        public void Resume()
        {
            if (state == SearchState.Paused)
            {
                state = SearchState.Started;
            }
        }

        /// <summary>
        /// Resets the search algorithm.
        /// </summary>
        public void Reset()
        {
            if (path != null)
            {
                path.Clear();
            }

            if (map != null)
            {
                for (int i = 0; i < map.Nodes.GetLength(0); i++)
                {
                    for (int j = 0; j < map.Nodes.GetLength(1); j++)
                    {
                        map.Nodes[i, j].Parent = null;
                        map.Nodes[i, j].Visited = false;
                        map.Nodes[i, j].Added = false;
                        map.Nodes[i, j].HeuristicCost = 0;
                        map.Nodes[i, j].NodeCost = 0;
                        map.Nodes[i, j].TotalCost = 0;
                    }
                }

                state = SearchState.NotStarted;
            }
            else
            {
                throw new Exception("Map cannot be null.");
            }
        }

        /// <summary>
        /// Stops the search algorithm.
        /// </summary>
        public void Stop()
        {
            state = SearchState.NotStarted;
        }

        /// <summary>
        /// Visits a node in order to check if it's the goal. If not, it's neighbors are added to the wait list.
        /// </summary>
        /// <param name="node">The node to be visited.</param>
        public virtual void VisitNode(MapNode node)
        {
            // Sets the current working node.
            currentNode = node;

            if (node != null)
            {
                // If node hasn't been visited.
                if (!node.Visited)
                {
                    // Marks it as visited.
                    node.Visited = true;

                    // Calls the OnNodeVisited event (if it exists).
                    if (onNodeVisited != null)
                    {
                        onNodeVisited(this, new NodeVisitedEventArgs(node));
                    }

                    // Checks if the current node is the end node.
                    if (node.Position == EndNode.Position)
                    {
                        // Makes the path from start to end node.
                        MakePath(node);
                        // Finishes the search.
                        state = SearchState.FinishedFound;

                        // Calls the OnSearchFinished event (if it exists).
                        // True means the goal was found.
                        if (onSearchFinished != null)
                        {
                            onSearchFinished(this, new SearchFinishedEventArgs(true));
                        }
                    }
                    else
                    {
                        // Gets the current node neighbors.
                        List<MapNode> neighbors = map.GetNeighborhood(node, type, orientation);

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
                state = SearchState.FinishedNotFound;
                // Calls the OnSearchFinished event (if it exists).
                // False means the goal was not found.
                if (onSearchFinished != null) onSearchFinished(this, new SearchFinishedEventArgs(false));
            }
        }

        /// <summary>
        /// Creates a LinkedList containing the path from start node to end node, if there is any.
        /// </summary>
        /// <param name="node">The end node.</param>
        protected void MakePath(MapNode node)
        {
            MapNode n = node;
            List<MapNode> list = new List<MapNode>();

            while (n != null)
            {
                list.Add(n);
                n = n.Parent;
            }

            list.Reverse();

            path = new LinkedList<MapNode>(list);
        }

        /// <summary>
        /// Gets or sets the current search state.
        /// </summary>
        public SearchState State
        {
            get { return state; }
            protected set { state = value; }
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        public LinkedList<MapNode> Path
        {
            get { return path; }
        }

        /// <summary>
        /// Gets or sets the search configuration.
        /// </summary>
        public SearchConfiguration Configuration
        {
            get { return configuration; }
            set
            {
                if (state == SearchState.Started)
                {
                    throw new Exception("Configuration can be changed only if search is not started.");
                }
                else
                {
                    configuration = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the search orientation.
        /// </summary>
        public SearchOrientation Orientation
        {
            get { return orientation; }
            set
            {
                if (state == SearchState.Started)
                {
                    throw new Exception("Orientation can be changed only if search is not started.");
                }
                else
                {
                    orientation = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the search type.
        /// </summary>
        public SearchType Type
        {
            get { return type; }
            set
            {
                if (state != SearchState.NotStarted)
                {
                    throw new Exception("Type can be changed only if search is not started.");
                }
                else
                {
                    type = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the map.
        /// </summary>
        public Map Map
        {
            get { return map; }
            set { map = value; }
        }

        /// <summary>
        /// Gets or sets the current node that is being visited.
        /// </summary>
        public MapNode CurrentNode
        {
            get { return currentNode; }
            protected set { currentNode = value; }
        }

        /// <summary>
        /// Gets or sets the search time limit, in milliseconds.
        /// </summary>
        public double TimeLimit
        {
            get { return timeLimit; }
            set { timeLimit = value; }
        }

        /// <summary>
        /// Gets the total elapsed search time so far.
        /// </summary>
        public double TotalTime
        {
            get { return totalTime; }
        }

        /// <summary>
        /// Gets or sets the search iterations limit.
        /// </summary>
        public int IterationLimit
        {
            get { return iterationLimit; }
            set { iterationLimit = value; }
        }

        /// <summary>
        /// Gets to total iterations taken so far.
        /// </summary>
        public int TotalIterations
        {
            get { return totalIterations; }
        }

        /// <summary>
        /// Gets to total Update calls taken so far.
        /// </summary>
        public int UpdateCalls
        {
            get { return updateCalls; }
        }

        /// <summary>
        /// Gets or sets a handler to OnSearchFinished event.
        /// </summary>
        public EventHandler<SearchFinishedEventArgs> OnSearchFinished
        {
            get { return onSearchFinished; }
            set { onSearchFinished = value; }
        }

        /// <summary>
        /// Gets or sets a handler to OnNodeAdded event.
        /// </summary>
        public EventHandler<NodeAddedEventArgs> OnNodeAdded
        {
            get { return onNodeAdded; }
            set { onNodeAdded = value; }
        }

        /// <summary>
        /// Gets or sets a handler to OnNodeVisited event.
        /// </summary>
        public EventHandler<NodeVisitedEventArgs> OnNodeVisited
        {
            get { return onNodeVisited; }
            set { onNodeVisited = value; }
        }
    }
}
