var menuTimers = new Array();
var menuElements = new Array();
var dynamicMenus = new Array();
var openMenus = new Array(10);
var loadingContent = new Array();
var mouseOverTimers = new Array();
var timerParameters = new Array();
var menuParameters = new Array();
var menuSettings = new Array();
var repositionedMenus = new Array();
var mouseOverLevel = new Array();

var x = 0;
var y = 0;
var show = true;
var currentMenuId = null;
var currentElem = null;
var lastMenuZindex = 20201;

function Get(id) {
    return document.getElementById(id);
}

function DisableRight() {
    return ((window.event != null) ? (window.event.button != 2) : false);
}

function DoNothing() {
    return false;
}

function GetScrollX() {
    if (typeof (window.pageXOffset) == 'number') {
        return window.pageXOffset;
    }
    else if (document.body && (document.body.scrollLeft || document.body.scrollTop)) {
        return document.body.scrollLeft;
    }
    else if (document.documentElement && (document.documentElement.scrollLeft || document.documentElement.scrollTop)) {
        return document.documentElement.scrollLeft;
    }

    return 0;
}

function GetScrollY() {
    if (typeof (window.pageYOffset) == 'number') {
        return window.pageYOffset;
    }
    else if (document.body && (document.body.scrollLeft || document.body.scrollTop)) {
        return document.body.scrollTop;
    }
    else if (document.documentElement && (document.documentElement.scrollLeft || document.documentElement.scrollTop)) {
        return document.documentElement.scrollTop;
    }

    return 0;
}

function GetMouseX(ev) {
    return ev.clientX + GetScrollX();
}

function GetMouseY(ev) {
    return ev.clientY + GetScrollY();
}

function IsIE7() {
    return jQuery.browser.msie && (parseInt(jQuery.browser.version, 10) === 7);
}

function IsLeftButton(ev) {
    var bt = ev.button;
    var s = navigator.userAgent.toLowerCase() + '';
    var b = jQuery.browser;
    if (b.msie) {
        return (bt == 0) || (bt == 1);
    }
    else if (b.mozilla || b.opera || b.webkit) {
        return (bt == 0);
    }
    else {
        return (bt == 1);
    }
}

function IsRightButton(ev) {
    return (ev.button == 2);
}

function PrepareContextMenu(ev) {
    if (ev != null) {
        var menuElem = Get(currentMenuId);
        if (menuElem != null) {
            var s = menuSettings[currentMenuId];
            if (s.horizontal == "Cursor") {
                x = GetMouseX(ev) + 1;
            }
            if (s.vertical == "Cursor") {
                y = GetMouseY(ev) + 1;
            }

            show = false;
            menuButtons = s.button;
            if (((menuButtons == "Right") || (menuButtons == "Both")) && (IsRightButton(ev))) {
                show = true;
            }
            if (((menuButtons == "Left") || (menuButtons == "Both")) && (IsLeftButton(ev))) {
                show = true;
            }
        }
    }

    return false;
}

function StopPropagation(ev) {
    if (ev == null) {
        ev = window.event;
    }

    ev = jQuery.event.fix(ev);
    ev.stopPropagation();
    ev.preventDefault();

    return false;
}

function ReturnFalse(ev) {

    return false;
}

function CM_CheckMenuHandled() {
    if (document.body.cmHandled) {
        return (document.body.cmHandled = false);
    }

    return true;
}

function InitContextMenu(menuId, elem) {
    currentMenuId = menuId;
    if (elem != null) {
        if (!elem.contextMenuInitialized) {
            elem.contextMenuInitialized = true;
            elem.onmousedown = PrepareContextMenu;

            elem.oncontextmenu = ReturnFalse;
            document.body.oncontextmenu = CM_CheckMenuHandled;
        }
    }

    return false;
}

function GetParentRelativeOffset(el) {
    var offsetX = 0;
    var offsetY = 0;
    var parent;

    if (el != null) {
        for (parent = el; parent; parent = parent.offsetParent) {
            var jp = jQuery(parent);
            if (jp.css('position') == 'relative') {
                var off = jp.offset();
                offsetX = off.left;
                offsetY = off.top;
                break;
            }
        }
    }

    return { x: offsetX, y: offsetY };
}

