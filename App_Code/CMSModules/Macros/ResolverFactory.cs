using System.Linq;

using CMS.CMSHelper;
using CMS.SettingsProvider;
using CMS.FormEngine;
using CMS.FormControls;

/// <summary>
/// Class providing resolvers for IntelliSense and ConditionBuilders throughout the system.
/// </summary>
public static class ResolverFactory
{
    #region "Methods"

    /// <summary>
    /// Returns static resolver if exists.
    /// </summary>
    /// <param name="name">Name of the resolver to return</param>
    private static ContextResolver GetStaticResolver(string name)
    {
        switch (name.ToLowerCSafe())
        {
            case "analyticsresolver": 
                return EmailTemplateMacros.AnalyticsResolver;

            case "workflowbasedocumentresolver": 
                return EmailTemplateMacros.WorkflowBaseDocumentResolver;

            case "workflowsimpledocumentresolver": 
                return EmailTemplateMacros.WorkflowSimpleDocumentResolver;

            case "automationsimpleresolver": 
                return EmailTemplateMacros.AutomationSimpleResolver;

            case "contactresolver":
                return EmailTemplateMacros.ContactResolver;

            case "contactactivityresolver":
                return EmailTemplateMacros.ContactActivityResolver;

            case "contactscoreresolver":
                return EmailTemplateMacros.ContactScoreResolver;

            case "variantresolver": 
                return EmailTemplateMacros.VariantResolver;

            case "newsletterresolver": 
                return EmailTemplateMacros.NewsletterResolver;

            case "newsletteroptinresolver": 
                return EmailTemplateMacros.NewsletterOptInResolver;

            case "emailtemplateresolver":
                return EmailTemplateMacros.EmailTemplateResolver;

            case "bookingresolver":
                return EmailTemplateMacros.BookingResolver;

            case "deviceprofilesresolver":
                return EmailTemplateMacros.DeviceProfilesResolver;
        }
        return null;
    }


    /// <summary>
    /// Returns a resolver based on given name.
    /// </summary>
    /// <param name="name">Name of the resolver</param>
    public static ContextResolver GetResolver(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return GetDefaultResolver();
        }

        string key = name.ToLowerCSafe();

        ContextResolver resolver = GetStaticResolver(name);
        if (resolver == null)
        {
            if (key.StartsWithCSafe("form."))
            {
                resolver = GetFormResolver(key.Substring(5));
            }
            else if (key.StartsWithCSafe("formdefinition."))
            {
                resolver = GetFormDefinitionResolver(name.Substring(15));
            }
            else
            {
                resolver = GetDefaultResolver();
            }
        }
        return resolver.CreateContextChild();
    }


    /// <summary>
    /// Returns new instance of resolver for given class (includes all fields, etc.).
    /// </summary>
    /// <param name="className">Name of the class</param>
    private static ContextResolver GetFormResolver(string className)
    {
        DataClassInfo dc = DataClassInfoProvider.GetDataClass(className);
        if (dc != null)
        {
            return GetFormDefinitionResolver(dc.ClassFormDefinition);
        }
        return null;
    }


    /// <summary>
    /// Returns new instance of resolver based on given form definition.
    /// </summary>
    /// <param name="formDefinition">Form definition</param>
    private static ContextResolver GetFormDefinitionResolver(string formDefinition)
    {
        FormInfo fi = new FormInfo(formDefinition);
        return GetFormResolver(fi);
    }


    /// <summary>
    /// Returns new instance of resolver for given class (includes all fields, etc.).
    /// </summary>
    /// <param name="formInfo">FormInfo</param>
    private static ContextResolver GetFormResolver(FormInfo formInfo)
    {
        ContextResolver mContextResolver = GetDefaultResolver();

        var controlPlaceholder = new EditingFormControl();
        foreach (var fieldInfo in formInfo.ItemsList.OfType<FormFieldInfo>())
        {
            mContextResolver.SetNamedSourceData(fieldInfo.Name, controlPlaceholder);
        }

        return mContextResolver;
    }


    /// <summary>
    /// Returns resolver which should be used if not other is applicable.
    /// </summary>
    private static ContextResolver GetDefaultResolver()
    {
        return CMSContext.CurrentResolver.CreateContextChild();
    }

    #endregion
}