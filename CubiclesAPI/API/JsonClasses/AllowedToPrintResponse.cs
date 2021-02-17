using Newtonsoft.Json;

namespace Cubicles.API.JsonClasses
{
    /// <summary>
    /// This classs represents response to the API request CheckIfCanPrint
    /// </summary>
    public sealed class AllowedToPrintResponse : JResponse
    {
        /// Properties
        #region Properties

        /// <summary> Indicates whether the user allowed to print or not </summary>
        [JsonProperty("allowedToPrint")]
        public bool AllowedToPrint { get; set; }

        /// <summary>
        /// The reason of the disallowance
        /// </summary>
        [JsonProperty("reason")]
        public string Reason { get; set; }

        #endregion

        /// Init
        #region Init

        /// <summary>
        /// Constructor
        /// </summary>
        public AllowedToPrintResponse() : base()
        {
            
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="result"></param>
        /// <param name="allowedToPrint"></param>
        /// <param name="reason"></param>
        public AllowedToPrintResponse(string result, bool allowedToPrint, string reason)
            : base(result)
        {
            AllowedToPrint = allowedToPrint;
            Reason = reason;
        }

        #endregion
    }
}
