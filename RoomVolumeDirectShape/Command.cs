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
    /// <summary>
    /// Category assigned to the room volume direct shape
    /// </summary>
    ElementId _id_category_for_direct_shape 
      = new ElementId( BuiltInCategory.OST_Rooms );

    /// <summary>
    /// DirectShape parameter to populate with JSON
    /// dictionary containing all room properies
    /// </summary>
    BuiltInParameter _bip_properties 
      = BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS;

    static string GetElementPropertiesJson( Element e )
    {
      return string.Empty;
    }

    /// <summary>
    /// Return parameter value formatted as string
    /// </summary>
    static string ParameterToString( Parameter p )
    {
      string val = "none";

      if( p == null )
      {
        return val;
      }

      // To get to the parameter value, we need to pause it depending on its storage type

      switch( p.StorageType )
      {
        case StorageType.Double:
          double dVal = p.AsDouble();
          val = dVal.ToString();
          break;
        case StorageType.Integer:
          int iVal = p.AsInteger();
          val = iVal.ToString();
          break;
        case StorageType.String:
          string sVal = p.AsString();
          val = sVal;
          break;
        case StorageType.ElementId:
          ElementId idVal = p.AsElementId();
          val = idVal.IntegerValue.ToString();
          break;
        case StorageType.None:
          break;
      }
      return val;
    }

    /// <summary>
    /// Return all the parameter values  
    /// deemed relevant for the given element
    /// in string form.
    /// </summary>
    List<string> GetParamValues( Element e )
    {
      // Two choices: 
      // Element.Parameters property -- Retrieves 
      // a set containing all  the parameters.
      // GetOrderedParameters method -- Gets the 
      // visible parameters in order.

      IList<Parameter> ps = e.GetOrderedParameters();

      List<string> param_values = new List<string>(
        ps.Count );

      foreach( Parameter p in ps )
      {
        // AsValueString displays the value as the 
        // user sees it. In some cases, the underlying
        // database value returned by AsInteger, AsDouble,
        // etc., may be more relevant.

        param_values.Add( string.Format( "{0}={1}",
          p.Definition.Name, p.AsValueString() ) );
      }
      return param_values;
    }

    static string GetRoomPropertiesJson( Room r )
    {
      return GetElementPropertiesJson( r );

      double baseOffset = r.BaseOffset;
      double limitOffset = r.LimitOffset;
      double unboundedHeight = r.UnboundedHeight;
      string upperLimit = r.UpperLimit.Name;
      double volume = r.Volume;

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

      string id_addin = uiapp.ActiveAddInId.ToString();
      
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
          string json_properties = GetRoomPropertiesJson( r );

          DirectShape ds = DirectShape.CreateElement( 
            doc, _id_category_for_direct_shape );

          ds.ApplicationId = id_addin;
          ds.ApplicationDataId = r.UniqueId;
          ds.SetShape( geo.ToList<GeometryObject>() );
          ds.get_Parameter( _bip_properties ).Set( json_properties );
        }

        tx.Commit();
      }

      return Result.Succeeded;
    }
  }
}
