using NodeGraph.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Text;
using Dynamo.Controls.PropertyEditors;
using System.Xml;

namespace Dynamo.Model.Nodes
{
    [Node("Transform/Crop")]
    public class CropImageNode : ExecutableNode
    {
        [Port("Image", true, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> Input;

        [Port("Position", true, typeof(PositionType), typeof(EnumPropertyEditor))]
        public PositionType PositionType = PositionType.Fractional;

        [Port("X Scale", true, typeof(float), typeof(FloatPropertyEditor))]
        public float XScale = 1.0f;

        [Port("Y Scale", true, typeof(float), typeof(FloatPropertyEditor))]
        public float YScale = 1.0f;

        [Port("X Offset", true, typeof(float), typeof(FloatPropertyEditor))]
        public float XOffset = 0.0f;

        [Port("Y Offset", true, typeof(float), typeof(FloatPropertyEditor))]
        public float YOffset = 0.0f;

        [Port("Result", false, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> Output;

        public CropImageNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Input == null) return;

            int x = PositionType.GetPixelPosition(XOffset, Input.Width);
            int y = PositionType.GetPixelPosition(YOffset, Input.Height);
            int width = PositionType.GetPixelPosition(XScale, Input.Width);
            int height = PositionType.GetPixelPosition(YScale, Input.Height);

            Output = Input.Clone(image => image.Crop(new Rectangle(x, y, width, height)));
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Position", PositionType.ToString());
            writer.WriteAttributeString("XScale", XScale.ToString());
            writer.WriteAttributeString("YScale", YScale.ToString());
            writer.WriteAttributeString("XOffset", XOffset.ToString());
            writer.WriteAttributeString("YOffset", YOffset.ToString());

            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            PositionType = (PositionType)Enum.Parse(typeof(PositionType), reader.GetAttribute("Position"));
            XScale = float.Parse(reader.GetAttribute("XScale"));
            YScale = float.Parse(reader.GetAttribute("YScale"));
            XOffset = float.Parse(reader.GetAttribute("XOffset"));
            YOffset = float.Parse(reader.GetAttribute("YOffset"));

            base.ReadXml(reader);
        }
    }
}