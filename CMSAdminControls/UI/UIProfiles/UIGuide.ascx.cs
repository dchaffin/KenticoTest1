using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSAdminControls_UI_UIProfiles_UIGuide : CMSUserControl
{
    #region "Variables"

    protected string mElementName = null;
    protected string mModuleName = null;

    private bool mModuleAvailabilityForSiteRequired = false;

    private int mColumns = 2;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets ArrayList of the guide parameters.
    /// </summary>
    public List<List<string>> GuideParameters
    {
        get
        {
            return guideElem.Parameters;
        }
    }


    /// <summary>
    /// Code name of the UI element.
    /// </summary>
    public string ElementName
    {
        get
        {
            return mElementName;
        }
        set
        {
            mElementName = value;
        }
    }


    /// <summary>
    /// Code name of the module.
    /// </summary>
    public string ModuleName
    {
        get
        {
            return mModuleName;
        }
        set
        {
            mModuleName = value;
        }
    }


    /// <summary>
    /// Number of displayed columns, by default set to 2.
    /// </summary>
    public int Columns
    {
        get
        {
            return mColumns;
        }
        set
        {
            mColumns = value;
        }
    }


    /// <summary>
    /// Gets the value which indicates whether guide contains some elements.
    /// </summary>
    public bool GuideEmpty
    {
        get
        {
            return (guideElem.Parameters.Count == 0);
        }
    }


    /// <summary>
    /// Indicates if site availability of the corresponding module (module with name in format "cms.[ElementName]") is required for each UI element in the menu. Takes effect only when corresponding module exists.
    /// </summary>
    public bool ModuleAvailabilityForSiteRequired
    {
        get
        {
            return mModuleAvailabilityForSiteRequired;
        }
        set
        {
            mModuleAvailabilityForSiteRequired = value;
        }
    }

    #endregion


    #region "Custom events"

    /// <summary>
    /// Guide item created delegate.
    /// </summary>
    public delegate List<string> GuideItemCreatedEventHandler(UIElementInfo uiElement, List<string> defaultItem);


    /// <summary>
    /// Guide item created event handler.
    /// </summary>
    public event GuideItemCreatedEventHandler OnGuideItemCreated;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(ModuleName))
        {
            // Reserve UI tabs
            DataRow sdr = SecurityHelper.ReserveSecurityLogItem("LoadUIGuide");

            // Get UI elements which represent the guide items
            DataSet ds = null;
            if (String.IsNullOrEmpty(ElementName))
            {
                ds = UIElementInfoProvider.GetUIMenuElements(ModuleName);
            }
            else
            {
                ds = UIElementInfoProvider.GetChildUIElements(ModuleName, ElementName);
            }

            // Create guide item collection
            CurrentUserInfo currentUser = CMSContext.CurrentUser;
            int i = 0;
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                var guideParams = new List<List<string>>(4);

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string elementName = ValidationHelper.GetString(dr["ElementName"], "");

                    if (currentUser.IsAuthorizedPerUIElement(ModuleName, elementName, ModuleAvailabilityForSiteRequired))
                    {
                        // Get parameters of the guide item
                        List<string> itemParams = GetGuideItemParameters(new UIElementInfo(dr));

                        // If item initialized, add it to the collection
                        if (itemParams != null)
                        {
                            guideParams.Add(itemParams);
                            i++;
                        }
                    }
                }

                // Initialize guide
                guideElem.Parameters = guideParams;
                guideElem.Columns = Columns;
            }

            // Log the security
            if (sdr != null)
            {
                SecurityHelper.SetLogItemData(sdr, currentUser.UserName, ModuleName, ElementName, i, CMSContext.CurrentSiteName);
            }
        }
    }


    /// <summary>
    /// Creates empty parameters list.
    /// </summary>
    public void InitEmptyParameters()
    {
        // Empty params
        guideElem.Parameters = new List<List<string>>(4);
        // Set column count
        guideElem.Columns = Columns;
    }


    /// <summary>
    /// Returns initialized array list with guide item parameters.
    /// </summary>
    /// <param name="uiElement">UI element data</param>        
    private List<string> GetGuideItemParameters(UIElementInfo uiElement)
    {
        // Ensure item caption
        string itemCaption = uiElement.ElementCaption;
        if (string.IsNullOrEmpty(itemCaption))
        {
            itemCaption = uiElement.ElementDisplayName;
        }
        itemCaption = ResHelper.LocalizeString(itemCaption);

        // Ensure item description
        string itemDescription = "";
        if (!String.IsNullOrEmpty(uiElement.ElementDescription))
        {
            itemDescription = ResHelper.LocalizeString(uiElement.ElementDescription);
        }
        else
        {
            string moduleName = ModuleName;
            if (moduleName.ToLowerCSafe().StartsWithCSafe("cms."))
            {
                moduleName = moduleName.Remove(0, 4);
            }
            itemDescription = GetString(String.Format("{0}.{1}.Description", moduleName, uiElement.ElementName));
        }

        // Ensure item URL
        string itemUrl = CMSContext.ResolveMacros(uiElement.ElementTargetURL);
        itemUrl = URLHelper.ResolveUrl(itemUrl);
        if (String.IsNullOrEmpty(itemUrl))
        {
            return null;
        }

        // Ensure item icon path      
        string itemBigIcon = "";
        string itemIcon = uiElement.ElementIconPath;
        if (!string.IsNullOrEmpty(itemIcon))
        {
            if (!ValidationHelper.IsURL(itemIcon))
            {
                int slashIndex = itemIcon.Replace('\\', '/').LastIndexOfCSafe('/');
                if ((slashIndex != -1) && (slashIndex < itemIcon.Length))
                {
                    // Trim file name of the small icon from the path
                    itemIcon = itemIcon.Remove(slashIndex);

                    // Get file name of the big icon
                    itemBigIcon = UIHelper.GetImagePath(Page, itemIcon + "/module.png", false, false);

                    // Check if exists
                    if (!FileHelper.FileExists(itemBigIcon))
                    {
                        itemBigIcon = UIHelper.GetImagePath(Page, itemIcon + "/object.png", false, false);
                        if (!File.Exists(Server.MapPath(itemBigIcon)))
                        {
                            // Use default big icon
                            itemBigIcon = "";
                        }
                    }
                }
            }
            else
            {
                itemBigIcon = itemIcon;
            }
        }

        // Use default big icon
        if (itemBigIcon == "")
        {
            itemBigIcon = GetImagePath("CMSModules/module.png");
        }

        var parameters = new List<string>(5);

        // Initialize guide item
        parameters.Add(itemBigIcon);
        parameters.Add(itemCaption);
        parameters.Add(itemUrl);
        parameters.Add(itemDescription);
        parameters.Add(uiElement.ElementName);

        // Handle additional initialization of the guide item
        if (OnGuideItemCreated != null)
        {
            parameters = OnGuideItemCreated(uiElement, parameters);
        }

        return parameters;
    }
}