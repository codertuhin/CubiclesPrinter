using Cubicles.Utility.Helpers;
using System;
using System.Windows;

namespace Cubicles.Utility
{
    /// <summary>
    /// This class allows to notify user about usage events or issues. WPF version
    /// </summary>
    public class WPFNotifier
    {
        /// Notify
        #region Notify

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public static MessageBoxResult Notify(string message, string caption = null)
        {
            return Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None, MessageBoxOptions.DefaultDesktopOnly);
        }

        #endregion

        /// Warning
        #region Warning

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public static MessageBoxResult Warning(string message, string caption = null)
        {
            return Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.None, MessageBoxOptions.DefaultDesktopOnly);
        }

        #endregion

        /// Question
        #region Question

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public static MessageBoxResult Question(string message, string caption = null)
        {
            return Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No, MessageBoxOptions.DefaultDesktopOnly);
        }

        #endregion

        /// DebugError
        #region DebugError

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static MessageBoxResult DebugError(Exception ex, object data = null)
        {
            #if (DEBUG)
            return DebugError(ex.ToString(), data, 3);
            #else
            return MessageBoxResult.None;
            #endif
        }

        public static MessageBoxResult DebugError(string message, object data = null, int index = 2)
        {
            #if (DEBUG)
            string messagex = string.Format("{1}{0}{2}", Environment.NewLine, ReflectionHelper.GetCallingString(index), message);
            LogHelper.Log(messagex);
            return Error(messagex, null, null, data);
            #else
            return MessageBoxResult.None;
            #endif
        }

        #endregion

        /// Error
        #region Error

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static MessageBoxResult Error(Exception ex, object data = null)
        {
            return Error(null, null, ex, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <param name="exception"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static MessageBoxResult Error(string message, string caption = null, Exception exception = null, object data = null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                if (exception == null)
                {
                    if (data == null)
                        return MessageBoxResult.None;

                    message = data.ToString();
                }
                else
                    message = exception.ToString();
            }
            else
            {
                if (exception != null)
                    message += Environment.NewLine + exception.ToString();
            }

            if (data != null)
            {
                message += Environment.NewLine + data.ToString();
            }

            LogHelper.Log(message);

            return Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None, MessageBoxOptions.DefaultDesktopOnly);
        }

        #endregion

        /// Show
        #region Show

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <param name="btns"></param>
        /// <param name="icon"></param>
        /// <param name="dbtn"></param>
        /// <param name="opts"></param>
        /// <returns></returns>
        private static MessageBoxResult Show(string message, string caption, MessageBoxButton btns, MessageBoxImage icon, MessageBoxResult dbtn, MessageBoxOptions opts)
        {
            if (string.IsNullOrWhiteSpace(message))
                return MessageBoxResult.None;

            if (string.IsNullOrWhiteSpace(caption))
                caption = "Cubicles";

            LogHelper.Log(message);

            return MessageBox.Show(message, caption, btns, icon, dbtn, opts);
        }

        #endregion
    }
}
