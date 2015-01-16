var JsPlumbMultichoiceNode = $class({

    Extends: JsPlumbConditionNode,

    constructor: function (readOnly, graph, definition) {
        JsPlumbConditionNode.call(this, readOnly, graph, definition);

        this.defaultSourcePointDefinition = [{ Type: "switchCase" }, { Type: "switchCase" }, { Type: "switchDefault"}];
        this.defaultCaseContent = "New condition";
    },


    /*
    *   Returns node HTML representation.
    */
    getDefaultHtmlElement: function () {
        return $j('<div class="Node ' + this.nodeStyle + '" id="' + this.htmlId + '">' +
                    this.getNameHtmlRepresentation() +
                    this.getContentHtmlRepresentation() +
                 '</div>');
    },


    /*
    *   Returns add icon HTML representation.
    */
    getAddIconHtmlRepresentation: function () {
        if (this.readOnly)
            return '';
        return '<div id="' + this.htmlId + '_add" class="button plus" title="' + this.getAddCaseTooltip() + '"></div>';
    },


    /*
    *   Returns new case row HTML representation.
    */
    getCaseRowTemplate: function (sourcePoint) {
        if (!sourcePoint.Label) {
            sourcePoint.Label = this.defaultCaseContent;
        }
        return '<div id="' + sourcePoint.HtmlID + '" class="box gradient Case"><div class="inner ' + this.getLocalizedClass(sourcePoint.IsLabelLocalized) + '"><span class="Editable">' + sourcePoint.Label + '</span></div>' +
                    this.getDeleteIconHtmlRepresentation(this.getDeleteCaseTooltip()) +
                    this.getEditIconHtmlRepresentation(this.getEditCaseTooltip()) +
                '</div>';
    },


    /*
    *   Returns add case tooltip.
    */
    getAddCaseTooltip: function () {
        return this.graph.getReadOnlyResourceString("AddCaseTooltip");
    },


    /*
    *   Returns delete case tooltip.
    */
    getDeleteCaseTooltip: function () {
        return this.graph.getReadOnlyResourceString("DeleteCaseTooltip");
    },


    /*
    *   Returns edit case tooltip.
    */
    getEditCaseTooltip: function () {
        return this.graph.getReadOnlyResourceString("EditCaseTooltip");
    },


    /*
    *   Returns query string of source point.
    */
    getSourcepointQueryString: function (sourcePointId) {
        if (sourcePointId) {
            return "&sourcepoint=" + sourcePointId;
        }
        return "";
    },


    /*
    *   Overrides default behavior.
    */
    setTimeout: function () {
        this.hasTimeout = true;
        this.setTimeoutProperties();
    }
});
