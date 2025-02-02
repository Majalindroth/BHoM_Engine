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

using System.Collections.Generic;
using BH.oM.Base;
using BH.oM.Environment.Elements;
using BH.oM.Environment.Fragments;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("copies a Panel into a new object")]
        [Input("panel", "An Environment Panel to copy from")]
        [Output("panel", "The copied Environment Panel")]
        public static Panel Copy(this Panel panel)
        {
            Panel aPanel = panel.GetShallowClone(true) as Panel;
            aPanel.ExternalEdges = new List<Edge>(panel.ExternalEdges);
            aPanel.Openings = new List<Opening>(panel.Openings);
            aPanel.CustomData = new Dictionary<string, object>(panel.CustomData);
            aPanel.Fragments = new List<IBHoMFragment>(panel.Fragments);
            aPanel.ConnectedSpaces = new List<string>(panel.ConnectedSpaces);
            aPanel.Construction = panel.Construction;
            aPanel.Type = panel.Type;
            return aPanel;
        }
    }
}
