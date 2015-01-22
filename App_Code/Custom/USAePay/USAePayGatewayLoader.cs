using System;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.Ecommerce;

/// <summary>
/// Loader class for USAePayGateway class
/// </summary>
[USAePayGatewayLoader]
public partial class CMSModuleLoader
{

    #region "Macro methods loader attribute"

    /// <summary>
    /// Module registration
    /// </summary>
    private class USAePayGatewayLoaderAttribute : CMSLoaderAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public USAePayGatewayLoaderAttribute()
        {
            RequiredModules = new string[] { ModuleEntry.ECOMMERCE };
        }

        /// <summary>
        /// Initializes the module
        /// </summary>
        public override void Init()
        {
            ClassHelper.OnGetCustomClass += GetCustomClass;
        }

        private static void GetCustomClass(object sender, ClassEventArgs e)
        {
            if (e.Object == null)
            {
                switch (e.ClassName.ToLower())
                {
                    case "usaepaygateway":
                        e.Object = new USAePayGateway();
                        break;
                }
            }
        }
    }

    #endregion
}