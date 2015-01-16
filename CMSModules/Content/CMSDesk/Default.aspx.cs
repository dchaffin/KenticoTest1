using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.DocumentEngine;
using CMS.UIControls;

public partial class CMSModules_Content_CMSDesk_Default : CMSContentPage
{
    #region "Variables & constants"

    private int? mResultNodeID = null;
    private string mResultMode = null;
    private string mResultDevice = null;
    protected string mResultAction = null;

    #endregion


    #region "Private properties"

    private int ExpandNodeID
    {
        get
        {
            return QueryHelper.GetInteger("expandnodeid", 0);
        }
    }


    private int SelectedNodeID
    {
        get
        {
            return ValidationHelper.GetInteger(Request.Params["selectedNodeId"], 0);
        }
    }


    private string SelectedCulture
    {
        get
        {
            return ValidationHelper.GetString(Request.Params["selectedCulture"], CMSContext.CurrentUser.PreferredCultureCode);
        }
    }


    private string SelectedMode
    {
        get
        {
            return ValidationHelper.GetString(Request.Params["selectedMode"], null);
        }
    }


    private string SelectedDevice
    {
        get
        {
            return ValidationHelper.GetString(Request.Params["selectedDevice"], null);
        }
    }


    private string SelectedSplitModeCulture
    {
        get
        {
            return ValidationHelper.GetString(Request.Params["selectedSplitModeCulture"], null);
        }
    }


    private TreeNode RootNode
    {
        get
        {
            // Root
            string baseDoc = "/";
            if (CurrentUser.UserStartingAliasPath != String.Empty)
            {
                // Change to user's root page
                baseDoc = CurrentUser.UserStartingAliasPath;
            }
            // Try to get culture-specific root node
            TreeNode rootNode = Tree.SelectSingleNode(CMSContext.CurrentSiteName, baseDoc, CMSContext.CurrentUser.PreferredCultureCode, false, null, false);
            if (rootNode == null)
            {
                // Get root node
                rootNode = Tree.SelectSingleNode(CMSContext.CurrentSiteName, baseDoc, TreeProvider.ALL_CULTURES, false, null, false);
            }
            return rootNode;
        }
    }


    protected int ResultNodeID
    {
        get
        {
            if (mResultNodeID == null)
            {
                // Get ID from query string
                mResultNodeID = NodeID;
                if (mResultNodeID <= 0)
                {
                    // Get ID selected by user
                    mResultNodeID = SelectedNodeID;
                    // If no node specified, add the root node id
                    if ((mResultNodeID) <= 0 && (NodeID <= 0))
                    {
                        TreeNode rootNode = RootNode;
                        if (rootNode != null)
                        {
                            mResultNodeID = rootNode.NodeID;
                        }
                    }
                }
            }
            return mResultNodeID.Value;
        }
    }
    

    /// <summary>
    /// Resulting viewmode. Prefers user choice over query string setting.
    /// </summary>
    protected string ResultMode
    {
        get
        {
            return mResultMode ?? (mResultMode = SelectedMode ?? (Mode ?? "edit"));
        }
    }
    

    /// <summary>
    /// Resulting device. Prefers user choice over query string setting.
    /// </summary>
    protected string ResultDevice
    {
        get
        {
            return mResultDevice ?? (mResultDevice = SelectedDevice ?? (Device ?? CMSContext.CurrentDeviceProfileName));
        }
    }

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        // Do not check document Read permission
        CheckDocPermissions = false;

        base.OnPreInit(e);

        // Do not include document manager to the controls collection
        EnsureDocumentManager = false;
        DocumentManager.RedirectForNonExistingDocument = false;
    }


    protected override void OnInit(EventArgs e)
    {
        EnsureScriptManager();

        if (!RequestHelper.IsPostBack() && !RequestHelper.IsCallback())
        {
            // Set the culture if specified in query string
            string culture = QueryHelper.GetString("culture", string.Empty);
            if (culture != string.Empty)
            {
                CurrentUser.PreferredCultureCode = culture;
            }
            // Set the view mode if specified in query string
            UpdateViewMode(ViewModeEnum.Edit);
        }

        // Check (and ensure) the proper content culture
        CheckPreferredCulture(false);

        contenttree.Values.AddRange(new[] { new UILayoutValue("NodeID", ResultNodeID), new UILayoutValue("ExpandNodeID", ExpandNodeID), new UILayoutValue("Culture", SelectedCulture) });
        paneMenu.Values.AddRange(new[] { new UILayoutValue("SelectedCulture", SelectedCulture), new UILayoutValue("SelectedMode", ResultMode), new UILayoutValue("SelectedDevice", ResultDevice) });
        contentfooter.Visible = !SettingsKeyProvider.DevelopmentMode;
        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register the scripts
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "defaultVariables", ScriptHelper.GetScript(@"var IsCMSDesk = true;"));
        ScriptHelper.RegisterScriptFile(Page, @"~/CMSModules/Content/CMSDesk/Content.js");

        UIPageURLSettings settings = new UIPageURLSettings
                                         {
                                             Mode = ResultMode,
                                             Action = Action,
                                             NodeID = ResultNodeID,
                                             Culture = CurrentUser.PreferredCultureCode
                                         };
        // Ensure action URL
        contentview.Src = urlRetriever.GetRequestedUrl(settings);
    }

    #endregion
}
