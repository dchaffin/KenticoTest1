ListPaging = function(obj) {

    var GroupID = obj.groupID,
        ClientID = obj.clientID,
        WPType = obj.wpType,
        FunctionRenderList = obj.functionRenderList,
        
        FilterPanel = jQuery("#" + obj.filterPanelID),
        InfoFilterPanel = jQuery("#" + obj.filterPanelInfoID),
        FilterTextbox = jQuery("#" + obj.filterTextboxID),
        FilterButton = jQuery("#" + obj.filterButtonID),
        FilterNotFoundStr = obj.filterNotFoundStr,
        FilterFoundStr = obj.filterFoundStr,
        FilterEnabled = obj.filterEnabled,
        FilterShowItems = (obj.filterShowItems >= 0) ? obj.filterShowItems : ChatManager.Settings.WPFilterLimit,
        FilterProperty = obj.filterProperty,

        PagingEnabled = obj.pagingEnabled,
        PagingItems = (obj.pagingItems > 0) ? obj.pagingItems : ChatManager.Settings.WPPagingItems,
        PagingContent = jQuery("#" + obj.pagingContentID),

        lastFilter = "",
        DisplayedSubList = null,
        ListOriginal = null,
        DisplayedPage = 0,
        List = null,
        FilteredList = null,
        that = this;

    if (FilterEnabled == true) {
        SetFilterKeydownEvent();
        FilterButton.click(function() {
            that.Filter(FilterTextbox.val());
            return false;
        });
    }
    

    this.SetPage = function(p) {
        var maxPage = Math.ceil(List.length / PagingItems) - 1;
        if (p < 0) {
            p = maxPage;
        }
        else if (p > maxPage) {
            p = 0;
        }
        DisplayedPage = p;
        RenderInternal();
    };


    this.NextPage = function() {
        that.SetPage(DisplayedPage + 1);
    };


    this.PrevPage = function() {
        that.SetPage(DisplayedPage - 1);
    };


    this.GetList = function() {
        return ListOriginal;
    };


    this.Render = function(lst) {
        ListOriginal = List = lst;
        if (FilterEnabled == true) {
            FilterPanel.show();
        }
        if (FilteredList != null) {
            that.Filter(lastFilter);
            return;
        }
        RenderInternal();
    };


    this.Filter = function(filter) {
        if ((filter == null) || (filter.length == 0)) {
            FilteredList = null;
            InfoFilterPanel.empty();
            lastFilter = "";
        }
        else {
            FilteredList = new Array();
            lastFilter = filter;
            for (var i = 0; i < ListOriginal.length; i++) {
                if (ListOriginal[i][FilterProperty].toLowerCase().indexOf(filter.toLowerCase()) >= 0) {
                    FilteredList.push(ListOriginal[i]);
                }
            }
            if (FilteredList.length == 0) {
                InfoFilterPanel.text(FilterNotFoundStr + " " + lastFilter);
            }
            else {
                InfoFilterPanel.text(FilteredList.length + " " + FilterFoundStr + " " + lastFilter);
            }
        }

        List = (FilteredList == null) ? ListOriginal : FilteredList;
        DisplayedPage = 0;
        RenderInternal();
    };


    this.Clear = function() {
        DisplayedPage = 0;
        PagingContent.empty();
        InfoFilterPanel.empty();
        lastFilter = "";

        if (FilterEnabled == true) {
            FilterPanel.hide();
            FilterTextbox.val("");
        }

        FilteredList = null;
    }


    function SetFilterKeydownEvent() {
        if (FilterTextbox.length > 0) {
            FilterTextbox.keydown(function(evt) {
                var e = window.event || evt;
                var key = e.keyCode;

                if (key == 13) {
                    if (e.preventDefault) e.preventDefault();
                    if (e.stopPropagation) e.stopPropagation();
                    e.returnValue = false;
                    that.Filter(FilterTextbox.val());
                }
            });
        }
    }


    function RenderInternal() {
        if (FilterEnabled == true) {
            if (FilteredList == null && (!ListOriginal || (ListOriginal.length < FilterShowItems))) {
                FilterPanel.hide();
            }
            else {
                FilterPanel.show();
            }
        }
        
        if (PagingEnabled) {
            DoPaging();
            RenderPaging();
            FunctionRenderList(DisplayedSubList);
        }
        else {
            FunctionRenderList(List);
        }
    }


    function RenderPaging() {
        PagingContent.empty();
        var maxPage = Math.ceil(List.length / PagingItems) - 1;
        if (maxPage <= 0) {
            return;
        }
        var linkStart = '<a href="#"; onclick="var obj = jQuery.ChatManager.GetWebpart(' + ((GroupID != null) ? ('\'' + GroupID + '\'') : 'null') + ', \'' + ClientID + '\', \'' + WPType + '\');';
        var html = '<div class="ChatWebPartPaging"><ul><li>' + linkStart + ' obj.ListPaging.SetPage(0); return false;">|<</a></li> <li>' + linkStart + ' obj.ListPaging.PrevPage(); return false;"><</a></li>';
        for (var i = 0; i <= maxPage; i++) {
            if (i == DisplayedPage) {
                html += '<li class="ActivePage">' + (i + 1) + '</li>';
            }
            else {
                html += '<li>' + linkStart + ' obj.ListPaging.SetPage(' + i + '); return false;">' + (i + 1) + '</a></li>';
            }
        }
        html += '<li>' + linkStart + ' obj.ListPaging.NextPage(); return false;">></a></li> <li>' + linkStart + ' obj.ListPaging.SetPage(' + maxPage + '); return false;">>|</a></li> </ul></div>';
        PagingContent.html(html);
    };


    function DoPaging() {
        if (List.length == 0) {
            DisplayedSubList = [];
            DisplayedPage = 0;
            return;
        }
        var maxPage = Math.ceil(List.length / PagingItems) - 1;
        if (DisplayedPage > maxPage) {
            DisplayedPage = maxPage;
        }
        var start = DisplayedPage * PagingItems;
        var end = start + PagingItems;
        if (end > List.length) {
            end = List.length;
        }
        DisplayedSubList = List.slice(start, end);
    };
}