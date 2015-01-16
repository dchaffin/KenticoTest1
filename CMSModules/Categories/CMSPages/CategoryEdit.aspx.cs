using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Categories_CMSPages_CategoryEdit : CMSLiveModalPage
{
    private int categoryId = 0;
    private int parentCategoryId = -1;
    private bool createPersonal = true;
    private CurrentUserInfo currentUser = null;


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        currentUser = CMSContext.CurrentUser;

        categoryId = QueryHelper.GetInteger("categoryId", 0);
        parentCategoryId = QueryHelper.GetInteger("parentId", -1);
        createPersonal = QueryHelper.GetBoolean("personal", true);

        catEdit.OnSaved += new EventHandler(catEdit_OnSaved);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        string titleText = GetString("categories.properties");
        if (categoryId == 0)
        {
            catEdit.ParentCategoryID = parentCategoryId;
            catEdit.UserID = createPersonal ? CMSContext.CurrentUser.UserID : 0;

            CheckReadPermission(catEdit.ParentCategory);

            // Prevent creating disabled category
            catEdit.ShowEnabled = false;

            titleText = GetString("multiplecategoriesselector.newcategory");
        }
        else
        {
            catEdit.CategoryID = categoryId;

            CheckReadPermission(catEdit.Category);
        }

        // Init buttons
        btnCancel.Attributes.Add("onclick", "CloseDialog();");
        btnOk.Click += new EventHandler(catEdit.btnSaveCategory_Click);

        CurrentMaster.Title.TitleText = titleText;
        CurrentMaster.Title.TitleImage = GetImageUrl("/CMSModules/CMS_Categories/module.png");
        Page.Title = titleText;
    }


    private void catEdit_OnSaved(object sender, EventArgs e)
    {
        string param = "edit";
        if (categoryId == 0)
        {
            param = "new|" + catEdit.CategoryID;
        }

        // Refresh opener and close dialog
        ltlScript.Text = ScriptHelper.GetScript("wopener.Refresh('" + param + "'); CloseDialog();");
    }


    private void CheckReadPermission(CategoryInfo category)
    {
        if ((category != null) && (category.CategoryUserID != currentUser.UserID))
        {
            // Check read permission for Categories module
            if (!currentUser.IsAuthorizedPerResource("CMS.Categories", "Read"))
            {
                RedirectToAccessDenied("CMS.Categories", "Read");
            }
        }
    }
}