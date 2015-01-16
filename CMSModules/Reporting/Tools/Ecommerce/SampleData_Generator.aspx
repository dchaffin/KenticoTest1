<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SampleData_Generator.aspx.cs"
    Inherits="CMSModules_Reporting_Tools_Ecommerce_SampleData_Generator" Theme="Default"
    EnableEventValidation="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel runat="server" ID="pnlUpdate">
        <ContentTemplate>
            <cms:CMSPanel runat="server" ID="pnlGenerateData" GroupingTextResourceString="com.generator.generategroup">
                <table>
                    <tr>
                        <td>
                            <cms:LocalizedLabel ResourceString="com.generator.generatepericope" runat="server"
                                ID="lblGeneratePericope" />
                            <br />
                            <br />
                            <cms:LocalizedButton runat="server" ID="btnGenerate" OnClick="btnGenerateClick" CssClass="SubmitButton"
                                ResourceString="com.generator.generatebutton" />
                        </td>
                    </tr>
                </table>
            </cms:CMSPanel>
            <br />
            <br />
            <cms:CMSPanel runat="server" ID="pnlDeleteData" GroupingTextResourceString="com.generator.deletegroup">
                <table>
                    <tr>
                        <td>
                            <cms:LocalizedLabel ResourceString="com.generator.deletepericope" runat="server"
                                ID="lblDeletePericope" />
                            <br />
                            <br />
                            <cms:LocalizedButton runat="server" ID="btnDelete" OnClick="btnDeleteClick" CssClass="SubmitButton"
                                ResourceString="com.generator.deletebutton" />
                        </td>
                    </tr>
                </table>
            </cms:CMSPanel>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
