<%@ Page Language="C#" AutoEventWireup="true" CodeFile="JoinDB.aspx.cs" Inherits="CMSInstall_JoinDB"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Contact management database join" %>

<%@ Register Src="~/CMSAdminControls/AsyncControl.ascx" TagName="AsyncControl" TagPrefix="cms" %>
<%@ Register Src="Controls/StepNavigation.ascx" TagName="StepNavigation" TagPrefix="cms" %>
<%@ Register Src="~/CMSInstall/Controls/WizardSteps/AsyncProgress.ascx" TagName="AsyncProgress"
    TagPrefix="cms" %>
<%@ Register Src="Controls/WizardSteps/JoinPrerequisites.ascx" TagName="JoinPrerequisites"
    TagPrefix="cms" %>
<%@ Register Src="Controls/WizardSteps/SeparationFinished.ascx" TagName="SeparationFinished"
    TagPrefix="cms" %>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="Server">
    <asp:Panel runat="server" ID="pnlBody">
        <asp:Panel ID="layPanel" runat="server" CssClass="InstallPanel SeparatePanel JoinDB">
            <asp:Label ID="lblHeader" CssClass="InstallHeader" runat="server" />
            <asp:Image ID="imgHeader" runat="server" CssClass="InstalHeader" />
            <asp:Panel runat="server" ID="pnlWizard" CssClass="InstallerContent">
                <asp:Wizard ID="wzdInstaller" runat="server" DisplaySideBar="False" OnNextButtonClick="wzdInstaller_NextButtonClick"
                    Width="100%">
                    <StepNavigationTemplate>
                        <cms:StepNavigation ID="stepNavigation" runat="server" NextPreviousVisible="True" />
                    </StepNavigationTemplate>
                    <StartNavigationTemplate>
                        <cms:StepNavigation ID="startStepNavigation" runat="server" />
                    </StartNavigationTemplate>
                    <FinishNavigationTemplate>
                        <cms:StepNavigation ID="finishNavigation" runat="server" NextButton-ResourceString="general.finish"
                            FinishStep="True" />
                    </FinishNavigationTemplate>
                    <WizardSteps>
                        <asp:WizardStep ID="stpPrerequisites" runat="server" StepType="Start">
                            <cms:JoinPrerequisites ID="joinPrerequisites" runat="server" />
                        </asp:WizardStep>
                        <asp:WizardStep ID="stpProgress" runat="server" AllowReturn="false" StepType="Step">
                            <cms:AsyncProgress ID="progress" runat="server" />
                        </asp:WizardStep>
                        <asp:WizardStep ID="stpFinish" runat="server" StepType="Finish">
                            <cms:SeparationFinished ID="separationFinished" runat="server" />
                        </asp:WizardStep>
                    </WizardSteps>
                </asp:Wizard>
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
    <cms:AsyncControl ID="asyncControl" runat="server" PostbackOnError="False" />
    <asp:HiddenField ID="hdnConnString" runat="server" />
</asp:Content>
