using Microsoft.Win32;

namespace CubiclesPrinter.Helpers
{
    /// <summary>
    /// This class contains methods for registry interactions
    /// </summary>
    public static class RegistryHelper
    {
        /// <summary>
        /// Reads registry 32 bit
        /// </summary>
        /// <param name="path">path to be read</param>
        /// <param name="key">key to be read</param>
        /// <returns>value</returns>
        public static string ReadRegistry32(string path, string key)
        {
            return ReadRegistry(path, key, RegistryView.Registry32);
        }

        /// <summary>
        /// Reads registry 64 bit
        /// </summary>
        /// <param name="path">path to be read</param>
        /// <param name="key">key to be read</param>
        /// <returns>value</returns>
        public static string ReadRegistry64(string path, string key)
        {
            return ReadRegistry(path, key, RegistryView.Registry64);
        }

        /// <summary>
        /// Reads registry
        /// </summary>
        /// <param name="path">path to be read</param>
        /// <param name="key">key to be read</param>
        /// <param name="view">32 or 64 bit view</param>
        /// <returns>value</returns>
        static string ReadRegistry(string path, string key, RegistryView view)
        {
            var retVal = string.Empty;
            RegistryKey localKey = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, view);
            localKey = localKey.OpenSubKey(path);
            if (localKey != null)
            {
                retVal = (string)localKey.GetValue(key);
            }
            return retVal;
        }
    }
}
