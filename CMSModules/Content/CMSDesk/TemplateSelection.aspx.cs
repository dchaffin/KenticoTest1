using System;
using System.Web.UI;

using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.SettingsProvider;
using CMS.ExtendedControls.ActionsConfig;
using CMS.PortalEngine;

public partial class CMSModules_Content_CMSDesk_TemplateSelection : CMSContentPage
{
    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        int classId = QueryHelper.GetInteger("classId", 0);

        // Load the root category of the selector
        DataClassInfo dci = DataClassInfoProvider.GetDataClass(classId);
        if (dci != null)
        {
            selTemplate.RootCategory = dci.ClassPageTemplateCategoryID;

            if (dci.ClassName.EqualsCSafe("cms.wireframe", true))
            {
                selTemplate.IsWireframe = true;
            }
        }

        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        lblChoose.Text = GetString("TemplateSelection.chooseTemplate");

        DocumentManager.Mode = FormModeEnum.Insert;
        DocumentManager.ParentNodeID = QueryHelper.GetInteger("parentnodeid", 0);
        DocumentManager.NewNodeCultureCode = QueryHelper.GetString("parentculture", null);
        DocumentManager.NewNodeClassID = QueryHelper.GetInteger("classid", 0);

        // Add the continue action
        menuElem.ShowSave = false;
        menuElem.AddExtraAction(new HeaderAction
                                    {
                                        CssClass = "MenuItemEdit",
                                        Text = GetString("General.Continue"),
                                        Tooltip = GetString("General.Continue"),
                                        ImageUrl = GetImageUrl("CMSModules/CMS_Content/EditMenu/continue.png"),
                                        SmallImageUrl = GetImageUrl("CMSModules/CMS_Content/EditMenu/16/continue.png"),
                                        CommandName = "Continue"
                                    });

        menuElem.HeaderActions.ActionPerformed += Menu_ActionPerformed;

        selTemplate.DocumentID = DocumentManager.ParentNode.DocumentID;
        selTemplate.ParentNodeID = DocumentManager.ParentNodeID;
    }


    protected void Menu_ActionPerformed(object sender, System.Web.UI.WebControls.CommandEventArgs e)
    {
        if (e.CommandName.EqualsCSafe("Continue", true))
        {
            string errorMessage = null;

            // Ensure the page template
            PageTemplateInfo pti = selTemplate.EnsureTemplate(null, Guid.Empty, ref errorMessage);

            if (!String.IsNullOrEmpty(errorMessage))
            {
                ShowError(errorMessage);
            }
            else
            {
                // Get the template ID
                int templateId = 0;
                if (pti != null)
                {
                    templateId = pti.PageTemplateId;
                }

                URLHelper.Redirect("~/CMSModules/Content/CMSDesk/Edit/edit.aspx" + URLHelper.Url.Query + "&templateid=" + templateId);
            }

        }
    }

    #endregion
}