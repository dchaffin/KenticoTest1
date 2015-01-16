using System;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.DataCom;
using CMS.EventLog;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

/// <summary>
/// A control that displays Data.com company mapping details.
/// </summary>
public partial class CMSModules_ContactManagement_Controls_UI_DataCom_CompanyMapping : CMSAdminControl
{
    #region "Variables"

    /// <summary>
    /// The Data.com company mapping to display.
    /// </summary>
    private EntityMapping mEntityMapping;


    /// <summary>
    /// Indicates whether the control is enabled.
    /// </summary>
    private bool mEnabled = true;
    
    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the Data.com company mapping to display.
    /// </summary>
    public EntityMapping Mapping
    {
        get
        {
            return mEntityMapping;
        }
        set
        {
            mEntityMapping = value;
        }
    }


    /// <summary>
    /// Gets or sets the value indicating whether the control is enabled.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            mEnabled = value;
        }
    }

    #endregion


    #region "Life cycle methods"

    protected override void OnPreRender(EventArgs e)
    {
        if (Mapping != null)
        {
            try
            {
                IDataComConfiguration configuration = DataComHelper.GetConfiguration(CMSContext.CurrentSiteID);
                FormInfo formInfo = DataComHelper.GetAccountFormInfo();
                EntityInfo entityInfo = DataComHelper.GetCompanyEntityInfo(configuration);
                HtmlTable table = CreateTable(formInfo, entityInfo, Mapping);
                Controls.Add(table);
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }
        base.OnPreRender(e);
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Creates and initializes a new instance of the HtmlTable class with the specified Data.com company mapping, and returns it.
    /// </summary>
    /// <param name="formInfo">The CMS account form info.</param>
    /// <param name="entityInfo">The Data.com company entity info.</param>
    /// <param name="mapping">The Data.com company mapping.</param>
    /// <returns>A new instance of the HtmlTable class initialized with the specified Data.com company mapping.</returns>
    private HtmlTable CreateTable(FormInfo formInfo, EntityInfo entityInfo, EntityMapping mapping)
    {
        HtmlTable table = new HtmlTable();
        table.Rows.Add(CreateHeaderRow());
        foreach (IFormItem formItem in formInfo.ItemsList)
        {
            FormFieldInfo formField = formItem as FormFieldInfo;
            if (formField != null)
            {
                EntityMappingItem mappingItem = mapping.GetItem(formField.Name);
                if (mappingItem != null)
                {
                    EntityAttributeInfo entityAttribute = entityInfo.GetAttributeInfo(mappingItem.EntityAttributeName);
                    if (entityAttribute != null)
                    {
                        HtmlTableRow row = new HtmlTableRow();
                        table.Rows.Add(row);
                        HtmlTableCell formFieldCell = new HtmlTableCell();
                        row.Cells.Add(formFieldCell);
                        formFieldCell.InnerText = ResHelper.LocalizeString(formField.Caption);
                        HtmlTableCell entityAttributeCell = new HtmlTableCell();
                        row.Cells.Add(entityAttributeCell);
                        entityAttributeCell.InnerText = ResHelper.LocalizeString(entityAttribute.DisplayName);

                        if (!Enabled)
                        {
                            formFieldCell.Style.Add("color", "#888888");
                            entityAttributeCell.Style.Add("color", "#888888");
                        }
                    }
                }
            }
        }

        return table;
    }


    /// <summary>
    /// Creates a header for the table with Data.com company mapping details, and returns it.
    /// </summary>
    /// <returns>A header for the table with Data.com company mapping details.</returns>
    private HtmlTableRow CreateHeaderRow()
    {
        HtmlTableRow row = new HtmlTableRow();
        HtmlTableCell sourceCell = new HtmlTableCell()
        {
            InnerText = GetString("datacom.kenticocms")
        };
        sourceCell.Style.Add("font-weight", "bold");
        row.Cells.Add(sourceCell);
        HtmlTableCell destinationCell = new HtmlTableCell()
        {
            InnerText = GetString("datacom.datacom")
        };
        destinationCell.Style.Add("font-weight", "bold");
        row.Cells.Add(destinationCell);

        if (!Enabled)
        {
            sourceCell.Style.Add("color", "#888888");
            destinationCell.Style.Add("color", "#888888");
        }

        return row;
    }


    /// <summary>
    /// Displays an error message and logs the specified exception to the event log.
    /// </summary>
    /// <param name="exception">The exception to handle.</param>
    private void HandleException(Exception exception)
    {
        ErrorSummary.Report(exception);
        EventLogProvider.LogException("Data.com Connector", "CompanyMappingControl", exception);
    }

    #endregion
}