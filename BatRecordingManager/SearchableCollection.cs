// *  Copyright 2016 Justin A T Halls
//  *
//  *  This file is part of the Bat Recording Manager Project
//
//         Licensed under the Apache License, Version 2.0 (the "License");
//         you may not use this file except in compliance with the License.
//         You may obtain a copy of the License at
//
//             http://www.apache.org/licenses/LICENSE-2.0
//
//         Unless required by applicable law or agreed to in writing, software
//         distributed under the License is distributed on an "AS IS" BASIS,
//         WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//         See the License for the specific language governing permissions and
//         limitations under the License.

 
using System;
using System.Collections.ObjectModel;

namespace BatRecordingManager
{
    /// <summary>
    ///     The SearchableCollection class collates a set of strings which can be
    ///     searched using the Search dialog.  The data in the SearchableCollection
    ///     is in the form of an ObservableCollection of 3-Tuples(int,int,string).
    ///     Typically the collection can be used to assemble strings to be searched
    ///     from a set of recordings in which case the first integer is the index
    ///     of a recording in the list, the second integer is the index of LaberlledSegement
    ///     in the recording, and the string is the comment for the Labelled Segment.
    ///     If the segment index is -1 then the string could be the recording note.
    ///     Other schema within the bounds of an int,int,string collection are
    ///     permitted.
    /// </summary>
    internal class SearchableCollection
    {
        public SearchableCollection()
        {
            searchableCollection = new ObservableCollection<Tuple<int, int, string>>();
        }

        /// <summary>
        /// returns the number of searchable items in the collection
        /// </summary>
        public int Count
        {
            get
            {
                return (searchableCollection.Count);
            }
        }

        public ObservableCollection<Tuple<int, int, string>> searchableCollection { get; set; } =
                    new ObservableCollection<Tuple<int, int, string>>();

        /// <summary>
        /// Adds a single searchable string with associated recording ID and index of the labelled segment
        /// within the recordings list of labelled segments
        /// </summary>
        /// <param name="recordingIndex"></param>
        /// <param name="segmentIndex"></param>
        /// <param name="target"></param>
        public void Add(int recordingIndex, int segmentIndex, string target)
        {
            searchableCollection.Add(new Tuple<int, int, string>(recordingIndex, segmentIndex, target));
        }

        /// <summary>
        /// adds items tot he searvhable list under the recording index specifiedd and with
        /// incrmenting numbers for the labelled segment number
        /// </summary>
        /// <param name="recordingIndex"></param>
        /// <param name="targetList"></param>
        public void AddRange(int recordingIndex, ObservableCollection<string> targetList)
        {
            if (targetList != null)
                for (var i = 0; i < targetList.Count; i++)
                    Add(recordingIndex, i, targetList[i]);
        }

        /// <summary>
        /// clears the list of searchable strings
        /// </summary>
        public void Clear()
        {
            for (var i = searchableCollection.Count - 1; i >= 0; i--) searchableCollection.RemoveAt(i);
        }

        public ObservableCollection<string> GetStringCollection()
        {
            var result = new ObservableCollection<string>();
            foreach (var item in searchableCollection) result.Add(item.Item3);
            return result;
        }

        /// <summary>
        /// returns the item at the specified index as a Tuple or null if the index is out of range
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Tuple<int, int, string> ItemAt(int index)
        {
            if (index >= 0 && index < searchableCollection.Count)
            {
                return (searchableCollection[index]);
            }
            else
            {
                return (null);
            }
        }
    }
}