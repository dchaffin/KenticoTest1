using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_Content_CMSDesk_ContentEditFrameset : CMSContentPage
{
    #region "Variables"

    protected string tabspage = "Edit/edittabs.aspx" + URLHelper.Url.Query;
    protected string name = null;

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        // Do not redirect for non-existing document (new culture version is created)
        DocumentManager.RedirectForNonExistingDocument = false;

        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        DocumentManager.Tree.CombineWithDefaultCulture = false;
        if (Node != null)
        {
            // Register script files
            ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/Content/CMSDesk/ContentEditFrameset.js");

            // Document from different site
            if (Node.NodeSiteID != CMSContext.CurrentSiteID)
            {
                URLHelper.Redirect(GetPageNotAvailable(string.Empty, false, Node.DocumentName));
            }

            ViewModeEnum currentMode = UpdateViewMode(CMSContext.ViewMode);

            // Check the product mode
            if (currentMode == ViewModeEnum.Product)
            {
                // Document type has to be marked as a product
                DataClassInfo classObj = DataClassInfoProvider.GetDataClass(Node.NodeClassName);
                if ((classObj == null) || !classObj.ClassIsProduct)
                {
                    CMSContext.ViewMode = ViewModeEnum.Properties;
                }
            }

            // Get the name for automatic title
            name = (Node.NodeAliasPath == "/" ? CMSContext.CurrentSite.DisplayName : Node.GetDocumentName());
        }
        else
        {
            // Document does not exist -> redirect to new culture version creation dialog
            RedirectToNewCultureVersionPage();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        if (!String.IsNullOrEmpty(name))
        {
            ScriptHelper.RegisterTitleScript(this, ResHelper.LocalizeString(name));
        }
    }

    #endregion
}