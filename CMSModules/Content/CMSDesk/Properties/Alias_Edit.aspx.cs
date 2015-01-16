using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_Content_CMSDesk_Properties_Alias_Edit : CMSPropertiesPage
{
    #region "Private variables"

    protected int aliasId = 0;

    bool isManagement = false;
    int defaultNodeID = 0;

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        isManagement = QueryHelper.GetBoolean("management", false);
        if (isManagement)
        {
            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master";
        }

        // Must be called after the master page file is set
        base.OnPreInit(e);
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.Content", "Properties.URLs"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "Properties.URLs");
        }

        if (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.Content", "URLs.Aliases"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "URLs.Aliases");
        }

        DataHelper.FillListControlWithEnum(typeof(AliasActionModeEnum), drpAction, "aliasaction.", DocumentAliasInfoProvider.AliasActionModeEnumString);

        if (isManagement)
        {
            pnlContent.CssClass += " PageContent";
        }

        // Disable document manager events
        DocumentManager.RegisterEvents = false;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterDialogScript(Page);

        if (isManagement)
        {
            CurrentMaster.PanelFooter.CssClass = "FloatRight";

            CurrentMaster.Title.TitleText = GetString("doc.urls.aliaslist");
            CurrentMaster.Title.TitleImage = GetImageUrl("CMSModules/CMS_Content/Menu/New.png");

            // Add close button 
            CurrentMaster.PanelFooter.Controls.Add(new LocalizedButton
            {
                ID = "btnCancel",
                ResourceString = "general.close",
                EnableViewState = false,
                OnClientClick = "return CloseDialog();",
                CssClass = "SubmitButton"
            });
        }
        else
        {
            HeaderActions.AddAction(new HeaderAction()
            {
                ImageUrl = GetImageUrl("CMSModules/CMS_Content/Properties/adddocumentalias.png"),
                Text = GetString("doc.urls.addnewalias"),
                RedirectUrl = ResolveUrl("Alias_Edit.aspx?nodeid=" + NodeID),
            });

            rightHeaderActions.AddAction(new HeaderAction()
            {
                ImageUrl = GetImageUrl("CMSModules/CMS_Content/Properties/documentaliases.png"),
                Text = GetString("doc.urls.viewallalias"),
                OnClientClick = "modalDialog('" + ResolveUrl("~/CMSModules/Content/CMSDesk/Properties/Alias_AliasList.aspx") + "?nodeid=" + NodeID + "','AliasManagement','90%','85%');",
            });
        }

        if (Node != null)
        {
            lblUrlInfoText.Text = Node.NodeAliasPath;

            // Check modify permissions
            if (!CheckDocumentPermissions(Node, PermissionsEnum.Modify))
            {
                ShowInformation(String.Format(GetString("cmsdesk.notauthorizedtoeditdocument"), Node.NodeAliasPath));

                usSelectCampaign.Enabled = false;
                txtURLExtensions.Enabled = false;

                ctrlURL.Enabled = false;

                cultureSelector.Enabled = false;
            }

            if (!RequestHelper.IsPostBack() && QueryHelper.GetInteger("saved", 0) == 1)
            {
                ShowChangesSaved();
            }

            lblDocumentCulture.Text = GetString("general.culture") + ResHelper.Colon;
            lblTrackCampaign.Text = GetString("doc.urls.trackcampaign") + ResHelper.Colon;
            lblURLExtensions.Text = GetString("doc.urls.urlextensions") + ResHelper.Colon;

            // Show path of document alias only if management mode edit 
            pnlUrlInfo.Visible = isManagement;

            // For management mode use DefaultNodeID 
            defaultNodeID = (isManagement) ? QueryHelper.GetInteger("defaultNodeID", 0) : NodeID;

            // Initialize page title
            string urls = GetString("Properties.Urls");
            string urlsUrl = string.Format("~/CMSModules/Content/CMSDesk/Properties/Alias_List.aspx?nodeid={0}&compare=1", defaultNodeID.ToString());
            string addAlias = GetString("doc.urls.addnewalias");
            string aliasManagement = GetString("doc.urls.aliaslist");
            string managementUrl = "~/CMSModules/Content/CMSDesk/Properties/Alias_AliasList.aspx?nodeid=" + defaultNodeID.ToString();

            string[,] pageTitleTabs = null;

            pageTitleTabs = new string[2, 3];
            // Initializes page title                    
            pageTitleTabs[0, 0] = isManagement ? aliasManagement : urls;
            pageTitleTabs[0, 1] = ResolveUrl(isManagement ? managementUrl : urlsUrl);
            pageTitleTabs[0, 2] = "";
            pageTitleTabs[1, 0] = addAlias;
            pageTitleTabs[1, 1] = "";
            pageTitleTabs[1, 2] = "";
            CurrentMaster.Title.Breadcrumbs = pageTitleTabs;

            cultureSelector.AddDefaultRecord = false;
            cultureSelector.SpecialFields = new string[,] { { GetString("general.selectall"), "" } };
            cultureSelector.CssClass = "ContentMenuLangDrop";

            aliasId = ValidationHelper.GetInteger(Request.QueryString["aliasid"], 0);

            CurrentMaster.Title.HelpName = "helpTopic";
            CurrentMaster.Title.HelpTopicName = "doc_documentalias_edit";

            if (!RequestHelper.IsPostBack())
            {
                cultureSelector.Value = Node.DocumentCulture;

                // Edit existing alias
                if (aliasId > 0)
                {
                    DocumentAliasInfo dai = DocumentAliasInfoProvider.GetDocumentAliasInfo(aliasId);

                    if (dai != null)
                    {
                        usSelectCampaign.Value = dai.AliasCampaign;

                        txtURLExtensions.Text = dai.AliasExtensions;
                        ctrlURL.URLPath = dai.AliasURLPath;

                        cultureSelector.Value = dai.AliasCulture;
                        pageTitleTabs[1, 0] = addAlias = TreePathUtils.GetURLPathDisplayName(dai.AliasURLPath);

                        drpAction.SelectedValue = DocumentAliasInfoProvider.AliasActionModeEnumString(dai.AliasActionMode);
                    }
                }
            }

            CurrentMaster.Title.Breadcrumbs = pageTitleTabs;

            // Register js synchronization script for split mode
            if (QueryHelper.GetBoolean("refresh", false) && CMSContext.DisplaySplitMode)
            {
                RegisterSplitModeSync(true, false, true);
            }
        }
    }

    #endregion


    protected void btnOK_Click(object sender, EventArgs e)
    {
        if (String.IsNullOrEmpty(ctrlURL.PlainURLPath))
        {
            ShowError(GetString("doc.urls.requiresurlpath"));
            return;
        }

        if (!usSelectCampaign.IsValid())
        {
            ShowError(GetString("campaign.validcodename"));
            return;
        }

        // Validate URL path
        if (!ctrlURL.IsValid())
        {
            ShowError(ctrlURL.ValidationError);
            return;
        }

        DocumentAliasInfo dai = null;

        // Edit existing alias
        if (aliasId > 0)
        {
            dai = DocumentAliasInfoProvider.GetDocumentAliasInfo(aliasId);
        }

        if (Node != null)
        {
            // Check modify permissions
            if (!CheckDocumentPermissions(Node, PermissionsEnum.Modify))
            {
                ShowError(String.Format(GetString("cmsdesk.notauthorizedtoreaddocument"), Node.NodeAliasPath));
                return;
            }

            if (dai == null)
            {
                dai = new DocumentAliasInfo();
            }

            // Check whether 
            if (!DocumentAliasInfoProvider.IsUnique(ctrlURL.URLPath, dai.AliasID, Convert.ToString(cultureSelector.Value), txtURLExtensions.Text.Trim(), CMSContext.CurrentSiteName, true, NodeID))
            {
                ShowError(GetString("doc.urls.doacaliasnotunique"));
                return;
            }

            // Set object properties
            dai.AliasURLPath = ctrlURL.URLPath;

            dai.AliasCampaign = ValidationHelper.GetString(usSelectCampaign.Value, String.Empty).Trim();
            dai.AliasExtensions = txtURLExtensions.Text.Trim();
            dai.AliasCulture = ValidationHelper.GetString(cultureSelector.Value, "");
            dai.AliasSiteID = CMSContext.CurrentSite.SiteID;
            dai.AliasActionMode = DocumentAliasInfoProvider.GetAliasActionModeEnum(drpAction.SelectedValue);
            dai.AliasNodeID = NodeID;

            // Insert into database
            DocumentAliasInfoProvider.SetDocumentAliasInfo(dai, Node.NodeSiteName);

            // Log synchronization
            DocumentSynchronizationHelper.LogDocumentChange(Node, TaskTypeEnum.UpdateDocument, Tree);

            aliasId = dai.AliasID;

            string url = "Alias_Edit.aspx?saved=1&nodeid=" + NodeID + "&aliasid=" + aliasId.ToString() + "&management=" + isManagement;
            if (isManagement)
            {
                url += "&defaultNodeID=" + defaultNodeID;
            }

            // Refresh the second frame in split mode
            if (CMSContext.DisplaySplitMode)
            {
                url += "&refresh=1";
            }

            URLHelper.Redirect(url);
        }
    }
}