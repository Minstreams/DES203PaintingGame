using UnityEngine;
using System;

/// <summary>
/// 一个分隔符
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public class SeparatorAttribute : PropertyAttribute
{
    /// <summary>
    /// 空白像素数
    /// </summary>
    public float Space = 16;

    public SeparatorAttribute()
    {
        this.order = -10;
    }
    public SeparatorAttribute(float space)
    {
        this.order = -10;
        this.Space = space;
    }
}