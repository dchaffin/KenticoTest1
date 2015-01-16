using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using CMS.SiteProvider;
using CMS.UIControls;
using CMS.CMSHelper;
using CMS.GlobalHelper;


public partial class CMSModules_BannerManagement_Controls_CategoryEdit : CMSAdminControl
{
    #region "Properties"

    private BannerCategoryInfo TypedEditedObject
    {
        get
        {
            return (BannerCategoryInfo)CMSContext.EditedObject;
        }
    }


    /// <summary>
    /// Indicates whether the previous banner category was saved
    /// </summary>
    private bool WasSaved
    {
        get
        {
            if (QueryHelper.GetInteger("saved", 0) == 1) 
            {
                return true;
            }
            return false;
        }
    }


    /// <summary>
    /// ID of site of the currently processed tag group
    /// </summary>
    private int? SiteID
    {
        get
        {
            int query = QueryHelper.GetInteger("siteid", 0);

            if ((query == 0) || (query == -4))
            {
                return null;
            }

            return query;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        EditForm.OnBeforeValidate += new EventHandler(EditForm_OnBeforeValidate);
        EditForm.OnAfterSave += new EventHandler(EditForm_OnAfterSave);
        EditForm.OnCheckPermissions += new EventHandler(EditForm_OnCheckPermissions);

        if (WasSaved)
        {
            ((CMSPage)Page).ShowChangesSaved();
        }
    }
    
    #endregion


    #region "Private methods"

    private void EditForm_OnBeforeValidate(object sender, EventArgs e)
    {
        // Set SiteID if creating a new object
        if (TypedEditedObject == null)
        {
            EditForm.Data["BannerCategorySiteID"] = SiteID;
        }
    }


    private void EditForm_OnAfterSave(object sender, EventArgs e)
    {
        ScriptHelper.RefreshTabHeader(Page, GetString("general.general"));
    }


    private void EditForm_OnCheckPermissions(object sender, EventArgs e)
    {
        int? siteIDToCheck;

        // Editing an existing banner category
        if (TypedEditedObject != null)
        {
            siteIDToCheck = TypedEditedObject.BannerCategorySiteID;
        }
        // Creating a new one
        else
        {
            siteIDToCheck = SiteID;
        }
        ((CMSBannerManagementPage)Page).CheckModifyPermission(siteIDToCheck);
    }
    #endregion
}
