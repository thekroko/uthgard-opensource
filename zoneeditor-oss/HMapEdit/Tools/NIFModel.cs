using System.IO;
using HMapEdit.NiLib;
using Microsoft.DirectX;
using MNL;
namespace HMapEdit.Tools {
  /// <summary>
  /// SlimDX Nif Model
  /// </summary>
  public class NIFModel {
    private readonly OBJModel _objModel;

    /// <summary>
    /// Loads a nif model
    /// </summary>
    /// <param name="nif"></param>
    public NIFModel(Stream nif) {
      NiFile nifile;
      using (var r = new BinaryReader(nif)) {
        nifile = new NiFile(r);
      }
      var obj = Ni2Obj.ToObj(nifile);
      using (var ms = new MemoryStream()) {
        obj.Save(new StreamWriter(ms));
        ms.Position = 0;
        _objModel = OBJLoader.Load(new StreamReader(ms));
      }
    }

    /// <summary>
    /// Renders the model
    /// </summary>
    public void Render() {
      if (_objModel != null)
        _objModel.Render();
    }

    /// <summary>
    /// Checks whether the ray intersects the mesh or not
    /// </summary>
    /// <param name="src"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    public bool Intersect(Vector3 src, Vector3 dir) {
      return _objModel == null ? false : _objModel.Intersect(src, dir);
    }
  }
}