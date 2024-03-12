namespace Dragonfly.NetHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Xml.Linq;

    public class ExpandoObjectHelper
    {
        private const string ThisClassName = "Dragonfly.NetHelpers.ExpandoObjectHelper";

        private static List<string> _KnownLists;

        public static void Parse(dynamic parent, XElement node, List<string> KnownLists = null)
        {
            if (KnownLists != null)
            {
                ExpandoObjectHelper._KnownLists = KnownLists;
            }
            IEnumerable<XElement> SortedElements = from XElement elt in node.Elements() orderby node.Elements(elt.Name.LocalName).Count() descending select elt;

            if (node.HasElements)
            {
                int NodeCount = node.Elements(SortedElements.First().Name.LocalName).Count();
                bool FoundNode = false;
                if (ExpandoObjectHelper._KnownLists != null && ExpandoObjectHelper._KnownLists.Count > 0)
                {
                    FoundNode = (from XElement el in node.Elements() where ExpandoObjectHelper._KnownLists.Contains(el.Name.LocalName) select el).Count() > 0;
                }

                if (NodeCount > 1 || FoundNode == true)
                {
                    // At least one of the child elements is a list
                    var Item = new ExpandoObject();
                    List<dynamic> ItemsList = null;
                    string ElementName = string.Empty;
                    foreach (var Element in SortedElements)
                    {
                        if (Element.Name.LocalName != ElementName)
                        {
                            ItemsList = new List<dynamic>();
                            ElementName = Element.Name.LocalName;
                        }

                        if (Element.HasElements ||
                            (ExpandoObjectHelper._KnownLists != null && ExpandoObjectHelper._KnownLists.Contains(Element.Name.LocalName)))
                        {
                            Parse(ItemsList, Element);
                            AddProperty(Item, Element.Name.LocalName, ItemsList);
                        }
                        else
                        {
                            Parse(Item, Element);
                        }
                    }

                    foreach (var Attribute in node.Attributes())
                    {
                        AddProperty(Item, Attribute.Name.ToString(), Attribute.Value.Trim());
                    }

                    AddProperty(parent, node.Name.ToString(), Item);
                }
                else
                {
                    var Item = new ExpandoObject();

                    foreach (var Attribute in node.Attributes())
                    {
                        AddProperty(Item, Attribute.Name.ToString(), Attribute.Value.Trim());
                    }

                    //element
                    foreach (var Element in SortedElements)
                    {
                        Parse(Item, Element);
                    }
                    AddProperty(parent, node.Name.ToString(), Item);
                }
            }
            else
            {
                AddProperty(parent, node.Name.ToString(), node.Value.Trim());
            }
        }

        private static void AddProperty(dynamic parent, string name, object value)
        {
            if (parent is List<dynamic>)
            {
                (parent as List<dynamic>).Add(value);
            }
            else
            {
                (parent as IDictionary<String, object>)[name] = value;
            }
        }
    }
}
