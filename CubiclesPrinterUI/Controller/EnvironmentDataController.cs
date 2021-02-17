using CubiclesPrinterLib;
using CubiclesPrinterUI.Classes;
using System.Collections.Generic;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Cubicles.Utility;
using System;
using Cubicles.API;
using Cubicles.Utility.Helpers;

namespace CubiclesPrinterUI.Controller
{
    /// <summary>
    /// EnvironmentDataController class.
    /// </summary>
    public class EnvironmentDataController
    {
        /// Private Variables
        #region Private Variables

        /// <summary> Singleton </summary>
        private static EnvironmentDataController _instance;

        #endregion

        /// Properties
        #region Properties

        /// <summary> Singleton </summary>
        public static EnvironmentDataController Instance {  get { return _instance; } }

        /// <summary> </summary>
        public PrinterData Printers { get; set; }

        /// <summary> </summary>
        public PCData UserPCs { get; set; }

        /// <summary> </summary>
        public int ColorPageCost { get; set; }

        /// <summary> </summary>
        public int BlackAndWhitePageCost { get; set; }

        #endregion

        /// Init
        #region Init

        /// <summary>
        /// Constructor.
        /// </summary>
        static EnvironmentDataController()
        {
            // create instance
            _instance = new EnvironmentDataController();

            // initialize data
            //_instance.Init();
        }

        #endregion

        /// Methods
        #region Methods

        /// <summary>
        /// Initializes data.
        /// </summary>
        public void Init()
        {
            Load();
        }

        /// <summary>
        /// Saves data.
        /// </summary>
        private void Save()
        {
            try
            {
                string json = JsonConvert.SerializeObject(this);
                if (!string.IsNullOrWhiteSpace(json))
                    File.WriteAllText(ConfigData.FilePath_EnvironmentConfig, json);
            }
            catch(Exception ex)
            {
                WPFNotifier.Error(ex);
            }                
        }

