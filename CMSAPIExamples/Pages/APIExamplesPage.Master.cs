using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.SettingsProvider;

public partial class CMSAPIExamples_Pages_APIExamplesPage : CMSMasterPage
{
    #region "Properties"

    public override PageTitle Title
    {
        get
        {
            return titleElem;
        }
    }

    #endregion


    #region "Page methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        Title.HelpTopicName = "CMS_API_examples_overview";

        lblCreate.Text = "Creating and managing";
        lblClean.Text = "Cleanup";
        lblCreateInfo.Text = "This section provides API examples for creating, getting and updating objects. It is recommended to follow the API examples order.";
        lblCleanInfo.Text = "This section provides API examples for deleting objects and their dependencies. The order of the cleanup examples is usually reversed.";
        btnRunAll.ImageUrl = UIHelper.GetImageUrl(Page, "/Others/APIExamples/run.png");
        btnCleanAll.ImageUrl = UIHelper.GetImageUrl(Page, "/Others/APIExamples/clean.png");

        if (!RequestHelper.IsPostBack())
        {
            if (AzureHelper.IsRunningOnAzure)
            {
                txtCode.Text = @"
//
// Viewing of the source code is disabled on Windows Azure.
//";
            }
            else
            {
                txtCode.Text = @"
//
// Source code of the example will be displayed after clicking the 'View code' button.
//";
            }
        }
    }

    #endregion


    #region Events

    /// <summary>
    /// Runs all create and update examples on the page.
    /// </summary>
    protected void btnRunAll_Click(object sender, EventArgs e)
    {
        CMSAPIExamplePage examplePage = Page as CMSAPIExamplePage;
        if (examplePage != null)
        {
            examplePage.RunAll();
        }
    }


    /// <summary>
    /// Runs all cleanup examples on the page.
    /// </summary>
    protected void btnCleanAll_Click(object sender, EventArgs e)
    {
        CMSAPIExamplePage examplePage = Page as CMSAPIExamplePage;
        if (examplePage != null)
        {
            examplePage.CleanUpAll();
        }
    }

    #endregion
}