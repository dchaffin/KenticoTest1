// ChatOnlineUsers webpart  object
function ChatSearchOnlineUsersWP(opt) {

    // Local variables
    var defaults = {
        onlineUserTemplate: "",
        chatRoomWindowUrl: "",
        contentClientID: "",
        clientID: "",
        GUID: "",
        textbox: "", 
        button: "", 
        pnlPaging: "", 
        pagingEnabled: false, 
        pagingItems:-1, 
        maxUsers:0,
        inviteMode:false,
        loadingDiv: "",
        resStrMoreFound: "",
        resStrNotFound: "",
        resStrFound: "",
        pnlInfo: "",
        envelopeID: "",
        groupID: "",
        invitePanel: ""
    };

    this.Options = jQuery.extend(defaults, opt);
    this.GroupManager = null;
    this.Type = "SearchOnlineUsers";

     var that = this,
        lastSearch = "",
        templateName = "chatOnlineUsersTemplate" + that.Options.clientID,
        content = jQuery("#" + that.Options.contentClientID),
        textbox = jQuery("#" + that.Options.textbox),
        pnlInfo = jQuery("#" + that.Options.pnlInfo),
        inviteMode = that.Options.inviteMode,
        windowUrl = that.Options.chatRoomWindowUrl,
        envelope = jQuery("#" + that.Options.envelopeID),
        invitePanel = jQuery("#" + that.Options.invitePanel);
        

    // Methods    
  
    // Process response from server
     this.ProcessResponse = function (users, memorySearched) {
         pnlInfo.empty();
         if ((users == null) || (users.length == 0)) {
             that.Clear();
             pnlInfo.append(jQuery("<div></div>").addClass("ChatSearchOnlineUsersNotFound").text(that.Options.resStrNotFound + " " + lastSearch));
             if (that.Options.inviteMode) {
                 invitePanel.hide();
             }
             return;
         }
         else {
             if ((!memorySearched) && that.Options.maxUsers > 0 && (users.length >= that.Options.maxUsers)) {
                 pnlInfo.append(jQuery("<div></div>").addClass("ChatSearchOnlineUsersMoreFound").text(that.Options.resStrMoreFound));
             }
             else {
                 pnlInfo.append(jQuery("<div></div>").addClass("ChatSearchOnlineUsersFound").text(that.Options.resStrFound + " " + lastSearch));
             }

             if (that.Options.inviteMode) {
                 invitePanel.show();
             }
         }

         that.ListPaging.Render(users);
     };


    this.Render = function(users) {
        content.empty();

        // Extend informations about users and fill the data to the panel
        var loggedUserID = ChatManager.Login.UserID;

        for (var i = 0; i < users.length; i++) {
            var usr = users[i];
            if (inviteMode == true) {
                usr.OnClick = "ChatManager.InviteToRoom(ChatManager.OpenedInvitePrompt.groupID, " + usr.ChatUserID + "); ChatManager.DialogsHelper.CloseDialog(ChatManager.OpenedInvitePrompt.overlay); return false;";
                if (usr.ChatUserID != loggedUserID) {
                    jQuery.tmpl(templateName, usr).appendTo(content);
                }
            }
            else {
                usr.OnClick = "ChatManager.InitOneToOneChat(" + usr.ChatUserID + ", \"" + windowUrl + "\", \"" + that.Options.GUID + "\"); return false;";
                usr.IsCurrentUser = (usr.ChatUserID == loggedUserID) ? true : false;
                jQuery.tmpl(templateName, usr).appendTo(content);
            }
        }
    }
    
    
    // Clears webpart content
    this.Clear = function() {
        content.empty();
        pnlInfo.empty();
        that.ListPaging.Clear();

        if (invitePanel.length > 0) {
            invitePanel.hide();
        }

        textbox.val(lastSearch);

        if (envelope.length > 0) {
            if (ChatManager.Login.IsLoggedIn == false) {
                envelope.hide();
            }
            else {
                envelope.show();
            }
        }
    };


    this.Reset = function() {
        lastSearch = "";
        that.Clear();
    }
    
    
    // Show or hide loading div
    this.ShowLoading = function(show) {
        if (show == true) {
            content.html(that.Options.loadingDiv); 
        }
        else{
            content.find(".ChatSearchOnlineUsersWPLoading").remove();
        }
    };


    function Inicialize() {
        textbox.keydown(function(evt) {
            var e = window.event || evt;
            var key = e.keyCode;

            if (key == 13) {
                if (e.preventDefault) e.preventDefault();
                if (e.stopPropagation) e.stopPropagation();
                e.returnValue = false;
                search();
                return false;
            }
        });

        // Build jQuery template
        jQuery.template(templateName, "<div class=\"SearchedChatOnlineUser\">" + that.Options.onlineUserTemplate + "</div>");

        // Onlick event handler
        jQuery("#" + that.Options.button).click(function() {
            search();
            return false;
        });
    }


    function search() {
        lastSearch = textbox.val();
        if (lastSearch == "") {
            that.Clear();
            return;
        }

        // if invite mode activated, room ID is sent
        if (that.Options.inviteMode == true) {
            if (that.GroupManager == null) {
                that.GroupManager = ChatManager.FindGroupManager(that.Options.groupID);
            }
            var roomID = null;
            if (that.GroupManager != null) {
                roomID = that.GroupManager.RoomInfo.ChatRoomID;
            }
        }

        ChatManager.SearchOnlineUsers(lastSearch, that, roomID);
    }


    this.ListPaging = new ListPaging(
        {
            groupID: that.Options.groupID,
            clientID: that.Options.clientID,
            wpType: that.Type,
            functionRenderList: that.Render,
            filterEnabled: false,
            pagingEnabled: that.Options.pagingEnabled,
            pagingItems: that.Options.pagingItems,
            pagingContentID: that.Options.pnlPaging
        });
    

    that.Clear();
    Inicialize()             
}


// Inits chat online users web part functionality
function InitChatSearchOnlineUsersWebpart(opt) {
    InicializeChatManager();

    // Create chat online users object and add it to ChatManager
    ChatManager.RegisterWebpart(new ChatSearchOnlineUsersWP(opt));  
}

