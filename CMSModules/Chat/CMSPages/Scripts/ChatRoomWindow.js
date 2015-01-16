function ChatRoomWindow(opt) {
    var options = {
        roomID: 0,
        password: "",
        pnlChatRoomPasswordPrompt: "",
        txtChatRoomPasswordPromptInput: "",
        btnChatRoomPasswordPromptSubmit: "",
        isOneToOne: true,
        pnlPasswordPromptError: "",
        pnlChatRoomWindow: "",
        ChatRoomMessagesClientID: "",
        btnClose: "",
        isSupport: false
    };
    
    jQuery.extend(options, opt);
    jQuery(Inicialize);

    var pnlChatRoomWindowTop = jQuery(options.pnlChatRoomWindow),
        messagesEnvelope = null,
        that = this;


    this.RecalculatePositions = function () {
        if (messagesEnvelope == null) {
            var wp = ChatManager.GetWebpart(null, options.ChatRoomMessagesClientID, "ChatMessages", true);
            if (wp) {
                messagesEnvelope = wp.GetEnvelope();
            }
        }
        if (messagesEnvelope) {
            messagesEnvelope.offset({ top: pnlChatRoomWindowTop.outerHeight(true) + 35 });
        }
    }    


    function Inicialize() {

        // Window event
        jQuery(window).bind("beforeunload", function(event) {
            for (var i = 0; i < ChatManager.ChatGroupManagers.length; i++) {
                ChatManager.ChatGroupManagers[i].LeaveRoom();
            }
        });

        ChatManager.Login.IsSupporter = options.isSupport;

        jQuery(options.btnClose).click(function () {
            if (jQuery.browser.opera || jQuery.browser.safari) {
                for (var i = 0; i < ChatManager.ChatGroupManagers.length; i++) {
                    ChatManager.ChatGroupManagers[i].LeaveRoom();
                }
            }
            if (jQuery.browser.safari) {
                setTimeout(window.close, 300);
            }
            else {
                window.close();
            }
            return false;
        });

        pnlChatRoomWindowTop.resize(that.RecalculatePositions);

        // Send mail to support
        if (options.isCustomerSupport) {
            jQuery(options.hplSupportSendMailClientID).click(function(event) {
                var params = "?roomid=" + options.roomID;

                // Open offline support request form.
                var win = window.open(ChatManager.Settings.SupportMailDialogURL + params, "OfflineSupportForm", ChatManager.Settings.SupportMailDialogSettings);
                win.focus();

                return false;
            });
        } 

        // Deal with password protected rooms
        if (options.password) {
            
            // Define overlay for password prompt
            ChatManager.DialogsHelper.SetDialogOverlay(options.pnlChatRoomPasswordPrompt);

            // Set event handling for input
            jQuery(options.btnChatRoomPasswordPromptSubmit).click(enterPassword);

            jQuery(options.txtChatRoomPasswordPromptInput).bind("keydown", function(evt) {
                var e = window.event || evt;
                var key = e.keyCode;

                if (key == 13) {
                    if (e.preventDefault) e.preventDefault();
                    if (e.stopPropagation) e.stopPropagation();
                    e.returnValue = false;

                    enterPassword();
                }
            });

            // Display prompt
            jQuery(options.pnlPasswordPromptError).empty();
            ChatManager.DialogsHelper.DisplayDialog(options.pnlChatRoomPasswordPrompt);
            jQuery(options.txtChatRoomPasswordPromptInput).focus();
        } 
        else {
            joinRoom(null);
        }

        RecalculatePositions();
    };
   
    
    function joinRoom(password, fnok, fnerr) {
        for (var i = 0; i < jQuery.ChatManager.ChatGroupManagers.length; i++) {
            jQuery.ChatManager.ChatGroupManagers[i].JoinRoom(options.roomID, password, fnok, fnerr);
        }
    };


    function enterPassword() {
        joinRoom(jQuery(options.txtChatRoomPasswordPromptInput).val(), function() {

            // When success, close the overlay
            ChatManager.DialogsHelper.CloseDialog(options.pnlChatRoomPasswordPrompt);
        }, function(err) {
            jQuery(options.pnlPasswordPromptError).html(err);
        });
        return false;
    };
}