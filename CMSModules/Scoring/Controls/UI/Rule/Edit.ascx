<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="CMSModules_Scoring_Controls_UI_Rule_Edit" %>
<%@ Register TagPrefix="cms" TagName="SelectValidity" Src="~/CMSAdminControls/UI/Selectors/SelectValidity.ascx" %>
<%@ Register Src="~/CMSFormControls/Basic/TextBoxControl.ascx" TagName="TextBoxControl"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Basic/CheckBoxControl.ascx" TagName="CheckBoxControl"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ContactManagement/FormControls/ActivityTypeSelector.ascx"
    TagName="ActivityTypeSel" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>

<cms:UIForm runat="server" ID="EditForm" ObjectType="om.rule" IsLiveSite="false"
    DefaultFieldLayout="Inline">
    <SecurityCheck Resource="CMS.Scoring" Permission="modify" />
    <LayoutTemplate>
        <asp:Panel ID="pnlGeneral" runat="server" CssClass="FieldPanel">
            <table>
                <tr>
                    <cms:FormField runat="server" ID="fDisplayName" Field="RuleDisplayName">
                        <td class="RuleSettingsLabel">
                            <cms:FormLabel ID="lblDisplayName" runat="server" EnableViewState="false" ResourceString="general.displayname"
                                DisplayColon="true" />
                        </td>
                        <td class="RuleSettingsControl">
                            <cms:LocalizableTextBox ID="txtDisplayName" runat="server" MaxLength="200" />
                        </td>
                    </cms:FormField>
                </tr>
                <tr>
                    <cms:FormField runat="server" ID="fCodeName" Field="RuleName">
                        <td class="RuleSettingsLabel">
                            <cms:FormLabel ID="lblCodeName" runat="server" EnableViewState="false" ResourceString="general.codename"
                                DisplayColon="true" />
                        </td>
                        <td class="RuleSettingsControl">
                            <cms:CodeName ID="txtName" runat="server" MaxLength="200" />
                        </td>
                    </cms:FormField>
                </tr>
                <tr>
                    <cms:FormField runat="server" ID="fValue" Field="RuleValue">
                        <td class="RuleSettingsLabel">
                            <cms:FormLabel ID="lblValue" runat="server" EnableViewState="false" ResourceString="om.score.scorevalue"
                                DisplayColon="true" />
                        </td>
                        <td class="RuleSettingsControl">
                            <cms:TextBoxControl ID="txtValue" runat="server" />
                        </td>
                    </cms:FormField>
                </tr>
                <tr>
                    <td colspan="2">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="RuleSettingsLabel">
                        <cms:FormLabel ID="lblType" runat="server" EnableViewState="false" ResourceString="om.score.ruletype"
                            DisplayColon="true" />
                    </td>
                    <td class="RuleSettingsControl">
                        <asp:RadioButtonList ID="radType" runat="server" RepeatDirection="Horizontal" AutoPostBack="true" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <cms:CMSUpdatePanel runat="server" ID="pnlUpdateActivity">
            <ContentTemplate>
                <asp:Panel ID="pnlSettings" runat="server" CssClass="FieldPanel">
                    <asp:PlaceHolder ID="plcAttribute" runat="server">
                        <table>
                            <tr>
                                <td class="RuleSettingsLabel">
                                    <cms:FormLabel ID="lblAttribute" runat="server" EnableViewState="false" ResourceString="om.score.attribute"
                                        DisplayColon="true" />
                                </td>
                                <td>
                                    <cms:LocalizedDropDownList ID="drpAttribute" runat="server" AutoPostBack="true" CssClass="DropDownField" />
                                </td>
                            </tr>
                        </table>
                        <cms:BasicForm ID="formCondition" runat="server" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plcActivitySettings" runat="server">
                        <table>
                            <tr>
                                <td class="RuleSettingsLabel">
                                    <cms:FormLabel ID="lblActivity" runat="server" EnableViewState="false" ResourceString="om.score.activity"
                                        DisplayColon="true" />
                                </td>
                                <td>
                                    <cms:ActivityTypeSel ID="ucActivityType" runat="server" ShowAll="false" AutoPostBack="true" />
                                </td>
                            </tr>
                        </table>
                        <cms:BasicForm ID="activityFormCondition" runat="server" />
                    </asp:PlaceHolder>
                </asp:Panel>
            </ContentTemplate>
        </cms:CMSUpdatePanel>
        <asp:PlaceHolder ID="plcActivity" runat="server">
            <asp:Panel ID="pnlActivity" runat="server" CssClass="FieldPanel">
                <table>
                    <tr>
                        <cms:FormField runat="server" ID="fRecurring" Field="RuleIsRecurring">
                            <td class="RuleSettingsLabel">
                                <cms:FormLabel ID="lblRecurring" runat="server" EnableViewState="false" ResourceString="om.score.recurringrule"
                                    DisplayColon="true" />
                            </td>
                            <td class="RuleSettingsControl">
                                <cms:CheckBoxControl ID="chkRecurring" runat="server" AutoPostBack="true" />
                            </td>
                        </cms:FormField>
                    </tr>
                    <tr>
                        <cms:FormField runat="server" ID="fMaxPoints" Field="RuleMaxPoints">
                            <td class="RuleSettingsLabel">
                                <cms:FormLabel ID="lblMaxPoints" runat="server" EnableViewState="false" ResourceString="om.score.maxpoints"
                                    DisplayColon="true" />
                            </td>
                            <td class="RuleSettingsControl">
                                <cms:TextBoxControl ID="txtMaxPoints" runat="server" />
                            </td>
                        </cms:FormField>
                    </tr>
                    <tr>
                        <td colspan="2">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td class="RuleSettingsLabel">
                            <cms:FormLabel ID="lblValidity" runat="server" EnableViewState="false" ResourceString="om.score.validity"
                                DisplayColon="true" />
                        </td>
                        <td class="RuleSettingsControl">
                            <cms:CMSUpdatePanel runat="server" ID="pnlUpdateValidity">
                                <ContentTemplate>
                                    <cms:SelectValidity ID="validity" runat="server" AutoPostBack="true" AutomaticallyDisableInactiveControl="true" />
                                </ContentTemplate>
                            </cms:CMSUpdatePanel>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </asp:PlaceHolder>
    </LayoutTemplate>
</cms:UIForm>
