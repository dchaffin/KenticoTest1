using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Xml;

using CMS.FormControls;
using CMS.EventLog;
using CMS.GlobalHelper;
using CMS.TreeEngine;
using CMS.WorkflowEngine;
using CMS.CMSHelper;
using CMS.SocialNetworking;
using CMS.DocumentEngine;
using CMS.LicenseProvider;
using CMS.SettingsProvider;

public partial class CMSFormControls_Documents_SocialMediaPublishing_FacebookAutoPost : FormEngineUserControl
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
            //return false;
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
            dataElement.Template = txtTemplateFacebook.Text;
            if (IsWorkflow)
            {
                dataElement.AutoPostAfterPublishing = chkEnabledFacebook.Checked;
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

                    if ((this.chkEnabledFacebook == null) || (this.txtTemplateFacebook == null))
                    {
                        this.pnlUpdate.LoadContainer();
                    }

                    DeserializeData(doc);
                }
            }
            catch (Exception ex)
            {
                EventLogProvider ev = new EventLogProvider();
                ev.LogEvent("FacebookAutoPost", "SetValue", ex, CMSContext.CurrentSiteID);
            }
        }
    }

    #endregion


    #region "Protected event handlers"

    protected void Page_Load(object sender, EventArgs e)
    {
        btnPostAtFacebook.Text = ResHelper.GetString("socialnetworking.facebook.postat");
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


    protected void btnPostAtFacebook_Click(object sender, EventArgs e)
    {
        if (txtTemplateFacebook.Text.Trim() == "")
        {
            ShowError(ResHelper.GetString("socialnetworking.facebook.emptyerror"));
            return;
        }

        string template = txtTemplateFacebook.Text;

        // Process template
        MacroResolver mr = MacroResolver.GetInstance();
        string textToSend = mr.ResolveMacros(template);

        // Send text
        string postUrl = FacebookProvider.Publish(textToSend, CMSContext.CurrentSiteName);

        // Check if post was succesfully published
        bool success = !string.IsNullOrEmpty(postUrl);

        if (success)
        {
            ShowConfirmation(ResHelper.GetString("socialnetworking.facebook.sendsuccess"));

            dataElement.IsPublished = true;
            dataElement.AutoPostAfterPublishing = false;
            dataElement.Template = txtTemplateFacebook.Text;
            dataElement.PostURL = postUrl;

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
                ev.LogEvent("FacebookAutoPost", "AfterPost", ex, CMSContext.CurrentSiteID);
            }
        }
        else
        {
            ShowError(ResHelper.GetString("socialnetworking.facebook.senderror"));
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Checks if entered data are valid. Data are not valid if checkbox is checked and template is empty.
    /// </summary>
    public override bool IsValid()
    {
        if (IsWorkflow && chkEnabledFacebook.Checked && txtTemplateFacebook.Text.Trim() == "")
        {
            ValidationError = ResHelper.GetString("socialnetworking.facebook.emptyerror");
            return false;
        }
        return true;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Renders controls depending on current state.
    /// </summary>
    private void RenderControls()
    {
        chkEnabledFacebook.Visible = IsWorkflow;
        btnPostAtFacebook.Visible = true;

        chkEnabledFacebook.Checked = dataElement.AutoPostAfterPublishing;
        txtTemplateFacebook.Text = dataElement.Template;

        // Set info message
        if (dataElement.IsPublished)
        {
            lblFacebookIsPublished.Text = ResHelper.GetString("socialnetworking.facebook.ispublished");
            lnkFacebookPostURL.NavigateUrl = dataElement.PostURL;
            lnkFacebookPostURL.Text = dataElement.PostURL;
            lnkFacebookPostURL.Target = "_blank";
            lnkFacebookPostURL.Visible = true;
        }
        else
        {
            lblFacebookIsPublished.Text = ResHelper.GetString("socialnetworking.facebook.isnotpublished");
        }

        bool isSetting = SocialNetworkingHelper.IsSettingSet(CMSContext.CurrentSiteName, SocialNetworkType.Facebook); 

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

        // Disable button "Post at Facebook" if document wasn't saved yet.
        btnPostAtFacebook.Enabled = (Enabled && (node.NodeID > 0) && isSetting);
        chkEnabledFacebook.Enabled = (Enabled && isSetting);
        txtTemplateFacebook.Enabled = Enabled;

        if (!btnPostAtFacebook.Enabled)
        {
            btnPostAtFacebook.CssClass += "Disabled";
        }

        if (!isSetting)
        {
            lblFacebookMissingSetting.Visible = true;
        }

        if (FacebookProvider.IsAccessTokenExpired(CMSContext.CurrentSiteName))
        {
            lblExpirationWarning.Visible = true;
        }
    }

    /// <summary>
    /// Serializes data from controls and returns XmlDocument.
    /// </summary>
    private XmlDocument SerializeData()
    {
        dataElement.SocialNetworkType = SocialNetworkType.Facebook;

        return SocialNetworkingHelper.SerializePublishingElement(dataElement);
    }


    /// <summary>
    /// Deserializes the passed XmlDocument and sets data directly into controls.
    /// </summary>
    /// <param name="xmlDocument">XmlDocument to be deserialized</param>
    private void DeserializeData(XmlDocument xmlDocument)
    {
        dataElement = SocialNetworkingHelper.DeserializePublishingElement(xmlDocument);

        if (dataElement.SocialNetworkType != SocialNetworkType.Facebook)
        {
            throw new Exception("Unrecognized XML format - wrong name element");
        }
    }

    #endregion
}