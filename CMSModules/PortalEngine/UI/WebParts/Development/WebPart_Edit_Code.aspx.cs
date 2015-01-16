using System;
using System.Data;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_UI_WebParts_Development_WebPart_Edit_Code : SiteManagerPage
{
    private int webpartId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        webpartId = ValidationHelper.GetInteger(Request.QueryString["webpartid"], 0);
        EditedObject = WebPartInfoProvider.GetWebPartInfo(webpartId);
        GenerateCode();
    }


    /// <summary>
    /// Generates the web part code.
    /// </summary>
    protected void GenerateCode()
    {
        string ascx = null;
        string code = null;

        // Generate the code
        CodeTemplateGenerator.GenerateWebPartCode(webpartId, txtBaseControl.Text.Trim(), out ascx, out code);

        txtASCX.Text = ascx;
        txtCS.Text = code;
    }


    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        GenerateCode();
    }
}