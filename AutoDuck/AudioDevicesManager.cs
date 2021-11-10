using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Collections.Generic;

namespace AutoDuckProgram
{
    public static class AudioDevicesManager
    {
        private static MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
        public static List<Capture> captures = new List<Capture>();

        private static MMDevice GetMMDevice(string id)
        {
            return deviceEnumerator.GetDevice(id);
        }

        public static List<AudioDevice> GetPlaybackDevices()
        {
            MMDeviceCollection devices = deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            List<AudioDevice> result = new List<AudioDevice>();
            for (int i = 0; i < devices.Count; i++)
            {
                result.Add(new AudioDevice(devices[i]));
            }
            return result;
        }

        public static List<AudioDevice> GetCaptureDevices()
        {
            MMDeviceCollection devices = deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
            List<AudioDevice> result = new List<AudioDevice>();
            for (int i = 0; i < devices.Count; i++)
            {
                result.Add(new AudioDevice(devices[i]));
            }
            return result;
        }

        public static void SetVolume(string deviceID, float volume)
        {
            volume = volume < 0 ? 0 : volume > 100 ? 100 : volume;
            GetMMDevice(deviceID).AudioEndpointVolume.MasterVolumeLevelScalar = volume / 100.0f;
        }

        public static float GetLoudness(string deviceID)
        {
            return GetMMDevice(deviceID).AudioMeterInformation.MasterPeakValue * 100;
        }

        public static void RestartRecording(string[] deviceID){
            StopRecording();
            System.Threading.Thread.Sleep(5);
            StartRecording(deviceID);
        }

        public static void StartRecording(string[] deviceID)
        {
            StopRecording();
            for (int i = 0; i < deviceID.Length; i++)
            {
                if (GetMMDevice(deviceID[i]).DataFlow == DataFlow.Capture)
                {
                    captures.Add(new Capture(new WasapiCapture(GetMMDevice(deviceID[i]))));
                }
                else
                {
                    captures.Add(new Capture(new WasapiLoopbackCapture(GetMMDevice(deviceID[i]))));
                }
            }
        }

        public static void StopRecording()
        {
            foreach (Capture cap in captures)
            {
                cap.Stop();
            }
            captures.Clear();
        }
    }
}
