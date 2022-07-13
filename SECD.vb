Module SECD

    Dim s As LK_Object ' stack
    Dim e As LK_Object ' environment
    Dim c As LK_Object 'control
    Dim d As LK_Object ' dump

    Dim _true As LK_Object
    Dim _false As LK_Object
    Public _nil As LK_Object

    Dim _work As LK_Object

    Dim stopped = 0

    Dim op_code() As Action = {Sub()
                                   Console.WriteLine($"Invalid opcode")
                               End Sub,
                               AddressOf _ld,
                               AddressOf _ldc,
                               AddressOf _ldf,
                               AddressOf _ap,
                               AddressOf _rtn,
                               AddressOf _dum,
                               AddressOf _rap,
                               AddressOf _sel,
                               AddressOf _join,
                               AddressOf _car,
                               AddressOf _cdr,
                               AddressOf _atom,
                               AddressOf _cons,
                               AddressOf _eq,
                               AddressOf _add,
                               AddressOf _sub,
                               AddressOf _mul,
                               AddressOf _div,
                               AddressOf _rem,
                               AddressOf _leq,
                               AddressOf _stop}

    Function disassemble(program As LK_Object) As String
        ' Returns a string representation of the instruction at the head of the program
        Debug.Assert(program.car().isNumber(), $"Not an opcode")
        Select Case program.car().number
            Case 1
                Return $"ld {program.cdr().car().ToString()}"
            Case 2
                Return $"ldc {program.cdr().car().ToString()}"
            Case 3
                Return $"ldf {program.car()}"
            Case 4
                Return $"ap"
            Case 5
                Return $"rtn"
            Case 6
                Return $"dum"
            Case 7
                Return $"rap"
            Case 8
                Return $"sel"
            Case 9
                Return "join"
            Case 10
                Return $"car"
            Case 11
                Return $"cdr"
            Case 12
                Return $"atom"
            Case 13
                Return $"cons"
            Case 14
                Return $"eq"
            Case 15
                Return $"add"
            Case 16
                Return $"sub"
            Case 17
                Return $"mul"
            Case 18
                Return $"div"
            Case 19
                Return $"rem"
            Case 20
                Return $"leq"
            Case 21
                Return $"stop"
            Case Else
                Return $"Invalid opcode {program.car().number}"
        End Select
    End Function

    Sub init()
        gc_init()

        _true = New LK_Object("T")
        _false = New LK_Object("F")
        _nil = New LK_Object("NIL")

        s = _nil
        c = _nil
        e = _nil
        d = _nil

        _work = _nil
    End Sub

    Sub _ldc()
        s = New LK_Object(c.cdr().car(), s)
        c = c.cdr().cdr()
    End Sub

    Sub _ld()
        Dim i = 0
        _work = e

        For i = 1 To numberValue(c.cdr().car().car())
            _work = _work.cdr()
        Next
        _work = _work.car()
        For i = 1 To numberValue(c.cdr().car().cdr())
            _work = _work.cdr
        Next
        _work = _work.car()
        s = New LK_Object(_work, s)
        c = c.cdr().cdr()
        _work = _nil
    End Sub

    Sub pushBoolean(b As Boolean)
        If b Then
            s = New LK_Object(_true, s.cdr())
        Else
            s = New LK_Object(_false, s.cdr())
        End If
    End Sub

    Sub nextInstruction()
        c = c.cdr()
    End Sub

    Sub _car()
        s = New LK_Object(s.car().car(), s.cdr())
        nextInstruction()
    End Sub

    Sub _cdr()
        s = New LK_Object(s.car().cdr(), s.cdr())
        nextInstruction()
    End Sub

    Sub _atom()
        pushBoolean(s.car().isNumber() OrElse s.car().isSymbol())
        nextInstruction()
    End Sub

    Sub _cons()
        s = New LK_Object(New LK_Object(s.car(), s.cdr().car()), s.cdr().cdr())
        nextInstruction()
    End Sub

    Sub _sub()
        s = New LK_Object(New LK_Object(numberValue(s.cdr().car()) - numberValue(s.car())), s.cdr().cdr())
        nextInstruction()
    End Sub

    Sub _add()
        s = New LK_Object(New LK_Object(numberValue(s.cdr().car()) + numberValue(s.car())), s.cdr().cdr())
        nextInstruction()
    End Sub

    Sub _mul()
        s = New LK_Object(New LK_Object(numberValue(s.cdr().car()) * numberValue(s.car())), s.cdr().cdr())
        nextInstruction()
    End Sub

    Sub _div()
        s = New LK_Object(New LK_Object(numberValue(s.cdr().car()) \ numberValue(s.car())), s.cdr().cdr())
        nextInstruction()
    End Sub

    Sub _rem()
        s = New LK_Object(New LK_Object(numberValue(s.cdr().car()) Mod numberValue(s.car())), s.cdr().cdr())
        nextInstruction()
    End Sub

    Sub _leq()
        pushBoolean(numberValue(s.cdr().car()) <= numberValue(s.car()))
        nextInstruction()
    End Sub

    Sub _eq()
        If s.car().isSymbol() AndAlso s.cdr().car().isSymbol() Then
            pushBoolean(stringValue(s.car) = stringValue(s.cdr().car()))
        ElseIf s.car().isNumber() AndAlso s.cdr().car().isNumber() Then
            pushBoolean((numberValue(s.car) = numberValue(s.cdr().car())))
        End If
        nextInstruction()
    End Sub

    Sub _ldf()
        _work = New LK_Object(c.cdr().car(), e)
        s = New LK_Object(_work, s)
        c = c.cdr().cdr()
        _work = _nil
    End Sub

    Sub _rtn()
        s = New LK_Object(s.car(), d.car())
        e = d.cdr().car()
        c = d.cdr().cdr().car()
        d = d.cdr().cdr().cdr()
    End Sub

    Sub _dum()
        e = New LK_Object(_nil, e)
        nextInstruction()
    End Sub

    Sub _rap()
        d = New LK_Object(c.cdr(), d)
        d = New LK_Object(e.cdr(), d)
        d = New LK_Object(s.cdr().cdr(), d)
        e = s.car().cdr()
        e.cons.car = s.cdr().car()
        c = s.car().car()
        s = _nil
    End Sub

    Sub _sel()
        d = New LK_Object(c.cdr().cdr().cdr(), d)
        If stringValue(s.car()) = stringValue(_true) Then
            c = c.cdr().car()
        Else
            c = c.cdr().cdr().car()
        End If
        s = s.cdr()
    End Sub

    Sub _join()
        c = d.car()
        d = d.cdr()
    End Sub

    Sub _ap()
        d = New LK_Object(c.cdr(), d)
        d = New LK_Object(e, d)
        d = New LK_Object(s.cdr().cdr(), d)
        e = New LK_Object(s.cdr().car(), s.car().cdr())
        c = s.car().car()
        s = _nil
    End Sub

    Sub _stop()
        stopped = 1
    End Sub

    Function execute(fn As LK_Object, arguments As LK_Object) As LK_Object
        s = New LK_Object(arguments, _nil)
        e = _nil
        c = fn
        d = _nil

        stopped = 0
        Dim pc = 0
        Do
            Debug.Assert(c.isCons() AndAlso c.car().isNumber())
            If isVerbose Then
                Console.WriteLine($"Executing {pc}: {disassemble(c)}")
            End If
            op_code(numberValue(c.car()))()
            pc += 1
        Loop Until stopped > 0

        Return s.car()
    End Function
End Module

