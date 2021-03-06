using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.WorkflowEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_OnlineMarketing_Controls_UI_MVTVariant_Edit : CMSAdminEditControl
{
    #region

    private string mVariantCodeName = string.Empty;

    #endregion


    #region "Properties"

    /// <summary>
    /// UIForm control used for editing objects properties.
    /// </summary>
    public UIForm UIFormControl
    {
        get
        {
            return EditForm;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            EditForm.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            EditForm.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets the code name of the variant.
    /// </summary>
    public string VariantCodeName
    {
        get
        {
            if (string.IsNullOrEmpty(mVariantCodeName))
            {
                mVariantCodeName = ValidationHelper.GetString(((FormEngineUserControl)UIFormControl.FieldControls["MVTVariantName"]).Value, string.Empty);
            }

            return mVariantCodeName;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        EditForm.OnAfterSave += new EventHandler(EditForm_OnAfterSave);
    }


    /// <summary>
    /// Handles the OnAfterSave event of the EditForm control.
    /// </summary>
    private void EditForm_OnAfterSave(object sender, EventArgs e)
    {
        if (UIFormControl.EditedObject != null)
        {
            // Log widget variant synchronization
            MVTVariantInfo variantInfo = (MVTVariantInfo)UIFormControl.EditedObject;

            // Clear cache
            CacheHelper.TouchKey("om.mvtvariant|bytemplateid|" + variantInfo.MVTVariantPageTemplateID);

            if (variantInfo.MVTVariantDocumentID > 0)
            {
                // Log synchronization
                TreeProvider tree = new TreeProvider(CMSContext.CurrentUser);
                TreeNode node = tree.SelectSingleDocument(variantInfo.MVTVariantDocumentID);
                DocumentSynchronizationHelper.LogDocumentChange(node, TaskTypeEnum.UpdateDocument, tree);
            }
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Validates the form data. Checks the code name format and if the code name is unique.
    /// </summary>
    public bool ValidateData()
    {
        // Check the required fields for emptiness
        bool isValid = EditForm.ValidateData();

        if (isValid)
        {
            if ((EditForm.EditedObject == null) || (EditForm.ParentObject == null))
            {
                UIFormControl.ShowError(GetString("general.saveerror"));
                return false;
            }

            // Create a temporary variant object in order to check the code name format rules and uniqueness
            MVTVariantInfo variant = new MVTVariantInfo();
            variant.MVTVariantName = VariantCodeName;
            variant.MVTVariantID = EditForm.EditedObject.GetIntegerValue("MVTVariantID", 0);
            variant.MVTVariantPageTemplateID = EditForm.ParentObject.GetIntegerValue("PageTemplateID", 0);

            // Validate the codename format
            if (!ValidationHelper.IsCodeName(VariantCodeName))
            {
                isValid = false;
                UIFormControl.ShowError(String.Format(GetString("general.codenamenotvalid"), VariantCodeName));
            }
            // Check if the code name already exists
            else if (!variant.CheckUniqueCodeName())
            {
                isValid = false;
                UIFormControl.ShowError(String.Format(GetString("general.codenamenotunique"), VariantCodeName));
            }
        }

        return isValid;
    }

    #endregion
}