using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using Cubicles.Extensions;
using Cubicles.Utility;
using Newtonsoft.Json;

namespace Cubicles.API.JsonClasses
{
    /// <summary>
    /// This class represents printer entry of GetPrinters API request
    /// </summary>
    public sealed class Printer : JResponse
    {
        /// Properties
        #region Properties

        /// <summary> Gets or sets printer name </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary> Gets or sets display printer name </summary>
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        /// <summary> Gets or sets if it's the default printer </summary>
        [JsonProperty("isDefault")]
        public bool IsDefault { get; set; }

        /// <summary> Gets or sets if it's the colored printer </summary>
        [JsonProperty("isColored")]
        public bool IsColored { get; set; }
        
        /// <summary> Gets or sets printer issue </summary>
        [JsonIgnore]
        public PrinterTrouble Issue { get; set; }

        /// <summary> Gets or sets printer settings </summary>
        [JsonIgnore]
        public PrinterSettings Settings { get; set; }

        #endregion

        /// Init
        #region Init

        /// <summary>
        /// Constructor
        /// </summary>
        public Printer()
        {
            
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">printer name</param>
        /// <param name="displayName">display printer name</param>
        /// <param name="isdefault">flag of the default printer</param>
        /// <param name="isColored">flag of the colored printer</param>
        public Printer(string name, string displayName, bool isdefault, bool isColored)
            : base("success")
        {
            Name = name;
            DisplayName = displayName;
            IsColored = isColored;
            IsDefault = isdefault;
        }

        #endregion

        /// Override
        #region Override

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns>a string</returns>
        public override string ToString()
        {
            return this.MegaToString();
        }
        
        #endregion
    }

    /// <summary>
    /// This class represents a list of printers
    /// </summary>
    public class Printers : ObservableCollection<Printer>
    {
        /// Properties
        #region Properties

        /// <summary> Gets colored printers </summary>
        public Printers Colored
        {
            get
            {
                Printers list = new Printers();
                foreach (Printer printer in this)
                    if (printer.IsColored)
                        list.Add(printer);

                return list;
            }
        }

        /// <summary> Gets only black and white printers </summary>
        public Printers BlackAndWhite
        {
            get
            {
                Printers list = new Printers();
                foreach (Printer printer in this)
                    if (!printer.IsColored)
                        list.Add(printer);

                return list;
            }
        }

        /// <summary> Gets names of the colored printers </summary>
        public List<string> ColoredNames
        {
            get
            {
                List<string> list = new List<string>();
                foreach (Printer printer in this)
                    if (printer.IsColored)
                        list.Add(printer.DisplayName);

                return list;
            }
        }

        /// <summary> Gets names of the black and white printers </summary>
        public List<string> BlackAndWhiteNames
        {
            get
            {
                List<string> list = new List<string>();
                foreach (Printer printer in this)
                    if (!printer.IsColored)
                        list.Add(printer.DisplayName);
                return list;
            }
        }

        /// <summary> Gets default printer name </summary>
        //public string DefaultPrinterName{get{return this.FirstOrDefault(x => x.IsDefault).DisplayName;}}

        public Printer DefaultPrinter{get{return this.FirstOrDefault(x => x.IsDefault);}}

        /// <summary> Gets all printers names </summary>
        public List<string> AllNames { get { return this.Select(x => x.DisplayName).ToList(); } }

        /// <summary> Gets real printers names </summary>
        public List<string> NamesInSystem { get { return this.Select(x => x.Name).ToList(); } }

        /// <summary> Gets real printers names </summary>
        public List<string> NamesInSystemWithoutServer 
        { 
            get 
            { 
                return this.Select(x => 
                { 
                    int pos = x.Name.LastIndexOf("\\");
                    if (pos > 0) 
                        return x.Name.Substring(pos + 1);
                    return x.Name;
                }).ToList(); 
            } 
        }

        #endregion

        /// Remove
        #region Remove

        /// <summary>
        /// Removes specified printer from the list by its' name
        /// </summary>
        /// <param name="printer">printer</param>
        public void RemoveByName(Printer printer)
        {
            if (printer == null)
                return;

            RemoveByName(printer.Name);
        }

