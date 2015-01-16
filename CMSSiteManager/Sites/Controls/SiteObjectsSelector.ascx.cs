using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;

public partial class CMSSiteManager_Sites_Controls_SiteObjectsSelector : FormEngineUserControl
{
    #region "Private variables"

    private string currentValues = string.Empty;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Site identifier. (Identifies site the objects are bound to.)
    /// </summary>
    public int SiteID
    {
        get;
        set;
    }


    /// <summary>
    /// Type of object that is bound to the site.
    /// </summary>
    public string ObjectType
    {
        get;
        set;
    }


    /// <summary>
    /// Binding object type.
    /// </summary>
    public string BindingObjectType
    {
        get;
        set;
    }


    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMsg;
        }
    }


    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            EnsureChildControls();

            base.Enabled = value;
            usObjects.Enabled = value;
        }
    }


    ///<summary>
    /// Gets or sets field value.
    ///</summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();
            return usObjects.Value;
        }
        set
        {
            EnsureChildControls();
            usObjects.Value = value;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            EnsureChildControls();
            base.IsLiveSite = value;
            usObjects.IsLiveSite = value;
        }
    }

    #endregion


    #region "Control events"

    /// <summary>
    /// Creates child controls and loads update panel container if it is required.
    /// </summary>
    protected override void CreateChildControls()
    {
        // If selector is not defined load update panel container
        if (usObjects == null)
        {
            pnlUpdate.LoadContainer();
        }
        // Call base method
        base.CreateChildControls();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            // Initialize selector
            usObjects.ObjectType = ObjectType;
            usObjects.WhereCondition = GetObjectTypeWhereCondition(ObjectType);
            usObjects.OnSelectionChanged += usObjects_OnSelectionChanged;
            SetTheValue();

            InitializeTitleLabel();

            ReloadData();

            // Reload data again, if page changed
            if ((usObjects.UniGrid.Pager != null) && (usObjects.UniGrid.Pager.UniPager != null))
            {
                usObjects.UniGrid.Pager.UniPager.OnPageChanged += UniPager_OnPageChanged;
            }
        }
        else
        {
            usObjects.StopProcessing = true;
        }
    }


    protected void usObjects_OnSelectionChanged(object sender, EventArgs e)
    {
        // Get new selection from selector
        string newValues = ValidationHelper.GetString(usObjects.Value, null);

        // Compare with previous selection and get items to remove
        string items = DataHelper.GetNewItemsInList(newValues, currentValues);

        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            // Remove items from site
            foreach (string item in newItems)
            {
                // Get identifier of parent object
                int parentId = ValidationHelper.GetInteger(item, 0);

                // Get binding object based on type
                BaseInfo bindingObj = CMSObjectHelper.GetObject(BindingObjectType);

                // Initialize values of removed object to allow recognition
                bindingObj.SetValue(bindingObj.TypeInfo.SiteIDColumn, SiteID);
                bindingObj.SetValue(bindingObj.TypeInfo.ParentIDColumn, parentId);

                // Get existing object if binding object has identifying column
                if (bindingObj.TypeInfo.IDColumn != TypeInfo.COLUMN_NAME_UNKNOWN)
                {
                    bindingObj = bindingObj.Generalized.GetExisting();
                }

                // Delete the binding
                bindingObj.Generalized.DeleteObject();
            }
        }


        // Compare with previous selection and get items to add
        items = DataHelper.GetNewItemsInList(currentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            // Add all new items to site
            foreach (string item in newItems)
            {
                // Get identifier of parent object
                int parentId = ValidationHelper.GetInteger(item, 0);

                // Get binding object based on type
                BaseInfo bindingObj = CMSObjectHelper.GetObject(BindingObjectType);

                // Initialize values
                bindingObj.SetValue(bindingObj.TypeInfo.SiteIDColumn, SiteID);
                bindingObj.SetValue(bindingObj.TypeInfo.ParentIDColumn, parentId);

                // Insert new object
                bindingObj.Generalized.SetObject();
            }
        }

        ShowChangesSaved();
    }


    private void UniPager_OnPageChanged(object sender, int pageNumber)
    {
        ReloadData();
    }

    #endregion


    #region "Methods"
    
    private string GetObjectTypeWhereCondition(string objectType)
    {
        switch (objectType)
        {
            case PredefinedObjectType.PAGETEMPLATE:
                return "[PageTemplateID] IN (SELECT [PageTemplateID] FROM [CMS_PageTemplate] WHERE [PageTemplateIsReusable] = 1)";

            case PredefinedObjectType.POLL:
                return "[PollID] IN (SELECT [PollID] FROM [Polls_Poll] WHERE [PollSiteID] IS NULL)";

            case PredefinedObjectType.SMTPSERVER:
                return "[ServerID] IN (SELECT [ServerID] FROM [CMS_SMTPServer] WHERE [ServerIsGlobal] = 0)";

            default:
                return null;
        }
    }


    private void InitializeTitleLabel()
    {
        BaseInfo bindingType = CMSObjectHelper.GetReadOnlyObject(BindingObjectType);
        if (bindingType != null)
        {
            string otherSide = bindingType.TypeInfo.ParentObjectType;
            string safeOtherSide = TranslationHelper.GetSafeClassName(otherSide);
            string friendlyType = GetString("objecttasks." + safeOtherSide);

            if (friendlyType.Length > 0)
            {
                char firstChar = friendlyType[0];
                if (char.IsUpper(firstChar))
                {
                    if (friendlyType.Length > 1)
                    {
                        char secondChar = friendlyType[1];
                        if (!char.IsUpper(secondChar))
                        {
                            friendlyType = friendlyType.ToLowerCSafe();
                        }
                    }
                }
            }
            lblAvialable.Text = string.Format(GetString("site_edit_siteobjects.objectsavailable"), friendlyType);
        }
    }


    private void SetTheValue()
    {
        // Get binding object
        BaseInfo bindingObj = CMSObjectHelper.GetObject(BindingObjectType);

        // Build where condition (select only bindings of the current site)
        string where = bindingObj.TypeInfo.SiteIDColumn + "=" + SiteID;

        string parentIdColumn = bindingObj.TypeInfo.ParentIDColumn;

        // Initialize selector from retrieved data
        DataSet ds = bindingObj.Generalized.GetData(null, where, null, 0, parentIdColumn, false);
        currentValues = TextHelper.Join(";", SystemDataHelper.GetStringValues(ds.Tables[0], parentIdColumn));

        if (!RequestHelper.IsPostBack())
        {
            Value = currentValues;
        }
    }


    /// <summary>
    /// Reloads the data of the UniSelector.
    /// </summary>
    public void ReloadData()
    {
        usObjects.Reload(true);
        pnlUpdate.Update();
    }

    #endregion
}