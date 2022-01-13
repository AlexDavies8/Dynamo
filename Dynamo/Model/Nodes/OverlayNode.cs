using NodeGraph.Model;
using SixLabors.ImageSharp;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using Dynamo.Controls.PropertyEditors;
using System.Xml;

namespace Dynamo.Model.Nodes
{

    [Node("Combine/Overlay", 400)]
    public class OverlayNode : ExecutableNode
    {
        [Port("Back", true, typeof(Image<Rgba32>), null, false)]
        public Image<Rgba32> Back = null;

        [Port("Front", true, typeof(Image<Rgba32>), null, false)]
        public Image<Rgba32> Front = null;

        [Port("Position", true, typeof(PositionType), typeof(EnumPropertyEditor), false)]
        public PositionType PositionType = PositionType.Fractional;

        [Port("X", true, typeof(float), typeof(FloatPropertyEditor))]
        public float XOffset = 0.0f;

        [Port("Y", true, typeof(float), typeof(FloatPropertyEditor))]
        public float YOffset = 0.0f;

        [Port("Blending Mode", true, typeof(PixelColorBlendingMode), typeof(EnumPropertyEditor), false)]
        public PixelColorBlendingMode BlendingMode = PixelColorBlendingMode.Normal;

        [Port("Opacity", true, typeof(float), typeof(FloatPropertyEditor))]
        public float Opacity = 1.0f;

        [Port("Result", false, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> Result = null;

        public OverlayNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Back == null) return;
            if (Front == null) return;

            Result = Back.Clone(x =>
            {
                x.DrawImage(
                    Front,
                    new Point(
                        PositionType.GetPixelPosition(XOffset, Back.Width),
                        PositionType.GetPixelPosition(YOffset, Back.Height)),
                    BlendingMode,
                    Opacity
                );
            });
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Position", PositionType.ToString());
            writer.WriteAttributeString("XOffset", XOffset.ToString());
            writer.WriteAttributeString("YOffset", YOffset.ToString());
            writer.WriteAttributeString("BlendingMode", BlendingMode.ToString());
            writer.WriteAttributeString("Opacity", Opacity.ToString());

            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            PositionType = (PositionType)Enum.Parse(typeof(PositionType), reader.GetAttribute("Position"));
            XOffset = float.Parse(reader.GetAttribute("XOffset"));
            YOffset = float.Parse(reader.GetAttribute("YOffset"));
            BlendingMode = (PixelColorBlendingMode)Enum.Parse(typeof(PixelColorBlendingMode), reader.GetAttribute("BlendingMode"));
            Opacity = float.Parse(reader.GetAttribute("Opacity"));

            base.ReadXml(reader);
        }
    }
}

