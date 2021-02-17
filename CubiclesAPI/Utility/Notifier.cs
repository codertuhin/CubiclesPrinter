using System;
using System.Windows.Forms;
using Cubicles.Utility.Helpers;

namespace Cubicles.Utility
{
    /*
    /// <summary>
    /// This class allows user to see the usage or error information and notifications
    /// </summary>
    public class Notifier
    {
        /// Question
        #region Question

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public static DialogResult Question(string message, string caption = null)
        {
            return Show(message, caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2, MessageBoxOptions.DefaultDesktopOnly);
        }

        #endregion

        /// Notify
        #region Notify

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public static DialogResult Notify(string message, string caption = null)
        {
            return Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
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
        public static DialogResult Warning(string message, string caption = null)
        {
            return Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
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
        public static DialogResult DebugError(Exception ex, object data = null)
        {
//#if (DEBUG)
            return DebugError(ex.ToString(), data, 3);
//#endif
            return DialogResult.None;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static DialogResult DebugError(string message, object data = null, int index = 2)
        {
//#if (DEBUG)
            string messagex = string.Format("{1}{0}{2}", Environment.NewLine, ReflectionHelper.GetCallingString(index), message);
            LogHelper.Log(messagex);
            return Error(messagex, null, null, data);
//#endif
            return DialogResult.None;
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
        public static DialogResult Error(Exception ex, object data = null)
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
        public static DialogResult Error(string message, string caption = null, Exception exception = null, object data = null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                if (exception == null)
                {
                    if (data == null)
                        return DialogResult.None;

                    message = data.ToString();
                }
                else
                {
                    message = exception.ToString();
                }
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

            return Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
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
        private static DialogResult Show(string message, string caption, MessageBoxButtons btns, MessageBoxIcon icon, MessageBoxDefaultButton dbtn, MessageBoxOptions opts)
        {
            if (string.IsNullOrWhiteSpace(message))
                return DialogResult.None;

            if (string.IsNullOrWhiteSpace(caption))
                //caption = Assembly.GetExecutingAssembly().FullName;
                caption = "Cubicles";

            return MessageBox.Show(message, caption, btns, icon, dbtn, opts);
        }

        #endregion
    }*/
}
