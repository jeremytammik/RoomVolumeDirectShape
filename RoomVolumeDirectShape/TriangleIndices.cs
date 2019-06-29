using System;

namespace RoomVolumeDirectShape
{
  class TriangleIndices
  {
    public int [] Indices;

    public TriangleIndices( int i, int j, int k )
    {
      Indices = new int[3] { i, j, k };
    }

    /// <summary>
    /// Display as a string.
    /// </summary>
    public override string ToString()
    {
      return string.Format( "({0},{1},{2})", 
        Indices[0], Indices[1], Indices[2] );
    }

  }
}
