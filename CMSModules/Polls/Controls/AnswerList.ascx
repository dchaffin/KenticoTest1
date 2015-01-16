<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Polls_Controls_AnswerList"
    CodeFile="AnswerList.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Panel ID="pnlBody" runat="server">
    <cms:UniGrid runat="server" ID="uniGrid" ObjectType="polls.pollanswer" OrderBy="AnswerOrder ASC"
        Columns="AnswerID, AnswerText, AnswerCount, AnswerEnabled, AnswerForm">
        <GridActions>
            <ug:Action Name="edit" Caption="$General.Edit$" Icon="edit.png" ExternalSourceName="edit"/>
            <ug:Action Name="delete" Caption="$General.Delete$" Icon="delete.png" Confirmation="$General.ConfirmDelete$" ExternalSourceName="delete" />
            <ug:Action Name="moveup" Caption="$General.MoveUp$" Icon="up.png" ExternalSourceName="moveup"/>
            <ug:Action Name="movedown" Caption="$General.MoveDown$" Icon="down.png" ExternalSourceName="movedown"/>
            <ug:Action Name="viewresults" Caption="$Polls.ViewResults$" Icon="ViewChart.png" ExternalSourceName="AnswerForm" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="AnswerText" Caption="$Unigrid.Polls_Answer.Columns.AnswerText$"
                Wrap="false" Localize="true">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="AnswerCount" Caption="$Unigrid.Polls_Answer.Columns.AnswerCount$"
                Wrap="false">
                <Filter Type="integer" />
            </ug:Column>
            <ug:Column Source="AnswerEnabled" ExternalSourceName="#yesno" Caption="$general.enabled$"
                Wrap="false" />
            <ug:Column Source="AnswerForm" ExternalSourceName="AnswerIsOpenEnded" Caption="$general.type$"
                Wrap="false" />
            <ug:Column Width="100%" />
        </GridColumns>
    </cms:UniGrid>
</asp:Panel>
