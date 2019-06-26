#region Namespaces
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
#endregion

namespace RoomVolumeDirectShape
{
  [Transaction( TransactionMode.Manual )]
  public class Command : IExternalCommand
  {
    // Cannot use OST_Rooms; DirectShape.CreateElement 
    // throws ArgumentExceptionL: Element id categoryId 
    // may not be used as a DirectShape category.

    /// <summary>
    /// Category assigned to the room volume direct shape
    /// </summary>
    ElementId _id_category_for_direct_shape
      = new ElementId( BuiltInCategory.OST_GenericModel );

    /// <summary>
    /// DirectShape parameter to populate with JSON
    /// dictionary containing all room properies
    /// </summary>
    BuiltInParameter _bip_properties
      = BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS;

    /// <summary>
    /// Return a JSON string representing a dictionary
    /// mapping string key to string value.
    /// </summary>
    static string FormatDictAsJson( 
      Dictionary<string, string> d )
    {
      List<string> keys = new List<string>( d.Keys );
      keys.Sort();

      List<string> key_vals = new List<string>( 
        keys.Count );

      foreach( string key in keys )
      {
        key_vals.Add( 
          string.Format( "\"{0}\" : \"{1}\"",
            key, d[key] ) );
      }
      return "{" + string.Join( ", ", key_vals ) + "}";
    }

    /// <summary>
    /// Return parameter storage type abbreviation
    /// </summary>
    static char ParameterStorageTypeChar(
      Parameter p )
    {
      if( null == p )
      {
        throw new ArgumentNullException(
          "p", "expected non-null parameter" );
      }

      char abbreviation = '?';

      switch( p.StorageType )
      {
        case StorageType.Double:
          abbreviation = 'r'; // real number
          break;
        case StorageType.Integer:
          abbreviation = 'n'; // integer number
          break;
        case StorageType.String:
          abbreviation = 's'; // string
          break;
        case StorageType.ElementId:
          abbreviation = 'e'; // element id
          break;
        case StorageType.None:
          throw new ArgumentOutOfRangeException(
            "p", "expected valid parameter "
            + "storage type, not 'None'" );
      }
      return abbreviation;
    }

    /// <summary>
    /// Return parameter value formatted as string
    /// </summary>
    static string ParameterToString( Parameter p )
    {
      string s = "null";

      if( p != null )
      {
        switch( p.StorageType )
        {
          case StorageType.Double:
            s = p.AsDouble().ToString( "0.##" );
            break;
          case StorageType.Integer:
            s = p.AsInteger().ToString();
            break;
          case StorageType.String:
            s = p.AsString();
            break;
          case StorageType.ElementId:
            s = p.AsElementId().IntegerValue.ToString();
            break;
          case StorageType.None:
            s = "none";
            break;
        }
      }
      return s;
    }

    /// <summary>
    /// Return all the element parameter values in a
    /// dictionary mapping parameter names to values
    /// </summary>
    static Dictionary<string, string> GetParamValues(
      Element e )
    {
      // Two choices: 
      // Element.Parameters property -- Retrieves 
      // a set containing all the parameters.
      // GetOrderedParameters method -- Gets the 
      // visible parameters in order.

      //IList<Parameter> ps = e.GetOrderedParameters();

      ParameterSet pset = e.Parameters;

      Dictionary<string, string> d
        = new Dictionary<string, string>( pset.Size );

      foreach( Parameter p in pset )
      {
        // AsValueString displays the value as the 
        // user sees it. In some cases, the underlying
        // database value returned by AsInteger, AsDouble,
        // etc., may be more relevant, as done by 
        // ParameterToString

        string key = string.Format( "{0}({1})",
          p.Definition.Name,
          ParameterStorageTypeChar( p ) );

        string val = ParameterToString( p );

        if( d.ContainsKey( key ) )
        {
          if( d[key] != val )
          {
            d[key] += " | " + val;
          }
        }
        else
        {
          d.Add( key, val );
        }
      }
      return d;
    }

    static string GetRoomPropertiesJson( Room r )
    {
      Dictionary<string, string> param_values
        = GetParamValues( r );

      // These room properties are all stored in 
      // parameters and therefore already captured

      //double baseOffset = r.BaseOffset;
      //double limitOffset = r.LimitOffset;
      //double unboundedHeight = r.UnboundedHeight;
      //string upperLimit = r.UpperLimit.Name;
      //double volume = r.Volume;

      return FormatDictAsJson( param_values );
    }

    public Result Execute(
      ExternalCommandData commandData,
      ref string message,
      ElementSet elements )
    {
      UIApplication uiapp = commandData.Application;
      UIDocument uidoc = uiapp.ActiveUIDocument;
      Application app = uiapp.Application;
      Document doc = uidoc.Document;

      string id_addin = uiapp.ActiveAddInId.GetGUID()
        .ToString();

      IEnumerable<Room> rooms
        = new FilteredElementCollector( doc )
        .WhereElementIsNotElementType()
        .OfClass( typeof( SpatialElement ) )
        .Where( e => e.GetType() == typeof( Room ) )
        .Cast<Room>();

      using( Transaction tx = new Transaction( doc ) )
      {
        tx.Start( "Generate Direct Shape Elements "
          + "Representing Room Volumes" );

        foreach( Room r in rooms )
        {
          Debug.Print( r.Name );

          GeometryElement geo = r.ClosedShell;

          // Convert to IList using LINQ

          IList<GeometryObject> shape 
            = geo.ToList<GeometryObject>();

          // Create IList step by step

          Solid solid = geo.First<GeometryObject>() 
            as Solid;

          // The room closed shell solid faces have a 
          // non-null graphics style id: 
          // Interior Fill 106074
          // The sphere faces' graphics style id is null. 
          // Maybe this graphics style does something 
          // weird in the Forge viewer?
          // Let's create a copy of the room solid and 
          // see whether that resets the graphics style.

          solid = SolidUtils.CreateTransformed( 
            solid, Transform.Identity );

          shape = new GeometryObject[] { solid };

          // Create a sphere

          var profile = new List<Curve>();

          // create shape with 2' radius
          var center = XYZ.Zero;
          double radius = 2.0;

          XYZ profile00 = center;
          var profilePlus = center + new XYZ( 0, radius, 0 );
          var profileMinus = center - new XYZ( 0, radius, 0 );

          profile.Add( Line.CreateBound( profilePlus, profileMinus ) );
          profile.Add( Arc.Create( profileMinus, profilePlus, center + new XYZ( radius, 0, 0 ) ) );

          var curveLoop = CurveLoop.Create( profile );
          var options = new SolidOptions( ElementId.InvalidElementId, ElementId.InvalidElementId );

          var frame = new Frame( center, XYZ.BasisX, -XYZ.BasisZ, XYZ.BasisY );
          var sphere = GeometryCreationUtilities.CreateRevolvedGeometry( frame, new CurveLoop[] { curveLoop }, 0, 2 * Math.PI, options );

          shape = new GeometryObject[] { solid, sphere };

          Dictionary<string, string> param_values
            = GetParamValues( r );

          string json = FormatDictAsJson( param_values );

          DirectShape ds = DirectShape.CreateElement(
            doc, _id_category_for_direct_shape );

          ds.ApplicationId = id_addin;
          ds.ApplicationDataId = r.UniqueId;
          ds.SetShape( shape );
          ds.get_Parameter( _bip_properties ).Set( json );
          ds.Name = "Room volume for " + r.Name;
        }
        tx.Commit();
      }
      return Result.Succeeded;
    }
  }
}
