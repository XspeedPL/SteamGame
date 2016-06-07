namespace XnaPathfidingLibrary
{
    /// <summary>
    /// This enum defines the possible states of a search algorithm.
    /// </summary>
    public enum SearchState
    {
        /// <summary>
        /// The search is not started. This is the best time to change the search properties.
        /// </summary>
        NotStarted,
        /// <summary>
        /// The search is running. Changing search properties may lead to errors and throw exceptions.
        /// </summary>
        Started,
        /// <summary>
        /// The search is finished and the goal was found.
        /// </summary>
        FinishedFound,
        /// <summary>
        /// The search is finished and the goal was not found.
        /// </summary>
        FinishedNotFound,
        /// <summary>
        /// The search is paused and may be resumed anytime.
        /// </summary>
        Paused
    }
}
