using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#if SILVERLIGHT && WINDOWS_PHONE
[assembly: AssemblyTitle("YouTrack.Rest.WindowsPhone")]
#elif SILVERLIGHT && !WINDOWS_PHONE
[assembly: AssemblyTitle("YouTrack.Rest.Silverlight")]
#else
[assembly: AssemblyTitle("YouTrack.Rest")]
#endif
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyProduct("YouTrack.Rest")]
[assembly: AssemblyCopyright("Copyright © Sauli Tähkäpää 2012")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("YouTrack.Rest.Tests")]
#if SILVERLIGHT && WINDOWS_PHONE
[assembly: InternalsVisibleTo("RestSharp.WindowsPhone")]
#elif SILVERLIGHT && !WINDOWS_PHONE
[assembly: InternalsVisibleTo("RestSharp.Silverlight")]
#endif

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM

#if SILVERLIGHT && WINDOWS_PHONE
[assembly: Guid("c2f83401-f008-4d80-90c4-caed70656e4d")]
#elif SILVERLIGHT && !WINDOWS_PHONE
[assembly: Guid("f447ed04-498f-4cc8-97a9-3500c924cafc")]
#else
[assembly: Guid("b0bac763-7e76-4c99-a246-dadb670d0797")]
#endif

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("0.4.0.0")]
[assembly: AssemblyFileVersion("0.4.0.0")]
