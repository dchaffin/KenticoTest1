
function SendEvent(eventType, refreshPage, data) {
    if (parent.SendEvent && (parent != this)) {
        parent.SendEvent(eventType, refreshPage, data);
    }
    else {

        // Set the refresh page
        if (!!refreshPage) {
            refreshPageOnClose = true;
        }

        // Top frame events (do not delegate them to the child frames)
        switch (eventType) {
            case 'close':
                CloseDialog();
                return;

            case 'setrefreshpage':
                refreshPageOnClose = true;
                return;

            case 'updatevariantposition':
                UpdateVariantPosition(data.itemCode, data.variantId);
                return;
        }

        // Start raising child frames events
        RaiseEvent(eventType, refreshPage);
    }
}

// Raise the event. If there are frames, delegate the event into the frames.
function RaiseEvent(eventType, refreshPage) {

    // Delegate the refreshPage flag to the child pages where the variable is defined (solves issue with CloseDialog() in the layout pages)
    if (typeof (refreshPageOnClose) !== 'undefined') {
        refreshPageOnClose = !!refreshPage;
    }

    var applyHandler = false;
    
    // If frames collection contains itself, apply handler
    if (frames != null) {
        for (var i = 0; i < frames.length; i++) {
            if (frames[i].location.href == this.location.href) {
                applyHandler = true;
                break;
            }
        }
    }

    // Other event handlers which are to be delegated to the child frames
    if (!applyHandler && ((frames != null) && (frames.length > 1))) {
        for (var i = 0; i < frames.length; i++) {
            try {
                if (frames[i].RaiseEvent) {
                    frames[i].RaiseEvent(eventType, refreshPage);
                }
            }
            catch (ex) {
            }
        }
    }
    // No child frames -> proceed the event
    else {
        if (eventType == 'ok') {
            if (typeof (OnOkHandler) == 'function') {
                OnOkHandler();
            }
        }
        if (eventType == 'apply') {
            if (typeof (OnApplyHandler) == 'function') {
                OnApplyHandler();
            }
        }
    }
}
