using System;
using System.Text;

using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_AdminControls_Pages_ObjectVersions : CMSObjectVersioningPage
{
    private bool dialogMode = false;


    protected override void OnPreInit(EventArgs e)
    {
        SetCulture();

        // Check hash
        if (!QueryHelper.ValidateHash("hash"))
        {
            RedirectToAccessDenied(ResHelper.GetString("dialogs.badhashtitle"));
        }

        // Set dialog mode
        dialogMode = QueryHelper.GetBoolean("editonlycode", false);
        if (dialogMode)
        {
            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master";

            // Set CSS classes
            CurrentMaster.PanelContent.CssClass = "PageContent";
            CurrentMaster.PanelFooter.CssClass = "FloatRight";

            // Add close button
            CurrentMaster.PanelFooter.Controls.Add(new LocalizedButton
                                                       {
                                                           ID = "btnClose",
                                                           ResourceString = "general.close",
                                                           EnableViewState = false,
                                                           OnClientClick = "return CloseDialog();",
                                                           CssClass = "SubmitButton"
                                                       });

            var showFooter = QueryHelper.GetBoolean("showfooter", true);
            CurrentMaster.FooterContainer.Visible = showFooter;
        }

        // Must be called after the master page file is set
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        string objectType = QueryHelper.GetString("objecttype", String.Empty);
        int objectId = QueryHelper.GetInteger("objectid", 0);

        versionList.ObjectID = objectId;
        versionList.ObjectType = objectType;
        versionList.IsLiveSite = false;
        versionList.RegisterReloadHeaderScript = !QueryHelper.GetBoolean("noreload", false);
        versionList.DialogMode = dialogMode;

        // Register refresh script to refresh wopener
        StringBuilder script = new StringBuilder();
        script.Append(@"
function RefreshContent() {
  var wopener = parent.wopener;  
  if(wopener != null) {
    if (wopener.RefreshPage != null) {
      wopener.RefreshPage();
    }
    else if (wopener.Refresh != null) {
      wopener.Refresh();
    } 
  }
  else
  {
     if((parent != null) && (parent.Refresh != null))
     {
        parent.Refresh();
     }
  }
}");
        // Register script
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "WOpenerRefresh", ScriptHelper.GetScript(script.ToString()));
    }
}