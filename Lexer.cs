using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum TokenType
{
    CONST_KEYWORD,
    VAL_KEYWORD,
    NAME,
    COLON,
    INT_KEYWORD,
    EQUALS,
    NUMBER,
    SEMICOLON,
    NEWLINE,
    INVALID
};

public enum TokenType1
{
    TOKEN_DOUBLE = 1,
    TOKEN_FINAL,
    TOKEN_IDENT,
    TOKEN_EQUALS,
    TOKEN_SEMICOLON,
    TOKEN_OPEN_BRACE,
    TOKEN_CLOSE_BRACE,
    TOKEN_COMMA,
    TOKEN_NUMBER,
    TOKEN_NEWLINE,
    TOKEN_INVALID
};

public class Token
{
    public string value;
    public int start,
               end;
    public TokenType type;

    public Token()
    {
        this.value = "";
        this.start = 0;
        this.end = 0;
        this.type = TokenType.NEWLINE;
    }

    public Token(string expr, int start, int end, TokenType type)
    {
        this.value = expr.Substring(start, end - start + 1);
        this.start = start;
        this.end = end;

        if (this.value == "const") this.type = TokenType.CONST_KEYWORD;
        else if (this.value == "val") this.type = TokenType.VAL_KEYWORD;
        else if (this.value == "int") this.type = TokenType.INT_KEYWORD;
        else this.type = type;
    }

    public string Stringize()
    {
        string ret = "name='" + (this.type == TokenType.NEWLINE ? "\\n" : this.value) + "' start=" 
            + this.start + " end=" + this.end + " type=" + this.type;
        return ret;
    }
};

public class Lexer
{
    public List<Token> tokenize(string text)
    {
        List<Token> tokens = new List<Token>();
        TokenType type = TokenType.NAME;
        int start = 0, end = 0;
        for (int i = 0; i < text.Length; i++)
        {
            start = end;
            switch (text[i])
            {
                case ' ':
                    end++;
                    continue;
                case >= 'a' and <= 'z':
                case >= 'A' and <= 'Z':
                    while ((i != text.Length) &&
                        ((text[i] >= 'a' && text[i] <= 'z') ||
                        (text[i] >= 'A' && text[i] <= 'Z') ||
                        (text[i] == '_') ||
                        (text[i] >= '0' && text[i] <= '9')))
                    {
                        i++;
                        end++;
                    }
                    i--;
                    end--;
                    type = TokenType.NAME;
                    break;
                case >= '0' and <= '9':
                    while ((i != text.Length) && (text[i] >= '0' && text[i] <= '9'))
                    {
                        i++;
                        end++;
                    }
                    i--;
                    end--;
                    type = TokenType.NUMBER;
                    break;
                case '=':
                    type = TokenType.EQUALS;
                    break;
                case ':':
                    type = TokenType.COLON;
                    break;
                case ';':
                    type = TokenType.SEMICOLON;
                    break;
                case '\n':
                case '\r':
                    type = TokenType.NEWLINE;
                    break;
                default:
                    type = TokenType.INVALID;
                    break;
            }
            tokens.Add(new Token(text, start, end, type));
            end++;
        }
        return tokens;
    }
}
