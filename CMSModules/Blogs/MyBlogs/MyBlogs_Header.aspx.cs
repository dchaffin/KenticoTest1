using System;

using CMS.UIControls;

[Title("CMSModules/CMS_Blog/module.png", "myblogs.header.myblogs", "my_blogs_comments")]
[Tabs("blogsContent")]
[Tab(0, "myblogs.header.comments", "MyBlogs_Comments_List.aspx", "SetHelpTopic('helpTopic', 'my_blogs_comments');")]
[Tab(1, "myblogs.header.blogs", "MyBlogs_Blogs_List.aspx", "SetHelpTopic('helpTopic', 'my_blogs_list');")]
public partial class CMSModules_Blogs_MyBlogs_MyBlogs_Header : CMSMyBlogsPage
{

}