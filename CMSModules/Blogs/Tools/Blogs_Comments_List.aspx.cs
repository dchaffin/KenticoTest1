using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.SettingsProvider;
using CMS.UIControls;

[Security(Resource = "CMS.Blog", UIElements = "Comments")]
public partial class CMSModules_Blogs_Tools_Blogs_Comments_List : CMSBlogsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // No cms.blog doc. type
        if (DataClassInfoProvider.GetDataClass("cms.blog") == null)
        {
            RedirectToInformation(GetString("blog.noblogdoctype"));
        }
    }
}