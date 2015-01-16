<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Newsletters_Controls_Newsletter_Preview" CodeFile="Newsletter_Preview.ascx.cs" %>

<div id="topPanel" class="PageHeaderLine">
    <table>
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblSubscriber" runat="server" EnableViewState="false" ResourceString="selectsubscriber.itemname" DisplayColon="true" />&nbsp;&nbsp;&nbsp;&nbsp;
            </td>
            <td class="AlignRight">
                <asp:LinkButton ID="lnkPrevious" runat="server" OnClientClick="getPreviousSubscriber(); return false;" />
                <asp:Label ID="lblPrevious" runat="server" EnableViewState="false" />
            </td>
            <td style="width: 300px;" align="center">
                <asp:Label ID="lblEmail" runat="server" EnableViewState="false" />
            </td>
            <td class="AlignLeft">
                <asp:LinkButton ID="lnkNext" runat="server" OnClientClick="getNextSubscriber(); return false;" />
                <asp:Label ID="lblNext" runat="server" EnableViewState="false" />
            </td>
        </tr>
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblSubject" runat="server" EnableViewState="false" ResourceString="general.subject" DisplayColon="true" />
            </td>
            <td colspan="3"><asp:Label ID="lblSubjectValue" runat="server" EnableViewState="false" /></td>
        </tr>
    </table>
</div>
<iframe id="preview" style="width:100%; background-color:#fff;" frameborder="0"></iframe>

<script type="text/javascript">
//<![CDATA[
    function SetIFrameHeight(frameId) {
        var F = document.getElementById(frameId);
        var T = document.getElementById('topPanel');
        var contentElem = document.getElementById('divContent');
        if ((F != null) && (contentElem != null)) {
            offset = 0;
            if (T != null) {
                offset = T.offsetHeight;

                // Test for IE browser and add 3px to hide scrollbar
                if (/MSIE/.test(navigator.userAgent)) {
                    offset += 3;
                }
            }
            F.height = (contentElem.offsetHeight - offset);
        }
    }
    window.afterResize = function () { SetIFrameHeight('preview'); };
//]]>
</script>

<script type="text/javascript">
<!--
    var previewFrame;
    var emailItem;
    var nextLbl;
    var nextLnk;
    var prevLbl;
    var prevLnk;
    var emailItem;
    var subjItem;

    function InitObjects() {
        previewFrame = document.getElementById('preview');
        emailItem = document.getElementById(lblEmail);
        nextLbl = document.getElementById(lblNext);
        nextLnk = document.getElementById(lnkNext);
        prevLbl = document.getElementById(lblPrev);
        prevLnk = document.getElementById(lnkPrev);
        emailItem = document.getElementById(lblEmail);
        subjItem = document.getElementById(lblSubj);
    }

    function pageLoad() {
        InitObjects();

        previewFrame.src = "Newsletter_Issue_ShowPreview.aspx?subscriberguid=" + guid[currentSubscriberIndex] + "&issueid=" + newsletterIssueId;

        if ((guid != null) && (guid.length > 0)) {
            if (guid.length > 1) {
                nextLbl.style.display = "none";
            }
            else {
                nextLnk.style.display = "none";
            }
            prevLnk.style.display = "none";

            if ((email != null) && (email.length > 0)) {
                emailItem.innerHTML = email[currentSubscriberIndex];
            }
            if ((subject != null) && (subject.length > 0)) {
                subjItem.innerHTML = subject[currentSubscriberIndex];
            }
        }
        else {
            // Hide area with preview iframe
            document.getElementById('preview').style.display = 'none';
        }
    }

    function getPreviousSubscriber() {
        if ((currentSubscriberIndex > 0) && (guid.length > 0) && (email.length > 0) && (subject.length > 0)) {
            currentSubscriberIndex--;
            SetPreview();

            if (currentSubscriberIndex == 0) {
                prevLnk.style.display = "none";
                prevLbl.style.display = "inline";
            }
            nextLnk.style.display = "inline";
            nextLbl.style.display = "none";
        }
    }

    function getNextSubscriber() {
        if ((currentSubscriberIndex < email.length - 1) && (currentSubscriberIndex < subject.length - 1) && (currentSubscriberIndex < guid.length - 1) && (guid.length > 0) && (email.length > 0) && (subject.length > 0)) {
            currentSubscriberIndex++;
            SetPreview();

            if (currentSubscriberIndex == email.length - 1) {
                nextLnk.style.display = "none";
                nextLbl.style.display = "inline";
            }
            prevLnk.style.display = "inline";
            prevLbl.style.display = "none";
        }
    }

    function SetPreview() {
        emailItem.innerHTML = email[currentSubscriberIndex];
        subjItem.innerHTML = subject[currentSubscriberIndex];
        previewFrame.src = "Newsletter_Issue_ShowPreview.aspx?subscriberguid=" + guid[currentSubscriberIndex] + "&issueid=" + newsletterIssueId;
    }

-->
</script>
