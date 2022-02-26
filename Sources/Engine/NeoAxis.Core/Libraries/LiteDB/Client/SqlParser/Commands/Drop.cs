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
        /// DROP INDEX {collection}.{indexName}
        /// DROP COLLECTION {collection}
        /// </summary>
        private BsonDataReader ParseDrop()
        {
            _tokenizer.ReadToken().Expect("DROP");

            var token = _tokenizer.ReadToken().Expect(TokenType.Word);

            if (token.Is("INDEX"))
            {
                var collection = _tokenizer.ReadToken().Expect(TokenType.Word).Value;
                _tokenizer.ReadToken().Expect(TokenType.Period);
                var name = _tokenizer.ReadToken().Expect(TokenType.Word).Value;

                _tokenizer.ReadToken().Expect(TokenType.EOF, TokenType.SemiColon);

                var result = _engine.DropIndex(collection, name);

                return new BsonDataReader(result);
            }
            else if(token.Is("COLLECTION"))
            {
                var collection = _tokenizer.ReadToken().Expect(TokenType.Word).Value;

                _tokenizer.ReadToken().Expect(TokenType.EOF, TokenType.SemiColon);

                var result = _engine.DropCollection(collection);

                return new BsonDataReader(result);
            }
            else
            {
                throw LiteException.UnexpectedToken(token, "INDEX|COLLECTION");
            }
        }
    }
}
#endif