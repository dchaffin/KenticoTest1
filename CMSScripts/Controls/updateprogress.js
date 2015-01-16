function UP_Init(id, time) {
    if (!window.noProgress) {
        var async = false;
        try {
            async = Sys.WebForms.PageRequestManager.getInstance()._postBackSettings.async;
        }
        catch (e) {
        }
        if (!async) {
            var height = 0;
            if (window.cmsfixpanelheight != null) {
                height = cmsfixpanelheight + 5;
            }
            setTimeout("var elm = document.getElementById('" + id + "');elm.style.display = 'inline';elm.style.top = " + height + " + 'px';", time);
        }
    }
    return true;
}