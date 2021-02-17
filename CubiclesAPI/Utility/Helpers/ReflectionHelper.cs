using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Cubicles.Utility.Helpers
{
    /// <summary>
    /// This class contains different reflection methods
    /// </summary>
    public class ReflectionHelper
    {
        
        /// <summary>
        /// Gets calling method from the stack trace
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        static string GetCallingMethod(int index)
        {
            try
            {
                return new StackTrace().GetFrame(index).GetMethod().Name;
            }
            catch
            {
                return "";
            }
        }

        
        /// <summary>
        /// Gets calling string from the stack trace
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCallingString(int index)
        {
            try
            {
                var mth = new StackTrace().GetFrame(index).GetMethod();
                return string.Format("{0} : {1}", mth.ReflectedType.Name, mth.Name);
            }
            catch
            {
                return "";
            }
        }

        /// ToString
        #region ToString

        /// <summary>
        /// Creates a string representation of object's properties enumeration
        /// Parameter can be null. No exception is thrown
        /// </summary>
        /// <param appName="src"></param>
        /// <returns></returns>
        public static string ToString(object src)
        {
            if (src == null)
                return "";

            try
            {
                PropertyInfo[] pis = src.GetType().GetProperties();
                if (pis == null || pis.Length < 1)
                    return "";

                StringBuilder SB = new StringBuilder();
                foreach (PropertyInfo pi in pis)
                {
                    try
                    {
                        string name = pi.Name;
                        if (string.IsNullOrEmpty(name))
                            continue;

                        object val = pi.GetValue(src, null);
                        if (val == null)
                        {
                            SB.AppendLine(string.Format("{0} : {1}, ", name, ""));
                        }
                        else
                        {
                            if (val.GetType() == typeof(double))
                            {
                                SB.AppendLine(string.Format("{0} : {1}, ", name, ((double)val).ToString("F")));
                            }
                            else
                                SB.AppendLine(string.Format("{0} : {1}, ", name, val));
                        }
                    }
                    catch
                    {
                    }
                }

                string sss = SB.ToString();
                int pos = sss.LastIndexOf(',');
                if (pos >= 0)
                {
                    sss = sss.Substring(0, pos);
                }

                return sss;
            }
            catch
            {
            }

            return "";
        }

        public static string FieldsToString(object src)
        {
            if (src == null)
                return "";

            try
            {
                FieldInfo[] pis = src.GetType().GetFields();
                if (pis == null || pis.Length < 1)
                    return "";

                StringBuilder SB = new StringBuilder();
                foreach (FieldInfo pi in pis)
                {
                    try
                    {
                        string name = pi.Name;
                        if (string.IsNullOrEmpty(name))
                            continue;

                        object val = pi.GetValue(src);
                        if (val == null)
                        {
                            SB.AppendLine(string.Format("{0} : {1}, ", name, ""));
                        }
                        else
                        {
                            if (val.GetType() == typeof(double))
                            {
                                SB.AppendLine(string.Format("{0} : {1}, ", name, ((double)val).ToString("F")));
                            }
                            else
                                SB.AppendLine(string.Format("{0} : {1}, ", name, val));
                        }
                    }
                    catch
                    {
                    }
                }

                string sss = SB.ToString();
                int pos = sss.LastIndexOf(',');
                if (pos >= 0)
                {
                    sss = sss.Substring(0, pos);
                }

                return sss;
            }
            catch
            {
            }

            return "";
        }

        #endregion
    }
}