function ContextMenu(menuId, elem, param, forceShow, mousex, mousey, checkRelative, ev, ctx) {
    var menuElem = Get(menuId);
    if (menuElem != null) {
        if (forceShow) {
            show = true;
        }
        if (elem != null) {
            currentElem = elem;
        }

        if (mouseOverTimers[menuId] != null) {
            clearInterval(mouseOverTimers[menuId]);
            mouseOverTimers[menuId] = null;
        }

        if (currentMenuId != null) {
            HideContextMenus(menuSettings[currentMenuId].level);
        }

        HideContextMenu(menuId);

        var s = menuSettings[menuId];

        if (window.event != null) {
            mousex = GetMouseX(window.event);
            mousey = GetMouseY(window.event);

            if (!forceShow) {
                show = false;
                var menuButtons = s.button;
                if (((menuButtons == "Right") || (menuButtons == "Both")) && IsRightButton(window.event)) {
                    show = true;
                    document.body.cmHandled = true;
                }
                if (((menuButtons == "Left") || (menuButtons == "Both")) && IsLeftButton(window.event)) {
                    show = true;
                }
            }
        }

        if ((mousex != null) && (s.horizontal == "Cursor")) {
            x = mousex;
        }
        if ((mousey != null) && (s.vertical == "Cursor")) {
            y = mousey;
        }

        var pos = jQuery(currentElem).offset();

        switch (s.vertical) {
            case "Top":
                y = pos.top;
                break;

            case "Bottom":
                y = pos.top + currentElem.offsetHeight;
                break;

            case "Middle":
                y = pos.top + currentElem.offsetHeight / 2;
                break;
        }

        switch (s.horizontal) {
            case "Left":
                x = pos.left;
                if (s.rtl) {
                    if (IsIE7()) {
                        // Icon offset of content tree menu for IE7
                        if (menuId == "nodeMenu" && menuLevel == 0) {
                            x += 16;
                            break;
                        }
                    }
                    x += currentElem.offsetWidth;
                }
                break;

            case "Right":
                x = pos.left;
                if (!s.rtl) {
                    x += currentElem.offsetWidth;
                }
                break;

            case "Center":
                x = pos.left + currentElem.offsetWidth / 2;
                break;

            case "Cursor":
                if (!s.rtl) {
                    x++;
                }
                else {
                    x--;
                }
                break;
        }

        if (s.rtl) {
            x -= s.offsetx;
        }
        else {
            x += s.offsetx;
        }
        y += s.offsety;

        if (show) {
            SetContextMenuParameter(menuId, param);
            var isDynamicMenu = (dynamicMenus[menuId] != null);
            if (isDynamicMenu) {
                var loading = loadingContent[menuId];
                if (loading != null) {
                    menuElem.innerHTML = loading;
                }
                else {
                    menuElem.innerHTML = "";
                }
                dynamicMenus[menuId](param);
            }

            var jm = jQuery(menuElem);

            if (ctx != null) {
                if (ctx.hideClass != null) {
                    var he = jm.find("." + ctx.hideClass);
                    he.hide();
                }
                if (ctx.showClass != null) {
                    var se = jm.find("." + ctx.showClass);
                    se.show();
                }
            }

            menuElem.style.display = 'block';
            menuElem.style.zIndex = lastMenuZindex++;

            // Reposition to the body
            if (s.level > 0) {
                var b = document.body;
                if (menuElem.parentNode != b) {
                    jQuery(menuElem).detach().appendTo("body");
                    repositionedMenus[menuId] = menuElem;
                }
            }

            menuElem.s = s;
            openMenus.push(menuElem);

            menuElem.currentElem = currentElem;
            menuElem.absOff = GetParentRelativeOffset(menuElem);

            menuElem.y = y;
            CM_ApplyY(menuElem);

            menuElem.x = x;
            CM_ApplyX(menuElem);

            var jMenu = jQuery(menuElem);
            jMenu.hide().fadeIn('fast');
            jMenu.addClass(s.rtl ? 'RTL' : 'LTR');

            if (!isDynamicMenu) {
                CM_SetHideTimeout(menuId);
                CM_Visible(menuElem.id);
            }
            var activeElem = currentElem;

            // Do not change container class
            var updateCss = true;
            for (var lvl = 0; lvl < s.activecssoffset; lvl++) {
                if (activeElem.parentNode != null) {
                    activeElem = activeElem.parentNode;
                }
                else {
                    updateCss = false;
                }
            }
            if ((s.activecss != '') && updateCss) {
                if (s.inactivecss == '') {
                    s.inactivecss = activeElem.className;
                }
                activeElem.oldClassName = s.inactivecss;
                activeElem.className = s.activecss;
            }
            menuElements[menuId] = currentElem;

            if (ev != null) {
                return StopPropagation(ev);
            }
        }
    }
}

