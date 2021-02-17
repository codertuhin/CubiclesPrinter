using System;

namespace Cubicles.Events
{
    /// <summary>
    /// This class represents event args with data
    /// </summary>
    public class DataEventArgs : EventArgs
    {
        /// Properties
        #region Properties

        /// <summary> Data </summary>
        public object Data { get; set; }

        #endregion

        /// Init
        #region Init

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">data</param>
        public DataEventArgs(object data)
        {
            Data = data;
        }

        #endregion
    }
}
