using System;

using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_DeviceProfile_Controls_LayoutBinding : CMSUserControl
{

    #region "Public properties"

    /// <summary>
    /// Gets or sets the source layout.
    /// </summary>
    public LayoutInfo SourceLayout { get; set; }


    /// <summary>
    /// Gets or sets the device profile.
    /// </summary>
    public DeviceProfileInfo DeviceProfile { get; set; }

    
    /// <summary>
    /// Gets the target layout for the specified device profile and source layout.
    /// </summary>
    public LayoutInfo TargetLayout { get; private set; }

    #endregion


    #region "Life-cycle methods"

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        EnsureChildControls();
        Initialize();
    }

    #endregion


    #region "Private methods"

    private void Initialize()
    {
        // Initialize source layout controls
        SourceLayoutThumbnail.AlternateText = HTMLHelper.HTMLEncode(GetString("device_profile.layoutmapping.sourcelayoutthumbalt"));
        SourceLayoutThumbnail.ImageUrl = GetLayoutThumbnailUrl(SourceLayout);
        SourceLayoutDisplayNameLabel.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(SourceLayout.LayoutDisplayName));

        // Initialize target layout controls
        TargetLayoutThumbnail.AlternateText = HTMLHelper.HTMLEncode(GetString("device_profile.layoutmapping.targetlayoutthumbalt"));
        TargetLayout = DeviceProfileLayoutInfoProvider.GetTargetLayoutInfo(DeviceProfile, SourceLayout);
        if (TargetLayout != null)
        {
            TargetLayoutThumbnail.ImageUrl = GetLayoutThumbnailUrl(TargetLayout);
            TargetLayoutDisplayNameLabel.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(TargetLayout.LayoutDisplayName));
        }
        else
        {
            TargetLayoutThumbnail.ImageUrl = GetImageUrl("CMSModules/CMS_DeviceProfile/no_target_layout.png");
            TargetLayoutDisplayNameLabel.Text = HTMLHelper.HTMLEncode(GetString("device_profile.layoutmapping.sethint"));
        }

        // Initialize script to open target layout selection dialog
        string baseUrl = URLHelper.ResolveUrl("~/CMSModules/DeviceProfile/Pages/SelectLayout.aspx");
        string url = String.Format("{0}?deviceProfileId={1:D}&sourceLayoutId={2:D}&targetLayoutId={3:D}", baseUrl, DeviceProfile.ProfileID, SourceLayout.LayoutId, TargetLayout != null ? TargetLayout.LayoutId : 0);
        string script = String.Format("modalDialog('{0}', 'SelectLayout', '1000', '785', null)", URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(url)));
        TargetLayoutItemControl.Attributes.Add("onclick", script);
    }


    private string GetLayoutThumbnailUrl(LayoutInfo layout)
    {
        string relativeUrl = null;
        if (layout.LayoutThumbnailGUID != Guid.Empty)
        {
            relativeUrl =  layout.Generalized.GetThumbnailUrl(0, 0, 64);
        }
        else
        {
            relativeUrl = GetImageUrl("Objects/CMS_Layout/notavailable.png");
        }

        return URLHelper.ResolveUrl(relativeUrl);
    }

    #endregion

}