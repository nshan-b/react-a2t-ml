using System.ComponentModel.DataAnnotations;

namespace react_a2t_ml.Models {
    // Sentiments table model
    public class Sentiments {
        public int Id { get; set; }
        
        //[Required]
        public string Text { get; set; }
        
        public string? Sentiment { get; set; }

        public double? Negative_Percent { get; set; }

        public double? Positive_Percent { get; set; }
    }
}
