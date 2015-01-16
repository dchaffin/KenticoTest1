// Web part toolbar element variables
var wptPanel = null;
var wptLayout = null;
var wptTrash = null;
var wptTrashDropHelper = null;
var wptTrashDropArea = null;
var wptTrashCue = null;
var wptMenu = null;
var wptScrollContainer = null;

// Toolbar helper variables
var wptStartSelectHandler = null;
var wptEnableAutoScroll = true;
var wptAutoScrollTimer;
var wptFilterWebPartsTimer;
var wptReload = false;

// Initial position variables
var wptInitContainerScroll = { x: 0, y: 0 };
var wptInitWindowScroll = { x: 0, y: 0 };
var wptPrevWindowScroll = { x: 0, y: 0 };

// Initialize the toolbar variables
function wptInit(forceReload) {
    if (forceReload) {
        // Renew the toolbar variables
        wptPanel = null;
        wptLayout = null;
        wptTrash = null;
        wptTrashDropHelper = null;
        wptTrashDropArea = null;
        wptScrollContainer = null;
    }

    // Load the web part toolbar element variables

    if (wptPanel === null) {
        wptPanel = jQuery('.WPTPanel:first');
    }

    if (wptLayout === null) {
        wptLayout = jQuery('#wptLayout:first', wptPanel);
    }

    if (wptTrash === null) {
        wptTrash = jQuery('.WPTTrash:first', wptPanel);
    }

    if (wptTrashDropHelper === null) {
        wptTrashDropHelper = jQuery('.WPTTrashDropHelper:first');
    }

    if (wptTrashCue === null) {
        wptTrashCue = jQuery('.WPTTrashCue:first', wptTrashDropHelper);
    }

    if (wptTrashDropArea === null) {
        wptTrashDropArea = jQuery('.WPTTrashDropArea:first', wptTrashDropHelper);
    }

    if (wptMenu === null) {
        wptMenu = jQuery('.WPTMenu:first', wptPanel);
    }

    if (wptScrollContainer === null) {
        wptScrollContainer = jQuery('.WPTItemsContainer:first', wptPanel);
        wptScrollContainer.height(wptLayout.height());
    }

    // Used forced reload
    if (forceReload) {
        // Disable selection for these elements. If not disabled then when dragging web parts, these elements get selected
        wptScrollContainer.spDisableSelection();

        var tooltipPosition = "";
        switch (wptPosition) {
            case "bottom": tooltipPosition = "top center";
                break;
            case "left": tooltipPosition = "center right";
                break;
            case "right": tooltipPosition = "center left";
                break;
            case "top":
            default: tooltipPosition = "bottom center";
                break;
        }

        var tooltipClass = (!wptIsRTL) ? "WPTTT LTR" : "WPTTT RTL";

        // Set the toolbar web part tooltip
        jQuery('.WPTSelectorEnvelope, .WPTSelectorEnvelopeHover')
            .mouseout(function () { /* IE8 fix */var tooltipObj = jQuery(this).data("tooltip"); if (tooltipObj != null) { tooltipObj.hide(); } })
            .tooltip({
                predelay: 500,
                effect: "fade",
                events: {
                    def: "mouseenter,mouseleave mousedown",
                    tooltip: ",mouseenter mouseleave"
                },
                tipClass: tooltipClass,
                position: tooltipPosition,
                onBeforeShow: function () {

                    // IE8 tooltip fix (fixes jQuery tools - tooltip bug)
                    if (jQuery('body').hasClass('IE8')) {
                        return true;
                    }

                    if (wptReload) {
                        return false;
                    }
                    // Display the tooltip only for the toolbar web parts which are not being dragged
                    return (this.getTrigger().get(0).style.position != "absolute");
                }
            });
    }
}

// Highlight the web parts
function wptToggle(el, highlight) {
    el.className = (highlight ? 'WPTSelectorEnvelopeHover' : 'WPTSelectorEnvelope');
}

// Process pressed keys for the search text box
function wptProceedSpecialKeys(el, e) {
    if (e.keyCode == 13) {
        return false;
    }
    else if (e.keyCode == 27) {
        jQuery(el).val('');
        e.preventDefault();
        e.stopPropagation();
        return false;
    }

    return true;
}

