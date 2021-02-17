using Cubicles.Extensions;
using System.Collections.Generic;
using System.Windows;

namespace CubiclesPrinterUI.Classes
{
    /// <summary>
    /// PCDataEntry class.
    /// </summary>
    public class PCDataEntry
    {
        /// Properties
        #region Properties

        /// <summary> Name </summary>
        public string Name { get; set; }

        /// <summary> Margin </summary>
        public Thickness Margin { get; set; }

        /// <summary> Map Position </summary>
        public MapPosition MapPosition { get; set; }

        #endregion

        /// Init
        #region Init

        /// <summary>
        /// Constructor.
        /// </summary>
        public PCDataEntry()
        {
        }

        #endregion

        /// Override
        #region Override

        /// <summary>
        /// Return a string that represents the current object.
        /// </summary>
        /// <returns>string representation</returns>
        public override string ToString()
        {
            return this.MegaToString();
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public enum MapPosition
    {
        Top = 0,
        Bottom = 1,
        Side = 2
    }

    /// <summary>
    /// PCDataList class - a collection of PCDataEntry
    /// </summary>
    public class PCData : List<PCDataEntry>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Thickness GetThickness(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                // Unrecognized
                return new Thickness(-400, -400, 0, 0); ;

            foreach (var item in this)
                if (item != null)
                    if (!string.IsNullOrWhiteSpace(item.Name))
                        if (item.Name.EqualsSimplified(name))
                            return item.Margin;

            // Unrecognized
            return new Thickness(-400, -400, 0, 0); ;
        }

        /// <summary>
        /// Checks if the specified name exists in the collection.
        /// </summary>
        /// <param name="name">name to be checked</param>
        /// <param name="position">position to be checked</param>
        /// <returns></returns>
        public bool HasPC(string name, MapPosition position)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            // iterate through all the items
            foreach (var item in this)
            {
                if (item != null)
                    if (item.Name.EqualsSimplified(name) && item.MapPosition == position)
                        return true;
            }

            return false;
        }
    }
}
