using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SiteProvider;

public partial class CMSModules_BannerManagement_FormControls_BannerTypeSelector : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Value. (int)BannerTypeEnum
    /// </summary>
    public override object Value
    {
        get
        {
            return drpBannerType.Value;
        }
        set
        {
            drpBannerType.Value = value;
        }
    }


    /// <summary>
    /// Underlying drop down list.
    /// </summary>
    public DropDownList DropDownList
    {
        get
        {
            return drpBannerType.DropDownList;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Sets dropdown items. Separate control has to be used, because Value is set eariler than parent control's Init event.
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="e">param</param>
    protected void Page_Init(object sender, EventArgs e)
    {
        if (!URLHelper.IsPostback())
        {
            drpBannerType.DropDownList.Items.Clear();

            IEnumerable<BannerTypeEnum> orderedBannerTypes = Enum.GetValues(typeof(BannerTypeEnum)).Cast<BannerTypeEnum>();

            // Image should be first/default (the rest does not matter).
            orderedBannerTypes = orderedBannerTypes.OrderBy(bt => bt != BannerTypeEnum.Image);

            foreach (BannerTypeEnum bannerType in orderedBannerTypes)
            {
                drpBannerType.DropDownList.Items.Add(new ListItem(ResHelper.GetString("banner.bannertype." + bannerType.ToString()), ((int)bannerType).ToString()));
            }
        }
    }

    #endregion
}
