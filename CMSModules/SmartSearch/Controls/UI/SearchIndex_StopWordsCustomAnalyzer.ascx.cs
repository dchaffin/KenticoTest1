using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.IO;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.SettingsProvider;

public partial class CMSModules_SmartSearch_Controls_UI_SearchIndex_StopWordsCustomAnalyzer : CMSUserControl
{
    #region "Variables"

    private SearchIndexInfo mIndexInfo = null;
    private DropDownList mDropDown = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Sets the search index info.
    /// </summary>
    public SearchIndexInfo IndexInfo
    {
        set
        {
            mIndexInfo = value;
        }
    }


    /// <summary>
    /// Sets the analyzer drop down element.
    /// </summary>
    public DropDownList AnalyzerDropDown
    {
        set
        {
            mDropDown = value;
        }
    }


    /// <summary>
    /// Gets the custom analyzer assembly name.
    /// </summary>
    public string CustomAnalyzerAssemblyName
    {
        get
        {
            return txtCustomAnalyzerAssemblyName.Text.Trim();
        }
    }


    /// <summary>
    /// Gets the custom analyzer class name.
    /// </summary>
    public string CustomAnalyzerClassName
    {
        get
        {
            return txtCustomAnalyzerClassName.Text.Trim();
        }
    }


    /// <summary>
    /// Gets the stop words file.
    /// </summary>
    public string StopWordsFile
    {
        get
        {
            return drpStopWords.SelectedValue;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        Initialize();
    }


    protected void Initialize()
    {
        // Prepare JavaScript
        string stopWordsScript = ScriptHelper.GetScript(@" function CheckAnalyzerOptions(){
        stopRow = document.getElementById('" + stopWordsRow.ClientID + @"');
        analyzerElem = document.getElementById('" + mDropDown.ClientID + @"');
        if ((analyzerElem.value == 'stop')||(analyzerElem.value == 'standard'))
        {
            stopRow.style.display = '';
        }
        else
        {
            stopRow.style.display = 'none';
        }

        customRow = document.getElementById('" + customAnalyzerAssemblyName.ClientID + @"');
        customRow2 = document.getElementById('" + customAnalyzerClassName.ClientID + @"');
        if (analyzerElem.value == 'custom')
        {
            customRow.style.display = '';
            customRow2.style.display = '';
        }
        else
        {
            customRow.style.display = 'none';
            customRow2.style.display = 'none';
        }
        }");

        // Add event to drop down list control
        mDropDown.Attributes.Add("onchange", "CheckAnalyzerOptions();");

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "StopWordsDropDown", stopWordsScript);

        if (!RequestHelper.IsPostBack())
        {
            string stopWordsDir = SearchIndexInfo.IndexPathPrefix + "_StopWords\\";
           
            // Get stop words files
            if (Directory.Exists(stopWordsDir))
            {
                string[] files = Directory.GetFiles(stopWordsDir, "*.txt");

                if (files != null)
                {
                    foreach (string file in files)
                    {
                        string name = file;
                        int index = name.LastIndexOfCSafe('\\');
                        if (index > -1)
                        {
                            name = name.Substring(index + 1);
                        }

                        name = name.Substring(0, name.LastIndexOfCSafe('.'));

                        // Fill drop down list
                        drpStopWords.Items.Add(new ListItem(name, name));
                    }
                }
            }

            drpStopWords.Items.Insert(0, new ListItem("(" + GetString("general.default") + ")", String.Empty));
        }

        bool hideCustom = true;
        bool hideStop = true;

        AnalyzerTypeEnum type = SearchIndexInfoProvider.AnalyzerCodenameToEnum(mDropDown.SelectedValue);

        if ((!RequestHelper.IsPostBack()) && (mIndexInfo != null))
        {
            // Set custom analyzer values
            type = mIndexInfo.IndexAnalyzerType;
            txtCustomAnalyzerAssemblyName.Text = mIndexInfo.CustomAnalyzerAssemblyName;
            txtCustomAnalyzerClassName.Text = mIndexInfo.CustomAnalyzerClassName;
            try
            {
                drpStopWords.SelectedValue = mIndexInfo.StopWordsFile;
            }
            catch
            {
            }
        }


        switch (type)
        {
            case AnalyzerTypeEnum.CustomAnalyzer:
                hideCustom = false;
                break;

            case AnalyzerTypeEnum.StandardAnalyzer:
            case AnalyzerTypeEnum.StopAnalyzer:
                hideStop = false;
                break;
        }

        // Hide if unnecessary
        stopWordsRow.Style.Clear();
        if (hideStop)
        {
            stopWordsRow.Style.Add("display", "none");
        }

        customAnalyzerAssemblyName.Style.Clear();
        customAnalyzerClassName.Style.Clear();

        if (hideCustom)
        {
            customAnalyzerAssemblyName.Style.Add("display", "none");
            customAnalyzerClassName.Style.Add("display", "none");
        }
    }

    #endregion
}