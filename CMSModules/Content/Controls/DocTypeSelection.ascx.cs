using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using CMS.CMSHelper;
using CMS.DocumentEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.LicenseProvider;

public partial class CMSModules_Content_Controls_DocTypeSelection : CMSAbstractNewDocumentControl
{
    #region "Variables"

    private DataSet dsClasses = null;
    private string lastPriorityClassName = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Unigrid object used for listing document types.
    /// </summary>
    public UniGrid Grid
    {
        get
        {
            return gridClasses;
        }
    }


    /// <summary>
    /// The count of document types found.
    /// </summary>
    public int ClassesCount
    {
        get;
        private set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentUserInfo currentUser = CMSContext.CurrentUser;

        // Setup unigrid
        gridClasses.GridView.ShowHeader = false;
        gridClasses.GridView.BorderWidth = 0;
        gridClasses.OnExternalDataBound += gridClasses_OnExternalDataBound;
        gridClasses.OnBeforeDataReload += gridClasses_OnBeforeDataReload;
        gridClasses.OnAfterRetrieveData += gridClasses_OnAfterRetrieveData;

        if (ConvertDocumentID > 0)
        {
            // Hide extra options
            plcNewABTestVariant.Visible = false;
            plcNewLink.Visible = false;
        }
        else
        {
            imgNewLink.ImageUrl = GetImageUrl("CMSModules/CMS_Content/Menu/Link.png");
            lblNewLink.Text = GetString("ContentNew.NewLink");
            lnkNewLink.NavigateUrl = "javascript:modalDialog('" + GetLinkDialogUrl(ParentNodeID) + "', 'contentselectnode', '90%', '85%')";

            plcNewABTestVariant.Visible = false;

            if (ParentNode != null)
            {
                // AB test variant settings
                if (SettingsKeyProvider.GetBoolValue(CMSContext.CurrentSiteName + ".CMSABTestingEnabled")
                    && AllowNewABTest
                    && !IsInDialog
                    && currentUser.IsAuthorizedPerResource("cms.ABTest", "Read")
                    && ModuleEntry.IsModuleLoaded("cms.onlinemarketing")
                    && ResourceSiteInfoProvider.IsResourceOnSite("CMS.ABTest", CMSContext.CurrentSiteName)
                    && LicenseHelper.CheckFeature(URLHelper.GetCurrentDomain(), FeatureEnum.ABTesting)
                    && (ParentNode.NodeAliasPath != "/"))
                {
                    string url = URLHelper.AddParameterToUrl(NewVariantUrl, "parentnodeid", ParentNodeID.ToString());
                    url = URLHelper.AddParameterToUrl(url, "parentculture", ParentCulture);

                    plcNewABTestVariant.Visible = true;
                    lblNewVariant.Text = GetString("abtesting.abtestvariant");
                    lnkNewVariant.NavigateUrl = URLHelper.GetAbsoluteUrl(url);

                    if (pnlFooter.Visible == false)
                    {
                        pnlABVariant.CssClass += "PageSeparator";
                    }
                }
            }

            imgNewVariant.ImageUrl = GetImageUrl("Objects/CMS_Variant/object_small.png");
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        gridClasses.GridView.Columns[1].Visible = false;
        gridClasses.GridView.GridLines = GridLines.None;

        // Set info label
        lblInfo.Text = Caption;

        // Show/hide new linked document panel
        if (!AllowNewLink)
        {
            plcNewLinkNew.Visible = false;
        }
    }

    #endregion


    #region "Methods"

    protected void gridClasses_OnBeforeDataReload()
    {
        if (ParentNode != null)
        {
            CurrentUserInfo currentUser = CMSContext.CurrentUser;

            // Check permission to create new document
            if (currentUser.IsAuthorizedToCreateNewDocument(ParentNode, null))
            {
                // Prepare where condition
                string where = "ClassID IN (SELECT ChildClassID FROM CMS_AllowedChildClasses WHERE ParentClassID=" + ValidationHelper.GetInteger(ParentNode.GetValue("NodeClassID"), 0) + ") " +
                               "AND ClassID IN (SELECT ClassID FROM CMS_ClassSite WHERE SiteID = " + CMSContext.CurrentSiteID + ")";

                if (!String.IsNullOrEmpty(gridClasses.CompleteWhereCondition))
                {
                    where += " AND (" + gridClasses.CompleteWhereCondition + ")";
                }

                if ((ConvertDocumentID > 0) || !PortalHelper.IsWireframingEnabled(CMSContext.CurrentSiteName))
                {
                    where += " AND ClassName <> 'CMS.Wireframe'";
                }

                // Add extra where condition
                where = SqlHelperClass.AddWhereCondition(where, Where);

                // Get the allowed child classes
                DataSet ds = DataClassInfoProvider.GetClasses("ClassID, ClassName, ClassDisplayName", where, null, gridClasses.TopN);

                List<DataRow> priorityRows = new List<DataRow>();
                DataTable resultTable = new DataTable();

                // Check user permissions for "Create" permission
                bool hasNodeAllowCreate = (currentUser.IsAuthorizedPerTreeNode(ParentNode, NodePermissionsEnum.Create) == AuthorizationResultEnum.Allowed);
                bool isAuthorizedToCreateInContent = currentUser.IsAuthorizedPerResource("CMS.Content", "Create");

                // No data loaded yet
                ClassesCount = 0;

                // If dataSet is not empty
                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    List<DataRow> rows = new List<DataRow>();
                    DataTable table = ds.Tables[0];
                    table.DefaultView.Sort = "ClassDisplayName";
                    resultTable = table.DefaultView.ToTable();

                    for (int i = 0; i < resultTable.Rows.Count; ++i)
                    {
                        DataRow dr = resultTable.Rows[i];
                        string doc = ValidationHelper.GetString(DataHelper.GetDataRowValue(dr, "ClassName"), string.Empty);

                        // Document type is not allowed, remove it from the data set
                        if (!isAuthorizedToCreateInContent && !currentUser.IsAuthorizedPerClassName(doc, "Create") && (!currentUser.IsAuthorizedPerClassName(doc, "CreateSpecific") || !hasNodeAllowCreate))
                        {
                            rows.Add(dr);
                        }
                        else
                        {
                            // Priority document types
                            // Page (Menu item)
                            if (doc.EqualsCSafe(DataClassInfoProvider.DefaultPageClassName, true))
                            {
                                priorityRows.Add(dr);
                                lastPriorityClassName = doc;
                            }
                            // Wireframe document
                            else if (doc.EqualsCSafe("cms.wireframe", true))
                            {
                                if (currentUser.IsAuthorizedPerResource("CMS.Design", "Wireframing"))
                                {
                                    priorityRows.Add(dr);
                                    lastPriorityClassName = doc;
                                }
                                else
                                {
                                    rows.Add(dr);
                                }
                            }
                        }
                    }

                    // Remove the document types
                    foreach (DataRow dr in rows)
                    {
                        resultTable.Rows.Remove(dr);
                    }

                    if (!DataHelper.DataSourceIsEmpty(resultTable))
                    {
                        int index = 0;

                        // Put priority rows to first position
                        foreach (DataRow priorityRow in priorityRows)
                        {
                            DataRow dr = resultTable.NewRow();
                            dr.ItemArray = priorityRow.ItemArray;

                            resultTable.Rows.Remove(priorityRow);
                            resultTable.Rows.InsertAt(dr, index);

                            index++;
                        }

                        ClassesCount = resultTable.Rows.Count;
                    }
                    else
                    {
                        // Show error message
                        SetErrorMessage(GetString("Content.NoPermissions"));

                        gridClasses.Visible = false;

                        ClassesCount = -1;
                    }
                }
                else
                {
                    if (!gridClasses.FilterIsSet && NoDataAsError)
                    {
                        // Show error message
                        SetErrorMessage(NoDataMessage);
                    }
                    else
                    {
                        gridClasses.ZeroRowsText = NoDataMessage;
                    }
                }

                dsClasses = new DataSet();
                dsClasses.Tables.Add(resultTable);

                gridClasses.DataSource = dsClasses;
            }
            else
            {
                // Show error message
                SetErrorMessage(GetString("Content.NoPermissions"));
            }
        }

        gridClasses.DataSource = dsClasses;
    }


