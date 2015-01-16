using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.Reporting;
using CMS.UIControls;

public partial class CMSModules_Reporting_Tools_ItemsList : CMSUserControl
{
    #region "Variables"

    private string mEditUrl = "";
    private int mReportID = 0;
    private ReportInfo mReport = null;
    private ReportItemType mItemType = ReportItemType.Graph;

    #endregion


    #region "Public properties"

    /// <summary>
    /// URL of the edit page.
    /// </summary>
    public string EditUrl
    {
        get
        {
            return mEditUrl;
        }
        set
        {
            mEditUrl = value;
        }
    }


    /// <summary>
    /// Gets or sets report id, which is used for loading the report if Report property is null.
    /// </summary>
    public int ReportID
    {
        get
        {
            return mReportID;
        }
        set
        {
            mReportID = value;
        }
    }


    /// <summary>
    /// Gets or sets the report object to load the items.
    /// </summary>
    public ReportInfo Report
    {
        get
        {
            return mReport;
        }
        set
        {
            mReport = value;
        }
    }


    /// <summary>
    /// Item type.
    /// </summary>
    public ReportItemType ItemType
    {
        get
        {
            return mItemType;
        }
        set
        {
            mItemType = value;
        }
    }

    #endregion


    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check 'Read' permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.reporting", "Read"))
        {
            CMSReportingPage.RedirectToCMSDeskAccessDenied("cms.reporting", "Read");
        }

        if (Report != null)
        {
            brsItems.ReportID = Report.ReportID;
        }

        brsItems.ReportType = ItemType;
        brsItems.Display = false;
        brsItems.IsLiveSite = IsLiveSite;

        btnPreview.ImageUrl = UIHelper.GetImageUrl(Page, "CMSModules/CMS_Reporting/previewitem.png");
        btnClone.ImageUrl = UIHelper.GetImageUrl(Page, "CMSModules/CMS_Reporting/clone.png");
        btnEdit.ImageUrl = UIHelper.GetImageUrl(Page, "CMSModules/CMS_Reporting/edit.png");
        btnDelete.ImageUrl = UIHelper.GetImageUrl(Page, "CMSModules/CMS_Reporting/delete.png");

        btnPreview.AlternateText = GetString("general.preview");
        btnPreview.ToolTip = GetString("general.preview");
        btnClone.AlternateText = GetString("general.clone");
        btnClone.ToolTip = GetString("general.clone");
        btnEdit.AlternateText = GetString("general.edit");
        btnEdit.ToolTip = GetString("general.edit");
        btnDelete.AlternateText = GetString("general.delete");
        btnDelete.ToolTip = GetString("general.delete");

