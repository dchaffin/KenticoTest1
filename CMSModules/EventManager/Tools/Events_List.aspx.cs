using System;

using CMS.UIControls;

public partial class CMSModules_EventManager_Tools_Events_List : CMSEventManagerPage
{
    protected override void OnPreRender(EventArgs e)
    {
        // Set the page title
        CurrentMaster.Title.TitleText = GetString("Events_List.HeaderCaption");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_EventManager/object.png");
        CurrentMaster.Title.HelpName = "helpTopic";
        CurrentMaster.Title.HelpTopicName = "booking_system_list";

        eventList.ReloadData();
        base.OnPreRender(e);
    }
}