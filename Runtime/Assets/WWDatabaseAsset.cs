using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {

    // Represents runtime asset representation for *.wwcsv files 
    // Those databases are designed to be immutable with fast random data 
    // retrieval
    [Icon("Packages/com.github.superjellie.jenga/Editor/Icons/database.png")]
    public class WWDatabaseAsset : ScriptableObject {

        // Sry for confusing naming, probs need to think of a better one
        [System.Serializable]
        public struct Data          { public string data; public int ptrID; }
        [System.Serializable]
        public struct ColumnValues  { public Data[] data; }
        [System.Serializable]
        public struct ValuesPointer { public int[] indicesInSorted; }

        public string[]        keys;
        public ColumnValues[]  columnsValues;
        public ValuesPointer[] pointers;

        // Example of structure. Let our table be: 
        // Column:Actor     Column:Sprite      Column:Dialogue
        // Amy              Envy               Good Mornin'
        // Cindy            Fear               H-hi!
        // Bob              Disgust            I dont know...

        // Then this asset will look like this:
        //
        // keys        values [sorted by data]
        // Actor       Amy:0           Bob:2           Cindy:1 
        // Dialogue    Good Mornin':0  H-hi!:1         I dont know...:2
        // Sprite      Disgust:2       Envy:0          Fear:1
        //  
        // pointers    Actor           Dialogue        Sprite  
        // [0]         0               0               1       
        // [1]         2               1               2       
        // [2]         1               2               0       

        // Represents key-data matcher
        [System.Serializable]
        public struct Match { 
            public string key; 
            public string value; 

            public Match(string key, string value) {
                this.key = key;
                this.value = value;
            }
        }

        // Finds lines matching keys
        // From this lines for each given column retrievs first non-empty value
        // (if there is any). Returns null when no matches found.
        //
        // In given example:
        //      GetData({ new("Actor", "Bob") }, { "Sprite", "Dialogue" })
        // Will return:
        //      { "Disgust", "I dont know..." }
        //
        // Will put null in result array if column is unknown or not matched
        // by keys
        //
        // This function should be O(log N) where N is size of database
        public string[] GetData(Match[] matchKeys, string[] columns) { 

            // Find pointers (read lines) that matched our keys
            var matchedPointers = MatchPointers(matchKeys);

            if (matchedPointers == null || matchedPointers.Length == 0)
                return null;

            // Now we want to gather result from pointers
            var result = new string[columns.Length];

            for (int i = 0; i < columns.Length; ++i) {
                var id = System.Array.BinarySearch(keys, columns[i]);
                if (id < 0) continue;

                var myColumn = columnsValues[id].data;

                foreach (var ptr in matchedPointers) {
                    if (ptr < 0) continue;
                    var valueIndex = pointers[ptr].indicesInSorted[id];
                    result[i] = valueIndex >= 0 && valueIndex < myColumn.Length
                        ? myColumn[valueIndex].data : "";

                    if (!string.IsNullOrEmpty(result[i])) break;
                }
            }

            return result;
        }

        public string[] GetData(int pointer, string[] columns) {
            var result = new string[columns.Length];

            if (pointer < 0 || pointer >= pointers.Length) return result;

            for (int i = 0; i < columns.Length; ++i) {
                var id = System.Array.BinarySearch(keys, columns[i]);
                if (id < 0) continue;

                var myColumn = columnsValues[id].data;

                var valueIndex = pointers[pointer].indicesInSorted[id];
                result[i] = valueIndex >= 0 && valueIndex < myColumn.Length
                    ? myColumn[valueIndex].data : "";
            }

            return result;
        }

        // I hate when i cant throw lambda at it...
        public class DataComparer : IComparer<Data> {
            public int Compare(Data x, Data y) => x.data.CompareTo(y.data);
        }

        // Matches pointers (read table lines) to key query
        public int[] MatchPointers(Match[] matchKeys) {
            var cmpData = new DataComparer();

            HashSet<int> matchedPointers = null;
            for (int i = 0; i < matchKeys.Length; ++i) {

                // Find index in our keys array
                // We use this for faster indexing (and simpler structure)
                // instead of using Dictionaries all over the place 
                var id = System.Array.BinarySearch(keys, matchKeys[i].key);

                // Skip unknown keys
                if (id < 0) continue;

                //
                var searchFor = matchKeys[i].value;
                var myColumn  = columnsValues[id].data;

                // Skip empty matches
                if (string.IsNullOrEmpty(searchFor)) continue;

                // Search for value
                var valueIndex = System.Array
                    .BinarySearch(
                        myColumn, new Data() { data = searchFor }, cmpData
                    );

                // 
                if (valueIndex < 0) return new int[0];

                // Go though range and gather matched pointers
                var myMatch = new HashSet<int>();

                for (int k = valueIndex; 
                    k < myColumn.Length && myColumn[k].data == searchFor; ++k)
                    myMatch.Add(myColumn[k].ptrID);

                for (int k = valueIndex; 
                    k > -1 && myColumn[k].data == searchFor; --k)
                    myMatch.Add(myColumn[k].ptrID);

                // We want to have common subset of all matched pointers
                if (matchedPointers == null)
                    matchedPointers = myMatch;
                else 
                    matchedPointers.IntersectWith(myMatch);
            }

            if (matchedPointers == null) return new int[0];

            var result = new int[matchedPointers.Count];
            matchedPointers.CopyTo(result);
            System.Array.Sort(result);
            return result;
        }

        // Populate build contxt with lines from table
        public class BuildContext {
            public string columnPrefix = "Column:";
            public string columnEnd    = "Column:End";
            public List<List<string>> lines = new();
            public HashSet<string> columns = new();

            public void AddLine(List<string> line) {
                lines.Add(line);
                foreach (var entry in line)
                    if (entry.StartsWith(columnPrefix) && entry != columnEnd)
                        columns.Add(entry.Substring(columnPrefix.Length));
            }
        }

        // Builds database from context
        public void Build(BuildContext ctx) {
            var cmpData = new DataComparer();
            
            // First build our column keys
            keys = new string[ctx.columns.Count];
            ctx.columns.CopyTo(keys);
            System.Array.Sort(keys);

            // columnLists will become columnsValues after we done
            var columnLists = new List<Data>[keys.Length];
            for (int i = 0; i < keys.Length; ++i) columnLists[i] = new();


            // We will go through lines keeping current column key in mind
            List<int> currentColumns = new();
            int nextPtr = 0;
            for (int i = 0; i < ctx.lines.Count; ++i) {
                var line = ctx.lines[i];

                // Extend our columns mind if needed
                while (currentColumns.Count < line.Count) 
                    currentColumns.Add(-1);

                // Remember new columns if any were introduced
                for (int k = 0; k < line.Count; ++k)
                    if (line[k].StartsWith(ctx.columnPrefix))
                        currentColumns[k] = System.Array.BinarySearch
                            (keys, line[k].Substring(ctx.columnPrefix.Length));

                // Populate our data lists
                bool hadAtLeastOneValue = false;
                for (int k = 0; k < line.Count; ++k) {
                    if (string.IsNullOrEmpty(line[k]))          continue;
                    if (currentColumns[k] < 0)                  continue;
                    if (line[k].StartsWith(ctx.columnPrefix))   continue;
                    if (line[k] == ctx.columnEnd)
                        { currentColumns[k] = -1; continue; }
                
                    columnLists[currentColumns[k]]
                        .Add(new Data() { data = line[k], ptrID = nextPtr });
                    hadAtLeastOneValue = true; 
                }

                // This way we will skip header and blank lines 
                if (hadAtLeastOneValue) nextPtr++;
            }

            // 
            pointers = new ValuesPointer[nextPtr];
            for (int i = 0; i < pointers.Length; ++i) {
                pointers[i].indicesInSorted = new int[keys.Length];

                // We will set pointers to -1 by default to not be confused
                for (int k = 0; k < keys.Length; ++k)
                    pointers[i].indicesInSorted[k] = -1;
            }

            // Make sorted columns
            columnsValues = new ColumnValues[keys.Length];
            for (int i = 0; i < keys.Length; ++i) {
                columnsValues[i].data = columnLists[i].ToArray();
                System.Array.Sort(columnsValues[i].data, cmpData);
            }

            // Populate pointers
            for (int i = 0; i < keys.Length; ++i)
                for (int k = 0; k < columnsValues[i].data.Length; ++k)
                    pointers[columnsValues[i].data[k].ptrID]
                        .indicesInSorted[i] = k;

        }

    }
}
