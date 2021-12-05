using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Microsoft.DirectX;

namespace HMapEdit {
  public enum ePolygon {
    None,
    Bounding,
    Water,
  }

  public enum eWater {
    River,
    Swamp,
    Lake,
    Lava,
    Ocean,
    DeepOcean,
  }

  public class Polygon {
    public static List<Polygon> Polygons = new List<Polygon>();
    public List<Vector2> Points = new List<Vector2>();

    private string m_WMultiTexture = "";
    private int m_WTesselation = 1;
    private string m_WTexture = "";
    private eWater m_WType = eWater.River;

    [Category("Edit Polygon")]
    public ePolygon Type { get; set; }

    [Category("Edit Polygon")]
    public Color Color {
      get {
        switch (Type) {
          case ePolygon.Water:
            switch (WType) {
              default:
              case eWater.River:
                return Color.Blue;
              case eWater.Lava:
                return Color.Red;
              case eWater.Swamp:
                return Color.FromArgb(64, 200, 64);
              case eWater.Lake:
                return Color.CornflowerBlue;
              case eWater.Ocean:
              case eWater.DeepOcean:
                return Color.DarkBlue;
            }
          case ePolygon.Bounding:
            return Color.Red;
          default:
            return Color.Gray;
        }
      }
    }

    [Category("Water")]
    public string WTexture {
      get { return m_WTexture; }
      set { m_WTexture = value; }
    }

    [Category("Water")]
    public string WMultiTexture {
      get { return m_WMultiTexture; }
      set { m_WMultiTexture = value; }
    }

    [Category("Water")]
    public int WFlow { get; set; }

    [Category("Water")]
    public int WHeight { get; set; }

    [Category("Water")]
    public int WTesselation {
      get { return m_WTesselation; }
      set { m_WTesselation = value; }
    }

    [Category("Water")]
    public eWater WType {
      get { return m_WType; }
      set { m_WType = value; }
    }
  }
}