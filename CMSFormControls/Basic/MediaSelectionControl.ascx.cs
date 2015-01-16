using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.ExtendedControls;

public partial class CMSFormControls_Basic_MediaSelectionControl : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return mediaSelector.Enabled;
        }
        set
        {
            mediaSelector.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            return mediaSelector.Value;
        }
        set
        {
            mediaSelector.Value = ValidationHelper.GetString(value, null);
        }
    }


    /// <summary>
    /// Indicates if control is placed on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return mediaSelector.IsLiveSite;
        }
        set
        {
            mediaSelector.IsLiveSite = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup control
        mediaSelector.ImagePathTextBox.CssClass = "EditingFormMediaPathTextBox";
        mediaSelector.ImagePreviewControl.CssClass = "EditingFormMediaPathPreview";
        mediaSelector.SelectImageButton.Attributes.Add("class", "EditingFormMediaPathButton");
        mediaSelector.ClearPathButton.Attributes.Add("class", "EditingFormMediaPathClearButton");
        mediaSelector.ShowClearButton = true;

        if (!String.IsNullOrEmpty(ControlStyle))
        {
            mediaSelector.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }
        if (!String.IsNullOrEmpty(CssClass))
        {
            mediaSelector.CssClass = CssClass;
            CssClass = null;
        }

        // Get dialog configuration
        DialogConfiguration mediaConfig = GetDialogConfiguration(FieldInfo);
        if (mediaConfig != null)
        {
            mediaConfig.SelectableContent = SelectableContentEnum.AllFiles;
            mediaConfig.OutputFormat = OutputFormatEnum.URL;
            mediaConfig.HideWeb = true;
            mediaSelector.DialogConfig = mediaConfig;
            mediaSelector.UseCustomDialogConfig = true;
        }

        CheckFieldEmptiness = true;
    }

    #endregion
}