using System;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_RelationshipNames_RelationshipName_General : SiteManagerPage
{
    #region "Protected variables"

    protected int relationshipNameId = 0;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initializes validators
        RequiredFieldValidatorCodeName.ErrorMessage = GetString("General.RequiresCodeName");
        RequiredFieldValidatorDisplayName.ErrorMessage = GetString("General.RequiresDisplayName");

        // Get ID of relationship name
        relationshipNameId = QueryHelper.GetInteger("relationshipnameid", 0);

        if (!RequestHelper.IsPostBack())
        {
            if (relationshipNameId > 0)
            {
                // Load data to form
                LoadData();

                if (QueryHelper.GetInteger("saved", 0) == 1)
                {
                    ShowChangesSaved();
                }
            }
        }
    }

    #endregion


    #region "Protected methods"

    /// <summary>
    /// Loads data of edited relationship name from DB into TextBoxes.
    /// </summary>
    protected void LoadData()
    {
        RelationshipNameInfo rni = RelationshipNameInfoProvider.GetRelationshipNameInfo(relationshipNameId);

        if (rni != null)
        {
            txtRelationshipNameCodeName.Text = rni.RelationshipName;
            txtRelationshipNameDisplayName.Text = rni.RelationshipDisplayName;

            if (!DataHelper.IsEmpty(rni.RelationshipAllowedObjects))
            {
                objectTypeSelector.ObjectType = rni.RelationshipAllowedObjects;
            }
        }
    }

    #endregion


    #region "Control events"

    /// <summary>
    /// Saves data of edited relationship name from TextBoxes into DB.
    /// </summary>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        // Finds whether required fields are not empty
        string result = new Validator().NotEmpty(txtRelationshipNameDisplayName.Text, GetString("General.RequiresDisplayName")).NotEmpty(txtRelationshipNameCodeName.Text, GetString("General.RequiresCodeName"))
            .IsCodeName(txtRelationshipNameCodeName.Text, GetString("general.invalidcodename"))
            .Result;

        if (result == string.Empty)
        {
            if (relationshipNameId > 0)
            {
                // Check the uniqueness of code name
                RelationshipNameInfo rni = RelationshipNameInfoProvider.GetRelationshipNameInfo(txtRelationshipNameCodeName.Text);
                if (rni == null || rni.RelationshipNameId == relationshipNameId)
                {
                    // Get relationshipname info by ID
                    rni = RelationshipNameInfoProvider.GetRelationshipNameInfo(relationshipNameId);
                    if (rni != null)
                    {
                        if (rni.RelationshipDisplayName != txtRelationshipNameDisplayName.Text)
                        {
                            // Refresh header
                            ScriptHelper.RefreshTabHeader(Page, null);
                        }

                        rni.RelationshipDisplayName = txtRelationshipNameDisplayName.Text;
                        rni.RelationshipName = txtRelationshipNameCodeName.Text;
                        rni.RelationshipAllowedObjects = objectTypeSelector.ObjectType;
                        // Save changes
                        RelationshipNameInfoProvider.SetRelationshipNameInfo(rni);

                        ShowChangesSaved();
                    }
                    else
                    {
                        ShowError(GetString("RelationshipNames.RelationshipNameDoesNotExists"));
                    }
                }
                else
                {
                    ShowError(GetString("RelationshipNames.RelationshipNameAlreadyExists"));
                }
            }
        }
        else
        {
            ShowError(result);
        }
    }

    #endregion
}