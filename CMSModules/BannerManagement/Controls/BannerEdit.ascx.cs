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

using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.CMSHelper;
using CMS.DataEngine;

public partial class CMSModules_BannerManagement_Controls_BannerEdit : CMSAdminEditControl
{
    #region "Properties"

    private BannerInfo TypedEditedObject
    {
        get
        {
            return (BannerInfo)CMSContext.EditedObject;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        EditForm.OnBeforeValidate += new EventHandler(EditForm_OnBeforeValidate);
        EditForm.OnAfterSave += new EventHandler(EditForm_OnAfterSave);
        EditForm.OnCheckPermissions += new EventHandler(EditForm_OnCheckPermissions);

        int bannerType;
        if (int.TryParse(bannerTypeSelector.DropDownList.SelectedValue, out bannerType))
        {
            contentEdit.BannerType = (BannerTypeEnum)bannerType;
        }
        else
        {
            contentEdit.BannerType = BannerTypeEnum.Plain;
        }
        contentEdit.BannerID = TypedEditedObject != null ? TypedEditedObject.BannerID : 0;

        bannerTypeSelector.DropDownList.AutoPostBack = true;
        bannerTypeSelector.DropDownList.SelectedIndexChanged += new EventHandler(DropDownList_SelectedIndexChanged);
    }


    private void EditForm_OnAfterSave(object sender, EventArgs e)
    {
        ScriptHelper.RefreshTabHeader(Page, GetString("general.general"));
    }


    private void DropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        int newBannerType;
        if (int.TryParse(bannerTypeSelector.DropDownList.SelectedValue, out newBannerType))
        {
            contentEdit.BannerType = (BannerTypeEnum)newBannerType;
        }
    }


    private void EditForm_OnBeforeValidate(object sender, EventArgs e)
    {
        BannerCategoryInfo catInfo = (BannerCategoryInfo)CMSContext.EditedObjectParent;

        EditForm.Data["BannerCategoryID"] = catInfo.BannerCategoryID;
        EditForm.Data["BannerSiteID"] = catInfo.BannerCategorySiteID;
    }


    private void EditForm_OnCheckPermissions(object sender, EventArgs e)
    {
        BannerCategoryInfo catInfo = (BannerCategoryInfo)CMSContext.EditedObjectParent;

        ((CMSBannerManagementPage)Page).CheckModifyPermission(catInfo.BannerCategorySiteID);
    }
    
    #endregion
}
