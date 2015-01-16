var webPartLocation = new Array();
var webPartZoneLocation = new Array();
var placeholderProperties = new Array();

var bordersToDeactivate = new Array();
var borderDeactivationEnabled = true;
var refreshPageLocationUrl = null;

window.useDraggedClass = true;

// General
function Get(id) {
    return document.getElementById(id);
}

function SetVal(id, val) {
    var elem = Get(id); if (elem != null) { elem.value = (val != null ? val : ''); }
}

function WP_GetMenuContext(el) {
    while (el != null) {
        if (el.className.indexOf("WebPartZone") >= 0) {
            break;
        }
        else if (el.className.indexOf("FreeLayout") >= 0) {
            return { hideClass: "NormalLayoutMenu", showClass: "FreeLayoutMenu" }
        }

        el = el.parentNode;
    }

    return { hideClass: "FreeLayoutMenu", showClass: "NormalLayoutMenu" }
}

function WP_ContextMenu(wp, param) {
    var p = wp.parentNode.parentNode.parentNode.parentNode;
    var ctx = WP_GetMenuContext(p);
    ContextMenu('webPartMenu', p, param, true, null, null, null, null, ctx); 
    return false;
}

function WG_ContextMenu(wg, param) {
    var p = wg.parentNode.parentNode.parentNode.parentNode;
    ContextMenu('widgetMenu', p, param, true); 
    return false;
}

function getZoneVariantId(zoneId) {
    return ((typeof (window.GetCurrentZoneVariantId) == 'function') ? GetCurrentZoneVariantId(zoneId) : 0);
}

// Web parts
function MoveWebPartAsync(webPartZoneId, webPartId, aliasPath, instanceGuid, targetZoneId, targetPosition, elem) {
    if (elem.keepPosition) {
        targetPosition = '';
    }
    var param = 'MoveWebPartAsync\n' + webPartZoneId + '\n' + webPartId + '\n' + aliasPath + '\n' + instanceGuid + '\n' + targetZoneId + '\n' + targetPosition + '\n' + getZoneVariantId(webPartZoneId) + '\n' + getZoneVariantId(targetZoneId);

    if (elem.flowLayout) {
        param += '\n' + elem.flowX + '\n' + elem.flowY;
    }

    Get('pageChanged').value = 1;
    PM_Callback(param, OnWebPartMoved, elem);
}

function SetWebPartProperty(clientId, propertyName, value, line) {
    var loc = webPartLocation[clientId + '_container'];
    value = value.replace(/[\r\n]+/g, '[\\n]');
    var param = 'SetWebPartProperty\n' + loc[0] + '\n' + loc[1] + '\n' + loc[2] + '\n' + loc[3] + '\n' + loc[5] + '\n' + propertyName + '\n' + value + '\n' + line;
    Get('pageChanged').value = 1;
    PM_Callback(param, OnReceiveScript);
}

function AddToWebPartProperty(clientId, propertyName, value, refresh) {
    var loc = webPartLocation[clientId + '_container'];
    var param = 'AddToWebPartProperty\n' + loc[0] + '\n' + loc[1] + '\n' + loc[2] + '\n' + loc[3] + '\n' + loc[5] + '\n' + propertyName + '\n' + value + '\n' + refresh;
    Get('pageChanged').value = 1;
    PM_Callback(param, OnReceiveScript);
}

function AddWebPart(webPartId) {
    setWebPart(webPartId);
    PM_Postback('AddWebPart');
}

function NewWebPart(webPartZoneId, aliasPath, templateId, layoutZone) {
    setZone(webPartZoneId);
    setAliasPath(aliasPath);
    setTemplate(templateId);
    isLayoutZone = !!layoutZone;
    setIsLayoutZone(isLayoutZone);
    modalDialog(webPartsPath + 'WebPartSelector.aspx' + (window.isWireframe ? '?wireframe=true' : ''), 'selectwebpart', '90%', '85%');
}

function AddWebPartWithoutDialog(webPartId, webPartZoneId, isLayoutZone, position) {
    setWebPart(webPartId);
    setTargetZone(webPartZoneId);
    setTargetPosition(position);
    setIsLayoutZone(isLayoutZone);
    PM_Postback('AddWebPartWithoutDialog');
}

