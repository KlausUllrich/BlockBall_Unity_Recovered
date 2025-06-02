using System;
using System.Collections.Generic;

using System.Text;
using System.Xml;
using System.Globalization;
using UnityEngine;

public class XmlUtils
{
    //-----------------------------------------------------------------------------------------------------------------------------------------
    static readonly string[] Seperators = new string[] { " " };

    //-----------------------------------------------------------------------------------------------------------------------------------------
    public static bool XmlAttributeExists(XmlElement pXmlElement, string sAttributeName)
    {
        return pXmlElement.HasAttribute(sAttributeName);
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    public static void SetXmlAttributeString(XmlDocument pDoc, XmlElement pXmlElement, string sAttributeName, string sValue)
    {
        pXmlElement.Attributes.Append(pDoc.CreateAttribute(sAttributeName)).InnerText = sValue;
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    public static string GetXmlAttributeString(XmlElement pXmlElement, string sAttributeName)
    {
        return GetXmlAttributeString(pXmlElement, sAttributeName, string.Empty);
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    public static string GetXmlAttributeString(XmlElement pXmlElement, string sAttributeName, string sDefault)
    {
        if (pXmlElement.HasAttribute(sAttributeName))
        {
            return pXmlElement.Attributes[sAttributeName].InnerText;
        }
        else
        {
            return sDefault;
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    public static void SetXmlAttributeInt(XmlDocument pDoc, XmlElement pXmlElement, string sAttributeName, int iValue)
    {
        SetXmlAttributeString(pDoc, pXmlElement, sAttributeName, iValue.ToString(CultureInfo.InvariantCulture.NumberFormat));
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    public static int GetXmlAttributeInt(XmlElement pXmlElement, string sAttributeName, int iDefault)
    {
        if (pXmlElement.HasAttribute(sAttributeName))
        {
            return Int32.Parse(pXmlElement.Attributes[sAttributeName].InnerText, CultureInfo.InvariantCulture.NumberFormat);
        }
        else
        {
            return iDefault;
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    public static void SetXmlAttributeEnum<T>(XmlDocument pDoc, XmlElement pXmlElement, string sAttributeName, T eValue)
    {
        SetXmlAttributeString(pDoc, pXmlElement, sAttributeName, eValue.ToString());
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    public static T GetXmlAttributeEnum<T>(XmlElement pXmlElement, string sAttributeName, T eDefault)
    {
        return (T) Enum.Parse(typeof(T), GetXmlAttributeString(pXmlElement, sAttributeName, eDefault.ToString()), true);
    }
        
    //-----------------------------------------------------------------------------------------------------------------------------------------
    public static void SetXmlAttributeLong(XmlDocument pDoc, XmlElement pXmlElement, string sAttributeName, long iValue)
    {
        SetXmlAttributeString(pDoc, pXmlElement, sAttributeName, iValue.ToString(CultureInfo.InvariantCulture.NumberFormat));
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    public static long GetXmlAttributeLong(XmlElement pXmlElement, string sAttributeName, long lDefault)
    {
        if (pXmlElement.HasAttribute(sAttributeName))
        {
            return (long)Int64.Parse(pXmlElement.Attributes[sAttributeName].InnerText, CultureInfo.InvariantCulture.NumberFormat);
        }
        else
        {
            return lDefault;
        }
    }
        
    //-----------------------------------------------------------------------------------------------------------------------------------------
    public static void SetXmlAttributeFloat(XmlDocument pDoc, XmlElement pXmlElement, string sAttributeName, float fValue)
    {
        SetXmlAttributeString(pDoc, pXmlElement, sAttributeName, fValue.ToString(CultureInfo.InvariantCulture.NumberFormat));
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    public static float GetXmlAttributeFloat(XmlElement pXmlElement, string sAttributeName, float fDefault)
    {
        if (pXmlElement.HasAttribute(sAttributeName))
        {
            return System.Single.Parse(pXmlElement.Attributes[sAttributeName].InnerText, CultureInfo.InvariantCulture.NumberFormat);
        }
        else
        {
            return fDefault;
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    public static void SetXmlAttributeBool(XmlDocument pDoc, XmlElement pXmlElement, string sAttributeName, bool bValue)
    {
        SetXmlAttributeString(pDoc, pXmlElement, sAttributeName, bValue ? "true" : "false");
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    public static bool GetXmlAttributeBool(XmlElement pXmlElement, string sAttributeName, bool bDefault)
    {
        if (pXmlElement.HasAttribute(sAttributeName))
        {
            string sValue = pXmlElement.Attributes[sAttributeName].InnerText;
            return sValue == "true" || sValue == "True" || sValue == "TRUE" || sValue == "yes" || sValue == "1";
        }
        else
        {
            return bDefault;
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    public static void SetXmlAttributeVector2(XmlDocument pDoc, XmlElement pXmlElement, string sAttributeName, float x, float y)
    {
        SetXmlAttributeString(pDoc, pXmlElement, sAttributeName, string.Format(CultureInfo.InvariantCulture.NumberFormat, "{0} {1}", x, y));
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    public static void SetXmlAttributeVector2(XmlDocument pDoc, XmlElement pXmlElement, string sAttributeName, Vector2 v)
    {
        SetXmlAttributeString(pDoc, pXmlElement, sAttributeName, string.Format(CultureInfo.InvariantCulture.NumberFormat, "{0} {1}", v.x, v.y));
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    public static Vector2 GetXmlAttributeVector2(XmlElement pXmlElement, string sAttributeName, Vector2 xDefault)
    {
        if (pXmlElement.HasAttribute(sAttributeName))
        {
            string sPoint = pXmlElement.Attributes[sAttributeName].InnerText;
            string[] asPivotElements = sPoint.Split(Seperators, StringSplitOptions.RemoveEmptyEntries);
            if (asPivotElements.GetLength(0) != 2)
            {
                return xDefault;
            }

            return new Vector2( Single.Parse(asPivotElements[0], CultureInfo.InvariantCulture.NumberFormat), 
                                Single.Parse(asPivotElements[1], CultureInfo.InvariantCulture.NumberFormat));
        }
        else
        {
            return xDefault;
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    public static void SetXmlAttributeVector3(XmlDocument pDoc, XmlElement pXmlElement, string sAttributeName, float x, float y, float z)
    {
        SetXmlAttributeString(pDoc, pXmlElement, sAttributeName, string.Format(CultureInfo.InvariantCulture.NumberFormat, "{0} {1} {2}", x, y, z));
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    public static void SetXmlAttributeVector3(XmlDocument pDoc, XmlElement pXmlElement, string sAttributeName, Vector3 v)
    {
        SetXmlAttributeString(pDoc, pXmlElement, sAttributeName, string.Format(CultureInfo.InvariantCulture.NumberFormat, "{0} {1} {2}", v.y, v.y, v.z));
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    public static Vector3 GetXmlAttributeVector3(XmlElement pXmlElement, string sAttributeName, Vector3 xDefault)
    {
        if (pXmlElement.HasAttribute(sAttributeName))
        {
            string sPoint = pXmlElement.Attributes[sAttributeName].InnerText;
            string[] asPivotElements = sPoint.Split(Seperators, StringSplitOptions.RemoveEmptyEntries);
            if (asPivotElements.GetLength(0) != 3)
            {
                return xDefault;
            }

            return new Vector3( Single.Parse(asPivotElements[0], CultureInfo.InvariantCulture.NumberFormat), 
                                Single.Parse(asPivotElements[1], CultureInfo.InvariantCulture.NumberFormat), 
                                Single.Parse(asPivotElements[2], CultureInfo.InvariantCulture.NumberFormat));
        }
        else
        {
            return xDefault;
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    public static void SetXmlAttributeQuaternion(XmlDocument pDoc, XmlElement pXmlElement, string sAttributeName, float x, float y, float z, float w)
    {
        SetXmlAttributeString(pDoc, pXmlElement, sAttributeName, string.Format(CultureInfo.InvariantCulture.NumberFormat, "{0} {1} {2} {3}", x, y, z, w));
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    public static void SetXmlAttributeQuaternion(XmlDocument pDoc, XmlElement pXmlElement, string sAttributeName, Quaternion q)
    {
        SetXmlAttributeString(pDoc, pXmlElement, sAttributeName, string.Format(CultureInfo.InvariantCulture.NumberFormat, "{0} {1} {2} {3}", q.x, q.y, q.z, q.w));
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    public static Quaternion GetXmlAttributeQuaternion(XmlElement pXmlElement, string sAttributeName, Quaternion xDefault)
    {
        if (pXmlElement.HasAttribute(sAttributeName))
        {
            string sPoint = pXmlElement.Attributes[sAttributeName].InnerText;
            string[] asPivotElements = sPoint.Split(Seperators, StringSplitOptions.RemoveEmptyEntries);
            if (asPivotElements.GetLength(0) != 4)
            {
                return xDefault;
            }

            return new Quaternion(Single.Parse(asPivotElements[0], CultureInfo.InvariantCulture.NumberFormat),
                                  Single.Parse(asPivotElements[1], CultureInfo.InvariantCulture.NumberFormat),
                                  Single.Parse(asPivotElements[2], CultureInfo.InvariantCulture.NumberFormat),
                                  Single.Parse(asPivotElements[3], CultureInfo.InvariantCulture.NumberFormat));
        }
        else
        {
            return xDefault;
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------------------------
}
