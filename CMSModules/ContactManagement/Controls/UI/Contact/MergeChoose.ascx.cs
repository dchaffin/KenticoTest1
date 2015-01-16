using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

using ContactFilter = CMSModules_ContactManagement_Controls_UI_Contact_Filter;

public partial class CMSModules_ContactManagement_Controls_UI_Contact_MergeChoose : CMSAdminListControl
{
    #region "Variables and constants"

    /// <summary>
    /// URL of collision dialog.
    /// </summary>
    private const string MERGE_DIALOG = "~/CMSModules/ContactManagement/Pages/Tools/Contact/CollisionDialog.aspx";

    private ContactInfo ci = null;
    private ContactFilter filter = null;

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
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            gridElem.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            gridElem.IsLiveSite = value;
            plcMess.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Current contact.
    /// </summary>
    public ContactInfo Contact
    {
        get
        {
            if (ci == null)
            {
                if (CMSPage.EditedObject != null)
                {
                    ci = (ContactInfo)CMSPage.EditedObject;
                }
            }
            return ci;
        }
    }


    /// <summary>
    /// Modal dialog identifier.
    /// </summary>
    private string Identifier
    {
        get
        {
            // Try to load data from control viewstate
            string identifier = ValidationHelper.GetString(hdnIdentifier.Value, String.Empty);
            if (string.IsNullOrEmpty(identifier))
            {
                // Create new Guid
                identifier = Guid.NewGuid().ToString();
                hdnIdentifier.Value = identifier;
            }

            return identifier;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        gridElem.OnFilterFieldCreated += new OnFilterFieldCreated(gridElem_OnFilterFieldCreated);
        gridElem.LoadGridDefinition();
        base.OnInit(e);
    }


    void gridElem_OnFilterFieldCreated(string columnName, UniGridFilterField filterDefinition)
    {
        filter = filterDefinition.ValueControl as ContactFilter;
        if (filter != null)
        {
            filter.NotMerged = true;
            filter.HideMergedFilter = true;
            filter.IsLiveSite = IsLiveSite;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (Contact != null)
        {
            // Current contact is global object
            if (Contact.ContactSiteID == 0)
            {
                filter.SiteID = CMSContext.CurrentSiteID;
                // Display site selector in site manager
                if (ContactHelper.IsSiteManager)
                {
                    filter.DisplaySiteSelector = true;
                    filter.SiteID = UniSelector.US_GLOBAL_RECORD;
                }
                // Display 'site or global' selector in CMS desk for global objects
                else if (ContactHelper.AuthorizedReadContact(CMSContext.CurrentSiteID, false) && ContactHelper.AuthorizedModifyContact(CMSContext.CurrentSiteID, false))
                {
                    filter.DisplayGlobalOrSiteSelector = true;
                }
                filter.HideMergedIntoGlobal = true;
            }
            else
            {
                filter.SiteID = Contact.ContactSiteID;
            }
            filter.ShowGlobalStatuses =
                ConfigurationHelper.AuthorizedReadConfiguration(UniSelector.US_GLOBAL_RECORD, false) &&
                (SettingsKeyProvider.GetBoolValue(CMSContext.CurrentSiteName + ".CMSCMGlobalConfiguration") || ContactHelper.IsSiteManager);
            gridElem.WhereCondition = SqlHelperClass.AddWhereCondition(gridElem.WhereCondition, "ContactID <> " + Contact.ContactID);
            gridElem.ZeroRowsText = GetString("om.contact.nocontacts");
            btnMergeSelected.Click += new EventHandler(btnMerge_Click);
            btnMergeAll.Click += new EventHandler(btnMergeAll_Click);

            if (Request["__EVENTARGUMENT"] == "saved")
            {
                ShowConfirmation(GetString("om.contact.merging"));
            }
        }
        else
        {
            StopProcessing = true;
            Visible = false;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        pnlButton.Visible = !DataHelper.DataSourceIsEmpty(gridElem.GridView.DataSource);
        gridElem.NamedColumns["sitename"].Visible = ((filter.SelectedSiteID < 0) && (filter.SelectedSiteID != UniSelector.US_GLOBAL_RECORD));
    }


    /// <summary>
    /// Button merge selected click.
    /// </summary>
    private void btnMerge_Click(object sender, EventArgs e)
    {
        if (ContactHelper.AuthorizedModifyContact(Contact.ContactSiteID, true))
        {
            if (gridElem.SelectedItems.Count > 0)
            {
                SetDialogParameters(false);
                OpenWindow();
            }
            else
            {
                ShowError(GetString("om.contact.selectcontacts"));
            }
        }
    }


    /// <summary>
    /// Button merge all click.
    /// </summary>
    private void btnMergeAll_Click(object sender, EventArgs e)
    {
        if (ContactHelper.AuthorizedModifyContact(Contact.ContactSiteID, true))
        {
            if (!DataHelper.DataSourceIsEmpty(gridElem.GridView.DataSource))
            {
                SetDialogParameters(true);
                OpenWindow();
            }
            else
            {
                ShowError(GetString("om.contact.selectcontacts"));
            }
        }
    }


    /// <summary>
    /// Sets the dialog parameters to the context.
    /// </summary>
    private void SetDialogParameters(bool mergeAll)
    {
        string condition = mergeAll ? SqlHelperClass.AddWhereCondition(gridElem.WhereCondition, gridElem.WhereClause) : SqlHelperClass.GetWhereCondition("ContactID", gridElem.SelectedItems);

        Hashtable parameters = new Hashtable();
        parameters["MergedContacts"] = new ContactListInfo().Generalized.GetData(null, condition, null, -1, null, false);
        parameters["ParentContact"] = Contact;
        parameters["issitemanager"] = ContactHelper.IsSiteManager;

        WindowHelper.Add(Identifier, parameters);
    }


    /// <summary>
    /// Registers JS for opening window.
    /// </summary>
    private void OpenWindow()
    {
        ScriptHelper.RegisterDialogScript(Page);

        string url = MERGE_DIALOG + "?params=" + Identifier;
        url += "&hash=" + QueryHelper.GetHash(url, false);

        StringBuilder script = new StringBuilder();
        script.Append(@"modalDialog('" + ResolveUrl(url) + @"', 'mergeDialog', 700, 700, null, null, true);");

        ScriptHelper.RegisterStartupScript(this, typeof(string), "MergeDialog" + ClientID, ScriptHelper.GetScript(script.ToString()));
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "RefreshPageScript", ScriptHelper.GetScript("function RefreshPage() { __doPostBack('" + this.ClientID + @"', 'saved'); }"));
    }

    #endregion
}