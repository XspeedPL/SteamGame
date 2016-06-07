using System;

namespace XnaPathfidingLibrary
{
    /// <summary>
    /// This class defines an event triggered when the search is finished.
    /// </summary>
    public class SearchFinishedEventArgs : EventArgs
    {
        /// <summary>
        /// Defines if the goal was found.
        /// </summary>
        private bool found;

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="found">Defines if the goal was found.</param>
        public SearchFinishedEventArgs(bool found)
        {
            this.found = found;
        }

        /// <summary>
        /// Defines if the goal was found.
        /// </summary>
        public bool Found
        {
            get { return found; }
        }
    }
}
