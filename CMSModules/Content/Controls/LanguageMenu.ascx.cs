using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

using MenuItem = CMS.UIControls.UniMenuConfig.Item;
using SubMenuItem = CMS.UIControls.UniMenuConfig.SubItem;

public partial class CMSModules_Content_Controls_LanguageMenu : CMSUserControl
{
    #region "Variables"

    protected string mSelectedCulture = null;
    protected string currentSiteName = null;
    protected int culturesCount = 0;
    private bool? mUseSmallLanguageButton = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Culture to be considered as selected.
    /// </summary>
    public string SelectedCulture
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectedCulture"), CMSContext.CurrentUser.PreferredCultureCode);
        }
        set
        {
            SetValue("SelectedCulture", value);
        }
    }


    /// <summary>
    /// If false, compare separator and button is not shown.
    /// </summary>
    public bool DisplayCompare
    {
        get
        {
            return pnlCompare.Visible;
        }
        set
        {
            pnlCompare.Visible = value;
        }
    }


    /// <summary>
    /// ID of site, cultures belong to.
    /// </summary>
    public int SiteID
    {
        get;
        set;
    }


    /// <summary>
    /// When in compact mode, only one culture button is displayed at once and buttons are rendered as small in two rows.
    /// </summary>
    public bool CompactMode
    {
        get;
        set;
    }


    /// <summary>
    /// Maximum number of cultures according to CompactMode property.
    /// </summary>
    private int MaxCulturesInRow
    {
        get
        {
            return CompactMode ? 1 : 2;
        }
    }


    /// <summary>
    /// Indicates whether small buttons are to be used for flags.
    /// </summary>
    public bool UseSmallLanguageButton
    {
        get
        {
            if (mUseSmallLanguageButton == null)
            {
                mUseSmallLanguageButton = (CompactMode && culturesCount > 1);
            }

            return mUseSmallLanguageButton.Value;
        }
        set
        {
            mUseSmallLanguageButton = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterJQuery(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSAdminControls/UI/UniMenu/UniMenu.js");

        currentSiteName = (SiteID != 0) ? SiteInfoProvider.GetSiteName(SiteID) : CMSContext.CurrentSiteName;
        DataSet siteCulturesDS = CultureInfoProvider.GetSiteCultures(currentSiteName);
        if (!DataHelper.DataSourceIsEmpty(siteCulturesDS))
        {
            // Register jQuery cookie script
            ScriptHelper.RegisterJQueryCookie(Page);

            string defaultCulture = CultureHelper.GetDefaultCulture(currentSiteName);
            DataTable siteCultures = siteCulturesDS.Tables[0];

            culturesCount = siteCultures.Rows.Count;
            if ((culturesCount <= MaxCulturesInRow) && (culturesCount > 1))
            {
                // Disable cultures menu
                btnCultures.StopProcessing = true;

                for (int i = 0; i < culturesCount; i++)
                {
                    string cultureCode = siteCultures.Rows[i]["CultureCode"].ToString();
                    string cultureShortName = siteCultures.Rows[i]["CultureShortName"].ToString();
                    string cultureLongName = ResHelper.LocalizeString(siteCultures.Rows[i]["CultureName"].ToString());

                    if (CMSString.Compare(cultureCode, defaultCulture, true) == 0)
                    {
                        cultureLongName += " " + GetString("general.defaultchoice");
                    }

                    MenuItem item = new MenuItem();
                    item.Text = HTMLHelper.HTMLEncode(cultureShortName);
                    item.Tooltip = cultureLongName;
                    item.CssClass = "BigButton";
                    item.OnClientClick = "ChangeLanguage('" + cultureCode + "')";
                    item.ImagePath = GetFlagIconUrl(cultureCode, "48x48");
                    item.ImageAltText = cultureLongName;
                    item.ImageAlign = ImageAlign.Top;
                    item.MinimalWidth = 48;
                    buttons.Buttons.Add(item);

                    if (SelectedCulture.ToLowerCSafe() == cultureCode.ToLowerCSafe())
                    {
                        buttons.SelectedIndex = i;
                    }
                }
            }
            else
            {
                // Do not show culture selection buttons
                buttons.StopProcessing = true;

                CultureInfo ci = CultureInfoProvider.GetCultureInfo(SelectedCulture);

                MenuItem item = new MenuItem();
                item.Text = ci.CultureShortName;
                item.Tooltip = GetString(ci.CultureName);
                item.ImagePath = GetFlagIconUrl(SelectedCulture, UseSmallLanguageButton ? "16x16" : "48x48");
                item.ImageAltText = GetString(ci.CultureName);

                SetStyles(item);

                // Generate submenu only if more cultures to choose from
                if (culturesCount > 1)
                {
                    foreach (DataRow row in siteCultures.Rows)
                    {
                        string cultureCode = row["CultureCode"].ToString();
                        string cultureShortName = row["CultureShortName"].ToString();
                        string cultureLongName = GetString(row["CultureName"].ToString());

                        if (CMSString.Compare(cultureCode, defaultCulture, true) == 0)
                        {
                            cultureLongName += " " + GetString("general.defaultchoice");
                        }

                        string flagUrl = GetFlagIconUrl(cultureCode, "16x16");
                        string flagBigUrl = GetFlagIconUrl(cultureCode, "48x48");
                        SubMenuItem menuItem = new SubMenuItem()
                        {
                            Text = cultureLongName,
                            Tooltip = cultureLongName,
                            ImagePath = flagUrl,
                            ImageAltText = cultureShortName,
                            OnClientClick = String.Format("CMSUniMenu.ChangeButton(##BUTTON##, {0}, {1}); ChangeLanguage({2});", ScriptHelper.GetString(cultureShortName), ScriptHelper.GetString(ResolveUrl(flagBigUrl)), ScriptHelper.GetString(cultureCode))
                        };

                        item.SubItems.Add(menuItem);
                    }
                }

                btnCultures.Buttons.Add(item);
            }

            if (culturesCount > 1)
            {
                string compare = GetString("SplitMode.Compare");

                // Split mode button
                MenuItem splitItem = new MenuItem();
                splitItem.Text = compare;
                splitItem.Tooltip = GetString("SplitMode.CompareLangVersions");
                splitItem.OnClientClick = "ChangeSplitMode()";
                splitItem.ImagePath = GetImageUrl("CMSModules/CMS_Content/Menu/Compare.png");
                splitItem.ImageAltText = compare;
                splitItem.AllowToggle = true;
                splitItem.IsToggled = CMSContext.DisplaySplitMode;

                SetStyles(splitItem);

                splitView.Buttons.Add(splitItem);
            }
            else
            {
                splitView.StopProcessing = true;
            }
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Applies styles to menu item according to CompactMode property.
    /// </summary>
    /// <param name="menuItem">Menu item to apply styles to.</param>
    private void SetStyles(MenuItem menuItem)
    {
        if (UseSmallLanguageButton)
        {
            menuItem.CssClass = "SmallButton";
            menuItem.ImageAlign = ImageAlign.AbsMiddle;
            menuItem.MinimalWidth = 80;
        }
        else
        {
            menuItem.CssClass = "BigButton";
            menuItem.ImageAlign = ImageAlign.Top;
            menuItem.MinimalWidth = 48;
        }
    }

    #endregion
}