using System;
using System.Data;
using System.Web.Mvc;

using CMS.DocumentEngine;

/// <summary>
/// Sample view for the news.
/// </summary>
public partial class Views_Global_NewsMVC_List : ViewPage
{
    /// <summary>
    /// Returns the News displayed on current page
    /// </summary>
    public TreeNodeDataSet NewsList
    {
        get
        {
            return (TreeNodeDataSet)ViewData["NewsList"];
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
    }
}