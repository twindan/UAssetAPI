﻿using System;
using System.Diagnostics;
using System.IO;
using UAssetAPI.PropertyTypes;
using UAssetAPI.StructTypes;

namespace UAssetAPI
{
    /*
        Main serializer for most category types
    */

    public static class MainSerializer
    {
#if DEBUG
        private static PropertyData lastType;
#endif

        public static PropertyData TypeToClass(string type, string name, AssetReader asset, BinaryReader reader = null, long leng = 0, bool includeHeader = true)
        {
            //Debug.WriteLine(type);
            PropertyData data;
            switch (type)
            {
                case "BoolProperty":
                    data = new BoolPropertyData(name, asset);
                    break;
                case "Int8Property":
                    data = new Int8PropertyData(name, asset);
                    break;
                case "Int16Property":
                    data = new Int16PropertyData(name, asset);
                    break;
                case "IntProperty":
                    data = new IntPropertyData(name, asset);
                    break;
                case "Int64Property":
                    data = new Int64PropertyData(name, asset);
                    break;
                case "UInt16Property":
                    data = new UInt16PropertyData(name, asset);
                    break;
                case "UInt32Property":
                    data = new UInt32PropertyData(name, asset);
                    break;
                case "UInt64Property":
                    data = new UInt64PropertyData(name, asset);
                    break;
                case "FloatProperty":
                    data = new FloatPropertyData(name, asset);
                    break;
                case "TextProperty":
                    data = new TextPropertyData(name, asset);
                    break;
                case "StrProperty":
                    data = new StrPropertyData(name, asset);
                    break;
                case "ObjectProperty":
                    data = new ObjectPropertyData(name, asset);
                    break;
                case "EnumProperty":
                    data = new EnumPropertyData(name, asset);
                    break;
                case "ByteProperty":
                    data = new BytePropertyData(name, asset);
                    break;
                case "NameProperty":
                    data = new NamePropertyData(name, asset);
                    break;
                case "ArrayProperty":
                    data = new ArrayPropertyData(name, asset);
                    break;
                case "MapProperty":
                    data = new MapPropertyData(name, asset);
                    break;
                case "StructProperty":
                    data = new StructPropertyData(name, asset);
                    break;
                case "Guid":
                    data = new GuidPropertyData(name, asset);
                    break;
                case "LinearColor":
                    data = new LinearColorPropertyData(name, asset);
                    break;
                case "Color":
                    data = new ColorPropertyData(name, asset);
                    break;
                case "Vector":
                    data = new VectorPropertyData(name, asset);
                    break;
                case "Vector2D":
                    data = new Vector2DPropertyData(name, asset);
                    break;
                case "Box":
                    data = new BoxPropertyData(name, asset);
                    break;
                case "IntPoint":
                    data = new IntPointPropertyData(name, asset);
                    break;
                case "DateTime":
                    data = new DateTimePropertyData(name, asset);
                    break;
                case "Timespan":
                    data = new TimespanPropertyData(name, asset);
                    break;
                case "Rotator":
                    data = new RotatorPropertyData(name, asset);
                    break;
                case "Quat":
                    data = new QuatPropertyData(name, asset);
                    break;
                case "Vector4":
                    data = new Vector4PropertyData(name, asset);
                    break;
                case "SoftObjectProperty":
                    data = new SoftObjectPropertyData(name, asset);
                    break;
                case "MulticastDelegateProperty":
                    data = new MulticastDelegatePropertyData(name, asset);
                    break;
                default:
#if DEBUG
                    Debug.WriteLine("-----------");
                    Debug.WriteLine("Parsing unknown type " + type);
                    Debug.WriteLine("Length: " + leng);
                    if (reader != null) Debug.WriteLine("Pos: " + reader.BaseStream.Position);
                    Debug.WriteLine("Last type: " + lastType.Type);
                    if (lastType is StructPropertyData) Debug.WriteLine("Last struct's type was " + ((StructPropertyData)lastType).StructType);
                    Debug.WriteLine("-----------");
#endif
                    if (leng > 0)
                    {
                        data = new UnknownPropertyData(name, asset);
                        data.Type = type;
                    }
                    else
                    {
                        if (reader == null) throw new FormatException("Unknown property type: " + type + " (on " + name + ")");
                        throw new FormatException("Unknown property type: " + type + " (on " + name + " at " + reader.BaseStream.Position + ")");
                    }
                    break;
            }
#if DEBUG
            lastType = data;
#endif
            if (reader != null)
            {
                data.Read(reader, includeHeader, leng);
            }
            return data;
        }

        public static PropertyData Read(AssetReader asset, BinaryReader reader, bool includeHeader)
        {
            string name = asset.GetHeaderReference((int)reader.ReadInt32());
            int widgetData = reader.ReadInt32();
            if (name.Equals("None")) return null;

            //Debug.WriteLine(name);
            int typeNum = (int)reader.ReadInt64();
            string type = name;
            if (typeNum > 0) type = asset.GetHeaderReference(typeNum);

            long leng = reader.ReadInt64();
            PropertyData result = TypeToClass(type, name, asset, reader, leng, includeHeader);
            result.WidgetData = widgetData;
            return result;
        }

        public static int Write(PropertyData property, AssetReader asset, BinaryWriter writer, bool includeHeader) // Returns location of the length
        {
            writer.Write((int)asset.SearchHeaderReference(property.Name));
            writer.Write(property.WidgetData);
            writer.Write((long)asset.SearchHeaderReference(property.Type));
            int oldLoc = (int)writer.BaseStream.Position;
            writer.Write((long)0); // initial length
            int realLength = property.Write(writer, includeHeader);
            int newLoc = (int)writer.BaseStream.Position;

            writer.Seek(oldLoc, SeekOrigin.Begin);
            writer.Write((long)realLength);
            writer.Seek(newLoc, SeekOrigin.Begin);
            return oldLoc;
        }
    }
}
