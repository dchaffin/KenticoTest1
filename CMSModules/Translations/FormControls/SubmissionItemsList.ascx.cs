using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Text;

using CMS.SettingsProvider;
using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.TranslationServices;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.IO;

public partial class CMSModules_Translations_FormControls_SubmissionItemsList : FormEngineUserControl
{
    #region "Variables"

    private int mSubmissionID = 0;

    #endregion


    #region "Properties"

    /// <summary>
    /// ID of the submission from which to display the items.
    /// </summary>
    public override object Value
    {
        get
        {
            return SubmissionID;
        }
        set
        {
            SubmissionID = ValidationHelper.GetInteger(value, 0);
        }
    }


    /// <summary>
    /// ID of the submission from which to display the items.
    /// </summary>
    public int SubmissionID
    {
        get
        {
            return mSubmissionID;
        }
        set
        {
            mSubmissionID = value;
            gridElem.WhereCondition = "SubmissionItemSubmissionID = " + value;
        }
    }


    /// <summary>
    /// Inner grid.
    /// </summary>
    public UniGrid Grid
    {
        get
        {
            return this.gridElem;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            this.gridElem.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            gridElem.IsLiveSite = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        this.gridElem.WhereCondition = "SubmissionItemSubmissionID = " + SubmissionID;
        this.gridElem.OnExternalDataBound += new OnExternalDataBoundEventHandler(gridElem_OnExternalDataBound);

        this.btnImportFromZip.OnClientClick = "ShowUploadDialog(" + SubmissionID + ", 0);";
        this.btnExportToZip.OnClientClick = "window.open('" + CMSContext.ResolveDialogUrl("~/CMSModules/Translations/CMSPages/DownloadTranslation.aspx?submissionid=" + SubmissionID) + "'); return false;";


        string script = "function ShowUploadDialog(submissionId, submissionItemTd) { modalDialog('" + CMSContext.ResolveDialogUrl("~/CMSModules/Translations/Pages/UploadTranslation.aspx") + "?itemid=' + submissionItemTd + '&submissionid=' + submissionId, 'Upload translation', 500, 180); }";
        ScriptHelper.RegisterClientScriptBlock(this.Page, typeof(string), "ShowUploadDialog", script, true);
    }


    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        if (sourceName.ToLowerCSafe() == "downloadxliff")
        {
            CMSImageButton btn = (CMSImageButton)sender;
            btn.OnClientClick = "window.open('" + CMSContext.ResolveDialogUrl("~/CMSModules/Translations/CMSPages/DownloadTranslation.aspx?itemid=" + ((DataRowView)((GridViewRow)parameter).DataItem)[0]) + "'); return false;"; ;
            return btn;
        }
        return parameter;
    }

    #endregion
}