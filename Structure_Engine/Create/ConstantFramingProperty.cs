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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Physical.FramingProperties;
using BH.oM.Structure.SectionProperties;
using BH.oM.Geometry.ShapeProfiles;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a ConstantFramingProperty from a ISectionProperty and orientation angle. Extracts the SectionProfile (if existing) and Structural MaterialFragment and creates a physical material with the same name")]
        [Input("sectionProperty", "Structural section property to extract profile and material from. For explicit sections lacking a profile only the material will get extracted")]
        [Input("orientationAngle", "Defines the sections rotation around its own axis")]
        [Input("name", "Name of the property. If null/empty the name of the section property will be used")]
        [Output("FramingProeprty", "The constructed physical Constant Framing Property to be used with IFramingElements such as Beams/Columns/Bracing")]
        public static ConstantFramingProperty ConstantFramingProperty(ISectionProperty sectionProperty, double orientationAngle, string name = "")
        {

            IProfile profile = null;
            if (sectionProperty is SteelSection)
                profile = (sectionProperty as SteelSection).SectionProfile;
            else if (sectionProperty is ConcreteSection)
                profile = (sectionProperty as ConcreteSection).SectionProfile;
            else
                Reflection.Compute.RecordWarning("Was not able to extract any section profile");


            BH.oM.Physical.Materials.Material material = null;

            if (sectionProperty.Material != null)
            {
                string matName = sectionProperty.Material.Name ?? "";
                material = Physical.Create.Material(matName, new List<oM.Physical.Materials.IMaterialProperties> { sectionProperty.Material });
            }
            else
            {
                Engine.Reflection.Compute.RecordWarning("Material from sectiion property of the bar is null");
            }

            name = string.IsNullOrEmpty(name) ? sectionProperty.Name : name;

            return Physical.Create.ConstantFramingProperty(profile, material, orientationAngle, name);

        }

        /***************************************************/
    }
}
