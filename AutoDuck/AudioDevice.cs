using NAudio.CoreAudioApi;

namespace AutoDuckProgram
{
    public struct AudioDevice
    {
        private string id;
        private string name;

        public string ID { get { return id; } }
        public string Name { get { return name; } }

        internal AudioDevice(MMDevice device)
        {
            id = device.ID;
            name = device.FriendlyName;
        }
    }
}
