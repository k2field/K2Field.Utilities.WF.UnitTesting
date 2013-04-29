using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;

namespace K2Field.Utilities.Testing.WF.Core
{
    public static class XmlHelper
    {
        public enum ReturnCanBeEmpty
        {
            Yes,
            No
        }

        public enum NameCaseSensitive
        {
            Yes,
            No
        }

        public static string GetAttributeValue(XmlNode node, string attributeName)
        {
            return GetAttributeValue(node, attributeName, string.Empty);
        }

        public static string GetAttributeValue(XmlNode node, string attributeName, NameCaseSensitive caseSensitive)
        {
            if (caseSensitive == NameCaseSensitive.No)
            {
                foreach (XmlAttribute attr in node.Attributes)
                {
                    if (attr.Name.Equals(attributeName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return attr.Value;
                    }
                }
            }
            return GetAttributeValue(node, attributeName, string.Empty);
        }

        public static string GetAttributeValue(XmlNode node, string attributeName, string valueIfNotFoundOrBlank)
        {
            #region Argument Check
            if (node == null)
            {
                throw new ArgumentException(string.Format("null node passed trying to get {0} attribute", attributeName));
            }
            #endregion
            string strReturn = valueIfNotFoundOrBlank;
            //if Node contains a filename attribute and it is populated 
            if (node.Attributes[attributeName] != null)
            {
                strReturn = node.Attributes[attributeName].Value;
            }
            return strReturn;
        }

        public static string GetChildNodeInnerText(XmlNode node, string childNodeName, ReturnCanBeEmpty fieldRequired)
        {

            #region Argument Check
            if (node == null)
            {
                throw new ArgumentException(string.Format("Parent null node passed trying to get '{0}' child node inner test", childNodeName));
            }
            if (node[childNodeName] == null)
            {
                throw new ArgumentException(string.Format("null child node passed trying to get '{0}' child node inner test", childNodeName));
            }
            #endregion

            string retValue = string.Empty;
            if (!string.IsNullOrEmpty(node[childNodeName].InnerText))
            {
                retValue = node[childNodeName].InnerText;
            }
            else
                if (fieldRequired == ReturnCanBeEmpty.No)
                {
                    string errMsg = string.Format("GetChildNodeInnerText would have returned an empty string, but this is not allowed as ReturnCanBeBlank argument is set to No. node:'{0}' childNodeName:'{1}'", node.Name, childNodeName);
                    throw new ArgumentException(errMsg);
                }
            return retValue;
        }

        public static XmlElement GetElement(XmlNode node, string elementName)
        {
            #region Argument Check
            if (node == null)
            {
                throw new ArgumentException(string.Format("null node passed trying to get {0} element", elementName));
            }
            #endregion

            if (node[elementName] != null)
            {
                return node[elementName];
            }
            else
            {
                throw new NullReferenceException(string.Format("{0} node has invalid element {1}",node.Name, elementName));
            }
        }
    }
}
