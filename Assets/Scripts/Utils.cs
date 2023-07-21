using System.Collections.Generic;
using System.Linq;
using Builder;

namespace Drone
{
    public static class Utils
    {
        public static bool GetActive(IEnumerable<InteractiveObject> controlObjs)
        {
            return controlObjs.Any(x => x.isActive);
        }
    }
}