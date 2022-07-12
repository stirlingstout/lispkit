Imports System.IO

Module Parser
    Const MAX_TOKEN_LENGTH = 80

    ' 
    '  <s-exp> ::= <atom> | (<s-exp list>)
    '  <s-exp list> ::= <s-exp> | <s-exp> . <s-exp> | <s-exp> <s-exp list>
    ' 

    Enum TokenType
        T_SYMBOL = 1
        T_NUMBER = 2
        T_DOT = 3

        T_LEFTPAREN = 4
        T_RIGHTPAREN = 5
        T_END
    End Enum
    Structure Token
        Dim stream As StreamReader
        Dim file As String
        Dim token As String

        Dim line As Integer
        Dim pos As Integer
        Dim word As Integer

        Sub New(stream As StreamReader)
            Me.stream = stream
            Me.line = 0
            Me.pos = 0
            Me.word = 0
        End Sub
    End Structure

    Dim token_space(MAX_TOKEN_LENGTH) As Char
    Dim t As Token

    Sub StartScan(f As StreamReader)
        t = New Parser.Token(f)
    End Sub

    Sub Scanner()
        Dim ch As Integer, next_ch As Integer

        t.token = Nothing

        token_space = ""

        ' skip white space
        Do While Not t.stream.EndOfStream
            ch = t.stream.Peek()
            If Convert.ToChar(ch) <> " " Then
                Exit Do
            End If
            If Convert.ToChar(ch) = Environment.NewLine(0) Then
                t.line += 1
                t.pos = 0
            End If
            ch = t.stream.Read()
            t.pos += 1
        Loop

        Debug.Assert(Not Char.IsControl(Convert.ToChar(ch)) And Convert.ToChar(ch) <> " ")

        Do While Not t.stream.EndOfStream
            ch = t.stream.Read()
            token_space &= Convert.ToChar(ch)
            If "().".Contains(Convert.ToChar(ch)) Then
                Exit Do
            End If
            next_ch = t.stream.Peek()
            If " ().".Contains(Convert.ToChar(next_ch)) Then
                Exit Do
            End If

            t.pos += 1
        Loop
        If Not String.IsNullOrEmpty(token_space) Then
            t.token = token_space
        End If
    End Sub

    Function token_type_str(t As TokenType) As String
        Select Case t
            Case TokenType.T_SYMBOL
                Return "Symbol"
            Case TokenType.T_NUMBER
                Return "Number"
            Case TokenType.T_DOT
                Return "Dot"
            Case TokenType.T_LEFTPAREN
                Return "Left parenthesis"
            Case TokenType.T_RIGHTPAREN
                Return "Right parenthesis"
            Case TokenType.T_END
                Return "End of file"
            Case Else
                Return "Unknown token type"
        End Select
    End Function

    Function token_type() As TokenType
        If String.IsNullOrEmpty(t.token) Then
            Return TokenType.T_END
        End If
        Select Case t.token(0)
            Case "."
                Return TokenType.T_DOT
            Case "("
                Return TokenType.T_LEFTPAREN
            Case ")"
                Return TokenType.T_RIGHTPAREN
            Case "0" To "9"
                Return TokenType.T_NUMBER
            Case Else
                Return TokenType.T_SYMBOL
        End Select
    End Function

    Sub match(tokenType As TokenType)
        If tokenType <> token_type() Then
            Console.WriteLine($"Error - did not expect token: '{t.token}'")
            Console.WriteLine($"Expected token type: {token_type_str(tokenType)}")
            Console.WriteLine($"Line {t.line}, column {t.pos}")
            End ' Really needs to return a value!
        End If
        Scanner()
    End Sub

    Function s_exp() As LK_Object
        Dim cell As LK_Object = _nil

        Select Case token_type()
            Case TokenType.T_NUMBER
                cell = New LK_Object(Convert.ToInt64(t.token))
                match(TokenType.T_NUMBER)
            Case TokenType.T_SYMBOL
                cell = New LK_Object(t.token)
                match(TokenType.T_SYMBOL)
            Case TokenType.T_LEFTPAREN
                match(TokenType.T_LEFTPAREN)
                cell = s_exp_list()
                match(TokenType.T_RIGHTPAREN)
            Case TokenType.T_END
            Case Else
                Console.WriteLine($"Error - did not expect token '{t.token}'")
                Console.WriteLine($"Line {t.line}, column {t.pos}")
                End ' Really needs to return a value!
        End Select

        Return cell
    End Function

    Function s_exp_list() As LK_Object
        Dim cell = New LK_Object(s_exp(), _nil)

        Select Case token_type()
            Case TokenType.T_RIGHTPAREN
            Case TokenType.T_DOT
                match(TokenType.T_DOT)
                cell.Cons.cdr = s_exp()
            Case TokenType.T_END
            Case Else
                cell.Cons.cdr = s_exp_list()
        End Select
        Return cell
    End Function

    Function get_exp(f As TextReader) As LK_Object
        StartScan(f)
        Scanner()
        Return s_exp()
    End Function

    Function get_exp_list(f As TextReader) As LK_Object
        StartScan(f)
        Scanner()
        Return s_exp_list()
    End Function

End Module

