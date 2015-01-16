using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Text;

using CMS.PortalEngine;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.SettingsProvider;
using CMS.CMSHelper;

public class CodeTemplateGenerator
{
    /// <summary>
    /// Generates the web part code.
    /// </summary>
    /// <param name="webpartId">Web part ID</param>
    /// <param name="baseControl">Base control nested within the web part</param>
    /// <param name="ascx">Returning ASCX code</param>
    /// <param name="code">Returning code behind</param>
    public static void GenerateWebPartCode(int webpartId, string baseControl, out string ascx, out string code)
    {
        string designer = null;

        WebPartInfo wpi = WebPartInfoProvider.GetWebPartInfo(webpartId);
        GenerateWebPartCode(wpi, baseControl, out ascx, out code, out designer);
    }


    /// <summary>
    /// Generates the web part code.
    /// </summary>
    /// <param name="wpi">Web part info</param>
    /// <param name="baseControl">Base control nested within the web part</param>
    /// <param name="ascx">Returning ASCX code</param>
    /// <param name="code">Returning code behind</param>
    /// <param name="designer">Returning designer file</param>
    public static void GenerateWebPartCode(WebPartInfo wpi, string baseControl, out string ascx, out string code, out string designer)
    {
        code = "";
        ascx = "";
        designer = "";

        if (wpi != null)
        {
            string templateFile = HttpContext.Current.Server.MapPath("~/App_Data/CodeTemplates/WebPart");

            // Generate the ASCX
            ascx = File.ReadAllText(templateFile + ".ascx.template");

            // Prepare the path
            string path = URLHelper.UnResolveUrl(WebPartInfoProvider.GetWebPartUrl(wpi), SettingsKeyProvider.ApplicationPath);

            // Prepare the class name
            string className = path.Trim('~', '/');
            if (className.EndsWithCSafe(".ascx"))
            {
                className = className.Substring(0, className.Length - 5);
            }
            className = ValidationHelper.GetIdentifier(className, "_");

            ascx = Regex.Replace(ascx, "(Inherits)=\"[^\"]+\"", "$1=\"" + className + "\"");

            // Replace the code file / code behind
            bool webApp = CMSContext.IsWebApplication;

            string fileAttr = "CodeFile";
            //if (webApp)
            //{
            //    fileAttr = "CodeBehind";
            //}

            ascx = Regex.Replace(ascx, "(CodeFile|CodeBehind)=\"[^\"]+\"", fileAttr + "=\"" + path + ".cs\"");

            // Generate the code
            code = File.ReadAllText(templateFile + ".ascx.cs.template");

            code = Regex.Replace(code, "( class\\s+)[^\\s]+", "$1" + className);

            // Prepare the properties
            FormInfo fi = new FormInfo(wpi.WebPartProperties);

            StringBuilder sbInit = new StringBuilder();

            string propertiesCode = CodeGenerator.GetPropertiesCode(fi, true, baseControl, sbInit, true);

            // Replace in code
            code = code.Replace("// ##PROPERTIES##", propertiesCode);
            code = code.Replace("// ##SETUP##", sbInit.ToString());

            // Generate the designer
            if (webApp)
            {
                designer = File.ReadAllText(templateFile + ".ascx.designer.cs.template");

                designer = Regex.Replace(designer, "( class\\s+)[^\\s]+", "$1" + className);
            }
        }
    }
}
