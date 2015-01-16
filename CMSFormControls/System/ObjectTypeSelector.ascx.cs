using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.CMSHelper;
using CMS.SettingsProvider;

public partial class CMSFormControls_System_ObjectTypeSelector : FormEngineUserControl
{
    #region "Variables"

    private string mObjectType = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return ObjectType;
        }
        set
        {
            ObjectType = value as string;
        }
    }


    /// <summary>
    /// Gets or sets the selected object type
    /// </summary>
    public string ObjectType
    {
        get
        {
            string result = drpType.SelectedValue.Trim(';');
            if (String.IsNullOrEmpty(result))
            {
                return null;
            }
            return ';' + drpType.SelectedValue + ';';
        }
        set
        {
            mObjectType = value;
            LoadItems();
            EnsureItem();
        }
    }


    /// <summary>
    /// Gets or sets whether "all" item is present in the list.
    /// </summary>
    public bool AllowAll
    {
        get;
        set;
    }

    #endregion


    #region "Control events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(CssClass))
        {
            drpType.CssClass = CssClass;
            CssClass = null;
        }

        // Initialize drop down list
        LoadItems();
    }


    private void LoadItems()
    {
        if (drpType.Items.Count == 0)
        {
            if (AllowAll)
            {
                drpType.Items.Add(new ListItem(GetString("general.selectall"), ""));
            }
            drpType.Items.Add(new ListItem(GetString("objecttypeselector.documents"), CMSObjectHelper.GROUP_DOCUMENTS));
            drpType.Items.Add(new ListItem(GetString("objecttypeselector.objects"), CMSObjectHelper.GROUP_OBJECTS));
        }
    }


    private void EnsureItem()
    {
        // If value is not already in dropdown list, add it there
        if (!String.IsNullOrEmpty(mObjectType))
        {
            string value = mObjectType.Trim(';');
            if (drpType.Items.FindByValue(value) == null)
            {
                var typeList = TypeHelper.GetTypes(value);
                string list = "";
                typeList.ForEach(s => list += ResHelper.GetString(ResHelper.GetAPIString("ObjectTasks." + s.Replace(".", "_"), s)) + ", ");
                drpType.Items.Add(new ListItem(list.Trim().TrimEnd(','), value));
            }
            drpType.SelectedValue = value;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!RequestHelper.IsPostBack())
        {
            // Empty string or both documents and object means (all) to be selected
            if (AllowAll && (String.IsNullOrEmpty(mObjectType) || (mObjectType.Contains(CMSObjectHelper.GROUP_DOCUMENTS) && mObjectType.Contains(CMSObjectHelper.GROUP_OBJECTS))))
            {
                drpType.SelectedIndex = 0;
            }
            else
            {
                EnsureItem();
            }
        }
    }

    #endregion
}