function ConfigureWebPartZone(webPartZoneId, aliasPath, templateId, layoutZone) {
    var variantId = 0;
    if (typeof (GetCurrentVariantId) == 'function') {
        variantId = GetCurrentVariantId('Variant_Zone_' + webPartZoneId);
    }

    var type = '';
    if (typeof (GetCurrentObjectVariantMode) == 'function') {
        type = GetCurrentObjectVariantMode('Variant_Zone_' + webPartZoneId);
        type = (type != '') ? '&variantmode=' + type : '';
    }

    modalDialog(webPartsPath + 'WebPartZoneProperties.aspx?aliaspath=' + aliasPath + '&culture=' + getCulture() + '&zoneid=' + webPartZoneId + '&templateid=' + templateId + '&layoutzone=' + layoutZone + type + ((variantId > 0) ? '&variantid=' + variantId : ''), 'configurewebpartzone', 800, 650);
}

function RemoveAllWebParts(webPartZoneId, aliasPath) {
    if (confirm(confirmRemoveAll)) {
        setZone(webPartZoneId); setAliasPath(aliasPath);
        PM_Postback('RemoveAllWebParts');
    }
}

function MoveAllWebParts(webPartZoneId, aliasPath, targetZoneId) {
    setZone(webPartZoneId); setAliasPath(aliasPath); setTargetZone(targetZoneId);
    PM_Postback('MoveAllWebParts');
}

function OnSelectWebPart(webPartId, skipDialog) {
    if (skipDialog) {
        AddWebPartWithoutDialog(webPartId, getZone(), getIsLayoutZone(), -1);
    }
    else {
        modalDialog(webPartsPath + 'WebPartProperties.aspx?aliaspath=' + getAliasPath() + '&culture=' + getCulture() + '&templateid=' + getTemplate() + '&zoneid=' + getZone() + '&zonevariantid=' + getZoneVariantId(getZone()) + '&webpartid=' + webPartId + '&layoutzone=' + getIsLayoutZone() + '&isnew=1&', 'configurewebpart', 900, 638);
    }
}

function MoveWebPart(webPartZoneId, webPartId, aliasPath, instanceGuid, targetZoneId, targetPosition) {
    setZone(webPartZoneId);
    setWebPart(webPartId);
    setAliasPath(aliasPath);
    setGuid(instanceGuid);
    setTargetZone(targetZoneId); 
    setTargetPosition(targetPosition);
    PM_Postback('MoveWebPart');
}

function trim(s) {
    if (s == null) return s;
    return s.replace(/^\s+|\s+$/g, "");
}

function MoveElemDown(el, bottom) {
    if (bottom) {
        var p = el.parentNode;
        p.removeChild(el);
        p.appendChild(el);
        return;
    }

    var next = el.nextSibling;
    while (next != null) {
        if (trim(next.className) == trim(el.className)) {
            var par = el.parentNode;
            next = next.nextSibling;
            par.removeChild(el);
            if (next != null) par.insertBefore(el, next);
            else par.appendChild(par);
            break;
        }
        next = next.nextSibling;
    }
}

function MoveWebPartUp(webPartZoneId, webPartId, aliasPath, instanceGuid, elId, top) {
    var param = '\n' + webPartZoneId + '\n' + webPartId + '\n' + aliasPath + '\n' + instanceGuid;
    var op = (top ? 'MoveWebPartTop' : 'MoveWebPartUp');

    var el = Get(elId + "_container");
    if (el.style.position == 'relative') {
        MoveElemDown(el, top);
        op = (top ? 'MoveWebPartBottom' : 'MoveWebPartDown');
    }
    else {
        MoveElemUp(el, top);
    }

    PM_Callback(op + param, OnReceiveScript);
}

function MoveElemUp(el, top) {
    if (top) {
        var p = el.parentNode;
        p.removeChild(el);
        var first = p.firstChild;
        if ((first != null) && (first != el)) {
            p.insertBefore(el, first);
        }
        else {
            p.appendChild(el);    
        }
        return;
    }

    var prev = el.previousSibling;
    while (prev != null) {
        if (trim(prev.className) == trim(el.className)) {
            var par = el.parentNode;
            par.removeChild(el);
            par.insertBefore(el, prev);
            break;
        }
        prev = prev.previousSibling;
    }
}

