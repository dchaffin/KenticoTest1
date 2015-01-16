using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.IO;

public partial class CMSFormControls_Classes_AssemblyClassSelector : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets value for form engine
    /// </summary>
    public override object Value
    {
        get
        {
            return AssemblyName;
        }
        set
        {
            AssemblyName = ValidationHelper.GetString(value, String.Empty);
        }
    }


    /// <summary>
    /// Checks validity of inputs
    /// </summary>
    public override bool IsValid()
    {
        try
        {
            object classObject = ClassHelper.GetClass(AssemblyName, ClassName);
            if (classObject != null)
            {
                Type parentType = Type.GetType(BaseClassName);

                if ((classObject.GetType().GetInterface(BaseClassName, true) != null) || ((parentType != null) && (classObject.GetType().IsSubclassOf(parentType))))
                {
                    return true;
                }
            }
        }
        catch
        {
            return false;
        }

        return false;
    }

    /// <summary>
    /// Gets a column name for full class name
    /// </summary>
    public string ClassNameColumnName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ClassNameColumnName"), String.Empty);
        }
        set
        {
            SetValue("ClassNameColumnName", value);
        }
    }


    /// <summary>
    /// Gets or sets filter for list assemblies.
    /// </summary>
    public string AssembliesFilter
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AssembliesFilter"), "*");
        }
        set
        {
            SetValue("AssembliesFilter", value);
        }
    }


    /// <summary>
    /// Gets or sets base class name to filter classes.
    /// </summary>
    public string BaseClassName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BaseClassName"), String.Empty);
        }
        set
        {
            SetValue("BaseClassName", value);
        }
    }


    /// <summary>
    /// Gets or sets assembly name value to textbox.
    /// </summary>
    public string AssemblyName
    {
        get
        {
            return drpAssemblyName.SelectedValue;
        }
        set
        {
            drpAssemblyName.SelectedValue = value;
        }
    }


    /// <summary>
    /// Gets or sets full class name value to textbox.
    /// </summary>
    public string ClassName
    {
        get
        {
            return drpClassName.SelectedValue;
        }
        set
        {
            drpClassName.SelectedValue = value;
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Returns other values related to this form control.
    /// </summary>
    /// <returns>Returns an array where first dimension is attribute name and the second dimension is its value.</returns>
    public override object[,] GetOtherValues()
    {
        if (!String.IsNullOrEmpty(ClassNameColumnName))
        {
            // Set properties names
            object[,] values = new object[1, 2];
            values[0, 0] = ClassNameColumnName;
            values[0, 1] = drpClassName.Text;
            return values;
        }
        return null;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            SetupControl();
        }
    }


    protected void TextBoxControl_TextChanged(object sender, EventArgs e)
    {
        FillClassNameSelector();
    }

    #endregion


    #region "Private methods"

    private void SetupControl()
    {
        drpAssemblyName.TextBoxControl.TextChanged += new EventHandler(TextBoxControl_TextChanged);

        if (!URLHelper.IsPostback())
        {
            drpAssemblyName.DropDownList.Items.Add("App_Code");

            // Fill assemblies list
            foreach (string file in Directory.GetFiles(Server.MapPath(ResolveUrl("~/bin")), AssembliesFilter + ".dll"))
            {
                drpAssemblyName.DropDownList.Items.Add(Path.GetFileNameWithoutExtension(file));
            }
        }

        if (drpClassName.DropDownList.Items.Count == 0)
        {
            FillClassNameSelector();
        }

        if (ContainsColumn(ClassNameColumnName))
        {
            drpClassName.Text = ValidationHelper.GetString(Form.Data.GetValue(ClassNameColumnName), String.Empty);
        }

        // Create and register delayed postback
        string delayedPostbackScript =
      @"var timer = null;

            function SetupUpdate()
            {   
                var txtAssembly = $j('#" + drpAssemblyName.TextBoxControl.ClientID + @"'); 
                if (txtAssembly) {
                    txtAssembly.keyup(StartTimeout);
                }
                $j('#" + pnlAssemblyName.ClientID + @" button').click(AddOnClick);
            }
            
            function StartTimeout()
            {   
                clearTimeout(timer);                
                timer = setTimeout('OnTimeout()', 700);                
            }

            function OnTimeout()
            {   
               " + ControlsHelper.GetPostBackEventReference(drpAssemblyName.TextBoxControl, drpAssemblyName.TextBoxControl.ClientID) + @"                        
            }

            function AddOnClick()
            {
                $j('#" + pnlAssemblyName.ClientID + @" li').click(StartTimeout);      
            }";

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "DelayedPostback", ScriptHelper.GetScript(delayedPostbackScript));
    }


    /// <summary>
    /// Fills class name selector.
    /// </summary>
    private void FillClassNameSelector()
    {
        Assembly selAssembly = null;
        try
        {
            selAssembly = ClassHelper.GetAssembly(drpAssemblyName.Text);
        }
        catch
        {
            // Assembly doesn't exist
        }

        drpClassName.DropDownList.Items.Clear();

        if (selAssembly != null)
        {
            Type baseType = Type.GetType(BaseClassName);

            // Get list of available classes
            List<Type> types = selAssembly.GetTypes().Where(t => !t.IsDefined(typeof(CompilerGeneratedAttribute), false) && ((String.IsNullOrEmpty(BaseClassName)) || (t.IsClass && ((t.GetInterface(BaseClassName, true) != null) || ((baseType != null) && (t.IsSubclassOf(baseType))))))).OrderBy(t => t.Name).ToList<Type>();
            bool selected = false;
            foreach (Type type in types)
            {
                if (!String.IsNullOrEmpty(type.Namespace) && !String.IsNullOrEmpty(type.Name))
                {
                    string fullName = type.Namespace + "." + type.Name;
                    drpClassName.DropDownList.Items.Add(fullName);

                    // Select first item after change
                    if (!selected && URLHelper.IsPostback())
                    {
                        drpClassName.SelectedValue = fullName;
                        selected = true;
                    }
                }
            }
        }

        // Clear selected value if drop down is empty
        if (drpClassName.DropDownList.Items.Count == 0)
        {
            drpClassName.SelectedValue = String.Empty;
        }
    }

    #endregion
}