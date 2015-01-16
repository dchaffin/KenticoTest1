using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.Newsletter;
using CMS.Scheduler;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

// Title
[Title("Objects/Newsletter_Newsletter/object.png", "newsletters.newsletters", "CMS_Newsletter_Newsletters", HelpName = "helpTopic")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_List : CMSNewsletterNewslettersPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // New record link
        string[,] actions = new string[1,6];
        actions[0, 0] = HeaderActions.TYPE_HYPERLINK;
        actions[0, 1] = GetString("Newsletter_List.NewItemCaption");
        actions[0, 2] = null;
        actions[0, 3] = ResolveUrl("Newsletter_New.aspx");
        actions[0, 4] = null;
        actions[0, 5] = GetImageUrl("Objects/Newsletter_Newsletter/add.png");

        // Setup Master page
        CurrentMaster.HeaderActions.Actions = actions;

        // Setup UniGrid
        UniGrid.OnAction += new OnActionEventHandler(uniGrid_OnAction);
        UniGrid.WhereCondition = "NewsletterSiteID=" + CMSContext.CurrentSiteID;
        UniGrid.ZeroRowsText = GetString("general.nodatafound");
    }


    /// <summary>
    /// Increment counter at the end of string.
    /// </summary>
    /// <param name="s">String</param>
    /// <param name="lpar">Left parathenses</param>
    /// <param name="rpar">Right parathenses</param>
    private string Increment(string s, string lpar, string rpar)
    {
        int i = 1;
        s = s.Trim();
        if ((rpar == String.Empty) || s.EndsWithCSafe(rpar))
        {
            int leftpar = s.LastIndexOfCSafe(lpar);
            if (lpar == rpar)
            {
                leftpar = s.LastIndexOfCSafe(lpar, leftpar - 1);
            }

            if (leftpar >= 0)
            {
                i = ValidationHelper.GetSafeInteger(s.Substring(leftpar + lpar.Length, s.Length - leftpar - lpar.Length - rpar.Length), 0);
                if (i > 0) // Remove parathenses only if parathenses found
                {
                    s = s.Remove(leftpar);
                }
                i++;
            }
        }

        s += lpar + i + rpar;
        return s;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            case "edit":
                URLHelper.Redirect("Newsletter_Frameset.aspx?newsletterid=" + Convert.ToString(actionArgument));
                break;

            case "delete":
                if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.newsletter", "configure"))
                {
                    RedirectToCMSDeskAccessDenied("cms.newsletter", "configure");
                }
                // delete Newsletter object from database
                NewsletterInfoProvider.DeleteNewsletterInfo(ValidationHelper.GetInteger(actionArgument, 0));
                break;
        }
    }

    #endregion
}