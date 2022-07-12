Module SECD

    Dim _stack As LK_Object
    Dim _environ As LK_Object
    Dim _control As LK_Object
    Dim _dump As LK_Object

    Dim _true As LK_Object
    Dim _false As LK_Object
    Dim _nil As LK_Object

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

    Sub init()
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

    Sub _ldc()
        _stack = New LK_Object(_control.cdr().car(), _stack)
        _control = _control.cdr().cdr()
    End Sub

    Sub _ld()
        Dim i = 0
        _work = _environ

        For i = 1 To numberValue(_control.cdr().car().car())
            _work = _work.cdr()
        Next
        _work = _work.car()
        For i = 1 To numberValue(_control.cdr().car().cdr())
            _work = _work.cdr
        Next
        _work = _work.car()
        _stack = New LK_Object(_work, _stack)
        _control = _control.cdr().cdr()
        _work = Nothing
    End Sub

    Sub pushBoolean(b As Boolean)
        If b Then
            _stack = New LK_Object(_true, _stack.cdr())
        Else
            _stack = New LK_Object(_false, _stack.cdr())
        End If
    End Sub

    Sub nextInstruction()
        _control = _control.cdr()
    End Sub

    Sub _car()
        _stack = New LK_Object(_stack.car().car(), _stack.cdr())
        nextInstruction()
    End Sub

    Sub _cdr()
        _stack = New LK_Object(_stack.car().cdr(), _stack.cdr())
        nextInstruction()
    End Sub

    Sub _atom()
        pushBoolean(_stack.car().isNumber() OrElse _stack.car().isSymbol())
        nextInstruction()
    End Sub

    Sub _cons()
        _stack = New LK_Object(New LK_Object(_stack.car(), _stack.cdr().car()), _stack.cdr().cdr())
        nextInstruction()
    End Sub

    Sub _sub()
        _stack = New LK_Object(New LK_Object(numberValue(_stack.cdr().car()) - numberValue(_stack.car())), _stack.cdr().cdr())
        nextInstruction()
    End Sub

    Sub _add()
        _stack = New LK_Object(New LK_Object(numberValue(_stack.cdr().car()) + numberValue(_stack.car())), _stack.cdr().cdr())
        nextInstruction()
    End Sub

    Sub _mul()
        _stack = New LK_Object(New LK_Object(numberValue(_stack.cdr().car()) * numberValue(_stack.car())), _stack.cdr().cdr())
        nextInstruction()
    End Sub

    Sub _div()
        _stack = New LK_Object(New LK_Object(numberValue(_stack.cdr().car()) \ numberValue(_stack.car())), _stack.cdr().cdr())
        nextInstruction()
    End Sub

    Sub _rem()
        _stack = New LK_Object(New LK_Object(numberValue(_stack.cdr().car()) Mod numberValue(_stack.car())), _stack.cdr().cdr())
        nextInstruction()
    End Sub

    Sub _leq()
        pushBoolean(numberValue(_stack.cdr().car()) <= numberValue(_stack.car()))
        nextInstruction()
    End Sub

    Sub _eq()
        If _stack.car().isSymbol() AndAlso _stack.cdr().car().isSymbol() Then
            pushBoolean(stringValue(_stack.car) = stringValue(_stack.cdr().car()))
        ElseIf _stack.car().isNumber() AndAlso _stack.cdr().car().isNumber() Then
            pushBoolean((numberValue(_stack.car) = numberValue(_stack.cdr().car())))
        End If
        nextInstruction()
    End Sub

    Sub _ldf()
        _work = New LK_Object(_control.cdr().car(), _environ)
        _stack = New LK_Object(_work, _stack)
        _control = _control.cdr().cdr()
        _work = _nil
    End Sub

    Sub _rtn()
        _stack = New LK_Object(_stack.car(), _dump.car())
        _environ = _dump.cdr().car()
        _control = _dump.cdr().cdr().car()
        _dump = _dump.cdr().cdr().cdr()
    End Sub

    Sub _dum()
        _environ = New LK_Object(_nil, _environ)
        nextInstruction()
    End Sub

    Sub _rap()
        _dump = New LK_Object(_control.cdr(), _dump)
        _dump = New LK_Object(_environ.cdr(), _dump)
        _dump = New LK_Object(_stack.cdr().cdr(), _dump)
        _environ = _stack.car().cdr()
        _environ.cons.car = _stack.car().cdr()
        _control = _stack.car().car()
        _stack = _nil
    End Sub

    Sub _sel()
        _dump = New LK_Object(_control.cdr().cdr().cdr(), _dump)
        If stringValue(_stack.car()) = stringValue(_true) Then
            _control = _control.cdr().car()
        Else
            _control = _control.cdr().cdr().car()
        End If
        _stack = _stack.cdr()
    End Sub

    Sub _join()
        _control = _dump.car()
        _dump = _dump.cdr()
    End Sub

    Sub _ap()
        _dump = New LK_Object(_control.cdr(), _dump)
        _dump = New LK_Object(_environ, _dump)
        _dump = New LK_Object(_stack.cdr().cdr(), _dump)
        _environ = New LK_Object(_stack.cdr().car(), _stack.car().cdr())
        _control = _stack.car().car()
        _stack = _nil
    End Sub

    Sub _stop()
        stopped = 1
    End Sub

    Function execute(fn As LK_Object, arguments As LK_Object) As LK_Object
        _stack = New LK_Object(arguments, _nil)
        _environ = _nil
        _control = fn
        _dump = _nil

        stopped = 0
        Do
            op_code(numberValue(_control.car()))()
        Loop Until stopped > 0

        Return _stack
    End Function
End Module

