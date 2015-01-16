using System;

using CMS.ExtendedControls;
using CMS.UIControls;

public partial class CMSModules_Settings_Controls_SettingsKeyControlSelector : CMSUserControl
{
    
    /// <summary>
    /// Gets or sets form control code name or user control file system path.
    /// </summary>
    public string ControlPath
    {
        get
        {
            if (rdbFormControl.Checked)
            {
                return (string)selectFormControl.Value;
            }
            else
            {
                return (string)selectFileName.Value;
            }
        }
        set
        {
            string controlPath = value;
            if (!String.IsNullOrEmpty(controlPath) && (controlPath.EndsWith(".ascx")))
            {
                pnlFormControl.Visible = false;
                pnlFileSystem.Visible = true;
                rdbFileSystem.Checked = true;
                selectFileName.Value = controlPath;
            }
            else
            {
                pnlFormControl.Visible = true;
                pnlFileSystem.Visible = false;
                rdbFormControl.Checked = true;
                selectFormControl.Value = controlPath;
            }
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            SetupControl();
        }

    }


    private void SetupControl()
    {
        // Setup file system selector for
        FileSystemDialogConfiguration config = new FileSystemDialogConfiguration();
        config.DefaultPath = "CMSFormControls";
        config.AllowedExtensions = "ascx";
        config.ShowFolders = false;
        selectFileName.DialogConfig = config;
    }


    /// <summary>
    /// Handles radio button event.
    /// </summary>
    protected void rdbFormControl_CheckedChanged(object sender, EventArgs e)
    {
        if (rdbFormControl.Checked)
        {
            pnlFormControl.Visible = true;
            pnlFileSystem.Visible = false;
        }
        else
        {
            pnlFormControl.Visible = false;
            pnlFileSystem.Visible = true;
        }
    }
}
