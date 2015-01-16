using System;
using System.Web;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;

public partial class CMSModules_Blogs_FormControls_SelectBlogName : FormEngineUserControl
{
    private string selectedSiteName = null;
    private bool siteNameIsAll = false;

    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();
            return usBlogs.Value;
        }
        set
        {
            EnsureChildControls();
            usBlogs.Value = value;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        SetFormSiteName();
        string siteName = String.IsNullOrEmpty(selectedSiteName) ? CMSContext.CurrentSiteName : selectedSiteName;

        // Prepare where condition
        usBlogs.WhereCondition = SqlHelperClass.AddWhereCondition(String.Empty, "SiteName = N'" + SqlHelperClass.GetSafeQueryString(siteName, false) + "'");
        bool isAdmin = CMSContext.CurrentUser.IsGlobalAdministrator;
        string[,] items = new string[1 + Convert.ToInt32(isAdmin),2];
        items[0, 0] = GetString("blogselector.myblogs");
        items[0, 1] = "##myblogs##";

        // Allow all item for admin
        if (isAdmin)
        {
            items[1, 0] = GetString("general.selectall");
            items[1, 1] = "##all##";
        }
        usBlogs.SpecialFields = items;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        bool reloaded = false;
        usBlogs.Enabled = true;

        if (siteNameIsAll)
        {
            usBlogs.Enabled = false;

            if (!URLHelper.IsPostback())
            {
                // Load the uniselector data in order to select the ##all## value
                usBlogs.Reload(true);
                reloaded = true;
            }

            if (usBlogs.DropDownSingleSelect.Items.Count > 0)
            {
                usBlogs.DropDownSingleSelect.SelectedValue = "##all##";
            }
        }

        // Update the update panel
        if (URLHelper.IsPostback()
            && DependsOnAnotherField)
        {
            if (!String.IsNullOrEmpty(selectedSiteName)
                && (!reloaded))
            {
                usBlogs.Reload(true);
            }

            pnlUpdate.Update();
        }
    }


    /// <summary>
    /// Sets the site name if the SiteName field is available in the form.
    /// </summary>
    private void SetFormSiteName()
    {
        if (DependsOnAnotherField
            && (Form != null)
            && Form.IsFieldAvailable("SiteName"))
        {
            string siteName = ValidationHelper.GetString(Form.GetFieldValue("SiteName"), "");
            if (siteName.EqualsCSafe(string.Empty, true) || siteName.EqualsCSafe("##all##", true))
            {
                siteNameIsAll = true;
                return;
            }
            else if (siteName.EqualsCSafe("##currentsite##", true))
            {
                siteName = CMSContext.CurrentSiteName;
            }

            if (!String.IsNullOrEmpty(siteName))
            {
                selectedSiteName = siteName;
                return;
            }
        }

        selectedSiteName = null;
    }


    /// <summary>
    /// Creates child controls and loads update panle container if it is required.
    /// </summary>
    protected override void CreateChildControls()
    {
        // If the selector is not defined load the update panel container
        if (usBlogs == null)
        {
            pnlUpdate.LoadContainer();
        }
        // Call base method
        base.CreateChildControls();
    }
}