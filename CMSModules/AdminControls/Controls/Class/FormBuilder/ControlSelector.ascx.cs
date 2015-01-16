using System;
using System.Data;
using System.Text;
using System.Linq;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.CMSHelper;
using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.ExtendedControls.DragAndDrop;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_AdminControls_Controls_Class_FormBuilder_ControlSelector : CMSUserControl
{
    private StringBuilder imageHTML = new StringBuilder();
    private string defaultWebPartImageUrl = null;

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        ScriptHelper.RegisterJQueryCookie(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSScripts/WebPartToolbar.js");
        ScriptHelper.RegisterStartupScript(this, typeof(string), "WPTPageScripts", ScriptHelper.GetScript(GetPageScripts()));
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "WPTSetupToolbar", ScriptHelper.GetScript("wptSetupSearch();"));
        LoadControls();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        ltrCategoryImage.Text = String.Format("<img alt=\"\" class=\"WPTItemImg\" src=\"{0}\" />", GetImageUrl("Objects/CMS_FormControl/object.png"));

        // Set the web part id
        pnlCategoryItem.Attributes.Add("data-webpartid", "category");
        pnlCategoryItem.Attributes.Add("data-dragkeepcopy", "1");
        pnlCategoryItem.Attributes.Add("onmouseover", "wptToggle(this, true);");
        pnlCategoryItem.Attributes.Add("onmouseout", "wptToggle(this, false);");

        categoryDragDropExtender.TargetControlID = pnlCategoryDraggable.ID;
        categoryDragDropExtender.DropCueID = pnlCue.ID;
        categoryDragDropExtender.DragItemClass = "WPTItem";
        categoryDragDropExtender.DragItemHandleClass = "WPTHandle";

        repItems.StopProcessing = false;

        // Setup the toolbar web part list drag-and-drop extender
        ddExtender.TargetControlID = scrollPanel.ScrollAreaContainer.ID;
        ddExtender.DropCueID = pnlCue.ID;
        ddExtender.DragItemClass = "WPTSelectorEnvelope";
        ddExtender.DragItemHandleClass = "WPTHandle";
        ddExtender.OnClientDrop = "";
        ddExtender.OnClientBeforeDrop = "wptListOnBeforeDrop";

        // Setup the drag-and-drop extender for highlighted web parts as well.
        // This solves the issue when changing categories (using the drop down) and you highlight a web part before the init script for drag-and-drop is run.
        ddExtenderHovered.TargetControlID = scrollPanel.ScrollAreaContainer.ID;
        ddExtenderHovered.DropCueID = pnlCue.ID;
        ddExtenderHovered.DragItemClass = "WPTSelectorEnvelopeHover";
        ddExtenderHovered.DragItemHandleClass = "WPTHandle";
        ddExtenderHovered.OnClientDrop = "";
        ddExtenderHovered.OnClientBeforeDrop = "wptListOnBeforeDrop";

        imgTrash.ToolTip = ResHelper.GetString("formbuilder.trashtooltip");
        imgTrash.ImageUrl = GetImageUrl("CMSModules/CMS_Content/WebPartToolbar/garbage_empty.png");

        repItems.ItemDataBound += new RepeaterItemEventHandler(repItems_ItemDataBound);
        ddTypeSelector.Attributes.Add("onchange", "wptCategoryChanged();");

        // Hide the compulsory drag-and-drop panel Cue, it is not being used
        pnlCue.Style.Add("display", "none");
        pnlCue.Style.Add("position", "absolute");
    }


    private void LoadControls()
    {
        defaultWebPartImageUrl = GetImageUrl("Objects/CMS_FormControl/object.png");
        // Specific web part category
        int selectedCategoryId = ValidationHelper.GetInteger(ddTypeSelector.Value, (int)FormUserControlTypeEnum.Unspecified);

        // Get all web parts for the selected category and its subcategories
        if (selectedCategoryId != (int)FormUserControlTypeEnum.Unspecified)
        {
            repItems.WhereCondition = SqlHelperClass.AddWhereCondition(repItems.WhereCondition, "UserControlType = " + (int)((FormUserControlTypeEnum)selectedCategoryId));
        }
        repItems.ReloadData(true);
    }


    /// <summary>
    /// Handles the ItemDataBound event of the repItems control.
    /// </summary>
    protected void repItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            object dataItem = e.Item.DataItem;

            if (dataItem.GetType() == typeof(DataRowView))
            {
                // Get data row
                DataRow dr = ((DataRowView)dataItem).Row;

                // Setup the web part item
                Panel WPTFlatItem = ((Panel)e.Item.FindControl("i"));
                if (WPTFlatItem != null)
                {
                    WPTFlatItem.ToolTip = "<div class='WPTTH'>" + Convert.ToString(dr["UserControlDisplayName"]) + "</div>";
                    if (!String.IsNullOrEmpty(dr["UserControlDescription"].ToString()))
                    {
                        WPTFlatItem.ToolTip += "<div class='WPTTC'>" + dr["UserControlDescription"] + "</div>";
                    }

                    // Set the web part id
                    WPTFlatItem.Attributes.Add("data-webpartid", Convert.ToString(dr["UserControlID"]));

                    // Ensure that when start dragging then a copy of the original web part item will be created
                    WPTFlatItem.Attributes.Add("data-dragkeepcopy", "1");
                    WPTFlatItem.Attributes.Add("onmouseover", "wptToggle(this, true);");
                    WPTFlatItem.Attributes.Add("onmouseout", "wptToggle(this, false);");
                }

                // Build the web part image html
                Literal ltrImage = ((Literal)e.Item.FindControl("ltrImage"));
                if (ltrImage != null)
                {
                    imageHTML.Length = 0;
                    imageHTML.Append("<img alt=\"\" class=\"WPTItemImg\" src=\"");
                    string metaFileGUIDStr = dr["UserControlThumbnailGUID"].ToString();

                    // Default web part image
                    imageHTML.Append(defaultWebPartImageUrl);

                    if (!string.IsNullOrEmpty(metaFileGUIDStr))
                    {
                        // Custom web part image
                        imageHTML.Append("\" rev=\"" + ResolveUrl("~/CMSPages/GetMetaFile.aspx"), "?maxsidesize=64&fileguid=", metaFileGUIDStr);
                    }

                    if (e.Item.ItemIndex > 20)
                    {
                        ScriptHelper.RegisterImageLazyLoad(this.Page, "WPTPanel", "WPTItemImg");
                    }

                    imageHTML.Append("\" />");
                    ltrImage.Text = imageHTML.ToString();
                }
            }
        }
    }


    /// <summary>
    /// Gets the page javascripts.
    /// </summary>
    private string GetPageScripts()
    {
        // Generate toolbar scripts
        StringBuilder sb = new StringBuilder();
        sb.Append(
@"        
var wptIsMinimizedCookie = 'true';
var wptPosition = 'top';
function wptSetupSearch()
{   
    $j('#", txtSearch.ClientID, @"')
        .keypress(function (e) {
            return wptProceedSpecialKeys(this, e);
        })
        .keyup(function (e) {
            var ret = wptProceedSpecialKeys(this, e);
            wptFilterWebParts(this);
            return ret;
        });
}

function wptFilterWebParts(txtBox) {
    wptFilter(txtBox);
    wptInit(true);
    scrollPanelInit('", scrollPanel.ClientID, @"', true);
}

function wptCategoryChanged() {
    $j('#" + txtSearch.ClientID + @"').val(''); 
    // Remove all tooltip temporary nodes
    jQuery('.WPTTT').remove(); ",
    ControlsHelper.GetPostBackEventReference(hdnUpdater, "update"), @";
}");

        return sb.ToString();
    }

}