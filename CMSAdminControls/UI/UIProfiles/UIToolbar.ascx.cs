using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

using Group = CMS.UIControls.UniMenuConfig.Group;

public partial class CMSAdminControls_UI_UIProfiles_UIToolbar : CMSUserControl
{
    #region "Variables"

    protected string preselectedItem = null;

    protected string mElementName = null;
    protected string mModuleName = null;
    protected string mTargetFrameset = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the value that indicates whether scroll panel should be used
    /// </summary>
    public bool DisableScrollPanel
    {
        get
        {
            return uniMenu.DisableScrollPanel;
        }
        set
        {
            uniMenu.DisableScrollPanel = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether specific CSS classes should be generated for each group element.
    /// </summary>
    public bool GenerateElementCssClass
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the value that indicates whether Edit icon in dev. mode should be displayed
    /// </summary>
    public bool DisableEditIcon
    {
        get;
        set;
    }


    /// <summary>
    /// Returns the UIElementInfo representing the first button of first group displayed.
    /// </summary>
    public UIElementInfo FirstUIElement
    {
        get
        {
            return uniMenu.FirstUIElement;
        }
    }


    /// <summary>
    /// Returns the UIElementInfo representing the explicitly highlighted UI element.
    /// </summary>
    public UIElementInfo HighlightedUIElement
    {
        get
        {
            return uniMenu.HighlightedUIElement;
        }
    }


    /// <summary>
    /// Returns the UIElementInfo representing the selected (either explicitly highlighted or first) UI element.
    /// </summary>
    public UIElementInfo SelectedUIElement
    {
        get
        {
            return uniMenu.SelectedUIElement;
        }
    }


    /// <summary>
    /// Indicates whether at least one group with at least one button is rendered in the menu.
    /// </summary>
    public bool MenuEmpty
    {
        get
        {
            return uniMenu.MenuEmpty;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether first item should be highlighted.
    /// </summary>
    public bool HighlightFirstItem
    {
        get
        {
            return uniMenu.HighlightFirstItem;
        }
        set
        {
            uniMenu.HighlightFirstItem = value;
        }
    }


    /// <summary>
    /// Indicates whether to remember item which was last selected and select it again.
    /// </summary>
    public bool RememberSelectedItem
    {
        get
        {
            return uniMenu.RememberSelectedItem;
        }
        set
        {
            uniMenu.RememberSelectedItem = value;
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
            uniMenu.ModuleName = value;
        }
    }


    /// <summary>
    /// Target frameset in which the links are opened.
    /// </summary>
    public string TargetFrameset
    {
        get
        {
            return mTargetFrameset;
        }
        set
        {
            mTargetFrameset = value;
            uniMenu.TargetFrameset = value;
        }
    }


    /// <summary>
    /// Query parameter name for the preselection of the item.
    /// </summary>
    public string QueryParameterName
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether current UI culture is RTL.
    /// </summary>
    public bool IsRTL
    {
        get
        {
            return uniMenu.IsRTL;
        }
        set
        {
            uniMenu.IsRTL = value;
        }
    }

    #endregion


    #region "Custom events"

    /// <summary>
    /// Button filtered delegate.
    /// </summary>
    public delegate bool ButtonFilterEventHandler(object sender, UniMenuArgs eventArgs);


    /// <summary>
    /// Button created delegate.
    /// </summary>
    public delegate void ButtonCreatedEventHandler(object sender, UniMenuArgs eventArgs);


    /// <summary>
    /// Groups created delegate.
    /// </summary>
    public delegate void GroupsCreatedEventHandler(object sender, List<Group> groups);


    /// <summary>
    /// Button filtered event handler.
    /// </summary>
    public event ButtonFilterEventHandler OnButtonFiltered;


    /// <summary>
    /// Button creating event handler.
    /// </summary>
    public event ButtonCreatedEventHandler OnButtonCreating;


    /// <summary>
    /// Button created event handler.
    /// </summary>
    public event ButtonCreatedEventHandler OnButtonCreated;


    /// <summary>
    /// Groups created event handler.
    /// </summary>
    public event GroupsCreatedEventHandler OnGroupsCreated;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        // Handle the pre-selection
        preselectedItem = QueryHelper.GetString(QueryParameterName, "");
        if (preselectedItem.StartsWithCSafe("cms.", true))
        {
            preselectedItem = preselectedItem.Substring(4);
        }

        uniMenu.HighlightItem = preselectedItem;

        // If element name is not set, use root module element
        string elemName = ElementName;
        if (String.IsNullOrEmpty(elemName))
        {
            elemName = ModuleName.Replace(".", "");
        }

        // Get the UI elements
        DataSet ds = UIElementInfoProvider.GetChildUIElements(ModuleName, elemName);
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            FilterElements(ds);

            // Prepare the list of elements
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string url = ValidationHelper.GetString(dr["ElementTargetURL"], "");

                Group group = new Group();
                if (url.EndsWithCSafe("ascx"))
                {
                    group.ControlPath = url;
                }
                else
                {
                    group.UIElementID = ValidationHelper.GetInteger(dr["ElementID"], 0);
                }

                group.CssClass = "ContentMenuGroup";

                if (GenerateElementCssClass)
                {
                    string name = ValidationHelper.GetString(dr["ElementName"], String.Empty).Replace(".", String.Empty);
                    group.CssClass += " ContentMenuGroup" + name;
                    group.SeparatorCssClass = "UniMenuSeparator" + name;

                }

                group.Caption = ResHelper.LocalizeString(ValidationHelper.GetString(dr["ElementCaption"], ""));
                uniMenu.Groups.Add(group);
            }

            // Raise groups created event
            if (OnGroupsCreated != null)
            {
                OnGroupsCreated(this, uniMenu.Groups);
            }

            // Button created & filtered event handler
            if (OnButtonCreating != null)
            {
                uniMenu.OnButtonCreating += uniMenu_OnButtonCreating;
            }
            if (OnButtonCreated != null)
            {
                uniMenu.OnButtonCreated += uniMenu_OnButtonCreated;
            }
            if (OnButtonFiltered != null)
            {
                uniMenu.OnButtonFiltered += uniMenu_OnButtonFiltered;
            }
        }

        // Add editing icon in development mode
        if (SettingsKeyProvider.DevelopmentMode && CMSContext.CurrentUser.IsGlobalAdministrator && !DisableEditIcon)
        {
            ResourceInfo ri = ResourceInfoProvider.GetResourceInfo(ModuleName);
            if (ri != null)
            {
                ltlAfter.Text += String.Format("<div class=\"UIElementsLink\" >{0}</div>", UIHelper.GetResourceUIElementsLink(Page, ri.ResourceId));
            }
        }
    }


    /// <summary>
    /// Filters the dataset with UI Elements according to UI Profile of current user by default and according to custom event (if defined).
    /// </summary>
    protected void FilterElements(DataSet dsElements)
    {
        // For all tables in dataset
        foreach (DataTable dt in dsElements.Tables)
        {
            ArrayList deleteRows = new ArrayList();

            // Find rows to filter out
            foreach (DataRow dr in dt.Rows)
            {
                UIElementInfo uiElement = new UIElementInfo(dr);
                bool allowed = CMSContext.CurrentUser.IsAuthorizedPerUIElement(ModuleName, uiElement.ElementName);

                if (!allowed)
                {
                    deleteRows.Add(dr);
                }
            }

            // Delete the filtered rows
            foreach (DataRow dr in deleteRows)
            {
                dt.Rows.Remove(dr);
            }
        }
    }


    protected void uniMenu_OnButtonCreating(object sender, UniMenuArgs e)
    {
        if (OnButtonCreating != null)
        {
            OnButtonCreating(sender, e);
        }
    }


    protected void uniMenu_OnButtonCreated(object sender, UniMenuArgs e)
    {
        if (OnButtonCreated != null)
        {
            OnButtonCreated(sender, e);
        }
    }


    protected bool uniMenu_OnButtonFiltered(object sender, UniMenuArgs e)
    {
        if (OnButtonFiltered != null)
        {
            return OnButtonFiltered(sender, e);
        }
        return false;
    }

    #endregion
}