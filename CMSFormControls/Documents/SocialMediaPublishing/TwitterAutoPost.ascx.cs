using System;
using System.Collections.Generic;
using System.Xml;

using CMS.FormControls;
using CMS.EventLog;
using CMS.GlobalHelper;
using CMS.WorkflowEngine;
using CMS.CMSHelper;
using CMS.SocialNetworking;
using CMS.DocumentEngine;
using CMS.LicenseProvider;
using CMS.SettingsProvider;

public partial class CMSFormControls_Documents_SocialMediaPublishing_TwitterAutoPost : FormEngineUserControl
{
    #region "Private fields and constants"

    private bool mEnabled = true;
    private bool mIsWorkflow;
    private TreeNode node;
    private SocialMediaPublishingElement dataElement;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets the workflow state of node. True if treeNode is in workflow.
    /// </summary>
    private bool IsWorkflow
    {
        get
        {
            return mIsWorkflow;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the enabled state of the contorl.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            mEnabled = value;
        }
    }


    /// <summary>
    /// Gets or sets the value of this control.
    /// 
    /// Value is XML string with data from controls.
    /// </summary>
    public override object Value
    {
        get
        {
            dataElement.Template = txtTemplateTwitter.Text;
            if (IsWorkflow)
            {
                dataElement.AutoPostAfterPublishing = chkEnabledTwitter.Checked;
            }
            return SerializeData().OuterXml;
        }
        set
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                if ((value != null) && !string.IsNullOrEmpty(value.ToString()))
                {
                    doc.LoadXml(value.ToString());

                    if ((chkEnabledTwitter == null) || (this.txtTemplateTwitter == null))
                    {
                        pnlUpdate.LoadContainer();
                    }

                    DeserializeData(doc);
                }
            }
            catch (Exception ex)
            {
                EventLogProvider ev = new EventLogProvider();
                ev.LogEvent("TwitterAutoPost", "SetValue", ex, CMSContext.CurrentSiteID);
            }
        }
    }

    #endregion


    #region "Protected event handlers"

    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterJQuery(Page);

        btnPostAtTwitter.Text = ResHelper.GetString("socialnetworking.twitter.postat");
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "TwitterAutoPost_" + ClientID, string.Format("countCharactersTwitterInit('{0}', '{1}', {2});", txtTemplateTwitter.ClientID, lblCharactersCount.ClientID, SocialNetworkingHelper.TWITTER_MAX_LENGTH), true);
    }


    protected void Page_Init(object sender, EventArgs e)
    {
        node = (TreeNode)Form.EditedObject;

        TreeNode workflowNode;

        if (Form.IsInsertMode)
        {
            workflowNode = (TreeNode)Form.ParentObject;
        }
        else
        {
            workflowNode = (TreeNode)Form.EditedObject;
        }

        // Setup the workflow information
        WorkflowManager wm = WorkflowManager.GetInstance(new TreeProvider(CMSContext.CurrentUser));
        WorkflowInfo wi = wm.GetNodeWorkflow(workflowNode);
        if (wi != null)
        {
            // Get current step info, do not update document
            WorkflowStepInfo si = wm.GetStepInfo(workflowNode) ??
                                  WorkflowStepInfoProvider.GetFirstStep(wi.WorkflowID);

            // Set IsWorkflow.
            mIsWorkflow = !string.IsNullOrEmpty(si.StepName);
        }
        else
        {
            mIsWorkflow = false;
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        RenderControls();
    }


    protected void btnPostAtTwitter_Click(object sender, EventArgs e)
    {
        if (txtTemplateTwitter.Text.Trim() == "")
        {
            ShowError(ResHelper.GetString("socialnetworking.twitter.emptyerror"));
            return;
        }

        string template = txtTemplateTwitter.Text;

        // Process template.
        MacroResolver mr = MacroResolver.GetInstance();
        string textToSend = mr.ResolveMacros(template);

        // Shorten URLs.
        textToSend = URLShortenerHelper.ShortenURLsInText(textToSend, SocialNetworkType.Twitter);

        ShowInformation(String.Format(ResHelper.GetString("socialnetworking.lengthafterprocessing"), textToSend.Length));

        // Send tweet and do not shorten URLs again
        string tweetUrl = TwitterProvider.PublishTweet(textToSend, CMSContext.CurrentSiteName, false);

        // Check if tweet was succesfully published.
        bool success = !string.IsNullOrEmpty(tweetUrl);

        if (success)
        {
            ShowConfirmation(ResHelper.GetString("socialnetworking.twitter.sendsuccess"));

            dataElement.IsPublished = true;
            dataElement.PostURL = tweetUrl;
            dataElement.AutoPostAfterPublishing = false;
            dataElement.Template = txtTemplateTwitter.Text;

            try
            {
                // Save dataElement into database
                node.SetValue(FieldInfo.Name, SerializeData().OuterXml);
                if (IsWorkflow)
                {
                    DocumentHelper.UpdateDocument(node, node.TreeProvider);
                }
                else
                {
                    node.Update();
                }

                RenderControls();
            }
            catch (Exception ex)
            {
                EventLogProvider ev = new EventLogProvider();
                ev.LogEvent("TwitterAutoPost", "AfterPost", ex, CMSContext.CurrentSiteID);
            }
        }
        else
        {
            ShowError(ResHelper.GetString("socialnetworking.twitter.senderror"));
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Checks if entered data are valid. Data are not valid if checkbox is checked and template is empty.
    /// </summary>
    public override bool IsValid()
    {
        if (IsWorkflow && chkEnabledTwitter.Checked && txtTemplateTwitter.Text.Trim() == "")
        {
            ValidationError = ResHelper.GetString("socialnetworking.twitter.emptyerror");
            return false;
        }
        return true;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Sets properties (Enabled, Text, Visible, etc.) of all controls based on various conditions.
    /// </summary>
    private void RenderControls()
    {
        chkEnabledTwitter.Visible = IsWorkflow;
        btnPostAtTwitter.Visible = true;

        chkEnabledTwitter.Checked = dataElement.AutoPostAfterPublishing;
        txtTemplateTwitter.Text = dataElement.Template;

        if (dataElement.IsPublished)
        {
            lblTwitterIsPublished.Text = ResHelper.GetString("socialnetworking.twitter.ispublished");
            lnkTwitterPostURL.NavigateUrl = dataElement.PostURL;
            lnkTwitterPostURL.Text = dataElement.PostURL;
            lnkTwitterPostURL.Target = "_blank";
            lnkTwitterPostURL.Visible = true;
        }
        else
        {
            lblTwitterIsPublished.Text = ResHelper.GetString("socialnetworking.twitter.isnotpublished");
        }

        bool isSetting = SocialNetworkingHelper.IsSettingSet(CMSContext.CurrentSiteName, SocialNetworkType.Twitter);

        // Check the license
        string currentDomain = URLHelper.GetCurrentDomain();
        if (!String.IsNullOrEmpty(currentDomain))
        {
            if (!LicenseHelper.CheckFeature(currentDomain, FeatureEnum.SocialAutoPost))
            {
                Enabled = false;
                isSetting = true;

                lblAccessDeniedWarning.Visible = true;
            }
        }

        // Disable button "Post at Twitter" if document wasn't saved yet.
        btnPostAtTwitter.Enabled = (Enabled && (node.NodeID > 0) && isSetting);
        chkEnabledTwitter.Enabled = (Enabled && isSetting);
        txtTemplateTwitter.Enabled = Enabled;

        if (!btnPostAtTwitter.Enabled)
        {
            btnPostAtTwitter.CssClass += "Disabled";
        }

        if (!isSetting)
        {
            lblTwitterMissingSetting.Visible = true;
        }

        lblCharactersCount.Text = string.Format("{0}/{1}", (dataElement.Template != null ? dataElement.Template.Length : 0), SocialNetworkingHelper.TWITTER_MAX_LENGTH);
        if ((dataElement.Template != null) && (dataElement.Template.Length > SocialNetworkingHelper.TWITTER_MAX_LENGTH))
        {
            lblCharactersCount.Style.Add("color", "red");
        }
    }

    /// <summary>
    /// Serializes data from controls and returns XmlDocument.
    /// </summary>
    private XmlDocument SerializeData()
    {
        dataElement.SocialNetworkType = SocialNetworkType.Twitter;

        return SocialNetworkingHelper.SerializePublishingElement(dataElement);
    }


    /// <summary>
    /// Deserializes the passed XmlDocument and sets data directly into controls.
    /// </summary>
    /// <param name="xmlDocument">XmlDocument to be desrialized</param>
    private void DeserializeData(XmlDocument xmlDocument)
    {
        dataElement = SocialNetworkingHelper.DeserializePublishingElement(xmlDocument);

        if (dataElement.SocialNetworkType != SocialNetworkType.Twitter)
        {
            throw new Exception("Unrecognized XML format - wrong name element");
        }
    }

    #endregion
}