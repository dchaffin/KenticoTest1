using System;
using System.Collections;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.DataCom;
using CMS.EventLog;
using CMS.ExtendedControls.ActionsConfig;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.UIControls;

/// <summary>
/// Provides UI elements to display and edit CMS contact and possibly update it from the specified Data.com contact.
/// </summary>
[EditedObject(OnlineMarketingObjectType.CONTACT, "contactId")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Tab_DataCom : CMSDataComPage, ICallbackEventHandler
{
    #region "Variables"

    /// <summary>
    /// The suggested Data.com contact filter when the Data.com search didn't return an exact match.
    /// </summary>
    private ContactFilter mFilter = null;


    /// <summary>
    /// A value that indicates whether the control should stop processing the request.
    /// </summary>
    private bool mStopProcessing = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the Data.com contact to display side-by-side with the CMS contact.
    /// </summary>
    public Contact Contact
    {
        get
        {
            return ViewState["DataComContact"] as Contact;
        }
        set
        {
            ViewState["DataComContact"] = value;
        }
    }


    /// <summary>
    /// Gets or sets the suggested Data.com contact filter when the Data.com search didn't return an exact match.
    /// </summary>
    public ContactFilter Filter
    {
        get
        {
            return mFilter;
        }
        set
        {
            mFilter = value;
        }
    }


    /// <summary>
    /// Gets the site identifier of the edited contact.
    /// </summary>
    public int ContactSiteID
    {
        get
        {
            return ContactHelper.ObjectSiteID(EditedObject);
        }
    }


    /// <summary>
    /// Gets the unique identifier to pass the parameters on to the Data.com contact selection dialog window.
    /// </summary>
    private string ParametersIdentifier
    {
        get
        {
            string identifier = ViewState["PID"] as string;
            if (identifier == null)
            {
                identifier = Guid.NewGuid().ToString("N");
                ViewState["PID"] = identifier;
            }

            return identifier;
        }
    }


    /// <summary>
    /// Gets the unique identifier to pass the parameters on to the Data.com Buy contact dialog window.
    /// </summary>
    private string BuyParametersIdentifier
    {
        get
        {
            string identifier = ViewState["BuyPID"] as string;
            if (identifier == null)
            {
                identifier = Guid.NewGuid().ToString("N");
                ViewState["BuyPID"] = identifier;
            }

            return identifier;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the control should stop processing the request.
    /// </summary>
    private bool StopProcessing
    {
        get
        {
            return mStopProcessing;
        }
        set
        {
            mStopProcessing = value;
        }
    }

    #endregion


    #region "Life cycle methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        ScriptHelper.RegisterDialogScript(Page);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        ContactInfo contact = EditedObject as ContactInfo;
        AuthorizeReadRequest(contact);
        IDataComConfiguration configuration = DataComHelper.GetConfiguration(ContactSiteID);
        if (configuration.GetToken() == null)
        {
            ShowWarning(GetString("datacom.notoken"), null, null);
        }
        else
        {
            try
            {
                if (!String.IsNullOrEmpty(ContactHiddenField.Value))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    Contact freshContact = serializer.Unserialize<Contact>(ContactHiddenField.Value);
                    if (Contact == null)
                    {
                        ContactForm.MergeHint = true;
                    }
                    else
                    {
                        if (Contact.ContactId != freshContact.ContactId)
                        {
                            ContactForm.MergeHint = true;
                        }
                        else if (String.IsNullOrEmpty(Contact.Phone) && String.IsNullOrEmpty(Contact.Email) && (!String.IsNullOrEmpty(freshContact.Phone) || !String.IsNullOrEmpty(freshContact.Email)))
                        {
                            ContactForm.MergeHint = true;
                            ContactForm.MergeHintAttributes = new string[] { "Phone", "Email" };
                        }
                    }
                    Contact = freshContact;
                }
                if (Contact == null)
                {
                    ContactInfo contactInfo = EditedObject as ContactInfo;
                    ContactIdentity identity = DataComHelper.CreateContactIdentity(contactInfo);
                    DataComClient client = DataComHelper.CreateClient(configuration);
                    IContactProvider provider = DataComHelper.CreateContactProvider(client, configuration);
                    ContactFinder finder = DataComHelper.CreateContactFinder(provider);
                    ContactFilter filterHint = null;
                    Contact match = finder.Find(identity, out filterHint);
                    if (match != null)
                    {
                        ShowInformation(GetString("datacom.contactmatch"));
                        Contact = match;
                        ContactForm.MergeHint = true;
                    }
                    else
                    {
                        ShowInformation(GetString("datacom.nocontactmatch"));
                    }
                    Filter = filterHint;
                }
                InitializeHeaderActions();
                InitializeDataComForm();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }
    }

    
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        if (Contact == null)
        {
            ContactFormPanel.CssClass = "Hidden";
            HeaderAction action = HeaderActions.ActionsList.SingleOrDefault(x => x.CommandName == "save");
            if (action != null)
            {
                action.Visible = false;
            }
        }
    }


    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        if (e.CommandName.ToLowerInvariant() == "save")
        {
            ContactInfo contact = EditedObject as ContactInfo;
            AuthorizeModifyRequest(contact);
            try
            {
                if (ContactForm.Validate() && ContactForm.Store())
                {
                    ContactForm.Merge();
                    BaseInfo data = ContactForm.Data as BaseInfo;
                    data.Generalized.SetObject();
                    Filter = null;
                    ShowChangesSaved();
                }
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes the form with the required dependencies and the specified CMS contact.
    /// </summary>
    protected void InitializeDataComForm()
    {
        IDataComConfiguration configuration = DataComHelper.GetConfiguration(ContactSiteID);
        ContactInfo contactInfo = EditedObject as ContactInfo;
        ContactForm.ParametersIdentifier = BuyParametersIdentifier;
        ContactForm.FormInformation = DataComHelper.GetContactFormInfo();
        ContactForm.EntityInfo = DataComHelper.GetContactEntityInfo();
        ContactForm.EntityMapping = configuration.GetContactMapping();
        ContactForm.EntityAttributeMapperFactory = DataComHelper.GetContactAttributeMapperFactory();
        ContactForm.Entity = Contact;
        ContactForm.EntityAttributeFormatter = DataComHelper.GetEntityAttributeFormatter();
        ContactForm.BuyContactEnabled = (configuration.GetUserCredential() != null);
        ContactForm.DefaultFieldLayout = CMS.FormControls.FieldLayoutEnum.ThreeColumns;
        ContactForm.DefaultFormLayout = CMS.FormEngine.FormLayoutEnum.Standard;
        ContactForm.Restore(contactInfo);
    }


    /// <summary>
    /// Adds actions to the page header.
    /// </summary>
    protected void InitializeHeaderActions()
    {
        IDataComConfiguration configuration = DataComHelper.GetConfiguration(ContactSiteID);
        bool searchEnabled = (configuration.GetToken() != null);
        string[,] actions = new string[1 + (searchEnabled ? 1 : 0), 11];
        actions[0, 0] = CMS.ExtendedControls.HeaderActions.TYPE_SAVEBUTTON;
        actions[0, 1] = GetString("general.apply");
        actions[0, 5] = GetImageUrl("CMSModules/CMS_Content/EditMenu/save.png");
        actions[0, 6] = "save";
        actions[0, 8] = "true";
        if (searchEnabled)
        {
            actions[1, 0] = CMS.ExtendedControls.HeaderActions.TYPE_LINKBUTTON;
            actions[1, 1] = GetString("general.search");
            actions[1, 2] = CreateSearchActionClientScript();
            actions[1, 5] = GetImageUrl("CMSModules/CMS_ContactManagement/search.png");
        }
        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
        HeaderActions.Actions = actions;
    }


    /// <summary>
    /// Creates a client script to open the dialog window to select a Data.com contact, and returns it.
    /// </summary>
    /// <returns>A client script to open the dialog window to select a Data.com contact.</returns>
    protected string CreateSearchActionClientScript()
    {
        string baseUrl = URLHelper.ResolveUrl("~/CMSModules/ContactManagement/Pages/Tools/DataCom/SelectContact.aspx");
        string url = String.Format("{0}?pid={1}", baseUrl, ParametersIdentifier);
        string script = String.Format("function DataCom_SelectContact (arg, context) {{ modalDialog('{0}', 'SelectContact', '1000', '700', null); return false; }}", URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(url)));
        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "DataCom_SelectContact", script, true);

        return String.Format("{0}; return false;", Page.ClientScript.GetCallbackEventReference(this, null, "DataCom_SelectContact", null));
    }


    /// <summary>
    /// Displays an error message and logs the specified exception to the event log.
    /// </summary>
    /// <param name="exception">The exception to handle.</param>
    private void HandleException(Exception exception)
    {
        ErrorSummary.Report(exception);
        StopProcessing = true;
        EventLogProvider.LogException("Data.com Connector", "ContactPage", exception);
    }

    #endregion


    #region ICallbackEventHandler Members

    string ICallbackEventHandler.GetCallbackResult()
    {
        return null;
    }


    void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
    {
        JsonSerializer serializer = new JsonSerializer();
        Hashtable parameters = new Hashtable();
        if (Filter != null)
        {
            parameters["Filter"] = serializer.Serialize(Filter);
        }
        parameters["SiteID"] = ContactSiteID;
        WindowHelper.Add(ParametersIdentifier, parameters);
    }

    #endregion
}