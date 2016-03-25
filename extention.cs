using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

////////////////////////////////////////////////////////////////////////////

/// <summary publish="true">
/// Extention methods for different class objects.
/// 
/// </summary>
/// <remarks> 
/// Copyright © 2001-2015 Jasem Y. Al-Shamlan (info@ia.com.kw), Internet Applications - Kuwait. All Rights Reserved.
///
/// This library is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by
/// the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
///
/// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License along with this library. If not, see http://www.gnu.org/licenses.
/// 
/// Copyright notice: This notice may not be removed or altered from any source distribution.
/// </remarks>
/// </summary>
public static class Extention
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public static T XmlDeserializeFromString<T>(this string objectData)
    {
        return (T)XmlDeserializeFromString(objectData, typeof(T));
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public static object XmlDeserializeFromString(this string objectData, Type type)
    {
        var serializer = new XmlSerializer(type);
        object result;

        using (TextReader reader = new StringReader(objectData))
        {
            result = serializer.Deserialize(reader);
        }

        return result;
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Extends strings to return bool values
    /// <see cref="http://stackoverflow.com/questions/16205436/convert-toboolean-fails-with-0-value"/>
    /// </summary>
    public static bool ToBool(this string str)
    {
        bool b;
        string cleanValue;

        cleanValue = (str ?? "").Trim();

        if (string.Equals(cleanValue, "False", StringComparison.OrdinalIgnoreCase)) b = false;
        else b = (string.Equals(cleanValue, "True", StringComparison.OrdinalIgnoreCase)) || (cleanValue != "0");

        return b;
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public static bool IsInMenuPath(this SiteMapNode thisnode, SiteMapNode node)
    {
        var temp = node;

        while (temp != null && temp != thisnode) temp = temp.ParentNode;

        return temp != null;
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Pick a randome value from List<>
    ///<see cref="http://stackoverflow.com/questions/2019417/access-random-item-in-list"/>
    /// </summary>
    public static T PickRandom<T>(this IEnumerable<T> source)
    {
        return source.PickRandom(1).Single();
    }

    /// <summary/>
    public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
    {
        return source.Shuffle().Take(count);
    }

    /// <summary/>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.OrderBy(x => Guid.NewGuid());
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// List<> get next element or get the first
    ///<see cref="http://stackoverflow.com/questions/776725/list-get-next-element-or-get-the-first"/>
    /// </summary>
    public static IEnumerable<T> PickNext<T>(this IEnumerable<T> source)
    {
        var enumerator = source.GetEnumerator();

        if (!enumerator.MoveNext()) yield break;

        while (true)
        {
            yield return enumerator.Current;

            if (!enumerator.MoveNext()) enumerator = source.GetEnumerator();
        }
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
}
