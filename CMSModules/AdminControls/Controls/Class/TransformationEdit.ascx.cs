using System;
using System.Data;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_AdminControls_Controls_Class_TransformationEdit : CMSPreviewControl
{
    #region "Variables"

    protected int previewState = 0;
    protected bool startWithFullScreen = false;
    private TransformationInfo mTransformationInfo = null;
    HeaderAction generate = null;
    bool addGenerateItem = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Edited transformation.
    /// </summary>
    private TransformationInfo TransformationInfo
    {
        get
        {
            if (mTransformationInfo == null)
            {
                mTransformationInfo = editElem.EditedObject as TransformationInfo;
            }
            return mTransformationInfo;
        }
    }


    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return editMenuElem.MessagesPlaceHolder;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Init(object sender, EventArgs e)
    {
        // Page has been opened in CMSDesk and only transformation code editing is allowed        
        if (DialogMode)
        {
            // Sets dialog mode and validates input (return in case of error)
            if (SetDialogMode())
            {
                return;
            }
        }

        ucTransfCode.ClassID = TransformationInfo.TransformationClassID;
        ucTransfCode.TransformationName = TransformationInfo.TransformationName;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        MessagesPlaceHolder.WrapperControlClientID = pnlScreen.ClientID;
        previewState = GetPreviewStateFromCookies(TRANSFORMATION);

        #region "Generate default transformation actions"
        
        // Hide generate button DDL for code if not coupled document or custom table
        if (TransformationInfo != null)
        {
            DataClassInfo classInfo = DataClassInfoProvider.GetDataClass(TransformationInfo.TransformationClassID);
            if ((classInfo != null) && ((classInfo.ClassIsCustomTable) || (classInfo.ClassIsDocumentType && classInfo.ClassIsCoupledClass)))
            {
                addGenerateItem = true;
            }
        }

        if (addGenerateItem)
        {
            // Generate default
            generate = new HeaderAction()
            {
                ControlType = HeaderActionTypeEnum.LinkButton,
                Text = GetString("DocumentType_Edit_Transformation_Edit.ButtonDefault"),
                Tooltip = GetString("transformationtypecode.generatetooltip"),
                OnClientClick = "GenerateDefaultCode('default'); return false;",
                ImageUrl = GetImageUrl("CMSModules/CMS_Class/Menu/generate.png"),
                SmallImageUrl = GetImageUrl("CMSModules/CMS_Class/Menu/16/generate.png"),
                GenerateSeparatorBeforeAction = true,
            };

            editMenuElem.ObjectEditMenu.AddExtraAction(generate);
        }

        #endregion

        // Add preview action
        HeaderAction preview = new HeaderAction()
        {
            ControlType = HeaderActionTypeEnum.LinkButton,
            Text = GetString("general.preview"),
            OnClientClick = "performToolbarAction('split');return false;",
            ImageUrl = GetImageUrl("CMSModules/CMS_Content/EditMenu/Preview.png"),
            SmallImageUrl = GetImageUrl("CMSModules/CMS_Content/EditMenu/16/Preview.png"),
            Visible = (previewState == 0),
            Tooltip = GetString("preview.tooltip"),
            GenerateSeparatorBeforeAction = true
        };

        editMenuElem.ObjectEditMenu.AddExtraAction(preview);
        editMenuElem.ObjectEditMenu.PreviewMode = true;
        editMenuElem.MenuPanel.CssClass = "PreviewMenu";
        editMenuElem.ObjectManager.OnAfterAction += new EventHandler<SimpleObjectManagerEventArgs>(ObjectManager_OnAfterAction);

        ucTransfCode.FullscreenMode = (previewState != 0);

        // Set correct styles according to locking
        if (CMSObjectHelper.UseCheckinCheckout)
        {
            pnlScreen.CssClass = "PreviewDefaultContentLarge";
        }
        else
        {
            pnlScreen.CssClass = "PreviewDefaultContent";
        }

        // Register action script for dialog purposes
        if (DialogMode)
        {
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(String), "PreviewHierarchyPerformAction", ScriptHelper.GetScript("function actionPerformed(action) { if (action == 'saveandclose') { document.getElementById('" + hdnClose.ClientID + "').value = '1'; }; " + editMenuElem.ObjectManager.GetJSFunction(ComponentEvents.SAVE, null, null) + "; }"));
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Disable generate action if object cannot be edited
        if (addGenerateItem && !editMenuElem.ObjectManager.IsObjectChecked())
        {
            generate.Visible = false;
            addGenerateItem = false;
        }

        // Add extra actions for ascx type
        if (addGenerateItem && ucTransfCode.IsAscx)
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
                OnClientClick = "GenerateDefaultCode('xml'); return false;",
                Tooltip = GetString("transformationtypecode.xmltooltip"),
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

        startWithFullScreen = ((previewState != 0) && editMenuElem.ObjectManager.IsObjectChecked());

        if (ucTransfCode.TransformationType == TransformationTypeEnum.Html)
        {
            startWithFullScreen = false;
        }

        // Wrong calculation for these browsers, when div is hidden.
        bool hide = (BrowserHelper.IsSafari() || BrowserHelper.IsChrome());
        pnlContent.Attributes["style"] = (startWithFullScreen && !hide) ? "display:none" : "display:block";

        RegisterInitScripts(pnlContent.ClientID, editMenuElem.MenuPanel.ClientID, startWithFullScreen);
        RegisterResizeHeaders();
    }


    protected void ObjectManager_OnAfterAction(object sender, SimpleObjectManagerEventArgs e)
    {
        if (e.ActionName == ComponentEvents.SAVE)
        {
            if (DialogMode)
            {
                string script = String.Empty;
                string selector = QueryHelper.GetString("selectorid", string.Empty);
                if (!string.IsNullOrEmpty(selector))
                {
                    // Selects newly created container in the UniSelector
                    script = string.Format(@"if (wopener && wopener.US_SelectNewValue_{0}) {{wopener.US_SelectNewValue_{0}('{1}'); }}",
                                          selector, TransformationInfo.TransformationFullName);
                }

                if (ValidationHelper.GetBoolean(hdnClose.Value, false))
                {
                    script += "; CloseDialog();";
                }

                if (script != String.Empty)
                {
                    ScriptHelper.RegisterStartupScript(Page, typeof(string), "UpdateSelector", ScriptHelper.GetScript(script));
                }
            }

            // Clear warning message
            MessagesPlaceHolder.WarningText = "";
            RegisterRefreshScript();
        }

        if (DialogMode)
        {
            switch (e.ActionName)
            {
                case ComponentEvents.SAVE:
                case ComponentEvents.CHECKOUT:
                case ComponentEvents.UNDO_CHECKOUT:
                case ComponentEvents.CHECKIN:
                    ScriptHelper.RegisterStartupScript(Page, typeof(string), "wopenerRefresh", ScriptHelper.GetScript("if (wopener && wopener.refresh) { wopener.refresh(); }"));
                    break;
            }
        }

        if (e.ActionName == ComponentEvents.CHECKOUT)
        {
            ucTransfCode.ShowMessage();
        }

        if (!DialogMode && (e.ActionName != ComponentEvents.CHECKOUT))
        {
            ScriptHelper.RefreshTabHeader(Page, null);
        }
    }


    private bool SetDialogMode()
    {
        // Error message variable
        string errorMessage = null;

        // Check if document type is registered under current site
        DataSet ds = ClassSiteInfoProvider.GetClassSites("ClassID", "ClassID = " + TransformationInfo.TransformationClassID + " AND SiteID = " + CMSContext.CurrentSiteID, null, -1);
        if (DataHelper.DataSourceIsEmpty(ds) && !CurrentUser.UserSiteManagerAdmin)
        {
            // Set error message
            errorMessage = GetString("formcontrols_selecttransformation.classnotavailablesite").Replace("%%code%%", HTMLHelper.HTMLEncode(TransformationInfo.ClassName));
        }
        else
        {
            fDisplayName.EditingControl.Enabled = false;
        }

        // Hide panel Menu and write error message
        if (!String.IsNullOrEmpty(errorMessage))
        {
            ShowError(errorMessage);
            return true;
        }

        return false;
    }

    #endregion
}