// Show/Hide web parts according to the search text
function wptFilter(searchText) {
    var isFirst = null;
    searchText = searchText.toLowerCase();
    jQuery('.WPTItemTxt', wptScrollContainer).each(function () {
        var jThis = jQuery(this);
        var display = 'none';
        if ((searchText.length == 0) || (jThis.html().toLowerCase().indexOf(searchText) != -1)) {
            display = 'block';
            if (isFirst === null) {
                isFirst = true;
            }
        }

        var webPart = jThis.parent().parent();
        webPart.css('display', display);
        if (isFirst) {
            display = 'none';
            isFirst = false;
        }
        // Show/Hide selector
        webPart.prev('.WPTSelectorSeparator').css('display', display);
    });

    // Show the scroll panel if not visible already
    wptScrollContainer.show();
}

// Minimize the toolbar
function wptMinimize() {
    jQuery('.WPTMaximized:first', wptPanel).hide();
    jQuery('.WPTMinimized:first', wptPanel).show();
    if (window.ResizeToolbar) { ResizeToolbar(); }
    jQuery.cookie(wptIsMinimizedCookie, true, { path: '/', expires: 31 });
    wptUpdateWindowPadding();
    // Hide the trash droppable area when the toolbar is minimized
    wptTrashDropArea.hide();
    wptTrashCue.hide();
}

// Maximize the toolbar
function wptMaximize() {
    jQuery('.WPTMaximized:first', wptPanel).show();
    jQuery('.WPTMinimized:first', wptPanel).hide();
    if (window.ResizeToolbar) { ResizeToolbar(); }
    jQuery.cookie(wptIsMinimizedCookie, null, { path: '/' });
    wptInit(true);
    wptReloadScrollPanel(true);
    wptUpdateWindowPadding();
    // Show the trash droppable area when the toolbar is maximized
    wptTrashDropArea.hide();
    wptTrashCue.hide();
    wptRefreshTrashDropAreaPosition();
}

// Apply page padding according to the toolbar position
function wptUpdateWindowPadding() {
    if (wptPosition != "top") {
        var form = jQuery('form');
        var padding = "padding-" + wptPosition;
        if (wptPosition == "bottom") {
            form.css(padding, wptLayout.outerHeight() + "px");
        }
        else {
            // Right
            if (wptPosition == "right") {
                jQuery(".WPTTableCellPadding").css("min-width", wptLayout.outerWidth());
            }
            // Left
            else {
                form.css(padding, wptLayout.outerWidth() + "px");
            }

            jQuery("#CMSHeaderDiv")
                .css("width", "auto")
                .css(padding, wptLayout.outerWidth() + "px");
        }
    }
}

// Load the not-loaded web part images with a delay
function wptLoadWebpartImages() {
    window.setTimeout(function () {
        jQuery('.WPTItemImg', wptPanel).each(function () {
            var jThis = jQuery(this);
            var revAttr = jThis.attr('rev');
            if (revAttr != null) {
                jThis.attr('src', WPTImgBaseSrc + revAttr);
                jThis.attr('rev', null);
            }
        });
    }, 500);
}

// Refresh the trash droppable area position
function wptRefreshTrashDropAreaPosition() {
    if (jQuery.cookie(wptIsMinimizedCookie) == "true") {
        // Hide the trash droppable area when the toolbar is minimized
        wptTrashDropArea.hide();
        wptTrashCue.hide();
    }
    else if (wptTrash != null) {
        var trashTop = wptTrash.offset().top;
        var trashLeft = wptTrash.offset().left;

        // Move the toolbar trash droppable area in the DOM to the form element (fixes the fixed positioning)
        if (wptTrashDropHelper.parent().get(0) != document.forms[0]) {
            wptTrashDropHelper.appendTo(jQuery(document.forms[0]));
        }

        // Set the trash droppable area position
        wptTrashDropArea.css({ top: trashTop, left: trashLeft });
        wptTrashCue.css({ top: trashTop, left: trashLeft });
        wptTrashDropArea.show();
        wptTrashCue.show();
    }
}

// Event risen before the item is dropped onto the trash droppable area
function wptTrashOnBeforeDrop(sender, e) {
    var item = e.get_droppedItem();
    // Work only with the regular web parts (not with the toolbar draggable web parts)
    if (item.getAttribute('data-dragkeepcopy') != 1) {
        // Confirmation dialog
        if (confirm(document.getElementById('removeWebpartText').value)) {
            // Set a flag to indicate confirmation that the user wants to remove the web part
            item.setAttribute('data-removewebpart', '1');
        }
        else {
            // Set a flag to indicate cancelling of the dragging
            item.setAttribute('data-dragcancelled', '1');
        }

        // Keep the document scroll position. Will be used after refresh
        parent.ScrollTop = document.body.scrollTop || document.documentElement.scrollTop;
    }
}

// Event risen before the item is dropped onto the toolbar web part list
function wptListOnBeforeDrop(sender, e) {
    var item = e.get_droppedItem();
    // Set a flag to indicate cancelling of the dragging
    item.setAttribute('data-dragcancelled', '1');
}

