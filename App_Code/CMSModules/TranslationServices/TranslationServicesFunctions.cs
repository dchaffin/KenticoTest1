using System;

using CMS.TranslationServices;

using TreeNode = CMS.DocumentEngine.TreeNode;

/// <summary>
/// Translation services functions.
/// </summary>
public class TranslationServicesFunctions
{
    /// <summary>
    /// Returns true if there is at least one translation submission item with target XLIFF.
    /// </summary>
    /// <param name="document">Document to check</param>
    public static bool IsTranslationReady(object document)
    {
        TreeNode doc = document as TreeNode;
        if (doc != null)
        {
            return TranslationServiceHelper.IsTranslationReady(doc);
        }
        return false;
    }
}