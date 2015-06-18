using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ServicesLayer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ILangService" in both code and config file together.
    [ServiceContract]
    public interface ILangService
    {
        /// <summary>
        /// Removes tags from the input text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [OperationContract]
        string RemoveTags(string text);


        /// <summary>
        /// In morphology and lexicography, a lemma (plural lemmas or lemmata) is 
        /// the canonical form, dictionary form, or citation form of a set of words (headword). 
        /// </summary>
        /// <param name="word">Input word</param>
        /// <returns>Canonical form of the input word</returns>
        [OperationContract]
        string GetLemma(string word);

        /// <summary>
        /// In linguistic morphology and information retrieval,
        /// stemming is the process for reducing inflected (or sometimes derived)
        /// words to their stem, base or root form—generally a written word form. 
        /// </summary>
        /// <param name="word">Input word</param>
        /// <returns>Base word form</returns>
        [OperationContract]
        string GetStemma(string word);

        [OperationContract]
        string[] GetSenetces(string text, string word);

        [OperationContract]
        bool WordOrder(string text, string[] words);

        [OperationContract]
        string[] DivideToSubsentences(string text);

        [OperationContract]
        bool ContainsWords(string text, string[] words);

    }
}
