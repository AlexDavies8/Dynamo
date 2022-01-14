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

namespace Dynamo.Model
{
    [Node("Maths/Scalar Math", 250)]
    public class MathNode : ExecutableNode
    {
        [Port("A", true, typeof(float), typeof(FloatPropertyEditor))]
        public float A = 0f;
        
        [Port("B", true, typeof(float), typeof(FloatPropertyEditor))]
        public float B = 0f;

        [Port("Operation", true, typeof(ScalarOperation), typeof(EnumPropertyEditor))]
        public ScalarOperation Operation = ScalarOperation.Add;

        [Port("Result", false, typeof(float), typeof(FloatPropertyEditor))]
        public float Result;

        public MathNode(Guid guid, Flowchart owner) : base(guid, owner)
        {
        }

        public override void Execute()
        {
            Result = Operation switch
            {
                ScalarOperation.Add => A + B,
                ScalarOperation.Subtract => A - B,
                ScalarOperation.Multiply => A * B,
                ScalarOperation.Divide => A / B,
                ScalarOperation.Min => A > B ? B : A,
                ScalarOperation.Max => A > B ? A : B,
                _ => A
            };
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("A", A.ToString());
            writer.WriteAttributeString("B", A.ToString());
            writer.WriteAttributeString("Operation", Operation.ToString());

            base.WriteXml(writer);
        }

        public override void ReadXml(XmlReader reader)
        {
            A = float.Parse(reader.GetAttribute("A"));
            B = float.Parse(reader.GetAttribute("B"));
            Operation = (ScalarOperation)Enum.Parse(typeof(ScalarOperation), reader.GetAttribute("Operation"));

            base.ReadXml(reader);
        }

        public enum ScalarOperation
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            Min,
            Max
        }
    }
}
