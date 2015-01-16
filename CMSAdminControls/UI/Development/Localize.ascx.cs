using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.ExtendedControls;
using CMS.GlobalHelper;
using System.Text;
using System.Collections;
using CMS.SettingsProvider;

public partial class CMSAdminControls_UI_Development_Localize : CMSContextMenuControl
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Register the dialog script
        ScriptHelper.RegisterDialogScript(this.Page);

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "Localize", ScriptHelper.GetScript(
@"
function EditString(key) {
    modalDialog('" + ResolveUrl("~/CMSFormControls/Selectors/LocalizableTextBox/LocalizeString.aspx") + @"?stringKey=' + escape(key), 'localizableString', 600, 635, null, null, true);
}
"
        ));
    }


    /// <summary>
    /// Compares two resource string items to each other
    /// </summary>
    /// <param name="first">First item</param>
    /// <param name="second">Second item</param>
    private static int CompareStrings(DictionaryEntry first, DictionaryEntry second)
    {
        bool firstUnknown = ((string)first.Key).EqualsCSafe((string)first.Value, true);
        bool secondUnknown = ((string)second.Key).EqualsCSafe((string)second.Value, true);

        if (firstUnknown == secondUnknown)
        {
            // Order by value
            string firstValue = ValidationHelper.GetString(first.Value, "");
            string secondValue = ValidationHelper.GetString(second.Value, "");

            return firstValue.CompareTo(secondValue);
        }
        else
        {
            // Order by unknown status
            return secondUnknown.CompareTo(firstUnknown);
        }
    }

    
    protected override void Render(HtmlTextWriter writer)
    {
        if (SettingsKeyProvider.DevelopmentMode)
        {
            // Render the resources
            var resources = ResHelper.CurrentResources;
            if (resources != null)
            {
                StringBuilder sb = new StringBuilder();

                // Sort the items by 
                List<DictionaryEntry> list = new List<DictionaryEntry>(resources.Count);
                foreach (DictionaryEntry item in resources)
                {
                    list.Add(item);
                }
                list.Sort(CompareStrings);

                bool lastUnknown = false;

                sb.Append("<div class=\"MenuHeader\">", ResHelper.GetString("localizable.localize"), "</div>");

                foreach (DictionaryEntry item in list)
                {
                    string key = (string)item.Key;
                    string value = ValidationHelper.GetString(item.Value, "");

                    // Handle the unknown status
                    bool unknown = key.EqualsCSafe(value, false);
                    if (!unknown && lastUnknown)
                    {
                        sb.Append("<div class=\"Separator\">&nbsp;</div>");
                    }
                    lastUnknown = unknown;

                    value = HTMLHelper.HTMLEncode(TextHelper.LimitLength(HTMLHelper.StripTags(value), 70));

                    sb.Append("<div class=\"Item\" onclick=\"EditString('", HTMLHelper.HTMLEncode(ScriptHelper.GetString(key, false)) + "'); return false;\"><div class=\"ItemPadding\">", (unknown ? "<img class=\"Icon\" src=\"" + GetImageUrl("Design/Controls/UniGrid/Actions/Warning.png") + "\" /> " : ""), "<span class=\"Name\"><strong>", value, "</strong> (", TextHelper.LimitLength(HTMLHelper.HTMLEncode(key), 50), ")</span></div></div>");
                }

                ltlStrings.Text = sb.ToString();
            }
        }
        
        base.Render(writer);
    }
}
