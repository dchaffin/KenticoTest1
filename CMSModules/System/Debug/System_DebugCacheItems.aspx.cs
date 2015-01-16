using System;
using System.Collections.Generic;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.UIControls;

public partial class CMSModules_System_Debug_System_DebugCacheItems : CMSDebugPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        btnClear.Text = GetString("Administration-System.btnClearCache");

        titleDummy.TitleText = GetString("Debug.DummyKeys");
        titleItems.TitleText = GetString("Debug.DataItems");
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ReloadData();
    }


    /// <summary>
    /// Loads the data.
    /// </summary>
    protected void ReloadData()
    {
        lock (HttpRuntime.Cache)
        {
            List<string> keyList = new List<string>();
            IDictionaryEnumerator CacheEnum = HttpRuntime.Cache.GetEnumerator();

            // Build the items list
            while (CacheEnum.MoveNext())
            {
                string key = CacheEnum.Key.ToString();
                if (!String.IsNullOrEmpty(key))
                {
                    keyList.Add(key);
                }
            }
            keyList.Sort();

            // Load the grids
            gridItems.AllItems = keyList;
            gridItems.ReloadData();

            gridDummy.AllItems = keyList;
            gridDummy.ReloadData();
        }
    }


    protected void btnClear_Click(object sender, EventArgs e)
    {
        CMSFunctions.ClearCache();

        ReloadData();
    }
}