        // Enable page methods to get item name
        ScriptManager man = ScriptManager.GetCurrent(this.Page);
        man.EnablePageMethods = true;
    }


    /// <summary>
    /// Pre render.
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        Initialize();
    }


    /// <summary>
    /// Sets buttons actions.
    /// </summary>
    protected void Initialize()
    {
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "GetSelectedItem" + ItemType.ToString(), ScriptHelper.GetScript(@"
            function getSelectedItem_" + ItemType.ToString() + "() { \n" +
            "   if (document.getElementById('" + brsItems.UniSelectorClientID + "') != null) { \n" +
            "      return document.getElementById('" + brsItems.UniSelectorClientID + "').value; " +
            "   } \n" +
            "   return 0; } \n\n" +
            
            "function DeleteItem_" + ItemType.ToString() + "() { \n" +
            "   if ((document.getElementById('" + brsItems.UniSelectorClientID + "') != null) && (document.getElementById('" + brsItems.UniSelectorClientID + "').value != '0')) { \n" +
            "       if (confirm(" + ScriptHelper.GetString(ResHelper.GetString("general.confirmdelete")) + ")) { \n" +
            "           document.getElementById('" + hdnItemId.ClientID + "').value = getSelectedItem_" + ItemType.ToString() + "();  " + Page.ClientScript.GetPostBackEventReference(btnHdnDelete, null) + " } \n" +
            "   } else { alert(" + ScriptHelper.GetString(ResHelper.GetString("Reporting_General.SelectObjectFirst")) + "); } \n" +
            "} \n\n" +

            "function InserMacro_" + ItemType.ToString() + "() { \n" +
            "   if ((document.getElementById('" + brsItems.UniSelectorClientID + "') != null) && (document.getElementById('" + brsItems.UniSelectorClientID + "').value != '0')) { \n" +
            "       PageMethods.GetReportItemName('" + ReportInfoProvider.ReportItemTypeToString(ItemType) + "',getSelectedItem_" + ItemType.ToString() + "(),OnComplete_" + ItemType.ToString() + ");\n" +
            "   } else { alert(" + ScriptHelper.GetString(ResHelper.GetString("Reporting_General.SelectObjectFirst")) + "); } \n" +
            @"} 

             function OnComplete_" + ItemType.ToString() + @"(result, response, context){
                InsertHTML('%%control:Report" + ItemType.ToString() + "?" + Report.ReportName + @".' + result +'%%');
             }  

             function CloneItem_" + ItemType.ToString() + @"(id){
               if ((document.getElementById('" + brsItems.UniSelectorClientID + @"') != null) && (document.getElementById('" + brsItems.UniSelectorClientID + @"').value != '0')) { 
                modalDialog('" + URLHelper.ResolveUrl("~/CMSModules/Objects/Dialogs/CloneObjectDialog.aspx?objectType=" + GetObjectType() + @"&objectId='+id") + @", 'CloneObject', 750, 400);
              } else { alert(" + ScriptHelper.GetString(ResHelper.GetString("Reporting_General.SelectObjectFirst")) + @"); } 
             }"));


        string modalHeight = "760";
        string modalWidth = "1050";

        if (Report != null)
        {
            btnAdd.OnClientClick = "modalDialog('" + ResolveUrl(mEditUrl) + "?reportId=" + Report.ReportID + "','ReportItemEdit'," + modalWidth + "," + modalHeight + ");return false;";
            btnEdit.OnClientClick = "if (getSelectedItem_" + ItemType.ToString() + "() != '0') { modalDialog('" + ResolveUrl(mEditUrl) + "?reportId=" + Report.ReportID + "&itemid='+ getSelectedItem_" + ItemType.ToString() + "(),'ReportItemEdit'," + modalWidth + "," + modalHeight + "); } else { alert(" + ScriptHelper.GetString(ResHelper.GetString("Reporting_General.SelectObjectFirst")) + ");} return false;";
            btnDelete.OnClientClick = "DeleteItem_" + ItemType.ToString() + "(); return false;";
            btnInsert.OnClientClick = "InserMacro_" + mItemType.ToString() + "(); return false;";
            btnClone.OnClientClick = "CloneItem_" + ItemType.ToString() + @"(getSelectedItem_" + ItemType.ToString() + "()); return false;";

            btnPreview.OnClientClick = "if (getSelectedItem_" + ItemType.ToString() + "() != '0') { modalDialog('" + ResolveUrl(mEditUrl) + "?preview=true&reportId=" + Report.ReportID + "&itemid='+ getSelectedItem_" + ItemType.ToString() + "(),'ReportItemEdit'," + modalWidth + "," + modalHeight + "); } else { alert(" + ScriptHelper.GetString(ResHelper.GetString("Reporting_General.SelectObjectFirst")) + ");} return false;";
        }
    }


    /// <summary>
    /// Returns current object type
    /// </summary>
    protected string GetObjectType()
    {
        // Switch by item type
        switch (ItemType)
        {
            // Graph
            case ReportItemType.Graph:
            case ReportItemType.HtmlGraph:
                return ReportGraphInfo.TYPEINFO.ObjectType;

            // Table
            case ReportItemType.Table:
                return ReportTableInfo.TYPEINFO.ObjectType;

            // Value
            case ReportItemType.Value:
                return ReportValueInfo.TYPEINFO.ObjectType;
        }

        return String.Empty;
    }


    protected void btnHdnDelete_Click(object sender, EventArgs e)
    {
        // Check 'Modify' permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.reporting", "Modify"))
        {
            CMSReportingPage.RedirectToCMSDeskAccessDenied("cms.reporting", "Modify");
        }

        // Check whether object is defined
        if (!String.IsNullOrEmpty(hdnItemId.Value))
        {
            // Get id
            int id = ValidationHelper.GetInteger(hdnItemId.Value, 0);

            // Switch by type
            switch (ItemType)
            {
                // Graph
                case ReportItemType.Graph:
                case ReportItemType.HtmlGraph:
                    ReportGraphInfoProvider.DeleteReportGraphInfo(id);
                    break;

                // Table
                case ReportItemType.Table:
                    ReportTableInfoProvider.DeleteReportTableInfo(id);
                    break;

                // Value
                case ReportItemType.Value:
                    ReportValueInfoProvider.DeleteReportValueInfo(id);
                    break;
            }
        }
    }

}