function ChatSupportRequest(opt) {
    InicializeChatManager();

    var content = jQuery("#" + opt.clientID);
    var proxy = ChatManager.WebService;
    if (proxy == null) {
        content.hide();
        return;
    }    

    var requestTemplateB = "requestTemplateB_" + opt.clientID;
    jQuery.template(requestTemplateB, opt.trans);

    CheckAvailability(null);
    
    // Define overlay for inform dialog
    ChatManager.DialogsHelper.SetDialogOverlay(opt.pnlInformDialog);

    // Set event handling for closing inform dialog
    jQuery(opt.btnInformDialogClose).click(function() {
        ChatManager.DialogsHelper.CloseDialog(opt.pnlInformDialog);
        return false;
    });

    content.click(function () {
        // Before opening popup check support chat.
        CheckAvailability(function (online) {
            if (online) {
                // Open live support chat window.
                proxy.CreateSupportChatRoom(function (response) {
                    if (response.StatusCode == 0) {
                        var room = response.Data;
                        var win = window.open(opt.onlineUrl + "?windowroomid=" + room.ChatRoomID + "&popupSettingsId=" + opt.guid, room.ChatRoomID, "width=600,height=800,location=0,scrollbars=1,resizable=1");
                        if (win != null) {
                            testWindow(win, 600, 800);
                        }
                        else {
                            alert(ChatManager.Settings.PopupWindowErrorMsg);
                        }
                    } else {
                        alert(response.StatusMessage);
                    }
                }, function (err) {
                    alert(err.get_message());
                }, null);
            }
            else {
                // Open offline support request form.
                if (opt.mailEnabled) {
                    var win = window.open(ChatManager.Settings.SupportMailDialogURL, "OfflineSupportForm", ChatManager.Settings.SupportMailDialogSettings);
                    testWindow(win, 600, 800);
                }
                else {
                    ChatManager.DialogsHelper.DisplayDialog(opt.pnlInformDialog);
                }
            }
        });
    });


    function testWindow(win, width, height) {
        if (win == null || typeof (win) == 'undefined') {
            alert(ChatManager.Settings.PopupWindowErrorMsg);
        }
        else {
            win.focus();
            if (jQuery.browser.opera || jQuery.browser.webkit) {
                setTimeout(function () {
                    if (!win.closed && (win.innerHeight != height) && (win.innerWidth != width)) {
                        alert(ChatManager.Settings.PopupWindowErrorMsg);
                    }
                }, 1000);
            }
        }
    }

    function CheckAvailability(callback) {
        proxy.GetSupportEngineersOnlineCount(function(response) {
            if (response.StatusCode == 0) {
                var online = (response.Data > 0);
                var data = {
                    LiveSupport: online,
                    EmailEnabled: opt.mailEnabled
                }

                content.empty();
                jQuery.tmpl(requestTemplateB, data).appendTo(content);

                if (callback) {
                    callback(online);
                }
            } else {
                alert(response.StatusMessage);
            }
        }, function(err) {
            alert(err.get_message());
        }, null);
    }
}