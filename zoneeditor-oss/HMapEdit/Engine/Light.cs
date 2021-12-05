using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace HMapEdit {
    public class Light {
        public static List<Light> Lights = new List<Light>();
        private LightColor m_Color = LightColor.OrangeYellow;
        private int m_Intensity = 1;

        private int m_ZOffset = 100;

        [Category("Light")]
        public int X { get; set; }

        [Category("Light")]
        public int Y { get; set; }

        [Category("Light")]
        public int Z { get; set; }

        [Category("Light")]
        public int ZOffset {
            get { return m_ZOffset; }
            set { m_ZOffset = value; }
        }

        [Category("Light")]
        public LightColor Color {
            get { return m_Color; }
            set { m_Color = value; }
        }

        [Category("Light")]
        public int Intensity {
            get { return Math.Min(m_Intensity, 3); }
            set { m_Intensity = value; }
        }
    }

    /// <summary>
    /// OuterInner
    /// Intensity = 10er
    /// Intentity 4 => Red
    /// </summary>
    public enum LightColor {
        White = 0,
        BlueWhite = 1,
        RedWhite = 2,
        OrangeWhite = 3,
        Yellow = 4,
        TurqoiseWhite = 5,
        VioletWhite = 6,
        GreenWhite = 7,
        GreenYellow = 8,
        OrangeYellow = 9,
    }
}