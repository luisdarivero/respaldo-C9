// This programme is based on the Buttercup's compiler code, provided by Ariel Ortiz.

/* 
    DeepLingo compiler - Semantic error exception class.
    Date: 12-March-2018
    Authors:
          A01169073 Aldo Reyna
          A01375051 Marina Torres
    File name: SemanticError.cs
*/


using System;

namespace DeepLingo {

    class SemanticError: Exception {

        public SemanticError(string message, Token token):
            base(String.Format(
                "Semantic Error: {0} \n" +
                "at row {1}, column {2}.",
                message,
                token.Row,
                token.Column)) {
        }
        
        public SemanticError(string message):
            base(String.Format(
                "Semantic Error: {0} \n",
                message)) {
        }
    }
}
