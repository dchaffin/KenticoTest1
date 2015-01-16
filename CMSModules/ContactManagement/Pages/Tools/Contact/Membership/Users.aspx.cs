using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.UIControls;

// Edited object
[EditedObject(OnlineMarketingObjectType.CONTACT, "contactid")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Membership_Users : CMSContactManagementContactsPage
{
    #region "Variables"

    private int contactId = 0;
    private bool globalContact = false;
    private bool mergedContact = false;
    private bool modifyAllowed = false;
    private ContactInfo ci = null;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        ci = (ContactInfo)EditedObject;
        CheckReadPermission(ci.ContactSiteID);

        globalContact = (ci.ContactSiteID <= 0);
        mergedContact = (ci.ContactMergedWithContactID > 0);
        modifyAllowed = ContactHelper.AuthorizedModifyContact(ci.ContactSiteID, false);

        contactId = QueryHelper.GetInteger("contactid", 0);

        string where = null;
        // Filter only site members in CMSDesk (for global contacts)
        if (!IsSiteManager && globalContact && AuthorizedForSiteContacts)
        {
            where += " (ContactSiteID IS NULL OR ContactSiteID=" + CMSContext.CurrentSiteID + ")";
        }

        // Choose correct query according to type of contact
        if (globalContact)
        {
            gridElem.ObjectType = OnlineMarketingObjectType.MEMBERSHIPGLOBALUSERLIST;
        }
        else if (mergedContact)
        {
            gridElem.ObjectType = OnlineMarketingObjectType.MEMBERSHIPMERGEDUSERLIST;
        }
        else
        {
            gridElem.ObjectType = OnlineMarketingObjectType.MEMBERSHIPUSERLIST;
        }

        // Query parameters
        QueryDataParameters parameters = new QueryDataParameters();
        parameters.Add("@ContactId", contactId);

        gridElem.WhereCondition = where;
        gridElem.QueryParameters = parameters;
        gridElem.OnAction += new OnActionEventHandler(gridElem_OnAction);
        gridElem.OnExternalDataBound += new OnExternalDataBoundEventHandler(gridElem_OnExternalDataBound);

        // Setup user selector
        selectUser.UniSelector.SelectionMode = SelectionModeEnum.MultipleButton;
        selectUser.UniSelector.OnItemsSelected += new EventHandler(UniSelector_OnItemsSelected);
        selectUser.UniSelector.ReturnColumnName = "UserID";
        selectUser.UniSelector.ButtonImage = GetImageUrl("Objects/CMS_User/add.png");
        selectUser.UniSelector.DialogLink.CssClass = "MenuItemEdit";
        selectUser.ShowSiteFilter = false;
        selectUser.ResourcePrefix = "addusers";
        selectUser.UniSelector.DialogButton.CssClass = "LongButton";
        selectUser.IsLiveSite = false;
        selectUser.SiteID = ci.ContactSiteID;

        // Hide header actions for global contact or merged contact
        CurrentMaster.HeaderActionsPlaceHolder.Visible = modifyAllowed && !globalContact && !mergedContact;
    }


    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "delete":
                DisableDeleteButton(sender);
                break;
        }

        return parameter;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Hide unwanted columns
        gridElem.NamedColumns["sitename"].Visible = globalContact;

        // Display contact full name if some merged contacts point to this contact or if required
        bool showFullName = globalContact;
        if (!showFullName)
        {
            object dataSrc = gridElem.GridView.DataSource;
            if (!DataHelper.DataSourceIsEmpty(dataSrc))
            {
                DataRow[] dr = null;
                if (dataSrc is DataSet)
                {
                    DataSet ds = (DataSet)dataSrc;
                    dr = ds.Tables[0].Select("ContactMergedWithContactID = " + contactId);
                }
                if (dataSrc is DataView)
                {
                    DataView dv = ((DataView)dataSrc);
                    dr = dv.Table.Select("ContactMergedWithContactID = " + contactId);
                }
                showFullName = (dr != null) && (dr.Length > 0);
            }
        }
        gridElem.NamedColumns["contactname"].Visible = showFullName;
    }


    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "delete":
                int membershipId = ValidationHelper.GetInteger(actionArgument, 0);
                if (membershipId > 0)
                {
                    // Check permissions
                    if (ContactHelper.AuthorizedModifyContact(ci.ContactSiteID, true))
                    {
                        MembershipInfoProvider.DeleteRelationship(membershipId);
                    }
                }
                break;
        }
    }


    private void UniSelector_OnItemsSelected(object sender, EventArgs e)
    {
        // Check permissions
        if (ContactHelper.AuthorizedModifyContact(ci.ContactSiteID, true))
        {
            string values = ValidationHelper.GetString(selectUser.UniSelector.Value, null);
            if (!String.IsNullOrEmpty(values))
            {
                // Store users one by one
                string[] userIds = values.Split(';');
                foreach (string userId in userIds)
                {
                    // Check if user ID is valid
                    int userIdInt = ValidationHelper.GetInteger(userId, 0);
                    if (userIdInt <= 0)
                    {
                        continue;
                    }
                    // Add new relation
                    int parentId = (ci.ContactMergedWithContactID == 0)
                                       ? ci.ContactID
                                       : ci.ContactMergedWithContactID;
                    MembershipInfoProvider.SetRelationship(userIdInt, MemberTypeEnum.CmsUser, ci.ContactID, parentId, true);

                    // When contact was merged update contact info
                    ci = ContactInfoProvider.GetContactInfo(contactId);
                }

                // When contact was merged then refresh complete page
                if ((ci != null) && (ci.ContactMergedWithContactID > 0))
                {
                    Page.Response.Redirect(URLHelper.Url.ToString(), true);
                }
                else
                {
                    gridElem.ReloadData();
                }
            }
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Disables delete button for merged, global contacts or when user has no modify rights.
    /// </summary>
    private void DisableDeleteButton(object sender)
    {
        if (globalContact || mergedContact || !modifyAllowed)
        {
            var button = ((ImageButton)sender);
            button.Attributes.Add("src", GetImageUrl("Design/Controls/UniGrid/Actions/DeleteDisabled.png"));
            button.Enabled = false;
        }
    }

    #endregion
}