using System;
using System.Text;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.Newsletter;
using CMS.SettingsProvider;

public partial class CMSModules_Newsletters_Controls_GroupSizeSlider : CMSAdminControl
{
    #region "Private variables"

    private bool mEnabled = true;

    private string[] COLORS = new string[] { "#FFD1D1", "#D1FFD1", "#D1E0FF", "#FFFFD1", "#C6FFEC", "#FFD1FF" };

    private string[] COLORS_GREY = new string[] { "#DEDEDE", "#ECECEC", "#DFDFDF", "#F9F9F9", "#EBEBEB", "#E3E3E3" };

    private List<IssueABVariantItem> mVariants = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets variants information (names).
    /// </summary>
    public List<IssueABVariantItem> Variants
    {
        get
        {
            if (mVariants == null)
            {
                mVariants = new List<IssueABVariantItem>();
                mVariants.Add(new IssueABVariantItem(0, String.Empty, false, IssueStatusEnum.Idle));
            }
            return mVariants;
        }
        set
        {
            mVariants = value;
        }
    }


    /// <summary>
    /// Gets or sets slider position (in percent).
    /// </summary>
    public int CurrentSize
    {
        get
        {
            if (IsRTL)
            {
                return 100 - ValidationHelper.GetInteger(hdnSize.Value, 100);
            }
            return ValidationHelper.GetInteger(hdnSize.Value, 0);
        }
        set
        {
            if (IsRTL)
            {
                hdnSize.Value = Convert.ToString(100 - value);

            }
            else
            {
                hdnSize.Value = value.ToString();
            }
        }
    }


    /// <summary>
    /// Gets or sets number of total subscribers.
    /// </summary>
    public int NumberOfSubscribers
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets number of subscribers in test group (if set value in CurrentSize is ignored).
    /// </summary>
    public int NumberOfTestSubscribers
    {
        get;
        set;
    }


    /// <summary>
    /// Enables/disables control.
    /// </summary>
    public bool Enabled
    {
        get 
        { 
            return mEnabled;
        }
        set
        {
            mEnabled = value;
        }
    }


