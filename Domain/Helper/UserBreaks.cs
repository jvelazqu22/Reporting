namespace Domain.Helper
{
    public class UserBreaks
    {
        public bool UserBreak1 { get; set; }
        public bool UserBreak2 { get; set; }
        public bool UserBreak3 { get; set; }
        public bool AnyBreaks { get { return UserBreak1 || UserBreak2 || UserBreak3; } }
    }
}
