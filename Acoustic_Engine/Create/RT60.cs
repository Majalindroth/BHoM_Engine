﻿using BH.oM.Acoustic;

namespace BH.Engine.Acoustic
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static RT60 RT60(double value, int receiverID, int speakerID)
        {
            return new RT60()
            {
                Value = value,
                ReceiverID = receiverID,
                SpeakerID = speakerID
            };
        }

        /***************************************************/
    }
}
