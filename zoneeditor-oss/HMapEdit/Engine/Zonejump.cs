using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.DirectX;

namespace HMapEdit {
    public class Zonejump {
        public static List<Zonejump> Zonejumps = new List<Zonejump>();
        private int m_EditIndex;

        private string m_Name = "???";

        [Category("Zonejump")]
        public string Name {
            get { return m_Name; }
            set { m_Name = value; }
        }

        [Category("Zonejump")]
        public int ID { get; set; }

        [Category("Zonejump")]
        [TypeConverter(typeof (ExpandableObjectConverter))]
        public Vector3 First { get; set; }

        [Category("Zonejump")]
        [TypeConverter(typeof (ExpandableObjectConverter))]
        public Vector3 Second { get; set; }

        [Category("Zonejump")]
        public int EditIndex {
            get { return m_EditIndex; }
            set {
                if (value > 2) value = 2;

                m_EditIndex = value;
            }
        }
    }
}