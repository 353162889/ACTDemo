using System.Collections.Generic;

namespace Game
{
    public class ForbidComponent : DataComponent
    {
        public int forbidIndex = 1;
        public Forbiddance forbiddance;
        public List<Forbiddance> lstForbids = new List<Forbiddance>();
    }
}