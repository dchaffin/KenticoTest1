using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Collections;

using CMS.EcommerceProvider;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.ExtendedControls;

/// <summary>
/// Custom payment gateway for USAePay integration
/// </summary>
public class USAePayGateway : CMSPaymentGatewayProvider
{
	
    /// <summary>
    /// Returns a payment gateway form with custom controls
    /// </summary>
    public override CMSPaymentGatewayForm GetPaymentDataForm()
    {
        try
        {
            return (CMSPaymentGatewayForm)this.ShoppingCartControl.LoadControl("~/Custom/USAePay/USAePayGatewayForm");
        }
        catch
        {
            return null;
        }
    }

    public override void ProcessPayment()
    {
        string url = this.GetPaymentGatewayUrl();

        if (url != "")
        {

        }
        else
        {

        }
    }

}