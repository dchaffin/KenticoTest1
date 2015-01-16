using System;

using CMS.PortalEngine;
using CMS.UIControls;
using CMS.GlobalHelper;

public partial class CMSModules_AdminControls_Controls_Preview_PreviewFooter : CMSUserControl
{
    /// <summary>
    /// Transformation info object
    /// </summary>
    public TransformationInfo TransInfo
    {
        get
        {
            return GetValue("TransInfo") as TransformationInfo;
        }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the preview control should display the "Save" and "Save and close" buttons.
    /// </summary>
    public bool ShowSaveButtons
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowSaveButtons"), true);
        }
        set
        {
            SetValue("ShowSaveButtons", value);
        }
    }


    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        btnCancel.Text = GetString("general.close");
        btnSaveAndClose.Text = GetString("general.saveandclose");
        btnSave.Text = GetString("general.save");

        if (TransInfo != null)
        {
            btnSave.Visible = false;         
        }
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        btnSave.Visible = ShowSaveButtons;
        btnSaveAndClose.Visible = ShowSaveButtons;

        base.OnPreRender(e);
    }
}
