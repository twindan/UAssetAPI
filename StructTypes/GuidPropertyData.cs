﻿using System;
using System.IO;
using UAssetAPI.PropertyTypes;

namespace UAssetAPI.StructTypes
{
    public class GuidPropertyData : PropertyData<Guid>
    {
        public GuidPropertyData(string name, AssetReader asset) : base(name, asset)
        {
            Type = "Guid";
        }

        public GuidPropertyData()
        {
            Type = "Guid";
        }

        public override void Read(BinaryReader reader, bool includeHeader, long leng)
        {
            if (includeHeader)
            {
                reader.ReadByte();
            }

            Value = new Guid(reader.ReadBytes(16));
        }

        public override int Write(BinaryWriter writer, bool includeHeader)
        {
            if (includeHeader)
            {
                writer.Write((byte)0);
            }

            writer.Write(Value.ToByteArray());
            return 16;
        }

        public override string ToString()
        {
            return Convert.ToString(Value);
        }

        public override void FromString(string[] d)
        {
            if (Guid.TryParse(d[0], out Guid res)) Value = res;
        }
    }
}