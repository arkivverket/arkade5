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
    internal class ArchiveFormatValidationMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ArchiveFormatValidationMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Arkivverket.Arkade.Core.Resources.ArchiveFormatValidationMessages", typeof(ArchiveFormatValidationMessages).Assembly);
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
        ///   Looks up a localized string similar to Content of {0} has been validated against the selected archive format:
        ///{1}.
        /// </summary>
        internal static string DirectoryValidationResultMessage {
            get {
                return ResourceManager.GetString("DirectoryValidationResultMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The validation failed. See error log for details..
        /// </summary>
        internal static string FileFormatValidationErrorMessage {
            get {
                return ResourceManager.GetString("FileFormatValidationErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} conforms with the selected archive format:
        ///{1}.
        /// </summary>
        internal static string ItemConformsWithFormat {
            get {
                return ResourceManager.GetString("ItemConformsWithFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} does not conform with the selected archive format.
        ///{1}.
        /// </summary>
        internal static string ItemDoesNotConformWithFormat {
            get {
                return ResourceManager.GetString("ItemDoesNotConformWithFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to  Total number of validated files: {0}
        ///   Valid files: {1}
        ///   Invalid files: {2}
        ///   Unable to determine: {3}
        /// Detailed report: {4}.
        /// </summary>
        internal static string PdfABatchValidationInfoMessage {
            get {
                return ResourceManager.GetString("PdfABatchValidationInfoMessage", resourceCulture);
            }
        }
    }
}
