<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AuthorizationSetupPrologue.aspx.cs" Inherits="CMSModules_ContactManagement_Pages_Tools_SalesForce_AuthorizationSetupPrologue" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" EnableEventValidation="false" Theme="Default" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="plcContent" runat="Server" EnableViewState="false">
	<asp:Panel ID="MainPanel" runat="server" CssClass="PageContent" EnableViewState="false">
		<div id="ConfirmationElement" style="display:none">
			<span class="AdvancedLabel Success"><%= GetString("sf.authorizationprologue.success") %></span>
		</div>
		<div id="ErrorElement" style="display:none">
			<span class="AdvancedLabel Error"><%= GetString("sf.authorizationprologue.error") %></span>
		</div>
		<p id="ProgressElement"><%= GetString("sf.authorizationprologue.progress") %></p>
		<script type="text/javascript">

		    jQuery(document).ready(function () {
		        var continueButtonId = "<%= ContinueButton.ClientID %>";
		        var authorizationSetupUrl = "<%= AuthorizationSetupUrl %>";
		        var authorizationSetupHandlerUrl = "<%= AuthorizationSetupHandlerUrl %>";

			    jQuery.ajax(authorizationSetupHandlerUrl, {
			        dataType: "jsonp",
			        timeout: 5000,
			        complete: function (request, status) {
			            jQuery("#ProgressElement").hide();
			            if (status == "success") {
			                jQuery("#ConfirmationElement").show();
			                window.setTimeout(function () {
			                    window.location.href = authorizationSetupUrl;
			                }, 1000);
			            }
			            else {
			                jQuery("#ErrorElement").show();
			                jQuery("#" + continueButtonId).removeClass("Hidden").click(function (e) {
			                    window.location.href = authorizationSetupUrl;
			                    e.preventDefault();
			                });

			            }
			        }
			    });
			});
	
		</script>
	</asp:Panel>
</asp:Content>
<asp:Content ID="FooterContent" ContentPlaceHolderID="plcFooter" runat="server">
	<div id="FooterPanel" runat="server" class="FloatRight">
		<cms:LocalizedButton ID="ContinueButton" runat="server" CssClass="SubmitButton Hidden" EnableViewState="False" ResourceString="general.continue" />
		<cms:LocalizedButton ID="CancelButton" runat="server" CssClass="SubmitButton" EnableViewState="False" ResourceString="general.cancel" OnClientClick="window.close(); return false;" />
	</div>
</asp:Content>