using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Remoting.Messaging;
using Serilog;

namespace myClass
{
    //public class TypeOfClassHandler
    //{
    //    private Dictionary<Type, Action<object>> typesOfClassesDictionary = new Dictionary<Type, Action<object>>();
    //    private static TypeOfClassHandler instance = new TypeOfClassHandler();
    //    private TypeOfClassHandler() 
    //    {
    //        CreateTheLogger();
    //        typesOfClassesDictionary.Add(typeof(Student),StudnentType);
    //        typesOfClassesDictionary.Add(typeof(Family), FamilyType);
    //        typesOfClassesDictionary.Add(typeof(Image), ImageType);
    //    }
    //    private void StudnentType(Object myObject)
    //    {
    //        Student student = (Student)myObject;
    //        Log.Information(student.ToString());
    //    }
    //    private void FamilyType(Object myObject)
    //    {
    //        Family family = (Family)myObject;
    //        Log.Information(family.ToString());
    //    }
    //    private void ImageType(Object myObject)
    //    {
    //        Image image = (Image)myObject;
    //        try
    //        {
    //            // Write the byte array to the file, effectively creating the image
    //            File.WriteAllBytes(@"D:\temp", image.GetBytes());
    //            Log.Information("The image {0} successfully saved to: D:\\temp", image.GetName());
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine("Error: " + e.Message);
    //        }
    //    }
    //    public static TypeOfClassHandler GetInstance()
    //    {
    //        return instance;
    //    }
    //    public void CastToCorrectClass(Object myObject)
    //    {
    //        typesOfClassesDictionary[myObject.GetType()](myObject);
    //    }
    //    public static void CreateTheLogger()
    //    {
    //        Log.Logger = new LoggerConfiguration()
    //            .WriteTo.Console()
    //        .WriteTo.File("C:\\Users\\User\\source\\repos\\Adam's TcpServer\\bin\\Debug\\ServerChatLog.txt", rollingInterval: RollingInterval.Infinite, shared: true)
    //        .CreateLogger();
    //    }
    //}
}
