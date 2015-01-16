using System;
using System.Text;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.SettingsProvider;

using MenuItem = CMS.UIControls.UniMenuConfig.Item;

public partial class CMSAdminControls_UI_UniMenu_Content_ModeMenu : CMSUserControl
{
    #region "Enums"

    public enum ContentViewMode
    {
        Edit,
        Preview,
        LiveSite,
        Listing
    }

    #endregion

    
    #region "Properties"

    /// <summary>
    /// Mode to be considered as selected.
    /// </summary>
    public string SelectedMode
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectedMode"), "edit");
        }
        set
        {
            SetValue("SelectedMode", value);
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set mode script
        StringBuilder setMode = new StringBuilder();
        setMode.AppendLine("function SetModeIcon(mode) {");
        setMode.AppendLine("    var index = 0;");
        setMode.AppendLine("    switch(mode)");
        setMode.AppendLine("    {");
        const int modeButtonMinimalWidth = 52;

        int i = 0;
        foreach (string name in Enum.GetNames(typeof(ContentViewMode)))
        {
            string lowerName = name.ToLowerCSafe();
            string toolTip = GetString("mode." + lowerName + "tooltip");
            MenuItem modeItem = new MenuItem();
            modeItem.Text = GetString("general." + lowerName);
            modeItem.Tooltip = toolTip;
            modeItem.CssClass = "BigButton";
            modeItem.OnClientClick = "if(!SetMode('" + lowerName + "')) return;";
            modeItem.ImagePath = GetImageUrl("CMSModules/CMS_Content/Menu/" + name + ".png");
            modeItem.ImageAltText = toolTip;
            modeItem.ImageAlign = ImageAlign.Top;
            modeItem.MinimalWidth = modeButtonMinimalWidth;
            buttonsBig.Buttons.Add(modeItem);

            if (SelectedMode.ToLowerCSafe() == name.ToLowerCSafe())
            {
                buttonsBig.SelectedIndex = i;
            }

            setMode.AppendLine("    case '" + lowerName + "':");
            setMode.AppendLine("        index = " + i + ";");
            setMode.AppendLine("    break;");

            i++;
        }

        setMode.AppendLine("    }");
        setMode.AppendLine("    SelectButtonIndex_" + buttonsBig.ClientID + "(index);");
        setMode.AppendLine("    return true;");
        setMode.AppendLine("}");
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "setModeScript", ScriptHelper.GetScript(setMode.ToString()));
    }

    #endregion
}