using System;
using System.Collections.Generic;

using CMS.Controls;
using CMS.SettingsProvider;

/// <summary>
/// Module loader class (registers methods for macro resolver).
/// </summary>
public partial class CMSModuleLoader
{
    /// <summary>
    /// List of attributes defined
    /// </summary>
    private static List<CMSLoaderAttribute> mAttributes = null;


    /// <summary>
    /// List of attributes defined
    /// </summary>
    protected static List<CMSLoaderAttribute> Attributes
    {
        get
        {
            if (mAttributes == null)
            {
                var list = new List<CMSLoaderAttribute>();

                // Get the attributes
                Type type = typeof(CMSModuleLoader);
                object[] attributes = type.GetCustomAttributes(typeof(CMSLoaderAttribute), true);

                bool found = ((attributes != null) && (attributes.Length > 0));
                if (found)
                {
                    // Process all attributes
                    foreach (CMSLoaderAttribute attribute in attributes)
                    {
                        list.Add(attribute);
                    }
                }

                mAttributes = list;
            }

            return mAttributes;
        }
    }


    /// <summary>
    /// Constructor
    /// </summary>
    protected CMSModuleLoader()
    {
    }


    /// <summary>
    /// Initializes all modules
    /// </summary>
    public static void InitializeModules()
    {
        foreach (CMSLoaderAttribute attr in Attributes)
        {
            if (attr.CheckModules())
            {
                attr.Init();
            }
        }
    }


    /// <summary>
    /// PreInitializes all modules
    /// </summary>
    public static void PreInitializeModules()
    {
        foreach (CMSLoaderAttribute attr in Attributes)
        {
            attr.PreInit();
        }
    }
}