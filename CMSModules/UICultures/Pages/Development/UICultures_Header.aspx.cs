using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_UICultures_Pages_Development_UICultures_Header : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize the control
        SetupControl();
    }


    #region "Private methods"

    /// <summary>
    /// Initializes the controls.
    /// </summary>
    private void SetupControl()
    {
        // Set title
        CurrentMaster.Title.TitleText = GetString("Development-UICulture_List.Title");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_UICulture/object.png");

        CurrentMaster.Title.HelpName = "helpTopic";
        CurrentMaster.Title.HelpTopicName = "uicultures_defaultculture";

        InitializeMenu();
    }


    /// <summary>
    /// Initialize the tab control on the master page.
    /// </summary>
    private void InitializeMenu()
    {
        string[,] tabs = new string[2,4];

        string defaultCulture = CultureHelper.DefaultUICulture;

        // Default UI culture strings
        tabs[0, 0] = GetString("UICultures.Default");
        tabs[0, 1] = "SetHelpTopic('helpTopic', 'uicultures_defaultculture');";
        tabs[0, 2] = "ResourceString/List.aspx?uicultureid=";
        try
        {
            UICultureInfo ci = UICultureInfoProvider.GetUICultureInfo(defaultCulture);
            if (ci != null)
            {
                tabs[0, 2] += ci.UICultureID;
            }
        }
        catch
        {
        }

        tabs[1, 0] = GetString("UICultures.Other");
        tabs[1, 1] = "SetHelpTopic('helpTopic', 'uicultures_tab_other');";
        tabs[1, 2] = "UICulture/List.aspx";

        // Set the target iFrame
        CurrentMaster.Tabs.UrlTarget = "uiculturesContent";

        // Assign tabs data
        CurrentMaster.Tabs.Tabs = tabs;
    }

    #endregion
}