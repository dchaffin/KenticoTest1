using System;
using System.Collections.Generic;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.MessageBoard;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;

using TreeNode = CMS.DocumentEngine.TreeNode;
using CMS.ExtendedControls;

public partial class CMSModules_MessageBoards_Controls_NewSubscription : CMSUserControl
{
    #region "Private variables"

    private BoardProperties mBoardProperties = null;
    private int mBoardID = 0;

    #endregion


    #region "Public properties"

    /// <summary>
    /// ForumId.
    /// </summary>
    public int BoardID
    {
        get
        {
            return mBoardID;
        }
        set
        {
            mBoardID = value;
        }
    }


    /// <summary>
    /// Properties passed from the upper control.
    /// </summary>
    public BoardProperties BoardProperties
    {
        get
        {
            return mBoardProperties;
        }
        set
        {
            mBoardProperties = value;
        }
    }


    /// <summary>
    /// Placeholder for messages.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMessages;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        string valGroup = UniqueID;

        lblEmail.ResourceString = "board.subscription.email";
        btnOk.ResourceString = "board.subscription.subscribe";
        btnOk.ValidationGroup = valGroup;

        rfvEmailRequired.ErrorMessage = GetString("board.subscription.noemail");
        rfvEmailRequired.ValidationGroup = valGroup;

        revEmailValid.ValidationGroup = valGroup;
        revEmailValid.ErrorMessage = GetString("board.messageedit.revemail");
        revEmailValid.ValidationExpression = @"^([\w0-9_\-\+]+(\.[\w0-9_\-\+]+)*@[\w0-9_-]+(\.[\w0-9_-]+)+)*$";
    }


    /// <summary>
    /// Pre-fill user e-mail.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!RequestHelper.IsPostBack())
        {
            if (txtEmail.Text.Trim() == "" && (CMSContext.CurrentUser.Email != null) && (CMSContext.CurrentUser.Email != ""))
            {
                txtEmail.Text = CMSContext.CurrentUser.Email;
            }
        }
    }


    /// <summary>
    /// OK click handler.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Check banned IP
        if (!BannedIPInfoProvider.IsAllowed(CMSContext.CurrentSiteName, BanControlEnum.AllNonComplete))
        {
            ShowError(GetString("General.BannedIP"));
            return;
        }

        // Check input fields
        string email = txtEmail.Text.Trim();
        string result = new Validator().NotEmpty(email, rfvEmailRequired.ErrorMessage)
            .IsEmail(email, GetString("general.correctemailformat")).Result;

        // Try to subscribe new subscriber
        if (result == "")
        {
            // Try to create a new board
            BoardInfo boardInfo = null;
            if (BoardID == 0)
            {
                // Create new message board according to webpart properties
                boardInfo = new BoardInfo(BoardProperties);
                BoardInfoProvider.SetBoardInfo(boardInfo);

                // Update information on current message board
                BoardID = boardInfo.BoardID;

                // Set board-role relationship                
                BoardRoleInfoProvider.SetBoardRoles(BoardID, BoardProperties.BoardRoles);

                // Set moderators
                BoardModeratorInfoProvider.SetBoardModerators(BoardID, BoardProperties.BoardModerators);
            }

            if (BoardID > 0)
            {
                // Check for duplicit e-mails
                DataSet ds = BoardSubscriptionInfoProvider.GetSubscriptions("(SubscriptionApproved <> 0) AND (SubscriptionBoardID=" + BoardID +
                                                                            ") AND (SubscriptionEmail='" + SqlHelperClass.GetSafeQueryString(email, false) + "')", null);
                if (DataHelper.DataSourceIsEmpty(ds))
                {
                    BoardSubscriptionInfo bsi = new BoardSubscriptionInfo();
                    bsi.SubscriptionBoardID = BoardID;
                    bsi.SubscriptionEmail = email;
                    if ((CMSContext.CurrentUser != null) && !CMSContext.CurrentUser.IsPublic())
                    {
                        bsi.SubscriptionUserID = CMSContext.CurrentUser.UserID;
                    }
                    BoardSubscriptionInfoProvider.Subscribe(bsi, DateTime.Now, true, true);

                    // Clear form
                    txtEmail.Text = "";
                    if (boardInfo == null)
                    {
                        boardInfo = BoardInfoProvider.GetBoardInfo(BoardID);
                    }

                    // If subscribed, log activity
                    if (bsi.SubscriptionApproved)
                    {
                        ShowConfirmation(GetString("board.subscription.beensubscribed"));
                        LogActivity(bsi, boardInfo);
                    }
                    else
                    {
                        string confirmation = GetString("general.subscribed.doubleoptin");
                        int optInInterval = BoardInfoProvider.DoubleOptInInterval(CMSContext.CurrentSiteName);
                        if (optInInterval > 0)
                        {
                            confirmation += "<br />" + string.Format(GetString("general.subscription_timeintervalwarning"), optInInterval);
                        }
                        ShowConfirmation(confirmation);
                    }
                }
                else
                {
                    result = GetString("board.subscription.emailexists");
                }
            }
        }

        if (result != String.Empty)
        {
            ShowError(result);
        }
    }


    /// <summary>
    /// Log activity (subscribing)
    /// </summary>
    private void LogActivity(BoardSubscriptionInfo bsi, BoardInfo bi)
    {
        Activity activity = new ActivitySubscriptionMessageBoard(bi, bsi, CMSContext.CurrentDocument, CMSContext.ActivityEnvironmentVariables);
        activity.Log();
    }

    #endregion
}