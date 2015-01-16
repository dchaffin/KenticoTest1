using System;

using CMS.Controls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.CMSHelper;
using System.Collections;

public partial class CMSFormControls_Filters_ObjectFilter : CMSAbstractBaseFilterControl
{
    #region "Private variables"

    private GeneralizedInfo currentObject = null;

    #endregion


    #region "Public properties"
    
    /// <summary>
    /// Gets or sets where condition to uni selector.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            if (plcParentObject.Visible)
            {
                return (ValidationHelper.GetString(parentSelector.Value, "") != "") ? currentObject.ParentIDColumn + " = " + parentSelector.Value : String.Empty;
            }
            else
            {
                return (currentObject.SiteIDColumn != TypeInfo.COLUMN_NAME_UNKNOWN) ? siteSelector.GetSiteWhereCondition(currentObject.SiteIDColumn) : String.Empty;
            }
        }
        set
        {
            base.WhereCondition = value;
        }
    }

    #endregion


    #region "Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        if (!StopProcessing)
        {
            SetupControl();
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        GeneralizedInfo parentObject = CMSObjectHelper.GetObject(currentObject.ParentObjectType);

        // Check if object or parent are site related
        if (((parentObject != null) && (parentObject.SiteIDColumn != TypeInfo.COLUMN_NAME_UNKNOWN)) || (currentObject.SiteIDColumn != TypeInfo.COLUMN_NAME_UNKNOWN))
        {
            // Initialize site selector
            siteSelector.Reload(false);
            siteSelector.PostbackOnDropDownChange = true;
            siteSelector.DropDownSingleSelect.SelectedIndexChanged += new EventHandler(DropDownSingleSelect_SelectedIndexChanged);

            // Check if global is allowed else select current site
            if (((parentObject != null) && parentObject.TypeInfo.SupportsGlobalObjects) || (currentObject.TypeInfo.SupportsGlobalObjects))
            {
                siteSelector.AllowGlobal = true;
            }
            else
            {
                if (!URLHelper.IsPostback())
                {
                    siteSelector.Value = CMSContext.CurrentSiteID;
                }
            }
        }
        else
        {
            plcSite.Visible = false;
        }
                
        if ((parentObject != null) && (parentObject.SiteIDColumn != TypeInfo.COLUMN_NAME_UNKNOWN))
        {
            // Get site where condition
            parentSelector.WhereCondition = siteSelector.GetSiteWhereCondition(parentObject.SiteIDColumn);
            parentSelector.Reload(true);
        }
    }


    /// <summary>
    /// Initialize filter controls.
    /// </summary>
    private void SetupControl()
    {
        if ((Parameters != null) && (Parameters["ObjectType"] != null))
        {
            // Get current object
            currentObject = CMSObjectHelper.GetObject(ValidationHelper.GetString(Parameters["ObjectType"], String.Empty));

            // Check if object is not null and has parent object
            if ((currentObject != null) && !String.IsNullOrEmpty(currentObject.ParentObjectType))
            {
                lblParent.ResourceString = "objecttype." + currentObject.ParentObjectType.Replace(".", "_");
                
                // Set parent object selector properties
                parentSelector.ObjectType = currentObject.ParentObjectType;
                parentSelector.DropDownSingleSelect.AutoPostBack = true;
                parentSelector.OnSelectionChanged += new EventHandler(parentSelector_OnSelectionChanged);
            }
            else
            {
                plcParentObject.Visible = false;
            }
        }
    }


    protected void DropDownSingleSelect_SelectedIndexChanged(object sender, EventArgs e)
    {
        FilterChanged = true;
        RaiseOnFilterChanged();
    }


    protected void parentSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        FilterChanged = true;
        RaiseOnFilterChanged();
    }

    #endregion
}