    /// <summary>
    /// Indicates if slider shows final results.
    /// </summary>
    private bool ShowFinalResults
    {
        get { return NumberOfTestSubscribers > 0; }
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


    #region "Methods"

    /// <summary>
    /// Reloads control data.
    /// </summary>
    /// <param name="forceReload">Indicates if force reload should be used</param>
    public override void ReloadData(bool forceReload)
    {
        InitVariantBoxes();
        InitJQuerySlider();
    }


    /// <summary>
    /// Inits boxes representing variants.
    /// </summary>
    private void InitVariantBoxes()
    {
        bool isRTL = IsRTL;
        int count = Variants.Count;
        int currentPercent = 0;
        int prevPercent = 0;
        pnlRes.Controls.Clear();
        for (int i = 0; i < count; i++)
        {
            currentPercent = (100 * i + 100) / count;

            Panel pnl = new Panel();
            pnl.ID = pnlRes.ClientID + "pnl" + i.ToString();
            pnl.Style.Add("background-color", (Enabled ? COLORS[i % COLORS.Length] : COLORS_GREY[i % COLORS_GREY.Length]));
            if (!isRTL && (i == 0))
            {
                pnl.CssClass = "firstbox";
            }
            else if (isRTL && (i == count - 1))
            {
                pnl.CssClass = "firstboxrtl";
            }
            else
            {
                pnl.CssClass = "otherbox";
            }
            if (i != count - 1)
            {
                pnl.Width = new Unit(String.Format("{0}%", currentPercent - prevPercent));
            }
            else 
            {
                // Special width for the last box due to strange behaviour of IE
                pnl.Width = new Unit(String.Format("{0}.99%", Math.Max(currentPercent - prevPercent - 1, 0)), CultureHelper.EnglishCulture);
            }

            pnl.ToolTip = HTMLHelper.HTMLEncode(Variants[i].IssueVariantName);
            pnl.Visible = true;
            pnlRes.Controls.Add(pnl);
            prevPercent = currentPercent;
        }
    }


    /// <summary>
    /// Initializes JQuery slider
    /// </summary>
    private void InitJQuerySlider()
    {
        ScriptHelper.RegisterJQueryUI(this.Page);

        int varCount = Variants.Count;
        int subscribersCount = NumberOfSubscribers;

        int minPercent = 0;
        int subsCount = (subscribersCount <= 0 ? 1 : subscribersCount);
        if (!ShowFinalResults)
        {
            // Get minimal number of percent to set (or maximum under RTL culture)
            minPercent = (int)Math.Ceiling((double)(100 * varCount) / subsCount);
            if (minPercent <= 0) { minPercent = 1; }
            if (minPercent > 100) { minPercent = 100; }

            if (Enabled && (CurrentSize < minPercent))
            {
                CurrentSize = minPercent;
            }

            if (IsRTL)
            {
                minPercent = 100 - minPercent;
            }
        }
        else
        {
            minPercent = 100 * NumberOfTestSubscribers / subsCount;
            if (minPercent <= 0) { minPercent = 1; }
            if (minPercent > 100) { minPercent = 100; }
            CurrentSize = minPercent;
        }

        StringBuilder sb = new StringBuilder();
        if (!ShowFinalResults)
        {
            // Returns correct number of subscribers for specified slider position
            sb.Append("function getSubs(perc, varsCount, subsCount) {");
            sb.Append("  var subs = (perc / 100) * subsCount;");
            sb.Append("  var newSubs = Math.floor(subs / varsCount) * varsCount;");
            sb.Append("  return newSubs; }");
            sb.AppendLine();
        }
        else
        {
            // Returns correct number of subscribers for specified slider position
            sb.Append("function getSubs(perc, varsCount, subsCount) {");
            sb.Append("  return ", NumberOfTestSubscribers, "; }");
            sb.AppendLine();
        }

        // Returns TRUE if slide is allowed (i.e. between allowed range)
        sb.Append("function onSlide_", this.ClientID, "(newPos) { ");
        if (IsRTL)
        {
            sb.Append(" if (newPos > ", minPercent, ") { ");
        }
        else
        {
            sb.Append(" if (newPos < ", minPercent, ") { ");
        }
        sb.Append("   jQuery('#", pnlSlider.ClientID, "').slider('option', 'value',", minPercent, ");");
        sb.Append("   onChange_", this.ClientID, @"(", minPercent, ");");
        sb.Append("   return false; }");
        sb.Append(" return true; ");
        sb.Append(" }");
        sb.AppendLine();
        // Resizes variant boxes and updates info message
        sb.Append("function onChange_", this.ClientID, @"(newPos) { jQuery('#",
                hdnSize.ClientID, "').val(newPos); ",
                (IsRTL?" newPos=100-newPos;":""),
                "var neww=newPos; if (neww<10) neww=10; if (neww>90) neww=90; jQuery('#",
                pnlRes.ClientID, "').width(newPos + '%'); jQuery('#", cellSub.ClientID, @"').width(neww + '%');",
                " var subs=getSubs(newPos, ", varCount, ", ", subscribersCount, "); ",
                " var msg=", ScriptHelper.GetString(GetString("newsletterissue_send.lbltestgroupsize")),
                "; jQuery('#" + lblTestGroup.ClientID + "').text(msg.replace('###', subs).replace('##', newPos)); var msg=",
                ScriptHelper.GetString(GetString("newsletterissue_send.lbltestgroupremainder")),
                "; var reminderCount = ", subscribersCount, "-subs; if (reminderCount < 0) { reminderCount = 0; }",
                "; jQuery('#", lblRemainder.ClientID, "').text(msg.replace('###', reminderCount).replace('##', 100-newPos)); }");
        sb.AppendLine();

        int currentSize = CurrentSize;
        if (IsRTL)
        {
            currentSize = 100 - currentSize;
        }
        // Initializes slider
        sb.Append("jQuery(function() { jQuery('#", pnlSlider.ClientID,
            @"').slider( { value: ", currentSize, (!Enabled ? ", disabled: true" : String.Empty), ", slide: function(event, ui) {");
        if (Enabled) 
        {
            sb.Append(" if (onSlide_", this.ClientID, "(ui.value)) { onChange_", this.ClientID, "(ui.value); return true; }; return false; ");
        }
        sb.Append(" } }) });");
        sb.AppendLine();
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "SliderMethods_" + this.ClientID, ScriptHelper.GetScript(sb.ToString()));

        // Updates variants boxes for the first time
        string script = " onChange_" + this.ClientID + "(" + currentSize + ");";
        ScriptHelper.RegisterStartupScript(this, typeof(string), "InitSlider_" + this.ClientID, ScriptHelper.GetScript(script));
    }

    #endregion
}