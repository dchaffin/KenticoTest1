function SupportRoomObj(opt) {
    this.RoomID = opt.id;
    this.MessageCount = opt.messageCount;
    this.Name = opt.name;
    this.Taken = opt.taken ? opt.taken : false;
    this.Window = null;
    this.Personal = opt.personal;
    this.UserID = opt.userID ? opt.userID : -1;
    this.ContactID = opt.contactID ? opt.contactID : -1;

    var strNewMesages = ChatSupportManager.Options.newMessagesFormat.replace("{1}", this.Name),
        winUrl = ChatSupportManager.Options.popupWindowUrl.replace("{RoomIDParam}", this.RoomID),
        that = this;

    
    this.GetText = function() {
        if (that.MessageCount == 0) {
            return that.Name;
        } 
        else {
            return strNewMesages.replace("{0}", this.MessageCount);
        }
    };


    this.SetUnreadMessagesCount = function(unreadMessagesCount) {
        that.MessageCount = unreadMessagesCount;
    }


    this.Popup = function() {
        // http://stackoverflow.com/a/2455480/637677
        // Open window with empty address, so it won't be reloaded if it already exists (is open in another tab for example)
        var win = window.open('', that.RoomID, "width=600,height=800,location=0,scrollbars=1,resizable=1");
        if (win != null){
            // If location is not correct url, window was not opened before and address should be set to the correct one
            if (win.location != winUrl) {
                win.location = winUrl;
            }
            that.Window = win;
            win.focus();
            testWindow(win, 600, 800);
        }
        else {
            alert(window.top.ChatSupportManager.Options.popupWindowError);
        }
    }


    this.Take = function(callback) {
        // Always call service and take room, so taken date time will be updated on the server side and correct number of unread messages will be send to all windows
        ChatSupportManager.TakeSupportRoom(that, callback);
    }


    function testWindow(win, width, height) {
	    if (win == null || typeof (win) == 'undefined') {
	        alert(window.top.ChatSupportManager.Options.popupWindowError);
	    }
	    else {
	        win.focus();
	        if (jQuery.browser.opera || jQuery.browser.webkit) {
	            setTimeout(function () {
	                if (!win.closed && (win.innerHeight != height) && (win.innerWidth != width)) {
	                    alert(window.top.ChatSupportManager.Options.popupWindowError);
	                }
	            }, 1000);
	        }
	    }
	}
}


