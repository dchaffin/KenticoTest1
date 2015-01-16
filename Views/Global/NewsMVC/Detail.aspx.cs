using System;
using System.Data;
using System.Web.Mvc;

using CMS.DocumentEngine;

/// <summary>
/// Sample view for the news.
/// </summary>
public partial class Views_Global_NewsMVC_Detail : ViewPage
{
    /// <summary>
    /// Returns the displayed document
    /// </summary>
    public TreeNode Document
    {
        get
        {
            return (TreeNode)ViewData["Document"];
        }
    }
}