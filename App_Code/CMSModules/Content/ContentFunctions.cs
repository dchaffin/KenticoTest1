using System;

using CMS.DocumentEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.GlobalHelper;

public class ContentFunctions
{
    /// <summary>
    /// Returns true if document is in one/all of selected categories.
    /// </summary>
    /// <param name="document">Document to check</param>
    /// <param name="categories">Category names separated with a semicolon</param>
    /// <param name="all">If true, document must be in all of the selected categories.</param>
    public static bool IsInCategories(object document, string categories, bool allCategories)
    {
        TreeNode doc = document as TreeNode;
        if (doc != null)
        {
            if (!String.IsNullOrEmpty(categories))
            {
                string[] categoryNames = categories.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                // Get categories selected by user
                string where = SqlHelperClass.GetWhereCondition("CategoryName", categoryNames);
                var categoryInfos = CategoryInfoProvider.GetDocumentCategories(doc.DocumentID, where, null, 0, null).Items;

                // Return true if all/any categories were found
                return allCategories ? (categoryInfos.Count == categoryNames.Length) : (categoryInfos.Count > 0);
            }

            // No categories were selected
            if (allCategories)
            {
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Returns true if document is translated into one of selected cultures.
    /// </summary>
    /// <param name="document">Document to check</param>
    /// <param name="cultures">Culture codes separated with a semicolon</param>
    /// <param name="publishedOnly">If true culture version must be published</param>
    public static bool IsTranslatedTo(object document, string cultures, bool publishedOnly)
    {
        TreeNode doc = document as TreeNode;
        if ((doc != null) && (!String.IsNullOrEmpty(cultures)))
        {
            string[] cultureCodes = cultures.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string where = SqlHelperClass.GetWhereCondition("DocumentCulture", cultureCodes);

            TreeNodeDataSet cultureVersions = doc.TreeProvider.SelectNodes(doc.NodeSiteName, doc.NodeAliasPath, TreeProvider.ALL_CULTURES, false, null, where, null, -1, publishedOnly, 0, TreeProvider.SELECTNODES_REQUIRED_COLUMNS);
            return !DataHelper.DataSourceIsEmpty(cultureVersions);
        }

        return false;
    }


    /// <summary>
    /// Returns true if document has any/all of the specified tags.
    /// </summary>
    /// <param name="document">Document</param>
    /// <param name="tags">Semicolon separated tags</param>
    /// <param name="allTags">Indicates whether all tags must be present or only one of them</param>
    public static bool HasTags(object document, string tags, bool allTags)
    {
        TreeNode doc = document as TreeNode;
        if (doc != null)
        {
            if (!String.IsNullOrEmpty(tags))
            {
                string[] tagNames = tags.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                string where = SqlHelperClass.GetWhereCondition("TagName", tagNames);
                var tagInfos = TagInfoProvider.GetTags(doc.DocumentID, where, null, 0, null).Items;

                // Return true if all/any tags were found
                return allTags ? (tagInfos.Count == tagNames.Length) : (tagInfos.Count > 0);
            }

            // No tags were selected
            if (allTags)
            {
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Returns true if document is in specified relationship with with selected document.
    /// </summary>
    /// <param name="document">Document to be checked</param>
    /// <param name="side">Relationship side</param>
    /// <param name="relationship">Relationship name</param>
    /// <param name="relatedDocumentPath">Alias path to selected document</param>
    /// <param name="relatedDocumentSite">Selected document site name</param>
    public static bool IsInRelationship(object document, string side, string relationship, string relatedDocumentPath, string relatedDocumentSite)
    {
        TreeNode doc = document as TreeNode;
        if (doc != null)
        {
            int leftNodeID = 0;
            int rightNodeID = 0;

            // Use site of the checked document when no other is specified
            if (String.IsNullOrEmpty(relatedDocumentSite))
            {
                relatedDocumentSite = doc.NodeSiteName;
            }

            // Prepare left and right document for relationship
            side = side.ToLowerCSafe();
            if (side == "left")
            {
                leftNodeID = doc.NodeID;
                rightNodeID = TreePathUtils.GetNodeIdByAliasPath(relatedDocumentSite, relatedDocumentPath);
            }
            else if (side == "right")
            {
                leftNodeID = TreePathUtils.GetNodeIdByAliasPath(relatedDocumentSite, relatedDocumentPath);
                rightNodeID = doc.NodeID;
            }

            // Get relationship ID from relationship name
            RelationshipNameInfo relationshipName = RelationshipNameInfoProvider.GetRelationshipNameInfo(relationship);
            if (relationshipName != null)
            {
                // Check whether relationship between the two documents exists
                return (RelationshipProvider.GetRelationshipInfo(leftNodeID, rightNodeID, relationshipName.RelationshipNameId) != null);
            }
        }

        return false;
    }
}