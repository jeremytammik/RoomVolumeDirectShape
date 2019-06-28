using System;

namespace RoomVolumeDirectShape
{
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

    // room data: element guid, room name, coordinateoffset, coordinatecount, vertexindexoffset, vertexcount( byte and object coount ), min and max x, y, z values


  }
}
