namespace ResoniteWasm.Locale;

public static class I18NGlobal {
    
    public static string Local(this string id, params object[] args) {
        string str = I18N.Instance.GetValue(id);
        return args.Length == 0 ? str : string.Format(str, args);
    }
}
