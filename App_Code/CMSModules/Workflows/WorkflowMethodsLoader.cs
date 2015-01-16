using System;

using CMS.SettingsProvider;

/// <summary>
/// workflow functions loader (registers workflow functions to macro resolver).
/// </summary>
[WorkflowModuleLoader]
public partial class CMSModuleLoader
{
    /// <summary>
    /// Attribute class ensuring correct initialization of methods in macro resolver.
    /// </summary>
    private class WorkflowModuleLoaderAttribute : CMSLoaderAttribute
    {
        /// <summary>
        /// Registers workflow methods.
        /// </summary>
        public override void Init()
        {
            WorkflowMethods.RegisterMethods();
        }
    }
}