using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSMasterPages_LiveSite_Dialogs_TabsHeader : CMSLiveMasterPage
{
    /// <summary>
    /// Tabs control.
    /// </summary>
    public override UITabs Tabs
    {
        get
        {
            return tabControlElem;
        }
    }


    /// <summary>
    /// PageTitle control.
    /// </summary>
    public override PageTitle Title
    {
        get
        {
            return titleElem;
        }
    }


    /// <summary>
    /// HeaderActions control.
    /// </summary>
    public override HeaderActions HeaderActions
    {
        get
        {
            if (base.HeaderActions != null)
            {
                return base.HeaderActions;
            }
            return actionsElem;
        }
    }


    /// <summary>
    /// Prepared for specifying the additional HEAD elements.
    /// </summary>
    public override Literal HeadElements
    {
        get
        {
            return ltlHeadElements;
        }
        set
        {
            ltlHeadElements = value;
        }
    }


    /// <summary>
    /// Left tabs panel.
    /// </summary>
    public override Panel PanelLeft
    {
        get
        {
            return pnlLeft;
        }
    }


    /// <summary>
    /// Right tabs panel.
    /// </summary>
    public override Panel PanelRight
    {
        get
        {
            return pnlRight;
        }
    }


    /// <summary>
    /// Separator panel.
    /// </summary>
    public override Panel PanelSeparator
    {
        get
        {
            return pnlSeparator;
        }
    }


    /// <summary>
    /// Panel containig title.
    /// </summary>
    public override Panel PanelTitle
    {
        get
        {
            return pnlTitle;
        }
    }


    /// <summary>
    /// Panel containing tab menu control.
    /// </summary>
    public override Panel PanelTabs
    {
        get
        {
            return pnlWhite;
        }
    }


    /// <summary>
    /// Frame resizer.
    /// </summary>
    public override CMSUserControl FrameResizer
    {
        get
        {
            return frmResizer;
        }
    }


    /// <summary>
    /// Tab master page doesn't hide page title.
    /// </summary>
    public override bool TabMode
    {
        get
        {
            return false;
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (Page is CMSPage)
        {
        }

        // Set dialog CSS class
        SetDialogClass();

        this.PageStatusContainer = plcStatus;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Hide actions panel if no actions are present and DisplayActionsPanel is false
        if (!DisplayActionsPanel)
        {
            if ((actionsElem.Actions == null) || (actionsElem.Actions.Length == 0))
            {
                pnlActions.Visible = false;
            }
        }
    }
}