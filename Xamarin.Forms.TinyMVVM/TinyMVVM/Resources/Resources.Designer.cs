﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Xamarin.Forms.TinyMVVM.TinyMVVM.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Xamarin.Forms.TinyMVVM.TinyMVVM.Resources.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot register a CompositeCommand in itself..
        /// </summary>
        internal static string CannotRegisterCompositeCommandInItself {
            get {
                return ResourceManager.GetString("CannotRegisterCompositeCommandInItself", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot register the same command twice in the same CompositeCommand..
        /// </summary>
        internal static string CannotRegisterSameCommandTwice {
            get {
                return ResourceManager.GetString("CannotRegisterSameCommandTwice", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to At least one cyclic dependency has been found in the module catalog. Cycles in the module dependencies must be avoided..
        /// </summary>
        internal static string CyclicDependencyFound {
            get {
                return ResourceManager.GetString("CyclicDependencyFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {1}: {2}. Priority: {3}. Timestamp:{0:u}..
        /// </summary>
        internal static string DefaultDebugLoggerPattern {
            get {
                return ResourceManager.GetString("DefaultDebugLoggerPattern", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Neither the executeMethod nor the canExecuteMethod delegates can be null..
        /// </summary>
        internal static string DelegateCommandDelegatesCannotBeNull {
            get {
                return ResourceManager.GetString("DelegateCommandDelegatesCannotBeNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to T for DelegateCommand&lt;T&gt; is not an object nor Nullable..
        /// </summary>
        internal static string DelegateCommandInvalidGenericPayloadType {
            get {
                return ResourceManager.GetString("DelegateCommandInvalidGenericPayloadType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot add dependency for unknown module {0}.
        /// </summary>
        internal static string DependencyForUnknownModule {
            get {
                return ResourceManager.GetString("DependencyForUnknownModule", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A module declared a dependency on another module which is not declared to be loaded. Missing module(s): {0}.
        /// </summary>
        internal static string DependencyOnMissingModule {
            get {
                return ResourceManager.GetString("DependencyOnMissingModule", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A duplicated module with name {0} has been found by the loader..
        /// </summary>
        internal static string DuplicatedModule {
            get {
                return ResourceManager.GetString("DuplicatedModule", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to To use the UIThread option for subscribing, the EventAggregator must be constructed on the UI thread..
        /// </summary>
        internal static string EventAggregatorNotConstructedOnUIThread {
            get {
                return ResourceManager.GetString("EventAggregatorNotConstructedOnUIThread", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An exception occurred while initializing module &apos;{0}&apos;. 
        ///    - The exception message was: {2}
        ///    - The Assembly that the module was trying to be loaded from was:{1}
        ///    Check the InnerException property of the exception for more information. If the exception occurred while creating an object in a DI container, you can exception.GetRootException() to help locate the root cause of the problem..
        /// </summary>
        internal static string FailedToLoadModule {
            get {
                return ResourceManager.GetString("FailedToLoadModule", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An exception occurred while initializing module &apos;{0}&apos;. 
        ///    - The exception message was: {1}
        ///    Check the InnerException property of the exception for more information. If the exception occurred 
        ///    while creating an object in a DI container, you can exception.GetRootException() to help locate the 
        ///    root cause of the problem..
        /// </summary>
        internal static string FailedToLoadModuleNoAssemblyInfo {
            get {
                return ResourceManager.GetString("FailedToLoadModuleNoAssemblyInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to load type for module {0}. 
        ///Error was: {1}..
        /// </summary>
        internal static string FailedToRetrieveModule {
            get {
                return ResourceManager.GetString("FailedToRetrieveModule", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid Delegate Reference Type Exception.
        /// </summary>
        internal static string InvalidDelegateRerefenceTypeException {
            get {
                return ResourceManager.GetString("InvalidDelegateRerefenceTypeException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The entity does not contain a property with that name.
        /// </summary>
        internal static string InvalidPropertyNameException {
            get {
                return ResourceManager.GetString("InvalidPropertyNameException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Module {0} depends on other modules that don&apos;t belong to the same group..
        /// </summary>
        internal static string ModuleDependenciesNotMetInGroup {
            get {
                return ResourceManager.GetString("ModuleDependenciesNotMetInGroup", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The member access expression does not access a property..
        /// </summary>
        internal static string PropertySupport_ExpressionNotProperty_Exception {
            get {
                return ResourceManager.GetString("PropertySupport_ExpressionNotProperty_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The expression is not a member access expression..
        /// </summary>
        internal static string PropertySupport_NotMemberAccessExpression_Exception {
            get {
                return ResourceManager.GetString("PropertySupport_NotMemberAccessExpression_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The referenced property is a static property..
        /// </summary>
        internal static string PropertySupport_StaticExpression_Exception {
            get {
                return ResourceManager.GetString("PropertySupport_StaticExpression_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Module {0} is marked for automatic initialization when the application starts, but it depends on modules that are marked as OnDemand initialization. To fix this error, mark the dependency modules for InitializationMode=WhenAvailable, or remove this validation by extending the ModuleCatalog class..
        /// </summary>
        internal static string StartupModuleDependsOnAnOnDemandModule {
            get {
                return ResourceManager.GetString("StartupModuleDependsOnAnOnDemandModule", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided String argument {0} must not be null or empty..
        /// </summary>
        internal static string StringCannotBeNullOrEmpty {
            get {
                return ResourceManager.GetString("StringCannotBeNullOrEmpty", resourceCulture);
            }
        }
    }
}
