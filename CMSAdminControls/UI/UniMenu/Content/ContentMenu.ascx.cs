using System;
using System.Web.UI.WebControls;

using CMS.UIControls;

using MenuItem = CMS.UIControls.UniMenuConfig.Item;

public partial class CMSAdminControls_UI_UniMenu_Content_ContentMenu : CMSUserControl
{
    #region "Variables"

    private string titlePrefix = "general.";
    private string tooltipPrefix = "documents.";

    private string mNewImageUrl = null;
    private string mDeleteImageUrl = null;

    private int mBigButtonMinimalWidth = 40;
    private int mSmallButtonMinimalWidth = 66;
    private int mShrinkedButtonMinimalWidth = 70;

    #endregion


    #region "Properties"

    /// <summary>
    /// Width of the big menu button.
    /// </summary>
    public int BigButtonMinimalWidth
    {
        get
        {
            return mBigButtonMinimalWidth;
        }
        set
        {
            mBigButtonMinimalWidth = value;
        }
    }


    /// <summary>
    /// Width of the small menu button.
    /// </summary>
    public int SmallButtonMinimalWidth
    {
        get
        {
            return mSmallButtonMinimalWidth;
        }
        set
        {
            mSmallButtonMinimalWidth = value;
        }
    }


    /// <summary>
    /// Width of 'New' and 'Delete' buttons when AllButtonsAreSmall is set to true.
    /// </summary>
    public int ShrinkedButtonMinimalWidth
    {
        get
        {
            return mShrinkedButtonMinimalWidth;
        }
        set
        {
            mShrinkedButtonMinimalWidth = value;
        }
    }


    /// <summary>
    /// When set to true, all buttons are rendered as small.
    /// </summary>
    public bool AllButtonsAreSmall
    {
        get;
        set;
    }


    /// <summary>
    /// Resource string key prefix used for obtaining titles for buttons. 'general.' by defualt.
    /// </summary>
    public string TitleStringPrefix
    {
        get
        {
            return titlePrefix;
        }
        set
        {
            titlePrefix = value;
        }
    }


    /// <summary>
    /// Resource string key prefix used for obtaining tooltips for buttons. 'documents.' by defualt.
    /// </summary>
    public string TooltipStringPrefix
    {
        get
        {
            return tooltipPrefix;
        }
        set
        {
            tooltipPrefix = value;
        }
    }


    /// <summary>
    /// The 'New document' action icon path.
    /// </summary>
    public string NewImageUrl
    {
        get
        {
            return mNewImageUrl ?? GetImageUrl("CMSModules/CMS_Content/Menu/New.png");
        }
        set
        {
            mNewImageUrl = value;
        }
    }


    /// <summary>
    /// The 'Delete document' action icon path.
    /// </summary>
    public string DeleteImageUrl
    {
        get
        {
            return mDeleteImageUrl ?? GetImageUrl("CMSModules/CMS_Content/Menu/Delete.png");
        }
        set
        {
            mDeleteImageUrl = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // New button
        MenuItem newItem = new MenuItem();
        newItem.Text = GetTitleString("new");
        newItem.Tooltip = GetTooltipString("newtooltip");
        newItem.OnClientClick = "if (!NewItem()) return;";
        newItem.ImagePath = NewImageUrl;
        newItem.ImageAltText = GetTooltipString("newtooltip");
        if (AllButtonsAreSmall)
        {
            AddSmallButton(newItem, ShrinkedButtonMinimalWidth);
        }
        else
        {
            AddBigButton(newItem);
        }

        // Delete button
        MenuItem deleteItem = new MenuItem();
        deleteItem.Text = GetTitleString("delete");
        deleteItem.Tooltip = GetTooltipString("deletetooltip");
        deleteItem.OnClientClick = "if(!DeleteItem()) return;";
        deleteItem.ImagePath = DeleteImageUrl;
        deleteItem.ImageAltText = GetTooltipString("deletetooltip");
        if (AllButtonsAreSmall)
        {
            AddSmallButton(deleteItem, ShrinkedButtonMinimalWidth);
        }
        else
        {
            AddBigButton(deleteItem);
        }

        // Copy button
        MenuItem copyItem = new MenuItem();
        copyItem.Text = GetTitleString("copy");
        copyItem.Tooltip = GetTooltipString("copytooltip");
        copyItem.OnClientClick = "if(CheckChanges()) { CopyRef(GetSelectedNodeId()); };";
        copyItem.ImagePath = GetImageUrl("CMSModules/CMS_Content/Menu/Copy.png");
        copyItem.ImageAltText = GetTooltipString("copytooltip");
        AddSmallButton(copyItem);

        // Move button
        MenuItem moveItem = new MenuItem();
        moveItem.Text = GetTitleString("move");
        moveItem.Tooltip = GetTooltipString("movetooltip");
        moveItem.OnClientClick = "if(CheckChanges()) { MoveRef(GetSelectedNodeId()); };";
        moveItem.ImagePath = GetImageUrl("CMSModules/CMS_Content/Menu/Move.png");
        moveItem.ImageAltText = GetTooltipString("movetooltip");
        AddSmallButton(moveItem);

        // Up button
        MenuItem upItem = new MenuItem();
        upItem.Text = GetTitleString("up");
        upItem.Tooltip = GetTooltipString("uptooltip");
        upItem.OnClientClick = "if(!MoveUp()) return;";
        upItem.ImagePath = GetImageUrl("CMSModules/CMS_Content/Menu/Up.png");
        upItem.ImageAltText = GetTooltipString("uptooltip");
        AddSmallButton(upItem);

        // Down button
        MenuItem downItem = new MenuItem();
        downItem.Text = GetTitleString("down");
        downItem.Tooltip = GetTooltipString("downtooltip");
        downItem.OnClientClick = "if(!MoveDown()) return;";
        downItem.ImagePath = GetImageUrl("CMSModules/CMS_Content/Menu/Down.png");
        downItem.ImageAltText = GetTooltipString("downtooltip");
        AddSmallButton(downItem);
    }


    /// <summary>
    /// Adds given menu item as small button of specified width.
    /// </summary>
    /// <param name="item">Menu item to be added.</param>
    /// <param name="width">Width of button.</param>
    private void AddSmallButton(MenuItem item, int width)
    {
        item.CssClass = "SmallButton";
        item.ImageAlign = ImageAlign.AbsMiddle;
        item.MinimalWidth = width;

        buttonsSmall.Buttons.Add(item);
    }


    /// <summary>
    /// Adds given menu item as small button.
    /// </summary>
    /// <param name="item">Menu item to be added.</param>
    private void AddSmallButton(MenuItem item)
    {
        AddSmallButton(item, SmallButtonMinimalWidth);
    }


    /// <summary>
    /// Adds given menu item as big button.
    /// </summary>
    /// <param name="item">Menu item to be added.</param>
    private void AddBigButton(MenuItem item)
    {
        item.CssClass = "BigButton";
        item.ImageAlign = ImageAlign.Top;
        item.MinimalWidth = BigButtonMinimalWidth;

        buttonsBig.Buttons.Add(item);
    }


    private string GetTitleString(string key)
    {
        return GetString(TitleStringPrefix + key);
    }


    private string GetTooltipString(string key)
    {
        return GetString(TooltipStringPrefix + key);
    }

    #endregion
}