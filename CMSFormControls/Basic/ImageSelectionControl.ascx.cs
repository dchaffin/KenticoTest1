using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.GlobalHelper;

public partial class CMSFormControls_Basic_ImageSelectionControl : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return imageSelector.Enabled;
        }
        set
        {
            imageSelector.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            return imageSelector.Value;
        }
        set
        {
            imageSelector.Value = ValidationHelper.GetString(value, null);
        }
    }


    /// <summary>
    /// Indicates if control is placed on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return imageSelector.IsLiveSite;
        }
        set
        {
            imageSelector.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets the value indicating if form control is in mode FileSelectionControl. If FALSE then form control is in mode ImageSelectionControl.
    /// </summary>
    public bool IsFileSelection
    {
        get
        {
            return FormHelper.IsFieldOfType(FieldInfo, FormFieldControlTypeEnum.FileSelectionControl);
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize control
        imageSelector.ImagePathTextBox.CssClass = "EditingFormImagePathTextBox";
        imageSelector.ImagePreviewControl.CssClass = "EditingFormImagePathPreview";
        imageSelector.SelectImageButton.Attributes.Add("class", "EditingFormImagePathButton");
        imageSelector.ClearPathButton.Attributes.Add("class", "EditingFormImagePathClearButton");
        imageSelector.ShowClearButton = true;
        imageSelector.IsLiveSite = IsLiveSite;

        // Setup control
        if (GetValue("width") != null)
        {
            imageSelector.ImageWidth = ValidationHelper.GetInteger(GetValue("width"), 0);
        }
        if (GetValue("height") != null)
        {
            imageSelector.ImageHeight = ValidationHelper.GetInteger(GetValue("height"), 0);
        }
        if (GetValue("maxsidesize") != null)
        {
            imageSelector.ImageMaxSideSize = ValidationHelper.GetInteger(GetValue("maxsidesize"), 0);
        }

        // Apply CSS styles
        if (!String.IsNullOrEmpty(ControlStyle))
        {
            imageSelector.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }
        if (!String.IsNullOrEmpty(CssClass))
        {
            imageSelector.CssClass = CssClass;
            CssClass = null;
        }

        // Set image selector dialog
        DialogConfiguration config = null;
        if (FieldInfo != null)
        {
            config = GetDialogConfiguration(FieldInfo);
        }
        else
        {
            config = new DialogConfiguration();
            config.OutputFormat = OutputFormatEnum.URL;
        }

        config.SelectableContent = SelectableContentEnum.OnlyImages;
        imageSelector.DialogConfig = config;
        imageSelector.ShowImagePreview = true;

        // Set properties specific to File selection control
        if (IsFileSelection)
        {
            imageSelector.DialogConfig.SelectableContent = SelectableContentEnum.AllFiles;
            imageSelector.ShowImagePreview = false;
        }

        CheckFieldEmptiness = true;
    }

    #endregion
}