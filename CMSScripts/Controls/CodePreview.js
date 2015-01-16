var IsCMSDesk = true;
var IsFullScreen = false;
var EditorElem = null;
var MovePreviewHeader = true;

function PreviewEditorFullScreen(isFullScreen, elemsrc, paneContentID) {
    var elem = $j(elemsrc);

    // Store values to global for redirect
    IsFullScreen = isFullScreen;
    EditorElem = elem;
    var parent = $j("#" + paneContentID);

    if (!isFullScreen) {
        elem.css('width', 'auto');
        elem.css('height', 'auto');
        elem.css('top', 'auto');
        elem.css('left', 'auto');
    }
    else {
        var offset = 0;
        // fullScreenOffset defines additional offset - depends on content control (f.e. when displayed page title)
        if (typeof (fullScreenOffset) != 'undefined') {
            offset = fullScreenOffset;
        }
        var border = elem.css('border-left-width').replace("px", "");

        var header = $j('.PreviewMenu');
        var basicHeader = header.height() -1;
        elem.width(parent.width() - (border * 2));
        elem.height(parent.height() - basicHeader - (border * 2) - offset);
        elem.offset({ top: parent.offset().top + basicHeader + offset, left: parent.offset().left });
    }
    ResizeHeaderButtons();
}

function LayoutResized(pane, $Pane, paneState) {
    if (IsFullScreen) {
        CM_FullScreen(IsFullScreen, EditorElem);
    }
    else {
        ResizeHeaderButtons();
    }
}

function ResizeHeaderButtonsInternal(parent, elem) {
    var parent = $j('#' + parent);
    var elem = $j('#' + elem);

    // Hide scrollbar for fullscreen macro editor. 
    var widthAdd = IsFullScreen && MovePreviewHeader ? 14 : 0;
    var isRTL = $j('body').hasClass('RTL');

    // Move left for vertical rtl scrollbar hide
    var leftMove = isRTL && widthAdd ? 14 : 0;
    elem.width(parent.width() + widthAdd);
    elem.offset({ left: parent.offset().left - leftMove });
}

function InitPreview(body, startWithFullScreen) {
    $j("#" + body).show();
    ResizeHeaderButtons();
    if (startWithFullScreen && !IsFullScreen && (CM_instances != null)) {
        CM_instances[0].toolbar.fitWindow();
    }

    if (typeof (CKEDITOR) != 'undefined' && CKEDITOR.instances.htmlEditor) {
        CKEDITOR.instances.htmlEditor.on('resize', function (event) { ResizeHeaderButtons(); });
        CKEDITOR.on('instanceReady', function (event) { ResizeHeaderButtons(); });
    }
}