function CM_Init(menuId) {
    var menuElem = repositionedMenus[menuId];
    if ((menuElem != null) && (menuElem.parentNode != null)) {
        menuElem.parentNode.removeChild(menuElem);
    }
}

function CM_ApplyY(menuElem) {
    var y = menuElem.y;
    var absOff = menuElem.absOff;
    var s = menuElem.s;
    var c = menuElem.currentElem;

    if (s.up) {
        y -= menuElem.offsetHeight;
    }

    if (absOff != null) {
        y -= absOff.y;
    }

    var bot = y + menuElem.offsetHeight;
    var top = y;

    var sy = window.scrollY;
    if (sy == null) sy = document.documentElement.scrollTop;
    var maxy = document.documentElement.offsetHeight + sy;
    if (bot > maxy) {
        var yOff = 0;
        switch (s.vertical) {
            case "Bottom":
                yOff = menuElem.offsetHeight + c.offsetHeight;
                break;

            default:
                yOff = menuElem.offsetHeight;
                break;
        }
        y -= yOff;
        top -= yOff;
    }
    if (top < 0) {
        y -= top;
    }

    menuElem.style.top = y + 'px';
}

function CM_ApplyX(menuElem) {
    var x = menuElem.x;
    var absOff = menuElem.absOff;
    var s = menuElem.s;
    var c = menuElem.currentElem;

    if (s.rtl) {
        x -= menuElem.offsetWidth;
    }

    if (absOff != null) {
        x -= absOff.x;
    }

    var right = x + menuElem.offsetWidth;
    var left = x;

    var sx = window.scrollX;
    if (sx == null) sx = document.documentElement.scrollLeft;
    var maxx = document.documentElement.offsetWidth + sx;
    if (right > maxx) {
        var xOff = menuElem.offsetWidth;
        x -= xOff;
        left -= xOff;
    }
    if (left < 0) {
        x -= left;
    }

    menuElem.style.left = x + 'px';
}

function CM_Visible(menuId) {
    var visibleHandler = eval('window.' + menuId + '_ContextMenuVisible');
    if (visibleHandler) {
        eval('window.' + menuId + '_ContextMenuVisible();');
    }
}

function HideContextMenu(menuId, hideSubmenus) {
    var menuElem = Get(menuId);
    if (menuElem != null) {
        if (menuTimers[menuId] != 'undefined') {
            var s = menuSettings[menuId];
            if (hideSubmenus) {
                var menuLevel = s.level;
                HideContextMenus(menuLevel + 1);
            }

            CM_ClearHideTimeout(menuId);

            if (mouseOverTimers[menuId] != null) {
                clearInterval(mouseOverTimers[menuId]);
                mouseOverTimers[menuId] = null;
            }
            if (menuElements[menuId] != null) {

                var activecssoffset = s.activecssoffset;
                var activeElem = menuElements[menuId];
                // Do not change container class
                var updateCss = true;
                for (var lvl = 0; lvl < activecssoffset; lvl++) {
                    if (activeElem.parentNode != null) {
                        activeElem = activeElem.parentNode;
                    }
                    else {
                        updateCss = false;
                    }
                }
                if ((s.activecss != '') && updateCss) {
                    activeElem.className = (activeElem.oldClassName != null) ? activeElem.oldClassName : '';
                }

                menuElements[menuId] = null;
            }

            jQuery(menuElem).hide().fadeOut('fast');
        }
    }
}

function SetContextMenuParameter(menuId, param) {
    menuParameters[menuId] = param;
    var paramElem = Get(menuId + '_parameter');
    if (paramElem != null) {
        paramElem.value = param;
    }

    if (typeof (OnSetContextMenuParameter) == 'function') {
        OnSetContextMenuParameter(menuId, param);
    }

    return null;
}

