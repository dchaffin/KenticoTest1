using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.Newsletter;
using CMS.CMSHelper;

// Set edited object
[EditedObject("newsletter.emailtemplate", "templateid")]
// Set breadcrumbs
[Breadcrumbs(2)]
[Breadcrumb(0, "NewsletterTemplate_Edit.ItemListLink", "~/CMSModules/Newsletters/Tools/Templates/NewsletterTemplate_List.aspx", "_parent")]
[Breadcrumb(1, Text = "{%EditedObject.TemplateDisplayName%}")]
// Set help
[Title("Objects/Newsletter_EmailTemplate/object.png", "newsletters.templates", "newsletter_template_general", HelpName = "helpTopic")]
public partial class CMSModules_Newsletters_Tools_Templates_NewsletterTemplate_Header : CMSNewsletterTemplatesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {

        InitTabs("Content");
        SetTab(0, GetString("general.general"), CMSContext.ResolveMacros("NewsletterTemplate_Edit.aspx?templateid={%EditedObject.TemplateID%}&tabmode={?tabmode?}"), "SetHelpTopic('helpTopic','newsletter_template_general')");

        // Add newsletter - template binding tab for ordinary newsletter template
        EmailTemplateInfo emailTemplate = (EmailTemplateInfo)EditedObject;
        if ((emailTemplate != null) && (emailTemplate.TemplateType == EmailTemplateType.Issue)) 
        {
            SetTab(1, GetString("newsletter.templatenewsletter"), CMSContext.ResolveMacros("Tab_Newsletters.aspx?templateid={%EditedObject.TemplateID%}&tabmode={?tabmode?}"), "SetHelpTopic('helpTopic','template_newsletters')");
        }
    }
}