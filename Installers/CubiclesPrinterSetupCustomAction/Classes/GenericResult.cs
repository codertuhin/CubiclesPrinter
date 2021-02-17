using System;

namespace CubiclesPrinterSetupCustomAction.Classes
{
    /// <summary>
    /// 
    /// </summary>
    public class GenericResult
    {
        /// <summary>
        /// 
        /// </summary>
        private string _method;

        /// <summary>
        /// 
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Method
        {
            get { return _method; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        public GenericResult(string method)
        {
            Success = false;
            Message = string.Empty;
            Exception = null;
            _method = method;
        }
    }
}
