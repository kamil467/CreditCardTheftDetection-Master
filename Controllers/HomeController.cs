using CreditCardTheftDetector.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using Plotly.NET;
using System.Diagnostics;
using static CreditCardTheftDetector.MLModel1;
using Microsoft.FSharp.Core; // use this for less verbose and more helpful intellisense
using Plotly.NET.LayoutObjects;
using Microsoft.ML.Trainers;
using Microsoft.ML.Data;

namespace CreditCardTheftDetector.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private const string DATA_FILEPATH = @"creditcard.csv";
        private static string logisticRegressionModelPath = Path.GetFullPath("MLModel1_LogisticRegression.zip");
        private static string decsionTreeModelPath = Path.GetFullPath("MLModel1_FastTreeOva.zip");
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
          

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult PredictorIndex()
        {
            ModelInput sampleData = CreateSingleDataSample(DATA_FILEPATH);
            ViewBag.AlgorithmnName = "Decision Tree";
            return View(sampleData);
        }
        [HttpPost]
        public IActionResult PredictorIndex(ModelInput modelInput)
        {
            var modelObject = new MLModel1();
            ModelOutput prediction = modelObject.Predict(modelInput, decsionTreeModelPath);
            ViewBag.Prediction = prediction;
            ViewBag.AlgorithmnName = "Decision Tree";
            return View();
        }

        private static ModelInput CreateSingleDataSample(string dataFilePath)
        {
            // Create MLContext  
            MLContext mlContext = new MLContext();

            // Load dataset  
            var tLoader = mlContext.Data.CreateTextLoader<CustomInputModel>(
                                            
                                            hasHeader: true,
                                            separatorChar: ',',
                                            allowQuoting: true,
                                            allowSparse: false);

            IDataView dv = tLoader.Load(dataFilePath);


            // Use first line of dataset as model input  
            // You can replace this with new test data (hardcoded or from end-user application)  
            ModelInput sampleForPrediction = mlContext.Data.CreateEnumerable<ModelInput>(dv, false)
                                                                        .First();
            return sampleForPrediction;
        }

        [HttpGet]
        public IActionResult GetChart()
        {
            // Get chart based on csv file 
            // Create MLContext  
            MLContext mlContext = new MLContext();

            // Load dataset  
            IDataView dataView = mlContext.Data.LoadFromTextFile<CustomInputModel>(
                                            path: DATA_FILEPATH,
                                            hasHeader: true,
                                            separatorChar: ',',
                                            allowQuoting: true,
                                            allowSparse: false);

            // Use first line of dataset as model input  
            // You can replace this with new test data (hardcoded or from end-user application)  
            var validTransactionCount = mlContext.Data.CreateEnumerable<ModelInput>(dataView, false)
                                      .Where(m => m.Class == 0).Count();
            var inValidTransaction = mlContext.Data.CreateEnumerable<ModelInput>(dataView, false)
                                      .Where(m => m.Class == 1).Count();


            PlotlyNetHelper.Say.stackedBar(inValidTransaction,validTransactionCount).Show();
            return View("Index");
        }


        [HttpGet]
        public IActionResult GetArea()
        {
            // Get chart based on csv file 
            // Create MLContext  
            MLContext mlContext = new MLContext();

            // Load dataset  
            IDataView dataView = mlContext.Data.LoadFromTextFile<CustomInputModel>(
                                            path: DATA_FILEPATH,
                                            hasHeader: true,
                                            separatorChar: ',',
                                            allowQuoting: true,
                                            allowSparse: false);

            // Use first line of dataset as model input  
            // You can replace this with new test data (hardcoded or from end-user application)  
            var validTranIndexList = new List<int>();
            var InValidTransactionIndex = new List<int>();
            var totalRecord = mlContext.Data.CreateEnumerable<ModelInput>(dataView, false).ToList();
            
            foreach(var model in totalRecord)
            {
                if (model.Class == 0)
                    validTranIndexList.Add(totalRecord.IndexOf(model));
                else
                    InValidTransactionIndex.Add(totalRecord.IndexOf(model));
            }


            PlotlyNetHelper.SplineArea.area2(validTranIndexList.ToArray(), InValidTransactionIndex.ToArray()).Show();
            return View("Index");
        }

        [HttpGet]
        public IActionResult ReTrainModel()
        {
            return View();

        }


        [HttpPost]
        public IActionResult ReTrainModel(ModelInput model)
        {
            // Create MLContext
            MLContext mlContext = new MLContext();

            // Define DataViewSchema of data prep pipeline and trained model
            DataViewSchema dataPrepPipelineSchema, modelSchema;

            // Load data preparation pipeline
            ITransformer dataPrepPipeline = mlContext.Model.Load("MLModel1.zip", out dataPrepPipelineSchema);

            // Load trained model
            ITransformer trainedModel = mlContext.Model.Load("MLModel1.zip", out modelSchema);

            var modelList = new List<ModelInput>
            {
                model
            };
            // Extract trained model parameters

            var originalModelParameters =
                ((ISingleFeaturePredictionTransformer<object>)trainedModel).Model;
            //Load New Data
            IDataView newData = mlContext.Data.LoadFromEnumerable<ModelInput>(modelList);

            // Preprocess Data
            IDataView transformedNewData = dataPrepPipeline.Transform(newData);

            // Retrain model
         //var  retrainedModel =
         //       mlContext.BinaryClassification.Trainers.FastTree()
         //           .Fit(transformedNewData, originalModelParameters as Microsoft.ML.IDataView.);


            ViewBag.IsRetrainDone = true;
            return View();
        }

        [HttpGet]
        public IActionResult GetLogictRegerssion()
        {
            ModelInput sampleData = CreateSingleDataSample(DATA_FILEPATH);
            ViewBag.AlgorithmnName = "Logistic Regression";
            return View("PredictorIndex",sampleData);
        }

        [HttpPost]
        public IActionResult GetLogictRegerssion(ModelInput modelInput)
        {
            var modelObject = new MLModel1();
            ModelOutput prediction = modelObject.Predict(modelInput, logisticRegressionModelPath);
            ViewBag.Prediction = prediction;
            ViewBag.AlgorithmnName = "Logistic Regression";
            return View("PredictorIndex");
        }


        [HttpGet]
        public IActionResult CompareBestOccuracy()
        {
            ModelInput sampleData = CreateSingleDataSample(DATA_FILEPATH);
            return View("CompareAlgorithm",sampleData);
        }

        [HttpPost]
        public IActionResult CompareBestOccuracy(ModelInput modelInput)
        {
            var modelObject = new MLModel1();
            ModelOutput logisticRegPrediction = modelObject.Predict(modelInput, logisticRegressionModelPath);
            ModelOutput decisionTreePrediction = modelObject.Predict(modelInput, decsionTreeModelPath);

            double[] l1 = Array.ConvertAll(logisticRegPrediction.Score, s => (double)s);
            double[] l2 = Array.ConvertAll(decisionTreePrediction.Score, s => (double)s);
            double[] finalArray = { l1[0], l1[1], l2[0], l2[1] };
            PlotlyNetHelper.CompareScore.cChart(finalArray).Show();
            return View("CompareAlgorithm");
        }
    }
}