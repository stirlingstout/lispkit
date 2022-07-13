Imports System
Imports Microsoft.VisualBasic

' Based on:
' A Lispkit implementation.

' Copyright (c) 2011  A. Carl Douglas

' Permission is hereby granted, free of charge, to any person obtaining
' a copy of this software and associated documentation files (the
' "Software"), to deal in the Software without restriction, including
' without limitation the rights to use, copy, modify, merge, publish,
' distribute, sublicense, and/or sell copies of the Software, and to
' permit persons to whom the Software is furnished to do so, subject to
' the following conditions:

' The above copyright notice and this permission notice shall be included
' in all copies or substantial portions of the Software.

' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
' EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
' MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
' IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
' CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
' TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
' SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

Module Program
    Enum ObjectType
        NUMBER = 1
        SYMBOL = 2
        CONS = 3
    End Enum

    Class ConsCell
        Property car As LK_Object
        Property cdr As LK_Object

        Sub New(car As LK_Object, cdr As LK_Object)
            Me.car = car
            Me.cdr = cdr
        End Sub
    End Class

    Class LK_Object
        Dim LK_Type As ObjectType

        Public number As Long
        Public symbol As String
        Public cons As ConsCell

        Sub New(l As Long)
            Me.LK_Type = ObjectType.NUMBER
            Me.number = l
        End Sub

        Sub New(s As String)
            Me.LK_Type = ObjectType.SYMBOL
            Me.symbol = s
        End Sub

        Sub New(car As LK_Object, cdr As LK_Object)
            Me.LK_Type = ObjectType.CONS
            Me.cons = New ConsCell(car, cdr)
        End Sub

        Function car() As LK_Object
            Debug.Assert(Me.LK_Type = ObjectType.CONS)
            Return Me.cons.car
        End Function

        Function cdr() As LK_Object
            Debug.Assert(Me.LK_Type = ObjectType.CONS)
            Return Me.cons.cdr
        End Function

        Function isNumber() As Boolean
            Return Me.LK_Type = ObjectType.NUMBER
        End Function

        Function isSymbol() As Boolean
            Return Me.LK_Type = ObjectType.SYMBOL
        End Function

        Function isCons() As Boolean
            Return Me.LK_Type = ObjectType.CONS
        End Function

        Function isNotCons() As Boolean
            Return Not Me.isCons()
        End Function

        Function isNil() As Boolean
            ' The C code for this (which is a function taking a parameter of (LK_)Object has the test
            ' for obj == _nil commented out (see print.c). It's only used in exp_print which has a test for Nothing and equality
            ' to _nil before it is used
            Return Me.LK_Type = ObjectType.SYMBOL AndAlso stringValue(Me) = stringValue(_nil)
        End Function

        Function isNotNil() As Boolean
            Return Not Me.isNil()
        End Function

        Function typeString() As String
            Select Case Me.LK_Type
                Case ObjectType.CONS
                    Return "Cons"
                Case ObjectType.NUMBER
                    Return "Number"
                Case ObjectType.SYMBOL
                    Return "Symbol"
                Case Else
                    Return "Unknown"
            End Select
        End Function
    End Class

    Dim _stack As LK_Object
    Dim _environ As LK_Object
    Dim _control As LK_Object
    Dim _dump As LK_Object

    Dim _true As LK_Object
    Dim _false As LK_Object
    Public _nil As LK_Object

    Dim _work As LK_Object

    Sub Main(args As String())
        Dim help = $"Lispkit ({Date.Now})
Usage: lispkit [option] [file] [file]
Example: lispkit compiler.ascii compiler.txt.ascii"

        Dim isVerbose = False
        If args.Length < 2 Then
            Console.WriteLine(help)
            End
        End If

        For Each arg In args
            If arg.ToLower().StartsWith("-h") Then
                Console.WriteLine(help)
            ElseIf arg.ToLower().StartsWith("-v") Then
                isVerbose = True
            End If
        Next

        Dim streams() As System.IO.TextReader = {Console.In, Console.In} ' fp in original

        For i = 0 To Math.Min(args.Length - 1, streams.Length - 1)
            Try
                streams(i) = New System.IO.StreamReader(args(i))
            Catch ex As Exception
                Console.WriteLine($"Could not load {args(i)}: {ex.Message}")
                End
            End Try
        Next

        Init()

        Dim fn = get_exp(streams(0))
        exp_print(fn)
        Dim arguments = get_exp_list(streams(1))
        Dim result = execute(fn, arguments)

        exp_print(result)
        Console.WriteLine()

        For Each s In streams
            s.Close()
        Next

        If isVerbose Then
            gc_stats()
        End If

        gc_exit()

        intern_free()

    End Sub

    Sub Init()
        gc_init()

        _true = New LK_Object("T")
        _false = New LK_Object("F")
        _nil = New LK_Object("NIL")

        _stack = _nil
        _control = _nil
        _environ = _nil
        _dump = _nil

        _work = _nil
    End Sub

#Region "Garbage collection routines"
    ''' We'll let the .NET Garbage collector do all the work
    Sub gc_init()

    End Sub

    Sub gc_stats()

    End Sub

    Sub gc_exit()

    End Sub

    Sub intern_free()

    End Sub
#End Region

End Module

