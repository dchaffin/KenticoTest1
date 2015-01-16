using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.PortalControls;
using CMS.UIControls;
using CMS.CMSHelper;
using CMS.GlobalHelper;

public partial class CMSModules_AdminControls_Controls_Class_FieldEditor_CategoryEdit : CMSUserControl
{
    #region "Properties"

    /// <summary>
    /// Value contained in category textbox.
    /// </summary>
    public string Value
    {
        get
        {
            return txtCategoryName.Text.Trim();
        }
        set
        {
            txtCategoryName.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets checkbox indicating if collapse is enabled for the category.
    /// </summary>
    public bool Collapsible
    {
        get
        {
            return chkCollapsible.Checked;
        }
        set
        {
            chkCollapsible.Checked = value;
        }
    }


    /// <summary>
    /// Gets or sets checkbox indicating if category is collapsed by default.
    /// </summary>
    public bool CollapsedByDefault
    {
        get
        {
            return chkCollapsedByDefault.Checked;
        }
        set
        {
            chkCollapsedByDefault.Checked = value;
        }
    }


    /// <summary>
    /// Gets or sets visibility of collapsion setup.
    /// </summary>
    public bool ShowCollapsionDetails
    {
        get
        {
            return pnlCollapsible.Visible;
        }
        set
        {
            pnlCollapsible.Visible = value;
        }
    }


    /// <summary>
    /// Gets or sets visibility macro.
    /// </summary>
    public string VisibleMacro
    {
        get
        {
            return visibleMacro.Text;
        }
        set
        {
            visibleMacro.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets visibility of category and all its fields.
    /// </summary>
    public bool CategoryVisible
    {
        get
        {
            return chkVisibility.Checked;
        }
        set
        {
            chkVisibility.Checked = value;
        }
    }


    /// <summary>
    /// Gets or sets macroresolver.
    /// </summary>
    public string ResolverName
    {
        get
        {
            return visibleMacro.ResolverName;
        }
        set
        {
            visibleMacro.ResolverName = value;
        }
    }


    /// <summary>
    /// Gets or sets value indicating if control is placed in wizard.
    /// </summary>
    public bool IsWizard
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        visibleMacro.TopOffset = IsWizard ? 205 : 40;
        pnlCategory.GroupingText = GetString("templatedesigner.section.category");
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        SetCollapsedByDefaultVisibility();
    }


    protected void chkCollapsible_Changed(object sender, EventArgs e)
    {
        SetCollapsedByDefaultVisibility();
    }


    /// <summary>
    /// Sets visibility and checked setting for CollapsedByDefault checkbox control 
    /// based on value of Collapsible checkbox control.
    /// </summary>
    private void SetCollapsedByDefaultVisibility()
    {
        pnlCollapsibleByDefault.Visible = Collapsible;
        CollapsedByDefault &= Collapsible;
    }

    #endregion
}