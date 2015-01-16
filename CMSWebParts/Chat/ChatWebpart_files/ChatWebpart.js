function ChatWebpartWP(headerClientID) {
    this.Type = "ChatWebpart";

    var header = jQuery("#" + headerClientID);

    this.Clear = function () {
        if (ChatManager.Login.IsLoggedIn) {
            header.show();
        }
        else {
            header.hide();
        }
    }

    this.Clear();
}


function InitChatWebpart(opt) {
    InicializeChatManager();
    ChatManager.RegisterWebpart(new ChatWebpartWP(opt));
}