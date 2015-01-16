var CMSUniMenu = {
    FxSpeed: 200,
    MenuDelay: 700,
    SubMenuTimes: {},
    SubMenus: {},

    InitializeButtonWidth: function () {
        $j('.SmallButton').each(function () {
            var j = $j(this), w = j.parentsUntil('td').first().innerWidth();
            j.parentsUntil('table').first().find('.MiddleSmallButton').width(w - 6);
        });
    },

    SelectMenuButton: function (buttonName) {
        if (typeof (SelectButton) === 'function') {
            SelectButton($j('div[name=' + buttonName + ']').get(0));
        }
    },

    ToogleSubMenu: function (button) {
        btn = $j(button), subItems = btn.find('.SubMenuItems');

        if (!btn.data('pos_fixed')) {
            btn.parents('.ui-layout-pane').first().css({ 'overflow': 'visible', 'z-index': '10' });
            btn.data('pos_fixed', 'true');

            var pos = btn.position();
            var border = subItems.outerWidth() - subItems.innerWidth();

            subItems.css({
                'position': 'absolute',
                'z-index': '11',
                'top': pos.top + btn.outerHeight(),
                'min-width': btn.innerWidth() - border
            });
            if ($j(document.body).hasClass('RTL') && (subItems.parents('.LTR').length == 0)) {
                subItems.css({ 'left': pos.left - subItems.outerWidth() + btn.outerWidth() });
            } else {
                subItems.css({ 'left': pos.left });
            }
        }

        // Save sub menu container
        CMSUniMenu.SubMenus[button.id] = subItems;

        if (subItems.is(':hidden')) {
            CMSUniMenu.ShowSubMenu(button.id);
        }
        else {
            CMSUniMenu.HideSubMenu(button.id);
        }
    },

    ShowSubMenu: function (btnId) {
        // Bind enter/leave events
        $j('#' + btnId).mouseleave(function () {
            if (!CMSUniMenu.SubMenus[btnId].is(':hidden')) {
                CMSUniMenu.SubMenuTimes[btnId] = setTimeout(function () { CMSUniMenu.HideSubMenu(btnId); }, CMSUniMenu.MenuDelay);
            }
        }).mouseenter(function () {
            if (!CMSUniMenu.SubMenus[btnId].is(':hidden')) {
                clearTimeout(CMSUniMenu.SubMenuTimes[btnId]);
            }
        });

        // Show sub menu
        CMSUniMenu.SubMenus[btnId].slideDown(CMSUniMenu.FxSpeed);

        // Relocate slider buttons
        var scrollers = jQuery('.ForwardScroller, .BackwardScroller', CMSUniMenu.SubMenus[btnId]);
        scrollers.each(function () {
            var scrollerObj = jQuery(this);
            var scrollerWidth = scrollerObj.width();
            var parentWidth = scrollerObj.parent().width();
            scrollerObj.css("left", (parentWidth / 2) - (scrollerWidth / 2));
            scrollPanelInit(jQuery('.ContextMenuContainer', CMSUniMenu.SubMenus[btnId]).attr('id'), true);
        });

    },

    HideSubMenu: function (btnId) {
        // Unbind current events
        btn = $j('#' + btnId).unbind('mouseleave').unbind('mouseenter');
        // Hide sub menu
        CMSUniMenu.SubMenus[btnId].slideUp(CMSUniMenu.FxSpeed);
    },

    ChangeButton: function (button, text, iconUrl) {
        if (button) {
            var jButton = $j(button);
            if (iconUrl) {
                jButton.find('img.MenuButtonImage').attr('src', iconUrl);
            }

            if (text) {
                jButton.find('span.MenuButtonText').text(text);
            }
        }
    }
}

$j(function () {
    CMSUniMenu.InitializeButtonWidth();
});