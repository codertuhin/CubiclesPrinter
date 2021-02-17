using Newtonsoft.Json;

namespace Cubicles.API.JsonClasses
{
    /// <summary>
    /// This class represents base response for API requests
    /// </summary>
    public class JResponse
    {
        /// Properties
        #region Properties

        /// <summary> Request result. Has to be true for successful request </summary>
        [JsonProperty("result")]
        public string ResultString { get; set; }

        /// <summary> Flag indicates whether request was successful or not </summary>
        [JsonIgnore]
        public bool Result
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ResultString))
                    return false;

                if (ResultString.ToLower() == "success")
                    return true;

                return false;
            }
        }

        #endregion

        /// Init
        #region Init

        /// <summary>
        /// Constructor
        /// </summary>
        public JResponse()
        {
            
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="result">Request result</param>
        public JResponse(string result)
        {
            ResultString = result;
        }

        #endregion
    }
}
