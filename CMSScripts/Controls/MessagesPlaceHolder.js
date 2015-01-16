// MessagesPlaceHolder.js

if (window.cmsLbls == null) {
    var cmsLbls = new Array();
}

function CMSHandleLabel(id, description, delay, close) {
    var elm = $j('#' + id);
    window.cmsLbls[id] = elm;
    var lblOp = (lblsOpacity / 100.0);
    elm.css('opacity', (delay == 0) ? lblOp : '1');

    CMSRfrLblPos(elm);
    $j(window).resize(function () { CMSRfrLblPos(elm); });

    if (description != '') {
        var desc = $j('<p>' + description + '</p>').hide();
        var lnk = $j('<span>' + lblDetails + '</span>').addClass('Lnk');
        elm.append($j('<span>&nbsp;(</span>')).append(lnk).append($j('<span>)</span>'));
        lnk.click(function () { desc.slideToggle('slow', function () { lnk.text(desc.is(':visible') ? lblLess : lblDetails); }); });
        elm.append(desc);
    }
    if (delay > 0) {
        elm.mouseenter(function () { elm.stop(true, true).fadeIn('fast'); });
        elm.mouseleave(function () { elm.delay(delay).fadeOut('fast') });
        elm.delay(delay).fadeOut('fast');
        if (description == '') {
            elm.click(function () { elm.hide(); });
        }
    }
    else {
        var plc = null;
        if (useRelPlc) {
            plc = $j('<div></div>').addClass('lblPlc').addClass(elm.attr('id')).css('height', elm.outerHeight(true));
            elm.after(plc);
        }

        if (close) {
            var close = $j('<span></span>').addClass('Close').click(function () { elm.hide(); if (plc != null) { plc.hide(); } });
            elm.append(close);
        }
        elm.hover(function () { elm.addClass("Hover"); }, function () { elm.removeClass("Hover"); });
    }
}


function CMSRfrLblPos(elm) {
    if (elm.is(':hidden') || elm.hasClass("Hover")) { return; }
    var opacity = elm.css('opacity');
    elm.css('opacity', 0).css('position', 'static').css('top', 'auto');
    var offset = elm.offset();
    elm.css('position', 'fixed');
    var top = CMSGetPlcPos();

    //Wrapper object
    var ctlrwrpobj = null;
    if (wrpCtrlid != '') {
        ctlrwrpobj = $j('#' + wrpCtrlid);
    }

    // Offset top
    var otop = offset.top;
    if (ctlrwrpobj != null) {
        otop = ctlrwrpobj.offset().top;
        elm.css('margin-top', '10px');
    }

    // Ensure wrapper max width
    if (ctlrwrpobj != null) {
        elm.css('max-width', ctlrwrpobj.width() - 100);
    }

    elm.prevAll(".AdvancedLabel").each(function (i) {
        var sib = $j(this);
        if (sib.offset().top >= top) {
            top += sib.outerHeight(true);
        }
    });

    top = (top > otop) ? top : otop;

    // Add Y offset if defined
    top = top + posOffsetY;

    elm.css('top', top).css('opacity', opacity);
    var isrtl = $j('body').hasClass('RTL');
    var xpos = isrtl ? 'right' : 'left';

    // Wrapper X-axis offset
    var wrapperOfsetX = 0;
    if ((ctlrwrpobj != null)) {
        if (isrtl) {
            wrapperOfsetX = $j(window).width() - (ctlrwrpobj.offset().left + ctlrwrpobj.outerWidth());
        }
        else {
            wrapperOfsetX = ctlrwrpobj.offset().left;
        }
    }
    elm.css(xpos, posOffsetX + wrapperOfsetX);
}

function CMSGetPlcPos() {
    var pos = 0;
    $j('.CMSFixPanel, #CMSHeaderPad, .PreviewMenu, .SplitToolbar.Vertical').each(function (i) { pos += $j(this).height(); });
    return pos;
}

function CMSRfrLblsPos() {
    if (window.cmsLbls != null) {
        for (var key in window.cmsLbls) {
            CMSRfrLblPos(window.cmsLbls[key]);
        }
    }
}

function CMSHndlLblAnchor(ctrlId, name) {
    var ctrl = $j('#' + ctrlId);
    var parent = ctrl.parent();
    var top = CMSGetPlcPos();
    if (useRelPlc) {
        $j('.lblPlc').each(function (i) { top += $j(this).outerHeight(true); });
    }
    else {
        $j('.AdvancedLabel').each(function (i) { top += $j(this).outerHeight(true); });
    }
    $j(window).scrollTop(parent.offset().top - top);
    $j('.AnchorFocus').each(function (i) { $j(this).removeClass('AnchorFocus'); });
    parent.addClass('AnchorFocus');
    var inEl = null;
    var forId = ctrl.prop('for');
    if (typeof (forId) != "undefined") {
        inEl = $j('#' + forId);
    }
    else {
        inEl = $j('input[name*="$' + name + '$"],textarea[name*="$' + name + '$"]');
    }

    if ((inEl != null) && (inEl.length > 0)) {
        if (typeof (inEl.focus) == 'function') {
            inEl.focus();
        };

        var ckEditor = window.CKEDITOR;
        if ((typeof (ckEditor) != 'undefined') && (ckEditor.instances != null)) {
            var oEditor = ckEditor.instances[inEl.attr("id")];
            if (oEditor != null) {
                oEditor.focus();
            }
        }
    }
}
