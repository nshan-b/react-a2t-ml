using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;

namespace react_a2t_ml.ML {
    public class Tokenize {
        private int _maxLength;
        private string _wordListPath;
        private string _givenSentence;
        private int[] _encodedSentence;
        private Dictionary<string, int> _wordEncodings;
        private Dictionary<int, string> _numEncodings;


        public Tokenize(string sentence, string wordList, int max) {
            _maxLength = max;
            _givenSentence = sentence;
            _encodedSentence = new int[_maxLength];
            _wordEncodings = new Dictionary<string, int>();
            _numEncodings = new Dictionary<int, string>();
            _wordListPath = wordList;

            this.EncodeSentence();
            this.EncodingToSentence();
        }

        
        public int[] EncodedSentence { get { return _encodedSentence; } }

        public void EncodingToSentence() {
            foreach (int number in _encodedSentence) {
                if (_numEncodings.ContainsKey(number)) {
                    Console.WriteLine(_numEncodings[number]);
                }
            }
        }
        public void EncodeSentence() {
            //using (var reader = new StreamReader("../../../../TestingML2/data/word_list.csv"))
            using (var reader = new StreamReader(_wordListPath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture)) {

                // Convert CSV rows into class objects? Anon type --->
                var typeDef = new {
                    Encoding = default(int),
                    Word = string.Empty,
                };

                var records = csv.GetRecords(typeDef);

                // Generate wordEncodings
                foreach (var item in records) {
                    _wordEncodings.Add(item.Word, item.Encoding);
                    _numEncodings.Add(item.Encoding, item.Word);
                }

                string[] splitSentence = _givenSentence.Split(' ');


                for (int i = _maxLength - 1, j = splitSentence.Length - 1; i >= 0 && j >= 0; i--, j--) {
                    string word = splitSentence[j];
                    if (_wordEncodings.ContainsKey(word)) {
                        _encodedSentence[i] = _wordEncodings[word];
                    }
                }

                foreach (int number in _encodedSentence) {
                    Console.WriteLine(number);
                }

                Console.WriteLine("Length " + _encodedSentence.Length);

            }

            return;
        }

    }
}
