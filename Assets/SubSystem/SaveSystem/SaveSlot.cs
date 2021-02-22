using System.Collections.Generic;

namespace GameSystem
{
    public class SaveSlot
    {
        public Dictionary<int, object> records = new Dictionary<int, object>();

        public SaveSlot() { }
        public SaveSlot(SaveSlot other)
        {
            // duplicate values
            records = new Dictionary<int, object>(other.records);
        }
    }
}
