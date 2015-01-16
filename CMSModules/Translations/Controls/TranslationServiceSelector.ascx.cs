using System;
using System.Text;
using System.Data;
using System.Web.UI.WebControls;

using CMS.DocumentEngine;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.TranslationServices;
using CMS.SettingsProvider;
using CMS.CMSHelper;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_Translations_Controls_TranslationServiceSelector : CMSUserControl
{
    #region "Variables"

    private string defaultCulture = CultureHelper.GetDefaultCulture(CMSContext.CurrentSiteName);
    private bool mAnyServiceAvailable = false;
    private bool mDisplayMachineServices = true;
    private string mMachineServiceSuffix = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets TreeProvider to use.
    /// </summary>
    public TreeProvider TreeProvider
    {
        get;
        set;
    }


    /// <summary>
    /// Gets number of services displayed.
    /// </summary>
    public int DisplayedServicesCount
    {
        get;
        private set;
    }


    /// <summary>
    /// If true, the name of the service is displayed when only one is available.
    /// </summary>
    public bool DisplayOnlyServiceName
    {
        get;
        set;
    }


    /// <summary>
    /// Gets name of the service displayed (if only one is displayed, if more services are displayed, null is returned)
    /// </summary>
    public string DisplayedServiceName
    {
        get;
        private set;
    }


    /// <summary>
    /// Gets or sets NodeID which to translates
    /// </summary>
    public int NodeID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets TranslationSettings object which will be used to process/submit translation.
    /// </summary>
    public TranslationSettings TranslationSettings
    {
        get;
        set;
    }


    /// <summary>
    /// Returns if the translation should process binary fields.
    /// </summary>
    public bool ProcessBinary
    {
        get
        {
            return chkProcessBinary.Checked;
        }
        set
        {
            chkProcessBinary.Checked = value;
        }
    }


    /// <summary>
    /// Returns the instructions of the submission.
    /// </summary>
    public string Instructions
    {
        get
        {
            return txtInstruction.Text;
        }
        set
        {
            txtInstruction.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets source language of translation.
    /// </summary>
    public string FromLanguage
    {
        get
        {
            return selectCultureElem.Value.ToString();
        }
        set
        {
            selectCultureElem.Value = value;
        }
    }


    /// <summary>
    /// Gets or sets target language of translation.
    /// </summary>
    public string TargetLanguage
    {
        get
        {
            return selectTargetCultureElem.Value.ToString();
        }
        set
        {
            selectTargetCultureElem.Value = value;
        }
    }


    /// <summary>
    /// Returns the instructions of the submission.
    /// </summary>
    public int Priority
    {
        get
        {
            return ValidationHelper.GetInteger(drpPriority.Value, 1);
        }
        set
        {
            drpPriority.Value = value;
        }
    }


    /// <summary>
    /// Returns the instructions of the submission.
    /// </summary>
    public DateTime Deadline
    {
        get
        {
            return dtDeadline.SelectedDateTime;
        }
        set
        {
            dtDeadline.SelectedDateTime = value;
        }
    }


    /// <summary>
    /// Gets or sets the selected service name.
    /// </summary>
    public string SelectedService
    {
        get
        {
            return hdnSelectedName.Value;
        }
        set
        {
            hdnSelectedName.Value = value;
        }
    }


    /// <summary>
    /// Determines whether to display machine translation services.
    /// </summary>
    public bool DisplayMachineServices
    {
        get
        {
            return mDisplayMachineServices;
        }
        set
        {
            mDisplayMachineServices = value;
        }
    }


    /// <summary>
    /// Determines whether to display machine translation services.
    /// </summary>
    public bool DisplayTargetlanguage
    {
        get
        {
            return plcTargetLang.Visible;
        }
        set
        {
            plcTargetLang.Visible = value;
        }
    }


    /// <summary>
    /// String which will be addad as a suffix of each machine translation service.
    /// </summary>
    public string MachineServiceSuffix
    {
        get
        {
            if (mMachineServiceSuffix == null)
            {
                return GetString("translationservice.machineservicesuffix");
            }
            return mMachineServiceSuffix;
        }
        set
        {
            mMachineServiceSuffix = value;
        }
    }


    /// <summary>
    /// Indicates if there is any enabled service available in the selector.
    /// </summary>
    public bool AnyServiceAvailable
    {
        get
        {
            return mAnyServiceAvailable;
        }
    }


    /// <summary>
    /// Returns true if there is at least one source culture which can be used for translation.
    /// </summary>
    public bool SourceCultureAvailable
    {
        get
        {
            return selectCultureElem.UniSelector.HasData;
        }
    }

    #endregion


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing && !RequestHelper.IsCallback())
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(
                "function SelectService(serviceName, supportsInstructions, supportsPriority, supportsAttachments, supportsDeadline, generateTarget) { \n",
                "    var nameElem = document.getElementById('", hdnSelectedName.ClientID, "');  \n",
                "    var generateTargetElem = document.getElementById('", hdnGenerateTarget.ClientID, "');  \n",
                "    if (nameElem != null) {  \n",
                "        nameElem.value = serviceName;  \n",
                "    } \n",
                "    if (generateTargetElem != null) \n",
                "    { \n",
                "        generateTargetElem.value = generateTarget; \n",
                "        document.getElementById('pnlInstructions').style.display = (supportsInstructions ? '' : 'none'); \n",
                "        document.getElementById('pnlPriority').style.display = (supportsPriority ? '' : 'none'); \n",
                "        document.getElementById('pnlDeadline').style.display = (supportsDeadline ? '' : 'none'); \n",
                "        document.getElementById('pnlProcessBinary').style.display = (supportsAttachments ? '' : 'none'); \n",
                "        var selectButton = document.getElementById('rad' + serviceName); \n",
                "        if (selectButton != null) { \n",
                "            selectButton.checked = 'checked'; \n",
                "        } \n",
                "    } \n",
                "} \n"
                );

            ScriptHelper.RegisterClientScriptBlock(this.Page, typeof(string), "TranslationServiceSelector", sb.ToString(), true);

            string format = "{% CultureName %}{% if (CultureCode == \"" + defaultCulture + "\") { \" \" +\"" + GetString("general.defaultchoice") + "\" } %}"; ;
            selectCultureElem.UniSelector.DisplayNameFormat = format;
            selectTargetCultureElem.UniSelector.DisplayNameFormat = format;

            if (TranslationSettings != null)
            {
                string where = "NOT CultureCode = '" + SqlHelperClass.GetSafeQueryString(TranslationSettings.TargetLanguage) + "'";
                if (NodeID > 0)
                {
                    where = SqlHelperClass.AddWhereCondition(where, "CultureCode IN (SELECT DocumentCulture FROM CMS_Document WHERE DocumentNodeID = " + NodeID + ")", "AND");
                }
                selectCultureElem.AdditionalWhereCondition = where;
            }

            ReloadData();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!StopProcessing)
        {
            if (!RequestHelper.IsPostBack())
            {
                dtDeadline.SelectedDateTime = DateTime.Now.AddDays(7);
                selectCultureElem.Value = defaultCulture;
            }
        }
    }


    /// <summary>
    /// Selects correct value.
    /// </summary>
    private void ReloadData()
    {
        string where = "TranslationServiceEnabled = 1";
        if (!DisplayMachineServices)
        {
            where += " AND TranslationServiceIsMachine = 0";
        }

        bool allowBinary = SettingsKeyProvider.GetBoolValue(CMSContext.CurrentSiteName + ".CMSAllowAttachmentTranslation");

        DataSet ds = TranslationServiceInfoProvider.GetTranslationServices(where, "TranslationServiceIsMachine DESC, TranslationServiceDisplayName ASC", 0, "TranslationServiceDisplayName, TranslationServiceName, TranslationServiceIsMachine, TranslationServiceSupportsPriority, TranslationServiceSupportsInstructions, TranslationServiceSupportsDeadline, TranslationServiceGenerateTargetTag");
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            string selected = hdnSelectedName.Value;
            string lastDisplayName = null;

            int i = 0;
            int count = ds.Tables[0].Rows.Count;

            string machSelectScript = null;

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string codeName = ValidationHelper.GetString(dr["TranslationServiceName"], "");

                // Check availability
                if (TranslationServiceHelper.IsServiceAvailable(codeName, CMSContext.CurrentSiteName))
                {
                    if (string.IsNullOrEmpty(selected) && (i == 0))
                    {
                        selected = codeName;
                    }

                    bool isMachine = ValidationHelper.GetBoolean(dr["TranslationServiceIsMachine"], false);
                    string displayName = ValidationHelper.GetString(dr["TranslationServiceDisplayName"], "");
                    bool supportsInstructions = ValidationHelper.GetBoolean(dr["TranslationServiceSupportsInstructions"], false);
                    bool supportsPriority = ValidationHelper.GetBoolean(dr["TranslationServiceSupportsPriority"], false);
                    bool supportsDeadline = ValidationHelper.GetBoolean(dr["TranslationServiceSupportsDeadline"], false);
                    bool generateTargetTag = ValidationHelper.GetBoolean(dr["TranslationServiceGenerateTargetTag"], false);

                    if (isMachine && !string.IsNullOrEmpty(MachineServiceSuffix))
                    {
                        displayName += MachineServiceSuffix;
                    }

                    string selectScript = "SelectService(" + ScriptHelper.GetString(codeName) + ", " + (supportsInstructions ? "true" : "false") + ", " +
                                          (supportsPriority ? "true" : "false") + ", " + (!isMachine && allowBinary ? "true" : "false") + ", " +
                                          (supportsDeadline ? "true" : "false") + ", " + (generateTargetTag ? "true" : "false") + ")";

                    bool isSelected = selected.Equals(codeName, StringComparison.CurrentCultureIgnoreCase);
                    if (isSelected)
                    {
                        hdnSelectedName.Value = selected;
                        if (string.IsNullOrEmpty(machSelectScript))
                        {
                            machSelectScript = selectScript;
                        }
                    }

                    string radioBtn = "<input id=\"rad" + codeName + "\" " + (isSelected ? "checked=\"checked\"" : "") + " type=\"radio\" name=\"services\" value=\"" + codeName + "\" onclick=\"" + selectScript + "\" />";
                    radioBtn += "<label for=\"rad" + codeName + "\">" + HTMLHelper.HTMLEncode(displayName) + "</label>";
                    lastDisplayName = displayName;

                    mAnyServiceAvailable = true;

                    ltlServices.Text += radioBtn + (i < count - 1 ? "<br />" : "");
                    i++;
                }
            }

            // If only one service is available, display it in a different way
            if (i == 1)
            {
                DisplayedServiceName = lastDisplayName;
                if (DisplayOnlyServiceName)
                {
                    ltlServices.Text = "<strong>" + HTMLHelper.HTMLEncode(lastDisplayName) + "</strong>";
                }
                else
                {
                    ltlServices.Text = "";
                }
            }

            if ((i > 1) && !string.IsNullOrEmpty(machSelectScript))
            {
                // Register script if more than one service is available
                ScriptHelper.RegisterStartupScript(this.Page, typeof(string), "TranslationServiceSelectorSelection", machSelectScript, true);
            }

            DisplayedServicesCount = i;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Validates the data. Returns error msg if something is not ok.
    /// </summary>
    private string ValidateData()
    {
        if (dtDeadline.SelectedDateTime != DataHelper.DATETIME_NOT_SELECTED)
        {
            if (dtDeadline.SelectedDateTime < DateTime.Now)
            {
                return GetString("translationservice.invaliddeadline");
            }
        }
        return null;
    }


    /// <summary>
    /// Submits the node for translation. Does not check the permissions, you need to chek it before calling this method.
    /// </summary>
    public string SubmitToTranslation()
    {
        string err = ValidateData();
        if (!string.IsNullOrEmpty(err))
        {
            return err;
        }

        TranslationSettings settings = TranslationSettings ?? new TranslationSettings();
        if (string.IsNullOrEmpty(settings.TargetLanguage))
        {
            settings.TargetLanguage = CMSContext.PreferredCultureCode;
        }
        settings.TranslateWebpartProperties = SettingsKeyProvider.GetBoolValue(CMSContext.CurrentSiteName + ".CMSTranslateWebpartProperties");
        settings.SourceLanguage = FromLanguage;
        settings.Instructions = Instructions;
        settings.Priority = Priority;
        settings.ProcessBinary = ProcessBinary;
        settings.TranslateAttachments = ProcessBinary;
        settings.TranslationDeadline = Deadline;
        settings.TranslationServiceName = hdnSelectedName.Value;
        settings.GenerateTargetTag = ValidationHelper.GetBoolean(hdnGenerateTarget.Value, false);

        TreeProvider tree = TreeProvider;
        if (tree == null)
        {
            tree = new TreeProvider();
        }
        TreeNode node = DocumentHelper.GetDocument(NodeID, settings.SourceLanguage, true, tree); //tree.SelectSingleNode(NodeID, settings.SourceLanguage, true, settings.TranslateDocCoupledData);

        TranslationSubmissionInfo submissionInfo = null;

        return TranslationServiceHelper.SubmitToTranslation(settings, node, out submissionInfo);
    }

    #endregion
}