namespace XnaPathfidingLibrary
{
    /// <summary>
    /// This enum defines the diferent search configurations.
    /// This defines how the search component should behave each time it's Update method is called.
    /// </summary>
    public enum SearchConfiguration
    {
        /// <summary>
        /// Search will be performed until the time limit exceds.
        /// </summary>
        Timed,
        /// <summary>
        /// Search will be performed until the goal is found or there are no more nodes to visit.
        /// </summary>
        NotTimed,
        /// <summary>
        /// Search will be performed until the amount of iterations exceds the specified limit.
        /// </summary>
        IterationLimited
    }
}
