using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace SNHU_CS499_SlavikPlamadyala
{
    public class myXML
    {
        /* <|Parent| |Attribute| = "|Attribute Value|">
        *      <|Element|>|Element Value|</|Element|>
        * </|Parent|>     
        */

        private XElement xmlElement;
        private string filePath;

        public struct Element
        {
            public string Name;
            public string Value;
        }
        public struct Attribute
        {
            public string Name;
            public string Value;
        }

        public myXML(string _filePath, out retStatus status)
        {
            filePath = _filePath;
            status = LoadFile();
        }
        public myXML(string _fileName, Environment.SpecialFolder folder, out retStatus status)
        {
            filePath = Environment.GetFolderPath(folder) + $"\\{_fileName}";
            status = LoadFile();
        }

        public retStatus LoadFile()
        {
            // Check if file needs to be created

            if (!File.Exists(filePath))
            {
                try
                {
                    using (StreamWriter writer = File.CreateText(filePath))
                    {
                        // Create root element
                        writer.WriteLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
                        writer.WriteLine("<config>");
                        writer.WriteLine("</config>");
                    }
                    return new retStatus();
                }
                catch
                {
                    return new retStatus(retStatus.ReturnCodes.ERR_UNABLE_CREATE_FILE);
                }
            }

            // Attempt to load file from selected file path
            try
            {
                xmlElement = XElement.Load(filePath);
                return new retStatus();
            }
            catch
            {
                return new retStatus(retStatus.ReturnCodes.ERR_BAD_FILEPATH);
            }

        }

        private XElement[] ElementToXElemenet(Element[] elements)
        {
            List<XElement> retElements = new List<XElement>();
            foreach (Element element in elements)
            {
                XElement newElement = new XElement(element.Name, element.Value);
                retElements.Add(newElement);
            }
            return retElements.ToArray();
        }

        #region Check
        /// <summary>
        /// Check if selected parent already exist inside file
        /// </summary>
        /// <param name="parentName">Parent name</param>
        /// <returns></returns>
        public bool HasParent(string parentName)
        {
            if (xmlElement == null) return false;
            IEnumerable<XElement> parents = xmlElement.Elements();

            // Check if name matches
            foreach (XElement parent in parents)
                if (parent.Name == parentName)
                    return true;

            return false;
        }
        /// <summary>
        /// Check if selected attribute name exists
        /// </summary>
        /// <param name="parentName">Parent name of attribute</param>
        /// <param name="attributeName">Attribute name</param>
        /// <returns></returns>
        public bool HasAttribute(string parentName, string attributeName)
        {
            if (xmlElement == null) return false;
            IEnumerable<XElement> parents = xmlElement.Elements();

            // Check if name matches
            foreach (XElement parent in parents)
                if (parent.Name == parentName)
                {
                    foreach (XAttribute attribute in parent.Attributes())
                        if (attribute.Name == attributeName)
                            return true;
                }

            return false;
        }
        /// <summary>
        /// Check if selected attribute name exists
        /// </summary>
        /// <param name="parentName">Parent name of attribute</param>
        /// <param name="attributeName">Attribute name</param>
        /// <param name="attributeValue">Attribute value</param>
        /// <returns></returns>
        public bool HasAttribute(string parentName, string attributeName, string attributeValue)
        {
            if (xmlElement == null) return false;
            IEnumerable<XElement> parents = xmlElement.Elements();

            // Check if name matches
            foreach (XElement parent in parents)
                if (parent.Name == parentName)
                {
                    foreach (XAttribute attribute in parent.Attributes())
                        if (attribute.Name == attributeName && attribute.Value == attributeValue)
                            return true;
                }

            return false;
        }

        public bool HasElement(string parentName, string elementName)
        {
            if (xmlElement == null) return false;
            IEnumerable<XElement> parents = xmlElement.Elements();

            // Check if name matches
            foreach (XElement parent in parents)
                if (parent.Name == parentName)
                {
                    IEnumerable<XElement> xElements = parent.Elements();
                    foreach (XElement element in xElements)
                        if (element.Name.NamespaceName == elementName)
                            return true;
                }

            return false;
        }

        #endregion Check

        #region Write
        /// <summary>
        /// Create parent element with selected name
        /// </summary>
        /// <param name="parentName">Parent element name</param>
        /// <returns></returns>
        public retStatus AddParent(string parentName)
        {
            if (xmlElement == null) return new retStatus(retStatus.ReturnCodes.ERR_XML_NOT_LOADED);
            try
            {
                xmlElement.Add(new XElement(parentName));
                xmlElement.Save(filePath);
                return new retStatus();
            }
            catch
            {
                return new retStatus(retStatus.ReturnCodes.ERR_XML_BAD_PARENT);
            }
        }

        /// <summary>
        /// Create attribute for selected parent
        /// </summary>
        /// <param name="parentName">Parent element name</param>
        /// <param name="attributeName">Attribute name</param>
        /// <param name="attributeValue">Attribute value</param>
        /// <returns></returns>
        public retStatus AddAttribute(string parentName, Attribute attribute)
        {
            if (xmlElement == null) return new retStatus(retStatus.ReturnCodes.ERR_XML_NOT_LOADED);
            try
            {
                retStatus status = GetXElement(parentName, out XElement parent);
                if (status.IsError) return status;
                parent.Add(new XAttribute(attribute.Name, attribute.Value));
                xmlElement.Save(filePath);
                return status;
            }
            catch
            {
                return new retStatus(retStatus.ReturnCodes.ERR_XML_BAD_ATTRIBUTE);
            }

        }

        /// <summary>
        /// Create element for selected parent
        /// </summary>
        /// <param name="parentName">Parent name</param>
        /// <param name="element">Element name and value</param>
        /// <returns></returns>
        public retStatus AddElement(string parentName, Element element)
        {
            if (xmlElement == null) return new retStatus(retStatus.ReturnCodes.ERR_XML_NOT_LOADED);
            try
            {
                retStatus status = GetXElement(parentName, out XElement parent);
                if (status.IsError) return status;
                parent.Add(new XElement(element.Name, element.Value));
                xmlElement.Save(filePath);
                return status;
            }
            catch
            {
                return new retStatus(retStatus.ReturnCodes.ERR_XML_BAD_ELEMENT);
            }
        }

        /// <summary>
        /// Create elements for selcted parent
        /// </summary>
        /// <param name="parentName">Parent name</param>
        /// <param name="elements">Array of elements to be created</param>
        /// <returns></returns>
        public retStatus AddElements(string parentName, Element[] elements)
        {
            if (xmlElement == null) return new retStatus(retStatus.ReturnCodes.ERR_XML_NOT_LOADED);
            try
            {
                retStatus status = GetXElement(parentName, out XElement parent);
                if (status.IsError) return status;

                // Add all elements
                foreach (Element element in elements)
                    parent.Add(new XElement(element.Name, element.Value));


                xmlElement.Save(filePath);
                return new retStatus();
            }
            catch
            {
                return new retStatus(retStatus.ReturnCodes.ERR_XML_BAD_ELEMENT);
            }
        }
        public retStatus ClearParentNodes(string parentName)
        {
            retStatus status = GetXElement(parentName, out XElement parent);
            if (status.IsError) return status;

            parent.RemoveNodes();
            xmlElement.Save(filePath);

            return status;
        }
        #endregion Write

        #region Read

        public retStatus GetAttributes(string parentName, out Attribute[] attributes)
        {
            attributes = new Attribute[0];
            if (xmlElement == null) return new retStatus(retStatus.ReturnCodes.ERR_XML_NOT_LOADED);

            retStatus status = GetXElement(parentName, out XElement parent);
            if (status.IsError) return status;

            List<Attribute> retAttributes = new List<Attribute>();
            foreach (XAttribute attribute in parent.Attributes())
                retAttributes.Add(new Attribute { Name = attribute.Name.LocalName, Value = attribute.Value });

            attributes = retAttributes.ToArray();
            return status;
        }

        public retStatus GetElements(string parentName, out Element[] elements)
        {
            elements = new Element[0];
            if (xmlElement == null) return new retStatus(retStatus.ReturnCodes.ERR_XML_NOT_LOADED);

            retStatus status = GetXElement(parentName, out XElement parent);
            if (status.IsError) return status;

            List<Element> retElements = new List<Element>();
            foreach (XElement element in parent.Elements())
                retElements.Add(new Element { Name = element.Name.LocalName, Value = element.Value });

            elements = retElements.ToArray();

            return status;
        }

        public retStatus GetElementValue(string parentName, string elementName, out Element retElement)
        {
            retElement = new Element();
            if (xmlElement == null) return new retStatus(retStatus.ReturnCodes.ERR_XML_NOT_LOADED);

            retStatus status = GetXElement(parentName, out XElement parent);
            if (status.IsError) return status;

            foreach (XElement element in parent.Elements())
                if (element.Name == elementName)
                {
                    retElement = new Element { Name = elementName, Value = element.Value };
                    return new retStatus();
                }

            return new retStatus(retStatus.ReturnCodes.ERR_XML_XELEMENT_MISSING);
        }

        #endregion Read

        #region Get Object
        private retStatus GetXElement(string parentName, out XElement parent)
        {
            parent = null;
            if (xmlElement == null) return new retStatus(retStatus.ReturnCodes.ERR_XML_NOT_LOADED);
            IEnumerable<XElement> parents = xmlElement.Elements();

            // Check if name matches
            foreach (XElement _parent in parents)
                if (_parent.Name == parentName)
                {
                    parent = _parent;
                    return new retStatus();
                }

            return new retStatus(retStatus.ReturnCodes.ERR_XML_XELEMENT_MISSING);
        }

        #endregion Get Object
    }
}
