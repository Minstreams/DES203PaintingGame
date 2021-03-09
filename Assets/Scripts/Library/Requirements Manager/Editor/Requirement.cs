namespace GameSystem.Requirements
{
    [System.Serializable]
    public struct Requirement
    {
        [Label] public string name;
        [Label] public string description;
        [Label] public string path;
        [Label] public RequirementPriority priority;
        [Label] public RequirementStatus status;
        [Label] public string responsiblePerson;
        [Label] public string comment;
        [Label] public string feedback;
    }
    [System.Flags]
    public enum RequirementPriority
    {
        normal = 1,
        urgent = 2,
        optional = 4,
    }
    [System.Flags]
    public enum RequirementStatus
    {
        @unchecked = 1,
        @checked = 2,
        stable = 4,
    }
}