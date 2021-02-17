using System;
using System.Collections.Generic;
using Cubicles.Extensions;
using Cubicles.Utility;
using Cubicles.Utility.Helpers;
using RestSharp;

namespace Cubicles.API
{
    /// <summary>
    /// This class allows to perform connection to server
    /// </summary>
    public sealed class Connection
    {
        /// Properties
        #region Properties
        
        /// <summary> Last call made by this class </summary>
        public static string LastCall { get; private set; }

        #endregion

        /// Get
        #region Get

        /// <summary>
        /// Initiates get request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static IRestResponse Get(string url, Dictionary<string, string> queryParameters = null)
        {
            try
            {
                string req = url;
                if (queryParameters != null && queryParameters.Count > 0)
                    req += "?" + queryParameters.GetAsQueryString();

                LastCall = req;

                LogHelper.Log(new Uri(req).AbsoluteUri);

                RestClient client = new RestClient(new Uri(req));
                RestRequest request = new RestRequest(Method.GET);
                request.Timeout = 5000;
                request.AddParameter("undefined", "{}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                return response;
            }
            catch (Exception ex)
            {
                WPFNotifier.DebugError(ex);
                return null;
            }
        }

        #endregion

        /// Post
        #region Post

        /// <summary>
        /// Initiates post request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static IRestResponse Post(string url, Dictionary<string, string> queryParameters = null)
        {
            try
            {
                string req = url;
                if (queryParameters != null && queryParameters.Count > 0)
                    req += "?" + queryParameters.GetAsQueryString();

                LastCall = req;

                LogHelper.LogDebug(new Uri(req).AbsoluteUri);

                RestClient client = new RestClient(new Uri(req));
                RestRequest request = new RestRequest(Method.POST);
                request.Timeout = 5000;
                request.AddParameter("undefined", "{}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                return response;
            }
            catch (Exception ex)
            {
                WPFNotifier.DebugError(ex);
                return null;
            }
        }

        #endregion
    }
}
