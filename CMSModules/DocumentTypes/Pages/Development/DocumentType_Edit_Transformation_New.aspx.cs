using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.SettingsProvider;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;

[Breadcrumbs(2)]
[Breadcrumb(0, "documenttype_edit_transformation_edit.transformations", "DocumentType_Edit_Transformation_List.aspx?documenttypeid={?documenttypeid?}", null)]
[Breadcrumb(1, "documenttype_edit_transformation_edit.newtransformation")]
public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_Transformation_New : CMSModalPage
{
    #region "Variables"

    private bool closeOnSave = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// If true, control is used in site manager.
    /// </summary>
    public bool IsSiteManager
    {
        get
        {
            return filter.IsSiteManager;
        }
        set
        {
            filter.IsSiteManager = value;
        }
    }


    public bool DialogMode
    {
        get
        {
            return QueryHelper.GetBoolean("editonlycode", false);
        }
    }


    public bool IsCustomTable
    {
        get
        {
            return QueryHelper.GetBoolean("iscustomtable", false);
        }
    }


    public int ClassID
    {
        get
        {
            return QueryHelper.GetInteger("documenttypeid", 0);
        }
    }

    #endregion


    protected void Page_PreInit(object sender, EventArgs e)
    {
        if (DialogMode)
        {
            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master";
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check for UI permissions
        if (!CurrentUser.IsAuthorizedPerUIElement("CMS.Content", new string[] { "Design", "Design.WebPartProperties", "WebPartProperties.General", "WebPartProperties.NewTransformations" }, CMSContext.CurrentSiteName))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "Design.WebPartProperties;WebPartProperties.General;WebPartProperties.NewTransformations");
        }

        if (DialogMode)
        {
            // Check hash
            if (!QueryHelper.ValidateHash("hash", "saved;name;selectorid;sitemanager;selectedvalue;tabmode;aliaspath;instanceguid;splitmode"))
            {
                URLHelper.Redirect(ResolveUrl(string.Format("~/CMSMessages/Error.aspx?title={0}&text={1}", GetString("dialogs.badhashtitle"), GetString("dialogs.badhashtext"))));
            }

            CurrentMaster.PanelContent.CssClass = "PageContent";
            CurrentMaster.PanelFooter.CssClass = "FloatRight";

            /*  // Add save button
              LocalizedButton btnSave = new LocalizedButton
              {
                  ID = "btnSave",
                  ResourceString = "general.save",
                  EnableViewState = false,
                  CssClass = "SubmitButton"
              };
              btnSave.Click += new EventHandler(btnSave_Click);
              CurrentMaster.PanelFooter.Controls.Add(btnSave);

              // Add save & close button
              LocalizedButton btnSaveAndClose = new LocalizedButton
              {
                  ID = "btnSaveAndClose",
                  ResourceString = "general.saveandclose",
                  CssClass = "LongSubmitButton",
                  EnableViewState = false
              };
              btnSaveAndClose.Click += new EventHandler(btnSaveAndClose_Click);
              CurrentMaster.PanelFooter.Controls.Add(btnSaveAndClose);*/

            // Add close button
            CurrentMaster.PanelFooter.Controls.Add(new LocalizedButton
            {
                ID = "btnCancel",
                ResourceString = "general.close",
                EnableViewState = false,
                OnClientClick = "return CloseDialog();",
                CssClass = "SubmitButton"
            });
        }

        editElem.OnBeforeSave += new EventHandler(editElem_OnBeforeSave);
        editElem.OnAfterSave += new EventHandler(editElem_OnAfterSave);

        ucTransfCode.ClassID = ClassID;
        if (ClassID <= 0)
        {
            // Class is not defined, display doc type filter to select under which class the transformation should be crated
            filter.SelectedValue = QueryHelper.GetString("selectedvalue", null);
            filter.FilterMode = PortalObjectType.TRANSFORMATION;

            ucTransfCode.ClassID = filter.ClassId;

            CurrentMaster.DisplayControlsPanel = true;
        }
        else
        {
            CurrentMaster.DisplayControlsPanel = false;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        int classId = ClassID;
        if (classId <= 0)
        {
            classId = filter.ClassId;
        }

        DataClassInfo classInfo = DataClassInfoProvider.GetDataClass(classId);
        if ((classInfo != null) && ((classInfo.ClassIsCustomTable) || (classInfo.ClassIsDocumentType && classInfo.ClassIsCoupledClass)))
        {

            // Generate default
            HeaderAction generate = new HeaderAction()
             {
                 ControlType = HeaderActionTypeEnum.LinkButton,
                 Text = GetString("DocumentType_Edit_Transformation_Edit.ButtonDefault"),
                 Tooltip = GetString("transformationtypecode.generatetooltip"),
                 OnClientClick = "GenerateDefaultCode('default'); return false;",
                 ImageUrl = GetImageUrl("CMSModules/CMS_Class/Menu/generate.png"),
                 SmallImageUrl = GetImageUrl("CMSModules/CMS_Class/Menu/16/generate.rss"),

             };

            HeaderActions.ActionsList.Add(generate);

            if (ucTransfCode.IsAscx)
            {
                HeaderAction atom = new HeaderAction()
                {
                    ControlType = HeaderActionTypeEnum.LinkButton,
                    Text = GetString("transformationtypecode.atom"),
                    OnClientClick = "GenerateDefaultCode('atom'); return false;",
                    Tooltip = GetString("transformationtypecode.atomtooltip"),
                    ImageUrl = GetImageUrl("CMSModules/CMS_Class/Menu/atom.png"),
                    SmallImageUrl = GetImageUrl("CMSModules/CMS_Class/Menu/16/atom.png"),
                };

                HeaderAction xml = new HeaderAction()
                {
                    ControlType = HeaderActionTypeEnum.LinkButton,
                    Text = GetString("transformationtypecode.xml"),
                    Tooltip = GetString("transformationtypecode.xmltooltip"),
                    OnClientClick = "GenerateDefaultCode('xml'); return false;",
                    ImageUrl = GetImageUrl("CMSModules/CMS_Class/Menu/xml.png"),
                    SmallImageUrl = GetImageUrl("CMSModules/CMS_Class/Menu/16/xml.png"),
                };

                HeaderAction rss = new HeaderAction()
                {
                    ControlType = HeaderActionTypeEnum.LinkButton,
                    Text = GetString("transformationtypecode.rss"),
                    OnClientClick = "GenerateDefaultCode('rss'); return false;",
                    Tooltip = GetString("transformationtypecode.rsstooltip"),
                    ImageUrl = GetImageUrl("CMSModules/CMS_Class/Menu/rss.png"),
                    SmallImageUrl = GetImageUrl("CMSModules/CMS_Class/Menu/16/rss.png"),
                };

                generate.AlternativeActions.Add(atom);
                generate.AlternativeActions.Add(rss);
                generate.AlternativeActions.Add(xml);
            }
        }

        base.OnPreRender(e);

        CurrentMaster.Title.HelpTopicName = "newedit_transformation";
        CurrentMaster.Title.HelpName = "helpTopic";

        if (DialogMode)
        {
            CurrentMaster.Title.Breadcrumbs = null;
            CurrentMaster.Title.TitleText = GetString("DocumentType_Edit_Transformation_Edit.NewTransformation");
            CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_Transformation/transformation_new.png");
        }
        else if (IsCustomTable)
        {
            SetBreadcrumb(0, GetString("DocumentType_Edit_Transformation_Edit.Transformations"), ResolveUrl("~/CMSModules/CustomTables/CustomTable_Edit_Transformation_List.aspx?customtableid=" + ClassID), null, null);
            SetHelp("customtable_edit_newedit_transformation", "helpTopic");
        }
    }


    protected void btnSaveAndClose_Click(object sender, EventArgs e)
    {
        closeOnSave = true;
        editElem.SaveData(null);
    }


    protected void btnSave_Click(object sender, EventArgs e)
    {
        editElem.SaveData(null);
    }


    /// <summary>
    /// Registers script for transformation selector update. It's usable on dialog mode.
    /// </summary>
    /// <param name="closeDialog">If true, dialog is closed after save</param>
    private void RegisterChangeSelector()
    {
        string script = String.Empty;

        TransformationInfo ti = editElem.EditedObject as TransformationInfo;

        // Check for selector ID
        string selector = QueryHelper.GetString("selectorid", string.Empty);
        if (!string.IsNullOrEmpty(selector))
        {
            // Add selector refresh
            script = string.Format(@"if (wopener && wopener.US_SelectNewValue_{0}) {{wopener.US_SelectNewValue_{0}('{1}'); }}",
                              selector, ti.TransformationFullName);
        }

        if (closeOnSave)
        {
            script += "CloseDialog();";
        }

        if (script != String.Empty)
        {
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(String), "UpdateSelector", ScriptHelper.GetScript(script));
        }
    }


    protected void editElem_OnBeforeSave(object sender, EventArgs e)
    {
        TransformationInfo tinfo = editElem.EditedObject as TransformationInfo;
        if (tinfo != null)
        {
            if (ClassID > 0)
            {
                tinfo.TransformationClassID = ClassID;
            }
            else
            {
                tinfo.TransformationClassID = filter.ClassId;
            }
        }
    }


    protected void editElem_OnAfterSave(object sender, EventArgs e)
    {
        TransformationInfo ti = editElem.EditedObject as TransformationInfo;
        if (ti != null)
        {
            string tpar = (DialogMode) ? "name=" + ti.TransformationFullName : "transformationid=" + ti.TransformationID;

            string editPage = null;
            if (IsCustomTable)
            {
                editPage = URLHelper.GetAbsoluteUrl("~/CMSModules/CustomTables/CustomTable_Edit_Transformation_Frameset.aspx?" + tpar + "&" + Request.QueryString);
            }
            else
            {
                editPage = URLHelper.GetAbsoluteUrl("~/CMSModules/DocumentTypes/Pages/Development/DocumentType_Edit_Transformation_Frameset.aspx?" + tpar + "&" + Request.QueryString);
            }

            // If dialog mode redirect after, selector is updated
            if (DialogMode)
            {
                RegisterChangeSelector();
                ScriptHelper.RegisterStartupScript(Page, typeof(String), "RedirectParent", ScriptHelper.GetScript("location ='" + editPage + "';"));
            }
            else
            {
                URLHelper.Redirect(editPage);
            }
        }
    }
}