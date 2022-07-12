using Microsoft.ML.Data;

namespace react_a2t_ml.ML.OnnxTypes {
    public class OnnxTypes {
        public class Input {
            [VectorType(35)]
            [ColumnName("x")]
            public double[] Data { get; set; }
        }

        public class InputNM {
            [VectorType(41)]
            [ColumnName("x")]
            public double[] Data { get; set; }
        }

        public class Output {
            [VectorType(2)]
            [ColumnName("dense_1")]
            public float[] Data { get; set; }
        }

        public class OutputNM {
            [VectorType(2)]
            [ColumnName("dense_3")]
            public float[] Data { get; set; }
        }
    }
}
