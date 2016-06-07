namespace XnaPathfidingLibrary
{
    /// <summary>
    /// Defines a serch heuristic for A*.
    /// </summary>
    public enum Heuristic
    {
        /// <summary>
        /// Manhattan distance.
        /// Manhattan is not supported yet.
        /// </summary>
        Manhattan, 
        /// <summary>
        /// Euclidean distance.
        /// </summary>
        Euclidean
    }
}
