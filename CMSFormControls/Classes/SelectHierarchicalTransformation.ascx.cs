using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.FormControls;

public partial class CMSFormControls_Classes_SelectHierarchicalTransformation : FormEngineUserControl
{
    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return SelectTransformation.Value;
        }
        set
        {
            SelectTransformation.Value = value;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        SelectTransformation.IsLiveSite = IsLiveSite;
        SelectTransformation.NewDialogPath = "~/CMSModules/DocumentTypes/Pages/Development/HierarchicalTransformations_New.aspx";
    }
}