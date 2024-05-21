using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum State
{
    STATE_INIT,
    STATE_CONST,
    STATE_VAL,
    STATE_NAME,
    STATE_COLON,
    STATE_INT,
    STATE_EQUALS,
    STATE_NUMBER,
    STATE_SEMICOLON,
    STATE_NEWLINE,
    STATE_ERROR
};

public enum State1
{
    TOKEN_DOUBLE = 1,
    TOKEN_FINAL,
    TOKEN_IDENT,
    TOKEN_WHITESPACE,
    TOKEN_EQUALS,
    TOKEN_SEMICOLON,
    TOKEN_OPEN_BRACE,
    TOKEN_CLOSE_BRACE,
    TOKEN_COMMA,
    TOKEN_NUMBER,
    TOKEN_ERROR,
};

public class ParseResult
{
    Token awaited;
    Token actual;
    public bool is_error = false;
    int line;

    public ParseResult(Token awaited, Token actual, int line)
    {
        this.awaited = awaited;
        this.actual = actual;
        if (awaited.type != actual.type)
            this.is_error = true;
        this.line = line;
    }
    public string Stringize(string expr)
    {
        string ret = "Ошибка: line: " + line + " ожидалось: " + this.awaited.type + ", текущий: " + "'" + this.actual.value + "'\n";

        return ret;
    }
    public string actualValue()
    {
        return this.actual.value;
    }
};

public class Parser
{
    public State state;
    int line = 1;

    public ParseResult parse(Token token)
    {
        Token awaited = new Token();
        if (token.type == TokenType.NEWLINE)
        {
            {
                line++;
                state = State.STATE_INIT;
                return new ParseResult(awaited, token, line);
            }
        }

        if (state == State.STATE_INIT)
        {
            awaited.type = TokenType.CONST_KEYWORD;
            if (token.type == TokenType.CONST_KEYWORD)
            {
                state = State.STATE_CONST;
                return new ParseResult(awaited, token, line);
            }
        }

        if (state == State.STATE_CONST)
        {
            awaited.type = TokenType.VAL_KEYWORD;
            if (token.type == awaited.type)
            {
                state = State.STATE_VAL;
                return new ParseResult(awaited, token, line);
            }
        }

        if (state == State.STATE_VAL)
        {
            awaited.type = TokenType.NAME;
            if (token.type == awaited.type)
            {
                state = State.STATE_NAME;
                return new ParseResult(awaited, token, line);
            }

        }
        if (state == State.STATE_NAME)
        {
            awaited.type = TokenType.COLON;
            if (token.type == awaited.type)
            {
                state = State.STATE_COLON;
                return new ParseResult(awaited, token, line);
            }
        }

        if (state == State.STATE_COLON)
        {
            awaited.type = TokenType.INT_KEYWORD;
            if (token.type == awaited.type)
            {
                state = State.STATE_INT;
                return new ParseResult(awaited, token, line);
            }
        }

        if (state == State.STATE_INT)
        {
            awaited.type = TokenType.EQUALS;
            if (token.type == awaited.type)
            {
                state = State.STATE_EQUALS;
                return new ParseResult(awaited, token, line);
            }
        }

        if (state == State.STATE_EQUALS)
        {
            awaited.type = TokenType.NUMBER;
            if (token.type == awaited.type)
            {
                state = State.STATE_NUMBER;
                return new ParseResult(awaited, token, line);
            }
        }

        if (state == State.STATE_NUMBER)
        {
            awaited.type = TokenType.SEMICOLON;
            if (token.type == awaited.type)
            {
                state = State.STATE_SEMICOLON;
                return new ParseResult(awaited, token, line);
            }
        }

        return new ParseResult(awaited, token, line);
    }
};
