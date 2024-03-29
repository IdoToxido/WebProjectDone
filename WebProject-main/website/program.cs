using System.Net;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
class Triangle(double? side0, double? side1, double? side2, double? angle0, double? angle1, double? angle2)
{
    public double? side0 { get; set; } = side0;
    public double? side1 { get; set; } = side1;
    public double? side2 { get; set; } = side2;
    public double? angle0 { get; set; } = angle0;
    public double? angle1 { get; set; } = angle1;
    public double? angle2 { get; set; } = angle2;
}
class Program
{
    public static void Main()
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://*:5000/");
        listener.Start();

        Console.WriteLine("Server started. Listening for requests...");
        Console.WriteLine("Main page on http://localhost:5000/website/index.html");

        while (true)
        {
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;


            string rawPath = request.RawUrl!;
            string absPath = request.Url!.AbsolutePath;


            Console.WriteLine("Received a request with path: " + rawPath);


            string filePath = "." + absPath;
            bool isHtml = request.AcceptTypes!.Contains("text/html");


            if (File.Exists(filePath) && (filePath.EndsWith(".html") || filePath.EndsWith(".css") || filePath.EndsWith(".js") || filePath.EndsWith(".png")))
            {
                byte[] fileBytes = File.ReadAllBytes(filePath);
                if (filePath.EndsWith(".html"))
                {
                    response.ContentType = "text/html; charset=utf-8";
                }
                else if (filePath.EndsWith(".css"))
                {
                    response.ContentType = "text/css";
                }
                else if (filePath.EndsWith(".js"))
                {
                    response.ContentType = "application/javascript";
                }
                else if (filePath.EndsWith(".png"))
                {
                    response.ContentType = "application/png";
                }
                else if (filePath.EndsWith(".otf"))
                {
                    response.ContentType = "application/otf";
                }
                response.OutputStream.Write(fileBytes);
            }

            else if (isHtml)
            {
                response.StatusCode = (int)HttpStatusCode.Redirect;
                response.RedirectLocation = "/website/404.html";
            }
            else if (absPath == "/message")
            {
                Console.WriteLine("Recieved triangle");
                string triangleJson = GetBody(request);
                Triangle triangle = JsonSerializer.Deserialize<Triangle>(triangleJson)!;
                Console.WriteLine("Triangle recieved properties: ");
                Function_Display_Properties(triangle);
                Console.WriteLine("End");

                Triangle RtnTriangle = TrigonomericMain(triangle);
                Console.WriteLine("Triangle sent properties");
                Function_Display_Properties(RtnTriangle);
                Console.WriteLine("End");
                
                response.OutputStream.Write(ToBytes(RtnTriangle));
                Console.WriteLine("Sent the triangle");
            }

            response.Close();
        }
    }

    public static byte[] ToBytes<T>(T value)
    {
        string json = JsonSerializer.Serialize(value);
        return Encoding.UTF8.GetBytes(json);
    }
    public static string GetBody(HttpListenerRequest request)
    {
        return new StreamReader(request.InputStream).ReadToEnd();
    }
    public static Triangle TrigonomericMain(Triangle triangle)
    {

        double?[] LNull = { triangle.side0, triangle.side1, triangle.side2 };
        double?[] ANull = { triangle.angle0, triangle.angle1, triangle.angle2 };

        double[] L = Function_Lengths_Convert_null_To_0(LNull);
        double[] A = Function_Angles_Convert_null_To_0(ANull);

        while (!Function_All_Done(L, A))
        {
            // 90* Triangle
            if (A[0] == 90)
            {
                // Pythagoras
                L = Function_Pythagoras(L);
                // Angles Trigo 90*
                A = Function_90_Degrees_Angle_Cal_By_Sin_Cosin_Tan(A, L);
                // Sides Trigo 90*
                L = Function_90_Degrees_Side_Cal_By_Sin_Cosin_Tan(A, L);
            }
            // Non 90* triangle
            if (A[0] != 90)
            {
                A = Function_Non_90_Degrees_Angle_Cal_By_CoSine(A, L);
                A = Function_Non_90_Degrees_Angle_Cal_By_Sin(A, L);
                L = Function_Non_90_Degrees_Side_Cal_By_CoSine(A, L);
                L = Function_Non_90_Degrees_Side_Cal_By_Sin(A, L);
            }
            A = Function_Complete_Angle(A);
        }
        L = Function_Round_Sides(L);
        A = Function_Round_Angles(A);

        foreach (double var in L)
        {
            Console.WriteLine(var);
        }
        foreach (double var in A)
        {
            Console.WriteLine(var);
        }


        return new Triangle(L[0], L[1], L[2], A[0], A[1], A[2]);
    }
    public static void Function_Display_Properties(Triangle triangle)
    {
        Console.WriteLine($"side 0: {triangle.side0}");

        Console.WriteLine($"side 1: {triangle.side1}");

        Console.WriteLine($"side 2: {triangle.side2}");

        Console.WriteLine($"angle 0: {triangle.angle0}");

        Console.WriteLine($"angle 1: {triangle.angle1}");

        Console.WriteLine($"angle 2: {triangle.angle2}");
    }
    public static double[] Function_Angles_Convert_null_To_0(double?[] A)
    {
        if (A == null)
        {
            throw new ArgumentNullException(nameof(A));
        }

        double[] rtn = new double[A.Length];
        for (int i = 0; i < A.Length; i++)
        {
            if (A[i] == null)
            {
                rtn[i] = 0;
            }
            else
            {
                rtn[i] = (double)A[i]!;
            }
        }
        return rtn;
    }
    public static double[] Function_Lengths_Convert_null_To_0(double?[] L)
    {
        if (L == null)
        {
            throw new ArgumentNullException(nameof(L));
        }

        double[] rtn = new double[L.Length];
        for (int i = 0; i < L.Length; i++)
        {
            if (L[i] == null)
            {
                rtn[i] = 0;
            }
            else
            {
                rtn[i] = (double)L[i]!;
            }
        }
        return rtn;
    }
    public static bool Function_All_Done(double[] Lengths, double[] Angles)
    {

        foreach (double var in Lengths)
        {
            if (var == 0)
            {
                return false;
            }
        }
        foreach (double var in Angles)
        {
            if (var == 0)
            {
                return false;
            }
        }
        return true;
    }
    public static double[] Function_Pythagoras(double[] L)
    {
        if (L[0] == 0 && L[2] != 0 && L[1] != 0)
        {
            L[0] = Math.Sqrt(Math.Pow(L[2], 2) - Math.Pow(L[1], 2));
        }
        if (L[1] == 0 && L[2] != 0 && L[0] != 0)
        {
            L[1] = Math.Sqrt((L[2] * L[2]) - (L[0] * L[0]));
            Console.WriteLine($"{L[2] * L[2]} - {L[0] * L[0]}");
        }
        if (L[2] == 0 && L[1] != 0 && L[0] != 0)
        {
            L[2] = Math.Sqrt(Math.Pow(L[1], 2) + Math.Pow(L[0], 2));
        }
        return L;

    }
    public static double[] Function_Complete_Angle(double[] A)
    {
        int givenAngles = 0;
        foreach (double var in A)
        {
            if (var != 0)
            {
                givenAngles++;
            }
        }
        if (givenAngles == 2)
        {
            if (A[0] == 0 && A[1] != 0 && A[2] != 0)
            {
                A[0] = 180 - A[1] - A[2];
            }

            if (A[1] == 0 && A[0] != 0 && A[2] != 0)
            {
                A[1] = 180 - A[0] - A[2];
            }
            if (A[2] == 0 && A[0] != 0 && A[1] != 0)
            {
                A[2] = 180 - A[0] - A[1];
            }
        }
        return A;
    }
    public static double[] Function_90_Degrees_Angle_Cal_By_Sin_Cosin_Tan(double[] A, double[] L)
    {
        // sin
        if (A[1] == 0 && L[1] != 0 && L[2] != 0)
        {
            Console.WriteLine(L[1]);
            A[1] = Math.Asin(L[1] / L[2]) * (180.0 / Math.PI);
        }
        if (A[2] == 0 && L[0] != 0 && L[2] != 0)
        {
            A[2] = Math.Asin(L[0] / L[2]) * (180.0 / Math.PI);

        }
        // cosin
        if (A[1] == 0 && L[0] != 0 && L[2] != 0)
        {
            A[1] = Math.Acos(L[0] / L[2]) * (180.0 / Math.PI);

        }
        if (A[2] == 0 && L[1] != 0 && L[2] != 0)
        {
            A[2] = Math.Acos(L[1] / L[2]) * (180.0 / Math.PI);

        }
        // tan
        if (A[1] == 0 && L[0] != 0 && L[1] != 0)
        {
            A[1] = Math.Atan(L[1] / L[0]) * (180.0 / Math.PI);

        }
        if (A[2] == 0 && L[0] != 0 && L[1] != 0)
        {
            A[2] = Math.Atan(L[0] / L[1]) * (180.0 / Math.PI);

        }
        return A;
    }
    public static double[] Function_90_Degrees_Side_Cal_By_Sin_Cosin_Tan(double[] A, double[] L)
    {
        //sin
        if (L[0] == 0 && L[2] != 0 && A[2] != 0)
        {
            L[0] = L[2] * Math.Sin(A[2] * (Math.PI / 180.0));
        }
        if (L[1] == 0 && L[2] != 0 && A[1] != 0)
        {
            L[1] = L[2] * Math.Sin(A[1] * (Math.PI / 180.0));
        }
        if (L[2] == 0 && L[0] != 0 && A[2] != 0)
        {
            L[2] = L[0] / Math.Sin(A[2] * (Math.PI / 180.0));
        }
        if (L[2] == 0 && L[1] != 0 && A[1] != 0)
        {
            L[2] = L[1] / Math.Sin(A[1] * (Math.PI / 180.0));
        }
        // cosin
        if (L[0] == 0 && L[2] != 0 && A[1] != 0)
        {
            L[0] = L[2] * Math.Cos(A[1] * (Math.PI / 180.0));
        }
        if (L[1] == 0 && L[2] != 0 && A[2] != 0)
        {
            L[1] = L[2] * Math.Cos(A[2] * (Math.PI / 180.0));
        }
        if (L[2] == 0 && L[0] != 0 && A[1] != 0)
        {
            L[2] = L[0] / Math.Cos(A[1] * (Math.PI / 180.0));
        }
        if (L[2] == 0 && L[1] != 0 && A[2] != 0)
        {
            L[2] = L[1] / Math.Cos(A[2] * (Math.PI / 180.0));
        }
        // tan
        if (L[0] == 0 && L[1] != 0 && A[2] != 0)
        {
            L[0] = L[1] * Math.Tan(A[2] * (Math.PI / 180.0));
        }
        if (L[1] == 0 && L[0] != 0 && A[1] != 0)
        {
            L[1] = L[0] * Math.Tan(A[1] * (Math.PI / 180.0));
        }
        if (L[0] == 0 && L[1] != 0 && A[1] != 0)
        {
            L[0] = L[1] / Math.Tan(A[1] * (Math.PI / 180.0));
        }
        if (L[1] == 0 && L[0] != 0 && A[2] != 0)
        {
            L[1] = L[0] / Math.Tan(A[2] * (Math.PI / 180.0));
        }

        return L;
    }
    public static double[] Function_Non_90_Degrees_Side_Cal_By_Sin(double[] A, double[] L)
    {
        // find L[0]
        if (L[0] == 0 && L[1] != 0 && A[2] != 0 && A[1] != 0)
        {
            L[0] = L[1] * Math.Sin(A[2] * (Math.PI / 180.0)) / Math.Sin(A[1] * (Math.PI / 180.0));
        }
        if (L[0] == 0 && L[2] != 0 && A[2] != 0 && A[0] != 0)
        {
            L[0] = L[2] * Math.Sin(A[2] * (Math.PI / 180.0)) / Math.Sin(A[0] * (Math.PI / 180.0));
        }
        // find L[1]
        if (L[1] == 0 && L[0] != 0 && A[1] != 0 && A[2] != 0)
        {
            L[1] = L[0] * Math.Sin(A[1] * (Math.PI / 180.0)) / Math.Sin(A[2] * (Math.PI / 180.0));
            ;
        }
        if (L[1] == 0 && L[2] != 0 && A[1] != 0 && A[0] != 0)
        {
            L[1] = L[2] * Math.Sin(A[1] * (Math.PI / 180.0)) / Math.Sin(A[0] * (Math.PI / 180.0));

        }
        // find L[2]
        if (L[2] == 0 && L[0] != 0 && A[0] != 0 && A[2] != 0)
        {
            L[2] = L[0] * Math.Sin(A[0] * (Math.PI / 180.0)) / Math.Sin(A[2] * (Math.PI / 180.0));
        }
        if (L[2] == 0 && L[1] != 0 && A[0] != 0 && A[1] != 0)
        {
            L[2] = L[1] * Math.Sin(A[0] * (Math.PI / 180.0)) / Math.Sin(A[1] * (Math.PI / 180.0));
        }

        return L;
    }
    public static double[] Function_Non_90_Degrees_Angle_Cal_By_Sin(double[] A, double[] L)
    {
        A = Function_Complete_Angle(A);
        if (A[0] == 0 && L[2] != 0 && A[1] != 0 && L[1] != 0)
        {
            A[0] = Math.Asin(L[2] * Math.Sin(A[1] * (Math.PI / 180.0)) / L[1]) * (180.0 / Math.PI);
        }
        A = Function_Complete_Angle(A);
        if (A[0] == 0 && L[2] != 0 && A[2] != 0 && L[0] != 0)
        {
            A[0] = Math.Asin(L[2] * Math.Sin(A[2] * (Math.PI / 180.0)) / L[0]) * (180.0 / Math.PI);
        }
        A = Function_Complete_Angle(A);
        if (A[1] == 0 && L[1] != 0 && A[0] != 0 && L[2] != 0)
        {
            A[1] = Math.Asin(L[1] * Math.Sin(A[0] * (Math.PI / 180.0)) / L[2]) * (180.0 / Math.PI);
        }
        A = Function_Complete_Angle(A);
        if (A[1] == 0 && L[1] != 0 && A[2] != 0 && L[0] != 0)
        {
            A[1] = Math.Asin(L[1] * Math.Sin(A[2] * (Math.PI / 180.0)) / L[0]) * (180.0 / Math.PI);
        }
        A = Function_Complete_Angle(A);
        if (A[2] == 0 && L[0] != 0 && A[1] != 0 && L[1] != 0)
        {
            A[2] = Math.Asin(L[0] * Math.Sin(A[1] * (Math.PI / 180.0)) / L[1]) * (180.0 / Math.PI);
        }
        A = Function_Complete_Angle(A);
        if (A[2] == 0 && L[0] != 0 && A[0] != 0 && L[2] != 0)
        {
            A[2] = Math.Asin(L[0] * Math.Sin(A[0] * (Math.PI / 180.0)) / L[2]) * (180.0 / Math.PI);
        }
        A = Function_Complete_Angle(A);
        return A;
    }
    public static double[] Function_Non_90_Degrees_Side_Cal_By_CoSine(double[] A, double[] L)
    {
        if (L[0] == 0 && L[1] != 0 && L[2] != 0 && A[2] != 0)
        {
            L[0] = Math.Sqrt(L[1] * L[1] + L[2] * L[2] - 2 * L[1] * L[2] * Math.Cos(A[2] * (Math.PI / 180.0)));
        }
        if (L[1] == 0 && L[0] != 0 && L[2] != 0 && A[1] != 0)
        {
            L[1] = Math.Sqrt(L[0] * L[0] + L[2] * L[2] - 2 * L[0] * L[2] * Math.Cos(A[1] * (Math.PI / 180.0)));
        }
        if (L[2] == 0 && L[0] != 0 && L[1] != 0 && A[0] != 0)
        {
            L[2] = Math.Sqrt(L[0] * L[0] + L[1] * L[1] - 2 * L[0] * L[1] * Math.Cos(A[0] * (Math.PI / 180.0)));
        }
        return L;
    }
    public static double[] Function_Non_90_Degrees_Angle_Cal_By_CoSine(double[] A, double[] L)
    {
        if (L[0] != 0 && L[1] != 0 && L[2] != 0)
        {
            if (A[0] == 0)
            {
                double PowSum = L[0] * L[0] + L[1] * L[1] - L[2] * L[2];
                A[0] = Math.Acos(PowSum / (2 * L[0] * L[1])) * (180.0 / Math.PI);
            }

            if (A[1] == 0)
            {
                double PowSum = L[0] * L[0] + L[2] * L[2] - L[1] * L[1];
                A[1] = Math.Acos(PowSum / (2 * L[0] * L[2])) * (180.0 / Math.PI);
            }

            if (A[2] == 0)
            {
                double PowSum = L[1] * L[1] + L[2] * L[2] - L[0] * L[0];
                A[2] = Math.Acos(PowSum / (2 * L[1] * L[2])) * (180.0 / Math.PI);
            }
        }
        return A;
    }
    public static double[] Function_Round_Sides(double[] L)
    {
        double[] Rounded_Sides = { };

        for (int i = 0; i < 3; i++)
        {
            if (L[i].ToString().Contains('.'))
            {
                if (L[i].ToString().Split('.')[1].Length > 3)
                {
                    L[i] = Math.Round(L[i], 3); ;
                }
            }
            Rounded_Sides = Rounded_Sides.Append(L[i]).ToArray();
        }

        return Rounded_Sides;
    }
    public static double[] Function_Round_Angles(double[] A)
    {
        double[] Rounded_Angles = { };
        for (int i = 0; i < 3; i++)
        {
            if (A[i].ToString().Contains('.'))
            {
                if (A[i].ToString().Split('.')[1].Length > 3)
                {
                    A[i] = Math.Round(A[i], 3); ;
                }
            }
            Rounded_Angles = Rounded_Angles.Append(A[i]).ToArray();
        }

        return Rounded_Angles;
    }
}