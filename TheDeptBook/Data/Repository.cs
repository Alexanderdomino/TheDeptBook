using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;
using TheDeptBook.Model;

namespace TheDeptBook.Data;

public class Repository
{
    internal static ObservableCollection<Debitor> ReadFile(string fileName)
    {
        // Create an instance of the XmlSerializer class and specify the type of object to deserialize.
        XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Debitor>));
        TextReader reader = new StreamReader(fileName);
        // Deserialize all the debitors.
        var debitors = (ObservableCollection<Debitor>)serializer.Deserialize(reader)!;
        reader.Close();
        return debitors;
    }
    internal static void SaveFile(string fileName, ObservableCollection<Debitor> debitors)
    {
        // Create an instance of the XmlSerializer class and specify the type of object to serialize.
        XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Debitor>));
        TextWriter writer = new StreamWriter(fileName);
        // Serialize all the debitors.
        serializer.Serialize(writer, debitors);
        writer.Close();
    }
}