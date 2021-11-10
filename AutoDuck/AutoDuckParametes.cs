using System.Collections.Generic;
using System.Threading;

namespace AutoDuckProgram
{
    public struct AutoDuckParametes
    {
        public List<string> masterIDs;
        public List<string> slaveIDs;

        public AutoDuckParametes(List<string> masterIDs, List<string> slaveIDs)
        {
            this.masterIDs = masterIDs;
            this.slaveIDs = slaveIDs;
        }
    }
}
