using System;
using System.Collections;
using System.Web;
using System.Web.UI;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.Controls;

public partial class CMSModules_Content_Controls_Dialogs_General_DialogPager : CMSUserControl
{
    #region "Properties"

    /// <summary>
    /// Page size items
    /// </summary>
    public string[] PageSizeItems 
    { 
        get
        {
            return pageSize.Items;
        }
        set
        {
            pageSize.Items = value;
        }
    }


    /// <summary>
    /// Pager control
    /// </summary>
    public UniPager Pager 
    {
        get
        {
            return pagerElem;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Gets the current page size
    /// </summary>
    public int GetPageSize(int defaultPageSize)
    { 
        return (pageSize.SelectedValue.ToUpperCSafe() == "-1") ? 0 : ValidationHelper.GetInteger(pageSize.SelectedValue, defaultPageSize);
    }

    #endregion
}