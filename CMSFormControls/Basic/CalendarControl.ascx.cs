using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;

public partial class CMSFormControls_Basic_CalendarControl : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return timePicker.Enabled;
        }
        set
        {
            timePicker.Enabled = value;
        }
    }


    /// <summary>
    /// If true, macros are allowed 
    /// </summary>
    public bool AllowMacros
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowMacros"), false);
        }
        set
        {
            SetValue("AllowMacros", value);
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            String value = timePicker.DateTimeTextBox.Text;
            if (AllowMacros && ValidationHelper.IsMacro(value))
            {
                return value;
            }

            if (timePicker.SelectedDateTime == DateTimeHelper.ZERO_TIME)
            {
                return null;
            }
            else
            {
                return timePicker.SelectedDateTime;
            }
        }
        set
        {
            if (GetValue("timezonetype") != null)
            {
                timePicker.TimeZone = TimeZoneInfoProvider.GetTimeZoneTypeEnum(ValidationHelper.GetString(GetValue("timezonetype"), ""));
            }
            if (GetValue("timezone") != null)
            {
                timePicker.CustomTimeZone = TimeZoneInfoProvider.GetTimeZoneInfo(ValidationHelper.GetString(GetValue("timezone"), ""));
            }

            string strValue = ValidationHelper.GetString(value, "");
            string strValueLowered = strValue.ToLowerCSafe();

            if (AllowMacros && ValidationHelper.IsMacro(strValue))
            {
                timePicker.DateTimeTextBox.Text = strValue;
                return;
            }

            if ((strValueLowered == DateTimeHelper.MACRO_DATE_TODAY.ToLowerCSafe()) || (strValueLowered == DateTimeHelper.MACRO_TIME_NOW.ToLowerCSafe()))
            {
                timePicker.SelectedDateTime = DateTime.Now;
            }
            else
            {
                timePicker.SelectedDateTime = ValidationHelper.GetDateTimeSystem(value, DateTimeHelper.ZERO_TIME);
            }
        }
    }


    /// <summary>
    /// Gets or sets if calendar control enables to edit time.
    /// </summary>
    public bool EditTime
    {
        get
        {
            return timePicker.EditTime;
        }
        set
        {
            timePicker.EditTime = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup control
        if (FieldInfo != null)
        {
            timePicker.AllowEmptyValue = FieldInfo.AllowEmpty;
        }
        timePicker.DisplayNow = ValidationHelper.GetBoolean(GetValue("displaynow"), true);
        timePicker.EditTime = ValidationHelper.GetBoolean(GetValue("edittime"), EditTime);
        timePicker.SupportFolder = "~/CMSAdminControls/Calendar";
        timePicker.DateTimeTextBox.CssClass = "EditingFormCalendarTextBox";
        timePicker.IsLiveSite = IsLiveSite;

        if (!String.IsNullOrEmpty(CssClass))
        {
            timePicker.CssClass = CssClass;
            CssClass = null;
        }
        if (!String.IsNullOrEmpty(ControlStyle))
        {
            timePicker.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }

        CheckFieldEmptiness = false;
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        // Check value
        string strValue = timePicker.DateTimeTextBox.Text.Trim();
        if ((FieldInfo != null) && !FieldInfo.AllowEmpty && String.IsNullOrEmpty(strValue))
        {
            // Empty error
            if (ErrorMessage != null)
            {
                if (ErrorMessage != ResHelper.GetString("BasicForm.InvalidInput"))
                {
                    ValidationError = ErrorMessage;
                }
                else
                {
                    ValidationError += ResHelper.GetString("BasicForm.ErrorEmptyValue");
                }
            }
            return false;
        }

        if (AllowMacros && ValidationHelper.IsMacro(strValue))
        {
            return true;
        }

        if ((((FieldInfo != null) && !FieldInfo.AllowEmpty) || !String.IsNullOrEmpty(strValue)) && (ValidationHelper.GetDateTime(strValue, DateTimeHelper.ZERO_TIME) == DateTimeHelper.ZERO_TIME))
        {
            DateTime showDate = new DateTime(2005, 1, 31);
            DateTime showDateTime = new DateTime(2005, 1, 31, 11, 59, 59);

            if (timePicker.EditTime)
            {
                // Error invalid DateTime
                ValidationError += ResHelper.GetString("BasicForm.ErrorInvalidDateTime") + " " + showDateTime + ".";
            }
            else
            {
                // Error invalid date
                ValidationError += ResHelper.GetString("BasicForm.ErrorInvalidDate") + " " + showDate.ToString("d") + ".";
            }

            return false;
        }

        if (!timePicker.IsValidRange())
        {
            ValidationError += GetString("general.errorinvaliddatetimerange");
            return false;
        }

        return true;
    }

    #endregion
}