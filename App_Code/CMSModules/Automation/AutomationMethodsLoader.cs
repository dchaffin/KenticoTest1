using System;

using CMS.SettingsProvider;

/// <summary>
/// Automation functions loader (registers automation functions to macro resolver).
/// </summary>
[AutomationModuleLoader]
public partial class CMSModuleLoader
{
    /// <summary>
    /// Attribute class ensuring correct initialization of methods in macro resolver.
    /// </summary>
    private class AutomationModuleLoaderAttribute : CMSLoaderAttribute
    {
        /// <summary>
        /// Registers automation methods.
        /// </summary>
        public override void Init()
        {
            AutomationMethods.RegisterMethods();
        }
    }
}