namespace InformationRetrieval.PL.ViewModels
{
    public class HomeViewModel
    {
        // For the 'Paste Text' tab
        public int NumberOfDocuments { get; set; }
        public List<string> PastedDocuments { get; set; }

        // For the 'Upload Files' tab
        public IFormFileCollection UploadedFiles { get; set; }

        // This property will receive the query form data upon submission.
        public QueryViewModel Query { get; set; }

        // This property receives the JSON string of the results from a hidden
        // field when the query form is submitted.
        public string ResultsAsJson { get; set; }

        // This holds the deserialized results to render the page.
        public ResultViewModel Results { get; set; }

        // Cache key for storing ProcessingResult in memory cache
        public string CacheKey { get; set; }

        // For phrase query input
        public string PhraseQueryText { get; set; }
    }
}
