using System;
using System.Data;
using System.Web.UI;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.Reporting;
using CMS.SettingsProvider;
using CMS.Synchronization;
using CMS.UIControls;

public partial class CMSModules_Reporting_Tools_ReportValue_Edit : CMSReportingModalPage
{
    #region "Variables"

    protected ReportValueInfo valueInfo = null;
    protected int valueId;
    protected ReportInfo reportInfo = null;
    private bool newValue = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    ///  Tab state (edit, preview, version)
    /// </summary>
    private int SelectedTab
    {
        get
        {
            return ValidationHelper.GetInteger(hdnSelectedTab.Value, 0);
        }
        set
        {
            hdnSelectedTab.Value = value.ToString();
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        ucSelectString.Scope = PredefinedObjectType.REPORT;
        ConnectionStringRow.Visible = CMSContext.CurrentUser.IsAuthorizedPerResource("cms.reporting", "SetConnectionString");
        txtQuery.FullScreenParentElementID = pnlWebPartForm_Properties.ClientID;

        // Test permission for query
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Reporting", "EditSQLQueries"))
        {
            txtQuery.Enabled = false;
        }
        else
        {
            txtQuery.Enabled = true;
        }

        versionList.OnAfterRollback += new EventHandler(versionList_onAfterRollback);

        string[,] tabs = new string[4, 4];
        tabs[0, 0] = GetString("general.general");
        tabs[0, 1] = "SetHelpTopic('helpTopic', 'report_value_properties')";
        tabs[1, 0] = GetString("general.preview");
        tabs[0, 1] = "SetHelpTopic('helpTopic', 'report_value_properties')";

        tabControlElem.Tabs = tabs;
        tabControlElem.UsePostback = true;
        CurrentMaster.Title.HelpName = "helpTopic";
        CurrentMaster.Title.HelpTopicName = "report_value_properties";
        CurrentMaster.Title.TitleCssClass = "PageTitleHeader";

        // Own javascript tab change handling -> because tab control raises changetab after prerender - too late
        // Own selected tab change handling
        RegisterTabScript(hdnSelectedTab.ClientID, tabControlElem);

        RegisterResizeAndRollbackScript(divFooter.ClientID, divScrolable.ClientID);

        rfvCodeName.ErrorMessage = GetString("general.requirescodename");
        rfvDisplayName.ErrorMessage = GetString("general.requiresdisplayname");

        int reportId = QueryHelper.GetInteger("reportid", 0);
        bool preview = QueryHelper.GetBoolean("preview", false);
        if (reportId > 0)
        {
            reportInfo = ReportInfoProvider.GetReportInfo(reportId);
        }

        if (reportInfo != null) //must be valid reportid parameter
        {
            ucSelectString.DefaultConnectionString = reportInfo.ReportConnectionString;

            // If preview by URL -> select preview tab
            bool isPreview = QueryHelper.GetBoolean("preview", false);
            if (isPreview && !RequestHelper.IsPostBack())
            {
                SelectedTab = 1;
            }

            if (PersistentEditedObject == null)
            {
                int id = QueryHelper.GetInteger("itemid", 0);
                if (id > 0)
                {
                    PersistentEditedObject = ReportValueInfoProvider.GetReportValueInfo(id);
                    valueInfo = PersistentEditedObject as ReportValueInfo;
                }
            }
            else
            {
                valueInfo = PersistentEditedObject as ReportValueInfo;
            }

            if (valueInfo != null)
            {
                CurrentMaster.Title.TitleText = GetString("Reporting_ReportValue_Edit.TitleText");
                CurrentMaster.Title.TitleImage = GetImageUrl("Objects/Reporting_ReportValue/object_light.png");

                valueId = valueInfo.ValueID;

                if (ObjectVersionManager.DisplayVersionsTab(valueInfo))
                {
                    tabs[2, 0] = GetString("objectversioning.tabtitle");
                    tabs[2, 1] = "SetHelpTopic('helpTopic', 'objectversioning_general');";
                    versionList.Object = valueInfo;
                    versionList.IsLiveSite = false;
                }
            }
            else //new item
            {
                CurrentMaster.Title.TitleText = GetString("Reporting_ReportValue_Edit.NewItemTitleText");
                CurrentMaster.Title.TitleImage = GetImageUrl("Objects/Reporting_ReportValue/new_light.png");

                chkSubscription.Checked = true;
                newValue = true;

                if (!RequestHelper.IsPostBack())
                {
                    ucSelectString.Value = String.Empty;
                }
            }

            // set help key
            CurrentMaster.Title.HelpTopicName = "report_value_properties";

            if (!RequestHelper.IsPostBack())
            {
                LoadData();
            }
        }
        else
        {
            btnOk.Visible = false;
            ShowError(GetString("Reporting_ReportValue_Edit.InvalidReportId"));
        }
        btnOk.Text = GetString("General.OK");
        btnCancel.Text = GetString("General.Cancel");
        btnApply.Text = GetString("General.Apply");

        if (preview && !RequestHelper.IsPostBack())
        {
            tabControlElem.SelectedTab = 1;
            ShowPreview();
        }
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        // Set correct help topic
        Control pbCtrl = ControlsHelper.GetPostBackControl(this);
        if ((tabControlElem.SelectedTab == 2) && ((pbCtrl != null) && !pbCtrl.ClientID.StartsWithCSafe(tabControlElem.ClientID)))
        {
            ScriptHelper.RegisterStartupScript(this, typeof(string), "SetTabHelpTopic", ScriptHelper.GetScript("SetHelpTopic('helpTopic', 'objectversioning_general');"));
            HideActionButtons();
        }
        else
        {
            ShowActionButtons();
        }

        divScrolable.CssClass = (tabControlElem.SelectedTab == 0) ? "ScrollableContent" : "";


        base.OnPreRender(e);
    }


    /// <summary>
    /// Load data from db.
    /// </summary>
    protected void LoadData()
    {
        if (valueInfo != null)
        {
            txtDisplayName.Text = valueInfo.ValueDisplayName;
            txtCodeName.Text = valueInfo.ValueName;
            txtQuery.Text = valueInfo.ValueQuery;
            chkIsProcedure.Checked = valueInfo.ValueQueryIsStoredProcedure;
            txtFormatString.Text = valueInfo.ValueFormatString;
            chkSubscription.Checked = ValidationHelper.GetBoolean(valueInfo.ValueSettings["SubscriptionEnabled"], true);
            ucSelectString.Value = valueInfo.ValueConnectionString;
        }
    }


    /// <summary>
    /// Saves the data and close the editor.
    /// </summary>
    /// <param name="sender">Button save</param>
    /// <param name="e">Params</param>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        if (Save(true))
        {
            ltlScript.Text += ScriptHelper.GetScript("RefreshContent();CloseDialog();");
        }
        else
        {
            tabControlElem.SelectedTab = 0;
            pnlPreview.Visible = false;
            divPanelHolder.Visible = true;
            categoryList.Visible = true;
        }
    }


