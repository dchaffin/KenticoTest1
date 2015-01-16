using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.Synchronization;
using CMS.UIControls;

// Set default page title
[Title(ResourceString = "objectversioning.viewversion.defaulttitle", ImageUrl = "CMSModules/CMS_ObjectVersioning/viewversion.png")]
public partial class CMSModules_Objects_Dialogs_ViewObjectVersion : CMSObjectVersioningPage
{
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        SetCulture();

        // Check hash
        if (!QueryHelper.ValidateHash("hash"))
        {
            RedirectToAccessDenied(ResHelper.GetString("dialogs.badhashtitle"));
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        btnClose.Text = GetString("General.Close");
        bool noCompare = ValidationHelper.GetBoolean(QueryHelper.GetString("nocompare", string.Empty), false);
        // Initialize view version control
        viewVersion.NoComparison = noCompare;
        int versionId = ValidationHelper.GetInteger(QueryHelper.GetString("versionhistoryid", string.Empty), 0);
        viewVersion.VersionCompareID = ValidationHelper.GetInteger(QueryHelper.GetString("comparehistoryid", string.Empty), 0);

        // Get version to initialize title
        ObjectVersionHistoryInfo version = ObjectVersionHistoryInfoProvider.GetVersionHistoryInfo(versionId);
        if (version != null)
        {
            string objectType = version.VersionObjectType;
            string objType = GetString("ObjectType." + objectType.Replace(".", "_"));
            string title = String.Format(GetString("objectversioning.viewversion.title"), objType, HTMLHelper.HTMLEncode(ResHelper.LocalizeString(version.VersionObjectDisplayName)));
            // Set title - do not display context help when comparison not available
            string helpTopic = noCompare ? null : "objectversioning_viewversion";
            SetTitle("CMSModules/CMS_ObjectVersioning/viewversion.png", title, helpTopic, "Object versioning - View version");

            viewVersion.Version = version;

            // Exclude site binding table data
            GeneralizedInfo infoObj = TranslationHelper.GetReadOnlyObject(objectType);
            viewVersion.ExcludedTableNames = CMSObjectHelper.GetTableName(infoObj.TypeInfo.SiteBindingObject);
        }

        // Set what data should be displayed
        bool showAll = ValidationHelper.GetBoolean(QueryHelper.GetBoolean("showall", false), false);
        viewVersion.ObjectDataOnly = !showAll;
    }
}