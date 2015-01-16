using System;

using CMS.GlobalHelper;
using CMS.IO;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.UIControls;

//[Title("", "", "newedit_transformation")]
public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_Transformation_Header : CMSModalDesignPage
{
    #region "Variables"

    private bool dialogMode = false;
    private int transformationId = 0;
    private string transformationName = null;

    private TransformationInfo transformation = null;

    #endregion


    #region "Page methods"

    protected override void OnPreInit(EventArgs e)
    {
        RequireSite = false;

        dialogMode = QueryHelper.GetBoolean("editonlycode", false);
        transformationId = QueryHelper.GetInteger("transformationid", 0);
        transformationName = QueryHelper.GetString("name", null);

        if (dialogMode)
        {
            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/TabsHeader.master";
        }
        else
        {
            CheckAccessToSiteManager();
        }

        base.OnPreInit(e);
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        InitEditedObject();
        InitMasterPage();
    }


    private void InitEditedObject()
    {
        if (PersistentEditedObject != null)
        {
            transformation = PersistentEditedObject as TransformationInfo;
            return;
        }

        if (transformationId > 0)
        {
            transformation = TransformationInfoProvider.GetTransformation(transformationId);
        }
        else
        {
            transformation = TransformationInfoProvider.GetTransformation(transformationName);
        }

        PersistentEditedObject = transformation;
    }


    private void InitMasterPage()
    {
        if (!dialogMode)
        {
            // Set breadcrumbs
            InitBreadcrumbs(2);
            SetBreadcrumb(0, GetString("DocumentType_Edit_Transformation_Edit.Transformations"), ResolveUrl("~/CMSModules/DocumentTypes/Pages/Development/DocumentType_Edit_Transformation_List.aspx?documenttypeid=" + transformation.TransformationClassID), "_parent", null);
            SetBreadcrumb(1, transformation.TransformationName, null, null, null);
        }
        else
        {
            SetTitle("Design/Selectors/selecttransformation.png", GetString("TransformationEdit.Title"), null, "helpTopic");

            string selector = QueryHelper.GetString("selectorid", string.Empty);
            
            if (!string.IsNullOrEmpty(selector) && RequestHelper.IsPostBack())
            {
                ScriptHelper.RegisterWOpenerScript(this);

                // Add selector update script
                string updateScript = string.Format(@"if (wopener && wopener.US_SelectNewValue_{0}) {{ wopener.US_SelectNewValue_{0}('{1}'); }}", selector, transformation.TransformationFullName);
                ScriptHelper.RegisterStartupScript(this, GetType(), "UpdateSelector", updateScript, true);
            }
        }

        // Set help
        string helpTopic = transformation.TransformationIsHierarchical ? "hierarchicaltransformation_tab" : "newedit_transformation";

        DataClassInfo classObj = DataClassInfoProvider.GetDataClass(transformation.TransformationClassID);
        if ((classObj != null) && classObj.ClassIsCustomTable)
        {
            helpTopic = "customtable_edit_newedit_transformation";
        }

        SetHelp(helpTopic, "helpTopic");

        // Set tabs
        InitTabs("t_edit_content");

        string url = "~/CMSModules/DocumentTypes/Pages/Development/DocumentType_Edit_Transformation_Edit.aspx" + URLHelper.Url.Query;
        if (transformationId == 0)
        {
            url = URLHelper.AddParameterToUrl(url, "name", transformation.TransformationFullName);
            url = URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash("?editonlycode=1"));
        }

        SetTab(0, (dialogMode && transformation.TransformationIsHierarchical) ? GetString("documenttype_edit_transformation_list.title") : GetString("general.general"), ResolveUrl(url), "SetHelpTopic('helpTopic', '" + helpTopic + "');");

        if (!dialogMode && !StorageHelper.IsExternalStorage(transformation.GetThemePath()))
        {
            SetTab(1, GetString("stylesheet.theme"), ResolveUrl("~/CMSModules/DocumentTypes/Pages/Development/DocumentType_Edit_Transformation_Theme.aspx" + URLHelper.Url.Query), "SetHelpTopic('helpTopic', '" + helpTopic + "');");
        }
    }

    #endregion
}