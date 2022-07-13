Module Print


    Sub exp_print(obj As LK_Object)
        If obj IsNot Nothing Then AndAlso obj.isNotNil() Then
            ' See comment in LK_Object.IsNil
            If obj.isNumber() Then
                Console.Write($"{obj.number} ")
            ElseIf obj.isSymbol() Then
                Console.Write($"{obj.symbol} ")
            ElseIf obj.isCons() Then
                If obj.car().isNotCons() AndAlso obj.cdr().isNotCons() AndAlso obj.cdr().isNotNil() Then
                    ' Dotted pair
                    exp_print(obj.car())
                    Console.Write(". ")
                    exp_print(obj.cdr())
                Else
                    If obj.car().isCons() Then
                        Console.Write("( ")
                    End If
                    exp_print(obj.car())
                    If obj.car().isCons() Then
                        Console.Write(") ")
                    End If
                    If obj IsNot Nothing AndAlso obj.cdr() IsNot Nothing AndAlso obj.cdr().isNotNil() Then
                        exp_print(obj.cdr())
                    End If
                End If
            Else
                Console.WriteLine($"Unknown type: {obj.typeString()}")
            End If
        End If
    End Sub

End Module
