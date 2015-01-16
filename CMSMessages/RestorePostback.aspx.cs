using System;
using System.Collections.Specialized;
using System.Data;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.IO;
using CMS.OutputFilter;
using CMS.UIControls;
using CMS.URLRewritingEngine;
using CMS.SettingsProvider;

public partial class CMSMessages_RestorePostback : MessagePage
{
    #region "Variables"

    protected string okText = null;
    protected string cancelText = null;
    protected string mBodyParams = null;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        titleElem.TitleText = GetString("RestorePostback.Header");
        titleElem.TitleImage = GetImageUrl("Others/Messages/info.png");

        lblInfo.Text = GetString("RestorePostback.Info");

        okText = GetString("General.OK");
        cancelText = GetString("General.Cancel");

        // Add the state fields
        SavedFormState state = FormStateHelper.GetSavedState();
        if (state != null)
        {
            // Render the hidden fields for the form items
            NameValueCollection form = state.Form;
            foreach (string name in form.Keys)
            {
                ltlValues.Text += "<input type=\"hidden\" name=\"" + name + "\" value=\"" + HttpUtility.HtmlEncode(form[name]) + "\" />\n";
            }

            state.Delete();
        }
        else
        {
            lblInfo.Text = GetString("RestorePostback.InfoNotSaved");

            if (Request.RawUrl.IndexOfCSafe("/RestorePostback.aspx", true) < 0)
            {
                mBodyParams = "onload=\"document.location.replace(document.location.href); return false;\"";
            }

            plcOK.Visible = false;
            cancelText = GetString("General.OK");
        }
    }


    /// <summary>
    /// Overridden render which removes everything unnecessary.
    /// </summary>
    protected override void Render(HtmlTextWriter writer)
    {
        StringBuilder stringBuilder = new StringBuilder();
        StringWriter stringWriter = new StringWriter(stringBuilder);
        HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);

        base.Render(htmlWriter);

        string html = stringBuilder.ToString();

        // Remove the action attribute
        int start = html.IndexOfCSafe("action=\"");
        int end = html.IndexOfCSafe("\"", start + 8) + 1;

        html = html.Remove(start, end - start + 1);

        // Remove anything generated at the end of the form
        start = html.IndexOfCSafe("##formend##");
        end = html.IndexOfCSafe("</form>");

        html = html.Remove(start, end - start);

        // Remove anything generated at the beginning of the form
        start = html.IndexOfCSafe("<form");
        start = html.IndexOf(">", start + 1) + 1;
        end = html.IndexOfCSafe("##formstart##") + 13;

        html = html.Remove(start, end - start);

        // Write the response
        writer.Write(html);
    }


    /// <summary>
    /// Error handling.
    /// </summary>
    protected override void OnError(EventArgs e)
    {
        base.OnError(e);

        // Delete the saved state
        SavedFormState state = FormStateHelper.GetSavedState();
        if (state != null)
        {
            state.Delete();
        }

        // Redirect to the same page
        Response.Redirect(URLRewriter.RawUrl);
    }


    /// <summary>
    /// Disable handler base tag.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        UseBaseTagForHandlerPage = false;
        base.OnInit(e);
    }
}