var screenLockWarningOn = false;
var screenLockWarningDialogOn = false;
var screenLockIsLocked = false;
var screenLockCounter = 0;
var screenLockIntervalCounter = null;
var screenLockTimeout = null;

var longPing = 300000;
var shortPing = 60000;

function getResultShort(result, context) {
    var resultType = ((result.indexOf('|') == -1) ? result : result.substr(0, result.indexOf('|')));

    switch (resultType) {
        case 'logout':
            parent.location.replace(result.substring(7));
            break;
        case 'valbad':
            $j('#screenLockDialogWarning').show();
            $j('#screenLockDialogWarningLogonAttempts').hide();
            $j('#screenLockDialogPassword').focus();
            break;
        case 'valok':
            hideModalPopup('screenLockDialog', 'screenLockBackground');
            $j('#screenLockDialogWarning').hide();
            screenLockIsLocked = false;
            CallServer(result.substring(6) * 1000);
            break;
        case 'isLocked':
            if (result.substring(9) == 'True') {
                if (!screenLockIsLocked) {
                    ShowScreenLockDialog();
                }
            }
            CallServer(longPing);
            break;
        case 'lockScreen':
            if (!screenLockIsLocked) {
                ShowScreenLockDialog();
            }
            CallServer(longPing);
            break;
        case 'hideWarning':
            hideModalPopup('screenLockDialog', 'screenLockBackground');
            screenLockIsLocked = false;
            if (screenLockWarningOn) {
                HideScreenLockWarning();
            }
            CallServer(result.substring(12) * 1000);
            break;
        case 'showWarning':
            hideModalPopup('screenLockDialog', 'screenLockBackground');
            screenLockIsLocked = false;
            screenLockCounter = result.substring(12);
            ShowScreenLockWarning();
            CallServer(shortPing);
            break;
        case 'cancelOk':
            HideScreenLockWarning();
            CallServer(result.substring(9) * 1000);
            break;
        case 'disabled':
            HideScreenLockWarning();      
            screenLockEnabled = false;
            clearTimeout(screenLockTimeout);
            break;
        case 'accountLocked':
            $j('#screenLockDialogWarning').hide();
            $j('#screenLockDialogWarningLogonAttempts').show();
            $j('#screenLockSignInButton').show();
            $j('#screenLockUnlockButton').hide();
            $j('#screenLockSignOutButton').hide();
            break;
    }
}

function ScreenLockLogoutUser() {
    serverRequest('logout');
}

function ScreenLockValidateUser() {
    serverRequest('validate|' + $j('#screenLockDialogPassword').val());
}

function ScreenLockRedirectToLogon(logonpage) {
    parent.location.replace(logonpage);
}

function HideScreenLockWarning() {
    clearInterval(screenLockIntervalCounter);
    screenLockIntervalCounter = null;

    $j('#screenLockWarning').hide();
    $j('#screenLockWarningDialog').hide();
    window.top.layouts[0].resizeAll();

    screenLockWarningOn = false;
    screenLockWarningDialogOn = false;
}

function HideScreenLockWarningAndSync(timeout) {
    if (timeout > 0) {
        HideScreenLockWarning();
    }
    CallServer(timeout * 1000);
    screenLockPinging = true;
}

function ShowScreenLockWarning() {
    screenLockWarningOn = true;
    if (screenLockIntervalCounter == null) {
        screenLockIntervalCounter = setInterval(UpdateScreenLockWarning, 1000);
    }

    $j('#screenLockWarning').show();
    window.top.layouts[0].resizeAll();

    var dialogOn = ($j('#modalBack').is(':visible')) ? true : false;
    if (dialogOn) {
        screenLockWarningDialogOn = true;
    }

    $j('#screenLockWarningDialog').show();

    UpdateScreenLockWarning();
}

function CallServer(timeoutPeriod) {
    clearTimeout(screenLockTimeout);
    screenLockTimeout = setTimeout('serverRequest("isLocked");', timeoutPeriod);
}

function UpdateScreenLockWarning() {
    if (screenLockWarningOn) {
        if (screenLockCounter > 0) {
            screenLockCounter--;
        }
        else {
            clearInterval(screenLockIntervalCounter);
            screenLockIntervalCounter = null;
            CallServer(0);
        }
        $j('#screenLockTime').html(screenLockCounter);
        $j('#screenLockTimeDialog').html(screenLockCounter);

        var dialogOn = ($j('#modalBack').is(':visible')) ? true : false;
        if (!dialogOn) {
            if (screenLockWarningDialogOn) {
                $j('#screenLockWarningDialog').hide();
                $j('#screenLockWarning').hide();
                window.top.layouts[0].resizeAll();

                screenLockWarningOn = false;
                screenLockWarningDialogOn = false;

                // Dialog was closed while countdown
                serverRequest('action');
            }
        }
    }
}

function ShowScreenLockDialog() {
    HideScreenLockWarning();
    showModalPopup('screenLockDialog', 'screenLockBackground');
    $j('#screenLockDialog').css('z-index', '42200');
    var txtPassword = $j('#screenLockDialogPassword');
    txtPassword.val('');
    txtPassword.focus();
    screenLockIsLocked = true;
}

function CancelScreenLockCountdown() {
    serverRequest('cancel');
}

function ScreenLockEnterHandler(event) {
    if (event.which == 13 || event.keyCode == 13) {
        ScreenLockValidateUser();
        return false;
    }
    return true;
}