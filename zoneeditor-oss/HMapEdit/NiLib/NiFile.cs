using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MNL
{
    public class NiFile
    {
        public const uint INVALID_REF = 0xFFFFFFFF;
        public const string CMD_TOP_LEVEL_OBJECT = "Top Level Object";
        public const string CMD_END_OF_FILE = "End Of File";

        public eNifVersion Version
        {
            get { return Header.Version; }
        }

        public NiHeader Header;
        public NiFooter Footer;

        public Dictionary<uint, NiObject> ObjectsByRef;

        public NiFile(BinaryReader reader)
        {
            Header = new NiHeader(this, reader);
            ReadNiObjects(reader);
            Footer = new NiFooter(this, reader);
            FixRefs();
        }

        private void ReadNiObjects(BinaryReader reader)
        {
            ObjectsByRef = new Dictionary<uint, NiObject>();

            var i = 0;
            while (true)
            {
                var type = "";
                if (Version >= eNifVersion.VER_5_0_0_1)
                {
                    if (Version <= eNifVersion.VER_10_1_0_106)
                    {
                        if (reader.ReadUInt32() != 0)
                        {
                            throw new Exception("Check value is not zero! Invalid file?");
                        }
                    }
                    type = Header.BlockTypes[Header.BlockTypeIndex[i]].Value;
                }
                else
                {
                    var objTypeStrLen = reader.ReadUInt32();
                    if (objTypeStrLen > 30 || objTypeStrLen < 6)
                    {
                        throw new Exception("Invalid object type string length!");
                    }
                    type = new string(reader.ReadChars((int)objTypeStrLen));

                    if (Header.Version < eNifVersion.VER_3_3_0_13)
                    {
                        if (type == CMD_TOP_LEVEL_OBJECT)
                            continue;

                        if (type == CMD_END_OF_FILE)
                            break;
                    }
                }

                uint index;
                if (Version < eNifVersion.VER_3_3_0_13)
                {
                    index = reader.ReadUInt32();
                }
                else
                {
                    index = (uint)i;
                }

                var csType = Type.GetType("MNL." + type);

                if (csType == null)
                {
                    throw new NotImplementedException(type);
                }

                var currentObject = (NiObject)Activator.CreateInstance(csType, new object[] { this, reader });
                ObjectsByRef.Add(index, currentObject);

                if (Version < eNifVersion.VER_3_3_0_13) continue;

                i++;

                if (i >= Header.NumBlocks)
                    break;
            }
        }


        private void FixRefs()
        {
            foreach(var obj in ObjectsByRef.Values)
                FixRefs(obj);
        }

        private void FixRefs(object obj)
        {
            foreach (var field in obj.GetType().GetFields())
            {
                if (field.FieldType.Name.Contains("NiRef"))
                {
                    if (field.FieldType.IsArray)
                    {
                        var values = (IEnumerable) field.GetValue(obj);
                        if (values == null) continue;
                        foreach (dynamic val in values)
                        {
                            //var method = val.GetType().GetMethod("SetRef");
                            //method.Invoke(val, new object[] { this });
                            val.SetRef(this);

                            if (field.Name == "Children")
                            {
                                //var isValidMethod = val.GetType().GetMethod("IsValid");
                                //if ((bool)isValidMethod.Invoke(val, null))
                                if(val.IsValid())
                                {
                                   // var child = val.GetType().GetProperty("Object").GetValue(val) as NiAVObject;
                                    var child = val.Object as NiAVObject;
                                    if (child == null)
                                    {
                                        throw new Exception("no child");
                                    }
                                    child.Parent = (NiNode)obj;
                                }
                            }
                        }
                    }
                    else
                    {
                        //var method = field.FieldType.GetMethod("SetRef");
                        dynamic value = field.GetValue(obj);
                        if (value == null) continue;
                        value.SetRef(this);
                        //method.Invoke(value, new object[] { this });
                    }
                }
            }
        }

        public NiAVObject FindRoot()
        {
            var avObj = ObjectsByRef.Values.OfType<NiAVObject>().Select(obj => obj).FirstOrDefault();

            if (avObj == null)
                return null;

            while (avObj.Parent != null)
                avObj = avObj.Parent;

            return avObj;
        }

        private void PrintNifTree()
        {
            var root = FindRoot();
            if (root == null)
            {
                Console.WriteLine("No Root!");
                return;
            }
            var depth = 0;
            PrintNifNode(root, depth);
        }

        private void PrintNifNode(NiAVObject root, int depth)
        {
            var ds = "";

            for (var i = 0; i < depth; i++)
                ds += "*";

            ds += " ";

            Console.WriteLine(ds + " " + root.Name);

            var niNode = root as NiNode;
            if (niNode != null)
            {
                foreach (var child in niNode.Children)
                {
                    if(child.IsValid())
                        PrintNifNode(child.Object, depth + 1);
                }
            }
        }
    }
}
