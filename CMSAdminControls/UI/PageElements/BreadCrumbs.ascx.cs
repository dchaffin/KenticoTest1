using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.UIControls;

public partial class CMSAdminControls_UI_PageElements_BreadCrumbs : CMSUserControl
{
    #region "Variables"

    private string[,] mBreadcrumbs = null;
    private string mSeparatorText = string.Empty;

    #endregion


    #region "Properties"

    /// <summary>
    /// 3-dimensional array of breadcrumbs. (0, 0) contains the caption, (0, 1) contains URL, (0, 2) contains target.
    /// </summary>
    public string[,] Breadcrumbs
    {
        get
        {
            return mBreadcrumbs;
        }
        set
        {
            mBreadcrumbs = value;
        }
    }


    /// <summary>
    /// Text of the BreadCrumbs separator.
    /// </summary>
    public string SeparatorText
    {
        get
        {
            return mSeparatorText;
        }
        set
        {
            mSeparatorText = value;
        }
    }

    #endregion


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Create the breadcrumbs
        if (Breadcrumbs != null)
        {
            // Generate the breadcrumbs controls
            for (int i = 0; i <= Breadcrumbs.GetUpperBound(0); i++)
            {
                // Add separator
                if (i > 0)
                {
                    Label sepLabel = new Label();
                    sepLabel.Text = !string.IsNullOrEmpty(SeparatorText) ? SeparatorText : "&nbsp;";
                    sepLabel.CssClass = "TitleBreadCrumbSeparator";
                    plcBreadcrumbs.Controls.Add(sepLabel);
                }
                // Make link if URL specified
                if ((Breadcrumbs[i, 1] != null) && (Breadcrumbs[i, 1] != string.Empty) && (i != Breadcrumbs.GetUpperBound(0)))
                {
                    HyperLink newLink = new HyperLink();
                    newLink.Text = Breadcrumbs[i, 0];
                    newLink.NavigateUrl = Breadcrumbs[i, 1];
                    newLink.Target = Breadcrumbs[i, 2];
                    if (i != Breadcrumbs.GetUpperBound(0))
                    {
                        newLink.CssClass = "TitleBreadCrumb";
                    }
                    else
                    {
                        newLink.CssClass = "TitleBreadCrumbLast";
                    }
                    plcBreadcrumbs.Controls.Add(newLink);
                }
                else // Make label if last item or URL not specified
                {
                    Label newLabel = new Label();
                    newLabel.Text = Breadcrumbs[i, 0];
                    newLabel.CssClass = (i != Breadcrumbs.GetUpperBound(0)) ? "TitleBreadCrumb" : "TitleBreadCrumbLast";
                    plcBreadcrumbs.Controls.Add(newLabel);
                }
            }
        }
    }


    /// <summary>
    /// Creates the breadcrumbs array from the given parameters.
    /// </summary>
    public void CreateStaticBreadCrumbs(string namepath)
    {
        if ((namepath == null) || (namepath.Trim().Trim('/') == string.Empty))
        {
            Breadcrumbs = null;
            return;
        }
        string[] names = namepath.Trim('/').Split('/');
        string[,] bc = new string[names.Length, 3];
        //
        int index = 0;
        foreach (string name in names)
        {
            bc[index, 0] = name;
            bc[index, 1] = string.Empty;
            bc[index, 2] = string.Empty;
            index++;
        }
        Breadcrumbs = bc;
    }
}