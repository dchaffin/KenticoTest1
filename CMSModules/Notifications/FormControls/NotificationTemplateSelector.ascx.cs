using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.Notifications;

public partial class CMSModules_Notifications_FormControls_NotificationTemplateSelector : FormEngineUserControl
{
    #region "Variables"

    private bool mUseTemplateNameForSelection = true;
    private bool mAddGlobalTemplates = true;
    private bool mAddNoneRecord = true;

    private int mTemplateID = 0;
    private string mTemplateName = "";

    private int mSiteId = 0;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            if (uniSelector != null)
            {
                uniSelector.Enabled = value;
            }
        }
    }


    /// <summary>
    /// Returns ClientID of the dropdown with templates.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return uniSelector.DropDownSingleSelect.ClientID;
        }
    }


    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            if (mUseTemplateNameForSelection)
            {
                return TemplateName;
            }
            else
            {
                return TemplateID;
            }
        }
        set
        {
            if (mUseTemplateNameForSelection)
            {
                TemplateName = ValidationHelper.GetString(value, "");
            }
            else
            {
                TemplateID = ValidationHelper.GetInteger(value, 0);
            }
        }
    }


    /// <summary>
    /// Gets or sets the Template ID.
    /// </summary>
    public int TemplateID
    {
        get
        {
            if (mUseTemplateNameForSelection)
            {
                // Convert name to ID
                string name = ValidationHelper.GetString(uniSelector.Value, "");
                NotificationTemplateInfo nti = NotificationTemplateInfoProvider.GetNotificationTemplateInfo(name, SiteId);
                if (nti != null)
                {
                    return nti.TemplateID;
                }
                return 0;
            }
            else
            {
                return ValidationHelper.GetInteger(uniSelector.Value, 0);
            }
        }
        set
        {
            if (uniSelector == null)
            {
                pnlUpdate.LoadContainer();
            }

            if (mUseTemplateNameForSelection)
            {
                // Convert ID to name
                NotificationTemplateInfo nti = NotificationTemplateInfoProvider.GetNotificationTemplateInfo(value);
                if (nti != null)
                {
                    mTemplateName = nti.TemplateName;
                    mTemplateID = nti.TemplateID;
                }
            }
            else
            {
                uniSelector.Value = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets the Template code name.
    /// </summary>
    public string TemplateName
    {
        get
        {
            if (mUseTemplateNameForSelection)
            {
                return ValidationHelper.GetString(uniSelector.Value, "");
            }
            else
            {
                // Convert ID to name
                int id = ValidationHelper.GetInteger(uniSelector.Value, 0);
                NotificationTemplateInfo nti = NotificationTemplateInfoProvider.GetNotificationTemplateInfo(id);
                if (nti != null)
                {
                    return nti.TemplateName;
                }
                return "";
            }
        }
        set
        {
            if (uniSelector == null)
            {
                pnlUpdate.LoadContainer();
            }

            if (mUseTemplateNameForSelection)
            {
                uniSelector.Value = value;
            }
            else
            {
                // Covnert name to ID
                NotificationTemplateInfo nti = NotificationTemplateInfoProvider.GetNotificationTemplateInfo(value, SiteId);
                if (nti != null)
                {
                    uniSelector.Value = nti.TemplateID;
                }
            }
        }
    }


    /// <summary>
    ///  If true, selected value is TemplateName, if false, selected value is TemplateID.
    /// </summary>
    public bool UseTemplateNameForSelection
    {
        get
        {
            return mUseTemplateNameForSelection;
        }
        set
        {
            mUseTemplateNameForSelection = value;
            if (uniSelector != null)
            {
                uniSelector.ReturnColumnName = (value ? "TemplateName" : "TemplateID");
            }
        }
    }


    /// <summary>
    /// Gets or sets the value which determines, whether to add (global) record to the dropdownlist.
    /// </summary>
    public bool AddGlobalTemplates
    {
        get
        {
            return mAddGlobalTemplates;
        }
        set
        {
            mAddGlobalTemplates = value;
        }
    }


    /// <summary>
    /// Gets or sets SiteId value.
    /// </summary>
    public int SiteId
    {
        get
        {
            if (mSiteId == 0)
            {
                mSiteId = CMSContext.CurrentSite.SiteID;
            }
            return mSiteId;
        }
        set
        {
            mSiteId = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines, whether to add none item record to the dropdownlist.
    /// </summary>
    public bool AddNoneRecord
    {
        get
        {
            return mAddNoneRecord;
        }
        set
        {
            mAddNoneRecord = value;
            if (uniSelector != null)
            {
                uniSelector.AllowEmpty = value;
            }
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            uniSelector.StopProcessing = true;
        }
        else
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    public void ReloadData()
    {
        uniSelector.IsLiveSite = IsLiveSite;
        uniSelector.ReturnColumnName = (UseTemplateNameForSelection ? "TemplateName" : "TemplateID");
        uniSelector.AllowEmpty = AddNoneRecord;
        if (AddGlobalTemplates)
        {
            uniSelector.SetValue("FilterMode", "notificationtemplateglobal");
        }
        else
        {
            uniSelector.SetValue("FilterMode", "notificationtemplate");
        }

        if (UseTemplateNameForSelection)
        {
            uniSelector.AllRecordValue = "";
            uniSelector.NoneRecordValue = "";
        }
    }
}