using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_UI_Layout_SaveNewPageTemplate : CMSModalDesignPage
{
    protected int pageTemplateId = 0;
    protected PageTemplateInfo pt = null;


    protected void Page_Load(object sender, EventArgs e)
    {
        pageTemplateId = QueryHelper.GetInteger("templateid", 0);
        if (pageTemplateId > 0)
        {
            pt = PageTemplateInfoProvider.GetPageTemplateInfo(pageTemplateId);
        }

        bool keep = QueryHelper.GetBoolean("assign", true);
        if (!keep)
        {
            chkKeep.Enabled = false;
            chkKeep.Checked = false;
        }

        // Check the authorization per UI element
        CurrentUserInfo currentUser = CMSContext.CurrentUser;
        if (!currentUser.IsAuthorizedPerUIElement("CMS.Content", new string[] { "Properties", "Properties.Template", "Template.SaveAsNew" }, CMSContext.CurrentSiteName))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "Properties;Properties.Template;Template.SaveAsNew");
        }

        CurrentMaster.Title.TitleText = GetString("PortalEngine.SaveNewPageTemplate.PageTitle");
        CurrentMaster.Title.TitleImage = GetImageUrl("CMSModules/CMS_Content/Template/save.png");

        btnOk.Text = GetString("general.ok");
        btnCancel.Text = GetString("general.cancel");

        // Preset category
        if (!RequestHelper.IsPostBack())
        {
            if (pt != null)
            {
                categorySelector.Value = pt.CategoryID.ToString();
                if (pt.IsReusable)
                {
                    plcKeep.Visible = false;
                }
            }
        }
    }


    protected void btnOK_Click(object sender, EventArgs e)
    {
        if (pt != null)
        {
            // Limit text length
            txtTemplateDisplayName.Text = TextHelper.LimitLength(txtTemplateDisplayName.Text.Trim(), 200, "");

            // finds whether required fields are not empty
            string result = new Validator()
                .NotEmpty(txtTemplateDisplayName.Text, GetString("Administration-PageTemplate_General.ErrorEmptyTemplateDisplayName"))
                .NotEmpty(txtTemplateCodeName.Text, GetString("Administration-PageTemplate_General.ErrorEmptyTemplateCodeName"))
                .IsCodeName(txtTemplateCodeName.Text, GetString("general.invalidcodename"))
                .Result;

            if (String.IsNullOrEmpty(result))
            {
                // Check if template with given name already exists            
                if (PageTemplateInfoProvider.PageTemplateNameExists(txtTemplateCodeName.Text))
                {
                    ShowError(GetString("general.codenameexists"));
                }

                if ((pt.IsReusable == true) || !chkKeep.Checked)
                {
                    // Clone template with clear
                    pt = pt.Clone(true);
                }
                pt.CodeName = txtTemplateCodeName.Text;
                pt.DisplayName = txtTemplateDisplayName.Text;
                pt.Description = txtTemplateDescription.Text;

                pt.CategoryID = Convert.ToInt32(categorySelector.Value);

                // Reset the Ad-hoc status
                pt.IsReusable = true;
                pt.PageTemplateNodeGUID = Guid.Empty;

                pt.PageTemplateSiteID = 0;

                try
                {
                    PageTemplateInfoProvider.SetPageTemplateInfo(pt);
                    int siteId = QueryHelper.GetInteger("siteid", 0);
                    if (siteId > 0)
                    {
                        PageTemplateInfoProvider.AddPageTemplateToSite(pt.PageTemplateId, siteId);
                    }

                    if (!chkKeep.Checked)
                    {
                        ShowInformation(GetString("PortalEngine.SaveNewPageTemplate.Saved"), null, true);

                        txtTemplateCodeName.Text = pt.CodeName;

                        pnlContent.Enabled = false;
                        btnOk.Visible = false;
                        btnCancel.Text = GetString("General.Close");
                    }
                    else
                    {
                        string script = null;

                        bool refresh = QueryHelper.GetBoolean("refresh", false);
                        if (refresh)
                        {
                            script = "wopener.location.replace(wopener.location); CloseDialog();";
                        }
                        else
                        {
                            script = "SelectTemplate(" + pt.PageTemplateId.ToString() + ");";
                        }
                        
                        ltlScript.Text = ScriptHelper.GetScript(script);
                    }
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
            else
            {
                ShowError(result);
            }
        }
    }
}