    /// <summary>
    /// Apply data changes.
    /// </summary>
    /// <param name="sender">Button apply</param>
    /// <param name="e">Params</param>
    protected void btnApply_Click(object sender, EventArgs e)
    {
        if (!Save(true))
        {
            tabControlElem.SelectedTab = 0;
            pnlPreview.Visible = false;
            divPanelHolder.Visible = true;
            categoryList.Visible = true;
        }
        else
            // Redirect for new reports
            if (newValue)
            {
                URLHelper.Redirect(ResolveUrl("~/CMSModules/Reporting/Tools/ReportValue_Edit.aspx?reportId=" + valueInfo.ValueReportID + "&itemId=" + valueInfo.ValueID));
            }
    }


    /// <summary>
    /// Save data.
    /// </summary>
    /// <returns>return true if save was successful</returns>
    protected bool Save(bool save)
    {
        string errorMessage = String.Empty;
        if (save)
        {
            // Check 'Modify' permission
            if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.reporting", "Modify"))
            {
                RedirectToAccessDenied("cms.reporting", "Modify");
            }

            errorMessage = new Validator()
                .NotEmpty(txtDisplayName.Text, rfvDisplayName.ErrorMessage)
                .NotEmpty(txtCodeName.Text, rfvCodeName.ErrorMessage)
                .NotEmpty(txtQuery.Text, GetString("Reporting_ReportGraph_Edit.ErrorQuery")).Result;

            if ((errorMessage == "") && (!ValidationHelper.IsIdentifier(txtCodeName.Text.Trim())))
            {
                errorMessage = GetString("general.erroridentifierformat");
            }

            string fullName = reportInfo.ReportName + "." + txtCodeName.Text.Trim();
            ReportValueInfo codeNameCheck = ReportValueInfoProvider.GetReportValueInfo(fullName);

            if ((errorMessage == "") && (codeNameCheck != null) && (codeNameCheck.ValueID != valueId))
            {
                errorMessage = GetString("Reporting_ReportValue_Edit.ErrorCodeNameExist");
            }
        }

        //test query in all cases
        if (!save)
        {
            errorMessage = new Validator().NotEmpty(txtQuery.Text, GetString("Reporting_ReportGraph_Edit.ErrorQuery")).Result;
        }

        if ((errorMessage == ""))
        {
            //new Value
            if (valueInfo == null)
            {
                valueInfo = new ReportValueInfo();
            }

            valueInfo.ValueDisplayName = txtDisplayName.Text.Trim();
            valueInfo.ValueName = txtCodeName.Text.Trim();

            if (CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Reporting", "EditSQLQueries"))
            {
                valueInfo.ValueQuery = txtQuery.Text.Trim();
            }

            valueInfo.ValueQueryIsStoredProcedure = chkIsProcedure.Checked;
            valueInfo.ValueFormatString = txtFormatString.Text.Trim();
            valueInfo.ValueReportID = reportInfo.ReportID;
            valueInfo.ValueSettings["SubscriptionEnabled"] = chkSubscription.Checked.ToString();
            valueInfo.ValueConnectionString = ValidationHelper.GetString(ucSelectString.Value, String.Empty);

            if (save)
            {
                ReportValueInfoProvider.SetReportValueInfo(valueInfo);
            }
        }
        else
        {
            ShowError(errorMessage);
            return false;
        }
        return true;
    }


    /// <summary>
    /// Show preview.
    /// </summary>
    private void ShowPreview()
    {
        if (reportInfo != null)
        {
            pnlPreview.Visible = true;
            divPanelHolder.Visible = false;
            categoryList.Visible = false;
            pnlVersions.Visible = false;

            FormInfo fi = new FormInfo(reportInfo.ReportParameters);
            // Get datarow with required columns
            DataRow dr = fi.GetDataRow();

            fi.LoadDefaultValues(dr, true);

            //reportGraph.ContextParameters 
            ctrlReportValue.ReportParameters = dr;

            ctrlReportValue.Visible = true;
            ctrlReportValue.ValueInfo = valueInfo;

            ctrlReportValue.ReloadData(true);
        }
    }


    /// <summary>
    /// Hide buttons that are connected with reporting object.
    /// </summary>
    private void HideActionButtons()
    {
        btnApply.Visible = false;
        btnOk.Visible = false;
        btnCancel.Text = GetString("general.close");
    }


    /// <summary>
    /// Show buttons that are connected with reporting object.
    /// </summary>
    private void ShowActionButtons()
    {
        btnApply.Visible = true;
        btnOk.Visible = true;
        btnCancel.Text = GetString("general.cancel");
    }


    /// <summary>
    /// Tab change.
    /// </summary>
    /// <param name="sender">Tab control</param>
    /// <param name="ea">Event params</param>
    protected void tabControlElem_clicked(object sender, EventArgs ea)
    {
        ShowActionButtons();
        divScrolable.CssClass = "";

        if (tabControlElem.SelectedTab == 2)
        {
            divPanelHolder.Visible = false;
            categoryList.Visible = false;
            pnlPreview.Visible = false;
            if (valueInfo != null)
            {
                pnlVersions.Visible = true;
                ScriptHelper.RegisterStartupScript(this, typeof(string), "SetTabHelpTopic", ScriptHelper.GetScript("SetHelpTopic('helpTopic', 'objectversioning_general');"));
            }
            HideActionButtons();
        }
        else if (tabControlElem.SelectedTab == 1)
        {
            //createGraphImage
            if (Save(false))
            {
                ShowPreview();
            }
            else
            {
                tabControlElem.SelectedTab = 0;
                divScrolable.CssClass = "ScrollableContent";
            }
        }
        else
        {
            divScrolable.CssClass = "ScrollableContent";
            pnlPreview.Visible = false;
            pnlVersions.Visible = false;
            divPanelHolder.Visible = true;
            categoryList.Visible = true;
        }
    }


    /// <summary>
    /// Get info from PersistentEditedObject and reload data
    /// </summary>
    private void ReloadDataAfrterRollback()
    {
        // Load rollbacked info
        GeneralizedInfo gi = PersistentEditedObject as GeneralizedInfo;
        if (gi != null)
        {
            valueInfo = gi.MainObject as ReportValueInfo;
        }
        LoadData();
    }


    protected void versionList_onAfterRollback(object sender, EventArgs e)
    {
        ReloadDataAfrterRollback();
    }
}