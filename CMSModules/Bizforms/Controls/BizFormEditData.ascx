<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BizFormEditData.ascx.cs" Inherits="CMSModules_BizForms_Controls_BizFormEditData" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<cms:unigrid runat="server" id="gridData" islivesite="false" >
    <GridActions>
       <ug:action name="edit" externalsourcename="edit" caption="$General.Edit$" icon="Edit.png" />
       <ug:action name="delete" externalsourcename="delete" caption="$General.Delete$" icon="Delete.png" confirmation="$general.confirmdelete$"/>
    </GridActions>
    <GridColumns>
    </GridColumns>
    <GridOptions DisplayFilter="true" />
</cms:unigrid>