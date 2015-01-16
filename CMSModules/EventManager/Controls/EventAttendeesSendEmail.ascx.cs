using System;
using System.Data;
using System.Threading;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.EventManager;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using DocTreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_EventManager_Controls_EventAttendeesSendEmail : CMSAdminControl
{
    #region "Variables"

    private int mEventID = 0;
    HeaderAction btnSend;

    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
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
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Event ID.
    /// </summary>
    public int EventID
    {
        get
        {
            return mEventID;
        }
        set
        {
            mEventID = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Init(object sender, EventArgs e)
    {
        InitHeaderActions();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        htmlEmail.AutoDetectLanguage = false;
        htmlEmail.DefaultLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        htmlEmail.EditorAreaCSS = String.Empty;
        htmlEmail.ResolverName = "BookingResolver";
    }


    public override void ReloadData(bool forceLoad)
    {
        TreeProvider tree = new TreeProvider(CMSContext.CurrentUser);
        DocTreeNode nd = tree.SelectSingleNode(EventID, CMSContext.PreferredCultureCode, tree.CombineWithDefaultCulture, false);
        if (nd == null)
        {
            ShowInformation(GetString("editedobject.notexists"));
            plcSend.Visible = false;
            lblTitle.Visible = false;
            return;
        }

        //Enable controls
        txtSenderName.Enabled = true;
        txtSenderEmail.Enabled = true;
        txtSubject.Enabled = true;
        htmlEmail.Enabled = true;
        btnSend.Enabled = true;

        if (forceLoad)
        {
            string siteName = CMSContext.CurrentSiteName;
            txtSenderEmail.Text = SettingsKeyProvider.GetStringValue(siteName + ".CMSEventManagerInvitationFrom");
            txtSenderName.Text = SettingsKeyProvider.GetStringValue(siteName + ".CMSEventManagerSenderName");
            txtSubject.Text = SettingsKeyProvider.GetStringValue(siteName + ".CMSEventManagerInvitationSubject");
        }

        // Disable form if no attendees present or user doesn't have modify permission
        if (CMSContext.CurrentUser.IsAuthorizedPerResource("cms.eventmanager", "Modify"))
        {
            DataSet ds = EventAttendeeInfoProvider.GetEventAttendees(EventID, null, null, "AttendeeID", 1);
            if (DataHelper.DataSourceIsEmpty(ds))
            {
                DisableForm();
                lblInfo.Text = GetString("Events_List.NoAttendees");
                lblInfo.Visible = true;
            }
        }
        else
        {
            DisableForm();
            ShowWarning(GetString("events_sendemail.modifypermission"), null, null);
        }
    }


    /// <summary>
    /// Initializes header action control.
    /// </summary>
    private void InitHeaderActions()
    {
        HeaderActions.ActionsList.Clear();

        HeaderActions.ActionsList.Add(btnSend = new HeaderAction()
            {
                ControlType = HeaderActionTypeEnum.LinkButton,
                CommandName = ComponentEvents.SUBMIT,
                Text = GetString("general.send"),
                Tooltip = GetString("general.send"),
                ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/resendemail.png"),
                Enabled = true
            });

        HeaderActions.ActionPerformed += new CommandEventHandler(hdrActions_ActionPerformed);
        HeaderActions.ReloadData();
    }


    protected void hdrActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case ComponentEvents.SUBMIT:
                Send();
                break;
        }
    }


    /// <summary>
    /// Disable form.
    /// </summary>
    private void DisableForm()
    {
        txtSenderName.Enabled = false;
        txtSenderEmail.Enabled = false;
        txtSubject.Enabled = false;
        htmlEmail.Enabled = false;
        btnSend.Enabled = false;
    }


    /// <summary>
    /// Sends e-mail to all attendees.
    /// </summary>
    protected void Send()
    {
        // Check 'Modify' permission
        if (!CheckPermissions("cms.eventmanager", "Modify"))
        {
            return;
        }

        txtSenderName.Text = txtSenderName.Text.Trim();
        txtSenderEmail.Text = txtSenderEmail.Text.Trim();
        txtSubject.Text = txtSubject.Text.Trim();

        // Validate the fields
        string errorMessage = new Validator().NotEmpty(txtSenderName.Text, GetString("Events_SendEmail.EmptySenderName"))
            .NotEmpty(txtSenderEmail.Text, GetString("Events_SendEmail.EmptySenderEmail"))
            .NotEmpty(txtSubject.Text, GetString("Events_SendEmail.EmptyEmailSubject"))
            .IsEmail(txtSenderEmail.Text, GetString("Events_SendEmail.InvalidEmailFormat"))
            .Result;

        if (!String.IsNullOrEmpty(errorMessage))
        {
            ShowError(errorMessage);
            return;
        }

        string subject = txtSubject.Text;
        string emailBody = htmlEmail.ResolvedValue;

        // Get event node data
        TreeProvider mTree = new TreeProvider();
        DocTreeNode node = mTree.SelectSingleNode(EventID);

        if (node != null && CMSString.Equals(node.NodeClassName, "cms.bookingevent", true))
        {
            // Initialize macro resolver
            ContextResolver resolver = ContextResolver.GetInstance();
            resolver.KeepUnresolvedMacros = true;
            resolver.SourceData = new object[] { node };
            // Add named source data
            resolver.SetNamedSourceData("Event", node);

            // Event date string macro
            DateTime eventDate = ValidationHelper.GetDateTime(node.GetValue("EventDate"), DateTimeHelper.ZERO_TIME);
            DateTime eventEndDate = ValidationHelper.GetDateTime(node.GetValue("EventEndDate"), DateTimeHelper.ZERO_TIME);
            bool isAllDay = ValidationHelper.GetBoolean(node.GetValue("EventAllDay"), false);

            string[,] macro = new string[1, 2];
            macro[0, 0] = "eventdatestring";
            macro[0, 1] = EventProvider.GetEventDateString(eventDate, eventEndDate, isAllDay, TimeZoneHelper.GetSiteTimeZoneInfo(CMSContext.CurrentSite), CMSContext.CurrentSiteName);
            resolver.SourceParameters = macro;

            // Resolve e-mail body and subject macros and make links absolute
            emailBody = resolver.ResolveMacros(emailBody);
            emailBody = URLHelper.MakeLinksAbsolute(emailBody);
            subject = TextHelper.LimitLength(resolver.ResolveMacros(subject), 450);

            // EventSendEmail manages sending e-mails to all attendees
            EventSendEmail ese = new EventSendEmail(EventID, CMSContext.CurrentSiteName,
                                                    subject, emailBody, txtSenderName.Text.Trim(), txtSenderEmail.Text.Trim());

            ShowConfirmation(GetString("Events_SendEmail.EmailSent"));
        }
    }

    #endregion
}