namespace InformationRetrieval.PL.ViewModels
{
    public class ResultViewModel
    {
        // For the Term-Document Matrix
        public List<string> Terms { get; set; }
        public List<string> DocumentNames { get; set; }
        public List<List<int>> IncidenceMatrix { get; set; }

        // For the Inverted Index
        public SortedDictionary<string, List<int>> InvertedIndex { get; set; }

        // For the Boolean Query results
        public List<string> QueryResults { get; set; }
    }
}
