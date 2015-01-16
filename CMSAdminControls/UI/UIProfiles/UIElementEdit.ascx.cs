using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSAdminControls_UI_UIProfiles_UIElementEdit : CMSUserControl
{
    #region "Variables"

    private int mElementID = 0;
    private int mResourceID = 0;
    private int mParentID = 0;
    private UIElementInfo elemInfo = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Current element id.
    /// </summary>
    public int ElementID
    {
        get
        {
            return mElementID;
        }
        set
        {
            mElementID = value;
        }
    }


    /// <summary>
    /// Current resource id.
    /// </summary>
    public int ResourceID
    {
        get
        {
            return mResourceID;
        }
        set
        {
            mResourceID = value;
        }
    }


    /// <summary>
    /// Current parent id.
    /// </summary>
    public int ParentID
    {
        get
        {
            return mParentID;
        }
        set
        {
            mParentID = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        plcCMSVersion.Visible = SettingsKeyProvider.DevelopmentMode;

        if (ElementID > 0)
        {
            elemInfo = UIElementInfoProvider.GetUIElementInfo(ElementID);
            if ((!RequestHelper.IsPostBack()) && (elemInfo != null))
            {
                // Load selected element
                txtDisplayName.Text = elemInfo.ElementDisplayName;
                txtName.Text = elemInfo.ElementName;
                chkCustom.Checked = elemInfo.ElementIsCustom;
                txtCaption.Text = elemInfo.ElementCaption;
                txtTargetURL.Text = elemInfo.ElementTargetURL;
                txtIconPath.Text = elemInfo.ElementIconPath;
                txtDescription.Text = elemInfo.ElementDescription;
                radRegular.Checked = (elemInfo.ElementSize == UIElementSizeEnum.Regular);
                radLarge.Checked = (elemInfo.ElementSize == UIElementSizeEnum.Large);
                elemSelector.ModuleID = elemInfo.ElementResourceID;
                // Exclude current element and children from dropdown list
                elemSelector.WhereCondition = "ElementIDPath NOT LIKE N'" + elemInfo.ElementIDPath + "%'";

                if (plcCMSVersion.Visible)
                {
                    versionSelector.Value = elemInfo.ElementFromVersion;
                }
            }
        }
        else
        {
            // Hide parent selector for creating new element
            plcParentElem.Visible = false;
            elemSelector.StopProcessing = true;

            // Set Is custom to default value
            if (!URLHelper.IsPostback())
            {
                chkCustom.Checked = (SettingsKeyProvider.DevelopmentMode ? false : true);
            }
        }
        rfvDisplayName.ErrorMessage = GetString("general.requiresdisplayname");
        rfvName.ErrorMessage = GetString("general.requirescodename");

        if (!URLHelper.IsPostback())
        {
            if (QueryHelper.GetBoolean("saved", false))
            {
                // Show message
                ShowChangesSaved();
            }
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if ((!RequestHelper.IsPostBack()) && (elemInfo != null))
        {
            elemSelector.ElementID = elemInfo.ElementParentID;
        }
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        string result = ValidateForm();
        if (String.IsNullOrEmpty(result))
        {
            if (ElementID == 0)
            {
                // Create new UI element info
                elemInfo = new UIElementInfo();
                elemInfo.ElementResourceID = ResourceID;
                elemInfo.ElementParentID = ParentID;
                elemInfo.ElementFromVersion = string.Empty;
                elemInfo.ElementOrder = UIElementInfoProvider.GetLastElementOrder(ParentID) + 1;
            }
            else
            {
                // If parent changed set last order
                if (elemSelector.ElementID != elemInfo.ElementParentID)
                {
                    elemInfo.ElementOrder = UIElementInfoProvider.GetLastElementOrder(elemSelector.ElementID) + 1;
                }
                elemInfo.ElementParentID = elemSelector.ElementID;
            }
            elemInfo.ElementDisplayName = txtDisplayName.Text.Trim();
            elemInfo.ElementName = txtName.Text.Trim();
            elemInfo.ElementIsCustom = chkCustom.Checked;
            elemInfo.ElementCaption = txtCaption.Text.Trim();
            elemInfo.ElementDisplayName = txtDisplayName.Text.Trim();
            elemInfo.ElementTargetURL = txtTargetURL.Text.Trim();
            elemInfo.ElementIconPath = txtIconPath.Text.Trim();
            elemInfo.ElementDescription = txtDescription.Text.Trim();
            elemInfo.ElementSize = (radRegular.Checked ? UIElementSizeEnum.Regular : UIElementSizeEnum.Large);
            if (plcCMSVersion.Visible)
            {
                elemInfo.ElementFromVersion = versionSelector.Value.ToString();
            }

            // Set UI element info
            UIElementInfoProvider.SetUIElementInfo(elemInfo);

            // Get updated element info (ElementIDPath was changed in DB)
            elemInfo = UIElementInfoProvider.GetUIElementInfo(elemInfo.ElementID);

            // Reload header and content after save
            StringBuilder sb = new StringBuilder();
            sb.Append("if (window.parent != null) {");
            if (ElementID == 0)
            {
                sb.Append("if (window.parent.parent.frames['uicontent'] != null) {");
                sb.Append("window.parent.parent.frames['uicontent'].location = '" + ResolveUrl("~/CMSModules/Modules/Pages/Development/Module_UI_EditFrameset.aspx") + "?moduleID=" + ResourceID + "&elementId=" + elemInfo.ElementID + "&parentId=" + elemInfo.ElementParentID + "&saved=1';");
                sb.Append("}");
                sb.Append("if (window.parent.frames['uicontent'] != null) {");
                sb.Append("window.parent.frames['uicontent'].location = '" + ResolveUrl("~/CMSModules/Modules/Pages/Development/Module_UI_EditFrameset.aspx") + "?moduleID=" + ResourceID + "&elementId=" + elemInfo.ElementID + "&parentId=" + elemInfo.ElementParentID + "&saved=1';");
                sb.Append("}");
            }
            else
            {
                sb.Append("if (window.parent.parent.frames['header'] != null) {");
                sb.Append("window.parent.parent.frames['header'].location = '" + ResolveUrl("~/CMSModules/Modules/Pages/Development/Module_UI_Header.aspx") + "?moduleID=" + ResourceID + "&elementId=" + elemInfo.ElementID + "&parentId=" + elemInfo.ElementParentID + "';");
                sb.Append("}");
                sb.Append("if (window.parent.frames['header'] != null) {");
                sb.Append("window.parent.frames['header'].location = '" + ResolveUrl("~/CMSModules/Modules/Pages/Development/Module_UI_Header.aspx") + "?moduleID=" + ResourceID + "&elementId=" + elemInfo.ElementID + "&parentId=" + elemInfo.ElementParentID + "';");
                sb.Append("}");
            }
            sb.Append("if (window.parent.parent.frames['tree'] != null) {");
            sb.Append("window.parent.parent.frames['tree'].location = '" + ResolveUrl("~/CMSModules/Modules/Pages/Development/Module_UI_Tree.aspx") + "?moduleID=" + ResourceID + "&path=" + elemInfo.ElementIDPath + "&elementId=" + elemInfo.ElementID + "&parentId=" + elemInfo.ElementParentID + "';");
            sb.Append("}");
            sb.Append("if (window.parent.frames['tree'] != null) {");
            sb.Append("window.parent.frames['tree'].location = '" + ResolveUrl("~/CMSModules/Modules/Pages/Development/Module_UI_Tree.aspx") + "?moduleID=" + ResourceID + "&path=" + elemInfo.ElementIDPath + "&elementId=" + elemInfo.ElementID + "&parentId=" + elemInfo.ElementParentID + "';");
            sb.Append("}");
            sb.Append("}");

            // Show message
            ShowChangesSaved();

            ltlScript.Text = ScriptHelper.GetScript(sb.ToString());
        }
        else
        {
            // Show error message
            ShowError(result);
        }
    }

    #endregion


    #region "Private methods"

    private string ValidateForm()
    {
        // Finds whether required fields are not empty or codename is in requested form
        string result = new Validator()
            .NotEmpty(txtDisplayName.Text.Trim(), GetString("general.requiresdisplayname"))
            .NotEmpty(txtName.Text.Trim(), GetString("general.requirescodename"))
            .IsCodeName(txtName.Text.Trim(), GetString("general.invalidcodename"))
            .Result;

        if (String.IsNullOrEmpty(result) && plcCMSVersion.Visible)
        {
            if (string.IsNullOrEmpty(versionSelector.Value.ToString()))
            {
                result = GetString("general.requirescmsversion");
            }
        }

        if (String.IsNullOrEmpty(result))
        {
            // Check if code name is unique
            UIElementInfo elemInfo = UIElementInfoProvider.GetUIElementInfo(ResourceID, txtName.Text.Trim());
            if ((elemInfo != null) && (elemInfo.ElementID != ElementID))
            {
                result = GetString("general.uniquecodenameerror");
            }
        }

        return result;
    }

    #endregion
}