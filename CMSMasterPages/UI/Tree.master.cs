using System;
using System.Web.UI.WebControls;

using CMS.UIControls;

public partial class CMSMasterPages_UI_Tree : CMSMasterPage
{
    #region "Properties"

    /// <summary>
    /// Resizer control.
    /// </summary>
    public override CMSUserControl FrameResizer
    {
        get
        {
            return borderElem.FrameResizer;
        }
    }


    /// <summary>
    /// Gets placeholder located after form element.
    /// </summary>
    public override PlaceHolder AfterFormPlaceholder
    {
        get
        {
            return plcAfterForm;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        PageStatusContainer = plcStatus;
    }

    #endregion
}