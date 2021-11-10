using System;
using System.Collections.Generic;
using System.Threading;

namespace AutoDuckProgram
{
    public class AutoDuck
    {
        public static float maxVolume = 100;
        public static float minVolume = 15;
        public static float volumeTresshold = 100;
        public static float delay = 1f;
        public static float fadeOutTime = 1f;
        private static int interval = 16;

        private AutoDuckParametes parameters;

        public bool shouldRun = false;
        public bool isRunning= false;

        private float currentVolume = maxVolume;
        private float delayTime = -1000f;

        public AutoDuckParametes Parameters { set => parameters = value; }

        public AutoDuck(){}
        
        public AutoDuck(AutoDuckParametes parameters)
        {
            this.parameters = parameters;
        }

        public void Stop()
        {
            shouldRun = false;
        }

        public void Start()
        {
            new Thread(StartAutoDuck).Start();
        }

        private void StartAutoDuck()
        {
            try
            {
                shouldRun = true;
                isRunning = true;
                float maxMicVolume;
                AudioDevicesManager.StartRecording(parameters.masterIDs.ToArray());
                while (shouldRun)
                {
                    maxMicVolume = 0;

                    foreach(string id in parameters.masterIDs)
                    {
                        maxMicVolume = AudioDevicesManager.GetLoudness(id) > maxMicVolume ? AudioDevicesManager.GetLoudness(id) : maxMicVolume;
                    }

                    if (maxMicVolume > volumeTresshold)
                    {
                        currentVolume = minVolume;
                        delayTime = delay;
                    }
                    else {
                        if (fadeOutTime != 0f)
                        {
                            currentVolume = Lerp(minVolume, maxVolume, Clamp(delayTime > 0f ? 0f : -delayTime / fadeOutTime, 1, 0));
                        }
                        else {
                            currentVolume = Lerp(minVolume, maxVolume, delayTime > 0f ? 0f : 1f);
                        }
                    }

                    delayTime -= interval / 1000f;

                    for (int i = 0; i < parameters.slaveIDs.Count; i++)
                    {
                        AudioDevicesManager.SetVolume(parameters.slaveIDs[i], currentVolume);
                    }
                    System.Threading.Thread.Sleep(interval);
                }
            }
            finally
            {
                isRunning = false;
                for (int i = 0; i < parameters.slaveIDs.Count; i++)
                {
                    AudioDevicesManager.SetVolume(parameters.slaveIDs[i], maxVolume);
                }
                AudioDevicesManager.StopRecording();

                System.Threading.Thread.CurrentThread.Abort();
            }
        }

        private static float Clamp(float value, float max, float min) {
            return value > max ? max : value < min ? min : value;
        }

        private static float Lerp(float value1, float value2, float t)
        {
            return value1 + (value2 - value1) * t;
        }
    }
}