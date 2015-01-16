var frameset = false;
var changed = false;
var allowSubmit = false;
var oldSubmit = null;
var oldLoad = null;

var iframes = null;
var confirmChanges = true;
var hdnSaveChangesId = 'saveChanges';

function AllowChanged(elementId) {
    if (window.checkChangedFields == null) {
        return true;
    }

    for (var i = 0; i < checkChangedFields.length; i++) {
        if (checkChangedFields[i] == elementId) {
            return true;
        }
    }
    return false;
}

function Changed() {
    changed = true;
}

function NotChanged() {
    changed = false;
}

function AllowSubmit() {
    allowSubmit = true;
}

function UnloadPage(evt) {
    if (DataChanged() && !allowSubmit && confirmChanges) {
        return confirmLeave;
    }
}

function CheckChanges() {
    if (DataChanged() && typeof (confirmLeave) != "undefined") {
        if (confirm(confirmLeave)) {
            changed = false;
            ResetEditorsIsDirty();
            return true;
        }
        else {
            return false;
        }
    }
    else {
        return true;
    }
}

function SubmitAction() {
    if (DataChanged() && !allowSubmit) {
        jQuery('#' + hdnSaveChangesId).val('1');
    }
    else {
        jQuery('#' + hdnSaveChangesId).val('0');
    }

    changed = false;
    allowSubmit = true;
    return true;
}

function SubmitPage() {
    if (!DataChanged() || allowSubmit || SubmitAction()) {
        if ((oldSubmit != null) && (oldSubmit != SubmitPage)) {
            oldSubmit();
        }
    }
}

function InitChangesLoad() {
    if ((oldLoad != null) && (oldLoad != InitChangesLoad)) {
        oldLoad();
    }
    if (window.theForm != null) {
        oldSubmit = theForm.onsubmit;
        theForm.onsubmit = SubmitPage;
    }

    var formChildren = jQuery("form *").not(".NotCheckChanges, .NotCheckChanges *").filter(":visible").filter(":input").not(":input[type=image]");
    formChildren.each(function (index) {
        var elem = jQuery(this);
        if (AllowChanged(elem.attr("id"))) {
            elem.bind('change', function () { Changed(); });
            if(!elem.is('[readonly]')) {
                elem.bind('keyup', function () { Changed(); });
            }
        }
    });
}

function InitChanges() {
    if (oldLoad == null) {
        oldLoad = window.onload;
        window.onload = InitChangesLoad;
    }

    if ((typeof (Sys) != 'undefined') && (typeof (Sys.WebForms) != 'undefined') && (typeof (Sys.WebForms.PageRequestManager) != 'undefined')) {
        var manager = Sys.WebForms.PageRequestManager.getInstance();
        if (manager) {
            manager.remove_endRequest(InitChangesLoad);
            manager.add_endRequest(InitChangesLoad);
        }
    }
}

function DataChanged() {
    return changed || EditorsChanged();
}

function FullTrim(text) {
    return text.replace(/\s+/g, "");
}

function EditorsChanged() {
    try {
        var ckEditor = window.CKEDITOR;
        if ((typeof (ckEditor) != 'undefined') && (ckEditor.instances != null)) {
            for (var name in ckEditor.instances) {
                if (AllowChanged(name)) {
                    var oEditor = ckEditor.instances[name];
                    if (oEditor.checkDirty()) {
                        var prevValue = oEditor._.previousValue;
                        var oldText = FullTrim(prevValue);

                        var newText = FullTrim(oEditor.getData());
                        if (oldText != newText) {
                            return true;
                        }
                    }
                }
            }
        }
    }
    catch (ex) {
    }
    return false;
}

function ResetEditorsIsDirty() {
    try {
        var ckEditor = window.CKEDITOR;
        if ((typeof (ckEditor) != 'undefined') && (ckEditor.instances != null)) {
            for (var name in ckEditor.instances) {
                if (AllowChanged(name)) {
                    var oEditor = ckEditor.instances[name];
                    if (oEditor.checkDirty()) {
                        oEditor.resetDirty();
                    }
                }
            }
        }
    }
    catch (ex) {
    }
    return false;
}

if (typeof (Sys) != "undefined" && typeof (Sys.Application) != "undefined") {
    Sys.Application.notifyScriptLoaded();
}