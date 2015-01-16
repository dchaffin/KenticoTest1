using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.DocumentEngine;
using CMS.UIControls;

public partial class CMSSiteManager_Sites_Site_Edit_Cultures : SiteManagerPage
{
    #region "Variables"

    protected string siteName = null;
    private SiteInfo si = null;
    protected string currentValues = "";
    protected string defaultCulture = null;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get the site info
        si = SiteInfoProvider.GetSiteInfo(QueryHelper.GetInteger("siteId", 0));
        if (si != null)
        {
            bool multilingual = LicenseHelper.CheckFeature(URLHelper.GetDomainName(si.DomainName), FeatureEnum.Multilingual);
            bool cultureOnSite = CultureInfoProvider.IsCultureOnSite(CultureHelper.GetDefaultCulture(si.SiteName), si.SiteName);
            if (!multilingual && !cultureOnSite)
            {
                lnkAssignDefault.Text = GetString("sitecultures.assigntodefault");
                lnkAssignDefault.Visible = true;
                plcAll.Visible = false;
            }
            else
            {
                // Redirect only if cultures not exceeded => to be able to unassign
                if (!CultureInfoProvider.LicenseVersionCheck(si.DomainName, FeatureEnum.Multilingual, VersionActionEnum.Edit))
                {
                    LicenseHelper.CheckFeatureAndRedirect(URLHelper.GetDomainName(si.DomainName), FeatureEnum.Multilingual);
                }
            }

            lblAvialable.Text = GetString("site_edit_cultures.culturetitle");


            siteName = si.SiteName;

            // Store default culture (it can't be removed)
            defaultCulture = CultureHelper.GetDefaultCulture(siteName);

            // Get the active cultures from DB
            DataSet ds = CultureInfoProvider.GetCultures("CultureID IN (SELECT CultureID FROM CMS_SiteCulture WHERE SiteID = " + si.SiteID + ")", null, 0, "CultureCode");
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                currentValues = TextHelper.Join(";", SystemDataHelper.GetStringValues(ds.Tables[0], "CultureCode"));
            }

            if (!RequestHelper.IsPostBack())
            {
                uniSelector.Value = currentValues;
            }
        }

        uniSelector.ReturnColumnName = "CultureCode";
        uniSelector.OnSelectionChanged += uniSelector_OnSelectionChanged;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Check if site hasn't assigned more cultures than license approve
        if ((si != null) && !CultureInfoProvider.LicenseVersionCheck(si.DomainName, FeatureEnum.Multilingual, VersionActionEnum.Insert))
        {
            uniSelector.ButtonAddItems.Enabled = false;
        }
        else if ((si != null) && !CultureInfoProvider.LicenseVersionCheck(si.DomainName, FeatureEnum.Multilingual, VersionActionEnum.Edit))
        {
            ShowError(GetString("licenselimitation.siteculturesexceeded"));
            uniSelector.ButtonAddItems.Enabled = false;
        }
    }


    protected void uniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        bool relaodNeeded = false;

        // Remove old items
        string newValues = ValidationHelper.GetString(uniSelector.Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, currentValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems != null)
            {
                // Initialize tree provider
                TreeProvider tree = new TreeProvider();

                // Add all new items to site
                foreach (string item in newItems)
                {
                    string cultureCode = ValidationHelper.GetString(item, "");

                    // Get the documents assigned to the culture being removed
                    DataSet nodes = tree.SelectNodes(siteName, "/%", cultureCode, false, null, null, null, -1, false);
                    if (DataHelper.DataSourceIsEmpty(nodes))
                    {
                        CultureInfoProvider.RemoveCultureFromSite(cultureCode, siteName);

                        ShowChangesSaved();
                    }
                    else
                    {
                        relaodNeeded = true;

                        AddError(String.Format(GetString("site_edit_cultures.errorremoveculturefromsite"), cultureCode) + '\n', null);
                    }
                }
            }
        }

        // Catch license limitations Exception
        try
        {
            // Add new items
            items = DataHelper.GetNewItemsInList(currentValues, newValues);
            if (!String.IsNullOrEmpty(items))
            {
                string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (newItems != null)
                {
                    // Add all new items to site
                    foreach (string item in newItems)
                    {
                        string cultureCode = ValidationHelper.GetString(item, "");
                        CultureInfoProvider.AddCultureToSite(cultureCode, siteName);
                    }
                }

                ShowChangesSaved();
            }
            
        }
        catch (Exception ex)
        {
            relaodNeeded = true;
            ShowError(ex.Message);
        }
        
        if (relaodNeeded)
        {
            // Get the active cultures from DB
            DataSet ds = CultureInfoProvider.GetCultures("CultureID IN (SELECT CultureID FROM CMS_SiteCulture WHERE SiteID = " + si.SiteID + ")", null, 0, "CultureCode");
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                currentValues = TextHelper.Join(";", SystemDataHelper.GetStringValues(ds.Tables[0], "CultureCode"));
                uniSelector.Value = currentValues;
                uniSelector.Reload(true);
            }
        }
    }


    /// <summary>
    /// Removes all cultures from sites and assignes back default site culture.
    /// </summary>
    protected void AssignDefaultCulture(object sender, EventArgs e)
    {
        string culture = CultureHelper.GetDefaultCulture(si.SiteName);
        CultureInfoProvider.RemoveSiteCultures(si.SiteName);
        CultureInfoProvider.AddCultureToSite(culture, si.SiteName);
        lnkAssignDefault.Visible = false;
        ShowChangesSaved();
    }
}