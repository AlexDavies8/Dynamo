﻿using NodeGraph.Model;
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
    [Node("Transform/Scale", 300)]
    public class ResizeImageNode : ExecutableNode
    {
        [Port("Image", true, typeof(Image<Rgba32>), null, false)]
        public Image<Rgba32> Input;

        [Port("Position", true, typeof(PositionType), typeof(EnumPropertyEditor), false)]
        public PositionType PositionType = PositionType.Fractional;

        [Port("X Scale", true, typeof(float), typeof(FloatPropertyEditor))]
        public float XScale = 1.0f;

        [Port("Y Scale", true, typeof(float), typeof(FloatPropertyEditor))]
        public float YScale = 1.0f;

        [Port("Resize Mode", true, typeof(ResizeMode), typeof(EnumPropertyEditor), false)]
        public ResizeMode ResizeMode = ResizeMode.Stretch;

        [Port("Result", false, typeof(Image<Rgba32>), null)]
        public Image<Rgba32> Output;

        public ResizeImageNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            if (Input == null) return;

            int x = PositionType.GetPixelPosition(XScale, Input.Width);
            int y = PositionType.GetPixelPosition(YScale, Input.Height);

            Output = Input.Clone(image => image.Resize(new ResizeOptions() { Size = new Size(x, y), Mode = ResizeMode }));
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Position", PositionType.ToString());
            writer.WriteAttributeString("XScale", XScale.ToString());
            writer.WriteAttributeString("YScale", YScale.ToString());
            writer.WriteAttributeString("Mode", ResizeMode.ToString());

            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            PositionType = (PositionType)Enum.Parse(typeof(PositionType), reader.GetAttribute("Position"));
            XScale = float.Parse(reader.GetAttribute("XScale"));
            YScale = float.Parse(reader.GetAttribute("YScale"));
            ResizeMode = (ResizeMode)Enum.Parse(typeof(ResizeMode), reader.GetAttribute("Mode"));

            base.ReadXml(reader);
        }
    }
}
