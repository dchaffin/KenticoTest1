<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AdvancedPopupHandler.ascx.cs"
    Inherits="CMSAdminControls_UI_AdvancedPopupHandler" %>
<div id="modalWrap" style="display: none;">
</div>
<script type="text/javascript">
//<![CDATA[
    window.AdvancedModalDialogs = true;

    var fullScreenSize = { width: '95%', height: '95%' };
    var globalSetTitle = <%=SetTitle.ToString().ToLower() %>;
    var dialogZIndex = 1000;
    var handlerDoc = $j(document);
    var handlerWin = $j(window);
    var visiblePopup = null;
    var topOverlaySelector = '.ui-widget-overlay:last';
    
    var hideBackground = function() {
        $j(topOverlaySelector).stop(false, true).fadeOut('slow');
    };

    var showBackground = function() {
        var background = $j(topOverlaySelector);
        background.css('filter', 'alpha(opacity=70)');
        background.stop(false, true).fadeIn('slow');
    };

    function bindBackground() {
        $j(topOverlaySelector).on("mouseenter", hideBackground);
    }

    function unbindBackground() {
        $j(topOverlaySelector).off("mouseenter", hideBackground);
    }
    
    /*
    // Scroll prevent prototype
    function scrollStop(e) {
        if(!e){ 
            // IE7, IE8, Chrome, Safari
            e = window.event; 
        }
        if(e.preventDefault) { 
            // Chrome, Safari, Firefox
            e.preventDefault(); 
        } 
        // IE7, IE8
        e.returnValue = false; 
    }

    function initScrollStop(elem) {
        if (elem.addEventListener) {    
            // All browsers except IE before version 9
            // Internet Explorer, Opera, Google Chrome and Safari
            elem.addEventListener ("mousewheel", scrollStop, false);
            // Firefox
            elem.addEventListener ("DOMMouseScroll", scrollStop, false);
        }
        else {
            if (elem.attachEvent) { 
                // IE before version 9
                elem.attachEvent ("onmousewheel", scrollStop);
            }
        }
    }
    initScrollStop(document);    
    */
    
    function generateGuid() {
        var S4 = function() {
           return (((1+Math.random())*0x10000)|0).toString(16).substring(1);
        };
        return (S4()+S4()+"-"+S4()+"-"+S4()+"-"+S4()+"-"+S4()+S4()+S4());
    }
    
    function getAbsolute(percent, ref) {
        if (percent.substr && percent.substr(percent.length - 1) === '%') {
            var leftPercent = percent.substr(0, percent.length - 1);
            percent = ref * leftPercent / 100;
        }
        return percent;
    }
    
    function getAbsoluteSize(width, height) {
        width = getAbsolute(width, handlerWin.width());
        height = getAbsolute(height, handlerWin.height());
        return { width: width, height: height };
    }
    
    function getAbsolutePosition(left, top) {
        left = getAbsolute(left, handlerDoc.width());
        top = getAbsolute(top, handlerDoc.width());
        return { left: left, top: top };
    }

    function storeModal(iframeElem) {
        var index = 0;
        var stored = false;
        for (var i = 0; i < window.allModals.length; i++) {
            if (window.allModals[i] == null) {
                // Add to free space in array
                window.allModals[i] = iframeElem;
                index = i;
                stored = true;
            }
        }
        if (!stored) {
            // Add to the end of array
            index = window.allModals.length;
            window.allModals[window.allModals.length] = iframeElem;
        }
        return index;
    }
    
    function getPercentSize(width, height) {
        var percentWidth = null;
        var percentHeight = null;
        if (width.substr && width.substr(width.length - 1) === '%') {
            percentWidth = width;
        } else {
            percentWidth = width / (handlerWin.width() / 100);
            if (percentWidth > 95) {
                percentWidth = 95;
            }
            percentWidth += '%';
        }

        if (height.substr && height.substr(height.length - 1) === '%') {
            percentHeight = height;
        } else {
            percentHeight = height / (handlerWin.height() / 100);
            if (percentHeight > 95) {
                percentHeight = 95;
            }
            percentHeight += '%';
        }

        return { width: percentWidth, height: percentHeight };
    }
    
    handlerWin.scroll(function (data) {
        if(window.allModals) {
            var scrollTop = handlerDoc.scrollTop();
            var scrollLeft = handlerDoc.scrollLeft();
            var i = 0;
            for (i = 0; i < window.allModals.length; i++) {
                var frame = window.allModals[i];
                var dialogElem = frame.parent();
                var actOff = dialogElem.data('actualOffset');
                dialogElem.css('left', actOff.left + scrollLeft);
                dialogElem.css('top', actOff.top + scrollTop);
                //dialogElem.animate({ left: actOff.left + scrollLeft, top: actOff.top + scrollTop });
            }
        }
    });

    function containsWindow(popupWindow, windowToFind, recursive) {
        if(recursive == undefined) {
            recursive = true;
        }
        if (popupWindow == windowToFind) {
            return true;
        } else {
            if (popupWindow.frames && popupWindow.frames.length > 0) {
                for (var i = 0; i < popupWindow.frames.length; i++) {
                    var frame = popupWindow.frames[i];
                    if(recursive) {
                        if (containsWindow(frame, windowToFind)) {
                            return true;
                        }
                    }
                    else {
                        if (popupWindow == windowToFind) {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    function isFullScreen() {
        var dialogElem = visiblePopup.parent();
        return dialogElem.data('fullScreen');
    }

    function toggleFullScreen() {
        var position = new Object();
        var currentOffset = new Object();
        var dialogElem = visiblePopup.parent();
        var size;
        if (isFullScreen()) {
            var origSize = dialogElem.data('originalSize');
            size = getAbsoluteSize(origSize.width, origSize.height);
            currentOffset = dialogElem.data('originalOffset');
            dialogElem.find('.ui-resizable-handle').show();
        } else {
            size = getAbsoluteSize(fullScreenSize.width, fullScreenSize.height);
            currentOffset = { top: ((handlerWin.height() - size.height) / 2), left: ((handlerWin.width() - size.width) / 2) };
            dialogElem.find('.ui-resizable-handle').hide();
        }
        
        dialogElem.data('actualOffset', currentOffset);
        dialogElem.data('actualSize', size);
        
        position.left = currentOffset.left + handlerDoc.scrollLeft();
        position.top = currentOffset.top + handlerDoc.scrollTop();
        
        visiblePopup.width(size.width).height(size.height);
        dialogElem.width(size.width).height(size.height);
        dialogElem.css('left', position.left).css('top', position.top);
        
        dialogElem.data('fullScreen', !dialogElem.data('fullScreen'));
    }

    function getExistingModal(name, opener) {
        var i = 0;
        for (i = 0; i < window.allModals.length; i++) {
            var frame = window.allModals[i];
            if (frame != null) {
                var frameObject = frame.get(0);
                var win = frameObject.contentWindow;
                if ((frameObject.name == name) && (win.wopener == opener)) {
                    return { frame: frame, index: i };
                }
            }
        }
        return null;
    }
    
    function ensureBackgroundClickHandler(win) {
        $j(topOverlaySelector).on('click', function (e) {
            // Allow to close window only when close button is not available
            if (win.closeButtonAvailable) {
                return;
            } else {
                closeDialog(win);
            }
        });
    }

    function ensureDragOverlay(elem) {
        var container = elem;
        var overlay = container.find('.hidden-dialog-overlay');
        if (!overlay.length) {
            overlay = $j('<div class="hidden-dialog-overlay"></div>');
            overlay.appendTo(container);
        }
        else {
            overlay.show();
        }
    }
    
    function destroyDragOverlay(elem) {
        elem.children('.hidden-dialog-overlay').hide();
    }
    
    function normalizeDimension(d) {
        if (d.toString().indexOf('px') != -1) {
            return parseInt(d, 10);
        }
        return d;
    }

    function advancedModal(url, name, width, height, otherParams, noWopener, forceModal, setTitle, opener) {
        width = normalizeDimension(width);
        height = normalizeDimension(height);
        var percentSize = getPercentSize(width, height);
        width = percentSize.width;
        height = percentSize.height;

        if (window.allModals == undefined) {
            window.allModals = new Array();
        }

        // Try to get frame with same name
        var modalObject = getExistingModal(name, opener);
        var modalFrame = null;
        
        if (modalObject != null) {
            modalFrame = modalObject.frame;
        }
        else {
            // Create new frame
            modalFrame = $j(document.createElement('iframe'));
            storeModal(modalFrame);
        }
        
        // Set currently visible popup
        visiblePopup = modalFrame;

        // Append frame to wrapping element
        var modalWrap = $j('#modalWrap');
        modalWrap.append(modalFrame);

        // Get absolute size
        var size = getAbsoluteSize(width, height);
        
        // Init frame params
        modalFrame.attr('width', size.width);
        modalFrame.attr('height', size.height);
        modalFrame.attr('class', 'UIPopupDialog');
        modalFrame.attr('frameBorder', 0);
        modalFrame.attr('id', generateGuid());
        modalFrame.attr('name', name);
        modalFrame.attr('src', 'about:blank');

        if (!(setTitle && globalSetTitle)) {
            modalFrame.get(0).stopAutoTitle = true;
        }
        storeWopener(modalFrame, opener);
        
        var mdl = modalFrame.dialog({
           resizable: true,
           modal: true,
           closeOnEscape: false,
           width: size.width,
           height: size.height,
           iframe: true,
           title: null,
           zIndex: dialogZIndex,
           create: function(event, ui) {
               // Load frame when dialog is shown to prevent multiple loading
               modalFrame.attr('src', url);
           }
        })
        .bind('dialogresizestart', function() { ensureDragOverlay($j(this).parent());
            if(!forceModal) {
                showBackground();
                unbindBackground();
            }
        })
        .bind('dialogresizestop', function() {
            var frame = $j(this);
            var dlg = frame.parent();
            destroyDragOverlay(dlg);
            var actSize = { width: frame.width(), height: frame.height() };
            dlg.data('originalSize', actSize);
            dlg.data('actualSize', actSize);
            
            if(!forceModal) {
                bindBackground();
            }

        })
        .bind('dialogdragstart', function() { ensureDragOverlay($j(this).parent());})
        .bind('dialogdragstop', function() { destroyDragOverlay($j(this).parent());})
        .width(size.width)
        .height(size.height)
        .css('padding', 0)
        ;
        
        // Get current offset and dimensions
        var dialogElem = visiblePopup.parent();
        var offset = dialogElem.offset();
        var completeOffset = { left: offset.left - handlerDoc.scrollLeft(), top: offset.top - handlerDoc.scrollTop() };
        dialogElem.data('originalOffset', completeOffset);
        dialogElem.data('actualOffset', completeOffset);

        var actualSize = { width: dialogElem.width(), height: dialogElem.height() };
        dialogElem.data('originalSize', actualSize);
        dialogElem.data('actualSize', actualSize);

        // Ensure closing by clicking on background overlay
        ensureBackgroundClickHandler(mdl.get(0).contentWindow);
        
        if(!forceModal) {
            mdl.on("mouseenter", showBackground);
            bindBackground();
        }
    }

    function isTitleWindow(popupWindow, windowToExamine) {
        if (popupWindow == windowToExamine) {
            return true;
        } else {
            if (popupWindow.frames && popupWindow.frames.length > 0) {
                var frame = popupWindow.frames[0];
                if (containsWindow(frame, windowToExamine, false)) {
                    return true;
                } else {
                    return isTitleWindow(frame, windowToExamine);
                }
            }
        }
        return false;
    }

    function closeDialog(win) {
        var i = 0;
        for (i = 0; i < window.allModals.length; i++) {
            var frame = window.allModals[i];
            var currentWindow = frame.get(0).contentWindow;
            // Find the iframe to hide
            if (containsWindow(currentWindow, win)) {
                // Get wopener
                var currentModalWopener = currentWindow.wopener;
                // Prevent IE to reload iframe on close
                if(!$j.browser.opera) {
                    frame.attr('src', 'about:blank');
                }
                // Remove window opener from collection
                removeWopener(win);
                // Remove iframe from DOM
                frame.remove();
                // Remove iframe from collection
                window.allModals.splice(i, 1);
                // Change title
                if (window.AutoTitle && !currentWindow.stopAutoTitle && globalSetTitle) {
                    AutoTitle(currentModalWopener);
                }
                break;
            }
        }
        for (i; i >= 0; i--) {
            // Get previous iframe used as popup
            var hiddenFrame = window.allModals[i];
            if (hiddenFrame != null) {
                // Set it as currently visible
                visiblePopup = hiddenFrame;
                break;
            } 
            else {
                visiblePopup = null;
            }
        }
    }

    function initWopeners() {
        if (window.allWopeners == undefined) {
            window.allWopeners = new Array();
        }
    }

    function storeWopener(frame, win) {
        initWopeners();
        window.allWopeners[frame.get(0).id] = win;
    }

    function removeWopener(win) {
        initWopeners();
        var key = getWopenerKey(win);
        if(key) {
            allWopeners[key] = null;
        }
    }
    
    function getWopenerKey(win) {
        initWopeners();
        if (window.allModals) {
            var i = 0;
            for (i = 0; i < window.allModals.length; i++) {
                var wopenerFrame = window.allModals[i];
                if (wopenerFrame != null) {
                    var wopWin = wopenerFrame.get(0).contentWindow;
                    if (containsWindow(wopWin, win)) {
                        return wopenerFrame.get(0).id;
                    }
                }
            }
        }
        return null;
    }
    
    function getWopener(win) {
        initWopeners();
        var key = getWopenerKey(win);
        if(key) {
            return  allWopeners[key];
        } else {
            return null;
        }
    }

    function getTop(win) {
        if (window.allModals && (win != top)) {
            var i = 0;
            for (i = 0; i < window.allModals.length; i++) {
                var frame = window.allModals[i];
                var currentWindow = frame.get(0);
                // Find the top frame
                if (containsWindow(currentWindow.contentWindow, win)) {
                    return currentWindow.contentWindow;
                }
            }
        }
        return window.top;
    }
    
    //]]>
</script>