        /// <summary>
        /// Loads local environment data.
        /// </summary>
        /// <returns>true if loaded; otherwise false</returns>
        private bool LoadLocal()
        {
            try
            {
                if (File.Exists(ConfigData.FilePath_EnvironmentConfig))
                {
                    string data = File.ReadAllText(ConfigData.FilePath_EnvironmentConfig);
                    if (!string.IsNullOrWhiteSpace(data))
                    {
                        return _Load(JToken.Parse(data));
                    }
                }
            }
            catch (Exception ex)
            {
                WPFNotifier.Error(ex);
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private bool _Load(JToken json)
        {
            if (json != null)
            {
                int colorPageCost;
                object cpc = json["ColorPageCost"];
                if (cpc != null)
                {
                    int.TryParse(cpc.ToString(), out colorPageCost);
                    ColorPageCost = colorPageCost;
                }

                int blackAndWhitePageCost;
                object bawpc = json["BlackAndWhitePageCost"];
                if (bawpc != null)
                {
                    int.TryParse(bawpc.ToString(), out blackAndWhitePageCost);
                    BlackAndWhitePageCost = blackAndWhitePageCost;
                }

                try
                {
                    Printers = json["Printers"].ToObject(typeof(PrinterData)) as PrinterData;
                }
                catch { }

                try
                {
                    UserPCs = json["UserPCs"].ToObject(typeof(PCData)) as PCData;
                }
                catch { }

                return true;
            }

            return false;
        }


        /// <summary>
        /// Loads remote environment data.
        /// </summary>
        /// <returns>true if loaded; otherwise false</returns>
        private bool LoadRemote()
        {
            try
            {
                var json = APIWrapper.GetEnvironment() as JToken;
                return _Load(json);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex);
                //WPFNotifier.Error(ex);
            }

            return false;
        }

        /// <summary>
        /// Loads data.
        /// </summary>
        private void Load()
        {
            // try to load data from server
            if (!LoadRemote())
                // load data from local environment config file
                LoadLocal();

            LogHelper.LogDebug("ColorPageCost " + ColorPageCost.ToString());
            LogHelper.LogDebug("BlackAndWhitePageCost " + BlackAndWhitePageCost.ToString());

            if (BlackAndWhitePageCost <= 0)
                BlackAndWhitePageCost = 1;

            if (ColorPageCost <= 0)
                ColorPageCost = 2;

            // check data and save
            if ((UserPCs != null && UserPCs.Count > 0) && (Printers != null && Printers.Count > 0))
                Save();

            // check
            if (UserPCs == null || UserPCs.Count < 1)
            {
                UserPCs = new PCData()
                {
                    new PCDataEntry{Name = "frontdesk1-pc", Margin = new Thickness(678, 18, 0, 0), MapPosition=MapPosition.Top},
                    new PCDataEntry{Name = "apiserver-pc", Margin = new Thickness(698, 18, 0, 0), MapPosition=MapPosition.Top},
                    new PCDataEntry{Name = "frontdesk2-pc", Margin = new Thickness(728, 18, 0, 0), MapPosition=MapPosition.Top},
                    new PCDataEntry{Name = "conference", Margin = new Thickness(66, 110, 0, 0), MapPosition=MapPosition.Top},
                    new PCDataEntry{Name = "backdesk-pc", Margin = new Thickness(90, 200, 0, 0), MapPosition=MapPosition.Side},
                    new PCDataEntry{Name = "printerleft-pc", Margin = new Thickness(760, 146, 0, 0), MapPosition=MapPosition.Top},
                    new PCDataEntry{Name = "printerright-pc", Margin = new Thickness(696, 146, 0, 0), MapPosition=MapPosition.Top},
                    new PCDataEntry{Name = "cubicle01-pc", Margin = new Thickness(628, 18, 0, 0), MapPosition=MapPosition.Top},
                    new PCDataEntry{Name = "cubicle02-pc", Margin = new Thickness(571, 18, 0, 0), MapPosition=MapPosition.Top},
                    new PCDataEntry{Name = "cubicle03-pc", Margin = new Thickness(514, 18, 0, 0), MapPosition=MapPosition.Top},
                    new PCDataEntry{Name = "cubicle04-pc", Margin = new Thickness(457, 18, 0, 0), MapPosition=MapPosition.Top},
                    new PCDataEntry{Name = "cubicle05-pc", Margin = new Thickness(400, 18, 0, 0), MapPosition=MapPosition.Top},
                    new PCDataEntry{Name = "cubicle06-pc", Margin = new Thickness(343, 18, 0, 0), MapPosition=MapPosition.Top},
                    new PCDataEntry{Name = "cubicle07-pc", Margin = new Thickness(286, 18, 0, 0), MapPosition=MapPosition.Top},
                    new PCDataEntry{Name = "cubicle08-pc", Margin = new Thickness(229, 18, 0, 0), MapPosition=MapPosition.Top},
                    new PCDataEntry{Name = "cubicle09-pc", Margin = new Thickness(172, 18, 0, 0), MapPosition=MapPosition.Top},
                    new PCDataEntry{Name = "cubicle10-pc", Margin = new Thickness(576, 172, 0, 0), MapPosition=MapPosition.Side},
                    new PCDataEntry{Name = "cubicle11-pc", Margin = new Thickness(526, 172, 0, 0), MapPosition=MapPosition.Side},
                    new PCDataEntry{Name = "cubicle12-pc", Margin = new Thickness(470, 172, 0, 0), MapPosition=MapPosition.Side},
                    new PCDataEntry{Name = "cubicle13-pc", Margin = new Thickness(418, 172, 0, 0), MapPosition=MapPosition.Side},
                    new PCDataEntry{Name = "cubicle14-pc", Margin = new Thickness(366, 172, 0, 0), MapPosition=MapPosition.Side},
                    new PCDataEntry{Name = "cubicle15-pc", Margin = new Thickness(314, 172, 0, 0), MapPosition=MapPosition.Side},
                    new PCDataEntry{Name = "cubicle16-pc", Margin = new Thickness(272, 172, 0, 0), MapPosition=MapPosition.Side},
                    new PCDataEntry{Name = "cubicle17-pc", Margin = new Thickness(210, 172, 0, 0), MapPosition=MapPosition.Side},
                    new PCDataEntry{Name = "cubicle18-pc", Margin = new Thickness(158, 172, 0, 0), MapPosition=MapPosition.Side},
                    new PCDataEntry{Name = "cubicle19-pc", Margin = new Thickness(176, 318, 0, 0), MapPosition=MapPosition.Bottom},
                    new PCDataEntry{Name = "cubicle20-pc", Margin = new Thickness(232, 318, 0, 0), MapPosition=MapPosition.Bottom},
                    new PCDataEntry{Name = "cubicle21-pc", Margin = new Thickness(288, 318, 0, 0), MapPosition=MapPosition.Bottom},
                    new PCDataEntry{Name = "cubicle22-pc", Margin = new Thickness(344, 318, 0, 0), MapPosition=MapPosition.Bottom},
                    new PCDataEntry{Name = "cubicle23-pc", Margin = new Thickness(400, 318, 0, 0), MapPosition=MapPosition.Bottom},
                };
            }

            // check valid printer destinations
            if (Printers == null || Printers.Count < 1)
            {
                // load defaults
                Printers = new PrinterData()
                {
                    new PrinterDataEntry(){Name ="backprinter", InternalName="backprinter", MarginGlow = new Thickness(207.5, 226, 0, 0), MarginLabel = new Thickness(141, 245, 0, 0), ExcludedPCNames = new List<string>(){ "backdesk-pc", "frontdesk1-pc", "frontdesk2-pc", "apiserver-pc" } },

                    // COLOR PRINTER $.75 CENTS PER PAGE
                    new PrinterDataEntry(){Name ="COLOR PRINTER $.75 CENTS PER PAGE", InternalName="colorprinter", MarginGlow = new Thickness(761, 190, 0, 0), MarginLabel = new Thickness(695, 212, 0, 0), ExcludedPCNames = new List<string>(){ "printerleft-pc", "printerright-pc", "frontdesk1-pc", "frontdesk2-pc", "apiserver-pc" }},
                    // BLACK & WHITE $.25 CENTS PER PAGE
                    new PrinterDataEntry(){Name ="BLACK & WHITE $.25 CENTS PER PAGE", InternalName="black-whiteprinter", MarginGlow = new Thickness(787.5, 190, 0, 0), MarginLabel = new Thickness(720, 212, 0, 0), ExcludedPCNames = new List<string>(){ "printerleft-pc", "printerright-pc", "frontdesk1-pc", "frontdesk2-pc", "apiserver-pc" }},
                    
                    new PrinterDataEntry(){Name ="cubicle01_printer", InternalName="cubicle01_printer",MarginGlow = new Thickness(685, 68.5, 0, 0), MarginLabel = new Thickness(618, 92, 0, 0), DoNotShowFootsteps = true},
                    new PrinterDataEntry(){Name ="cubicle02_printer", InternalName="cubicle02_printer",MarginGlow = new Thickness(629.5, 68.5, 0, 0), MarginLabel = new Thickness(563, 92, 0, 0), DoNotShowFootsteps = true},
                    new PrinterDataEntry(){Name ="cubicle05_printer", InternalName="cubicle05_printer",MarginGlow = new Thickness(464.5, 69, 0, 0), MarginLabel = new Thickness(398, 92, 0, 0), DoNotShowFootsteps = true},
                };
            }

               
            /*
            LogHelper.Log("Environment Data Printers");
            foreach (var item in Printers)
                LogHelper.Log(item.ToString());

            LogHelper.Log("Environment Data UserPCs");
            foreach (var item in UserPCs)
                LogHelper.Log(item.ToString());*/

            //Save();
        }

        #endregion
    }
}
