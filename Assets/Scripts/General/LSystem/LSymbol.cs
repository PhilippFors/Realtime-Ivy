namespace General.LSystem
{
    [System.Serializable]
    public struct LSymbol
    {
        public string identifier;

        public bool Equals(LSymbol id) => id.identifier.Equals(identifier);
    }
}