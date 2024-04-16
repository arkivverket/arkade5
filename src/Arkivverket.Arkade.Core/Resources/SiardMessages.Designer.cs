﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Arkivverket.Arkade.Core.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class SiardMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SiardMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Arkivverket.Arkade.Core.Resources.SiardMessages", typeof(SiardMessages).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find a siard file in the extract.
        /// </summary>
        public static string CouldNotFindASiardFile {
            get {
                return ResourceManager.GetString("CouldNotFindASiardFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} is not valid for SIARD version {1}:
        ///{2}.
        /// </summary>
        public static string DeserializationUnsuccessfulMessage {
            get {
                return ResourceManager.GetString("DeserializationUnsuccessfulMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ERROR.
        /// </summary>
        public static string ErrorMessage {
            get {
                return ResourceManager.GetString("ErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to External LOB file &apos;{0}&apos; not found.
        /// </summary>
        public static string ExternalLobFileNotFoundMessage {
            get {
                return ResourceManager.GetString("ExternalLobFileNotFoundMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to NB! If a package is created, Arkade will not be able to include any external blobs/clobs.
        /// </summary>
        public static string ExternalLobsNotCopiedWarning {
            get {
                return ResourceManager.GetString("ExternalLobsNotCopiedWarning", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to LOB is inlined in an unsupported format.
        /// </summary>
        public static string InlinedLobContentHasUnsupportedEncoding {
            get {
                return ResourceManager.GetString("InlinedLobContentHasUnsupportedEncoding", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} is being validated with Database Preservation Toolkit Developer version {1}.
        /// </summary>
        public static string ValidationMessage {
            get {
                return ResourceManager.GetString("ValidationMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Siard Validation.
        /// </summary>
        public static string ValidationMessageIdentifier {
            get {
                return ResourceManager.GetString("ValidationMessageIdentifier", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Validation results.
        /// </summary>
        public static string ValidationResultMessage {
            get {
                return ResourceManager.GetString("ValidationResultMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Results of conformity test performed by {0}.
        /// </summary>
        public static string ValidationResultTestName {
            get {
                return ResourceManager.GetString("ValidationResultTestName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} version {1}.
        /// </summary>
        public static string ValidationTool {
            get {
                return ResourceManager.GetString("ValidationTool", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SIARD validator only supports SIARD 2.1 version.
        /// </summary>
        public static string ValidatorDoesNotSupportVersionMessage {
            get {
                return ResourceManager.GetString("ValidatorDoesNotSupportVersionMessage", resourceCulture);
            }
        }
    }
}
