using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Services;

public class FTSService
{
    private ConcurrentDictionary<string, List<string>> _invertedIndexPairs =
        new ConcurrentDictionary<string, List<string>>();

    public void AddRange(IEnumerable<KeyValuePair<string, string>> pairs)
    {
        foreach (var pair in pairs)
        {
            foreach (var token in Analyze(pair.Value))
            {
                if (!_invertedIndexPairs.TryGetValue(token, out var keys))
                {
                    keys = new List<string>();
                    _invertedIndexPairs[token] = keys;
                }

                if (keys.Count > 0 && keys[keys.Count - 1] == pair.Key)
                {
                    continue;
                }
                keys.Add(pair.Key);
            }
        }
    }

    public List<string>? Search(string text)
    {
        List<string> foundValues = null;

        foreach (var token in Analyze(text))
        {
            if (_invertedIndexPairs.TryGetValue(token, out var keys))
            {
                foundValues = (foundValues == null) ? keys : foundValues.Intersect(keys).ToList();
            }
            else
            {
                return null; // does not exist
            }
        }
        return foundValues;
    }

    private string[] Tokenize(string text)
    {
        string nonAlphaNumericPattern = @"[^A-Za-z0-9]+";
        string[] tokenizedTextSplit = Regex.Split(text, nonAlphaNumericPattern);

        List<string> advancedSplit = new List<string>();
        foreach (string initialToken in tokenizedTextSplit)
        {
            string alphaNumericPattern = @"(?<=\d)(?=\w)";
            advancedSplit.AddRange(Regex.Split(initialToken, alphaNumericPattern));
        }
        return advancedSplit.ToArray();
    }

    private string[] LowerCaseFilter(string[] tokens)
    {
        return tokens.Select(token => token.ToLower()).ToArray();
    }

    private string[] StopWordFilter(string[] tokens)
    {
        string[] stopWords = new string[] { "-", "x", "/", "\\", "_" };
        return tokens.Where(token => !stopWords.Contains(token)).ToArray();
    }

    public string[] Analyze(string text)
    {
        string[] tokens = Tokenize(text);
        tokens = LowerCaseFilter(tokens);
        tokens = StopWordFilter(tokens);
        return tokens;
    }
}