// Event risen before the item is dropped onto a web part zone
function OnBeforeDropWebPart(sender, e) {
    var item = e.get_droppedItem();
    // Work only with the toolbar web parts
    if (item.getAttribute('data-dragkeepcopy') == 1) {
        jQuery('.WPTLoaderBackground').height(jQuery(document).height());
        jQuery('#wptLoader').show();
        window.setTimeout(function () { jQuery('#wptLoader').hide(); }, 2000);

        // Keep the document scroll position. Will be used after refresh
        parent.ScrollTop = document.body.scrollTop || document.documentElement.scrollTop;
    }
}

// jQuery events
jQuery(document).ready(function () {
    // Initialize the toolbar variables
    wptInit(true);
    // Apply page padding according to the toolbar position
    wptUpdateWindowPadding();
    // Refresh the droppable trash area position
    wptRefreshTrashDropAreaPosition();
    // Use the lazy load for the rest of the web part images
    wptLoadWebpartImages();
    // Scroll to the saved position
    if ((typeof (parent.ScrollTop) != "undefined") && (parent.ScrollTop > 0)) {
        jQuery(document).scrollTop(parent.ScrollTop);
        parent.ScrollTop = 0;
    }
});

jQuery(window).scroll(function () {
    // Proceed only after all data have been loaded
    if (typeof (wptPosition) != "undefined") {
        // Refresh the droppable trash area position
        wptRefreshTrashDropAreaPosition();
    }
});

jQuery(window).resize(function () {
    // Proceed only after all data have been loaded
    if (typeof (wptPosition) != "undefined") {
        // Refresh the droppable trash area position
        wptRefreshTrashDropAreaPosition();
        // Initialize the toolbar variables
        wptInit(true);
        // Apply page padding according to the toolbar position
        wptUpdateWindowPadding();
    }
});


// DragAndDrop handlers

// Event risen when start dragging and the dragged item is being copied to ensure that the copied item will be dragged (not the original)
function OnCopyDraggedItem(originalItem, copiedItem) {
    // Work only with the toolbar draggable web parts (not with the regular page web parts)
    var itemAttrKeepCopy = (originalItem.getAttribute('data-dragkeepcopy') == 1);
    if (itemAttrKeepCopy) {
        originalItem.className = "WPTSelectorEnvelope";
        // Ensure that the dragged web part will be hovered for the whole time
        copiedItem.onmouseout = function () { };
    }
}

// Event risen before the item is being dragged
function OnBeforeStartDrag(el) {
    // Work only with the toolbar draggable web parts (not with the regular page web parts)
    var itemAttrKeepCopy = (el.getAttribute('data-dragkeepcopy') == 1);
    if (itemAttrKeepCopy) {
        var docScroll = { x: 0, y: 0 };
        docScroll.x = document.body.scrollLeft || document.documentElement.scrollLeft;
        docScroll.y = document.body.scrollTop || document.documentElement.scrollTop;

        // Chrome text selection correction
        wptStartSelectHandler = document.onselectstart;
        document.onselectstart = function () { return false; };

        // Setup the scroll variables used for fixing of the fixed positioning
        if (wptPosition == "top") {
            wptInitWindowScroll.x = -docScroll.x;
            wptInitWindowScroll.y = -docScroll.y - 2 * (wptPanel.offset().top - docScroll.y);  // Because of the fixed positioning
        }
        else {
            wptInitWindowScroll.x = docScroll.x;
            wptInitWindowScroll.y = docScroll.y;
        }

        wptInitContainerScroll.x = wptScrollContainer.scrollLeft();
        wptInitContainerScroll.y = wptScrollContainer.scrollTop();
        wptPrevWindowScroll.x = docScroll.x;
        wptPrevWindowScroll.y = docScroll.y;

        var location = Sys.UI.DomElement.getLocation(el);
        var containerX = wptScrollContainer.scrollLeft();
        var containerY = wptScrollContainer.scrollTop();
        var wptX = wptLayout.position().left;
        var wptY = wptLayout.position().top;

        // Fix the IE RTL scrollLeft behavior
        var bodyElem = jQuery('body');
        if (bodyElem.hasClass('IE8') || bodyElem.hasClass('IE9')) {
            containerX = wptInitContainerScroll.x = (-1) * wptScrollContainer.scrollLeft();
        }

        // Correct the dragged element location
        if (location.x < 0) {
            location.x = 0;
        }
        if (location.y < 0) {
            location.y = 0;
        }
        location.x += (2 * docScroll.x - containerX - 2 * wptX); // 2*wptX - because wpt is in fixed positioning
        location.y += (2 * docScroll.y - containerY - 2 * wptY);

        Sys.UI.DomElement.setLocation(el, location.x, location.y);
    }
}