        /// <summary>
        /// Removes specified printer from the list by its' name
        /// </summary>
        /// <param name="printerName">printer name</param>
        public void RemoveByName(string printerName)
        {
            if (string.IsNullOrWhiteSpace(printerName))
                return;

            for (int i = 0; i < this.Count; i++)
                if (this[i].Name == printerName)
                {
                    RemoveAt(i);
                    return;
                }
        }

        #endregion

        /// IndexOf
        #region IndexOf

        /// <summary>
        /// Gets printer index in list by its' name
        /// </summary>
        /// <param name="printer">printer</param>
        /// <returns>index of this printer</returns>
        public int IndexOfByName(Printer printer)
        {
            if (printer == null)
                return -1;

            return IndexOfByName(printer.Name);
        }

        /// <summary>
        /// Gets printer index in list by its' name
        /// </summary>
        /// <param name="printerName">printer name</param>
        /// <returns>index of this printer</returns>
        public int IndexOfByName(string printerName)
        {
            if (string.IsNullOrWhiteSpace(printerName))
                return -1;

            for(int i = 0; i < this.Count; i++)
                if (this[i].Name == printerName)
                    return i;

            return -1;
        }

        #endregion

        /// Contains
        #region Contains

        /// <summary>
        /// Checks if the list of printers contains specified printer
        /// </summary>
        /// <param name="printer">printer</param>
        /// <returns>true if contains; otherwise false</returns>
        public bool ContainsByName(Printer printer)
        {
            if (printer == null)
                return false;

            return ContainsByName(printer.Name);
        }

        /// <summary>
        /// Checks if the list of printers contains specified printer
        /// </summary>
        /// <param name="printerName">printer name</param>
        /// <returns>true if contains; otherwise false</returns>
        public bool ContainsByName(string printerName)
        {
            if (string.IsNullOrWhiteSpace(printerName))
                return false;

            foreach (var printer in this)
            {
                if (printer.Name == printerName)
                    return true;
            }

            return false;
        }
        
        #endregion

        /// Filter Printers
        #region Filter Printers

        /// <summary>
        /// Performs printer filtering by color option. 
        /// </summary>
        /// <param name="colored">can print color</param>
        /// <returns>list of either all printers or colored printers only</returns>
        public Printers ByColor(bool colored)
        {
            if (colored)
                return Colored;

            return this;
        }

        #endregion

        /// Names
        #region Names

        /// <summary>
        /// Gets names of the printers filtered by color option
        /// </summary>
        /// <param name="colored">can print color</param>
        /// <returns>names of the printers</returns>
        public List<string> NamesByColor(bool colored)
        {
            if (colored)
                return ColoredNames;

            return BlackAndWhiteNames;
        }

        /// <summary>
        /// Gets printer's display name by it's real name
        /// </summary>
        /// <param name="printerName"></param>
        /// <returns></returns>
        public string GetDisplayName(string printerName)
        {
            if (string.IsNullOrWhiteSpace(printerName))
                return null;

            foreach (var item in this)
            {
                if (item.Name == printerName)
                    return item.DisplayName;
            }

            return null;
        }

        #endregion

        /// Methods
        #region Methods

        /// <summary>
        /// Saves printers data.
        /// </summary>
        public void Save(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            try
            {
                /*
                JArray array = new JArray();
                foreach (var item in this)
                    array.Add(JToken.Parse(JsonConvert.SerializeObject(item)));

                string json = array.ToString();*/
                string json = JsonConvert.SerializeObject(this.ToArray());

                if (!string.IsNullOrWhiteSpace(json))
                    File.WriteAllText(fileName, json);
            }
            catch (Exception ex)
            {
                WPFNotifier.Error(ex);
            }
        }

        #endregion
    }

    /// PrintTrouble
    #region PrintTrouble

    /// <summary>
    /// Enumeration of printer troubles
    /// </summary>
    [Flags]
    public enum PrinterTrouble
    {
        None = 0,
        OutOfToner = 1,
        OutOfPaper = 2,
        Offline = 4,
        PaperJammed = 8,
        Error = 16,
    }

    #endregion
}
