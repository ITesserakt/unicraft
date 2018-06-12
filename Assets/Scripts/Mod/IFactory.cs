namespace mc2.mod {
    public interface IFactory {
        string ShortName { set; }
        string FullName { set; }
        
        IItem Generate();
    }
}