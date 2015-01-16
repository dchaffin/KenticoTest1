var CMSViewValidate = {
    PreviewWidth: 0,
    PreviewHeight: 0,
    Rotated: false,
    RotationCheckboxInitialized: false,
    BodyClassInitialized: false,
    FrameScrollPane: null,
    ScrollSize: 16,
    ViewElem: [],
    ValidateElem: [],
    DeviceElem: [],
    TabsHeaderElem: [],
    Separ: [],
    HeaderLine: [],
    ResizeContentArea: function (tabID) {
        CMSViewValidate.Separ = $j('.HeaderSeparator').filter(':visible');
        if (!CMSViewValidate.TabsHeaderElem.length) {
            CMSViewValidate.TabsHeaderElem = $j('.ui-tabs-nav');
        }
        if (!CMSViewValidate.ViewElem.length) {
            CMSViewValidate.ViewElem = $j('#pageview');
        }
        if (!CMSViewValidate.DeviceElem.length) {
            CMSViewValidate.DeviceElem = $j('.DeviceFrame');
        }
        if (!CMSViewValidate.ValidateElem.length) {
            CMSViewValidate.ValidateElem = $j('#pagevalidate');
        }

        if (!CMSViewValidate.HeaderLine.length) {
            CMSViewValidate.HeaderLine = $j('.CMSFixPanel');
        }

        if (CMSViewValidate.TabsHeaderElem.length && CMSViewValidate.Separ.length) {
            var newHeight = (document.body.offsetHeight - CMSViewValidate.TabsHeaderElem.outerHeight() - CMSViewValidate.Separ.outerHeight());

            if (CMSViewValidate.ViewElem.length) {
                if (!CMSViewValidate.ViewElem.parent().hasClass('jspPane')) {
                    CMSViewValidate.ViewElem.height(newHeight);
                }
                else {
                    CMSViewValidate.DeviceElem.height(newHeight);
                }
            }

            if (CMSViewValidate.ValidateElem.length) {
                CMSViewValidate.ValidateElem.height(newHeight);
            }
        }
        else {
            if (CMSViewValidate.HeaderLine.length) {
                CMSViewValidate.DeviceElem.height(document.body.offsetHeight - CMSViewValidate.HeaderLine.outerHeight());
            }
        }
    },
    InitializeFrame: function (width, height, rotate) {
        var deviceFrame = $j('.DeviceFrame');

        var topLine = deviceFrame.find('.TopLine');
        var centerLine = deviceFrame.find('.CenterLine');
        var bottomLine = deviceFrame.find('.BottomLine');

        var topLeft = topLine.find('.LeftPiece');
        var topCenter = topLine.find('.CenterPiece');
        var topRight = topLine.find('.RightPiece');

        var centerLeft = centerLine.find('.LeftPiece');
        var centerCenter = centerLine.find('.CenterPiece');
        var centerRight = centerLine.find('.RightPiece');

        var bottomLeft = bottomLine.find('.LeftPiece');
        var bottomCenter = bottomLine.find('.CenterPiece');
        var bottomRight = bottomLine.find('.RightPiece');

        var iFrame = centerCenter.find('iframe');

        if ((width > 0) && (height > 0)) {

            CMSViewValidate.PreviewWidth = width;
            CMSViewValidate.PreviewHeight = height;

            if (rotate) {
                this.Rotated = true;

                width = CMSViewValidate.PreviewHeight;
                height = CMSViewValidate.PreviewWidth;

                deviceFrame.addClass('Rotated');
            }
            else {
                this.Rotated = false;

                deviceFrame.removeClass('Rotated');
            }

            if (!this.BodyClassInitialized) {
                this.BodyClassInitialized = true;
                $j('body').addClass('DevicePreview');

                centerCenter.css({ 'visible': 'hidden' });
            }

            topCenter.width(width);
            bottomCenter.width(width);

            centerLeft.height(height);
            centerRight.height(height);

            centerCenter.width(width).height(height);
            iFrame.width(width).height(height).attr('scrolling', 'no').bind('load', function () {
                CMSViewValidate.FitIframe();
                CMSViewValidate.DeviceWindowResize();
            });
            centerCenter.find('.CenterPanel').width(width).height(height).css({ 'overflow': 'hidden' });

            topLine.width(topLeft.outerWidth() + topCenter.outerWidth() + topRight.outerWidth());
            centerLine.width(centerLeft.outerWidth() + centerCenter.outerWidth() + centerRight.outerWidth());
            bottomLine.width(bottomLeft.outerWidth() + bottomCenter.outerWidth() + bottomRight.outerWidth());

            this.ShowScrollbars([topLeft, topCenter, topRight, topCenter, centerLeft, centerCenter, centerRight, bottomLeft, bottomCenter, bottomRight]);
            this.InitRotationCheckbox();
            this.ResizeContentArea();
            this.FitIframe();

            $j(window).bind('resize', function () {
                CMSViewValidate.DeviceWindowResize();
            });
        }
        else {
            deviceFrame.css({ 'margin': '0', 'width': '100%', 'height': '100%' });

            topLeft.width(0).height(0);
            topCenter.width(0).height(0);
            topRight.width(0).height(0);

            centerLeft.width(0).height(0);
            centerCenter.width('100%').height('100%');
            centerRight.width(0).height(0);

            bottomLeft.width(0).height(0);
            bottomCenter.width(0).height(0);
            bottomRight.width(0).height(0);
        }
    },
    FitIframe: function () {
        try {
            var devFrame = $j('.DeviceFrame'), iFrame = devFrame.find('iframe'), centerPanel = devFrame.find('.CenterPanel');

            var w = iFrame.contents().find('html').get(0).scrollWidth;
            var h = iFrame.contents().find('html').get(0).scrollHeight;

            if ((w > 0) && (h > 0)) {

                if (h > CMSViewValidate.GetHeight()) {
                    h += CMSViewValidate.ScrollSize;
                }

                if (w > CMSViewValidate.GetWidth()) {
                    w += CMSViewValidate.ScrollSize;
                }

                iFrame.width(w).height(h);

                if (CMSViewValidate.FrameScrollPane) {
                    CMSViewValidate.FrameScrollPane.reinitialise();
                }

                centerPanel.css({ 'overflow': 'auto' }).jScrollPane({ showArrows: true, includeScrollToPaneSize: false });
                CMSViewValidate.FrameScrollPane = centerPanel.data('jsp');
                devFrame.find('.CenterPiece .jspVerticalBar').hide();
                devFrame.find('.CenterPiece .jspHorizontalBar').hide();

                if (CMSViewValidate.FrameScrollPane) {
                    iFrame.contents().find('body').unbind('mouseup').bind('mouseup', function () {
                        var frameScrollPane = null;
                        if (window.CMSViewValidate) {
                            frameScrollPane = window.CMSViewValidate.FrameScrollPane;
                        }
                        if (window.parent.CMSViewValidate) {
                            frameScrollPane = window.parent.CMSViewValidate.FrameScrollPane;
                        }
                        if (frameScrollPane) {
                            frameScrollPane.cancelDrag();
                        }
                    });
                }

                devFrame.find('.CenterPiece').css({ 'visible': 'visible' });
            }

        } catch (e) { }
    },
    ShowScrollbars: function (elem) {
        if (elem) {
            for (var i = 0; i < elem.length; i++) {
                elem[i].mouseenter(function () {
                    $j('.DeviceFrame').find('.CenterPiece .jspVerticalBar').show();
                    $j('.DeviceFrame').find('.CenterPiece .jspHorizontalBar').show();
                }).mouseleave(function () {
                    $j('.DeviceFrame').find('.CenterPiece .jspVerticalBar').hide();
                    $j('.DeviceFrame').find('.CenterPiece .jspHorizontalBar').hide();
                });
            }
        }
    },
    InitRotationCheckbox: function () {
        if (!this.RotationCheckboxInitialized) {
            this.RotationCheckboxInitialized = true;
            var rotateCheckbox = $j('.ViewRotate').find('input:checkbox');
            var cRotate = $j.cookie('CMSDeviceProfileRotate');
            rotateCheckbox.bind('change', function () {
                var checked = $j(this).attr('checked')
                $j.cookie('CMSDeviceProfileRotate', (checked === 'checked' ? '1' : '0'));
                CMSViewValidate.InitializeFrame(CMSViewValidate.PreviewWidth, CMSViewValidate.PreviewHeight, (checked === 'checked'));
                CMSViewValidate.DeviceWindowResize();
            });
        }
    },
    RotateDevice: function () {
        var cRotate = $j.cookie('CMSDeviceProfileRotate');
        $j.cookie('CMSDeviceProfileRotate', ((cRotate == 0 || cRotate == null) ? '1' : '0'));
        CMSViewValidate.InitializeFrame(CMSViewValidate.PreviewWidth, CMSViewValidate.PreviewHeight, (cRotate == 0 || cRotate == null));
        CMSViewValidate.DeviceWindowResize();
    },
    DeviceWindowResize: function () {
        var jWin = $j(window),
        devFrame = $j('.DeviceFrame'),
        winWidth = jWin.innerWidth(),
        devWidth = $j('.TopLine').outerWidth();

        if (winWidth < devWidth) {
            var devLeft = devFrame.find('.CenterLine .LeftPiece').outerWidth(),
            devRight = devFrame.find('.CenterLine .RightPiece').outerWidth();

            var leftDelta = ((devWidth - winWidth) / 2) + ((devLeft - devRight) / 2);

            devFrame.scrollLeft(leftDelta);
        }
    },
    ToogleRotationCheckbox: function (show) {
        var rotateDiv = $j('.ViewRotate');
        if (show) {
            rotateDiv.show();
            if (this.BodyClassInitialized) {
                $j('body').addClass('DevicePreview');
            }
        }
        else {
            rotateDiv.hide();
            $j('body').removeClass('DevicePreview');
        }
    },
    GetWidth: function () {
        if (CMSViewValidate.Rotated) {
            return CMSViewValidate.PreviewHeight;
        }
        else {
            return CMSViewValidate.PreviewWidth;
        }
    },
    GetHeight: function () {
        if (CMSViewValidate.Rotated) {
            return CMSViewValidate.PreviewWidth;
        }
        else {
            return CMSViewValidate.PreviewHeight;
        }
    }
};

$j(window).bind('resize', function () {
    if (CMSViewValidate.ResizeContentArea) {
        CMSViewValidate.ResizeContentArea();
    }
}).trigger('resize');