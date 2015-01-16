using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CMS.GlobalHelper;

public class MessageBoardMethods
{
    /// <summary>
    /// Registers all blog methods to macro resolver.
    /// </summary>
    public static void RegisterMethods()
    {
        // Get user name
        MacroMethod getBoardMessagesCount = new MacroMethod("GetBoardMessagesCount", GetBoardMessagesCount)
        {
            Comment = "Returns count of messages in messageboard.",
            Type = typeof(int),
            MinimumParameters = 2,
            AllowedTypes = new List<Type>() { typeof(TransformationNamespace) }
        };
        getBoardMessagesCount.AddParameter("documentId", typeof(int), "ID of the document.");
        getBoardMessagesCount.AddParameter("boardWebpartName", typeof(object), "Name of the webpart used for creating messageboard.");
        getBoardMessagesCount.AddParameter("type", typeof(object), "String constant representing the type of board 'user', 'group' or 'document' (default) are accepted.");
        MacroMethods.RegisterMethod(getBoardMessagesCount);
    }


    /// <summary>
    /// Returns user name.
    /// </summary>
    /// <param name="parameters">(Document ID; Board Webpart Name) OR (Document ID; Board Webpart Name; Type)</param>
    public static object GetBoardMessagesCount(params object[] parameters)
    {
        switch (parameters.Length)
        {
            case 2:
                return MessageBoardFunctions.GetBoardMessagesCount(ValidationHelper.GetInteger(parameters[0], 0), ValidationHelper.GetString(parameters[1], ""), "document");

            case 3:
                return MessageBoardFunctions.GetBoardMessagesCount(ValidationHelper.GetInteger(parameters[0], 0), ValidationHelper.GetString(parameters[1], ""), ValidationHelper.GetString(parameters[2], ""));

            default:
                throw new NotSupportedException();
        }
    }
}
