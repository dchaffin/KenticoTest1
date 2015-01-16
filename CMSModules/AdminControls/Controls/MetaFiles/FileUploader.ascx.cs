using System;
using System.Collections.Generic;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_AdminControls_Controls_MetaFiles_FileUploader : CMSUserControl
{
    #region "Variables"

    /// <summary>
    /// Resource string for "Add another file" link.
    /// </summary>
    private string strRemove = null;

    /// <summary>
    /// List containing all uploader controls.
    /// </summary>
    private List<CMSFileUpload> uploadersList = null;


    private int mMaxUploadFields = 5;
    private int mDefaultUploadFields = 0;
    private string mAddButtonImagePath = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the path to the image displayed next to add attachment link.
    /// </summary>
    public string AddButtonImagePath
    {
        get
        {
            return mAddButtonImagePath;
        }
        set
        {
            mAddButtonImagePath = value;
        }
    }


    /// <summary>
    /// Gets or sets the maximal allowed number of uploaders.
    /// </summary>
    public int MaxUploadFields
    {
        get
        {
            return mMaxUploadFields;
        }
        set
        {
            mMaxUploadFields = value;
        }
    }


    /// <summary>
    /// Gets or sets the number of displayed files after the control is initialized.
    /// </summary>
    public int DefaultUploadFields
    {
        get
        {
            return mDefaultUploadFields;
        }
        set
        {
            mDefaultUploadFields = value;
        }
    }


    /// <summary>
    /// Gets the array of posted files from uploaders.
    /// </summary>
    public HttpPostedFile[] PostedFiles
    {
        get
        {
            List<HttpPostedFile> retval = new List<HttpPostedFile>();
            foreach (CMSFileUpload uploader in uploadersList)
            {
                if (uploader.HasFile)
                {
                    retval.Add(uploader.PostedFile);
                }
            }
            return retval.ToArray();
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Hide everything and do nothing when no uploaders should be displayed
        if (mMaxUploadFields < 1)
        {
            pnlAdd.Visible = false;
            pnlUploaders.Visible = false;
            return;
        }

        strRemove = GetString("general.remove");

        // Initialize the list
        uploadersList = new List<CMSFileUpload>(mMaxUploadFields);

        // Generate the controls dynamically and initialize JS variables
        GenerateUploaders();

        if (String.IsNullOrEmpty(mAddButtonImagePath))
        {
            imgAdd.Visible = false;
        }
        else
        {
            imgAdd.ImageUrl = mAddButtonImagePath;
        }
        lnkAdd.NavigateUrl = "#";
        lnkAdd.Attributes.Add("onclick", "addFile(); return false;");
    }


    /// <summary>
    /// Generates the uploaders, saves them to the hastable and returns ClientID of the first uploader
    /// which should be visible when "Add file" link is clicked.
    /// </summary>
    private void GenerateUploaders()
    {
        string availableUploaders = "";
        string visibleUploaders = "";

        // Maximum visible field at the begenning is the smaller number of those two
        int stillVisible = Math.Min(mMaxUploadFields, mDefaultUploadFields);

        for (int i = 0; i < mMaxUploadFields; i++)
        {
            // Create new panel and put it to page
            Panel pnlUploader = new Panel();
            pnlUploaders.Controls.Add(pnlUploader);

            CMSFileUpload uploader = new CMSFileUpload();
            pnlUploader.Controls.Add(uploader);
            uploadersList.Add(uploader);

            pnlUploader.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));

            HyperLink lnkRemove = new HyperLink();
            lnkRemove.Text = strRemove;
            lnkRemove.CssClass = "NewItemLink";
            lnkRemove.NavigateUrl = "#";
            lnkRemove.Attributes.Add("onclick", "removeFile('" + pnlUploader.ClientID + "', '" + uploader.ClientID + "'); return false;");
            pnlUploader.Controls.Add(lnkRemove);

            // Set correct visibility
            if (stillVisible > 0)
            {
                pnlUploader.Attributes.Add("style", "display: block;");
                visibleUploaders += ", '" + pnlUploader.ClientID + "'";
                stillVisible--;
            }
            else
            {
                availableUploaders += ", '" + pnlUploader.ClientID + "'";
                pnlUploader.Attributes.Add("style", "display: none;");
            }
        }

        visibleUploaders = visibleUploaders.Trim(',');
        availableUploaders = availableUploaders.Trim(',');

        // Initialize JS variables
        ltlScript.Text += ScriptHelper.GetScript(
            "var addLinkElemPanel = document.getElementById('" + pnlAdd.ClientID + "');\n" +
            "var addLinkElem = document.getElementById('" + lnkAdd.ClientID + "'); \n\n" +
            "var strAddAnotherFile = '" + GetString("fileuploader.addanotherfile") + "'; \n" +
            "var strAttachFile = '" + GetString("fileuploader.attachfile") + "'; \n\n" +
            "var uploadersVisible = new Array(" + visibleUploaders + "); \n" +
            "var uploadersAvailable = new Array(" + availableUploaders + "); \n\n" +
            "reorderUploaders(); \n\n");
    }
}