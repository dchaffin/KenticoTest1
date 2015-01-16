using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.Ecommerce;

/// <summary>
/// Ecommerce methods - wrapping methods for macro resolver.
/// </summary>
public static class EcommerceMethods
{
    #region "Methods - Registrations"
    
    /// <summary>
    /// Registers all ecommerce methods to macro resolver.
    /// </summary>
    public static void RegisterMethods()
    {
        // Get add to shopping cart link
        MacroMethod getAddToShoppingCartLink = new MacroMethod("GetAddToShoppingCartLink", GetAddToShoppingCartLink)
        {
            Comment = "Returns link to \"add to shoppingcart\".",
            Type = typeof(string),
            MinimumParameters = 1,
            AllowedTypes = new List<Type>() { typeof(TransformationNamespace) }
        };
        getAddToShoppingCartLink.AddParameter("productId", typeof(object), "Product ID.");
        getAddToShoppingCartLink.AddParameter("enabled", typeof(object), "Indicates whether product is enabled or not.");
        MacroMethods.RegisterMethod(getAddToShoppingCartLink);

        // Get add to wishlist link
        MacroMethod getAddToWishListLink = new MacroMethod("GetAddToWishListLink", GetAddToWishListLink)
        {
            Comment = "Returns link to add specified product to the user's wishlist.",
            Type = typeof(string),
            MinimumParameters = 1,
            AllowedTypes = new List<Type>() { typeof(TransformationNamespace) }
        };
        getAddToWishListLink.AddParameter("productId", typeof(object), "Product ID.");
        MacroMethods.RegisterMethod(getAddToWishListLink);

        // Get remove from wishlist link
        MacroMethod getRemoveFromWishListLink = new MacroMethod("GetRemoveFromWishListLink", GetRemoveFromWishListLink)
        {
            Comment = "Returns link to remove specified product from the user's wishlist.",
            Type = typeof(string),
            MinimumParameters = 1,
            AllowedTypes = new List<Type>() { typeof(TransformationNamespace) }
        };
        getRemoveFromWishListLink.AddParameter("productId", typeof(object), "Product ID.");
        MacroMethods.RegisterMethod(getRemoveFromWishListLink);

        // Get public status name
        MacroMethod gtPublicStatusName = new MacroMethod("GetPublicStatusName", GetPublicStatusName)
        {
            Comment = "Returns the public SKU status display name.",
            Type = typeof(string),
            MinimumParameters = 1,
            AllowedTypes = new List<Type>() { typeof(TransformationNamespace) }
        };
        gtPublicStatusName.AddParameter("statusId", typeof(object), "Status ID");
        MacroMethods.RegisterMethod(gtPublicStatusName);

        // Get shopping cart URL
        MacroMethod shoppingCartURL = new MacroMethod("ShoppingCartURL", ShoppingCartURL)
        {
            Comment = "Returns URL to the shopping cart.",
            Type = typeof(string),
            MinimumParameters = 1,
            AllowedTypes = new List<Type>() { typeof(TransformationNamespace) }
        };
        shoppingCartURL.AddParameter("siteName", typeof(string), "Site name.");
        MacroMethods.RegisterMethod(shoppingCartURL);

        // Get wishlist URL
        MacroMethod wishlistURL = new MacroMethod("WishlistURL", WishlistURL)
        {
            Comment = "Returns URL to the wish list.",
            Type = typeof(string),
            MinimumParameters = 1,
            AllowedTypes = new List<Type>() { typeof(TransformationNamespace) }
        };
        wishlistURL.AddParameter("siteName", typeof(string), "Site name");
        MacroMethods.RegisterMethod(wishlistURL);

        // Get product URL
        MacroMethod getProductUrl = new MacroMethod("GetProductUrl", GetProductUrl)
        {
            Comment = "Returns product URL.",
            Type = typeof(string),
            MinimumParameters = 1,
            AllowedTypes = new List<Type>() { typeof(TransformationNamespace) }
        };
        getProductUrl.AddParameter("skuId", typeof(object), "SKU ID.");
        MacroMethods.RegisterMethod(getProductUrl);

        // Get product URL by GUID
        MacroMethod getProductUrlByGUID = new MacroMethod("GetProductUrlByGUID", GetProductUrlByGUID)
        {
            Comment = "Returns user friendly URL of the specified SKU.",
            Type = typeof(string),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(TransformationNamespace) }
        };
        getProductUrlByGUID.AddParameter("skuGuid", typeof(object), "SKU GUID.");
        getProductUrlByGUID.AddParameter("skuName", typeof(object), "SKU name.");
        getProductUrlByGUID.AddParameter("siteName", typeof(object), "Site name.");
        MacroMethods.RegisterMethod(getProductUrlByGUID);

