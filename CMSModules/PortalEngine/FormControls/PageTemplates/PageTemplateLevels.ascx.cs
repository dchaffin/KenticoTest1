using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.CMSHelper;
using CMS.DocumentEngine;

public partial class CMSModules_PortalEngine_FormControls_PageTemplates_PageTemplateLevels : FormEngineUserControl
{
    #region "Variables"

    private bool mDocummentSettings = false;
    
    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets tree path - if set is created from TreePath.
    /// </summary>
    public string TreePath
    {
        get
        {
            return treeElem.TreePath;
        }
        set
        {
            treeElem.TreePath = value;
        }
    }


    /// <summary>
    /// Gets or sets Level, levels are rendered only if TreePath is not set. 
    /// </summary>
    public int Level
    {
        get
        {
            return treeElem.Level;
        }
        set
        {
            treeElem.Level = value;
        }
    }


    /// <summary>
    /// Gets or sets value.
    /// </summary>
    public override object Value
    {
        get
        {
            // Inherit all
            if (radInheritAll.Checked)
            {
                return String.Empty;
            }

            // Do not inherit any content
            if (radNonInherit.Checked)
            {
                return "/";
            }

            // Inherit from master
            if (radInheritMaster.Checked)
            {
                return "\\";
            }

            return treeElem.Value;
        }
        set
        {
            if (!RequestHelper.IsPostBack() || String.IsNullOrEmpty((string)treeElem.Value))
            {
                treeElem.Value = value;
                SetRadioButtons();
            }
        }
    }


    /// <summary>
    /// Document settings.
    /// </summary>
    public bool DocumentSettings
    {
        get
        {
            return mDocummentSettings;
        }
        set
        {
            mDocummentSettings = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (DocumentManager != null)
        {
            CMS.DocumentEngine.TreeNode node = DocumentManager.Node;
            if (node != null)
            {
                DocumentSettings = true;

                // Try get info whether exist linked document in path
                DataSet ds = DocumentManager.Tree.SelectNodes(CMSContext.CurrentSiteName, "/%", node.DocumentCulture, false, null, "NodeLinkedNodeID IS NOT NULL AND (N'" + SqlHelperClass.GetSafeQueryString(node.NodeAliasPath) + "' LIKE NodeAliasPath + '%')", null, -1, false, 1, "Count(*) AS NumOfDocs");

                // If node is not link or none of parent documents is not linked document use document name path
                if (!node.IsLink && ValidationHelper.GetInteger(DataHelper.GetDataRowValue(ds.Tables[0].Rows[0], "NumOfDocs"), 0) == 0)
                {
                    TreePath = TreePathUtils.GetParentPath("/" + node.DocumentNamePath);
                }
                else
                {
                    // Otherwise use alias path
                    TreePath = TreePathUtils.GetParentPath("/" + node.NodeAliasPath);
                }
            }
        }

        if (DocumentSettings)
        {
            radInheritAll.Text = GetString("InheritLevels.UseTemplateSettigns");
        }
        else
        {
            radInheritAll.Text = GetString("InheritLevels.InheritAll");
        }
        radNonInherit.Text = GetString("InheritLevels.NonInherit");
        radSelect.Text = GetString("InheritLevels.Select");
        radInheritMaster.Text = GetString("InheritLevels.InheritMaster");
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        plcTree.Visible = radSelect.Checked;
    }


    /// <summary>
    /// Sets the value to the radio buttons
    /// </summary>
    private void SetRadioButtons()
    {
        radInheritAll.Checked = true;
        string treeValue = ValidationHelper.GetString(treeElem.Value, string.Empty);

        // Do not inherit any content
        if (treeValue == "/")
        {
            radNonInherit.Checked = true;
        }
        // Inherit from master
        else if (treeValue == "\\")
        {
            radInheritMaster.Checked = true;
        }
        //  Inherited levels
        else if (!String.IsNullOrEmpty(treeValue))
        {
            radSelect.Checked = true;
        }
    }

    #endregion
}