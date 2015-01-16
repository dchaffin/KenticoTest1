using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.UIControls;
using CMS.GlobalHelper;

public partial class CMSModules_EmailTemplates_Pages_Frameset : CMSEmailTemplatesPage
{
    #region "Variables"

    protected int mHeaderHeight = 0;
    private bool mDialog = false;

    #endregion


    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // Set smaller header for dialog since it doesn't have breadcrumbs
        mDialog = QueryHelper.GetBoolean("editonlycode", false);
        mHeaderHeight = mDialog ? TabsFrameHeight : TabsBreadHeadFrameHeight;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (mDialog)
        {
            RegisterModalPageScripts();
        }
    }
}