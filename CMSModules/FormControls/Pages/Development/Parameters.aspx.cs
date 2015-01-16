using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.URLRewritingEngine;
using CMS.FormEngine;


public partial class CMSModules_FormControls_Pages_Development_Parameters : SiteManagerPage
{
    #region "Private variables"

    private FormUserControlInfo fuci = null;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.BodyClass += " FieldEditorBody";

        // If saved is found in query string
        if ((!RequestHelper.IsPostBack()) && (QueryHelper.GetInteger("saved", 0) == 1))
        {
            ShowChangesSaved();
        }

        // Load form control
        int controlId = QueryHelper.GetInteger("controlId", 0);
        fuci = FormUserControlInfoProvider.GetFormUserControlInfo(controlId);
        EditedObject = fuci;
        if (fuci != null)
        {
            if (fuci.UserControlParentID > 0)
            {
                fieldEditor.Visible = false;
                pnlDefaultEditor.Visible = true;
                defaultValueEditor.Visible = true;
                defaultValueEditor.OnEditorLoaded += new CMSModules_PortalEngine_UI_WebParts_Development_DefaultValueEditor.EditorLoadedEventHandler(GetFormInfo);
                defaultValueEditor.DefaultValueXMLDefinition = fuci.UserControlParameters;

                // Add handler to
                defaultValueEditor.XMLCreated += new EventHandler(defaultValueEditor_XMLCreated);
            }
            else
            {
                // Initialize field editor
                fieldEditor.Mode = FieldEditorModeEnum.FormControls;
                fieldEditor.FormDefinition = fuci.UserControlParameters;
                fieldEditor.OnAfterDefinitionUpdate += fieldEditor_OnAfterDefinitionUpdate;
            }
        }
    }


    /// <summary>
    /// Gets FormInfo from current or parent form control.
    /// </summary>
    /// <returns>User control form info</returns>
    private FormInfo GetFormInfo()
    {
        FormInfo fi = null;

        // Control has parent
        if (fuci.UserControlParentID > 0)
        {
            FormUserControlInfo fuc = FormUserControlInfoProvider.GetFormUserControlInfo(fuci.UserControlParentID);
            fi = FormHelper.GetFormControlParameters(fuc.UserControlCodeName, fuc.UserControlParameters, true);
        }
        else
        {
            fi = FormHelper.GetFormControlParameters(fuci.UserControlCodeName, fuci.UserControlParameters, true);
        }

        return fi;
    }


    /// <summary>
    /// Field editor updated event.
    /// </summary>
    private void fieldEditor_OnAfterDefinitionUpdate(object sender, EventArgs e)
    {
        // Update Form user control parameters
        if (fuci != null)
        {
            fuci.UserControlParameters = fieldEditor.FormDefinition;
            FormUserControlInfoProvider.SetFormUserControlInfo(fuci);
        }
    }


    /// <summary>
    /// XML created, save it.
    /// </summary>
    protected void defaultValueEditor_XMLCreated(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(defaultValueEditor.ErrorMessage))
        {
            ShowError(defaultValueEditor.ErrorMessage);
            return;
        }

        if ((fuci != null) && (fuci.UserControlParentID > 0))
        {
            fuci.UserControlParameters = defaultValueEditor.DefaultValueXMLDefinition;
            FormUserControlInfoProvider.SetFormUserControlInfo(fuci);
        }

        // Redirect with saved assign
        string url = URLHelper.UpdateParameterInUrl(URLRewriter.CurrentURL, "saved", "1");
        URLHelper.Redirect(url);
    }
}