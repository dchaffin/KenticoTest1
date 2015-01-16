using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSFormControls_LiveSelectors_InsertImageOrMedia_NewCMSFolder : CMSLiveModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (QueryHelper.ValidateHash("hash"))
        {
            // Initialize controls
            SetupControls();
        }
        else
        {
            createFolder.Visible = false;
            string url = ResolveUrl("~/CMSMessages/Error.aspx?title=" + GetString("dialogs.badhashtitle") + "&text=" + GetString("dialogs.badhashtext") + "&cancel=1");
            ScriptHelper.RegisterStartupScript(Page, typeof(string), "redirect", ScriptHelper.GetScript("if (window.parent != null) { window.parent.location = '" + url + "' }"));
        }
    }


    /// <summary>
    /// Initializes controls.
    /// </summary>
    private void SetupControls()
    {
        // Get data from query string
        int parentNodeId = QueryHelper.GetInteger("nodeid", 0);

        createFolder.OnFolderChange += createFolder_OnFolderChange;
        createFolder.CancelClick += createFolder_CancelClick;
        createFolder.IsLiveSite = true;

        // Initialize information on library
        createFolder.ParentNodeID = QueryHelper.GetInteger("nodeid", 0);
        createFolder.ParentCulture = QueryHelper.GetString("culture", null);

        Page.Header.Title = GetString("dialogs.newfoldertitle");

        CurrentMaster.Title.TitleText = GetString("dialogs.folder.new");
        CurrentMaster.Title.TitleImage = ResolveUrl(GetImageUrl("CMSModules/CMS_Content/Dialogs/newfolder.png"));
    }


    #region "Event handlers"

    protected void createFolder_CancelClick()
    {
        ltlScript.Text = ScriptHelper.GetScript("wopener.SetAction('cancelfolder', ''); wopener.RaiseHiddenPostBack(); CloseDialog();");
    }


    protected void createFolder_OnFolderChange(int nodeToSelect)
    {
        ltlScript.Text = ScriptHelper.GetScript("wopener.SetAction('newfolder', '" + nodeToSelect.ToString() + "'); wopener.RaiseHiddenPostBack(); CloseDialog();");
    }

    #endregion
}