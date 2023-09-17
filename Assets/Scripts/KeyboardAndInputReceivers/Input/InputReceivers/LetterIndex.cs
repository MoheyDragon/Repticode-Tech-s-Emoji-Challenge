using System;

public struct LetterIndex : IEquatable<LetterIndex>
{
    public int taskIndex;
    public int subTaskIndex;
    public int letterIndex;
    public override bool Equals(object obj)
    {
        return obj is LetterIndex currentLetterIndex &&
            taskIndex == currentLetterIndex.taskIndex &&
            subTaskIndex == currentLetterIndex.subTaskIndex &&
            letterIndex == currentLetterIndex.letterIndex;
    }
    public bool Equals(LetterIndex other)
    {
        throw new NotImplementedException();
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    public override string ToString()
    {
        return base.ToString();
    }
    public static bool operator ==(LetterIndex a, LetterIndex b)
    {
        return a.taskIndex == b.taskIndex && a.subTaskIndex == b.subTaskIndex && a.letterIndex == b.letterIndex;
    }
    public static bool operator !=(LetterIndex a, LetterIndex b)
    {
        return !(a == b);
    }
}