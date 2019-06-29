using Autodesk.Revit.DB.Architecture;

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
  class GltfNodeData
  {
    public int ElementId { get; set; }
    public string RoomName { get; set; }
    public string UniqueId { get; set; }
    public IntPoint3d Min { get; set; }
    public IntPoint3d Max { get; set; }
    public int CoordinatesBegin { get; set; }
    public int CoordinatesCount { get; set; }
    public int TriangleVertexIndicesBegin { get; set; }
    public int TriangleVertexIndexCount { get; set; }

    public GltfNodeData( Room r )
    {
      ElementId = r.Id.IntegerValue;
      UniqueId = r.UniqueId;
      RoomName = r.Name;
    }

    /// <summary>
    /// Display as a string.
    /// </summary>
    public override string ToString()
    {
      return string.Format( 
        "{0},{1},{2},{3},{4},{5},{6},{7},{8}",
        ElementId,
        UniqueId,
        RoomName,
        Min,
        Max,
        CoordinatesBegin,
        CoordinatesCount,
        TriangleVertexIndicesBegin,
        TriangleVertexIndexCount );
    }
  }
}
