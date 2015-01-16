using System;

using CMS.SettingsProvider;

/// <summary>
/// Messageboard functions loader (registers messageboard functions to macro resolver).
/// </summary>
[MessageBoardModuleLoaderAttribute]
public partial class CMSModuleLoader
{
    /// <summary>
    /// Attribute class ensuring correct initialization of methods in macro resolver.
    /// </summary>
    public class MessageBoardModuleLoaderAttribute : CMSLoaderAttribute
    {
        /// <summary>
        /// Registers messageboard methods.
        /// </summary>
        public override void Init()
        {
            MessageBoardMethods.RegisterMethods();
        }
    }
}