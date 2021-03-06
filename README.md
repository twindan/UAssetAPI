# UAsset API
A .uasset API written in C# to facilitate reading and modifying game assets.

## A quick example
```cs
// The goal is to modify a ChildSlotComponent in the Seat3_Large asset in order to rotate and translate the slots so that the seat faces forward
AssetWriter y = new AssetWriter(@"C:\Users\Alexandros\Desktop\astroneer_tinkering\pak\Astro\Content\Components_Large\Seat3_Large.uasset", null, null); // no skips, no force reads
Console.WriteLine("Data preserved: " + (y.VerifyParsing() ? "YES" : "NO"));
{
    Category baseUs = y.data.categories[17]; // Category 18 is the ChildSlotComponent we want to modify
    if (baseUs is NormalCategory us)
    {
        // Here we make a new StructProperty that we'll insert in the ChildSlotComponent category to translate it
        var newStruc = new StructPropertyData("RelativeLocation", y.data)
        {
            StructType = "Vector",
            Value = new List<PropertyData>
            {
                new VectorPropertyData("Vector", y.data)
                {
                    Value = new float[] { 0, 175, 100 } // Translation in unreal units (X, Y, Z)
                }
            }
        };

        for (int j = 0; j < us.Data.Count; j++)
        {
            PropertyData me = us.Data[j];
            if (me.Name == "RelativeRotation" && me is StructPropertyData struc)
            {
                IList<PropertyData> cosa = struc.Value;
                for (int m = 0; m < cosa.Count; m++)
                {
                    PropertyData algo = cosa[m];
                    if (algo is RotatorPropertyData)
                    {
                        float[] newRot = ((RotatorPropertyData)algo).Value;
                        newRot[0] += 90; // Rotate 90 degrees pitch
                        newRot[2] += 180; // Rotate 180 degrees roll
                        break;
                    }
                }

                us.Data.Insert(j, newStruc); // The position of the new StructProperty in the category doesn't actually matter
                break;
            }
        }
    }

    // Save to a separate directory for packing
    Directory.CreateDirectory(Path.GetDirectoryName(y.path.Replace("pak", "mod")));
    y.Write(y.path.Replace("pak", "mod"));
}
```