    protected DataSet gridClasses_OnAfterRetrieveData(DataSet ds)
    {
        // Check if there are more options
        if (RedirectWhenNoChoice
            && !AllowNewABTest
            && !AllowNewLink
            && !URLHelper.IsPostback()
            && !DataHelper.DataSourceIsEmpty(ds))
        {
            DataTable table = ds.Tables[0];
            if (table.Rows.Count == 1)
            {
                int classId = ValidationHelper.GetInteger(table.Rows[0]["ClassId"], 0);

                // Redirect when only one document type found
                if (!string.IsNullOrEmpty(SelectionUrl))
                {
                    URLHelper.Redirect(GetSelectionUrl(classId));
                }
            }
        }

        return ds;
    }


    protected object gridClasses_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        if (sourceName.ToLowerCSafe() == "classname")
        {
            DataRowView drv = (DataRowView)parameter;

            // Get properties
            string className = ValidationHelper.GetString(drv["ClassName"], string.Empty);
            string classDisplayName = ResHelper.LocalizeString(CMSContext.ResolveMacros(ValidationHelper.GetString(drv["ClassDisplayName"], string.Empty)));
            int classId = ValidationHelper.GetInteger(drv["ClassId"], 0);

            string nameFormat = "<img style=\"border-right-width: 0px; border-top-width: 0px; border-bottom-width: 0px; border-left-width: 0px;\" src=\"{0}\" />{1}";

            // Append link if url specified
            if (!string.IsNullOrEmpty(SelectionUrl))
            {
                string url = GetSelectionUrl(classId);
                if (IsInDialog)
                {
                    url = URLHelper.UpdateParameterInUrl(url, "dialog", "1");
                    url = URLHelper.UpdateParameterInUrl(url, "reloadnewpage", "1");
                }

                // Prepare attributes
                string attrs = "";
                if (!string.IsNullOrEmpty(ClientTypeClick))
                {
                    attrs = string.Format("onclick=\"{0}\"", ClientTypeClick);
                }

                nameFormat = string.Format("<a class=\"ContentNewClass\" href=\"{0}\" {2}>{1}</a>", url, nameFormat, attrs);
            }

            // Format items to output
            return string.Format(nameFormat, ResolveUrl(GetDocumentTypeIconUrl(className)), HTMLHelper.HTMLEncode(classDisplayName)) + GenerateSpaceAfter(className);
        }

        return HTMLHelper.HTMLEncode(parameter.ToString());
    }


    /// <summary>
    /// Generates empty line after menu item link.
    /// </summary>
    /// <param name="className">Class name</param>
    public string GenerateSpaceAfter(object className)
    {
        string classNameStr = ValidationHelper.GetString(className, string.Empty).ToLowerCSafe();
        if (classNameStr.EqualsCSafe(lastPriorityClassName, true))
        {
            return "<br /><br />";
        }

        return string.Empty;
    }


    private void SetErrorMessage(string message)
    {
        // Show error message
        ShowError(message);

        lblInfo.Visible = false;
        pnlFooter.Visible = false;
        pnlABVariant.Visible = false;
    }

    #endregion
}
