function ChatLogin(opt) {
    var defaults = {
        clientID: -1,
        pnlChatUserLoggedIn: "",
        pnlChatUserChangeNicknameForm: "",
        pnlChatUserLoginError: "",
        pnlChatUserLoginRelog: "",
        txtChatUserLoginRelogNickname: "",
        lblChatUserLoginRelogNickname: "",
        btnChatUserLoginRelog: "",
        lblChatUserLoggedInInfoValue: "",
        btnChatUserChangeNicknameButton: "",
        txtChatUserChangeNicknameInput: "",
        lblChatUserLoginErrorText: "",
        btnLogout: "",
        resStrLogout: "",
        redirectURLLogout:"", 
        redirectURLEnter:"",
        lblChatUserLoginRelogText: "", 
        resStrLogAsAnonym: "", 
        resStrLogAsCMS: "",
        resStrNoService: ""
    };
    var that = this;

    this.Type = "ChatLogin";
    this.Options = defaults;
    jQuery.extend(that.Options, opt);
    
    // Convert appropriate parameters to DOM objects
    var clientID = that.Options.clientID, 
        
        pnlLoggedIn = jQuery("#" + that.Options.pnlChatUserLoggedIn),
        pnlChangeNicknameForm = jQuery("#" + that.Options.pnlChatUserChangeNicknameForm),
        pnlError = jQuery("#" + that.Options.pnlChatUserLoginError),
        pnlLoginRelog = jQuery("#" + that.Options.pnlChatUserLoginRelog),

        lblLoginRelogText = jQuery("#" + that.Options.lblChatUserLoginRelogText),
        txtLoginRelogNickname = jQuery("#" + that.Options.txtChatUserLoginRelogNickname),
        lblLoginRelogNickname = jQuery("#" + that.Options.lblChatUserLoginRelogNickname),
        lblLoggedInInfo = jQuery("#" + that.Options.lblChatUserLoggedInInfoValue),

        txtChangeNickname = jQuery("#" + that.Options.txtChatUserChangeNicknameInput),
        btnChangeNickname = jQuery("#" + that.Options.btnChatUserChangeNicknameButton),

        lblError = jQuery("#" + that.Options.lblChatUserLoginErrorText),

        btnLogout = jQuery("#" + that.Options.btnLogout),

        loginFunction = function() {
            var fnok = null;
            if (that.Options.redirectURLEnter.length > 0) {
                fnok = function() { self.window.location = that.Options.redirectURLEnter; };
            }
            ChatManager.Login.CheckState(clientID, txtLoginRelogNickname.val(), fnok);
            return false;
        },

        logoutFunction = function () {
            var fnok = null;
            if (that.Options.redirectURLLogout.length > 0) {
                fnok = function () { self.window.location = that.Options.redirectURLLogout; };
            }
            ChatManager.Login.Logout(fnok);
            return false;
        };

    Inicialize(opt);
    

    // Displays login info form
    this.DisplayInfo = function () {

        // User is logged in
        if (ChatManager.Login.IsLoggedIn) {
            pnlChangeNicknameForm.hide();
            pnlLoginRelog.hide();
            lblLoggedInInfo.text(ChatManager.Login.Nickname + " ");
            btnLogout.val(that.Options.resStrLogout);
            pnlLoggedIn.show();
        }

        // User not logged in
        else {
            pnlLoggedIn.hide();
            pnlChangeNicknameForm.hide();

            if ((ChatManager.Settings.AnonymsAllowedGlobally == false) && ChatManager.Login.IsAnonymous) {
                pnlLoginRelog.hide();
                that.Error({ StatusMessage: ChatManager.Settings.AnonymsNotAllowedGlobalyMsg });
                return;
            }

            that.DisplayLoginUserForm();
            pnlLoginRelog.show();
        }
        that.ErrorClose();
    }


    // Displays login form
    this.DisplayLoginUserForm = function() {
        
        // User is anonymous
        if (ChatManager.Login.Nickname.length == 0) {
            lblLoginRelogNickname.hide();
            txtLoginRelogNickname.show();
            lblLoginRelogText.text(that.Options.resStrLogAsAnonym);
        }
        else {
            lblLoginRelogNickname.text(jQuery.ChatManager.Login.Nickname);
            txtLoginRelogNickname.hide();
            lblLoginRelogNickname.show();
            lblLoginRelogText.text(that.Options.resStrLogAsCMS);
        }
        pnlLoginRelog.show();
    }
    

    // Displays change nickname form
    this.DisplayChangeNicknameForm = function() {
        txtChangeNickname.val(ChatManager.Login.Nickname);
        pnlLoggedIn.hide();
        pnlLoginRelog.hide();
        pnlChangeNicknameForm.show();
    }
    

    // Displays error mesage
    this.Error = function(status) {
        lblError.html(status.StatusMessage);
        pnlError.show();
    }
    

    // Closes error message
    this.ErrorClose = function() {
        pnlError.hide();
    }


    this.HideContent = function () {
        pnlChangeNicknameForm.hide();
        pnlLoggedIn.hide();
        pnlLoginRelog.hide();
        that.Error({StatusMessage: that.Options.resStrNoService});
    }


    function Inicialize(opt) {

        // Change nickname when enter key pressed
        txtChangeNickname.bind("keydown", function(evt) {
            var e = window.event || evt;
            var key = e.keyCode;

            if (key == 13) {
                if (e.preventDefault) e.preventDefault();
                if (e.stopPropagation) e.stopPropagation();
                e.returnValue = false;

                ChatManager.Login.ChangeNickname(txtChangeNickname.val(), clientID);
                return false;
            }
        });

        jQuery("#" + that.Options.btnChatUserLoginRelog).click(loginFunction);
        jQuery("#" + that.Options.btnLogout).click(logoutFunction);

        // Login user when enter key pressed
        txtLoginRelogNickname.bind("keydown", function(evt) {
            var e = window.event || evt;
            var key = e.keyCode;

            if (key == 13) {
                if (e.preventDefault) e.preventDefault();
                if (e.stopPropagation) e.stopPropagation();
                e.returnValue = false;

                loginFunction();
                return false;
            }
        });
    }
    
    that.DisplayInfo();
};


// Inits chat login web part functionality
function InitChatLogin(opt) {
    InicializeChatManager();
    var logWp = new ChatLogin(opt);
    ChatManager.RegisterWebpart(logWp);
    if (ChatManager.WebService == null) {
        logWp.HideContent();
    }
}

