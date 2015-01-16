using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_FormControls_Pages_Development_Header : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int controlId = QueryHelper.GetInteger("controlId", 0);
        string controlName = null;
        bool isInherited = false;

        if (controlId > 0)
        {
            FormUserControlInfo fuci = FormUserControlInfoProvider.GetFormUserControlInfo(controlId);
            if (fuci != null)
            {
                controlName = fuci.UserControlDisplayName;
                if (fuci.UserControlParentID > 0)
                {
                    isInherited = true;
                }
            }
        }

        // Initializes page title
        string[,] breadcrumbs = new string[2,3];
        breadcrumbs[0, 0] = GetString("Development_FormUserControl_Edit.Controls");
        breadcrumbs[0, 1] = "~/CMSModules/FormControls/Pages/Development/List.aspx";
        breadcrumbs[0, 2] = "_parent";
        breadcrumbs[1, 0] = controlName;
        breadcrumbs[1, 1] = "";
        breadcrumbs[1, 2] = "";

        // Initialize page
        CurrentMaster.Title.Breadcrumbs = breadcrumbs;
        CurrentMaster.Title.TitleText = GetString("Development_FormUserControl_Edit.Edit");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_FormControl/object.png");
        CurrentMaster.Title.HelpTopicName = "edit_form_control";
        CurrentMaster.Title.HelpName = "helpTopic";

        if (!RequestHelper.IsPostBack())
        {
            string[,] tabs = new string[2,4];

            tabs[0, 0] = GetString("general.general");
            tabs[0, 1] = "SetHelpTopic('helpTopic', 'edit_form_control');";
            tabs[0, 2] = "Edit.aspx?controlId=" + controlId;
            tabs[1, 0] = GetString("general.properties");

            if (isInherited)
            {
                tabs[1, 1] = "SetHelpTopic('helpTopic', 'properties_form_control_inherited');";
            }
            else
            {
                tabs[1, 1] = "SetHelpTopic('helpTopic', 'properties_form_control');";
            }

            tabs[1, 2] = "Parameters.aspx?controlId=" + controlId;

            CurrentMaster.Tabs.Tabs = tabs;
            CurrentMaster.Tabs.UrlTarget = "FormUserControl";
        }
    }
}