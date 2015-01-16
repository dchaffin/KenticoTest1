using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using AjaxControlToolkit;
using CMS.UIControls;

public partial class CMSModules_AdminControls_Controls_Class_FormBuilder_FormBuilder : CMSUserControl
{
    public string FormClassName
    {
        get;
        set;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        fieldSelector.FormClassNames.Add(FormClassName);
    }
}