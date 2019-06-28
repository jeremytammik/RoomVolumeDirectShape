using Autodesk.Revit.DB.Architecture;
using System;

namespace RoomVolumeDirectShape
{
  /// <summary>
  /// Room data for glTF export:
  /// element id and guid, room name, 
  /// coordinateoffset, coordinatecount, 
  /// vertexindexoffset, vertexcount
  /// (byte or object coount?), 
  /// min and max x, y, z coord values
  /// </summary>
  class RoomData
  {
    public int ElementId { get; set; }
    public string UniqueId { get; set; }
    public string RoomName { get; set; }
    public int MinX { get; set; }
    public int MinY { get; set; }
    public int MinZ { get; set; }
    public int MaxX { get; set; }
    public int MaxY { get; set; }
    public int MaxZ { get; set; }
    public int CoordinatesBegin { get; set; }
    public int CoordinatesCount { get; set; }
    public int TriangleVertexIndexBegin { get; set; }
    public int TriangleVertexIndexCount { get; set; }

    public RoomData( Room r )
    {
      ElementId = r.Id.IntegerValue;
      UniqueId = r.UniqueId;
      RoomName = r.Name;
    }
  }
}