function MoveWebPartDown(webPartZoneId, webPartId, aliasPath, instanceGuid, elId, bottom) {
    var param = '\n' + webPartZoneId + '\n' + webPartId + '\n' + aliasPath + '\n' + instanceGuid;
    var op = (bottom ? 'MoveWebPartBottom' : 'MoveWebPartDown');

    var el = Get(elId + "_container");
    if (el.style.position == 'relative') {
        MoveElemUp(el, bottom);
        op = (top ? 'MoveWebPartTop' : 'MoveWebPartUp');
    }
    else {
        MoveElemDown(el, bottom);
    }

    PM_Callback(op + param, OnReceiveScript);
}

function CloneWebPart(webPartZoneId, webPartId, aliasPath, instanceGuid) {
    setZone(webPartZoneId); setWebPart(webPartId); setAliasPath(aliasPath); setGuid(instanceGuid);
    PM_Postback('CloneWebPart');
}

function RemoveWebPart(webPartZoneId, webPartId, aliasPath, instanceGuid) {
    if (confirm(confirmRemove)) {
        setZone(webPartZoneId); setWebPart(webPartId); setAliasPath(aliasPath); setGuid(instanceGuid);
        PM_Postback('RemoveWebPart');
    }
}

function OnAddWebPartVariant(query) {
    modalDialog(webPartsPath + 'WebPartProperties.aspx' + query + '&isnewvariant=true', 'configurewebpart', 900, 638);
}

function OnAddWebPartZoneVariant(query) {
    modalDialog(webPartsPath + 'WebPartZoneProperties.aspx' + query + '&isnewvariant=true', 'configurewebpartzone', 800, 650);
}

function ConfigureWebPart(webPartZoneId, webPartId, aliasPath, instanceGuid, templateId, refreshOnCancel) {
    var variantId = 0;
    var zoneVariantId = 0;
    if (typeof (GetCurrentVariantId) == 'function') {
        variantId = GetCurrentVariantId('Variant_WP_' + instanceGuid.replace(/-/g, ''));
        zoneVariantId = GetCurrentVariantId('Variant_Zone_' + webPartZoneId);
    }

    modalDialog(webPartsPath + 'WebPartProperties.aspx?aliaspath=' + aliasPath + '&culture=' + getCulture() + '&zoneid=' + webPartZoneId + '&webpartid=' + webPartId + '&instanceguid=' + instanceGuid + '&templateid=' + templateId + ((variantId > 0) ? '&variantid=' + variantId : '') + ((zoneVariantId > 0) ? '&zonevariantid=' + zoneVariantId : '') + (!!refreshOnCancel ? '&refreshoncancel=true' : ''), 'configurewebpart', 900, 638);
}

// Template
function EditTemplate(aliasPath) {
    modalDialog(editTemplateUrl + '?templateId=' + placeholderProperties[aliasPath]['templateId'] + '&aliaspath=' + aliasPath + '&dialog=1', 'edittemplate', 850, 680);
}

function CloneTemplate(aliasPath) {
    setAliasPath(aliasPath);
    PM_Postback('CloneTemplate');
}

// Widgets
function NewWidget(widgetZoneId, aliasPath, templateId, zoneType, layoutZone) {
    setZone(widgetZoneId); setZoneType(zoneType); setAliasPath(aliasPath); setTemplate(templateId); isLayoutZone = !!layoutZone; setIsLayoutZone(isLayoutZone);
    modalDialog(widgetSelectorUrl + '?aliaspath=' + aliasPath + '&culture=' + getCulture() + '&templateid=' + templateId + '&zoneid=' + widgetZoneId + '&zonetype=' + zoneType + '&layoutzone=' + layoutZone, 'selectwidget', '90%', '85%');
}

function AddWidgetWithoutDialog(widgetId, widgetZoneId, isLayoutZone) {
    setWebPart(widgetId);
    setTargetZone(widgetZoneId);
    setIsLayoutZone(isLayoutZone);
    PM_Postback('AddWidgetWithoutDialog');
}

function OnSelectWidget(widgetId, skipDialog) {
    if (skipDialog) {
        AddWidgetWithoutDialog(widgetId, getZone(), getIsLayoutZone());
    }
    else {
        modalDialog(widgetPropertiesUrl + '?aliaspath=' + getAliasPath() + '&culture=' + getCulture() + '&templateid=' + getTemplate() + '&zoneid=' + getZone() + '&zonetype=' + getZoneType() + '&widgetid=' + widgetId + '&layoutzone=' + getIsLayoutZone() + '&isnew=1' + getDashBoardParameter(), 'configurewidget', 900, 600);
    }
}

