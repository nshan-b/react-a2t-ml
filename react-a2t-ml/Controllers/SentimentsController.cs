using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using react_a2t_ml.Data;
using react_a2t_ml.Models;
using react_a2t_ml.ML;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using react_a2t_ml.ModelsML;
using react_a2t_ml.ML.OnnxTypes;

namespace react_a2t_ml.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class SentimentsController : ControllerBase {
        private readonly react_a2t_mlContext _context;
        //private readonly react_a2t_mlContext

        // Given imdb model from ML.NET example
        private string _smPath = Path.Combine(Environment.CurrentDirectory, "ML/SentimentModel");

        // Custom twitter models
        private string _bmPath = Path.Combine(Environment.CurrentDirectory, "ML/OnnxModels/basic_model.onnx");
        private string _amPath = Path.Combine(Environment.CurrentDirectory, "ML/OnnxModels/advanced_model.onnx");
        private string _bmPathPB = Path.Combine(Environment.CurrentDirectory, "ML/PBModels/basic_model");


        // Word encodings
        private string _bmWordListPath = Path.Combine(Environment.CurrentDirectory, "ML/WordLists/bm_word_list.csv");
        private string _amWordListPath = Path.Combine(Environment.CurrentDirectory, "ML/WordLists/am_word_list.csv");



        public SentimentsController(react_a2t_mlContext context) {
            _context = context;
        }

        public static dynamic PredictOnnx(string modelPath, string wordListPath, string text, string outputName) {

            // Input & Output Names of our model:
            var outputColumnNames = new[] { outputName };
            var inputColumnNames = new[] { "x" };

            // Start context
            var mlContext = new MLContext();
            var pipeline = mlContext.Transforms.ApplyOnnxModel(
                outputColumnNames, inputColumnNames, modelPath
            );

            // Convert sentence to int[] and recast to double[] for Onnx types
            if (outputName != "dense_3") {
                var maxLen = 35;
                Tokenize sentence = new Tokenize(text, wordListPath, maxLen);
                var input = new OnnxTypes.Input { Data = sentence.EncodedSentence.Select(d => (double)d).ToArray() };
                var dataView = mlContext.Data.LoadFromEnumerable(new[] { input });
                var transformedValues = pipeline.Fit(dataView).Transform(dataView);
                var output = mlContext.Data.CreateEnumerable<OnnxTypes.Output>(transformedValues, reuseRowObject: false);
                return output;
            }
            else {
                var maxLen = 41;
                Tokenize sentence = new Tokenize(text, wordListPath, maxLen);
                var input = new OnnxTypes.InputNM { Data = sentence.EncodedSentence.Select(d => (double)d).ToArray() };
                var dataView = mlContext.Data.LoadFromEnumerable(new[] { input });
                var transformedValues = pipeline.Fit(dataView).Transform(dataView);
                var output = mlContext.Data.CreateEnumerable<OnnxTypes.OutputNM>(transformedValues, reuseRowObject: false);
                return output;
            }

            
        }

        // OLD
        //public static void PredictPB(string modelPath, string wordListPath, string text) {


        //    //TensorFlowModel tensorFlowModel = mlContext.Model.LoadTensorFlowModel(_smPath);
        //    //IEstimator<ITransformer> pipeline =
        //    //    // Split text into individual words
        //    //    mlContext.Transforms.Text.TokenizeIntoWords("TokenizedWords", "Comment")
        //    //    // Map each word to an integer value. The array of integer makes up the input features.
        //    //    .Append(mlContext.Transforms.Conversion.MapValue("VariableLengthFeatures", lookupMap,
        //    //        lookupMap.Schema["Words"], lookupMap.Schema["Ids"], "TokenizedWords"))
        //    //    // Resize variable length vector to fixed length vector.
        //    //    .Append(mlContext.Transforms.CustomMapping(ResizeFeaturesAction, "Resize"))
        //    //    // Passes the data to TensorFlow for scoring
        //    //    .Append(tensorFlowModel.ScoreTensorFlowModel("Prediction/Softmax", "Features"))
        //    //    // Retrieves the 'Prediction' from TensorFlow and copies to a column
        //    //    .Append(mlContext.Transforms.CopyColumns("Prediction", "Prediction/Softmax"));
        //}

        // OLD (basic prediction engine)
        //public static string PredictSentiment(MLContext mlContext, ITransformer model, string text) {
        //    var engine = mlContext.Model.CreatePredictionEngine<UserComment, UserCommentPrediction>(model);
        //    var comment = new UserComment() {
        //        Comment = text
        //    };

        //    var resultPrediction = engine.Predict(comment);
        //    if (resultPrediction.Prediction[0] > resultPrediction.Prediction[1]) {
        //        return "bad";
        //    }
        //    else {
        //        return "good";
        //    }
        //    //return resultPrediction.Prediction[0].ToString() + resultPrediction.Prediction[1].ToString();
        //}

        //public static string PredictSentimentBM(MLContext mlContext, ITransformer model, string text) {
        //    var engine = mlContext.Model.CreatePredictionEngine<BasicModelText, BasicModelPrediction>(model);
        //    Tokenize sentence = new Tokenize("pizza is awesome");

