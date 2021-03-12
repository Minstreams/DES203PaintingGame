using System;
using System.Collections.Generic;

namespace GameSystem.Requirements
{
    [System.Serializable]
    public class Requirement : IComparable
    {
        [Label] public string name = "new requirement";
        [Label] public string description = "description";
        [Label] public string path;
        [Label] public RequirementPriority priority = RequirementPriority.normal;
        [Label] public RequirementStatus status = RequirementStatus.@unchecked;
        [Label] public string responsiblePerson;
        [Label] public string comment;
        [Label] public string feedback;

        public int CompareTo(object obj)
        {
            return string.Compare(path, (obj as Requirement).path);
        }
    }
    [System.Flags]
    public enum RequirementPriority
    {
        optional = 1,
        normal = 2,
        urgent = 4,
    }
    [System.Flags]
    public enum RequirementStatus
    {
        @unchecked = 1,
        @checked = 2,
        stable = 4,
    }
}