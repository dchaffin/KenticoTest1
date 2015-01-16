using System;
using System.Text.RegularExpressions;

using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.GlobalHelper;

public partial class CMSFormControls_SQL_WhereCondition : SqlFormControl
{
    /// <summary>
    /// Editing textbox
    /// </summary>
    protected override CMSTextBox TextBoxControl
    {
        get
        {
            return txtWhere;
        }
    }


    /// <summary>
    /// Gets the regular expression for the safe value
    /// </summary>
    protected override Regex GetSafeRegEx()
    {
        return SecurityHelper.WhereRegex;
    }


    protected new void Page_Load(object sender, EventArgs e)
    {
        CheckMinMaxLength = true;
        CheckRegularExpression = true;
    }
}