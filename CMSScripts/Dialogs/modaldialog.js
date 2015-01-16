var ctrlPressed = false;

function modalDialog(url, name, width, height, otherParams, noWopener, forceModal, forceNewWindow, setTitle) {
    // Set default parameter values
    if (setTitle == undefined) {
        setTitle = true;
    }
    if (forceModal == undefined) {
        forceModal = true;
    }
    if (otherParams == undefined) {
        otherParams = 'toolbar=no,directories=no,menubar=no,modal=yes,dependent=yes,resizable=yes';
    }

    var advanced = false;
    try {
        advanced = window.top.AdvancedModalDialogs;
    } catch (err) {
    }

    if (advanced && !forceNewWindow && !ctrlPressed) {
        window.top.advancedModal(url, name, width, height, otherParams, noWopener, forceModal, setTitle, this);
    }
    else {
        var win = window;
        var dHeight = height;
        var dWidth = width;
        if (width.toString().indexOf('%') != -1) {
            dWidth = Math.round(screen.width * parseInt(width, 10) / 100);
        }
        if (height.toString().indexOf('%') != -1) {
            dHeight = Math.round(screen.height * parseInt(height, 10) / 100);
        }

        var oWindow = win.open(url, name, 'width=' + dWidth + ',height=' + dHeight + ',' + otherParams);
        if (oWindow) {
            oWindow.opener = this;
            oWindow.focus();
        }

    }
    ctrlPressed = false;
}

// Detect CTRL
/* PROTOTYPE - requires jQuery */
/*
$j(window).keydown(function (evt) {
if (evt.which == 17) {
ctrlPressed = true;
}
}).keyup(function (evt) {
if (evt.which == 17) {
ctrlPressed = false;
}
});
*/