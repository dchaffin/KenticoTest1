using System;

using CMS.SettingsProvider;

/// <summary>
/// Content functions loader (registers content functions to macro resolver).
/// </summary>
[ContentModuleLoader]
public partial class CMSModuleLoader
{
    /// <summary>
    /// Attribute class ensuring correct initialization of methods in macro resolver.
    /// </summary>
    public class ContentModuleLoaderAttribute : CMSLoaderAttribute
    {
        /// <summary>
        /// Registers content methods.
        /// </summary>
        public override void Init()
        {
            ContentMethods.RegisterMethods();
        }
    }
}