namespace XmlEditor
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new XmlEditorForm() {
                //XmlFile = "C:\\Users\\master\\Downloads\\TheConfiguration.xml",
                //XmlSearch = "//Fields/Field",
                //XmlMappings = [
                //    ("Caption", "./Caption/TStr/T/S"),
                //    ("FieldID", "./FieldID"),
                //]
            }.Init());
        }
    }
}