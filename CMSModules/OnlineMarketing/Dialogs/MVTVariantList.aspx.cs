using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.UIControls;

using Action = CMS.UIControls.UniGridConfig.Action;

public partial class CMSModules_OnlineMarketing_Dialogs_MVTVariantList : CMSVariantDialogPage
{
    #region "Variables"

    /// <summary>
    /// Indicates whether editing a web part or a zone variant.
    /// </summary>
    private VariantTypeEnum variantType = VariantTypeEnum.Zone;

    // Setup the list control properties
    private int pageTemplateId = 0;
    private string zoneId = string.Empty;
    private Guid instanceGuid = Guid.Empty;
    private string aliasPath = string.Empty;
    private string webPartId = string.Empty;

    #endregion


    #region "Page methods"

    /// <summary>
    /// Raises the <see cref="E:Init"/> event.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("cms.mvtest", "Read"))
        {
            RedirectToAccessDenied(String.Format(GetString("general.permissionresource"), "Read", "MVT testing"));
        }

        variantType = listElem.VariantType = VariantTypeFunctions.GetVariantTypeEnum(QueryHelper.GetString("varianttype", string.Empty));

        // Check permissions and redirect
        OnlineMarketingContext.CheckPermissions(variantType);

        // Get the alias path of the current node
        if (Node == null)
        {
            listElem.StopProcessing = true;
        }

        // Set NodeID in order to check the access to the document
        listElem.NodeID = NodeID;

        // Setup the list control properties
        pageTemplateId = listElem.PageTemplateID = QueryHelper.GetInteger("templateid", 0);
        zoneId = listElem.ZoneID = QueryHelper.GetText("zoneid", string.Empty);
        instanceGuid = listElem.InstanceGUID = QueryHelper.GetGuid("instanceguid", Guid.Empty);
        aliasPath = QueryHelper.GetString("aliaspath", string.Empty);
        webPartId = QueryHelper.GetString("webpartid", string.Empty);
        btnClose.Text = GetString("general.close");
    }


    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup the modal dialog
        SetCulture();
        RegisterEscScript();

        ScriptHelper.RegisterDialogScript(this);
        ScriptHelper.RegisterWOpenerScript(this);

        // Setup the title, image, help
        PageTitle title = CurrentMaster.Title;
        title.HelpName = "helpTopic";

        Action editAction = null;

        title.TitleText = GetString("mvtvariant.list");
        title.TitleImage = GetImageUrl("Objects/OM_MVTVariant/object.png");
        // Must be set be to help icon created
        title.HelpTopicName = "mvtvariant_list";

        // Set the dark header (+ dark help icon)
        CurrentMaster.PanelBody.CssClass += " DialogsPageHeader";
        title.HelpIconUrl = GetImageUrl("General/HelpLargeDark.png");

        // Get Edit action button
        editAction = listElem.Grid.GridActions.Actions[0] as Action;
        editAction.OnClick = "OpenVariantProperties({0}); return false;";

        listElem.OnDelete += new EventHandler(listElem_OnDelete);
    }


    /// <summary>
    /// Raises the <see cref="E:PreRender"/> event.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Setup the modal dialog
        RegisterModalPageScripts();

        // Choose the correct javascript method for opening web part/zone properties
        string functionName = string.Empty;
        // Set a script to open the web part properties modal dialog
        string parameters = string.Empty;
        // Set the script to show the corresponding variant
        string setVariant = string.Empty;

        // Choose the correct javascript method for opening web part/zone properties
        switch (variantType)
        {
            case VariantTypeEnum.WebPart:
                functionName = "ConfigureWebPart";
                parameters = "'" + zoneId + "','" + webPartId + "','" + aliasPath + "','" + instanceGuid + "'," + pageTemplateId;
                setVariant = "SetVariant('Variant_WP_" + instanceGuid.ToString("N") + "', variantId);";
                break;

            case VariantTypeEnum.Widget:
                functionName = "ConfigureWidget";
                parameters = "'" + zoneId + "','" + webPartId + "','" + aliasPath + "','" + instanceGuid + "'," + pageTemplateId;
                setVariant = "SetVariant('Variant_WP_" + instanceGuid.ToString("N") + "', variantId);";
                break;

            case VariantTypeEnum.Zone:
                functionName = "ConfigureWebPartZone";
                parameters = "'" + zoneId + "','" + aliasPath + "'," + pageTemplateId + "," + QueryHelper.GetText("islayoutzone", "false");
                setVariant = "SetVariant('Variant_Zone_" + zoneId + "', variantId);";
                break;
        }

        setVariant += " wopener.UpdateCombinationPanel();";

        string script = @"
            function OpenVariantProperties(variantId)
            {
                CloseDialog();

                if ((wopener." + functionName + @") && (wopener.SetVariant))
                {
                    wopener." + setVariant + @"
                    wopener." + functionName + "(" + parameters + @");
                }
            }";

        // If any variant has been deleted => refresh the design page
        if (ValidationHelper.GetBoolean(hdnRefreshSlider.Value, false))
        {
            script += @"
            function RefreshPage() {
                wopener = parent.wopener;
                if (wopener.RefreshPage) {
                    wopener.RefreshPage();
                }
            }

            window.onunload = RefreshPage;";
        }

        ltrScript.Text = ScriptHelper.GetScript(script);
    }


    /// <summary>
    /// Handles the OnDelete event of the listElem control.
    /// </summary>
    protected void listElem_OnDelete(object sender, EventArgs e)
    {
        // Set the flag that a variant has been deleted
        hdnRefreshSlider.Value = "true";
    }

    #endregion
}