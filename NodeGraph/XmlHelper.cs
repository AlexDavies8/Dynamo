using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace NodeGraph
{
    public static class XmlHelper
    {
        public static void ReadMultiple(this XmlReader reader, string[] groups, params (string, Action<XmlReader>)[] targetElements)
        {
            Dictionary<string, Action<XmlReader>> targetElementsDictionary = new Dictionary<string, Action<XmlReader>>();
            foreach (var pair in targetElements)
                targetElementsDictionary.Add(pair.Item1, pair.Item2);
            ReadMultiple(reader, groups, targetElementsDictionary);
        }

        public static void ReadMultiple(this XmlReader reader, string[] groups, Dictionary<string, Action<XmlReader>> targetElements)
        {
            Dictionary<string, bool> isDone = new Dictionary<string, bool>();
            foreach (var group in groups)
                isDone.Add(group, false);

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                    foreach (var pair in targetElements)
                        if (pair.Key == reader.Name)
                            pair.Value(reader);

                if (reader.NodeType == XmlNodeType.EndElement || reader.IsEmptyElement)
                    foreach (var group in groups)
                        if (group == reader.Name)
                            isDone[group] = true;

                if (!isDone.ContainsValue(false))
                    break;
            }
        }
    }
}
