using System;
using System.Text.RegularExpressions;

using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.GlobalHelper;

public partial class CMSFormControls_SQL_OrderBy : SqlFormControl
{
    /// <summary>
    /// Editing textbox
    /// </summary>
    protected override CMSTextBox TextBoxControl
    {
        get
        {
            return txtOrder;
        }
    }


    /// <summary>
    /// Gets the regular expression for the safe value
    /// </summary>
    protected override Regex GetSafeRegEx()
    {
        return SecurityHelper.OrderByRegex;
    }


    protected new void Page_Load(object sender, EventArgs e)
    {
        CheckMinMaxLength = true;
        CheckRegularExpression = true;
    }
}