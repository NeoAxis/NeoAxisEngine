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
        /// CHECKPOINT
        /// </summary>
        private BsonDataReader ParseCheckpoint()
        {
            _tokenizer.ReadToken().Expect(Pragmas.CHECKPOINT);

            // read <eol> or ;
            _tokenizer.ReadToken().Expect(TokenType.EOF, TokenType.SemiColon);

            var result = _engine.Checkpoint();

            return new BsonDataReader(result);
        }
    }
}
#endif