using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[System.Serializable]
public class ModelData
{
    public List<string> vocab_keys;
    public List<int> vocab_values;
    public List<float> idf;
    public List<string> classes;
    public List<float> coefficients_flat;
    public List<float> intercepts;
}

public class DialogueClassifier
{
    private ModelData modelData;
    private Dictionary<string, int> fastVocabulary;
    private int numFeatures;

    private HashSet<string> stopWords = new HashSet<string> {
        "a", "an", "the", "and", "but", "if", "or", "because", "as", "until",
        "while", "of", "at", "by", "for", "with", "about", "against", "between",
        "into", "through", "during", "before", "after", "above", "below", "to",
        "from", "up", "down", "in", "out", "on", "off", "over", "under"
    };

    public DialogueClassifier(TextAsset jsonFile)
    {
        modelData = JsonUtility.FromJson<ModelData>(jsonFile.text);

        if (modelData.vocab_keys == null || modelData.vocab_keys.Count == 0)
        {
            Debug.LogError($"[DialogueClassifier] FATAL ERROR: Vocabulary failed to load from {jsonFile.name}. The JSON file is using the old unsupported dictionary format. Please re-export using the updated Python script.");
            return;
        }

        fastVocabulary = new Dictionary<string, int>();
        for (int i = 0; i < modelData.vocab_keys.Count; i++)
        {
            fastVocabulary[modelData.vocab_keys[i]] = modelData.vocab_values[i];
        }

        numFeatures = modelData.idf.Count;
        Debug.Log($"[DialogueClassifier] Successfully loaded {jsonFile.name}. Vocabulary size: {fastVocabulary.Count}");
    }

    public string Predict(string inputText)
    {
        string processedText = inputText.ToLower();
        MatchCollection matches = Regex.Matches(processedText, @"\b\w\w+\b");

        List<string> validWords = new List<string>();
        foreach (Match match in matches)
        {
            string word = match.Value;
            if (!stopWords.Contains(word))
            {
                validWords.Add(word);
            }
        }

        List<string> ngrams = new List<string>(validWords);
        for (int i = 0; i < validWords.Count - 1; i++)
        {
            ngrams.Add(validWords[i] + " " + validWords[i + 1]);
        }

        Dictionary<int, int> termFrequencies = new Dictionary<int, int>();
        int matchedTokens = 0;
        List<string> matchedWordsLog = new List<string>();

        foreach (string token in ngrams)
        {
            if (fastVocabulary.TryGetValue(token, out int index))
            {
                matchedTokens++;
                matchedWordsLog.Add(token);

                if (termFrequencies.ContainsKey(index))
                    termFrequencies[index]++;
                else
                    termFrequencies[index] = 1;
            }
        }

        if (matchedTokens == 0)
        {
            Debug.LogWarning($"[DialogueClassifier] Evaluated '{inputText}'. 0 tokens matched. Triggering Neutral fallback.");
            return "Neutral_Choice";
        }

        Debug.Log($"[DialogueClassifier] Matched Tokens: {string.Join(", ", matchedWordsLog)}");

        Dictionary<int, double> tfidfVector = new Dictionary<int, double>();
        double sumSquared = 0.0;

        foreach (var kvp in termFrequencies)
        {
            int index = kvp.Key;
            int tf = kvp.Value;

            double sublinearTf = 1.0 + Math.Log(tf);
            double tfidfValue = sublinearTf * modelData.idf[index];

            tfidfVector[index] = tfidfValue;
            sumSquared += tfidfValue * tfidfValue;
        }

        double norm = Math.Sqrt(sumSquared);
        if (norm > 0)
        {
            List<int> keys = new List<int>(tfidfVector.Keys);
            foreach (int key in keys)
            {
                tfidfVector[key] /= norm;
            }
        }

        int numClasses = modelData.classes.Count;
        double[] scores = new double[numClasses];

        int maxIndex = 0;
        double maxScore = double.MinValue;

        for (int c = 0; c < numClasses; c++)
        {
            scores[c] = modelData.intercepts[c];

            foreach (var kvp in tfidfVector)
            {
                int featureIndex = kvp.Key;
                int flatIndex = (c * numFeatures) + featureIndex;
                scores[c] += kvp.Value * modelData.coefficients_flat[flatIndex];
            }

            Debug.Log($"[DialogueClassifier] Score for {modelData.classes[c]}: {scores[c]}");

            if (scores[c] > maxScore)
            {
                maxScore = scores[c];
                maxIndex = c;
            }
        }

        return modelData.classes[maxIndex];
    }
}