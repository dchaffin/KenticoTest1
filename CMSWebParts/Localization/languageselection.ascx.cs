using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;

using CMS.PortalControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.CMSHelper;

public partial class CMSWebParts_Localization_languageselection : CMSAbstractLanguageWebPart
{
    #region "Variables"
    
    private string mSeparator = " ";

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the display layout.
    /// </summary>
    public string DisplayLayout
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DisplayLayout"), "");
        }
        set
        {
            SetValue("DisplayLayout", value);
            mSeparator = value.ToLowerCSafe() == "vertical" ? "<br />" : " ";
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Reloads data for partial caching.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do not process
        }
        else
        {
            // Get list of cultures
            List<string[]> cultures = GetCultures();
            
            // Check whether exists more than one culture
            if ((cultures != null) && ((cultures.Count > 1) || (HideCurrentCulture && (cultures.Count > 0))))
            {
                // Set separator with dependence on layout
                mSeparator = DisplayLayout.ToLowerCSafe() == "vertical" ? "<br />" : " ";
                
                // Cultures literal
                ltlHyperlinks.Text = String.Empty;
                // Indicates whether separator can be added
                bool addSeparator = false;
                // Keep current document culture
                string currentCulture = CMSContext.CurrentDocument.DocumentCulture;

                // Loop thru all cultures
                foreach (string[] data in cultures)
                {
                    string url = data[0];
                    string code = data[1];
                    string name = data[2];

                    // Add separator if it;s allowed
                    if (addSeparator)
                    {
                        ltlHyperlinks.Text += mSeparator;
                    }

                    // Display link if document culture for current document is not the same
                    if (CMSString.Compare(code, currentCulture, true) != 0)
                    {
                        ltlHyperlinks.Text += "<a href=\"" + URLHelper.ResolveUrl(url) + "\">" + HTMLHelper.HTMLEncode(name) + "</a>";
                    }
                        // For the same doc. cultures display plain text
                    else
                    {
                        ltlHyperlinks.Text += name;
                    }
                    // Add separator for next run
                    addSeparator = true;
                }
            }
                // Hide lang. selector if there is not more than one culture
            else
            {
                Visible = false;
            }
        }
    }

    #endregion
}