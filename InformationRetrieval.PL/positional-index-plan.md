# Plan: Add Positional Index and Phrase Search

Goal
- Add a Positional Index so users can search a phrase and get documents where the words appear in the same sequence (consecutive positions), using the same tokenization rules already in use.

Note on UI stack
- The current solution is ASP.NET Core MVC (Controllers + Views). We will keep MVC for consistency with the existing codebase.

## 1) Business Models: Add Positional Index

- Add a new data structure to hold positions:
  - PositionalIndex: SortedDictionary<string, Dictionary<int, List<int>>>
    - Key: term
    - Value: per-document map (docId -> sorted list of token positions)
- Extend ProcessingResult to include PositionalIndex:
  - public PositionalIndex PositionalIndex { get; set; }

Rationale:
- Keeps existing InvertedIndex intact for boolean queries.
- Positions are required to evaluate phrase queries efficiently.

## 2) Index Building: Populate Positional Index

- Update IndexBuilderService.Build:
  - While tokenizing each document, iterate tokens with index i (position).
  - For each token:
    - Add to vocabulary (existing).
    - Add docId to inverted index (existing).
    - Add position i to positionalIndex[term][docId].
  - Ensure positions are stored sorted (append in order as we scan).
- Continue building the Term-Document incidence matrix as is.

Complexity: O(total tokens), minimal overhead beyond the existing pass.

## 3) New Service: Phrase Query

- Add an interface IPositionalQueryService:
  - List<string> ExecutePhraseQuery(string phrase, PositionalIndex indexData, List<string> documentNames, ITextProcessorService tokenizer)
- Implement PositionalQueryService:
  - Tokenize the phrase using the same TextProcessorService (lowercase, hyphen handling, dot-handling).
  - If the phrase has a single term, fall back to an inverted-index style lookup (or treat as positions where term occurs).
  - For multi-term phrases, use positional intersection:

Pseudo:
- postings = positionalIndex[term0] (map docId -> positions)
- For each nextTerm in phrase:
  - nextPostings = positionalIndex[nextTerm]
  - merged = new map
  - For each doc in intersection(postings.docs, nextPostings.docs):
    - For each p in postings[doc]:
      - Check if (p + 1) exists in nextPostings[doc]
      - If yes, add (p + 1) to merged[doc] (to carry forward possible chains)
  - postings = merged
- Documents with any positions in final postings are matches.

- Finally, convert matching docIds to names via documentNames.

Edge cases:
- Missing term → return empty.
- Empty phrase or whitespace → return empty.
- Maintain case-insensitive behavior and consistent tokenization rules.

## 4) Dependency Injection

- In Program.cs:
  - Register IPositionalQueryService → PositionalQueryService.
  - Add memory cache (recommended) to avoid serializing a large positional index to the view:
    - builder.Services.AddMemoryCache();

## 5) Results State Handling (Recommended: IMemoryCache)

Problem:
- Current design serializes Results into a hidden field (ResultsAsJson). A positional index can be quite large.

Plan:
- Store the full ProcessingResult in IMemoryCache after Process():
  - var key = Guid.NewGuid().ToString("N");
  - _cache.Set(key, bllResult, TimeSpan.FromMinutes(20));
- Pass this key to the view via HomeViewModel (add property string CacheKey).
- In HandleQuery and new HandlePhraseQuery:
  - Retrieve ProcessingResult from cache using CacheKey.
  - Fallback to existing ResultsAsJson deserialization only if cache is missing (optional for backward-compat).
- Continue returning a ResultViewModel for UI rendering (no need to include positional data in the PL model).

Changes:
- HomeViewModel: add string CacheKey
- HomeController: inject IMemoryCache, wire key set/get

## 6) Controller: Add Phrase Query Action

- Add [HttpPost] IActionResult HandlePhraseQuery(HomeViewModel model):
  - Get processing result from cache using model.CacheKey (fallback to ResultsAsJson if necessary).
  - Call _positionalQueryService.ExecutePhraseQuery(model.PhraseQueryText, processingResult.PositionalIndex, processingResult.Matrix.DocumentNames, _textProcessor).
  - Set results.QueryResults to the returned doc names.
  - Return the Index view with updated model (preserve pasted documents like existing flow).

- Also add constructor injection:
  - IPositionalQueryService _positionalQueryService
  - IMemoryCache _cache

## 7) View Models

- HomeViewModel:
  - Add: string CacheKey
  - Add: string PhraseQueryText (input field from UI)
- ResultViewModel:
  - No positional index required (keeps UI payload light).

## 8) Views: Add UI for Phrase Search

- In Views/Home/Index.cshtml:
  - Add a new "Phrase Query" section under the Boolean Query UI:
    - Input: <input asp-for="PhraseQueryText" placeholder="e.g. information retrieval system" />
    - Hidden: CacheKey
    - Hidden: ResultsAsJson (optional fallback)
    - Submit button posts to HandlePhraseQuery
  - Reuse the existing Query Results display (Model.Results.QueryResults) to show phrase matches.
- Optional: Add a collapsible summary of Positional Index (e.g., counts only), but not required.

## 9) Tokenization Consistency

- Ensure phrase tokenization uses TextProcessorService.Tokenize, preserving:
  - lowercase
  - hyphen splitting (so "state-of-the-art" matches "state of the art")
  - dotted abbreviations

## 10) Testing & Validation

- Cases:
  - Single-term phrase behaves like term search.
  - Multi-term phrase with contiguous terms matches.
  - Overlapping occurrences (e.g., "a a") handled correctly.
  - Hyphenated input vs. text ("state-of-the-art" vs "state of the art").
  - No matches, partial overlaps, and documents without all terms.
- Performance sanity with several documents and longer phrases.

## 11) Optional Enhancements

- Phrase with gaps (proximity queries): allow k-gap (p_{i+1} == p_i + k) – not required now.
- UI: Show the number of positions matched per doc.

## 12) Implementation Order

1. BLL models: add PositionalIndex and extend ProcessingResult.
2. IndexBuilderService: build positional index.
3. Add IPositionalQueryService + PositionalQueryService with positional intersection.
4. DI: register service + AddMemoryCache.
5. HomeViewModel: add CacheKey, PhraseQueryText.
6. HomeController:
   - Store ProcessingResult in cache on Process()
   - Add HandlePhraseQuery and wire cache retrieval + service call
7. Index.cshtml: add Phrase Query form and hidden CacheKey.
8. Validate end-to-end and adjust view rendering for results.

Outcome
- Users can input a phrase and get back documents where the terms appear consecutively and in order, consistent with existing tokenization behavior.