using System;

using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.UIControls;

public partial class CMSModules_DocumentTypes_Pages_Development_HierarchicalTransformations_General : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int transID = QueryHelper.GetInteger("transid", 0);
        if (transID != 0)
        {
            TransformationInfo ti = TransformationInfoProvider.GetTransformation(transID);
            ucTransf.TransInfo = ti;
        }
    }
}