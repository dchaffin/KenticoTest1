using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Controls;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.SettingsProvider;

using DataPager = CMS.Controls.DataPager;

public partial class CMSWebParts_Ecommerce_Products_ProductDataList : CMSAbstractWebPart
{
    #region "Pager properties"

    /// <summary>
    /// Pager control.
    /// </summary>
    public DataPager PagerControl
    {
        get
        {
            return productDataList.PagerControl;
        }
    }


    /// <summary>
    /// Enable paging.
    /// </summary>
    public bool EnablePaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnablePaging"), productDataList.EnablePaging);
        }
        set
        {
            SetValue("EnablePaging", value);
            productDataList.EnablePaging = value;
        }
    }


    /// <summary>
    /// Pager position.
    /// </summary>
    public PagingPlaceTypeEnum PagerPosition
    {
        get
        {
            return PagerControl.GetPagerPosition(DataHelper.GetNotEmpty(GetValue("PagerPosition"), PagerControl.PagerPosition.ToString()));
        }
        set
        {
            SetValue("PagerPosition", value.ToString());
            PagerControl.PagerPosition = value;
        }
    }


    /// <summary>
    /// Gets or sets the page size.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), PagerControl.PageSize);
        }
        set
        {
            SetValue("PageSize", value);
            PagerControl.PageSize = value;
        }
    }


    /// <summary>
    /// Pager query string key.
    /// </summary>
    public string QueryStringKey
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("QueryStringKey"), PagerControl.QueryStringKey), PagerControl.QueryStringKey);
        }
        set
        {
            SetValue("QueryStringKey", value);
            PagerControl.QueryStringKey = value;
        }
    }


    /// <summary>
    /// Show first / last links
    /// </summary>
    public bool ShowFirstLast
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowFirstLast"), PagerControl.ShowFirstLast);
        }
        set
        {
            SetValue("ShowFirstLast", value);
            PagerControl.ShowFirstLast = value;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Hide control for zero rows.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), productDataList.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            productDataList.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Zero rows text.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("ZeroRowsText"), productDataList.ZeroRowsText), productDataList.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
            productDataList.ZeroRowsText = value;
        }
    }


    /// <summary>
    /// Repeat columns.
    /// </summary>
    public int RepeatColumns
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("RepeatColumns"), productDataList.RepeatColumns);
        }
        set
        {
            SetValue("RepeatColumns", value);
            productDataList.RepeatColumns = value;
        }
    }


    /// <summary>
    /// Repeat layout.
    /// </summary>
    public RepeatLayout RepeatLayout
    {
        get
        {
            return CMSDataList.GetRepeatLayout(DataHelper.GetNotEmpty(GetValue("RepeatLayout"), productDataList.RepeatLayout.ToString()));
        }
        set
        {
            SetValue("RepeatLayout", value.ToString());
            productDataList.RepeatLayout = value;
        }
    }


    /// <summary>
    /// Repeat Direction.
    /// </summary>
    public RepeatDirection RepeatDirection
    {
        get
        {
            return CMSDataList.GetRepeatDirection(DataHelper.GetNotEmpty(GetValue("RepeatDirection"), productDataList.RepeatDirection.ToString()));
        }
        set
        {
            SetValue("RepeatDirection", value.ToString());
            productDataList.RepeatDirection = value;
        }
    }


    /// <summary>
    /// Select TOP N records.
    /// </summary>
    public int SelectTopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectTopN"), productDataList.SelectTopN);
        }
        set
        {
            SetValue("SelectTopN", value);
            productDataList.SelectTopN = value;
        }
    }


    /// <summary>
    /// Gets or sets the source filter name.
    /// </summary>
    public string FilterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterName"), productDataList.FilterName);
        }
        set
        {
            SetValue("FilterName", value);
            productDataList.FilterName = value;
        }
    }

    #endregion


    #region "Transformation properties"

    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the results.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("TransformationName"), productDataList.TransformationName), productDataList.TransformationName);
        }
        set
        {
            SetValue("TransformationName", value);
            productDataList.TransformationName = value;
        }
    }


    /// <summary>
    /// Alternating transformation name.
    /// </summary>
    public string AlternatingTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("AlternatingTransformationName"), productDataList.AlternatingTransformationName), productDataList.AlternatingTransformationName);
        }
        set
        {
            SetValue("AlternatingTransformationName", value);
            productDataList.AlternatingTransformationName = value;
        }
    }


    /// <summary>
    /// Selected item transformation name.
    /// </summary>
    public string SelectedItemTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("SelectedItemTransformationName"), productDataList.SelectedItemTransformationName), productDataList.SelectedItemTransformationName);
        }
        set
        {
            SetValue("SelectedItemTransformationName", value);
            productDataList.AlternatingTransformationName = value;
        }
    }

    #endregion


    #region "Content"

    /// <summary>
    /// Where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WhereCondition"), productDataList.WhereCondition);
        }
        set
        {
            SetValue("WhereCondition", value);
            productDataList.WhereCondition = value;
        }
    }


    /// <summary>
    /// Order by.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OrderBy"), productDataList.OrderBy);
        }
        set
        {
            SetValue("OrderBy", value);
            productDataList.OrderBy = value;
        }
    }

    #endregion


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        // In design mode is pocessing of control stoped
        if (StopProcessing)
        {
            productDataList.StopProcessing = true;
        }
        else
        {
            int querySkuId = QueryHelper.GetInteger("SKUid", 0);

            // Pager
            PagerControl.PagerPosition = PagerPosition;
            productDataList.EnablePaging = EnablePaging;
            PagerControl.PageSize = PageSize;
            PagerControl.QueryStringKey = QueryStringKey;
            PagerControl.ShowFirstLast = ShowFirstLast;

            // Public
            productDataList.RepeatColumns = RepeatColumns;
            productDataList.RepeatLayout = RepeatLayout;
            productDataList.RepeatDirection = RepeatDirection;
            productDataList.HideControlForZeroRows = HideControlForZeroRows;
            productDataList.ZeroRowsText = ZeroRowsText;
            productDataList.FilterName = FilterName;

            // Transformations
            productDataList.AlternatingTransformationName = AlternatingTransformationName;
            productDataList.TransformationName = TransformationName;
            productDataList.SelectedItemTransformationName = SelectedItemTransformationName;

            // Select one SKU product by query string SKU id
            if (querySkuId > 0)
            {
                WhereCondition = SqlHelperClass.AddWhereCondition(WhereCondition, "(SKUId = " + querySkuId + ")");
                productDataList.TransformationName = SelectedItemTransformationName;
            }

            // Data settings
            productDataList.DataSource = SKUInfoProvider.GetSKUs(WhereCondition, OrderBy);
            productDataList.SelectTopN = SelectTopN;
            productDataList.WhereCondition = WhereCondition;
            productDataList.OrderBy = OrderBy;

            if ((this.productDataList.SelectTopN > 0) && (!DataHelper.DataSourceIsEmpty(this.productDataList.DataSource)))
            {
                for (int i = 0; i < ((DataView)this.productDataList.DataSource).Table.Rows.Count; i++)
                {
                    if (i >= this.productDataList.SelectTopN)
                    {
                        ((DataView)this.productDataList.DataSource).Delete(this.productDataList.SelectTopN);
                    }
                }
                ((DataView)this.productDataList.DataSource).Table.AcceptChanges();
            }

            productDataList.ReloadData(false);
            productDataList.EnablePaging = true;
        }
    }


    /// <summary>
    /// OnPrerender override (Set visibility).
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = productDataList.Visible;

        if (DataHelper.DataSourceIsEmpty(productDataList.DataSource) && (HideControlForZeroRows))
        {
            Visible = false;
        }
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
        productDataList.DataBind();
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        productDataList.ClearCache();
    }
}