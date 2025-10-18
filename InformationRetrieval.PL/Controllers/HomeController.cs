using InformationRetrieval.BLL.Models;
using InformationRetrieval.BLL.Services;
using InformationRetrieval.PL.Models;
using InformationRetrieval.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using System.IO;

namespace InformationRetrieval.PL.Controllers
{
    public class HomeController : Controller
    {
        private readonly IIndexBuilderService _indexBuilderService;
        private readonly IBooleanQueryService _queryService;

        public HomeController(IIndexBuilderService indexBuilderService, IBooleanQueryService queryService)
        {
            _indexBuilderService = indexBuilderService;
            _queryService = queryService; // Assign the service
        }


        public IActionResult Index()
        {
            var model = new HomeViewModel();
            return View(model);
        }


        [HttpPost]
        public IActionResult Process(HomeViewModel model)
        {
            var documents = new Dictionary<string, string>();

            // 1. Prepare documents from user input, prioritizing file uploads.
            if (model.UploadedFiles != null && model.UploadedFiles.Count > 0)
            {
                foreach (var file in model.UploadedFiles)
                {
                    if (file.Length > 0)
                    {
                        // Use Path.GetFileNameWithoutExtension to remove the extension.
                        var documentName = Path.GetFileNameWithoutExtension(file.FileName);

                        using (var reader = new StreamReader(file.OpenReadStream()))
                        {
                            documents.Add(documentName, reader.ReadToEnd());
                        }
                    }
                }
            }
            else if (model.PastedDocuments != null && model.PastedDocuments.Any(d => !string.IsNullOrWhiteSpace(d)))
            {
                for (int i = 0; i < model.PastedDocuments.Count; i++)
                {
                    // Only add non-empty documents.
                    if (!string.IsNullOrWhiteSpace(model.PastedDocuments[i]))
                    {
                        // Name documents sequentially as per the plan (e.g., "D1", "D2").
                        documents.Add($"D{i + 1}", model.PastedDocuments[i]);
                    }
                }
            }

            // Don't proceed if no valid documents were submitted.
            if (!documents.Any())
            {
                return RedirectToAction("Index");
            }

            // 2. Call the BLL to process the data.
            var bllResult = _indexBuilderService.Build(documents);

            // 3. Map BLL models to the PL's ResultViewModel.
            var resultViewModel = new ResultViewModel
            {
                Terms = bllResult.Matrix.Terms,
                DocumentNames = bllResult.Matrix.DocumentNames,
                IncidenceMatrix = bllResult.Matrix.Incidence,
                InvertedIndex = bllResult.Index.Index,
                QueryResults = null
            };

            // 4. Update the main model and return the view to display the results.
            var viewModel = new HomeViewModel
            {
                Results = resultViewModel,
                PastedDocuments = model.PastedDocuments // This line sends the data back
            };

            return View("Index", viewModel);
        }


        [HttpPost]
        public IActionResult HandleQuery(HomeViewModel model)
        {
            // 1. Deserialize the results that were stored in the hidden form field.
            var results = JsonSerializer.Deserialize<ResultViewModel>(model.ResultsAsJson);

            // 2. Create the BLL query model from the view model's query data.
            var queryModel = new QueryModel
            {
                Term1 = model.Query.Term1,
                Term2 = model.Query.Term2,
                Operator = model.Query.Operator
            };

            // 3. Re-create the InvertedIndex object to pass to the service.
            var invertedIndex = new InvertedIndex { Index = results.InvertedIndex };

            // 4. Call the BLL service to get the results.
            var queryResults = _queryService.ExecuteQuery(queryModel, invertedIndex, results.DocumentNames);

            // 5. Update the results object with the new query matches.
            results.QueryResults = queryResults;

            // 6. Prepare the final model to send back to the view.
            // We pass the original query data back so the form fields can be re-populated.
            var finalModel = new HomeViewModel {
                Results = results,
                Query = model.Query,
                PastedDocuments = model.PastedDocuments
            };

            return View("Index", finalModel);
        }
    }
}