function ChatSupportManagerObj(opt) {

    var options = {
        controlID: "",
        tick : 5000,
        contextMenuID : "",
        drpNewMessageWarningID : "",
        lblErrorID : "",
        pnlNotificationBubbleID : "",
        btnHideBubbleID : "",
        lblBubbleTitleID : "",
        youAreAvailableText : "",
        youAreNotAvailableText : "",
        selectRoomToOpen : "",
        newMessagesFormat : "",
        newSupportRequestsSingular : "",
        newSupportRequestsPlural : "",
        noNewSupportRequests: "",
        imgOnlineSrc : "",
        imgOfflineSrc : "",
        popupWindowUrl :"",
        settingsUrl : "",
        imgStatus: "",
        pnlErroID: "",
        strCloseError: "",
        newMessagesTitle: "",
        disableAutoLogin: false
    };

    jQuery.extend(options, opt);
    this.Options = options;

    // Control's objects
    var drpNewRequests = jQuery("#" + options.drpNewMessageWarningID),
        lblError = jQuery("#" + options.lblErrorID),
        pnlError = jQuery("#" + options.pnlErrorID),
        pnlNotificationBubble = jQuery("#" + options.pnlNotificationBubbleID),
        btnBubbHideBubble = jQuery("#" + options.btnHideBubbleID),
        lblBubbleTitle = jQuery("#" + options.lblBubbleTitleID),
        imgChatStatus = jQuery("#" + options.imgStatus),

        // Context menu's objects
        btnLogin = jQuery("#" + options.contextMenuID + " .ChatPnlLogin"),
        btnSettings = jQuery("#" + options.contextMenuID + " .ChatPnlSettings"),
        btnLogout = jQuery("#" + options.contextMenuID + " .ChatPnlLogout"),
        btnShowBubble = jQuery("#" + options.contextMenuID + " .ChatPnlShowBubble"),

        windowNameOnline = "chat_support_online",

        lastChangeTimestamp = null,
        onlineSupportID = null,

        pingTimeout = null,
        checkClosedWindowsTimeout = null,
        tabBlinkTimeout = null,
        tabTittle = "",

        // Value of the first option in drop down list
        defaultRequestOptionValue = -1,

        // "associative array" of rooms
        rooms = {},
        roomsUserId = {},
        roomsContactId = {},

        // Indicates if supporter is online
        isOnline = false,

        // Support service proxy
        proxy = null;

    if ((typeof (window.chat) !== 'undefined') && (chat != null) && (chat.IChatSupportService != null)) {
        proxy = chat.IChatSupportService;
    }
    else {
        jQuery("#" + options.controlID).hide();
    }


    this.InitiateChat = function (userID, contactID){
        if (userID <= 0 && contactID <= 0){
            return;
        }

        var isContact = false;

        // If userID is not set, use contactID and mark isContact as true (otherwise keep userID)
        if (userID <= 0) {
            userID = contactID;
            isContact = true;
        }

        if (!isOnline){
            EnterSupport(function() { 
                InitiateChatInternal(userID, isContact); 
            });
        }
        else {
            var room = null;
            if (isContact){
                room = roomsContactId[userID];
            }
            else {
                room = roomsUserId[userID];
            }

            if (room != null){
                room.Take();
            }
            else {
                InitiateChatInternal(userID, isContact);
            }
        }
    }


    this.TakeSupportRoom = function(roomObj, callback) {
        CallSupportWebService("SupportTakeRoom", function() {
            roomObj.MessageCount = 0;
            roomObj.Taken = true;
            if (callback) {
                callback();
            }
            roomObj.Popup();
        }, Error, [roomObj.RoomID]);
    }


    function InitiateChatInternal(userContactID, isContact){
        CallSupportWebService(isContact ? "InitiateChatByContactID" : "InitiateChatByUserID", function(roomResponse){
            var contactID = isContact ? userContactID : -1;
            var userID = !isContact ? userContactID : -1;

            // Personal rooms are not currently used - passing always false (5th parameter)
            var room = AddRoom(roomResponse.ChatRoomID, roomResponse.DisplayName, roomResponse.UnreadMessagesCount, true, false, userID, contactID);
            if (isContact){
                roomsContactId[contactID] = room;
            }
            else {
                roomsUserId[userID] = room;
            }
            room.Popup();
            DisplayControls();
        }, Error, [userContactID]);
    }

    
    function LeaveSupportRoom(roomID) {
        CallSupportWebService("SupportLeaveRoom", function() {
             delete rooms[roomID];
             DisplayControls(true);
        }, function(response){ 
            Error(response);
        }, 
        [roomID]);
    }    


    // Gets numer of rooms in memory    
    function CountRooms(notReadOnly){
        var count = 0;
        for (var roomID in rooms) {
            if (rooms.hasOwnProperty(roomID)) {
                if (notReadOnly && (rooms[roomID].MessageCount == 0)){
                    continue
                }
                count++;
            }
        }
        return count;
    }   


    function SetOnline(newValue) {
        isOnline = newValue;

        if (isOnline) {
            btnLogin.hide();
            btnLogout.show();
            imgChatStatus.attr({
                src: options.imgOnlineSrc,
                title: options.youAreAvailableText,
            });
        } 
        else {
            btnLogin.show();
            btnLogout.hide();
            imgChatStatus.attr({
                src: options.imgOfflineSrc,
                title: options.youAreNotAvailableText,
            });
        }
    }


    function AddRoom(id, name, messageCount, taken, personal, userID, contactID) {
        var room = new SupportRoomObj({
            id: id,
            messageCount: messageCount,
            name: name,
            taken: taken,
            personal: personal,
            userID: userID,
            contactID: contactID
        });
        rooms[room.RoomID] = room;
        return room;
    };


    function CheckClosedWindows() {
        var deletedAnyRoom = false;
        jQuery.each(rooms, function(index, room) {
            if (room.Window && room.Window.closed) {
                LeaveSupportRoom(room.RoomID);
                delete roomsContactId[room.ContactID];
                delete roomsUserId[room.UserID];
                delete rooms[room.RoomID];
                deletedAnyRoom = true;
            }
        });
        checkClosedWindowsTimeout = setTimeout(CheckClosedWindows, 500);
        if (deletedAnyRoom == true) {
            DisplayControls();
        }
    }


    function StartCheckingForClosedWindows() {
        checkClosedWindowsTimeout = setTimeout(CheckClosedWindows, 500);
    }


    function StopCheckingForClosedWindows() {
        clearTimeout(checkClosedWindowsTimeout);
    }


    function ClearPingTimeout() {
        if (pingTimeout != null) {
            clearTimeout(pingTimeout);
        }
    }


    function ResetPingTimeout() {
        ClearPingTimeout();
        pingTimeout = setTimeout(PingChatService, options.tick);
    }


    // Local functions definitions    
    function DisplayControls(removedOnly, popupBubble) {
        if (pnlError.css("display") != "none") {
            pnlNotificationBubble.hide();
            btnShowBubble.hide();
            return;
        }
        var roomsCount = CountRooms();

        if (roomsCount < 1) {
            pnlNotificationBubble.hide();
            btnShowBubble.hide();
            drpNewRequests.children().remove();
            
            // Stop blinking tab
            TabBlinkStarter(false);
        } 
        else {
            if (!removedOnly && popupBubble){
                pnlNotificationBubble.show();
                btnShowBubble.hide();
            }

            drpNewRequests.children().remove()
            drpNewRequests.append(jQuery("<option></option>").text(options.selectRoomToOpen).val(defaultRequestOptionValue));

            jQuery.each(rooms, function(index, room) {
                var option = jQuery("<option></option>").val(room.RoomID).text(room.GetText());
                if (room.Personal){
                    option.css("background-color", "green");
                }
                else if (room.Taken) {
                    option.css("background-color", "yellow");
                }
                drpNewRequests.append(option);
            });

            drpNewRequests.show();

            var newRooms = CountRooms(true);
            if (newRooms == 0){
                lblBubbleTitle.text(options.noNewSupportRequests);
                pnlNotificationBubble.removeClass("Active");
                
                // Stop blinking tab
                TabBlinkStarter(false);
            } else {
                pnlNotificationBubble.addClass("Active");
                if (newRooms == 1) {
                    lblBubbleTitle.text(options.newSupportRequestsSingular);
                } else {
                    lblBubbleTitle.text(options.newSupportRequestsPlural.replace("{0}", newRooms));
                }

                TabBlinkStarter(true);
            }
        }
    }


    function PingChatService() {
        CallSupportWebService("SupportPing", ProcessUpdates, function(response) {
            ResetPingTimeout();
            Error(response);
        }, [lastChangeTimestamp]);
    }


    function QuitSupport() {
        if (window.name == windowNameOnline){
            window.name = "";
        }
        ClearPingTimeout();
        lastChangeTimestamp = null;
        onlineSupportID = null;

        SetOnline(false);

        rooms = {};
        DisplayControls();
        StopCheckingForClosedWindows();
    }


    function EnterSupport(callback){
        CallSupportWebService("EnterSupport", function (enterSupportData) {
            if (window.name.length == 0){
                window.name = windowNameOnline;
            }
            ProcessEnterSupport();
            if (callback != null){
                callback();
            }
        }, Error, []);
    }


    function ProcessEnterSupport(){
        SetOnline(true);
        lastChangeTimestamp = null;
        PingChatService();
        StartCheckingForClosedWindows();    
    }


    function ProcessUpdates(response) {
        var supporterID = response.OnlineSupportChatUserID;
        if (supporterID == null){
            QuitSupport();
            return;
        }
        else {
            if (onlineSupportID == null){
                onlineSupportID = supporterID;
            }
            else if (supporterID != onlineSupportID){
                QuitSupport();
                return;
            }
        }
        
        if (response.Rooms != null){
            ProcessRooms(response.Rooms);
        }

        ResetPingTimeout();
    }


    function ProcessRooms(supportRooms){
        if (supportRooms.LastChange) {
            lastChangeTimestamp = supportRooms.LastChange;
        }

        var roomsList = supportRooms.List;
        var isNew = false, isRemoved = false, popupBubble = false;

        if ((roomsList != null) && (roomsList.length > 0)) {
            for (var i = 0; i < roomsList.length; i++) {
                var receivedRoom = roomsList[i];
                var room = rooms[receivedRoom.ChatRoomID];

                if (receivedRoom.IsRemoved){
                    if (room != null){
                        delete rooms[receivedRoom.ChatRoomID];
                        isRemoved = true;
                    }
                    continue;
                }

                if (room == null) {
                    // Personal rooms are not currently used - passing always false (5th parameter)
                    var roomObj = AddRoom(receivedRoom.ChatRoomID, receivedRoom.DisplayName, receivedRoom.UnreadMessagesCount, receivedRoom.IsTaken, false); 
                    popupBubble = true;
                } else {
                    room.SetUnreadMessagesCount(receivedRoom.UnreadMessagesCount);
                    room.Taken = receivedRoom.IsTaken;
                    if (receivedRoom.UnreadMessagesCount > 0){
                        popupBubble = true;
                    }
                }
                
            }
            isNew = true;
        }

        if (isNew || isRemoved){
            DisplayControls(!isNew, popupBubble);
        }
    }


    function Error(err) {
        drpNewRequests.hide();
        pnlNotificationBubble.hide();
        pnlError.show();
        btnShowBubble.hide();

        var message;
        if (err.StatusMessage) {
            message = err.StatusMessage;
        } else if (err.get_message) {
            message = err.get_message();
        } else {
            message = err;
        }
        lblError.text(message);
    }


    function TabBlink(){
        var title = window.top.document.title;
        if (title == options.newMessagesTitle){
            window.top.document.title = tabTittle;
        }
        else {
            tabTittle = title;
            window.top.document.title = options.newMessagesTitle;
        }
        tabBlinkTimeout = setTimeout(TabBlink, 2000);
    }


    function TabBlinkStarter(start){
        if (start && (tabBlinkTimeout == null)){
            TabBlink();
        }
        else if (tabBlinkTimeout){
            clearTimeout(tabBlinkTimeout);
            window.top.document.title = tabTittle;
            tabBlinkTimeout = null;
        }
    }


    // Call webservice function and process response
    function CallSupportWebService(fn, fnOk, fnErr, args) {
        if (proxy == null) {
            return;
        }

        if (!args) {
            args = [];
        }

        args.push(function(response) {
            // 0 is OK
            if (response.StatusCode == 0) {
                if (fnOk != null) {
                    if (response.Data) {
                        fnOk(response.Data);
                    } else {
                        fnOk();
                    }
                }
            } else {
                fnErr(response);
            }
        });
        args.push(fnErr);
        args.push(null);
        proxy[fn].apply(proxy, args);
    }


    function Inicialize() {
        // This function hides bubble and shows/hides appropriate buttons
        var hideBubbleFn = function () {
            pnlNotificationBubble.hide();
            btnShowBubble.show();
            TabBlinkStarter(false);
            return false;
        }

        btnBubbHideBubble.click(hideBubbleFn);

        btnShowBubble.click(function () {
            pnlNotificationBubble.show();
            btnShowBubble.hide();
            return false;
        });

        drpNewRequests.change(function() {
            var roomID = drpNewRequests.val();
            if ((roomID != defaultRequestOptionValue) && (rooms[roomID] != null)) {
                rooms[roomID].Take(DisplayControls);
            }
            drpNewRequests.val(defaultRequestOptionValue);
        });

        pnlError.click(function () {
            pnlError.hide();
            DisplayControls();
            return false;
        });
        
        lblError.attr("title", options.strCloseError);

        // Support login button method
        btnLogin.click(function () {
            EnterSupport();
            return false;
        });

        // Support logout button method
        btnLogout.click(function () {
            CallSupportWebService("LeaveSupport", QuitSupport, Error, []);
            return false;
        });

        // Support setting methods
        btnSettings.click(function () {
            modalDialog(options.settingsUrl, "SupportChatSettings", 600, 500);
            return false;
        });

        // Initialization
        pnlError.hide();
        
        if (window.name == windowNameOnline) {
            ProcessEnterSupport();
        }
        else {
            SetOnline(false);
        }

        DisplayControls();
    }

    Inicialize();
}
 

function InitChatSupportManager(opt){
    window.top.ChatSupportManager = new ChatSupportManagerObj(opt);
}