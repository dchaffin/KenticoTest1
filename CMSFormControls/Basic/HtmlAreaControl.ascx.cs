using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using System.Collections.Generic;

using CMS.CKEditor;
using CMS.CMSHelper;
using CMS.Controls;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;

public partial class CMSFormControls_Basic_HtmlAreaControl : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return editor.Enabled;
        }
        set
        {
            editor.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            return editor.ResolvedValue;
        }
        set
        {
            editor.ResolvedValue = ValidationHelper.GetString(value, null);
        }
    }


    /// <summary>
    /// Toolbar used in editor.
    /// </summary>
    public string ToolbarSet
    {
        get
        {
            return editor.ToolbarSet;
        }
        set
        {
            editor.ToolbarSet = value;
        }
    }


    /// <summary>
    /// Gets current editor.
    /// </summary>
    public CMSHtmlEditor CurrentEditor
    {
        get
        {
            return editor;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set control properties
        editor.AutoDetectLanguage = false;
        editor.DefaultLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

        string toolbarSet = editor.ToolbarSet;

        if (Form != null)
        {
            editor.DialogParameters = Form.DialogParameters;
        }

        // Get editor area toolbar 
        toolbarSet = DataHelper.GetNotEmpty(GetValue("toolbarset"), (Form != null) ? Form.HtmlAreaToolbar : "");
        editor.ToolbarSet = toolbarSet;
        editor.ToolbarLocation = DataHelper.GetNotEmpty(GetValue("toolbarlocation"), (Form != null) ? Form.HtmlAreaToolbarLocation : "");

        // Set form dimensions
        UnitType widthUnitType = GetUnitType(GetValue("widthunittype"));
        UnitType heightUnitType = GetUnitType(GetValue("heightunittype"));
        editor.Width = new Unit(ValidationHelper.GetInteger(GetValue("width"), 700), widthUnitType);
        editor.Height = new Unit(ValidationHelper.GetInteger(GetValue("height"), 300), heightUnitType);

        // Get editor area starting path
        String startingPath = ValidationHelper.GetString(GetValue("startingpath"), "");
        editor.StartingPath = startingPath;

        // Set current context resolver
        editor.ResolverName = ResolverName;

        // Get editor area css file
        string mCssStylesheet = ValidationHelper.GetString(GetValue("cssstylesheet"), "");
        if (!String.IsNullOrEmpty(mCssStylesheet))
        {
            editor.EditorAreaCSS = CSSHelper.GetStylesheetUrl(mCssStylesheet);
        }
        else if (toolbarSet.EqualsCSafe("Wireframe", true))
        {
            // Special case for wireframe editor
            editor.EditorAreaCSS = "~/CMSAdminControls/CKeditor/skins/kentico/wireframe.css";
        }
        else if (CMSContext.CurrentSite != null)
        {
            editor.EditorAreaCSS = FormHelper.GetHtmlEditorAreaCss(CMSContext.CurrentSiteName);
        }

        // Set live site info
        editor.IsLiveSite = IsLiveSite;

        // Set direction
        if (CultureHelper.IsPreferredCultureRTL())
        {
            editor.ContentsLangDirection = LanguageDirection.RightToLeft;
        }
        else
        {
            editor.ContentsLangDirection = LanguageDirection.LeftToRight;
        }

        // Get dialog configuration
        DialogConfiguration mediaConfig = GetDialogConfiguration(FieldInfo);
        if (mediaConfig != null)
        {
            // Override starting path from main configuration
            if (!String.IsNullOrEmpty(startingPath))
            {
                mediaConfig.ContentStartingPath = startingPath;
                mediaConfig.LibStartingPath = startingPath;
            }

            // Set configuration for 'Insert image or media' dialog                        
            editor.MediaDialogConfig = mediaConfig;
            // Set configuration for 'Insert link' dialog        
            editor.LinkDialogConfig = mediaConfig.Clone();
            // Set configuration for 'Quickly insert image' dialog
            editor.QuickInsertConfig = mediaConfig.Clone();
        }

        // Set CSS settings
        if (!String.IsNullOrEmpty(ControlStyle))
        {
            editor.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }
        if (!String.IsNullOrEmpty(CssClass))
        {
            editor.CssClass = CssClass;
            CssClass = null;
        }

        CheckRegularExpression = true;
        CheckFieldEmptiness = true;
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        string plainText = editor.ResolvedValue;
        int maxControlSize = 0;

        // Check min/max text length
        if (GetValue("size") != null)
        {
            maxControlSize = ValidationHelper.GetInteger(GetValue("size"), 0);
        }
        if (FieldInfo != null)
        {
            if ((FieldInfo.MaxStringLength > -1) && ((maxControlSize > FieldInfo.MaxStringLength) || (maxControlSize == 0)))
            {
                maxControlSize = FieldInfo.MaxStringLength;
            }
        }
        if ((maxControlSize > 0) || ((FieldInfo != null) && (FieldInfo.MinStringLength > 0)))
        {
            plainText = Regex.Replace(plainText, @"(>)(\r|\n)*(<)", "><");
            plainText = Regex.Replace(plainText, "(<[^>]*>)([^<]*)", "$2");
            // Just substitute spec.chars with one letter
            plainText = Regex.Replace(plainText, "(&#x?[0-9]{2,4};|&quot;|&amp;|&nbsp;|&lt;|&gt;|&euro;|&copy;|&reg;|&permil;|&Dagger;|&dagger;|&lsaquo;|&rsaquo;|&bdquo;|&rdquo;|&ldquo;|&sbquo;|&rsquo;|&lsquo;|&mdash;|&ndash;|&rlm;|&lrm;|&zwj;|&zwnj;|&thinsp;|&emsp;|&ensp;|&tilde;|&circ;|&Yuml;|&scaron;|&Scaron;)", "@");

            string error = null;
            int minControlSize = 0;
            if (FieldInfo != null)
            {
                minControlSize = FieldInfo.MinStringLength;
            }
            bool valid = CheckLength(minControlSize, maxControlSize, plainText.Length, ref error, ErrorMessage);
            ValidationError = error;
            return valid;
        }
        return true;
    }


    /// <summary>
    /// Returns the arraylist of the field IDs (Client IDs of the inner controls) that should be spell checked.
    /// </summary>
    public override List<string> GetSpellCheckFields()
    {
        List<string> result = new List<string>();
        result.Add(editor.ClientID);
        return result;
    }


    private UnitType GetUnitType(object unitTypeValue)
    {
        if (unitTypeValue != null)
        {
            switch (unitTypeValue.ToString().ToLowerCSafe())
            {
                case "px":
                    return UnitType.Pixel;

                case "em":
                    return UnitType.Em;

                case "percentage":
                    return UnitType.Percentage;
            }
        }
        return UnitType.Pixel;
    }

    #endregion
}