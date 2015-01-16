using System;
using System.Web.UI.WebControls;

using CMS.Controls;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.UIControls;

public partial class CMSModules_Objects_Controls_Locking_ObjectEditPanel : CMSUserControl, IObjectEditPanel
{
    #region "Properties"

    /// <summary>
    /// Messages placeholder.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return pnlMessagePlaceholder;
        }
    }


    /// <summary>
    /// Header actions of the object edit menu.
    /// </summary>
    public override HeaderActions HeaderActions
    {
        get
        {
            return editMenuElem.HeaderActions;
        }
    }


    /// <summary>
    /// Gets the object edit menu control.
    /// </summary>
    public ObjectEditMenu ObjectEditMenu
    {
        get
        {
            return editMenuElem;
        }
    }


    /// <summary>
    /// Gets the object edit menu control.
    /// </summary>
    public IObjectEditMenu AbstractObjectEditMenu
    {
        get
        {
            return editMenuElem;
        }
    }


    /// <summary>
    /// Gets the object manager control used in the object edit menu.
    /// </summary>
    public CMSObjectManager ObjectManager
    {
        get
        {
            return editMenuElem.ObjectManager;
        }
    }


    /// <summary>
    /// Gets the object manager control used in the object edit menu.
    /// </summary>
    public ICMSObjectManager AbstractObjectManager
    {
        get
        {
            return ObjectManager;
        }
    }


    /// <summary>
    /// Returns panel in which the ObjectEditMenu is wrapped.
    /// </summary>
    public Panel MenuPanel
    {
        get
        {
            return pnlMenu;
        }
    }


    /// <summary>
    /// If set, than the breadcrumbs included in this control will be displayed.
    /// This is used in preview mode for new item creation where the page title has to be part of the inner preview control and cannot use master page's title.
    /// </summary>
    public string[,] PageTitleBreadcrumbs
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the value that indicates if the control is used in the preview mode.
    /// </summary>
    public bool PreviewMode
    {
        get
        {
            return editMenuElem.PreviewMode;
        }
        set
        {
            editMenuElem.PreviewMode = value;
        }
    }

    #endregion


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        bool displayBreadCrumbs = (PageTitleBreadcrumbs != null);

        titleElem.StopProcessing = !displayBreadCrumbs;
        titleElem.Visible = displayBreadCrumbs;

        if (displayBreadCrumbs)
        {
            titleElem.Breadcrumbs = PageTitleBreadcrumbs;
        }
    }
}
