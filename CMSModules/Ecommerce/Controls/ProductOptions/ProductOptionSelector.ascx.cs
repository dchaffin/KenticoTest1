using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.FormControls;

public partial class CMSModules_Ecommerce_Controls_ProductOptions_ProductOptionSelector : ProductOptionSelector
{
    protected void Page_Load(object sender, EventArgs e)
    {
        LoadSelector();
    }


    /// <summary>
    /// Loads selector's data.
    /// </summary>
    private void LoadSelector()
    {
        if (SelectionControl != null)
        {
            // Add selection control to the collection 
            pnlSelector.Controls.Add(SelectionControl);

            if (SelectionControl is FormEngineUserControl)
            {
                FormEngineUserControl fc = (FormEngineUserControl)SelectionControl;
                fc.SetValue("ProductOptionCategoryID", OptionCategoryId);
                fc.IsLiveSite = IsLiveSite;
            }

            if (IsSelectionControlEmpty())
            {
                // Load selection control data according to the optiong category data
                ReloadData();
            }

            // There is no choice -> hide control
            if (!HasSelectableOptions())
            {
                pnlContainer.Visible = false;
            }
            // Option category is not empty -> display option category details
            else if (OptionCategory != null)
            {
                // Show / hide option category name
                if (ShowOptionCategoryName)
                {
                    lblCategName.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(OptionCategory.CategoryDisplayName));
                }
                else
                {
                    lblCategName.Visible = false;
                }

                // Show / hide option category description
                if (ShowOptionCategoryDescription)
                {
                    lblCategDescription.Text = ResHelper.LocalizeString(OptionCategory.CategoryDescription);
                }
                else
                {
                    lblCategDescription.Visible = false;
                }
            }

            // WAI validation
            if (OptionCategory.CategorySelectionType == OptionCategorySelectionTypeEnum.Dropdownlist)
            {
                lblCategName.AssociatedControlClientID = SelectionControl.ClientID;
            }
        }
    }


    // Reloads selector's data
    public void ReloadSelector()
    {
        pnlSelector.Controls.Clear();

        LoadCategorySelectionControl();

        LoadSelector();
    }


    /// <summary>
    /// Validates selected/entered product option. If it is valid, returns true, otherwise returns false.
    /// </summary> 
    public override bool IsValid()
    {
        // Check length of the text product option
        if ((OptionCategory.CategorySelectionType == OptionCategorySelectionTypeEnum.TextBox ||
             OptionCategory.CategorySelectionType == OptionCategorySelectionTypeEnum.TextArea) &&
            OptionCategory.CategoryTextMaxLength > 0)
        {
            // Get text length
            int textLength = GetSelectedSKUOptions().Length;

            // Validate text length
            if (textLength > OptionCategory.CategoryTextMaxLength)
            {
                lblError.Text = string.Format(GetString("com.optioncategory.maxtextlengthexceeded"), OptionCategory.CategoryTextMaxLength);
                plnError.Visible = true;
                return false;
            }
        }

        return true;
    }
}