function ConfigureWidget(widgetZoneId, widgetId, aliasPath, instanceGuid, templateId) {
    var variantId = 0;
    var zoneVariantId = 0;
    if (typeof (GetCurrentVariantId) == 'function') {
        variantId = GetCurrentVariantId('Variant_WP_' + instanceGuid.replace(/-/g, ''));
    }

    modalDialog(widgetPropertiesUrl + '?aliaspath=' + aliasPath + '&culture=' + getCulture() + '&templateid=' + templateId + '&zoneid=' + widgetZoneId + '&widgetid=' + widgetId + '&instanceguid=' + instanceGuid + ((variantId > 0) ? '&variantid=' + variantId : '') + getDashBoardParameter(), 'configurewidget', 900, 600);
}

function RemoveWidget(webPartZoneId, webPartId, aliasPath, instanceGuid) {
    if (confirm(confirmRemoveWidget)) {
        setZone(webPartZoneId); setWebPart(webPartId); setAliasPath(aliasPath); setGuid(instanceGuid);
        PM_Postback('RemoveWebPart');
    }
}

function RemoveAllWidgets(webPartZoneId, aliasPath) {
    if (confirm(confirmRemoveAllWidgets)) {
        setZone(webPartZoneId); setAliasPath(aliasPath);
        PM_Postback('RemoveAllWebParts');
    }
}

function ToggleMinimizeWidget(elem, id, widgetZoneId, widgetId, aliasPath, instanceGuid) {
    var param = '\n' + widgetZoneId + '\n' + widgetId + '\n' + aliasPath + '\n' + instanceGuid;
    containerElem = Get(id + '_container');
    // Inner container
    containerElem = containerElem.firstChild;
    if (!/ MinimizedWidget/.test(containerElem.className)) {
        param = 'MinimizeWidget' + param;
        containerElem.className += ' MinimizedWidget';
        elem.src = elem.src.replace(/minimize/i, 'maximize');
        elem.title = maximizeWidget;
        PM_Callback(param, WidgetStateChanged, containerElem);
    } else {
        param = 'MaximizeWidget' + param;
        containerElem.className = containerElem.className.replace(/ MinimizedWidget/, '');
        elem.src = elem.src.replace(/maximize/i, 'minimize');
        elem.title = minimizeWidget;
        PM_Callback(param, WidgetStateChanged, containerElem);
    }
}

function WidgetStateChanged(rvalue, context) { }

function CannotModifyUserWidgets() {
    alert(cannotModifyWidgets);
}

function OnBeforeDropWebPart(sender, e) {
    var container = e.get_container();
    var item = e.get_droppedItem();
    var position = e.get_position();

    if (typeof (BeforeDropWebPart) == 'function') {
        BeforeDropWebPart(container, item, position);
    }
}

function OnDropWebPart(sender, e) {
    var container = e.get_container();
    var item = e.get_droppedItem();
    var position = e.get_position();

    item.originalClassName = item.className;

    if (window.useDraggedClass) {
        item.originalClassName = item.originalClassName.replace("DraggedWebPart", "");
    }

    item.className = item.className + ' WebPartUpdating';

    var uid = item.getAttribute('id');
    var loc = webPartLocation[uid];

    var zid = container.getAttribute('id');
    var targetzoneid = webPartZoneLocation[zid][0];

    if ((typeof (loc) != 'undefined') && (loc != null)) {
        DeactivateBorder(uid.replace('_container', ''), 0);

        if (typeof (OnDropWebPartVariants) == 'function') {
            OnDropWebPartVariants(uid, targetzoneid, loc[1], loc[2], loc[3], loc[4]);
        }

        MoveWebPartAsync(loc[0], loc[1], loc[2], loc[3], targetzoneid, position, item);
    }
    else {
        var pnlWebPart = container.getAttribute('id');
        var webPartLoc = webPartZoneLocation[pnlWebPart];

        var webPartZoneId = webPartLoc[0];
        var aliaspath = webPartLoc[1];
        var templateId = webPartLoc[2];
        var isLayoutZone = !!webPartLoc[4];
        var webPartId = item.getAttribute('data-webpartid');
        var zoneVariantId = getZoneVariantId(webPartZoneId);

        if (item.flowLayout) {
            item.flowX += document.documentElement.scrollLeft;
            item.flowY += document.documentElement.scrollTop;

            position += '|' + item.flowX + '|' + item.flowY;
        }

        if (item.getAttribute('data-skipdialog') != 1) {
            modalDialog(webPartsPath + 'WebPartProperties.aspx?aliaspath=' + aliaspath + '&culture=' + getCulture() + '&templateid=' + templateId + '&zoneid=' + webPartZoneId + ((zoneVariantId > 0) ? '&zonevariantid=' + zoneVariantId : '') + '&webpartid=' + webPartId + '&layoutzone=' + isLayoutZone + '&isnew=1&position=' + position, 'configurewebpart', 900, 638);
        }
        else {
            AddWebPartWithoutDialog(webPartId, webPartZoneId, isLayoutZone, position);
        }
    }
}

