﻿/*
 * parameters file is part of the Buildings and Habitats object Model (BHoM)
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
 * along with parameters code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.oM.Geometry;
using BH.oM.Architecture.Theatron;
using System;

namespace BH.Engine.Architecture.Theatron
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        public static TierProfile TierProfile(ProfileParameters parameters, Point lastPointPrevTier)
        {
            TierProfile tierProfile = new TierProfile();

            setEyePoints(ref tierProfile, parameters, lastPointPrevTier.X, lastPointPrevTier.Z);

            sectionSurfPoints(ref tierProfile, parameters);

            tierProfile.Profile = Geometry.Create.Polyline(tierProfile.FloorPoints);

            return tierProfile;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        private static void setEyePoints(ref TierProfile tierProfile, ProfileParameters parameters, double lastX, double lastZ)
        {
            double prevX, prevZ;
            for (int i = 0; i < parameters.NumRows; i++)
            {
                double x = 0;
                double y = 0;
                double z = 0;
                if (parameters.SuperRiser && i == parameters.SuperRiserStartRow)
                {
                    double deltaSHoriz = parameters.StandingEyePositionX - parameters.EyePositionX;//differences between standing and seated eye posiitons
                    double deltaSVert = parameters.StandingEyePositionZ - parameters.EyePositionZ;
                    //shift the previous positions to give standing eye position and add in the super riser specific horiznotal	
                    prevX = tierProfile.EyePoints[i - 1].X - (deltaSHoriz);
                    prevZ = tierProfile.EyePoints[i - 1].Z + (deltaSVert);
                    x = prevX + parameters.StandingEyePositionX + parameters.SuperRiserKerbWidth + parameters.SuperRiserEyePositionX;
                    z = prevZ + parameters.TargetCValue + parameters.RowWidth * ((prevZ + parameters.TargetCValue) / prevX);
                }
                else
                {
                    if (parameters.SuperRiser && i == parameters.SuperRiserStartRow + 1)//row after the super riser
                    {
                        //x shifts to include 3 rows for super platform back nib wall nib and row less horiz position
                        //also a wider row is required
                        double widthSp = (2 * parameters.RowWidth) + parameters.SuperRiserKerbWidth + parameters.RowWidth - parameters.EyePositionX;
                        x = tierProfile.EyePoints[i - 1].X + widthSp;
                        //z is with standar c over the wheel chair posiiton but could be over the handrail
                        z = tierProfile.EyePoints[i - 1].Z + parameters.TargetCValue + widthSp * ((tierProfile.EyePoints[i - 1].Z + parameters.TargetCValue) / tierProfile.EyePoints[i - 1].X);
                    }
                    else
                    {
                        if (i == 0)
                        {
                            x = parameters.StartX + lastX;

                            z = parameters.StartZ + lastZ;
                        }
                        else
                        {
                            x = tierProfile.EyePoints[i - 1].X + parameters.RowWidth;
                            z = tierProfile.EyePoints[i - 1].Z + parameters.TargetCValue + parameters.RowWidth * ((tierProfile.EyePoints[i - 1].Z + parameters.TargetCValue) / tierProfile.EyePoints[i - 1].X);

                        }
                    }

                }
                if (parameters.RiserHeightRounding > 0) z = Math.Round(z / parameters.RiserHeightRounding) * parameters.RiserHeightRounding;
                var p = Engine.Geometry.Create.Point(x, y, z);
                tierProfile.EyePoints.Add(p);

            }

        }
        /***************************************************/

        private static void sectionSurfPoints(ref TierProfile tierProfile, ProfileParameters parameters)
        {

            double x = 0; double y = 0; double z = 0;

            Point p;
            for (int i = 0; i < tierProfile.EyePoints.Count; i++)
            {
                if (parameters.SuperRiser && i == parameters.SuperRiserStartRow)
                {
                    //4 surface points are needed beneath the wheel chair eye point
                    //p1 x is same as previous
                    z = tierProfile.EyePoints[i - 1].Z - parameters.EyePositionX + 0.1;//z is previous eye - eyeH + something
                    p = Engine.Geometry.Create.Point(x, y, z);
                    tierProfile.FloorPoints.Add(p);

                    //p2 z is the same a sprevious
                    x = x + parameters.SuperRiserKerbWidth;
                    p = Geometry.Create.Point(x, y, z);
                    tierProfile.FloorPoints.Add(p);

                    //p3 x is unchanged
                    z = tierProfile.EyePoints[i].Z - parameters.SuperRiserEyePositionZ;
                    p = Geometry.Create.Point(x, y, z);
                    tierProfile.FloorPoints.Add(p);

                    //p4 z unchnaged
                    x = x + 3 * parameters.RowWidth;
                    p = Geometry.Create.Point(x, y, z);
                    tierProfile.FloorPoints.Add(p);

                }
                else
                {
                    if (parameters.SuperRiser && i == parameters.SuperRiserStartRow + 1)//row after the super riser
                    {
                        //4 surface points are needed beneath the wheel chair eye point
                        //p1 x is same as previous
                        z = tierProfile.EyePoints[i].Z - parameters.EyePositionZ+ parameters.BoardHeight;//boardheight is handrail height
                        p = Geometry.Create.Point(x, y, z);
                        tierProfile.FloorPoints.Add(p);

                        //p2 z unchanged
                        x = x + parameters.SuperRiserKerbWidth;
                        p = Geometry.Create.Point(x, y, z);
                        tierProfile.FloorPoints.Add(p);

                        //p3 x unchanged
                        z = tierProfile.EyePoints[i].Z - parameters.EyePositionZ;
                        p = Geometry.Create.Point(x, y, z);
                        tierProfile.FloorPoints.Add(p);

                        //p4 z unchanged
                        x = tierProfile.EyePoints[i].X + parameters.EyePositionX;
                        p = Geometry.Create.Point(x, y, z);
                        tierProfile.FloorPoints.Add(p);

                    }
                    else
                    {//standard two points per eye
                        for (var j = 0; j < 2; j++)
                        {

                            if (j == 0)
                            {
                                z = tierProfile.EyePoints[i].Z - parameters.EyePositionZ;
                                x = tierProfile.EyePoints[i].X - parameters.RowWidth + parameters.EyePositionX;

                            }
                            else
                            {
                                x = tierProfile.EyePoints[i].X + parameters.EyePositionX;
                            }
                            p = Geometry.Create.Point(x, y, z);
                            tierProfile.FloorPoints.Add(p);

                        }
                    }
                }

            }

        }
    }
}
