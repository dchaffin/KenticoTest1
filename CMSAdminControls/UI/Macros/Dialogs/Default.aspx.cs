using System;

using CMS.CMSHelper;
using CMS.UIControls;

public partial class CMSAdminControls_UI_Macros_Dialogs_Default : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Checks user permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.WYSIWYGEditor", "InsertMacro"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.WYSIWYGEditor", "InsertMacro");
        }
    }
}