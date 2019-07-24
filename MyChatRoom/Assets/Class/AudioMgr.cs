using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Classes
{
    public static class MicrophoneMgr
    {

        private static AudioClip clip;

        private static int maxRecordTime = 10;
        private static int samplingRate = 12000;

        public static bool TryStartRecording()
        {
            try
            {
                Microphone.End(null);
                clip = Microphone.Start(null, false, maxRecordTime, samplingRate);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public static void EndRecording(out int length, out AudioClip outClip)
        {
            int lastPos = Microphone.GetPosition(null);

            if (Microphone.IsRecording(null))
            {
                length = lastPos / samplingRate;
            }
            else
            {
                length = maxRecordTime;
            }

            Microphone.End(null);

            if (length < 1.0f)
            {
                outClip = null;
                return;
            }

            outClip = clip;
        }
    }
}
