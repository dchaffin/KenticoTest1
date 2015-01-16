using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.UIControls;
using CMS.Newsletter;
using CMS.SettingsProvider;
using CMS.SiteProvider;

public partial class CMSModules_Newsletters_Controls_VariantSlider : CMSUserControl
{
    private List<IssueABVariantItem> mVariants = null;
    private int mIssueID = 0;
    private bool mEditingEnabled = true;


    #region "Public properties"

    /// <summary>
    /// Gets or sets newsletter issue ID. Variants list is refreshed automatically
    /// when issue ID is about to change.
    /// </summary>
    public int IssueID
    {
        get
        {
            return mIssueID;
        }
        set
        {
            if (mIssueID != value)
            {
                mIssueID = value;
                if (mIssueID > 0)
                {
                    List<IssueABVariantItem> variants = IssueHelper.GetIssueVariants(mIssueID, null);
                    mVariants = variants;
                }
                variantDailog.IssueID = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets list of variants.
    /// </summary>
    public List<IssueABVariantItem> Variants
    {
        get
        {
            if (mVariants == null)
            {
                mVariants = new List<IssueABVariantItem>();
            }
            return mVariants;
        }
        set
        {
            mVariants = value;
        }
    }


    /// <summary>
    /// Gets or sets current variant (zero based index from the list).
    /// </summary>
    public int CurrentVariant
    {
        get
        {
            int currentPos = 0;
            if (IsRTL)
            {
                currentPos = ValidationHelper.GetInteger(hdnSliderPosition.Text, Variants.Count);
                return Variants.Count - currentPos;
            }
            else
            {
                currentPos = ValidationHelper.GetInteger(hdnSliderPosition.Text, 1);
            }
            return currentPos - 1;
        }
        private set
        {
            if (IsRTL)
            {
                value = Variants.Count - value;
            }
            else
            {
                value++;
            }
            txtSlider.Text = value.ToString();
            hdnSliderPosition.Text = value.ToString();
        }
    }


    /// <summary>
    /// ID of original issue. Value is valid only when A/B testing
    /// issue becomes regular issue.
    /// </summary>
    public int OriginalIssueID
    {
        get;
        private set;
    }


    /// <summary>
    /// Resource string for "add variant". If NULL default string is used.
    /// </summary>
    public string ToolTipAddVariant
    {
        get;
        set;
    }


    /// <summary>
    /// Resource string for "remove variant". If NULL default string is used.
    /// </summary>
    public string ToolTipRemoveVariant
    {
        get;
        set;
    }


    /// <summary>
    /// Resource string for "properties". If NULL default string is used.
    /// </summary>
    public string ToolTipProperties
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets value that indicates whether editing buttons is allowed.
    /// </summary>
    public bool EditingEnabled
    {
        get { return mEditingEnabled; }
        set { mEditingEnabled = value; }
    }


    /// <summary>
    /// Returns TRUE if current culture is RTL culture :-).
    /// </summary>
    private bool IsRTL
    {
        get
        {
            return CultureHelper.IsUICultureRTL(); 
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Occurs when current selection was changed.
    /// </summary>
    public event EventHandler OnSelectionChanged;


    /// <summary>
    /// Occurs when selected variant was deleted.
    /// </summary>
    public event EventHandler OnVariantDeleted;


    /// <summary>
    /// Occurs when new variant was created.
    /// </summary>
    public event EventHandler OnVariantAdded;

    #endregion


    #region "Page methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        sliderExtender.HandleImageUrl = GetImageUrl("CMSModules/CMS_Newsletter/slider.gif");

        imgRemoveVariant.Style.Add("display", "block");

        txtSlider.Attributes.Add("onchange", "$j('#" + hdnSliderPosition.ClientID + "').change();");
        txtSlider.Style.Add("display", "none");

        int totalVariants = Variants.Count;
        if (!RequestHelper.IsPostBack())
        {
            CurrentVariant = 0;
        }
        hdnSliderPosition.Style.Add("display", "none");
        hdnSliderPosition.Attributes.Add("onchange", "OnHiddenChanged_" + this.ClientID + "(this, document.getElementById('" + lblPart.ClientID + "'), document.getElementById('" + lblVariantName.ClientID  + "'), '" + sliderExtender.BehaviorID + @"' );");

        imgAddVariant.ToolTip = (ToolTipAddVariant == null ? GetString("newsletterslider.addvariant") : GetString(ToolTipAddVariant));
        imgRemoveVariant.ToolTip = (ToolTipRemoveVariant == null ? GetString("newsletterslider.removevariant") : GetString(ToolTipRemoveVariant));
        imgVariantProperties.ToolTip = (ToolTipProperties == null ? GetString("newsletterslider.properties") : GetString(ToolTipProperties));

        variantDailog.OnAddVariant += new EventHandler(variantDailog_OnAddVariant); 
        variantDailog.OnDeleteVariant += new EventHandler(variantDailog_OnDeleteVariant);
        variantDailog.OnUpdateVariant += new EventHandler(variantDailog_OnUpdateVariant);

        variantDailog.Variants = Variants;

        sliderExtender.Minimum = 1;
        sliderExtender.Maximum = totalVariants;
        sliderExtender.Steps = totalVariants;

        btnSubmit.Click += new EventHandler(btnSubmit_Click);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Enable/disable buttons
        imgAddVariant.Enabled = EditingEnabled;
        imgVariantProperties.Enabled = EditingEnabled;
        imgRemoveVariant.Enabled = EditingEnabled;

        if (!EditingEnabled)
        {
            imgAddVariant.Attributes.CssStyle.Add("cursor", "default");
            imgVariantProperties.Attributes.CssStyle.Add("cursor", "default");
            imgRemoveVariant.Attributes.CssStyle.Add("cursor", "default");
        }

        // Load correct image
        imgAddVariant.ImageUrl = GetImageUrl(String.Format("CMSModules/CMS_Newsletter/variantadd{0}.png", (!imgAddVariant.Enabled ? "_disabled" : String.Empty)));
        imgVariantProperties.ImageUrl = GetImageUrl(String.Format("CMSModules/CMS_Newsletter/variantedit{0}.png", (!imgVariantProperties.Enabled ? "_disabled" : String.Empty)));
        imgRemoveVariant.ImageUrl = GetImageUrl(String.Format("CMSModules/CMS_Newsletter/variantdelete{0}.png", (!imgRemoveVariant.Enabled ? "_disabled" : String.Empty)));

        int totalVariants = Variants.Count;
        variantDailog.Variants = Variants;

        sliderExtender.Minimum = 1;
        sliderExtender.Maximum = totalVariants;
        sliderExtender.Steps = totalVariants;
        lblTotal.Text = totalVariants.ToString();

        InitScript(totalVariants);

        int i = CurrentVariant;
        if (i >= totalVariants)
        {
            i = totalVariants - 1;
            CurrentVariant = i;
        }

        if (i < totalVariants)
        {
            lblVariantName.Text = HTMLHelper.HTMLEncode(Variants[i].IssueVariantName) + " " +
                (Variants[i].IsWinner? HTMLHelper.HTMLEncode(GetString("newsletterabtest.winner")) : String.Empty);
        }
        i++;
        lblPart.Text = i.ToString();
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Sets slider to the given variant.
    /// </summary>
    /// <param name="issueId">Issue ID</param>
    public void SetVariant(int issueId)
    {
        int i = Variants.Count - 1;
        for (; (i >= 0) && (Variants[i].IssueID != issueId); i--) ;

        if (i < 0)
        {
            i = 0;
        }

        CurrentVariant = i;
        lblABVar.Text = i.ToString();
    }

    #endregion


    #region "Dialog handling methods"

    protected void variantDailog_OnUpdateVariant(object sender, EventArgs e)
    {
        VariantEventArgs args = null;
        if (!(e is VariantEventArgs))
        {
            return;
        }
        args = (VariantEventArgs)e;

        int currVariant = CurrentVariant;
        IssueABVariantItem issueVariant = Variants[currVariant];
        IssueInfo issue = IssueInfoProvider.GetIssueInfo(issueVariant.IssueID);
        if (issue != null)
        {
            issue.IssueVariantName = args.DisplayName;
            IssueInfoProvider.SetIssueInfo(issue);
            Variants[currVariant].IssueVariantName = args.DisplayName;
        }
    }


    protected void variantDailog_OnDeleteVariant(object sender, EventArgs e)
    {
        if (Variants.Count <= 0)
        {
            return;
        }

        // Allow modifying issues in idle state only
        IssueInfo parentIssue = IssueInfoProvider.GetOriginalIssue(IssueID);
        if ((parentIssue == null) || (parentIssue.IssueStatus != IssueStatusEnum.Idle))
        {
            return;
        }

        int currVariant = CurrentVariant;
        if (Variants.Count > 2)
        {
            IssueInfoProvider.DeleteIssueInfo(Variants[currVariant].IssueID);
            Variants.RemoveAt(currVariant);
            RaiseVariantDeleted();
        }
        else
        {
            if (Variants.Count == 2)
            {
                // Remove variant
                IssueInfoProvider.DeleteIssueInfo(Variants[currVariant].IssueID);
                Variants.RemoveAt(currVariant);
            }

            // Get remaining variant
            IssueABVariantItem issueVariant = Variants[0];
            Variants.Clear();
            IssueInfo issue = IssueInfoProvider.GetIssueInfo(issueVariant.IssueID);
            if (issue != null)
            {
                // Transfer all data from child to parent and delete child
                string ignoredColumns = ";issueid;issueguid;issuevariantname;";
                foreach (string column in parentIssue.ColumnNames)
                {
                    if (!ignoredColumns.Contains(String.Format(";{0};", column.ToLowerCSafe())))
                    {
                        parentIssue.SetValue(column, issue.GetValue(column));
                    }
                }
                parentIssue.IssueIsABTest = false;
                parentIssue.IssueVariantName = null;
                parentIssue.IssueVariantOfIssueID = 0;
                IssueInfoProvider.SetIssueInfo(parentIssue);

                // Delete files from parent issue if any
                MetaFileInfoProvider.DeleteFiles(parentIssue.IssueID, NewsletterObjectType.NEWSLETTERISSUE, MetaFileInfoProvider.OBJECT_CATEGORY_ISSUE);
                MetaFileInfoProvider.DeleteFiles(parentIssue.IssueID, NewsletterObjectType.NEWSLETTERISSUEVARIANT, MetaFileInfoProvider.OBJECT_CATEGORY_ISSUE);

                // Move attachments (meta files) from child to parent
                MetaFileInfoProvider.MoveMetaFiles(issue.IssueID, parentIssue.IssueID, NewsletterObjectType.NEWSLETTERISSUEVARIANT, MetaFileInfoProvider.OBJECT_CATEGORY_ISSUE, NewsletterObjectType.NEWSLETTERISSUE, MetaFileInfoProvider.OBJECT_CATEGORY_ISSUE);
                MetaFileInfoProvider.MoveMetaFiles(issue.IssueID, parentIssue.IssueID, NewsletterObjectType.NEWSLETTERISSUE, MetaFileInfoProvider.OBJECT_CATEGORY_ISSUE, NewsletterObjectType.NEWSLETTERISSUE, MetaFileInfoProvider.OBJECT_CATEGORY_ISSUE);

                // Delete last variant
                IssueInfoProvider.DeleteIssueInfo(issue);
                OriginalIssueID = parentIssue.IssueID;

                // A/B variants have been deleted => deleting A/B test itself
                ABTestInfo abi = ABTestInfoProvider.GetABTestInfoForIssue(OriginalIssueID);
                ABTestInfoProvider.DeleteABTestInfo(abi);
            }
            RaiseVariantDeleted();
        }
    }


    protected void variantDailog_OnAddVariant(object sender, EventArgs e)
    {
        VariantEventArgs args = null;
        if (!(e is VariantEventArgs))
        {
            return;
        }
        args = (VariantEventArgs)e;

        if (args.ID > 0)
        {
            this.IssueID = args.ID;
            SetVariant(args.ID);
            RaiseVariantAdded();
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Raises "delete" event.
    /// </summary>
    protected void RaiseVariantDeleted() 
    {
        if (OnVariantDeleted != null)
        {
            OnVariantDeleted(this, EventArgs.Empty);
        }
    }


    /// <summary>
    /// Raises "add" event.
    /// </summary>
    protected void RaiseVariantAdded()
    {
        if (OnVariantAdded != null)
        {
            OnVariantAdded(this, EventArgs.Empty);
        }
    }


    /// <summary>
    /// Initialize javascript.
    /// </summary>
    /// <param name="totalVariants">Number of all variants</param>
    private void InitScript(int totalVariants) 
    {
        StringBuilder variants = new StringBuilder();
        StringBuilder variantsText = new StringBuilder();
        StringBuilder variantsWinner = new StringBuilder();
        foreach (IssueABVariantItem variant in Variants)
        {
            variants.Append(ScriptHelper.GetString(HTMLHelper.HTMLEncode(variant.IssueVariantName)));
            variants.Append(", ");
            variantsText.Append(ScriptHelper.GetString(variant.IssueVariantName));
            variantsText.Append(", ");
            variantsWinner.Append(variant.IsWinner ? ScriptHelper.GetString(GetString("newsletterabtest.winner")) : "''");
            variantsWinner.Append(", ");
        }
        variants.Append("''");
        variantsText.Append("''");
        variantsWinner.Append("''");
        StringBuilder scr = new StringBuilder();
        scr.AppendFormat(
@"var variantsName_{0} = new Array({1});
var variantsNameText_{0} = new Array({2});
var variantsWinner_{0} = new Array({3});", ClientID, variants, variantsText, variantsWinner);
        scr.AppendLine();
        scr.AppendFormat(
@"function GetCurrVariantText_{0}() {{
  var hdnEl = document.getElementById('" + hdnSliderPosition.ClientID + @"');
  var variant = parseInt(hdnEl.value); {1}  
  if (variant > 0 && variant <= variantsNameText_{0}.length) {{ return variantsNameText_{0}[variant-1]; }}
  return '';
}}
function OnHiddenChanged_{0}(hdnEl, lblPartEl, lblVariantEl, sliderExtenderId) {{
    var sliderExtender = $find(sliderExtenderId);
    var newPosition = hdnEl.value;
    var variant = parseInt(newPosition); {1}
    sliderExtender.set_Value(newPosition);
    if (variant > 0) {{ lblPartEl.innerHTML = variant; lblVariantEl.innerHTML = variantsName_{0}[variant-1] + ' ' + variantsWinner_{0}[variant-1];" +
     Page.ClientScript.GetPostBackEventReference(btnSubmit, null) +
@"    }}
}}", ClientID, (IsRTL ? "variant = " + totalVariants + "-variant+1;" : ""));

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "SliderScripts_" + ClientID, scr.ToString(), true);

        string scriptTemplate = "ShowVariantDialog_" + this.variantDailog.ClientID + "('{0}', {1}); return false;";
        imgAddVariant.OnClientClick = String.Format(scriptTemplate, "addvariant", "''");
        imgRemoveVariant.OnClientClick = String.Format(scriptTemplate, "removevariant", "''");
        imgVariantProperties.OnClientClick = String.Format(scriptTemplate, "properties", "GetCurrVariantText_" + ClientID + "()");
    }


    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (OnSelectionChanged != null)
        {
            OnSelectionChanged(this, EventArgs.Empty);
        }
    }

    #endregion
}