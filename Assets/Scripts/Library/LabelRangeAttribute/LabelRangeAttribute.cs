using System;
using UnityEngine;

/// <summary>
/// 代替原变量标签，集成range功能，适用于float
/// to replace default label style with a slide to define the range of a float
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class LabelRangeAttribute : PropertyAttribute
{
    /// <summary>
    /// 标签内容，留空显示默认内容
    /// the content of label. leave empty to show default label.
    /// </summary>
    public string Label = "";
    /// <summary>
    /// 常量在游戏中不允许改变
    /// if Const, disallow changes during play
    /// </summary>
    public bool Const = false;
    public bool Disable = false;

    public float Left;
    public float Right;

    public LabelRangeAttribute(float left, float right)
    {
        this.Disable = true;
        this.Left = left;
        this.Right = Math.Max(left, right);
    }
    public LabelRangeAttribute(string label, float left, float right)
    {
        this.Label = label;
        this.Left = left;
        this.Right = Math.Max(left, right);
    }
    public LabelRangeAttribute(string label, float left, float right, bool isConst)
    {
        this.Label = label;
        this.Left = left;
        this.Right = Math.Max(left, right);
        this.Const = isConst;
    }
}