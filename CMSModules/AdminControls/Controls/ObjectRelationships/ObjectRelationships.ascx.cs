using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using System.Collections;

using CMS.CMSHelper;
using CMS.EmailEngine;
using CMS.ExtendedControls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.PortalEngine;
using CMS.Scheduler;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebFarmSync;
using CMS.WorkflowEngine;

public partial class CMSModules_AdminControls_Controls_ObjectRelationships_ObjectRelationships : CMSUserControl
{
    #region "Private fields"

    /// <summary>
    /// Table of the registered object types [objectType -> true]
    /// </summary>
    private Hashtable registeredObjects = new Hashtable();

    private TranslationHelper th = null;

    private bool loaded = false;

    private GeneralizedInfo mObject = null;
    private GeneralizedInfo mRelatedObject = null;

    private string mObjectType = null;
    private int mObjectID = -1;
    private string[] mObjectTypes = null;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Returns true if the left side is active.
    /// </summary>
    private bool ActiveLeft
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["ActiveLeft"], false);
        }
        set
        {
            ViewState["ActiveLeft"] = value;
        }
    }


    /// <summary>
    /// Collection of available object types.
    /// </summary>
    private string[] ObjectTypes
    {
        get
        {
            if (mObjectTypes == null)
            {
                return mObjectTypes =
                       new string[]
                           {
                               FormObjectType.BIZFORM,
                               PredefinedObjectType.NEWSLETTER,
                               PredefinedObjectType.NEWSLETTERISSUE,
                               PredefinedObjectType.NEWSLETTERSUBSCRIBER,
                               PredefinedObjectType.NEWSLETTERTEMPLATE,
                               PredefinedObjectType.FORUMGROUP,
                               PredefinedObjectType.FORUM,
                               SiteObjectType.ROLE,
                               SchedulerObjectType.SCHEDULEDTASK,
                               EmailObjectType.EMAILTEMPLATE,
                               WorkflowObjectType.WORKFLOWSCOPE,
                               PredefinedObjectType.ORDER,
                               PredefinedObjectType.SHIPPINGOPTION,
                               PredefinedObjectType.PAYMENTOPTION,
                               PredefinedObjectType.POLL,
                               PredefinedObjectType.REPORTCATEGORY,
                               PredefinedObjectType.REPORT,
                               SiteObjectType.USER,
                               SiteObjectType.SITE,
                               WebFarmObjectType.WEBFARMSERVER,
                               SiteObjectType.CSSSTYLESHEET,
                               SiteObjectType.COUNTRY,
                               SiteObjectType.CULTURE,
                               SiteObjectType.DOCUMENTTYPE,
                               EmailObjectType.EMAILTEMPLATE,
                               SiteObjectType.FORMUSERCONTROL,
                               SiteObjectType.INLINECONTROL,
                               SiteObjectType.RESOURCE,
                               PortalObjectType.PAGELAYOUT,
                               PortalObjectType.PAGETEMPLATE,
                               SiteObjectType.RELATIONSHIPNAME,
                               SiteObjectType.UICULTURE,
                               PortalObjectType.WEBPARTCONTAINER,
                               PortalObjectType.WEBPART,
                               LicenseObjectType.LICENSEKEY,
                               PredefinedObjectType.CUSTOMER,
                               PredefinedObjectType.SKU,
                               PredefinedObjectType.OPTIONCATEGORY,
                               PredefinedObjectType.MANUFACTURER,
                               PredefinedObjectType.SUPPLIER,
                               PredefinedObjectType.DISCOUNTCOUPON,
                               PredefinedObjectType.DISCOUNTLEVEL,
                               PredefinedObjectType.DEPARTMENT,
                               PredefinedObjectType.TAXCLASS,
                               PredefinedObjectType.CURRENCY,
                               PredefinedObjectType.EXCHANGETABLE,
                               PredefinedObjectType.ORDERSTATUS,
                               PredefinedObjectType.PUBLICSTATUS,
                               PredefinedObjectType.INTERNALSTATUS
                           };
            }

            return mObjectTypes;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Current object.
    /// </summary>
    public GeneralizedInfo Object
    {
        get
        {
            if (mObject == null)
            {
                mObject = CMSObjectHelper.GetObjectById(ObjectType, ObjectID);
            }

            return mObject;
        }
    }


    /// <summary>
    /// Related object type.
    /// </summary>
    public GeneralizedInfo RelatedObject
    {
        get
        {
            if (mRelatedObject == null)
            {
                string selected = drpRelatedObjType.SelectedValue;
                if (!String.IsNullOrEmpty(selected))
                {
                    mRelatedObject = CMSObjectHelper.GetReadOnlyObject(selected);
                }
            }

            return mRelatedObject;
        }
    }


    /// <summary>
    /// Type of the current object.
    /// </summary>
    public string ObjectType
    {
        get
        {
            return mObjectType;
        }
        set
        {
            mObjectType = value;
        }
    }


    /// <summary>
    /// ID of the current object.
    /// </summary>
    public int ObjectID
    {
        get
        {
            return mObjectID;
        }
        set
        {
            mObjectID = value;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        LoadData();
    }


    protected override void OnPreRender(EventArgs e)
    {
        int selectedSiteId = ValidationHelper.GetInteger(siteSelector.Value, 0);

        pnlNew.Visible = (!pnlSite.Visible || (selectedSiteId >= 0)) && (RelatedObject != null);

        string safeObjectType = null;

        string activeType = null;
        string currentType = null;

        // Initialize form labels
        if (Object != null)
        {
            safeObjectType = Object.ObjectType.Replace(".", "_");
            currentType = " (" + GetString("ObjectType." + safeObjectType) + ")";
        }

        if (RelatedObject != null)
        {
            safeObjectType = RelatedObject.ObjectType.Replace(".", "_");
            activeType = " (" + GetString("ObjectType." + safeObjectType) + ")";
        }

        // Setup link texts
        imgNewRelationship.ImageUrl = GetImageUrl("CMSModules/CMS_Content/Properties/addrelationship.png");
        lnkNewRelationship.Text = GetString("ObjRelationship.New");

        if (pnlAddNew.Visible)
        {
            btnSwitchSides.Text = GetString("Relationship.SwitchSides");

            btnOk.Text = GetString("General.Save");
            btnAnother.Text = GetString("General.SaveAndAnother");
            btnCancel.Text = GetString("General.Cancel");

            leftCell.Text = GetString("ObjRelationship.LeftSide");
            rightCell.Text = GetString("ObjRelationship.RightSide");
            middleCell.Text = GetString("Relationship.RelationshipName");

            // Handle the active items
            if (ActiveLeft)
            {
                leftCell.Text += activeType;
                rightCell.Text += currentType;

                selLeftObj.Visible = true;
                lblLeftObj.Visible = false;

                selRightObj.Visible = false;
                lblRightObj.Visible = true;
            }
            else
            {
                leftCell.Text += currentType;
                rightCell.Text += activeType;

                selLeftObj.Visible = false;
                lblLeftObj.Visible = true;

                selRightObj.Visible = true;
                lblRightObj.Visible = false;
            }
        }
        else
        {
            if (!loaded)
            {
                gridItems.ReloadData();
            }
        }

        base.OnPreRender(e);
    }


    public void LoadData()
    {
        if (!RequestHelper.IsPostBack())
        {
            // Initialize controls
            SetupControl();
        }

        // Init events
        gridItems.OnAction += gridItems_OnAction;
        gridItems.OnBeforeDataReload += gridItems_OnBeforeDataReload;

        gridItems.WhereCondition = GetWhereCondition(drpRelatedObjType.SelectedItem != null ? drpRelatedObjType.SelectedItem.Value : null);

        // Init site selector
        siteSelector.OnSelectionChanged += siteSelector_OnSelectionChanged;
        siteSelector.DropDownSingleSelect.AutoPostBack = true;

        siteSelector.SpecialFields = new string[,]
                                         {
                                             { GetString("general.selectall"), "-1" },
                                             { GetString("general.globalobjects"), "0" }
                                         };

        // Display items that are available
        if (pnlAddNew.Visible)
        {
            DisplayAvailableItems();
        }

        ControlsHelper.RegisterPostbackControl(btnOk);
        ControlsHelper.RegisterPostbackControl(btnCancel);
    }

    #endregion


    #region "Grid view handling"

    protected void gridItems_OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            case "delete":
                // Look for info on relationship being removed
                int leftObjId = ValidationHelper.GetInteger(Request.Params["leftObjId"], -1);
                string leftObjType = ValidationHelper.GetString(Request.Params["leftObjType"], "");
                int relationshipId = ValidationHelper.GetInteger(Request.Params["relationshipId"], -1);
                int rightObjId = ValidationHelper.GetInteger(Request.Params["rightObjId"], -1);
                string rightObjType = ValidationHelper.GetString(Request.Params["rightObjType"], "");

                // Remove the relationship if all the necessary information available
                if ((leftObjId > -1) && (leftObjType.Trim() != "") && (relationshipId > -1) && (rightObjId > -1) && (rightObjType.Trim() != ""))
                {
                    ObjectRelationshipInfoProvider.RemoveRelationship(leftObjId, leftObjType, rightObjId, rightObjType, relationshipId);
                }

                // Reload the data
                gridItems.ReloadData();
                break;
        }
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Refreshes the selection of site.
    /// </summary>
    protected void RefreshNewSiteSelection()
    {
        if (RelatedObject != null)
        {
            if ((RelatedObject.SiteIDColumn != TypeInfo.COLUMN_NAME_UNKNOWN) && (CMSObjectHelper.GetSiteBindingObject(RelatedObject) == null))
            {
                pnlSite.Visible = true;

                siteSelector_OnSelectionChanged(null, null);
            }
            else
            {
                pnlSite.Visible = false;
            }
        }
    }


    protected void lnkNewRelationship_Click(object sender, EventArgs e)
    {
        // Hide and disable unused controls
        pnlEditList.Visible = false;
        pnlAddNew.Visible = true;

        RefreshNewSiteSelection();

        // Initialize drop-down list with available relationship types
        DisplayAvailableRelationships();

        // Initialize drop=down list with available relationship items
        DisplayAvailableItems();

        // Supply the current object name
        lblLeftObj.Text = Object.ObjectDisplayName;
        lblRightObj.Text = Object.ObjectDisplayName;
    }


    protected void btnSwitchSides_Click(object sender, EventArgs e)
    {
        bool newActiveLeft = !ActiveLeft;
        if (newActiveLeft)
        {
            selLeftObj.Value = selRightObj.Value;
        }
        else
        {
            selRightObj.Value = selLeftObj.Value;
        }

        ActiveLeft = newActiveLeft;

        // Display the items that are available
        DisplayAvailableItems();
    }


    protected void drpRelatedObjType_SelectedIndexChanged(object sender, EventArgs e)
    {
        RefreshNewSiteSelection();
    }


    protected void btnAnother_Click(object sender, EventArgs e)
    {
        // Add the relationship between objects
        if (AddRelationship())
        {
            string safeObjectType = RelatedObject.ObjectType.Replace(".", "_");
            string activeType = GetString("ObjectType." + safeObjectType);

            string name = ActiveLeft ? selLeftObj.DropDownSingleSelect.SelectedItem.Text : selRightObj.DropDownSingleSelect.SelectedItem.Text;

            lblInfo.Text = String.Format(ResHelper.GetString("Relationship.Saved"), activeType, name);
            lblInfo.Visible = true;
        }
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        // Add the relationship between objects
        if (AddRelationship())
        {
            // Load the list dialog
            pnlEditList.Visible = true;
            pnlAddNew.Visible = false;
            drpRelatedObjType.Enabled = true;

            // Reload the data
            gridItems.ReloadData();
        }
    }


    protected void btnCancel_Click(object sender, EventArgs e)
    {
        // Load the list dialog
        pnlEditList.Visible = true;
        pnlAddNew.Visible = false;
        drpRelatedObjType.Enabled = true;

        // Reload the data
        gridItems.ReloadData();
    }


    /// <summary>
    /// Handles site selection change event.
    /// </summary>
    protected void siteSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        // If the new relationship is being added
        if (pnlAddNew.Visible)
        {
            DisplayAvailableItems();
        }
        else
        {
            // Reload the data
            gridItems.ReloadData();
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Reloads the data for the UniGrid control displaying the objects related to the current object.
    /// </summary>
    private void gridItems_OnBeforeDataReload()
    {
        loaded = true;

        registeredObjects.Clear();

        // Prepare the translations table
        th = new TranslationHelper();
        th.UseDisplayNameAsCodeName = true;
    }


    /// <summary>
    /// Inserts the new relationship according the selected values.
    /// </summary>
    private bool AddRelationship()
    {
        if (ObjectID > 0)
        {
            if (drpRelationship.SelectedItem == null)
            {
                lblError.Text = GetString("ObjRelationship.MustSelect");
                lblError.Visible = true;
                return false;
            }

            // Get information on type of the selected relationship
            int selectedRelationshipId = ValidationHelper.GetInteger(drpRelationship.SelectedItem.Value, -1);
            string selectedObjType = null;

            // If the main objectis on the left side selected object is taken from rifht drop-down list
            bool currentOnLeft = !ActiveLeft;
            int selectedObjId = currentOnLeft ? ValidationHelper.GetInteger(selRightObj.Value, -1) : ValidationHelper.GetInteger(selLeftObj.Value, -1);

            // Get information on type of the selected object
            selectedObjType = drpRelatedObjType.SelectedItem.Value;

            // If all the necessary information are present
            if ((selectedObjId <= 0) || (selectedRelationshipId <= 0) || (selectedObjType == null))
            {
                lblError.Text = GetString("ObjRelationship.MustSelect");
                lblError.Visible = true;
                return false;
            }

            if (currentOnLeft)
            {
                ObjectRelationshipInfoProvider.AddRelationship(ObjectID, ObjectType, selectedObjId, selectedObjType, selectedRelationshipId);
                return true;
            }
            else
            {
                ObjectRelationshipInfoProvider.AddRelationship(selectedObjId, selectedObjType, ObjectID, ObjectType, selectedRelationshipId);
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    private void SetupControl()
    {
        // Information on current object supplied
        if ((ObjectID != 0) && (ObjectType != null))
        {
            // Fill in the available objects into the filter
            DisplayAvailableObjects();
        }
        else
        {
            lblError.Visible = true;
            lblError.Text = "[ObjectRelationships.ascx] SetupControl()- ObjectID or ObjectType wasn't initialized.";
        }
    }


    /// <summary>
    /// Gets the where condition for the selected object type.
    /// </summary>
    /// <param name="selectedObjectType">Selected object type</param>
    public string GetWhereCondition(string selectedObjectType)
    {
        if (Object != null)
        {
            string where = null;

            if (RelatedObject != null)
            {
                // Get the site name

                if ((RelatedObject.SiteIDColumn != TypeInfo.COLUMN_NAME_UNKNOWN) && (CMSObjectHelper.GetSiteBindingObject(RelatedObject) == null))
                {
                    if (siteSelector.DropDownSingleSelect.Items.Count == 0)
                    {
                        siteSelector.Value = CMSContext.CurrentSiteID;
                    }

                    if (siteSelector.HasData)
                    {
                        // Set the site name for registration
                        int selectedSiteId = ValidationHelper.GetInteger(siteSelector.Value, 0);
                        if (selectedSiteId >= 0)
                        {
                            string siteWhere = QueryProvider.GetQuery(RelatedObject.ObjectType + ".selectall", RelatedObject.IDColumn, SqlHelperClass.GetSiteIDWhereCondition(RelatedObject.SiteIDColumn, selectedSiteId), null, 0);

                            // Where condition for the left object
                            string rightWhere = ObjectRelationshipInfoProvider.GetWhereCondition(ObjectID, ObjectType, 0, false, true, selectedObjectType);
                            rightWhere += " AND RelationshipLeftObjectID IN (" + siteWhere + ")";

                            // Where condition for the left object
                            string leftWhere = ObjectRelationshipInfoProvider.GetWhereCondition(ObjectID, ObjectType, 0, true, false, selectedObjectType);
                            leftWhere += " AND RelationshipRightObjectID IN (" + siteWhere + ")";

                            // --- Add site conditions here

                            where = SqlHelperClass.AddWhereCondition(leftWhere, rightWhere, "OR");
                        }
                    }
                }
            }

            if (String.IsNullOrEmpty(where))
            {
                // Get using regular where
                where = ObjectRelationshipInfoProvider.GetWhereCondition(ObjectID, ObjectType, 0, true, true, selectedObjectType);
            }

            return where;
        }

        return null;
    }


    /// <summary>
    /// Fills the given drop-down list with the available relationship types.
    /// </summary>
    private void DisplayAvailableRelationships()
    {
        if (drpRelationship.Items.Count == 0)
        {
            // Get the relationships from DB
            DataSet ds = RelationshipNameInfoProvider.GetAllRelationshipNames(CMSContext.CurrentSite.SiteID, "RelationshipAllowedObjects LIKE '%;##OBJECTS##;%'");
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    drpRelationship.Items.Add(new ListItem(dr["RelationshipDisplayName"].ToString(), dr["RelationshipNameID"].ToString()));
                }

                drpRelationship.Enabled = true;
            }
            else
            {
                drpRelationship.Items.Add(new ListItem(GetString("General.NoneAvailable"), ""));
                drpRelationship.Enabled = false;
            }
        }
    }


    /// <summary>
    /// Fills given drop-down list with the items of particular type.
    /// </summary>
    private void DisplayAvailableItems()
    {
        if (RelatedObject != null)
        {
            // Prepare the site where
            string where = null;
            if (RelatedObject.SiteIDColumn != TypeInfo.COLUMN_NAME_UNKNOWN)
            {
                int selectedSiteId = -1;

                // Add where 
                if (CMSObjectHelper.GetSiteBindingObject(RelatedObject) == null)
                {
                    selectedSiteId = ValidationHelper.GetInteger(siteSelector.Value, -1);
                }

                if (selectedSiteId == 0)
                {
                    where = RelatedObject.SiteIDColumn + " IS NULL";
                }
                else
                {
                    where = RelatedObject.SiteIDColumn + " = " + selectedSiteId;
                }
            }

            // Load the object selectors
            if (ActiveLeft)
            {
                // Active left selector
                selLeftObj.Enabled = true;
                selLeftObj.ObjectType = RelatedObject.ObjectType;
                selLeftObj.WhereCondition = where;
                selLeftObj.Reload(true);

                if (!selLeftObj.HasData)
                {
                    selLeftObj.DropDownSingleSelect.Items.Add(new ListItem(GetString("General.NoneAvailable"), ""));
                    selLeftObj.Enabled = false;
                }
            }
            else
            {
                // Active right selector
                selRightObj.Enabled = true;
                selRightObj.ObjectType = RelatedObject.ObjectType;
                selRightObj.WhereCondition = where;
                selRightObj.Reload(true);

                if (!selRightObj.HasData)
                {
                    selRightObj.DropDownSingleSelect.Items.Add(new ListItem(GetString("General.NoneAvailable"), ""));
                    selRightObj.Enabled = false;
                }
            }
        }
    }


    /// <summary>
    /// Fills given drop-down list with the available object types.
    /// </summary>
    private void DisplayAvailableObjects()
    {
        // For each type of available objects create a new item and insert it to the list
        if (ObjectTypes != null)
        {
            // All selection
            ListItem li = new ListItem(ResHelper.GetString("General.SelectAnything"), "");
            drpRelatedObjType.Items.Add(li);

            SortedDictionary<string, string> types = new SortedDictionary<string, string>();

            // Prepare the list of items
            foreach (string objType in ObjectTypes)
            {
                types[GetString("ObjectTasks." + objType.Replace(".", "_").Replace("#", "_"))] = objType;
            }

            // Fill in the list
            foreach (KeyValuePair<string, string> item in types)
            {
                string objType = item.Value;

                li = new ListItem(item.Key, objType);
                drpRelatedObjType.Items.Add(li);

                // Preselect the same object type as the main object if available
                if (!RequestHelper.IsPostBack() && objType.EqualsCSafe(ObjectType, true))
                {
                    li.Selected = true;
                }
            }

            drpRelatedObjType.DataBind();
        }
    }

    #endregion
}