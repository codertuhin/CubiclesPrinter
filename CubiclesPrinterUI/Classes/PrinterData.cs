using Cubicles.Extensions;
using System.Collections.Generic;
using System.Windows;

namespace CubiclesPrinterUI.Classes
{
    /// <summary>
    /// PrinterDataEntry class;
    /// </summary>
    public class PrinterDataEntry
    {
        /// Properties
        #region Properties

        /// <summary> Name </summary>
        public string Name { get; set; }

        /// <summary> Internal Name </summary>
        public string InternalName { get; set; }

        /// <summary> Cost Per Page </summary>
        public string CostPerPage { get; set; }

        /// <summary> Excluded PC Names </summary>
        public List<string> ExcludedPCNames { get; set; }

        /// <summary> Label </summary>
        public Thickness MarginLabel { get; set; }

        /// <summary> Glow </summary>
        public Thickness MarginGlow { get; set; }

        /// <summary> Do Not Show Footsteps </summary>
        public bool DoNotShowFootsteps { get; set; }

        #endregion

        /// Init
        #region Init

        /// <summary>
        /// Constructor.
        /// </summary>
        public PrinterDataEntry()
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
    /// PrinterData class - a collection of PrinterDataEntry objects.
    /// </summary>
    public class PrinterData : List<PrinterDataEntry>
    {
        /// Methods
        #region Methods

        /// <summary>
        /// Checks if the specified name exists in the printer data collection.
        /// </summary>
        /// <param name="name">name to be checked</param>
        /// <returns>true if exists; otherwise false</returns>
        public bool HasEntry(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            // iterate through all the items
            foreach (var item in this)
            {
                if (item != null)
                    if (item.Name.EqualsSimplified(name))
                        return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="printername"></param>
        /// <param name="pcname"></param>
        /// <returns></returns>
        public bool IsValidDestination(string printername, string pcname)
        {
            if (string.IsNullOrWhiteSpace(printername))
                return false;

            // iterate through all the items
            foreach (var item in this)
            {
                if (item != null)
                {
                    if (item.DoNotShowFootsteps)
                        continue;

                        if (item.Name.EqualsSimplified(printername))
                        {
                            if (item.ExcludedPCNames != null)
                                foreach (var epc in item.ExcludedPCNames)
                                    if (epc.EqualsSimplified(pcname))
                                        return false;

                            return true;
                        }
                }
            }

            return false;
        }

        public string GetValidDestination(string printername, string pcname)
        {
            if (string.IsNullOrWhiteSpace(printername))
                return null;

            // iterate through all the items
            foreach (var item in this)
            {
                if (item != null)
                {
                    if (item.DoNotShowFootsteps)
                        continue;

                    if (item.Name.EqualsSimplified(printername))
                    {
                        if (item.ExcludedPCNames != null)
                            foreach (var epc in item.ExcludedPCNames)
                                if (epc.EqualsSimplified(pcname))
                                    return null;

                        return item.InternalName;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Thickness GetLabel(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                // Unrecognized
                return new Thickness(-400, -400, 0, 0); ;

            foreach (var item in this)
                if (item != null)
                    if (!string.IsNullOrWhiteSpace(item.Name))
                        if (item.Name.EqualsSimplified(name))
                            return item.MarginLabel;

            // Unrecognized
            return new Thickness(-400, -400, 0, 0); ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Thickness GetGlow(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                // Unrecognized
                return new Thickness(-400, -400, 0, 0); ;

            foreach (var item in this)
                if (item != null)
                    if (!string.IsNullOrWhiteSpace(item.Name))
                        if (item.Name.EqualsSimplified(name))
                            return item.MarginGlow;

            // Unrecognized
            return new Thickness(-400, -400, 0, 0); ;
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
}
