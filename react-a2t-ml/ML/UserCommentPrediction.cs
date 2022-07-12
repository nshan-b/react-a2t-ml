using Microsoft.ML.Data;

namespace react_a2t_ml.ML {
    public class UserCommentPrediction {
        [VectorType(2)]
        public float[] Prediction { get; set; }
    }
}
