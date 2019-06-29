using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle( "RoomVolumeDirectShape" )]
[assembly: AssemblyDescription( "Revit C# .NET Add-In generating DirectShape elements to represent room volumes" )]
[assembly: AssemblyConfiguration( "" )]
[assembly: AssemblyCompany( "Autodesk Inc." )]
[assembly: AssemblyProduct( "RoomVolumeDirectShape Revit C# .NET Add-In" )]
[assembly: AssemblyCopyright( "Copyright 2019 (C) Jeremy Tammik, Autodesk Inc." )]
[assembly: AssemblyTrademark( "" )]
[assembly: AssemblyCulture( "" )]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible( false )]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid( "321044f7-b0b2-4b1c-af18-e71a19252be0" )]

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
//
// History:
//
// 2019-05-27 2020.0.0.0 initial rough draft ready for testing and refining
// 2019-05-27 2020.0.0.1 implemented first working version
// 2019-05-27 2020.0.0.2 properly set direct shape `Name` property
// 2019-06-26 2020.0.0.3 fixing for Forge viewer, updated for eason
// 2019-06-26 2020.0.0.4 the solid returned by Room.GetClosedShell does not display in the Forge viewer; implemented CopyGeometry to create a new solid to replace it; triangles work; EdgeLoops does not; GetEdgesAsCurveLoops appeared to work
// 2019-06-27 2020.0.0.5 the solid copied from the room closed shell generated using GetEdgesAsCurveLoops does not display properly in the Forge viewer; reverted to triangulation again
// 2019-06-27 2020.0.0.6 added assertions
// 2019-06-27 2020.0.0.6 created a new solid from the room closed shell using SolidUtils.TessellateSolidOrShell
// 2019-06-27 2020.0.0.7 added code to generate glTF facet data
// 2019-06-27 2020.0.0.7 store glTF facet data to binary file
// 2019-06-29 2020.0.0.8 implemented gltf data export for multiple rooms
// 2019-06-29 2020.0.0.9 corrected min max calculation
// 2019-06-29 2020.0.0.10 corrected min max calculation
//
[assembly: AssemblyVersion( "2020.0.0.10" )]
[assembly: AssemblyFileVersion( "2020.0.0.10" )]
