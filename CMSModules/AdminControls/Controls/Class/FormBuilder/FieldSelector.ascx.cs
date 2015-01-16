using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.FormEngine;
using CMS.SettingsProvider;

using AjaxControlToolkit;

public partial class CMSModules_AdminControls_Controls_Class_FormBuilder_FieldSelector : CMSUserControl
{
    #region "Variables"

    private readonly List<string> mFormClassNames = new List<string>(); 

    #endregion


    #region "Properties"

    /// <summary>
    /// Class names of forms
    /// </summary>
    public List<string> FormClassNames
    {
        get
        {
            return mFormClassNames;
        }
    } 

    #endregion 
	

    #region "Methods"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        var dci = DataClassInfoProvider.GetDataClass(FormClassNames.FirstOrDefault());
        var FormDefinition = dci.ClassFormDefinition;
        var fi = new FormInfo(FormDefinition);

        lstFields.DataSource = fi.ItemsList.Select(x => new ListItem { Text = x.ToString(), Value = x.ToString() }).ToList();
        lstFields.DataBind();
    }   

    #endregion


}