        //    var comment = new BasicModelText() {
        //        Text = sentence.EncodedSentence
        //    };

        //    var resultPrediction = engine.Predict(comment);
        //    if (resultPrediction.Prediction[0] > resultPrediction.Prediction[1]) {
        //        return "bad";
        //    }
        //    else {
        //        return "good";
        //    }
        //}

        // GET: api/Sentiments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sentiments>>> GetSentiments()
        {
            var res = await _context.Sentiments.ToListAsync();
            return Ok(res);


            //return Ok(_context.Sentiments.ToList());
              //if (_context.Sentiments == null)
              //{
              //    return NotFound();
              //}
              //  return await _context.Sentiments.ToListAsync();
        }

        // GET: api/Sentiments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sentiments>> GetSentiments(int id)
        {
          if (_context.Sentiments == null)
          {
              return NotFound();
          }
            var sentiments = await _context.Sentiments.FindAsync(id);

            if (sentiments == null)
            {
                return NotFound();
            }

            return sentiments;
        }

        // PUT: api/Sentiments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSentiments(int id, Sentiments sentiments)
        {
            if (id != sentiments.Id)
            {
                return BadRequest();
            }

            _context.Entry(sentiments).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SentimentsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Sentiments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Sentiments>> PostSentiments(Sentiments sentiments)
        {
            string text = sentiments.Text;
            //var output = PredictOnnx(_bmPath, _bmWordListPath, text, "dense_1");
            var output = PredictOnnx(_amPath, _amWordListPath, text, "dense_3");

            var results = new List<float[]>();


            foreach (var prediction in output) {
                // Get prediction data
                float[] data = new float[] { prediction.Data[0], prediction.Data[1] };
                results.Add(data);
            }

            var res = new { data = results };

            string finalSentiment = "good";

            if (res.data[0][0] > res.data[0][1]) {
                 finalSentiment = "bad";
            }


            // Save it in the database
            var sql = @"INSERT INTO Sentiments (text, sentiment, negative_percent, positive_percent) VALUES (@text, @sentiment, @negative_percent, @positive_percent)";
            var sqlRes = _context.Database.ExecuteSqlInterpolated(
                $"INSERT INTO Sentiments (text, sentiment, negative_percent, positive_percent) VALUES ({text.Substring(0, Math.Min(text.Length, 128))}, {finalSentiment}, {res.data[0][0]}, {res.data[0][1]})"
            );

            // Return expected results
            return Ok(res);


            //var lookupMap = mlContext.Data.LoadFromTextFile(
            //    Path.Combine(_smPath, "imdb_word_index.csv"),
            //    columns: new[] {
            //        new TextLoader.Column("Words", DataKind.String, 0),
            //        new TextLoader.Column("Ids", DataKind.Int32, 1)
            //    },
            //    separatorChar: ','
            //);

            //Action<VariableLength, FixedLength> ResizeFeaturesAction = (s, f) =>
            //{
            //    var features = s.VariableLengthFeatures;
            //    Array.Resize(ref features, Config.FeatureLength);
            //    f.Features = features;
            //};

            //TensorFlowModel tensorFlowModel = mlContext.Model.LoadTensorFlowModel(_smPath);
            //IEstimator<ITransformer> pipeline =
            //    // Split text into individual words
            //    mlContext.Transforms.Text.TokenizeIntoWords("TokenizedWords", "Comment")
            //    // Map each word to an integer value. The array of integer makes up the input features.
            //    .Append(mlContext.Transforms.Conversion.MapValue("VariableLengthFeatures", lookupMap,
            //        lookupMap.Schema["Words"], lookupMap.Schema["Ids"], "TokenizedWords"))
            //    // Resize variable length vector to fixed length vector.
            //    .Append(mlContext.Transforms.CustomMapping(ResizeFeaturesAction, "Resize"))
            //    // Passes the data to TensorFlow for scoring
            //    .Append(tensorFlowModel.ScoreTensorFlowModel("Prediction/Softmax", "Features"))
            //    // Retrieves the 'Prediction' from TensorFlow and copies to a column
            //    .Append(mlContext.Transforms.CopyColumns("Prediction", "Prediction/Softmax"));

            //// Create an executable model from the estimator pipeline
            //IDataView dataView = mlContext.Data.LoadFromEnumerable(new List<UserComment>());
            //ITransformer model = pipeline.Fit(dataView);

            //var res = PredictSentiment(mlContext, model, text);


            // End doing something
            //return Ok(res);
            //return Ok(sentiments);

        //  if (_context.Sentiments == null)
        //  {
        //      return Problem("Entity set 'react_a2t_mlContext.Sentiments'  is null.");
        //  }
        //    _context.Sentiments.Add(sentiments);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetSentiments", new { id = sentiments.Id }, sentiments);
        //}

        // DELETE: api/Sentiments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSentiments(int id)
        {
            if (_context.Sentiments == null)
            {
                return NotFound();
            }
            var sentiments = await _context.Sentiments.FindAsync(id);
            if (sentiments == null)
            {
                return NotFound();
            }

            _context.Sentiments.Remove(sentiments);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SentimentsExists(int id)
        {
            return (_context.Sentiments?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
