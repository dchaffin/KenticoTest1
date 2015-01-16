// ChatRooms webpart  object
function ChatRooms(opt) {

    // Variables
    var defaults = {
        templateListItem: "",
        groupID: -1,
        clientID: -1,
        pnlChatRoomsList: "",
        btnChatRoomsPromptSubmit: "",
        pnlChatRoomsPrompt: "",
        txtChatRoomsPromptInput: "",
        pnlChatRoomsCreatePrompt: "",
        txtChatRoomsCreateName: "",
        chkChatRoomsCreateIsPrivate: "",
        txtChatRoomsCreatePassword: "",
        chkChatRoomsCreateAllowAnonym: "",
        btnChatRoomsCreatePromptSubmit: "",
        btnChatRoomsCreateRoom: "",
        lblChatRoomsCreateError: "",
        loadingDiv: "",
        txtChatRoomsCreateDescription: "",
        btnChatRoomsPromptClose: "",
        btnChatRoomsCreatePromptClose: "",
        btnChatRoomsDeletePromptSubmit: "",
        btnChatRoomsDeletePromptClose: "",
        pnlChatRoomsDeletePrompt: "",
        btnChatRoomsAbandonPromptSubmit: "",
        btnChatRoomsAbandonPromptClose: "",
        pnlChatRoomsAbandonPrompt: "",
        pnlChatRoomsEditPrompt: "",
        txtChatRoomsEditName: "",
        txtChatRoomsEditDescription: "",
        chkChatRoomsEditIsPrivate: "",
        txtChatRoomsEditPassword: "",
        chkChatRoomsEditAllowAnonym: "",
        btnChatRoomsEditPromptSubmit: "",
        btnChatRoomsEditPromptClose: "",
        txtChatRoomsEditPasswordConfirm: "",
        lblChatRoomsEditError: "",
        passwordTxt: "",
        passwordNewTxt: "",
        lblChatRoomsEditPasswordNew: "",
        txtChatRoomsCreatePasswordConfirm: "",
        pnlChatRoomsEditError: "",
        chkChatRoomsHasPassword: "",
        pnlChatRoomsCreateError: "",
        pagingItems: -1,
        pagingEnabled: false,
        pnlFilterClientID: "",
        btnFilter: "",
        txtFilter: "",
        filterEnabled: false,
        pnlPagingClientID: "",
        pnlInfo: "",
        resStrNoFound: "",
        resStrResults: "",
        filterCount: -1,
        passwordPromptError: "",
        redirectURL: "",
        redirectGroup: "",
        envelopeID: "",
        pnlErrorCreate: "",
        pnlErrorEdit: "",
        enablePopup: true,
        guid: "",
        passwordNotMatchStr: ""
    };

    this.Type = "ChatRooms";
    this.GroupManager = null;
    this.Options = jQuery.extend(defaults, opt);
    this.RoomToDelete = -1;
    this.RoomToEdit = -1;
    this.RoomToAbandon = -1;

    var that = this,
        content = jQuery("#" + that.Options.pnlChatRoomsList),
        envelope = jQuery("#" + that.Options.envelopeID),
        loadingBackup = "",
        templateID = "ChatRoomsTemplateListItem" + that.Options.clientID,
        
        RoomPasswordTextbox = jQuery("#" + that.Options.txtChatRoomsPromptInput),
        RoomPasswordError = jQuery("#" + that.Options.passwordPromptError),
        RoomPasswordPrompt = jQuery("#" + that.Options.pnlChatRoomsPrompt),

        btnCreateRoom = jQuery("#" + that.Options.btnChatRoomsCreateRoom),
        
        CreateRoomControls = {
            NameTxt: jQuery("#" + that.Options.txtChatRoomsCreateName),
            IsPrivateChk: jQuery("#" + that.Options.chkChatRoomsCreateIsPrivate),
            PasswordTxt: jQuery("#" + that.Options.txtChatRoomsCreatePassword),
            PasswordConfirmTxt: jQuery("#" + that.Options.txtChatRoomsCreatePasswordConfirm),
            AllowAnonymChk: jQuery("#" + that.Options.chkChatRoomsCreateAllowAnonym),
            DescriptionTxt: jQuery("#" + that.Options.txtChatRoomsCreateDescription),
            ErrorPnl: jQuery("#" + that.Options.pnlChatRoomsCreateError),
            ErrorLbl: jQuery("#" + that.Options.lblChatRoomsCreateError),
            PromptPnl: jQuery("#" + that.Options.pnlChatRoomsCreatePrompt),
            btnSubmit: jQuery("#" + that.Options.btnChatRoomsCreatePromptSubmit),
            btnClose: jQuery("#" + that.Options.btnChatRoomsCreatePromptClose)
        },
        
        EditRoomControls = {
            NameTxt: jQuery("#" + that.Options.txtChatRoomsEditName),
            IsPrivateChk: jQuery("#" + that.Options.chkChatRoomsEditIsPrivate),
            HasPassword: jQuery("#" + that.Options.chkChatRoomsHasPassword),
            PasswordTxt: jQuery("#" + that.Options.txtChatRoomsEditPassword),
            PasswordConfirmTxt: jQuery("#" + that.Options.txtChatRoomsEditPasswordConfirm),
            AllowAnonymChk: jQuery("#" + that.Options.chkChatRoomsEditAllowAnonym),
            DescriptionTxt: jQuery("#" + that.Options.txtChatRoomsEditDescription),
            ErrorPnl: jQuery("#" + that.Options.pnlChatRoomsEditError),
            ErrorLbl: jQuery("#" + that.Options.lblChatRoomsEditError),
            PromptPnl: jQuery("#" + that.Options.pnlChatRoomsEditPrompt),
            btnSubmit: jQuery("#" + that.Options.btnChatRoomsEditPromptSubmit),
            btnClose: jQuery("#" + that.Options.btnChatRoomsEditPromptClose),
            lblPassword: jQuery("#" + that.Options.lblChatRoomsEditPasswordNew)
        },

        PasswordPrompt = jQuery("#" + that.Options.pnlChatRoomsPrompt),
        PasswordInput = jQuery("#" + that.Options.txtChatRoomsPromptInput),
        DeleteRoomPrompt = jQuery("#" + that.Options.pnlChatRoomsDeletePrompt),
        AbandonRoomPrompt = jQuery("#" + that.Options.pnlChatRoomsAbandonPrompt)
        ;


    this.Clear = function() {
        content.empty();
        envelope.hide();
        that.ListPaging.Clear();
    }


    this.ShowLoading = function(show) {
        if (show == true) {
            loadingBackup = content.html();
            content.html(that.Options.loadingDiv);
            envelope.show();
        }
        else {
            content.html(loadingBackup);
        }
    };


    // Process response
    this.ShowRooms = function(rooms) {
        envelope.show();
        that.ListPaging.Render(rooms);
    };


    this.OpenDeleteRoomPrompt = function(roomID) {
        that.RoomToDelete = roomID;
        ChatManager.DialogsHelper.DisplayDialog(DeleteRoomPrompt);   
    };


    this.OpenAbandonRoomPrompt = function(roomID) {
        that.RoomToAbandon = roomID;
        ChatManager.DialogsHelper.DisplayDialog(AbandonRoomPrompt);   
    };


    this.OpenEditRoomPrompt = function(roomID) {
        var roomObj = ChatManager.ChatRoomsList[roomID];
        if (roomObj == null) {
            return;
        }

        that.RoomToEdit = roomID;

        // Set the values
        if (roomObj.HasPassword) {
            EditRoomControls.lblPassword.text(that.Options.passwordNewTxt);
            EditRoomControls.HasPassword.attr("checked", "checked");
        }
        else {
            EditRoomControls.lblPassword.text(that.Options.passwordTxt);
            EditRoomControls.HasPassword.removeAttr("checked");
        }

        EditRoomControls.NameTxt.val(roomObj.DisplayName);
        EditRoomControls.DescriptionTxt.val(roomObj.Description);

        if (roomObj.IsPrivate) {
            EditRoomControls.IsPrivateChk.attr("checked", "checked");
        }
        else {
            EditRoomControls.IsPrivateChk.removeAttr("checked");
        }

        if (roomObj.AllowAnonym) {
            EditRoomControls.AllowAnonymChk.attr("checked", "checked");
        }
        else {
            EditRoomControls.AllowAnonymChk.removeAttr("checked");
        }

        EditRoomControls.PasswordTxt.val("");
        EditRoomControls.PasswordConfirmTxt.val("");

        EditRoomControls.ErrorPnl.hide();
        EditRoomControls.ErrorLbl.empty();

        ChatManager.DialogsHelper.DisplayDialog(EditRoomControls.PromptPnl);
    };


    function RenderRooms(rooms) {
        content.empty();

        // Add links (onclick event handlers) to all items in list
        for (var i = 0; i < rooms.length; i = i + 1) {
            var room = rooms[i];
            var redirectLocation = "";
            if (that.Options.redirectURL.length > 0) {
                redirectLocation = that.Options.redirectURL + "?roomid=" + room.ChatRoomID;
                if (that.Options.redirectGroup.length > 0) {
                    redirectLocation += "&groupname=" + that.Options.redirectGroup;
                }
            }

            // Check user is in this room
            room.IsCurrentRoom = (room.ChatRoomID == that.GroupManager.RoomID) ? true : false;
            room.AllowAnonymous = room.AllowAnonym;
            var iconsCount = 0;

            // Check if the room is private -> if true add option to abandon room
            if (room.IsPrivate) {
                room.Abandon = "ChatManager.OpenAbandonRoomPrompt(\"" + that.Options.groupID + "\", '" + that.Options.clientID + "', " + room.ChatRoomID + "); return false;";
                iconsCount++;
            }
            else {
                delete room.Abandon;
            }

            // Check if user has permissions to edit and delete rooms
            if (ChatManager.Login.UserPermissions.ManageRooms || room.CanManage) {
                room.Delete = "ChatManager.OpenDeleteRoomPrompt(\"" + that.Options.groupID + "\", '" + that.Options.clientID + "', " + room.ChatRoomID + "); return false;";
                room.Edit = "ChatManager.OpenEditRoomPrompt(\"" + that.Options.groupID + "\", '" + that.Options.clientID + "', " + room.ChatRoomID + "); return false;";
                iconsCount += 2;
            }
            else {
                delete room.Delete;
                delete room.Edit;
            }

            var roomTemplate = jQuery.tmpl(templateID, room);

            var roomLink = roomTemplate.find("a.JoinRoom");
            roomLink.mousedown(generateFunction(room.ChatRoomID, room.HasPassword, redirectLocation));

            var roomTitle = roomTemplate.find("div.ChatRoomTitle");
            roomTitle.css("margin-right", (16 * iconsCount) + "px");

            if (redirectLocation.length > 0) {
                roomLink.attr("href", redirectLocation);
            }

            if (that.Options.enablePopup && !jQuery.browser.opera) {
                roomLink.attr("title", that.Options.resStrTooltipPopup);
            }



            roomTemplate.appendTo(content);
        }

        // Check user permitions for creating rooms
        if (ChatManager.Login.UserPermissions.ManageRooms || ChatManager.Login.UserPermissions.CreateRooms) {
            btnCreateRoom.show();
        }
        else {
            btnCreateRoom.hide();
        }
    };


    function generateFunction(id, hasPassword, redirectLocation) {
        var redirectFn = null;
        if (redirectLocation.length > 0) {
            redirectFn = function () {
                self.window.location = redirectLocation;
            }
        }
        var joinFn = null;
        if (hasPassword == true) {
            joinFn = function () {
                OpenPasswordPrompt(id);
            };
        }
        else {
            joinFn = function () {
                ChatManager.JoinRoom(that.Options.groupID, id, null, redirectFn);
                return false;
            };
        }
        return function (event) {
            switch (event.which) {
                // Left mouse button                  
                case 1:
                    joinFn();
                    break;
                // Midle mouse button  
                case 2:
                    if ((that.Options.enablePopup == true) && (redirectLocation.length == 0)) {
                        if (confirm(ChatManager.Settings.PopupWindowConfirmation) == true) {
                            ChatManager.OpenChatWindow({ roomID: id, url: ChatManager.Settings.PopupWindowURL, GUID: that.Options.guid });
                        }
                        else {
                            break;
                        }
                    }
                    else if ((redirectLocation.length > 0) && hasPassword) {
                        joinFn();
                    }
                    break;
            }
            return false;
        };
    }


    function OpenPasswordPrompt(id) {
        ChatManager.ChatRoomsClickedRoom = id;
        ChatManager.ChatRoomsGroupID = that.Options.groupID;
        RoomPasswordTextbox.val("");
        RoomPasswordError.empty();
        ChatManager.DialogsHelper.DisplayDialog(RoomPasswordPrompt);
        RoomPasswordTextbox.focus();
    }


    function JoinRoomWithPassword(psw) {
        ChatManager.JoinRoom(ChatManager.ChatRoomsGroupID, ChatManager.ChatRoomsClickedRoom, psw, function() {
            if (that.Options.redirectURL.length > 0) {
                var redirectLocation = that.Options.redirectURL + "?roomid=" + ChatManager.ChatRoomsClickedRoom;
                if (that.Options.redirectGroup.length > 0) {
                    redirectLocation += "&groupname=" + that.Options.redirectGroup;
                }
                self.window.location = redirectLocation;
            }
            ChatManager.DialogsHelper.CloseDialog(RoomPasswordPrompt);
        }, function(err) {
            RoomPasswordError.html(err);
        });
    }


    function InicializeCreateEditRoomPrompt(edit) {

        var controls = edit ? EditRoomControls : CreateRoomControls;

        // Define overlay 
        ChatManager.DialogsHelper.SetDialogOverlay(controls.PromptPnl);
        ChatManager.DialogsHelper.HandleTextAreaMaxLength(controls.DescriptionTxt, 500);

        if (!edit) {
            // Set event handling for opening create room prompt - clear dialog
            btnCreateRoom.click(function() {
                controls.ErrorPnl.hide();
                controls.ErrorLbl.empty();
                controls.NameTxt.val("");
                controls.DescriptionTxt.val("");
                controls.AllowAnonymChk.attr("checked", true);
                controls.IsPrivateChk.removeAttr("checked");
                controls.PasswordTxt.val("");
                controls.PasswordConfirmTxt.val("");
                ChatManager.DialogsHelper.DisplayDialog(controls.PromptPnl);
                return false;
            });
        }

        // Set event handling for submit button
        controls.btnSubmit.click(function() {
            // Get user input
            var name = controls.NameTxt.val().trim();
            var isPrivate = (controls.IsPrivateChk.attr("checked") == "checked") ? true : false;
            var password = controls.PasswordTxt.val();
            var passwordConfirm = controls.PasswordConfirmTxt.val();
            var allowAnonym = (controls.AllowAnonymChk.attr("checked") == "checked") ? true : false;
            var description = controls.DescriptionTxt.val();
            var hasPassword = (!edit || controls.HasPassword.attr("checked") == "checked") ? true : false; ;

            if (hasPassword && (password != passwordConfirm)) {
                controls.ErrorLbl.text(that.Options.passwordNotMatchStr);
                controls.ErrorPnl.show();
                return false;
            }

            var fnok = function() {
                if (edit) {
                    that.RoomToEdit = -1;
                }
                ChatManager.DialogsHelper.CloseDialog(controls.PromptPnl);
            }

            var fnerr = function(err) {
                controls.ErrorLbl.html(err);
                controls.ErrorPnl.show();
            }

            if (edit) {
                ChatManager.EditRoom(that.RoomToEdit, name, isPrivate, hasPassword, password, allowAnonym, description, fnok, fnerr);
            }
            else {
                ChatManager.CreateRoom(name, isPrivate, password, allowAnonym, description, fnok, fnerr);
            }
            return false;
        });

        // Set event handling for closing the prompt
        controls.btnClose.click(function() {
            ChatManager.DialogsHelper.CloseDialog(controls.PromptPnl);
            if (edit) {
                that.RoomToEdit = -1;
            }
            return false;
        }); 
    }



    function InicializePasswordPrompt() {

        // Define overlay for entering password
        ChatManager.DialogsHelper.SetDialogOverlay(PasswordPrompt);

        // Set event handling for entering password
        jQuery("#" + that.Options.btnChatRoomsPromptSubmit).click(function() {
            JoinRoomWithPassword(PasswordInput.val());
            return false;
        });

        // Set event handling for closing password prompt
        jQuery("#" + that.Options.btnChatRoomsPromptClose).click(function() {
            ChatManager.DialogsHelper.CloseDialog(PasswordPrompt);
            return false;
        });

        // Bind event handle for entering password -> allow pressing ENTER
        PasswordInput.bind("keydown", function(evt) {
            var e = window.event || evt;
            var key = e.keyCode;

            if (key == 13) {
                if (e.preventDefault) e.preventDefault();
                if (e.stopPropagation) e.stopPropagation();
                e.returnValue = false;

                JoinRoomWithPassword(PasswordInput.val());
                return false;
            }
        });
    }


    function InicializeDeleteAbandonPrompts() {
    
        // Delete room prompt
        ChatManager.DialogsHelper.SetDialogOverlay(DeleteRoomPrompt);
       
        // Set event handling for deleting rooms
        jQuery("#" + that.Options.btnChatRoomsDeletePromptSubmit).click(function() {
            ChatManager.DialogsHelper.CloseDialog(DeleteRoomPrompt);
            ChatManager.DeleteRoom(that.RoomToDelete);
            that.RoomToDelete = -1;
            return false;
        });

        // Set event handling for closing delete room prompt
        jQuery("#" + that.Options.btnChatRoomsDeletePromptClose).click(function() {
            ChatManager.DialogsHelper.CloseDialog(DeleteRoomPrompt);
            that.RoomToDelete = -1;
            return false;
        });
        
        // Abandon room prompt
        ChatManager.DialogsHelper.SetDialogOverlay(AbandonRoomPrompt);

        // Set event handling for abandon rooms
        jQuery("#" + that.Options.btnChatRoomsAbandonPromptSubmit).click(function() {
            ChatManager.LeaveRoomPermanently(that.RoomToAbandon);
            that.RoomToAbandon = -1;
            ChatManager.DialogsHelper.CloseDialog(AbandonRoomPrompt);
            return false;
        });

        // Set event handling for closing abandon room prompt
        jQuery("#" + that.Options.btnChatRoomsAbandonPromptClose).click(function() {
            ChatManager.DialogsHelper.CloseDialog(AbandonRoomPrompt);
            that.RoomToAbandon = -1;
            return false;
        });
    }


    function Inicialize() {

        // Build jQuery templates
        jQuery.template(templateID, that.Options.templateListItem);

        // Inicialize create room prompt
        InicializeCreateEditRoomPrompt(false);

        // Inicialize edit room prompt
        InicializeCreateEditRoomPrompt(true);
        
        InicializeDeleteAbandonPrompts();
        InicializePasswordPrompt();
    };


    this.ListPaging = new ListPaging(
        {
            groupID: that.Options.groupID,
            clientID: that.Options.clientID,
            wpType: that.Type,
            functionRenderList: RenderRooms,
            filterPanelID: that.Options.pnlFilterClientID,
            filterPanelInfoID: that.Options.pnlInfo,
            filterNotFoundStr: that.Options.resStrNoFound,
            filterFoundStr: that.Options.resStrResults,
            filterEnabled: that.Options.filterEnabled,
            filterShowItems: that.Options.filterCount,
            filterTextboxID: that.Options.txtFilter,
            filterButtonID: that.Options.btnFilter,
            filterProperty: "DisplayName",
            pagingEnabled: that.Options.pagingEnabled,
            pagingItems: that.Options.pagingItems,
            pagingContentID: that.Options.pnlPagingClientID
        });


    Inicialize();
    that.Clear();
}


// Inits chat rooms web part functionality
function InitChatRooms(opt) {
    InicializeChatManager();
    ChatManager.RegisterWebpart(new ChatRooms(opt));
}