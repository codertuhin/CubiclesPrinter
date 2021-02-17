using System.Collections;
using System.ComponentModel;
using CubiclesPrinterSetupCustomAction.Helpers;

namespace CubiclesPrinterSetupCustomAction
{
    /// <summary>
    /// 
    /// </summary>
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        /// Init
        #region Init

        /// <summary>
        /// Constructor
        /// </summary>
        public Installer()
        {
            InitializeComponent();
        }

        #endregion

        /// Install
        #region Install

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            InstallHelper.Install();
        }

        #endregion

        /// Uninstall
        #region Uninstall

        /// <summary>
        /// 
        /// </summary>
        /// <param name="savedState"></param>
        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);

            InstallHelper.Uninstall();
        }

        #endregion

        /// Commit
        #region Commit

        /// <summary>
        /// 
        /// </summary>
        /// <param name="savedState"></param>
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
        }

        #endregion
    }
}
