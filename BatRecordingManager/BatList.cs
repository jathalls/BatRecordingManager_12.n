
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BatRecordingManager
{
    /// <summary>
    /// class to contain a pair of lists of Bat, one for manually identified bats
    /// and the second for automatically identified bats
    /// </summary>
    public class BatList
    {
        public BatList()
        {
        }

        public List<Bat> autoBats { get; set; } = new List<Bat>();
        public List<Bat> bats { get; set; } = new List<Bat>();

        public ObservableCollection<Bat> GetBatList(bool byAutoID)
        {
            var result = new ObservableCollection<Bat>();
            if (byAutoID)
            {
                result = new ObservableCollection<Bat>(autoBats ?? new List<Bat>());
            }
            else
            {
                result = new ObservableCollection<Bat>(bats ?? new List<Bat>());
            }

            return (result);
        }
    }
}