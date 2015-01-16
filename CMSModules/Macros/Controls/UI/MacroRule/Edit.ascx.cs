using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.SiteProvider;
using CMS.SettingsProvider;
using CMS.FormEngine;

public partial class CMSModules_Macros_Controls_UI_MacroRule_Edit : CMSAdminEditControl
{
    #region "Variables"

    protected string mResourceName = "";

    #endregion


    #region "Properties"

    /// <summary>
    /// Name of the resource for which the rule is edited.
    /// </summary>
    public string ResourceName
    {
        get
        {
            return mResourceName;
        }
        set
        {
            mResourceName = value;
        }
    }


    /// <summary>
    /// Gets or sets macro resolver name.
    /// </summary>
    public string ResolverName
    {
        get
        {
            return EditForm.ResolverName;
        }
        set
        {
            EditForm.ResolverName = value;
        }
    }


    /// <summary>
    /// UIForm control used for editing objects properties.
    /// </summary>
    public UIForm UIFormControl
    {
        get
        {
            return this.EditForm;
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
            this.EditForm.StopProcessing = value;
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
            EditForm.IsLiveSite = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        ShowInformation(GetString("macros.macrorule.ruletextinfo"));

        EditForm.OnBeforeSave += new EventHandler(EditForm_OnBeforeSave);
    }


    protected void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        MacroRuleInfo info = EditForm.EditedObject as MacroRuleInfo;
        if (info != null)
        {
            // Generate automatic fields when present in UserText
            FormEngineUserControl control = EditForm.FieldControls["MacroRuleText"];
            if (control != null)
            {
                string userText = ValidationHelper.GetString(control.Value, "");
                if (!string.IsNullOrEmpty(userText))
                {
                    Regex regex = RegexHelper.GetRegex("\\{[-_a-zA-Z0-9]*\\}");
                    MatchCollection match = regex.Matches(userText);
                    if (match.Count > 0)
                    {
                        FormInfo fi = new FormInfo(info.MacroRuleParameters);
                        foreach (Match m in match)
                        {
                            foreach (Capture c in m.Captures)
                            {
                                string name = c.Value.Substring(1, c.Value.Length - 2).ToLowerCSafe();
                                FormFieldInfo ffi = fi.GetFormField(name);
                                if (ffi == null)
                                {
                                    ffi = new FormFieldInfo();
                                    ffi.Name = name;
                                    ffi.DataType = FormFieldDataTypeEnum.Text;
                                    ffi.Size = 100;
                                    ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;
                                    ffi.Caption = "select operation";
                                    ffi.AllowEmpty = true;
                                    switch (name)
                                    {
                                        case "_is":
                                            ffi.DefaultValue = ";is";
                                            ffi.Settings["controlname"] = "MacroNegationOperator";
                                            ffi.Settings["EditText"] = "false";
                                            ffi.Settings["Options"] = ";is\r\n!;is not";
                                            break;

                                        case "_was":
                                            ffi.DefaultValue = ";was";
                                            ffi.Settings["controlname"] = "MacroNegationOperator";
                                            ffi.Settings["EditText"] = "false";
                                            ffi.Settings["Options"] = ";was\r\n!;was not";
                                            break;

                                        case "_will":
                                            ffi.DefaultValue = ";will";
                                            ffi.Settings["controlname"] = "MacroNegationOperator";
                                            ffi.Settings["EditText"] = "false";
                                            ffi.Settings["Options"] = ";will\r\n!;will not";
                                            break;

                                        case "_has":
                                            ffi.DefaultValue = ";has";
                                            ffi.Settings["controlname"] = "MacroNegationOperator";
                                            ffi.Settings["EditText"] = "false";
                                            ffi.Settings["Options"] = ";has\r\n!;does not have";
                                            break;

                                        case "_perfectum":
                                            ffi.DefaultValue = ";has";
                                            ffi.Settings["controlname"] = "MacroNegationOperator";
                                            ffi.Settings["EditText"] = "false";
                                            ffi.Settings["Options"] = ";has\r\n!;has not";
                                            break;

                                        case "_any":
                                            ffi.DefaultValue = "false;any";
                                            ffi.Settings["controlname"] = "macro_any-all_bool_selector";
                                            ffi.Settings["EditText"] = "false";
                                            ffi.Settings["Options"] = "false;any\r\ntrue;all";
                                            break;

                                        default:
                                            ffi.FieldType = FormFieldControlTypeEnum.TextBoxControl;
                                            ffi.Size = 1000;
                                            ffi.Caption = "enter text";
                                            break;
                                    }

                                    fi.AddFormField(ffi);
                                }
                            }
                        }
                        info.MacroRuleParameters = fi.GetXmlDefinition();
                    }
                }
            }
        }

        if (!string.IsNullOrEmpty(ResourceName))
        {
            EditForm.EditedObject.SetValue("MacroRuleResourceName", ResourceName);
        }
        EditForm.EditedObject.SetValue("MacroRuleIsCustom", !SettingsKeyProvider.DevelopmentMode);
    }

    #endregion
}