// Event risen when the method setLocation() in the DragAndDrop extender is fired
function OnSetLocation(el, point) {
    // Work only with the toolbar draggable web parts (not with the regular page web parts)
    var itemAttrKeepCopy = (el.getAttribute('data-dragkeepcopy') == 1);
    if (itemAttrKeepCopy) {
        
        var docScroll = { x: 0, y: 0 };
        docScroll.x = document.body.scrollLeft || document.documentElement.scrollLeft;
        docScroll.y = document.body.scrollTop || document.documentElement.scrollTop;

        // Correct the X position when autoscroll is active
        if (wptPrevWindowScroll.x == docScroll.x) {
            // Correct the element location (due to used fixed positioning)
            point.x = point.x - docScroll.x + wptInitWindowScroll.x + wptInitContainerScroll.x;
        }
        // Correct the Y position when autoscroll is active
        if (wptPrevWindowScroll.y == docScroll.y) {
            // Correct the element location (due to used fixed positioning)
            point.y = point.y - docScroll.y + wptInitWindowScroll.y + wptInitContainerScroll.y;
        }

        // Save the previous document scroll position
        wptPrevWindowScroll.x = docScroll.x;
        wptPrevWindowScroll.y = docScroll.y;
    }

    return point;
}

// Event risen when the user drops the draggable web part onto the droppable area
function OnDragEnd(el) {
    // Work only with the toolbar draggable web parts (not with the regular page web parts)
    var itemAttrKeepCopy = (el.getAttribute('data-dragkeepcopy') == 1);
    if (itemAttrKeepCopy) {
        // Restore the onmouseout event 
        el.onmouseout = function () { this.className = 'WPTSelectorEnvelope'; };
        el.className = "WPTSelectorEnvelope";
        // Restore the onselectstart event (due to a Chrome bug)
        document.onselectstart = wptStartSelectHandler;
    }
}

// Event risen when the user moves a draggable web part onto a droppable area
function OnShowDropCue(cueElem, dropItem) {
    var jCue = jQuery(cueElem);
    // Delete the scroll timer (which is supposed to enable scrolling)
    window.clearTimeout(wptAutoScrollTimer);

    // Work with the toolbar trash droppable area
    if (jCue.hasClass('WPTTrashCue')) {
        // Disable scrolling
        wptEnableAutoScroll = false;

        if (dropItem.getAttribute('data-dragkeepcopy') == 1) {
            // Do not highlight the trash for web parts dragged from the toolbar
            jCue.hide();
        }
        else {
            jCue.show();
            jCue.css({ height: "" });
            // Highlight the trash (dragged web parts belongs to the page - not to the toolbar)
            wptTrash.addClass('WPTTrashHover');
        }
    }
}

// Event risen when the drop cue is being repositioned. This function fixes the toolbars "fixed" positioning together with scrolled document.
function OnRepositionDropCue(el, location) {
    if (el.getAttribute('data-dragkeepcopy') == 1) {
        var scrollLeft = document.body.scrollLeft || document.documentElement.scrollLeft;
        var scrollTop = document.body.scrollTop || document.documentElement.scrollTop;
        location.x += scrollLeft;
        location.y += scrollTop;
    }

    return location;
}

// Event risen when the user moves a draggable web part out from a droppable area
function OnHideDropCue(cueElem) {
    // Refresh the trash droppable area
    wptRefreshTrashDropAreaPosition();
    // Delete the scroll timer (which is supposed to enable scrolling)
    window.clearTimeout(wptAutoScrollTimer);
    // Enable scrolling in 2 seconds
    wptAutoScrollTimer = window.setTimeout(function () { wptEnableAutoScroll = true; }, 2000);

    if (jQuery(cueElem).hasClass('WPTTrashCue')) {
        // Disable highlighting of the trash
        wptTrash.removeClass('WPTTrashHover');

        // Reset the drop cue width (It is automatically updated by the DragAndDrop extender. The drop cue width is updated by the dropped item width when the drop cue is displayed)
        wptTrashCue.css("width", "auto");
    }
}

// Event risen before the DragAndDrop extender wants to scroll the window
function OnEnableAutoScroll() {
    if (wptTrash.hasClass('WPTTrashHover')) {
        window.clearTimeout(wptAutoScrollTimer);
        wptEnableAutoScroll = false;
    }

    // Enable/disable scrolling
    return wptEnableAutoScroll;
}