function OnWebPartMoved(rvalue, context) {
    if ((rvalue != null) && (rvalue != '')) {
        var values = rvalue.split('<#>');
        switch (values[0]) {
            case 'U': window.top.location.replace(window.top.location.href); break;
            case 'R': RefreshPage(true); break;
            case 'E': alert(values[1]); break;
            case 'ER': alert(values[1]); RefreshPage(); break;
            case 'CK': if (typeof (CKRenameWidgetTextareas) == 'function') { CKRenameWidgetTextareas(values[1], values[2]); } break;
        }
    }
    context.className = context.originalClassName;
}

function OnReceiveScript(rvalue, context) {
    if ((rvalue != null) && (rvalue != '')) {
        setTimeout(rvalue, 1);
    }
}

function UpdateRefreshPageUrlParam(param, value) {
    if ((typeof (refreshPageLocationUrl) == 'undefined') || (refreshPageLocationUrl == null)) {
        refreshPageLocationUrl = document.location + '';
    }

    refreshPageLocationUrl = jQuery.param.querystring(refreshPageLocationUrl, param + "=" + value);
}

function RefreshPage(usePostback) {
    if (usePostback) {
        PM_Postback('Refresh');
    }
    else {
        var url = document.location + '';
        if ((typeof (refreshPageLocationUrl) != 'undefined') && (refreshPageLocationUrl != null)) {
            url = refreshPageLocationUrl;
        }
        document.location.replace(url.replace('safemode=1', 'safemode=0'));
    }
}

function RefreshWOpener(window) {
    if (window.refreshPageOnClose) {
        RefreshPage();
    }
}

function ActivateBorder(id, elem) {
    if ((elem != null) && (elem.onselectstart == null)) {
        elem.onselectstart = function () { return false; };
    }

    ClearBorderDeactivation(id);

    var e = Get(id + '_border');
    if (e != null) {
        if ((e.className.indexOf('WebPartZoneBorder') != -1) || (e.className.indexOf('WebPartBorder') != -1)) {
            e.className = ' ' + e.className + ' ';
            e.className = e.className.replace(' WebPartZoneBorder ', ' WebPartZoneBorderActive ');
            e.className = e.className.replace(' WebPartBorder ', ' WebPartBorderActive ');
            // Trim end
            e.className = e.className.replace(/^\s+|\s+$/g, '');
        }
    }
}

function DeactivateBorder(id, time) {
    if (borderDeactivationEnabled) {
        if (time <= 0) {
            bordersToDeactivate[id] = null;
            var e = Get(id + '_border');
            if (e != null) {
                if ((e.className.indexOf('WebPartZoneBorder') != -1) || (e.className.indexOf('WebPartBorder') != -1)) {
                    e.className = e.className.replace('WebPartZoneBorderActive', ' WebPartZoneBorder ');
                    e.className = e.className.replace('WebPartBorderActive', ' WebPartBorder ');

                    // Trim end
                    e.className = e.className.replace(/^\s+|\s+$/g, '');
                }
            }
        } else {
            ClearBorderDeactivation(id);
            bordersToDeactivate[id] = setTimeout("DeactivateBorder('" + id + "', 0)", time);
        }
    }
}

function ClearBorderDeactivation(id) {
    var toDeactivate = bordersToDeactivate[id];
    if (toDeactivate != null) {
        bordersToDeactivate[id] = null;
        clearTimeout(toDeactivate);
    }
}
