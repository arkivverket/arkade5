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
    public class ExceptionMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ExceptionMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Arkivverket.Arkade.Core.Resources.ExceptionMessages", typeof(ExceptionMessages).Assembly);
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
        ///   Looks up a localized string similar to TestRun for the test {0} with ID {1} was not added to test suite. Probable cause: Another test has already been added with the same ID.
        /// </summary>
        public static string AddTestRunToTestSuite {
            get {
                return ResourceManager.GetString("AddTestRunToTestSuite", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Processing area is not set..
        /// </summary>
        public static string ArkadeProcessAreaNotSet {
            get {
                return ResourceManager.GetString("ArkadeProcessAreaNotSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Arkade could not find any information about {0} from {1}.
        /// </summary>
        public static string FileDescriptionParseError {
            get {
                return ResourceManager.GetString("FileDescriptionParseError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot find the file: {0}.
        /// </summary>
        public static string FileNotFound {
            get {
                return ResourceManager.GetString("FileNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not read the file: {0}.
        /// </summary>
        public static string FileNotRead {
            get {
                return ResourceManager.GetString("FileNotRead", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid field format &apos;{0}&apos; for data type &apos;{1}&apos;. Accepted field formats are: {2}.
        /// </summary>
        public static string InvalidFieldFormatMessage {
            get {
                return ResourceManager.GetString("InvalidFieldFormatMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not combine {0} and {1}.
        /// </summary>
        public static string PathCombine {
            get {
                return ResourceManager.GetString("PathCombine", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Arkade did not manage to interpret information about period separator from {0}.
        /// </summary>
        public static string PeriodSeparationParseError {
            get {
                return ResourceManager.GetString("PeriodSeparationParseError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Something went wrong while cleaning up this Arkade session.
        ///
        ///To ensure all temporary files from the session are removed from your system, please delete &apos;{0}&apos; manually..
        /// </summary>
        public static string ProcessAreaCleanUpFailed {
            get {
                return ResourceManager.GetString("ProcessAreaCleanUpFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Something went wrong while validating Siard file. Please see arkade-tmp/logs/ for details..
        /// </summary>
        public static string SiardValidatorError {
            get {
                return ResourceManager.GetString("SiardValidatorError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find the validator library. Please download the library from {0}, and save the file at {1}..
        /// </summary>
        public static string SiardValidatorLibraryNotFound {
            get {
                return ResourceManager.GetString("SiardValidatorLibraryNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not start validator. Please make sure that Java Runtime is installed and available by entering &quot;java -version&quot; in a console..
        /// </summary>
        public static string SiardValidatorOpenError {
            get {
                return ResourceManager.GetString("SiardValidatorOpenError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown package type: {0}.
        /// </summary>
        public static string UnknownPackageType {
            get {
                return ResourceManager.GetString("UnknownPackageType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Write access denied.
        /// </summary>
        public static string WriteAccessDeniedCaption {
            get {
                return ResourceManager.GetString("WriteAccessDeniedCaption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Arkade does not have permission to write to this location..
        /// </summary>
        public static string WriteAccessDeniedMessage {
            get {
                return ResourceManager.GetString("WriteAccessDeniedMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Line {0}: {1}.
        /// </summary>
        public static string XmlValidationErrorMessage {
            get {
                return ResourceManager.GetString("XmlValidationErrorMessage", resourceCulture);
            }
        }
    }
}
