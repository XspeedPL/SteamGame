namespace XnaPathfidingLibrary
{
    /// <summary>
    /// This enum defines if the diagonals should be treated as neighbors or not.
    /// </summary>
    public enum SearchType
    {
        /// <summary>
        /// Only the nodes up, down, left and right of the current node are considered neighbors.
        /// </summary>
        FourWay,
        /// <summary>
        /// All adjacent nodes are considered neighbors.
        /// </summary>
        EightWay
    }
}
