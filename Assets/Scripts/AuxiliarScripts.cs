using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class AuxiliarScripts : MonoBehaviour {

    /// <summary>
    /// Returns the name of the variable
    /// </summary>
    public static string GetVariableName<T>(Expression<Func<T>> memberExpression) {
        MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
        return expressionBody.Member.Name;
    }
}
