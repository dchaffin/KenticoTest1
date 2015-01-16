using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_Widgets_Controls_WidgetGeneral : CMSAdminEditControl
{
    #region "Variables"

    private WidgetInfo mWidgetInfo = null;
    private int mWidgetWebpartId = 0;
    private int mWidgetCategoryId = 0;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the widget info object which is edited. If not loaded yet, ItemID is used for get it from DB.
    /// </summary>
    public WidgetInfo WidgetInfo
    {
        get
        {
            // If not loaded yet
            if ((mWidgetInfo == null) && (ItemID > 0))
            {
                mWidgetInfo = WidgetInfoProvider.GetWidgetInfo(ItemID);
            }

            return mWidgetInfo;
        }

        set
        {
            mWidgetInfo = value;
        }
    }


    /// <summary>
    /// Gets or sets the parent webpart(Id) of widget.
    /// </summary>
    public int WidgetWebpartId
    {
        get
        {
            return mWidgetWebpartId;
        }
        set
        {
            mWidgetWebpartId = value;
        }
    }


    /// <summary>
    /// Gets or sets the widget category of widget.
    /// </summary>
    public int WidgetCategoryId
    {
        get
        {
            return mWidgetCategoryId;
        }
        set
        {
            mWidgetCategoryId = value;
        }
    }
    
    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (WidgetInfo != null)
        {
            ucLayouts.WhereCondition = "[WebPartLayoutWebPartID]=" + WidgetInfo.WidgetWebPartID;

            if (!RequestHelper.IsPostBack())
            {
                txtCodeName.Text = WidgetInfo.WidgetName;
                txtDisplayName.Text = WidgetInfo.WidgetDisplayName;
                txtDescription.Text = WidgetInfo.WidgetDescription;
                chkSkipInsertProperties.Checked = WidgetInfo.WidgetSkipInsertProperties;

                WebPartInfo webpart = WebPartInfoProvider.GetWebPartInfo(WidgetInfo.WidgetWebPartID);
                if (webpart != null)
                {
                    txtBasedOn.Text = webpart.WebPartDisplayName;
                }

                categorySelector.Value = WidgetInfo.WidgetCategoryID;

                // Init file uploader
                lblThumbnail.Visible = true;
                thumbnailFile.Visible = true;
                thumbnailFile.ObjectID = ItemID;
                thumbnailFile.ObjectType = PortalObjectType.WIDGET;
                thumbnailFile.Category = MetaFileInfoProvider.OBJECT_CATEGORY_THUMBNAIL;

                // Init layout selector
                ucLayouts.Value = WidgetInfo.WidgetLayoutID;

                string[,] specialFields = new string[1, 2];
                specialFields[0, 0] = GetString("WebPartPropertise.Default");
                specialFields[0, 1] = "0";

                ucLayouts.SpecialFields = specialFields;
                ucLayouts.IsLiveSite = false;
            }

            // Validator texts
            rfvDisplayName.ErrorMessage = GetString("general.requiresdisplayname");
            rfvCodeName.ErrorMessage = GetString("general.requirescodename");
        }
        else
        {
            Visible = false;
        }
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        if (!CheckPermissions("cms.widgets", PERMISSION_MODIFY))
        {
            return;
        }

        // Create new widget info if new widget
        if (WidgetInfo == null)
        {
            // Parent webpart must be set
            if ((WidgetWebpartId == 0) || (WidgetCategoryId == 0))
            {
                return;
            }

            WidgetInfo = new WidgetInfo();
            WidgetInfo.WidgetWebPartID = WidgetWebpartId;
            WidgetInfo.WidgetCategoryID = WidgetCategoryId;
        }

        txtCodeName.Text = TextHelper.LimitLength(txtCodeName.Text.Trim(), 100, "");
        txtDisplayName.Text = TextHelper.LimitLength(txtDisplayName.Text.Trim(), 100, "");

        // Perform validation
        string errorMessage = new Validator().NotEmpty(txtCodeName.Text, rfvCodeName.ErrorMessage).IsCodeName(txtCodeName.Text, GetString("general.invalidcodename"))
            .NotEmpty(txtDisplayName.Text, rfvDisplayName.ErrorMessage).Result;

        if (errorMessage == "")
        {
            // If name changed, check if new name is unique
            if (CMSString.Compare(WidgetInfo.WidgetName, txtCodeName.Text, true) != 0)
            {
                WidgetInfo widget = WidgetInfoProvider.GetWidgetInfo(txtCodeName.Text);
                if (widget != null)
                {
                    ShowError(GetString("general.codenameexists"));
                    return;
                }
            }

            WidgetInfo.WidgetName = txtCodeName.Text;
            WidgetInfo.WidgetDisplayName = txtDisplayName.Text;
            WidgetInfo.WidgetDescription = txtDescription.Text;
            WidgetInfo.WidgetLayoutID = ValidationHelper.GetInteger(ucLayouts.Value, 0);
            WidgetInfo.WidgetCategoryID = ValidationHelper.GetInteger(categorySelector.Value, WidgetInfo.WidgetCategoryID);
            WidgetInfo.WidgetSkipInsertProperties = chkSkipInsertProperties.Checked;

            WidgetInfoProvider.SetWidgetInfo(WidgetInfo);

            ShowChangesSaved();

            // Raise save for frame reload
            RaiseOnSaved();
        }
        else
        {
            ShowError(errorMessage);
        }
    }
}