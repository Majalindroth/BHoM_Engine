﻿/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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

using BH.oM.Base;
using BH.oM.Data.Collections;
using BH.oM.Diffing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Reflection;
using BH.Engine.Serialiser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using MongoDB.Bson;

namespace BH.Engine.Diffing
{
    public static partial class Convert
    {
        ///***************************************************/
        ///**** Public Methods                            ****/
        ///***************************************************/

        public static byte[] ToDiffingByteArray(this object obj, PropertyInfo[] fieldsToIgnore = null)
        {
            List<PropertyInfo> propList = fieldsToIgnore?.ToList();

            if (propList == null || propList.Count == 0)
                return BsonExtensionMethods.ToBson(obj.ToBsonDocument()); // .ToBsonDocument() for consistency with other cases

            List<string> propNames = new List<string>();
            propList.ForEach(prop => propNames.Add(prop.Name));

            return ToDiffingByteArray(obj, propNames);
        }

        public static byte[] ToDiffingByteArray(this object obj, List<string> fieldsToIgnore)
        {
            if (fieldsToIgnore == null || fieldsToIgnore.Count == 0)
                return BsonExtensionMethods.ToBson(obj);

            var objDoc = obj.ToBsonDocument();

            fieldsToIgnore.ForEach(propName =>
                objDoc.Remove(propName)
            );

            return BsonExtensionMethods.ToBson(objDoc);
        }


        ///***************************************************/

    }
}
