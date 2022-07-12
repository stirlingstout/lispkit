Module LIspKit
    Public Function numberValue(obj As LK_Object) As Int64
        Debug.Assert(obj.isNumber(), "Object is not a number")

        Return obj.number
    End Function

    Public Function stringValue(obj As LK_Object) As String
        Debug.Assert(obj.isSymbol(), "Object is not a string")
        Return obj.symbol
    End Function


End Module
