#if !NO_LITE_DB
using System;
using System.Collections.Generic;
using System.Linq;
using Internal.LiteDB.Engine;
using static Internal.LiteDB.Constants;

namespace Internal.LiteDB
{
    internal partial class SqlParser
    {
        /// <summary>
        /// BEGIN [ TRANS | TRANSACTION ]
        /// </summary>
        private BsonDataReader ParseBegin()
        {
            _tokenizer.ReadToken().Expect("BEGIN");

            var token = _tokenizer.ReadToken().Expect(TokenType.Word, TokenType.EOF, TokenType.SemiColon);

            if (token.Is("TRANS") || token.Is("TRANSACTION"))
            {
                _tokenizer.ReadToken().Expect(TokenType.EOF, TokenType.SemiColon);
            }

            var transactionId = _engine.BeginTrans();

            return new BsonDataReader(transactionId);
        }
    }
}
#endif