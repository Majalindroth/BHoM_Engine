/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.oM.Reflection.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Return the custom description of a C# class member (e.g. property, method, field)")]
        public static string Description(this MemberInfo member)
        {
            DescriptionAttribute attribute = member.GetCustomAttribute<DescriptionAttribute>();

            string desc = "";
            if (attribute != null && !string.IsNullOrWhiteSpace(attribute.Description))
                desc = attribute.Description + Environment.NewLine;

            if (member.ReflectedType != null)
                desc += member.Name + " is a " + member.MemberType.ToString() + " of " + member.ReflectedType.ToText(true);

            return desc;
        }

        /***************************************************/

        [Description("Return the custom description of a C# method argument")]
        public static string Description(this ParameterInfo parameter)
        {
            IEnumerable<InputAttribute> inputDesc = parameter.Member.GetCustomAttributes<InputAttribute>().Where(x => x.Name == parameter.Name);

            string desc = "";
            if (inputDesc.Count() > 0)
            {
                desc = inputDesc.First().Description + Environment.NewLine;
            }
            if (parameter.ParameterType != null)
            {
                desc += parameter.ParameterType.Description();
            }
            return desc;
        }

        /***************************************************/

        [Description("Return the custom description of a C# class")]
        public static string Description(this Type type)
        {
            if (type == null)
            {
                return "";
            }

            DescriptionAttribute attribute = type.GetCustomAttribute<DescriptionAttribute>();

            string desc = "";

            if (attribute != null)
                desc = attribute.Description + Environment.NewLine;

            //Add the default description
            desc += "This is a " + type.ToText();

            Type innerType = type;

            while (typeof(IEnumerable).IsAssignableFrom(innerType) && innerType.IsGenericType)
                innerType = innerType.GenericTypeArguments.First();

            if (innerType.IsInterface)
            {
                desc += ":";
                List<Type> t = innerType.ImplementingTypes();
                int m = Math.Min(15, t.Count);

                for (int i = 0; i < m; i++)
                    desc += $"{t[i].ToText()}, ";

                if (t.Count > m)
                    desc += "and more...";
                else
                    desc = desc.Remove(desc.Length - 2, 2);

                return desc;
            }

            return desc;

        }

        /***************************************************/

        [Description("Return the custom description of a C# element such as Type, MemberInfo, and ParamaterInfo")]
        [Input("item", "This item can either be a Type, a MemberInfo, or a ParamaterInfo")]
        public static string IDescription(this object item)
        {
            if (item is ParameterInfo)
                return Description(item as ParameterInfo);
            else if (item is Type)
                return Description(item as Type);
            else if (item is MemberInfo)
                return Description(item as MemberInfo);
            else
                return "";
        }

        /***************************************************/
    }
}
