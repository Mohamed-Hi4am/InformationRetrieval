## Summary of the Information Retrieval Solution

This is an ASP.NET Core 8 MVC application implementing a basic Information Retrieval System with the following architecture:

### Project Structure

The solution follows a 3-tier architecture:

1. InformationRetrieval.PL (Presentation Layer) - ASP.NET Core MVC web application
2. InformationRetrieval.BLL (Business Logic Layer) - Core processing logic
3. InformationRetrieval.DAL (Data Access Layer) - Model definitions (implied from references)

---

### Core Functionality

The application provides four main features:

#### 1. Document Input
- Paste Text: Users can dynamically specify how many documents to paste (named D1, D2, etc.)
- Upload Files: Users can upload multiple .txt files (file names without extension become document names)

#### 2. Term-Document Incidence Matrix
- Tokenizes documents using regex pattern `[a-z0-9]+(?:\.[a-z0-9]+)*`
- Normalizes to lowercase
- Handles hyphenated words by converting hyphens to spaces
- Builds a binary matrix showing which terms appear in which documents
- Displays results in a sortable table format

#### 3. Inverted Index
- Creates a mapping of each term to its posting list (document IDs where it appears)
- Uses `SortedDictionary` for consistent alphabetical ordering
- Displays term-to-document relationships in a table

#### 4. Boolean Query Execution
- Supports three operators:
  - AND: Returns documents containing both terms (intersection)
  - OR: Returns documents containing either term (union)
  - NOT: Returns documents NOT containing the term
- Uses the inverted index for efficient query processing
- Returns document names matching the query

---

### Key Services (Business Logic Layer)

1. ITextProcessorService / TextProcessorService
   - Tokenizes text into terms
   - Handles normalization and special character processing

2. IIndexBuilderService / IndexBuilderService
   - Builds term-document matrix
   - Constructs inverted index
   - Returns ProcessingResult containing both structures

3. IBooleanQueryService / BooleanQueryService
   - Executes boolean queries (AND, OR, NOT)
   - Uses inverted index for lookups
   - Returns matching document names

---

### View Models (Presentation Layer)

- HomeViewModel: Main view model with document input, query data, and results
- QueryViewModel: Boolean query parameters (Term1, Term2, Operator)
- ResultViewModel: Contains matrix, inverted index, and query results

---

### Controller Flow

HomeController has three actions:
1. Index(): Displays initial form
2. Process(): Processes documents → generates matrix/index → displays results
3. HandleQuery(): Executes boolean query → updates results → redisplays page

---

### Technology Stack

- .NET 8 with C# 12.0
- ASP.NET Core MVC (not Razor Pages, despite context note - uses asp-controller/asp-action tags)
- Bootstrap 5 for UI
- Dependency Injection for service registration
- System.Text.Json for serialization

---

### Notable Implementation Details

- Results persist between query submissions via JSON serialization in hidden form fields
- Dynamic JavaScript for adding/removing document input fields
- Tab-based UI for switching between paste and upload modes
- Clean separation of concerns with interface-based services
- Copyright footer shows "Mohamed Hisham" as the creator
