using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSAdminControls_UI_UniGrid_Controls_ObjectMenu : CMSContextMenuControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterApplicationConstants(Page);

        // Get the object type
        string param = ContextMenu.Parameter;
        string objectType = null;
        bool groupObject = false;
        if (param != null)
        {
            string[] parms = param.Split(';');
            objectType = parms[0];
            if (parms.Length == 2)
            {
                groupObject = ValidationHelper.GetBoolean(parms[1], false);
            }
        }

        // Get empty info
        GeneralizedInfo obj = null;
        if (objectType != null)
        {
            obj = CMSObjectHelper.GetReadOnlyObject(objectType);
            // Get correct info for listings
            if (obj.TypeInfo.Inherited)
            {
                obj = CMSObjectHelper.GetReadOnlyObject(obj.TypeInfo.OriginalObjectType);
            }
        }

        if (obj == null)
        {
            Visible = false;
            return;
        }

        CurrentUserInfo curUser = CMSContext.CurrentUser;
        string curSiteName = CMSContext.CurrentSiteName;

        string menuId = ContextMenu.MenuID;

        // Relationships
        if (obj.TypeInfo.HasObjectRelationships)
        {
            iRelationships.ImageUrl = UIHelper.GetImageUrl(Page, "Design/Controls/UniGrid/Actions/Relationships.png");
            iRelationships.Text = ResHelper.GetString("General.Relationships");
            iRelationships.Attributes.Add("onclick", "ContextRelationships(GetContextMenuParameter('" + menuId + "'));");
        }
        else
        {
            iRelationships.Visible = false;
        }

        // Export
        if (obj.TypeInfo.AllowSingleExport)
        {
            if (curUser.IsAuthorizedPerResource("cms.globalpermissions", "ExportObjects", curSiteName))
            {
                iExport.ImageUrl = UIHelper.GetImageUrl(Page, "Design/Controls/UniGrid/Actions/ExportObject.png");
                iExport.Text = ResHelper.GetString("General.Export");
                iExport.Attributes.Add("onclick", "ContextExportObject(GetContextMenuParameter('" + menuId + "'), false);");
            }
            else
            {
                iExport.Visible = false;
            }

            if (obj.GUIDColumn != TypeInfo.COLUMN_NAME_UNKNOWN)
            {
                if (curUser.IsAuthorizedPerResource("cms.globalpermissions", "BackupObjects", curSiteName))
                {
                    iBackup.ImageUrl = UIHelper.GetImageUrl(Page, "Design/Controls/UniGrid/Actions/BackupObject.png");
                    iBackup.Text = ResHelper.GetString("General.Backup");
                    iBackup.Attributes.Add("onclick", "ContextExportObject(GetContextMenuParameter('" + menuId + "'), true);");
                }
                else
                {
                    iBackup.Visible = false;
                }

                if (curUser.IsAuthorizedPerResource("cms.globalpermissions", "RestoreObjects", curSiteName))
                {
                    iRestore.ImageUrl = UIHelper.GetImageUrl(Page, "Design/Controls/UniGrid/Actions/RestoreObject.png");
                    iRestore.Text = ResHelper.GetString("General.Restore");
                    iRestore.Attributes.Add("onclick", "ContextRestoreObject(GetContextMenuParameter('" + menuId + "'), true);");
                }
                else
                {
                    iRestore.Visible = false;
                }
            }
            else
            {
                iBackup.Visible = false;
                iRestore.Visible = false;
            }
        }
        else
        {
            iExport.Visible = false;
            iBackup.Visible = false;
            iRestore.Visible = false;
        }

        // Versioning
        if (obj.AllowRestore && UniGridFunctions.ObjectSupportsDestroy(obj) && curUser.IsAuthorizedPerObject(PermissionsEnum.Destroy, obj.ObjectType, curSiteName))
        {
            iDestroy.ImageUrl = UIHelper.GetImageUrl(Page, "Design/Controls/UniGrid/Actions/Delete.png");
            iDestroy.Text = ResHelper.GetString("security.destroy");
            iDestroy.Attributes.Add("onclick", "ContextDestroyObject_" + ClientID + "(GetContextMenuParameter('" + menuId + "'))");
        }
        else
        {
            iDestroy.Visible = false;
        }

        // Clonning
        if (obj.AllowClone)
        {
            iClone.ImageUrl = UIHelper.GetImageUrl(Page, "Design/Controls/UniGrid/Actions/Clone.png");
            iClone.Text = ResHelper.GetString("general.clone");
            iClone.Attributes.Add("onclick", "ContextCloneObject" + "(GetContextMenuParameter('" + menuId + "'))");
        }
        else
        {
            iClone.Visible = false;
        }

        bool ancestor = iRelationships.Visible;
        sep1.Visible = (iClone.Visible || iDestroy.Visible) && ancestor;
        ancestor |= (iClone.Visible || iDestroy.Visible);
        sep2.Visible = (iBackup.Visible || iRestore.Visible || iExport.Visible) && ancestor;

        Visible = iRelationships.Visible || iExport.Visible || iBackup.Visible || iDestroy.Visible || iClone.Visible;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (Visible)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
function ContextRelationships(definition) {
   var url = applicationUrl + 'CMSModules/AdminControls/Pages/ObjectRelationships.aspx?objecttype=' + escape(definition[0]) + '&objectid=' + escape(definition[1]);
        modalDialog(url, ""relationships"", 900, 600);
    }
    
function ContextExportObject(definition, backup) {
   var query = ''; 
   if (backup) {
       query += '&backup=true';
   }
   modalDialog(applicationUrl + 'CMSModules/ImportExport/SiteManager/ExportObject.aspx?objectType=' + escape(definition[0]) + '&objectId=' + definition[1] + query, 'ExportObject', 750, 200);
}

function ContextRestoreObject(definition, backup) {
    var query = '';
    if (backup) {
       query += '&backup=true';
    }
    modalDialog(applicationUrl + 'CMSModules/ImportExport/SiteManager/RestoreObject.aspx?objectType=' + escape(definition[0]) + '&objectId=' + definition[1] + query, 'RestoreObject', 750, 400);
}

function ContextCloneObject(definition) {
    modalDialog(applicationUrl + 'CMSModules/Objects/Dialogs/CloneObjectDialog.aspx?objectType=' + escape(definition[0]) + '&objectId=' + definition[1], 'CloneObject', 750, 400);
}");
            // Register general export scripts
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ObjectMenuExportScripts", sb.ToString(), true);

            sb = new StringBuilder();
            sb.Append(@"
function ContextDestroyObject_", ClientID, @"(definition)
{
   if(confirm('", ScriptHelper.GetString(ResHelper.GetString("objectversioning.destroyobjectconfirmation"), false), @"')) {
      if(UG_DestroyObj_", ContextMenu.ParentElementClientID, @") {
          var param = definition.toString().split(',');
          if((param != null) && (param.length == 2)) {
              UG_DestroyObj_", ContextMenu.ParentElementClientID, @"(param[1]);
          }
      }
   }
}");
            // Register destroy script for particular menu
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ObjectMenuDestroyScript_" + ClientID, sb.ToString(), true);
        }
    }
}