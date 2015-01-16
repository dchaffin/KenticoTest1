using System;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_UI_PageTemplates_PageTemplate_HeaderTab : CMSEditTemplatePage
{
    #region "Variables"

    private PageTemplateInfo pti = null;
    private bool dialog = false;

    #endregion


    #region "Page events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        dialog = QueryHelper.GetBoolean("dialog", false);

        lblTemplateHeader.Text = GetString("pagetemplate_header.addheader");

        pnlObjectLocking.ObjectManager.OnSaveData += new EventHandler<SimpleObjectManagerEventArgs>(ObjectManager_OnSaveData);
        pnlObjectLocking.ObjectManager.OnAfterAction += new EventHandler<SimpleObjectManagerEventArgs>(ObjectManager_OnAfterAction);

        pti = PageTemplateInfoProvider.GetPageTemplateInfo(PageTemplateID);
        CMSContext.EditedObject = pti;

        if (!RequestHelper.IsPostBack())
        {
            // Load data to form
            LoadData();
        }
    }


    private void LoadData()
    {
        if (pti != null)
        {
            txtTemplateHeader.Text = pti.PageTemplateHeader;
            chkAllowInherit.Checked = pti.PageTemplateAllowInheritHeader;
            chkInheritParent.Checked = pti.PageTemplateInheritParentHeader;
        }
    }


    protected void ObjectManager_OnAfterAction(object sender, SimpleObjectManagerEventArgs e)
    {
        if (e.ActionName != ComponentEvents.SAVE)
        {
            LoadData();
        }

        if (dialog)
        {
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "parentWOpenerRefresh", ScriptHelper.GetScript("if (parent && parent.wopener && parent.wopener.refresh) { parent.wopener.refresh(); }"));
        }
    }


    protected void ObjectManager_OnSaveData(object sender, SimpleObjectManagerEventArgs e)
    {
        if (PageTemplate != null)
        {
            PageTemplate.PageTemplateHeader = txtTemplateHeader.Text.Trim();
            PageTemplate.PageTemplateAllowInheritHeader = chkAllowInherit.Checked;
            PageTemplate.PageTemplateInheritParentHeader = chkInheritParent.Checked;

            try
            {
                // Save changes
                PageTemplateInfoProvider.SetPageTemplateInfo(PageTemplate);
                ShowChangesSaved();
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowError(ResHelper.GetStringFormat("general.sourcecontrolerror", ex.Message));
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);

            }
        }
    }

    #endregion
}