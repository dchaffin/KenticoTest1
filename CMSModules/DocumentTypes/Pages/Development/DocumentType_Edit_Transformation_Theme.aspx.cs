using System;

using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.UIControls;

public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_Transformation_Theme : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup the file system browser
        int transformationId = QueryHelper.GetInteger("transformationid", 0);
        if (transformationId > 0)
        {
            TransformationInfo ti = TransformationInfoProvider.GetTransformation(transformationId);
            EditedObject = ti;

            if (ti != null)
            {
                // Ensure the theme folder
                themeElem.Path = ti.GetThemePath();
            }
        }
        else
        {
            EditedObject = null;
        }
    }
}