function GetContextMenuParameter(menuId) {
    return menuParameters[menuId];

    var paramElem = Get(menuId + '_parameter');
    if (paramElem != null) {
        return paramElem.value;
    }
    return null;
}

function CM_Receive(rvalue, context) {
    var menuElem = repositionedMenus[context];

    if (menuElem == null) {
        menuElem = Get(context);
    }

    jQuery(menuElem).html(rvalue);

    CM_ApplyX(menuElem);
    CM_ApplyY(menuElem);

    CM_Visible(context);
    CM_SetHideTimeout(context);
}

function CM_ClearHideTimeout(menuId) {
    if (menuTimers[menuId] != null) {
        clearTimeout(menuTimers[menuId]);
        menuTimers[menuId] = null;
    }
}

function CM_SetHideTimeout(context, hideSubMenus) {
    menuTimers[context] = setTimeout("HideContextMenuTimeout('" + context + "', " + hideSubMenus + ");", 2000);
}

function HideContextMenuTimeout(menuId, hideSubMenus) {
    var s = menuSettings[menuId];
    if (CM_IsCursorOnMenu(s.level)) {
        CM_SetHideTimeout(menuId, hideSubMenus);
        return;
    }

    HideContextMenu(menuId, true);
}

function CM_Close(menuId) {
    HideContextMenu(menuId, true);
}

function HideContextMenus(startLevel) {
    for (i = openMenus.length - 1; i >= startLevel; i--) {
        var menu = openMenus[i];
        if (menu != null) {
            if (menu.s.level >= startLevel) {
                menu.style.display = 'none';
                openMenus[i] = null;
            }
        }
    }
}

function CM_IsCursorOnMenu(level) {
    for (i = level; i < mouseOverLevel.length; i++) {
        if (mouseOverLevel[i]) {
            return true;
        }
    }

    return false;
}

function HideAllContextMenus() {
    if (!CM_IsCursorOnMenu(10)) {
        HideContextMenus(0);
    }
}

function CM_MenuOver(level) {
    mouseOverLevel[level] = true;
}

function CM_MenuOut(level) {
    mouseOverLevel[level] = false;
}

function CM_Out(ev, menuId) {
    if (ev == null) {
        ev = window.event;
    }

    if (!ev.cmHandled) {
        if (mouseOverTimers[menuId] != null) {
            clearInterval(mouseOverTimers[menuId]);
            mouseOverTimers[menuId] = null;
        }

        ev.cmHandled = true;
    }

    return false;
}

function CM_Over(ev, menuId, elem, param, getCtx) {
    if (ev == null) {
        ev = window.event;
    }

    InitContextMenu(menuId, elem, getCtx);

    if (!ev.cmHandled) {
        var menuElem = Get(menuId);
        if (menuElem != null) {
            var s = menuSettings[menuId];
            if (s.mouseover) {
                currentElem = elem;

                if ((Get(menuId).style.display != 'none') && (timerParameters[menuId] != param)) {
                    HideContextMenu(menuId, true);
                }

                if (Get(menuId).style.display == 'none') {
                    timerParameters[menuId] = null;
                    if (mouseOverTimers[menuId] == null) {
                        timerParameters[menuId] = param;
                        var events = "";
                        if (window.event != null) {
                            events += ", true";
                            events += ", " + GetMouseX(window.event);
                            events += ", " + GetMouseY(window.event);
                        }
                        mouseOverTimers[menuId] = setInterval("ContextMenu('" + menuId + "', null, '" + param + "'" + events + ");", 1000);
                    }
                }
            }
        }

        ev.cmHandled = true;
    }

    return false;
}

function CM_Up(ev, menuId, elem, param, checkRelative, getCtx) {
    if (ev == null) {
        ev = window.event;
    }

    if (!ev.cmHandled) {
        var ctx = (getCtx != null ? getCtx(elem) : null);
        ContextMenu(menuId, elem, param, null, null, null, checkRelative, ev, ctx);

        ev.cmHandled = true;
    }

    return false;
}

function RegisterAsyncCloser(closer) {
    if ((typeof (Sys) != 'undefined') && (typeof (Sys.WebForms) != 'undefined') && (typeof (Sys.WebForms.PageRequestManager) != 'undefined')) {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(closer);
    }
}

if (typeof (Sys) != "undefined" && typeof (Sys.Application) != "undefined") {
    Sys.Application.notifyScriptLoaded();
}