using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.SiteProvider;
using CMS.SettingsProvider;
using CMS.LicenseProvider;

public partial class CMSModules_WebAnalytics_Pages_Tools_Conversion_Edit : CMSModalPage
{
    #region "Variables"

    private bool modalDialog = false;

    // Help variable for set info label of UI form
    private string infoText = String.Empty;

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        // Licence for CaC
        if (DataHelper.GetNotEmpty(URLHelper.GetCurrentDomain(), "") != "")
        {
            LicenseHelper.CheckFeatureAndRedirect(URLHelper.GetCurrentDomain(), FeatureEnum.CampaignAndConversions);
        }

        // Checks all permissions for web analytics
        CMSWebAnalyticsPage.CheckAllPermissions();

        string conversionName = QueryHelper.GetString("conversionName", String.Empty);
        int conversionID = QueryHelper.GetInteger("conversionID", 0);
        ConversionInfo ci = null;

        if (conversionName != String.Empty)
        {
            // Try to check dialog mode
            conversionName = conversionName.Trim(';');
            ci = ConversionInfoProvider.GetConversionInfo(conversionName, CMSContext.CurrentSiteName);
        }

        if (conversionID != 0)
        {
            ci = ConversionInfoProvider.GetConversionInfo(conversionID);
        }

        // Test whether conversion is in current site, if not - test if user is authorized for conversion's site
        if (ci != null)
        {
            int siteID = CMSContext.CurrentSiteID;
            if (ci.ConversionSiteID != siteID)
            {
                if (!CMSContext.CurrentUser.IsInSite(SiteInfoProvider.GetSiteName(ci.ConversionSiteID)))
                {
                    RedirectToAccessDenied(GetString("conversion.currentsite"));
                    return;
                }
            }
        }

        if ((conversionName != String.Empty) && (ci == null))
        {
            // Set warning text
            infoText = String.Format(GetString("conversion.editedobjectnotexits"), HTMLHelper.HTMLEncode(conversionName));

            // Create new conversion info based on conversion name
            ci = new ConversionInfo();
            ci.ConversionName = conversionName;
            ci.ConversionDisplayName = conversionName;
        }

        modalDialog = QueryHelper.GetBoolean("modalDialog", false);
        if (modalDialog)
        {
            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master";
            if (ci != null)
            {
                CurrentMaster.Title.TitleText = GetString("analytics.conversion");
                CurrentMaster.Title.TitleImage = GetImageUrl("Objects/Analytics_Conversion/object.png");
            }
            else
            {
                CurrentMaster.Title.TitleText = GetString("conversion.conversion.new");
                CurrentMaster.Title.TitleImage = GetImageUrl("Objects/Analytics_Conversion/new.png");
            }
            SetDialogButtons();
        }

        if (ci != null)
        {
            EditedObject = ci;
        }

        // Must be called after the master page file is set
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if ((EditedObject == null) && !modalDialog)
        {
            string[,] breadcrumbs = new string[2, 3];
            breadcrumbs[0, 0] = GetString("conversion.conversion.list");
            breadcrumbs[0, 1] = "~/CMSModules/WebAnalytics/Pages/Tools/Conversion/List.aspx";
            breadcrumbs[1, 0] = GetString("conversion.conversion.new");

            // Set the title
            CurrentMaster.Title.Breadcrumbs = breadcrumbs;
        }

        // Set proper css style for modal dialog
        if (modalDialog)
        {
            pnlContent.CssClass = "PageContent";
        }

        // Set info label
        editElem.UIFormControl.ShowInformation(infoText);

        CurrentMaster.Title.HelpTopicName = "conversion_general";
        CurrentMaster.Title.HelpName = "helpTopic";
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (QueryHelper.GetBoolean("saved", false) && !URLHelper.IsPostback())
        {
            UpdateUniSelector(false);
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Adds scripts to update parent's uniselector (used in modal dialogs)
    /// </summary>
    /// <param name="closeOnSave">If true, window close JS is added</param>
    private void UpdateUniSelector(bool closeOnSave)
    {
        string selector = QueryHelper.GetString("selectorid", string.Empty);
        ConversionInfo info = editElem.UIFormControl.EditedObject as ConversionInfo;
        if (!string.IsNullOrEmpty(selector) && (info != null))
        {
            ScriptHelper.RegisterWOpenerScript(this);
            // Add selector refresh
            string script =
                string.Format(@"if (wopener) {{ wopener.US_SelectNewValue_{0}('{1}'); }}", selector, info.ConversionName);

            if (closeOnSave)
            {
                script += "CloseDialog();";
            }

            ScriptHelper.RegisterStartupScript(this, GetType(), "UpdateSelector", script, true);
        }
    }


    private void SetDialogButtons()
    {
        CurrentMaster.PanelFooter.CssClass = "FloatRight";

        // Add save button
        LocalizedButton btnSave = new LocalizedButton
                                      {
                                          ID = "btnSave",
                                          ResourceString = "general.save",
                                          EnableViewState = false,
                                          CssClass = "SubmitButton"
                                      };

        btnSave.Click += (sender, e) => editElem.Save(true);
        mCurrentMaster.PanelFooter.Controls.Add(btnSave);

        // Add save & close button
        LocalizedButton btnSaveAndCancel = new LocalizedButton
                                               {
                                                   ID = "btnSaveCancel",
                                                   ResourceString = "general.saveandclose",
                                                   EnableViewState = false,
                                                   CssClass = "LongSubmitButton"
                                               };
        btnSaveAndCancel.Click += (sender, e) =>
                                      {
                                          if (editElem.Save(false))
                                          {
                                              // Register script for save value to uniselector and close the window
                                              UpdateUniSelector(true);
                                          }
                                      };
        mCurrentMaster.PanelFooter.Controls.Add(btnSaveAndCancel);

        // Add close button every time
        mCurrentMaster.PanelFooter.Controls.Add(new LocalizedButton
                                                    {
                                                        ID = "btnCancel",
                                                        ResourceString = "general.close",
                                                        EnableViewState = false,
                                                        OnClientClick = "return CloseDialog();",
                                                        CssClass = "SubmitButton"
                                                    });
    }

    #endregion
}