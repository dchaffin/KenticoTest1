using System;
using System.Linq;

using CMS.CMSHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

// Edited object
[EditedObject(PredefinedObjectType.USER, "userid")]
public partial class CMSModules_Membership_Pages_Users_User_Edit_CustomFields : CMSUsersPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (EditedObject != null)
        {
            // Check that only global administrator can edit global administrator's accounts
            UserInfo ui = (UserInfo)EditedObject;
            CheckUserAvaibleOnSite(ui);

            if (!CheckGlobalAdminEdit(ui))
            {
                plcUserCustomFields.Visible = false;
                plcUserSettingsCustomFields.Visible = false;
                ShowError(GetString("Administration-User_List.ErrorGlobalAdmin"));
                return;
            }

            // Setup user info for user custom fields dataform
            formUserCustomFields.Info = ui;

            // If table has not any custom field hide custom field placeholder
            if ((formUserCustomFields.Info == null) || !formUserCustomFields.BasicForm.FormInformation.GetFormElements(true, false, true).Any())
            {
                plcUserCustomFields.Visible = false;
                formUserCustomFields.StopProcessing = true;
            }
            else
            {
                // Setup the User DataForm
                formUserCustomFields.BasicForm.HideSystemFields = true;
                formUserCustomFields.BasicForm.CssClass = "ContentDataFormButton";
                formUserCustomFields.BasicForm.SubmitButton.Visible = false;
            }

            // Setup user settings info for user settings custom fields dataform
            formUserSettingsCustomFields.Info = ui.UserSettings;

            if ((formUserSettingsCustomFields.Info == null) || !formUserSettingsCustomFields.BasicForm.FormInformation.GetFormElements(true, false, true).Any())
            {
                plcUserSettingsCustomFields.Visible = false;
                formUserSettingsCustomFields.StopProcessing = true;
            }
            else
            {
                // Setup the UserSettings DataForm
                formUserSettingsCustomFields.BasicForm.HideSystemFields = true;
                formUserSettingsCustomFields.BasicForm.CssClass = "ContentDataFormButton";
                formUserSettingsCustomFields.BasicForm.SubmitButton.Visible = false;

                formUserSettingsCustomFields.OnAfterSave += formUserSettingsCustomFields_OnAfterSave;
            }
        }
        else
        {
            formUserCustomFields.StopProcessing = true;
            formUserSettingsCustomFields.StopProcessing = true;
        }
    }


    protected void formUserSettingsCustomFields_OnAfterSave(object sender, EventArgs e)
    {
        // Force staging task creation on save
        formUserSettingsCustomFields.Info.Generalized.TouchParent();
    }



    protected void btnOk_OnClick(object sender, EventArgs e)
    {
        // Check permissions
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Users", "Modify"))
        {
            RedirectToAccessDenied("CMS.Users", "Modify");
        }

        bool saved = true;

        // Try to save first form
        if (formUserCustomFields.Visible && (formUserCustomFields.BasicForm != null))
        {
            saved &= formUserCustomFields.BasicForm.SaveData(null, false);
        }

        // When first form saved successfully, try to save the second one
        if (saved && formUserSettingsCustomFields.Visible && (formUserSettingsCustomFields.BasicForm != null))
        {
            saved &= formUserSettingsCustomFields.BasicForm.SaveData(null, false);
        }

        // Both forms were saved correctly
        if (saved)
        {
            ShowChangesSaved();
        }
    }
}