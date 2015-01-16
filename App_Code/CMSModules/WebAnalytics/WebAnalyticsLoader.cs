using System;

using CMS.SettingsProvider;

/// <summary>
/// Web analytics functions loader (registers online marketing functions to macro resolver).
/// </summary>
[WebAnalyticsModuleLoader]
public partial class CMSModuleLoader
{
    /// <summary>
    /// Attribute class ensuring correct initialization of methods in macro resolver.
    /// </summary>
    private class WebAnalyticsModuleLoaderAttribute : CMSLoaderAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public WebAnalyticsModuleLoaderAttribute()
        {
            // Require Web analytics module to load properly
            RequiredModules = new string[] { ModuleEntry.ONLINEMARKETING };
        }


        /// <summary>
        /// Registers web analytics methods.
        /// </summary>
        public override void Init()
        {
            WebAnalyticsMethods.RegisterMethods();
        }
    }

    public class VisitorNamespace
    {
        public VisitorNamespace()
        {

        }
    }
}