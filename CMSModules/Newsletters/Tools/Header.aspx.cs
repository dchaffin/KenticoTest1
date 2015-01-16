using System;

using CMS.GlobalHelper;
using CMS.UIControls;

[Tabs("CMS.Newsletter", "", "newslettersContent")]
public partial class CMSModules_Newsletters_Tools_Header : CMSNewsletterPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Load action page directly, if set by URL
        switch (QueryHelper.GetString("action", null))
        {
            case "newnewsletter":
                CurrentMaster.Tabs.StartPageURL = URLHelper.ResolveUrl("Newsletters/Newsletter_New.aspx");
                break;
        }

        // Register script for unimenu button selection
        AddMenuButtonSelectScript(this, "Newsletters", null, "menu");

        // Allow resizer
        CurrentMaster.FrameResizer.Visible = true;
    }

    #endregion
}