using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.FormControls;

public partial class CMSModules_PortalEngine_FormControls_WebPartLayouts_WebPartLayoutSelector : FormEngineUserControl
{
    #region "Variables"

    private bool mShowNewItem = false;
    private bool mShowDefaultItem = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Value.
    /// </summary>
    public override object Value
    {
        get
        {
            return uniselect.Value;
        }
        set
        {
            uniselect.Value = value;
        }
    }


    /// <summary>
    /// Whether (New) item is shown.
    /// </summary>
    public bool ShowNewItem
    {
        get
        {
            return mShowNewItem;
        }
        set
        {
            mShowNewItem = value;
        }
    }


    /// <summary>
    /// Whehter (Defualt) item is shown.
    /// </summary>
    public bool ShowDefaultItem
    {
        get
        {
            return mShowDefaultItem;
        }
        set
        {
            mShowDefaultItem = value;
        }
    }


    /// <summary>
    /// Where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return uniselect.WhereCondition;
        }
        set
        {
            uniselect.WhereCondition = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Page load event.
    /// </summary>    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            uniselect.StopProcessing = true;
            return;
        }

        // Get icon
        uniselect.IconPath = GetImageUrl("Objects/CMS_WebPartLayout/object.png");

        string[,] specialFields = new string[2,2];

        if (ShowDefaultItem)
        {
            specialFields[0, 0] = GetString("WebPartPropertise.Default");
            specialFields[0, 1] = "|default|";
        }

        if (ShowNewItem)
        {
            specialFields[1, 0] = GetString("WebPartPropertise.New");
            specialFields[1, 1] = "|new|";
        }

        uniselect.SpecialFields = specialFields;
        uniselect.DropDownSingleSelect.AutoPostBack = true;
        uniselect.IsLiveSite = IsLiveSite;
    }


    /// <summary>
    /// On changed.
    /// </summary>
    protected void uniselect_OnSelectionChanged(object sender, EventArgs e)
    {
        RaiseOnChanged();
    }

    #endregion
}