        // Get manufacturer
        MacroMethod getManufacturer = new MacroMethod("GetManufacturer", GetManufacturer)
        {
            Comment = "Gets object from the specified column of the manufacturer with specific ID.",
            Type = typeof(object),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(TransformationNamespace) }
        };
        getManufacturer.AddParameter("id", typeof(int), "Manufacturer ID.");
        getManufacturer.AddParameter("column", typeof(string), "Column name.");
        MacroMethods.RegisterMethod(getManufacturer);

        // Get department
        MacroMethod getDepartment = new MacroMethod("GetDepartment", GetDepartment)
        {
            Comment = "Gets object from the specified column of the department with specific ID.",
            Type = typeof(object),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(TransformationNamespace) }
        };
        getDepartment.AddParameter("id", typeof(int), "Department ID.");
        getDepartment.AddParameter("column", typeof(string), "Column name.");
        MacroMethods.RegisterMethod(getDepartment);

        // Get supplier
        MacroMethod getSupplier = new MacroMethod("GetSupplier", GetSupplier)
        {
            Comment = "Gets object from the specified column of the supplier with specific ID.",
            Type = typeof(object),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(TransformationNamespace) }
        };
        getSupplier.AddParameter("id", typeof(int), "Supplier ID.");
        getSupplier.AddParameter("column", typeof(string), "Column name.");
        MacroMethods.RegisterMethod(getSupplier);

        // Get internal status
        MacroMethod getInternalStatus = new MacroMethod("GetInternalStatus", GetInternalStatus)
        {
            Comment = "Gets object from the specified column of the internal status with specific ID.",
            Type = typeof(object),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(TransformationNamespace) }
        };
        getInternalStatus.AddParameter("id", typeof(int), "Internal status ID.");
        getInternalStatus.AddParameter("column", typeof(string), "Column name.");
        MacroMethods.RegisterMethod(getInternalStatus);

        // Get public status
        MacroMethod getPublicStatus = new MacroMethod("GetPublicStatus", GetPublicStatus)
        {
            Comment = "Gets object from the specified column of the public status with specific ID.",
            Type = typeof(object),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(TransformationNamespace) }
        };
        getPublicStatus.AddParameter("id", typeof(int), "Public status ID.");
        getPublicStatus.AddParameter("column", typeof(string), "Column name.");
        MacroMethods.RegisterMethod(getPublicStatus);

        // Get product image
        MacroMethod getProductImage = new MacroMethod("GetProductImage", GetProductImage)
        {
            Comment = "Returns complete HTML code of the specified resized product image, if not such image exists, default image is returned.",
            Type = typeof(string),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(TransformationNamespace) }
        };
        getProductImage.AddParameter("imageUrl", typeof(object), "Product image url.");
        getProductImage.AddParameter("alt", typeof(object), "Image alternate text.");
        getProductImage.AddParameter("maxSideSize", typeof(object), "Max side size of the image.");
        getProductImage.AddParameter("width", typeof(object), "Width of the image.");
        getProductImage.AddParameter("height", typeof(object), "Height of the image.");
        MacroMethods.RegisterMethod(getProductImage);

        // Format price
        MacroMethod formatPrice = new MacroMethod("FormatPrice", FormatPrice)
        {
            Comment = "Returns price rounded and formatted according to the current currency properties.",
            Type = typeof(string),
            MinimumParameters = 1,
            AllowedTypes = new List<Type>() { typeof(TransformationNamespace) }
        };
        formatPrice.AddParameter("price", typeof(double), "Price to be formatted");
        formatPrice.AddParameter("round", typeof(bool), "True - price is rounded according to the current currency settings before formatting");
        MacroMethods.RegisterMethod(formatPrice);
        
        // Cart items count
        MacroMethod getShoppingCartItemsCount = new MacroMethod("GetShoppingCartItemsCount", GetShoppingCartItemsCount)
        {
            Comment = "Returns number of products in current shopping cart.",
            Type = typeof(int),
            MinimumParameters = 0,
            AllowedTypes = new List<Type>() { typeof(TransformationNamespace) }
        };
        MacroMethods.RegisterMethod(getShoppingCartItemsCount);
    }

    #endregion


    #region "Methods - Definitions"

    /// <summary>
    /// Returns link to "add to shopping cart".
    /// </summary>
    /// <param name="parameters">Product ID; Indicates whether product is enabled or not</param>
    public static object GetAddToShoppingCartLink(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 1:
                return EcommerceFunctions.GetAddToShoppingCartLink(parameters[0]);

            case 2:
                return EcommerceFunctions.GetAddToShoppingCartLink(parameters[0], parameters[1]);

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns link to add specified product to the user's wishlist.
    /// </summary>
    /// <param name="parameters">Product ID</param>
    public static object GetAddToWishListLink(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 1:
                return EcommerceFunctions.GetAddToWishListLink(parameters[0]);

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns link to remove specified product from the user's wishlist.
    /// </summary>
    /// <param name="parameters">Product ID</param>
    public static object GetRemoveFromWishListLink(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 1:
                return EcommerceFunctions.GetRemoveFromWishListLink(parameters[0]);

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns the public SKU status display name.
    /// </summary>
    /// <param name="parameters">Status ID</param>
    public static object GetPublicStatusName(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 1:
                return EcommerceFunctions.GetPublicStatus(parameters[0], "SKUPublicStatusID");

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns URL to the shopping cart.
    /// </summary>
    /// <param name="parameters">Site name</param>
    public static object ShoppingCartURL(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 1:
                return EcommerceFunctions.ShoppingCartURL(ValidationHelper.GetString(parameters[0], ""));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns URL to the wish list.
    /// </summary>
    /// <param name="parameters">Site name</param>
    public static object WishlistURL(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 1:
                return EcommerceFunctions.WishlistURL(ValidationHelper.GetString(parameters[0], ""));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns user friendly URL of the specified SKU and site name.
    /// </summary>
    /// <param name="parameters">SKU ID</param>
    public static object GetProductUrl(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 1:
                return EcommerceFunctions.GetProductUrl(parameters[0]);

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns user friendly URL of the specified SKU and site name.
    /// </summary>
    /// <param name="parameters">(SKU Guid; SKU Name) OR (SKU Guid; SKU Name; Site Name)</param>
    public static object GetProductUrlByGUID(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return EcommerceFunctions.GetProductUrl(parameters[0], parameters[1]);

            case 3:
                return EcommerceFunctions.GetProductUrl(parameters[0], parameters[1], parameters[2]);

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Gets object from the specified column of the manufacturer with specific ID.
    /// </summary>
    /// <param name="parameters">Manufacturer ID; Column name</param>
    public static object GetManufacturer(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return EcommerceFunctions.GetManufacturer(parameters[0], ValidationHelper.GetString(parameters[1], ""));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Gets object from the specified column of the department with specific ID.
    /// </summary>
    /// <param name="parameters">Department ID; Column name</param>
    public static object GetDepartment(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return EcommerceFunctions.GetDepartment(parameters[0], ValidationHelper.GetString(parameters[1], ""));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Gets object from the specified column of the supplier with specific ID.
    /// </summary>
    /// <param name="parameters">Supplier ID; Column name</param>
    public static object GetSupplier(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return EcommerceFunctions.GetSupplier(parameters[0], ValidationHelper.GetString(parameters[1], ""));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Gets object from the specified column of the internal status with specific ID.
    /// </summary>
    /// <param name="parameters">Internal status ID; Column name</param>
    public static object GetInternalStatus(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return EcommerceFunctions.GetInternalStatus(parameters[0], ValidationHelper.GetString(parameters[1], ""));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Gets object from the specified column of the public status with specific ID.
    /// </summary>
    /// <param name="parameters">Public status ID; Column name</param>
    public static object GetPublicStatus(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return EcommerceFunctions.GetPublicStatus(parameters[0], ValidationHelper.GetString(parameters[1], ""));

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Gets document name of specified nodeid.
    /// </summary>
    /// <param name="parameters">Document node ID</param>
    public static string GetDocumentName(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 1:
                return EcommerceFunctions.GetDocumentName(parameters[0]);

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns complete HTML code of the specified resized product image, if not such image exists, default image is returned.
    /// </summary>
    /// <param name="parameters">
    /// (Product image URL; Image alternate text) OR 
    /// (Product image URL; Image alternate text; Max side size) OR 
    /// (Product image URL; Image alternate text; Width of image; Height of image)</param>
    public static object GetProductImage(params object[] parameters)
    {
        string img = "<img src=\"{0}\" alt=\"{1}\" />";

        switch (parameters.Length)
        {
            case 2:
                return string.Format(img, EcommerceFunctions.GetSKUImageUrl(parameters[0], 0, 0, 0, 0), parameters[1]);

            case 3:
                return string.Format(img, EcommerceFunctions.GetSKUImageUrl(parameters[0], 0, 0, parameters[2], 0), parameters[1]);

            case 4:
                return string.Format(img, EcommerceFunctions.GetSKUImageUrl(parameters[0], parameters[2], parameters[3], 0, 0), parameters[1]);

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns price rounded and formatted according to the current currency properties.
    /// </summary>
    /// <param name="parameters">price; round</param>
    public static string FormatPrice(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 1:
                return GetFormattedPrice(parameters[0], true);

            case 2:
                return GetFormattedPrice(parameters[0], parameters[1]);

            default:
                throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Returns number of products in current shopping cart.
    /// </summary>
    /// <param name="parameters">no parameters</param>
    public static object GetShoppingCartItemsCount(params object[] parameters)
    {
        if ((ECommerceContext.CurrentShoppingCart != null) && (ECommerceContext.CurrentShoppingCart.CartItems != null))
        {
            return ECommerceContext.CurrentShoppingCart.CartItems.Count;
        }
        return 0;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Returns price rounded and formatted according to the current currency properties.
    /// </summary>
    /// <param name="priceObj">Price to be formatted</param>
    /// <param name="roundObj">True - price is rounded according to the current currency settings before formatting</param>
    private static string GetFormattedPrice(object priceObj, object roundObj)
    {
        double price = ValidationHelper.GetDouble(priceObj, 0);
        bool round = ValidationHelper.GetBoolean(roundObj, true);

        return ECommerceContext.CurrentShoppingCart.GetFormattedPrice(price, round);
    }

    #endregion
}