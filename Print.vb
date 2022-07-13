Module Print


    Sub exp_print(obj As LK_Object)
        If obj IsNot Nothing Then
            If obj.isSymbol() Then
                Console.Write($"{obj.symbol} ")
            ElseIf obj.isNumber() Then
                Console.Write($"{obj.number} ")
            Else
                Debug.Assert(obj.isCons(), $"Unknown type: {obj.typeString()}")

                Console.Write("( ")
                Dim p = obj
                Do While p.isCons()
                    exp_print(p.car())
                    p = p.cdr()
                Loop
                If p.isSymbol() AndAlso p.symbol = "NIL" Then
                Else
                    Console.Write(". ")
                    exp_print(p)
                End If
                Console.Write(") ")
            End If
        Else
            Debug.Fail("Trying to print Nothing")
        End If
    End Sub

End Module
